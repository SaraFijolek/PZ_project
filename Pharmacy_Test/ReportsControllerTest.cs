using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pharmacy_API.Controllers;
using Pharmacy_API.Data;
using Pharmacy_API.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pharmacy_Test
{
    [TestClass]
    public class ReportsControllerTests
    {
        private ReportsController _controller;

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            ApplicationDbContext context = GetInMemoryDbContext();
            var lekiData = new List<Lek>
            {
                new Lek { ID = 1, Nazwa = "Medicine1", Stan_w_magazynie = 3, Preparat = "abc", Substancja_czynna = "abc" },
                new Lek { ID = 2, Nazwa = "Medicine2", Stan_w_magazynie = 7, Preparat = "abc", Substancja_czynna = "abc" },
                new Lek { ID = 3, Nazwa = "Medicine3", Stan_w_magazynie = 2, Preparat = "abc", Substancja_czynna = "abc" }
            };

            for (int i = 0; i < lekiData.Count; i++)
            {
                await context.LEKI.AddAsync(lekiData[i]);
            }
            await context.SaveChangesAsync();

            var fakturyData = new List<Faktura>
            {
                new Faktura { Ilosc = 5, Cena_sprzedazy = (decimal)12.50, ID_Leku = 1, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "abc", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 7, Cena_sprzedazy = (decimal)17.50, ID_Leku = 1, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "abc", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 1, Cena_sprzedazy = (decimal)15.50, ID_Leku = 2, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "def", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 2, Cena_sprzedazy = (decimal)18.50, ID_Leku = 2, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "def", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 9, Cena_sprzedazy = (decimal)22.33, ID_Leku = 3, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "123", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 8, Cena_sprzedazy = (decimal)30.00, ID_Leku = 4, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "321", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2025-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) },
                new Faktura { Ilosc = 8, Cena_sprzedazy = (decimal)30.00, ID_Leku = 4, Dane_klienta = "A", Dane_pracownika = "B", Nazwa_leku = "321", Nr_faktury = "1", Data_wystawienia = DateTime.ParseExact("2027-12-20 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) }

            };

            for (int i = 0; i < fakturyData.Count; i++) 
            {
                await context.FAKTURA.AddAsync(fakturyData[i]);
            }
            await context.SaveChangesAsync();

            _controller = new ReportsController(context);
        }

        [TestMethod]
        public async Task LowStockTest()
        {
            int threshold = 5;
            IActionResult result = await _controller.LowStock(threshold);

            dynamic value = ((OkObjectResult)result).Value;
            var itemsProp = value.GetType().GetProperty("items");
            var lekiObject = itemsProp.GetValue(value);
            string json = JsonSerializer.Serialize(lekiObject);
            List<Lek> leki = JsonSerializer.Deserialize<List<Lek>>(json);

            List<Lek> lekiThreshold = leki.Where(lek => lek.Stan_w_magazynie <= threshold).ToList();
            Assert.AreEqual(lekiThreshold.Count, 2);
        }

        [TestMethod]
        public async Task SalesReportTest()
        {
            DateTime startDate = DateTime.ParseExact("2025-12-01 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact("2026-01-01 12:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            IActionResult result = await _controller.SalesReport(startDate, endDate);

            dynamic value = ((OkObjectResult)result).Value;
            var totalItemsProp = value.GetType().GetProperty("totalItems");
            var totalItemsObject = totalItemsProp.GetValue(value);
            string json = JsonSerializer.Serialize(totalItemsObject);
            int totalItems = JsonSerializer.Deserialize<int>(json);

            var totalRevenueProp = value.GetType().GetProperty("totalRevenue");
            var totalRevenueObject = totalRevenueProp.GetValue(value);
            json = JsonSerializer.Serialize(totalRevenueObject);
            double totalRevenue = JsonSerializer.Deserialize<double>(json);

            Assert.AreEqual(totalItems, 32);
            Assert.AreEqual(totalRevenue, 678.47);
        }
    }
}
