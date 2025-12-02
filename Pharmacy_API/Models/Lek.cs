using System.ComponentModel.DataAnnotations;

namespace Pharmacy_API.Models
{
    public class Lek
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(100)]
        public string Nazwa { get; set; }
        public bool Refundacja { get; set; }
        public bool Recepta { get; set; }
        [MaxLength(100)]
        public string Substancja_czynna { get; set; }
        [MaxLength(100)]
        public string Preparat { get; set; }
        public decimal Cena { get; set; }
        public int Stan_w_magazynie { get; set; }
    }
}
