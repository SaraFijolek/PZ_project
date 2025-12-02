namespace Pharmacy_API.DTO
{
    public class CreateFakturaDto
    {
        public int ID_Leku { get; set; }
        public int? ID_Klienta { get; set; }
        public int Ilosc { get; set; } = 1;
        public string Dane_klienta { get; set; }  
    }
}
