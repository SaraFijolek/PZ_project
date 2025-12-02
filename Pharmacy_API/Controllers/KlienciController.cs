using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Data;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Worker")]
    public class KlienciController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public KlienciController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var query = _db.KLIENT.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k =>
                    k.Imie.Contains(search) ||
                    k.Nazwisko.Contains(search) ||
                    k.PESEL.Contains(search));
            }

            var klienci = await query.ToListAsync();
            return Ok(klienci);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var klient = await _db.KLIENT.FindAsync(id);
            if (klient == null)
                return NotFound();

            return Ok(klient);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKlientDto dto)
        {
            if (string.IsNullOrEmpty(dto.Imie) || string.IsNullOrEmpty(dto.Nazwisko))
                return BadRequest("Imię i nazwisko są wymagane");

            
            if (!string.IsNullOrEmpty(dto.PESEL))
            {
                var exists = await _db.KLIENT.AnyAsync(k => k.PESEL == dto.PESEL);
                if (exists)
                    return BadRequest("Klient z tym numerem PESEL już istnieje");
            }

            var klient = new Klient
            {
                Imie = dto.Imie,
                Nazwisko = dto.Nazwisko,
                PESEL = dto.PESEL,
                Data_urodzenia = dto.Data_urodzenia,
                Adres = dto.Adres,
                Nr_telefonu = dto.Nr_telefonu
            };

            _db.KLIENT.Add(klient);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = klient.ID }, klient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateKlientDto dto)
        {
            var klient = await _db.KLIENT.FindAsync(id);
            if (klient == null)
                return NotFound();

            klient.Imie = dto.Imie ?? klient.Imie;
            klient.Nazwisko = dto.Nazwisko ?? klient.Nazwisko;
            klient.Adres = dto.Adres ?? klient.Adres;
            klient.Nr_telefonu = dto.Nr_telefonu ?? klient.Nr_telefonu;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var klient = await _db.KLIENT.FindAsync(id);
            if (klient == null)
                return NotFound();

            _db.KLIENT.Remove(klient);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

