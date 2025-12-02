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
        [MaxLength(11)]
        public string? PESEL { get; set; }

        public DateTime? Data_urodzenia { get; set; }

        [MaxLength(200)]
        public string? Adres { get; set; }

        [MaxLength(20)]
        public string? Nr_telefonu { get; set; }

        // Relacja z fakturami
        public ICollection<Faktura>? Faktury { get; set; }
    }
}
