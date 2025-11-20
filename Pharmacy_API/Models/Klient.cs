using System.ComponentModel.DataAnnotations;

namespace Pharmacy_API.Models
{
    public class Klient
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(100)]
        public string Nazwisko { get; set; }
        [MaxLength(100)]
        public string Imie { get; set; }
    }
}
