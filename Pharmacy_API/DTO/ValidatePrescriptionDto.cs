namespace Pharmacy_API.DTO
{
    public class ValidatePrescriptionDto
    {
        public int ID_Leku { get; set; }
        public bool HasPrescriptionDocument { get; set; } 
        public string PrescriptionNumber { get; set; }
        public int? ID_Klienta { get; set; } 
    }
}
