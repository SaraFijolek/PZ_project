namespace Pharmacy_API.DTO
{
    public class UpdateEmployeeDto
    {
        public string? Imie { get; set; }
        public string? Nazwisko { get; set; }
        public string? Zmiana { get; set; }
        public bool? Admin { get; set; }
    }
}
