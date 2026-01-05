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

            Klient klient = null;
            if (dto.ID_Klienta.HasValue)
            {
                klient = await _db.KLIENT.FindAsync(dto.ID_Klienta.Value);
                if (klient == null)
                    return BadRequest(new { message = "Klient o podanym ID nie istnieje" });
            }

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
                Lek = lek,
                ID_Klienta = klient?.ID,
                Klient = klient,
                ID_Pracownika = pracownik.Id,
                Pracownik = pracownik,
                Ilosc = dto.Ilosc,
                Cena_sprzedazy = lek.Cena
            };

            _db.FAKTURA.Add(faktura);
            await _db.SaveChangesAsync();

            // zwracamy DTO zamiast encji
            var result = new
            {
                faktura.ID,
                faktura.Nr_faktury,
                faktura.Nazwa_leku,
                Klient = klient != null ? new { klient.ID, klient.Imie, klient.Nazwisko } : null,
                faktura.Dane_klienta,
                faktura.Dane_pracownika,
                faktura.Data_wystawienia,
                faktura.Ilosc,
                faktura.Cena_sprzedazy
            };

            return CreatedAtAction(nameof(GetById), new { id = faktura.ID }, result);
        }

        [HttpGet]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.FAKTURA
                .Include(f => f.Lek)
                .Include(f => f.Klient)
                .Select(f => new
                {
                    f.ID,
                    f.Nr_faktury,
                    Lek = new { f.Lek.ID, f.Lek.Nazwa },
                    Klient = f.Klient != null ? new { f.Klient.ID, f.Klient.Imie, f.Klient.Nazwisko } : null,
                    f.Dane_pracownika,
                    f.Data_wystawienia,
                    f.Ilosc,
                    f.Cena_sprzedazy
                })
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
                .Where(f => f.ID == id)
                .Select(f => new
                {
                    f.ID,
                    f.Nr_faktury,
                    Lek = new { f.Lek.ID, f.Lek.Nazwa },
                    Klient = f.Klient != null ? new { f.Klient.ID, f.Klient.Imie, f.Klient.Nazwisko } : null,
                    f.Dane_pracownika,
                    f.Data_wystawienia,
                    f.Ilosc,
                    f.Cena_sprzedazy
                })
                .FirstOrDefaultAsync();

            if (faktura == null) return NotFound();
            return Ok(faktura);
        }

        [HttpGet("my-history")]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int? last = 50)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var history = await _db.FAKTURA
                .Where(f => f.ID_Pracownika == userId)
                .Include(f => f.Lek)
                .Include(f => f.Klient)
                .OrderByDescending(f => f.Data_wystawienia)
                .Take(last ?? 50)
                .Select(f => new
                {
                    f.ID,
                    f.Nr_faktury,
                    Lek = new { f.Lek.ID, f.Lek.Nazwa },
                    Klient = f.Klient != null ? new { f.Klient.ID, f.Klient.Imie, f.Klient.Nazwisko } : null,
                    f.Dane_pracownika,
                    f.Data_wystawienia,
                    f.Ilosc,
                    f.Cena_sprzedazy
                })
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("client/{clientId}/history")]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetClientHistory(int clientId)
        {
            var klient = await _db.KLIENT.FindAsync(clientId);
            if (klient == null)
                return NotFound("Klient nie istnieje");

            var history = await _db.FAKTURA
                .Where(f => f.ID_Klienta == clientId)
                .Include(f => f.Lek)
                .OrderByDescending(f => f.Data_wystawienia)
                .Select(f => new
                {
                    f.ID,
                    f.Nr_faktury,
                    Lek = new { f.Lek.ID, f.Lek.Nazwa },
                    f.Dane_pracownika,
                    f.Data_wystawienia,
                    f.Ilosc,
                    f.Cena_sprzedazy
                })
                .ToListAsync();

            return Ok(new
            {
                Klient = new { klient.ID, klient.Imie, klient.Nazwisko },
                Historia = history
            });
        }

        [HttpGet("my-stats")]
        [Authorize(Policy = "Worker")]
        public async Task<IActionResult> GetMyStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var query = _db.FAKTURA.Where(f => f.ID_Pracownika == userId);

            if (from.HasValue)
                query = query.Where(f => f.Data_wystawienia >= from.Value);
            if (to.HasValue)
                query = query.Where(f => f.Data_wystawienia <= to.Value);

            var faktury = await query.ToListAsync();

            var stats = new
            {
                LiczbaTransakcji = faktury.Count,
                LacznaWartoscSprzedazy = faktury.Sum(f => f.Ilosc * f.Cena_sprzedazy),
                LacznaIloscProduktow = faktury.Sum(f => f.Ilosc),
                PeriodOd = from,
                PeriodDo = to
            };

            return Ok(stats);
        }
    }
}
