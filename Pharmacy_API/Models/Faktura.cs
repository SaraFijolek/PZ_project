using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_API.Models
{
    public class Faktura
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string Nr_faktury { get; set; }
        [MaxLength(100)]
        public string Nazwa_leku { get; set; }
        public string Dane_klienta { get; set; }
        public string Dane_pracownika { get; set; }
        public DateTime Data_wystawienia { get; set; }

        
        public int Ilosc { get; set; } = 1;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cena_sprzedazy { get; set; } 

     
        public int? ID_Leku { get; set; }
        [ForeignKey("ID_Leku")]
        public Lek Lek { get; set; }

        public int? ID_Klienta { get; set; }
        [ForeignKey("ID_Klienta")]
        public Klient Klient { get; set; }

        public int? ID_Pracownika { get; set; }
        [ForeignKey("ID_Pracownika")]
        public ApplicationUser Pracownik { get; set; }
    }
}

