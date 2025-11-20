namespace Pharmacy_API.DTO
{
    public class ValidatePrescriptionDto
    {
        public int ID_Leku { get; set; }
        public bool HasPrescriptionDocument { get; set; } // czy klient posiada receptę (frontend przesyła true/false)
        public string PrescriptionNumber { get; set; } // opcjonalnie numer recepty
        public int? ID_Klienta { get; set; } // opcjonalnie
    }
}
