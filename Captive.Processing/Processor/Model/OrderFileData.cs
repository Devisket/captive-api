
namespace Captive.Processing.Processor.Model
{
    public class OrderFileData
    {
        public required string CheckType { get; set; }
        public required string BRSTN { get; set; }
        public required string AccountNumber { get; set; }
        public required string AccountName { get; set; }
        public required string ConCode { get; set; }
        public required string FormType { get; set; }
        public required string Quantity { get; set; }
        public string? DeliverTo { get; set; }
    }
}
