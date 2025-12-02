using Microsoft.AspNetCore.Identity;

namespace Pharmacy_API.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public DateTime? Data_zatrudnienia { get; set; }
        public string Zmiana { get; set; }
        public bool Admin { get; set; }
        public ICollection<Faktura>? Faktury { get; set; }
    }
}
