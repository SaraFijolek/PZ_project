using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pharmacy_API.Controllers;
using Pharmacy_API.Data;
using Pharmacy_API.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy_Test
{
    [TestClass]
    public class ReportsControllerTests
    {
        private Mock<ApplicationDbContext> _mockContext;
        private ReportsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            var lekiData = new List<Lek>
    {
        new Lek { ID = 1, Stan_w_magazynie = 3 },
        new Lek { ID = 2, Stan_w_magazynie = 7 },
        new Lek { ID = 3, Stan_w_magazynie = 2 }
    };

            var mockSet = new Mock<DbSet<Lek>>();
            mockSet.As<IQueryable<Lek>>().Setup(m => m.Provider).Returns(lekiData.AsQueryable().Provider);
            mockSet.As<IQueryable<Lek>>().Setup(m => m.Expression).Returns(lekiData.AsQueryable().Expression);
            mockSet.As<IQueryable<Lek>>().Setup(m => m.ElementType).Returns(lekiData.AsQueryable().ElementType);
            mockSet.As<IQueryable<Lek>>().Setup(m => m.GetEnumerator()).Returns(() => lekiData.AsQueryable().GetEnumerator());

            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.LEKI).Returns(mockSet.Object);

            _controller = new ReportsController(_mockContext.Object);
        }

        [TestMethod]
        public async Task LowStock_ReturnsOkResultWithExpectedData()
        {
            var result = await _controller.LowStock(5);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            dynamic data = okResult.Value;

            Assert.AreEqual(3, data.count);
        }
    }
}
