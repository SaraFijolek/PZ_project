using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_API.Data;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FakturyController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public FakturyController(ApplicationDbContext db) { _db = db; }

        
        [HttpPost]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> CreateFaktura([FromBody] CreateFakturaDto dto)
        {
            var lek = await _db.LEKI.FindAsync(dto.ID_Leku);
            if (lek == null) return BadRequest(new { message = "Lek nie istnieje" });
            if (dto.Ilosc <= 0) return BadRequest(new { message = "Ilość musi być > 0" });
            if (lek.Stan_w_magazynie < dto.Ilosc) return BadRequest(new { message = "Niewystarczający stan magazynowy" });

            var klient = dto.ID_Klienta.HasValue ? await _db.KLIENT.FindAsync(dto.ID_Klienta.Value) : null;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = int.Parse(userIdClaim);
            var pracownik = await _db.Users.FindAsync(userId);
            if (pracownik == null) return Unauthorized();

            
            lek.Stan_w_magazynie -= dto.Ilosc;

           
            var nr = $"F-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            var faktura = new Faktura
            {
                Nr_faktury = nr,
                Nazwa_leku = lek.Nazwa,
                Dane_klienta = klient != null ? $"{klient.Imie} {klient.Nazwisko}" : dto.Dane_klienta,
                Dane_pracownika = $"{pracownik.Imie} {pracownik.Nazwisko}",
                Data_wystawienia = DateTime.UtcNow,
                ID_Leku = lek.ID,
                ID_Klienta = klient?.ID,
                ID_Pracownika = pracownik.Id,
                Ilosc = dto.Ilosc,
                Cena_sprzedazy = lek.Cena
            };

            _db.FAKTURA.Add(faktura);
            await _db.SaveChangesAsync();

           
            return CreatedAtAction(nameof(GetById), new { id = faktura.ID }, faktura);
        }

       
        [HttpGet]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.FAKTURA
                .Include(f => f.Lek)
                .Include(f => f.Klient)
                .ToListAsync();
            return Ok(list);
        }

        
        [HttpGet("{id}")]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetById(int id)
        {
            var faktura = await _db.FAKTURA
                .Include(f => f.Lek)
                .Include(f => f.Klient)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (faktura == null) return NotFound();
            return Ok(faktura);
        }
    }
}
