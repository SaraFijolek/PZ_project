namespace Pharmacy_API.DTO
{
    public class CreateEmployeeDto
    {
        public string Email { get; set; }
        public string Haslo { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public DateTime? Data_zatrudnienia { get; set; }
        public string? Zmiana { get; set; }
        public bool Admin { get; set; }
    }
}
