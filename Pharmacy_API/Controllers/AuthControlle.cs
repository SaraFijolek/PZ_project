using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;
using Schematics.API.Service.Infrastructure;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtTokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IJwtTokenService tokenService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest("Dane rejestracji są wymagane");
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Haslo))
                return BadRequest("Email i hasło są wymagane");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Imie = dto.Imie,
                Nazwisko = dto.Nazwisko,
                Data_zatrudnienia = dto.Data_zatrudnienia,
                Zmiana = dto.Zmiana ?? "Dzienna",
                Admin = false
            };

            var result = await _userManager.CreateAsync(user, dto.Haslo);
            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync("Worker"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Worker"));

            await _userManager.AddToRoleAsync(user, "Worker");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Haslo))
                return BadRequest("Email i hasło są wymagane");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Nieprawidłowy email lub hasło");

            if (!await _userManager.CheckPasswordAsync(user, dto.Haslo))
                return Unauthorized("Nieprawidłowy email lub hasło");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest("Email jest wymagany");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                
                return Ok(new { message = "Jeśli adres email istnieje, link resetujący został wysłany" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            
            return Ok(new
            {
                message = "Token resetujący wygenerowany",
                token, 
                userId = user.Id
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.NoweHaslo))
                return BadRequest("Email, token i nowe hasło są wymagane");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Nieprawidłowy request");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NoweHaslo);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Hasło zostało zresetowane pomyślnie" });
        }
    }
}

