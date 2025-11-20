using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;
using Schematics.API.Service.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pharmacy_API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtTokenService _tokenService; 

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config,
                              RoleManager<IdentityRole<int>> roleManager,
                              IJwtTokenService tokenService) 
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _tokenService = tokenService; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Login,
                Imie = dto.Imie,
                Nazwisko = dto.Nazwisko,
                Admin = dto.Admin,
                Data_zatrudnienia = dto.Data_zatrudnienia,
                Zmiana = dto.Zmiana
            };

            var result = await _userManager.CreateAsync(user, dto.Haslo);
            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            if (!await _roleManager.RoleExistsAsync("Worker"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Worker"));

            var roleToAdd = dto.Admin ? "Admin" : "Worker";
            await _userManager.AddToRoleAsync(user, roleToAdd);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Login);
            if (user == null) return Unauthorized();

            if (!await _userManager.CheckPasswordAsync(user, dto.Haslo)) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new { token });
        }
    }


}
