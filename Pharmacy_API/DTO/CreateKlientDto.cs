namespace Pharmacy_API.DTO
{
    public class CreateKlientDto
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string? PESEL { get; set; }
        public DateTime? Data_urodzenia { get; set; }
        public string? Adres { get; set; }
        public string? Nr_telefonu { get; set; }
    }
}
