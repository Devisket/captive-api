namespace Captive.Model.Dto
{
    public class CheckOrderBarcodeDto
    {
        public Guid CheckOrderId { get; set; }
        public string AccountNumber {  get; set; }
        public string BRSTN { get; set; }
        public int Quantity {  get; set; }
        public List<CheckInventoryDetailBarcodeDto> CheckInventories{ get; set; }
    }

    public class CheckInventoryDetailBarcodeDto
    {
        public Guid CheckInventoryDetailId { get; set; }
        public string StartingSeries {  get; set; }
        public string EndingSeries{ get; set; }
    }
}
