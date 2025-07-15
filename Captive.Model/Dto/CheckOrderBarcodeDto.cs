namespace Captive.Model.Dto
{
    public class CheckOrderBarcodeDto
    {
        public Guid CheckOrderId { get; set; }
        public string AccountNumber {  get; set; }
        public string BRSTN { get; set; }
        public List<string> StartingSeries {  get; set; }
    }
}
