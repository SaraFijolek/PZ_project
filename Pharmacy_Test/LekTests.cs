using Moq;
using Pharmacy_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_Test
{
    [TestClass]
    public class LekTests
    {
        private Mock<Lek> mockLek;

        [TestInitialize]
        public void TestSetup()
        {
            mockLek = new Mock<Lek>();
        }

        [TestMethod]
        public void Lek_Properties_CorrectlySet()
        {
            mockLek.Object.ID = 1;
            mockLek.Object.Nazwa = "Medicine Name";
            mockLek.Object.Refundacja = true;
            mockLek.Object.Recepta = false;
            mockLek.Object.Substancja_czynna = "Active Substance";
            mockLek.Object.Preparat = "Preparation";
            mockLek.Object.Cena = 2.5M;
            mockLek.Object.Stan_w_magazynie = 100;

            Assert.AreEqual(1, mockLek.Object.ID);
            Assert.AreEqual("Medicine Name", mockLek.Object.Nazwa);
            Assert.IsTrue(mockLek.Object.Refundacja);
            Assert.IsFalse(mockLek.Object.Recepta);
            Assert.AreEqual("Active Substance", mockLek.Object.Substancja_czynna);
            Assert.AreEqual("Preparation", mockLek.Object.Preparat);
            Assert.AreEqual(2.5M, mockLek.Object.Cena);
            Assert.AreEqual(100, mockLek.Object.Stan_w_magazynie);
        }

        [TestMethod]
        public void Lek_Nazwa_MaxLength()
        {
            string longName = new string('a', 101);
            mockLek.Object.Nazwa = longName;
            var validationResults = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(mockLek.Object, new ValidationContext(mockLek.Object), validationResults, true));    
        }

        [TestMethod]
        public void Lek_Substancja_czynna_MaxLength()
        {
            string longActiveSubstance = new string('a', 101);
            mockLek.Object.Substancja_czynna = longActiveSubstance;
            var validationResults = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(mockLek.Object, new ValidationContext(mockLek.Object), validationResults, true));
        }

        [TestMethod]
        public void Lek_Preparat_MaxLength()
        {
            string longPreparation = new string('a', 101);
            mockLek.Object.Preparat = longPreparation;
            var validationResults = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(mockLek.Object, new ValidationContext(mockLek.Object), validationResults, true));
        }
    }
}