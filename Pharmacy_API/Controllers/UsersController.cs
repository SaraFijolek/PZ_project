using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Data;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles  = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _db;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Haslo))
                return BadRequest("Email i hasło są wymagane");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Imie = dto.Imie,
                Nazwisko = dto.Nazwisko,
                Data_zatrudnienia = dto.Data_zatrudnienia ?? DateTime.UtcNow,
                Zmiana = dto.Zmiana ?? "Dzienna",
                Admin = dto.Admin
            };

            var result = await _userManager.CreateAsync(user, dto.Haslo);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            
            var roleName = dto.Admin ? "Admin" : "Worker";
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole<int>(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            return CreatedAtAction(nameof(GetEmployee), new { id = user.Id },
                new { user.Id, user.Email, user.Imie, user.Nazwisko, user.Admin });
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Imie,
                    u.Nazwisko,
                    u.Data_zatrudnienia,
                    u.Zmiana,
                    u.Admin
                })
                .ToListAsync();

            return Ok(users);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Imie,
                user.Nazwisko,
                user.Data_zatrudnienia,
                user.Zmiana,
                user.Admin,
                Roles = roles
            });
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            user.Imie = dto.Imie ?? user.Imie;
            user.Nazwisko = dto.Nazwisko ?? user.Nazwisko;
            user.Zmiana = dto.Zmiana ?? user.Zmiana;
            user.Admin = dto.Admin ?? user.Admin;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        
        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("Użytkownik nie istnieje");

            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest("Rola nie istnieje");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.RoleName);

            
            user.Admin = dto.RoleName == "Admin";
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Rola przypisana pomyślnie" });
        }
    }
}