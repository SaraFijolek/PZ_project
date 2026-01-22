using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy_API.Controllers;
using Pharmacy_API.Data;
using Pharmacy_API.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_Test
{
    [TestClass]
    public class LekiControllerTest
    {
        private LekiController _controller;

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

            _controller = new LekiController(context);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            IActionResult result = await _controller.GetAll();
            List<Lek> leki = ((result as OkObjectResult).Value) as List<Lek>;
            Assert.AreEqual(leki.Count, 3);
        }

        [TestMethod]
        public async Task GetTest()
        {
            int ID = 1;
            int falseID = 999;

            IActionResult result = await _controller.Get(ID);
            Lek lek = ((result as OkObjectResult).Value) as Lek;
            Assert.IsNotNull(lek);
            Assert.AreEqual(lek.Nazwa, "Medicine1");

            result = await _controller.Get(falseID);
            Assert.IsNull(result as OkObjectResult);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            Lek lek = new Lek { ID = 4, Nazwa = "Medicine4", Stan_w_magazynie = 3, Preparat = "abc", Substancja_czynna = "abc" };
            IActionResult result = await _controller.GetAll();
            List<Lek> leki = ((result as OkObjectResult).Value) as List<Lek>;
            int previewsCount = leki.Count();
            await _controller.Create(lek);
            result = await _controller.GetAll();
            leki = ((result as OkObjectResult).Value) as List<Lek>;
            int afterCount = leki.Count();
            Assert.AreEqual(previewsCount + 1, afterCount);
        }
    }
}
