namespace Pharmacy_API.DTO
{
    public class RegisterDto
    {
        public string Login { get; set; }
        public string Haslo { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public bool Admin { get; set; }
        public DateTime? Data_zatrudnienia { get; set; }
        public string Zmiana { get; set; }
    }
}
