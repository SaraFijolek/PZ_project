using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Data;
using Pharmacy_API.DTO;
using Pharmacy_API.Models;

namespace Pharmacy_API.Controllers
{
    public class LekiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public LekiController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> GetAll() => Ok(await _db.LEKI.ToListAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> Get(int id)
        {
            var lek = await _db.LEKI.FindAsync(id);
            if (lek == null) return NotFound();
            return Ok(lek);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Lek model)
        {
            _db.LEKI.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.ID }, model);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Lek model)
        {
            var lek = await _db.LEKI.FindAsync(id);
            if (lek == null) return NotFound();
          
            lek.Nazwa = model.Nazwa;
            lek.Refundacja = model.Refundacja;
            lek.Recepta = model.Recepta;
            lek.Substancja_czynna = model.Substancja_czynna;
            lek.Preparat = model.Preparat;
            lek.Cena = model.Cena;
            lek.Stan_w_magazynie = model.Stan_w_magazynie;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var lek = await _db.LEKI.FindAsync(id);
            if (lek == null) return NotFound();
            _db.LEKI.Remove(lek);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpGet("search")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var results = await _db.LEKI
                .Where(l => l.Nazwa.Contains(q) || l.Substancja_czynna.Contains(q))
                .ToListAsync();
            return Ok(results);
        }
        [HttpPost("validate-prescription")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> ValidatePrescription([FromBody] ValidatePrescriptionDto dto)
        {
            var lek = await _db.LEKI.FindAsync(dto.ID_Leku);
            if (lek == null) return NotFound(new { message = "Lek nie istnieje" });

            if (!lek.Recepta)
            {
                
                return Ok(new { ok = true, message = "Lek nie wymaga recepty" });
            }

           
            if (dto.HasPrescriptionDocument)
            {
                
                return Ok(new { ok = true, message = "Recepta dostarczona" });
            }
            else
            {
                return BadRequest(new { ok = false, message = "Lek wymaga recepty — brak dokumentu" });
            }
        }
    }
}
