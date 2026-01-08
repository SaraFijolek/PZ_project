//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Pharmacy_API.Controllers;
//using Pharmacy_API.Data;
//using Pharmacy_API.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pharmacy_Test
//{
//    internal class ReportsControllerTests
//    {
//        private ApplicationDbContext _context;

//        [TestInitialize]
//        public void Setup()
//        {
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;

//            _context = new ApplicationDbContext(options);
//            _context.LEKI.AddRange((IEnumerable<Lek>)GetSampleLeki());
//            _context.FAKTURA.AddRange((IEnumerable<Faktura>)GetSampleFaktura());
//            _context.SaveChanges();
//        }

//        [TestMethod]
//        public async Task LowStock_ReturnsOkResult()
//        {
//            var controller = new ReportsController(_context);
//            var result = await controller.LowStock(threshold: 10);
//            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//        }

//        [TestMethod]
//        public async Task SalesReport_ReturnsOkResult()
//        {
//            var controller = new ReportsController(_context);
//            DateTime start = new DateTime(2021, 1, 1);
//            DateTime end = new DateTime(2021, 12, 31);
//            var result = await controller.SalesReport(start, end);
//            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//        }

//        private IEnumerable<LEKI> GetSampleLeki()
//        {
//            return new List<LEKI>
//        {
//            new LEKI { ID = 1, Nazwa = "Medicine 1", Stan_w_magazynie = 3 },
//        };
//        }

//        private IEnumerable<FAKTURA> GetSampleFaktura()
//        {
//            return new List<FAKTURA>
//        {
//            new FAKTURA { ID_Leku = 1, Nazwa_leku = "Medicine 1", Ilosc = 5, Cena_sprzedazy = 2.0M, Data_wystawienia = DateTime.UtcNow },
//        };
//        }
//    }
//}
