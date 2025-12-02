using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;
using System.Security.Claims;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Worker")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
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

        
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

           
            if (!string.IsNullOrEmpty(dto.Imie))
                user.Imie = dto.Imie;

            if (!string.IsNullOrEmpty(dto.Nazwisko))
                user.Nazwisko = dto.Nazwisko;

            if (!string.IsNullOrEmpty(dto.NumerTelefonu))
                user.PhoneNumber = dto.NumerTelefonu;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Profil zaktualizowany pomyślnie" });
        }

        
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.StareHaslo,
                dto.NoweHaslo);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Hasło zmienione pomyślnie" });
        }
    }
}

