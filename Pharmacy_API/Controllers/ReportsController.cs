using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Data;

namespace Pharmacy_API.Controllers
{
    [ApiController]
    [Route("reports")]
    [Authorize(Policy = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ReportsController(ApplicationDbContext db) { _db = db; }

        
        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] int threshold = 5)
        {
            var items = await _db.LEKI
                .Where(l => l.Stan_w_magazynie <= threshold)
                .Select(l => new
                {
                    l.ID,
                    l.Nazwa,
                    l.Stan_w_magazynie,
                    l.Cena,
                    l.Refundacja,
                    l.Recepta
                })
                .ToListAsync();

            return Ok(new { threshold, count = items.Count, items });
        }

        
        [HttpGet("sales")]
        public async Task<IActionResult> SalesReport([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            if (!start.HasValue) start = DateTime.UtcNow.AddMonths(-1); 
            if (!end.HasValue) end = DateTime.UtcNow;

            var query = _db.FAKTURA
                .Where(f => f.Data_wystawienia >= start && f.Data_wystawienia <= end)
                .GroupBy(f => new { f.ID_Leku, f.Nazwa_leku })
                .Select(g => new
                {
                    ID_Leku = g.Key.ID_Leku,
                    Nazwa_leku = g.Key.Nazwa_leku,
                    Ilosc_sprzedana = g.Sum(x => x.Ilosc),
                    Przychod = g.Sum(x => x.Ilosc * x.Cena_sprzedazy)
                })
                .OrderByDescending(x => x.Przychod);

            var results = await query.ToListAsync();

            var totalItems = results.Sum(r => r.Ilosc_sprzedana);
            var totalRevenue = results.Sum(r => r.Przychod);

            return Ok(new
            {
                periodFrom = start,
                periodTo = end,
                totalItems,
                totalRevenue,
                breakdown = results
            });
        }
    }
}
