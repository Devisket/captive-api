namespace Captive.Processing.Processor.Model
{
    public class CheckOrder
    {
        public string AccountNo { get; set; }
        public string BRSTN { get; set; }
        public char CheckType { get; set; }
        public string AccountName { get; set; }
        public int Concode { get; set; }
        public string FormType { get; set; }
        public int Quantity { get; set; }
        public string? DeliverTo { get; set; }
    }
}
