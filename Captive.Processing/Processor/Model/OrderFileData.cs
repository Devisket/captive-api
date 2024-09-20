
namespace Captive.Processing.Processor.Model
{
    public class OrderFileData
    {
        public required string CheckType { get; set; }
        public required string BRSTN { get; set; }
        public required string AccountNumber { get; set; }
        public required string AccountName { get; set; }
        public string? Concode { get; set; }
        public required string FormType { get; set; }
        public required int Quantity { get; set; }
        public string? DeliverTo { get; set; }

        public string? StartingSeries { get; set; }
        public string? EndingSeries { get; set; }
    }
}
