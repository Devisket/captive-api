namespace Captive.Processing.Processor.Model
{
    public class CheckInventory
    {
        public char CheckType { get; set; }
        public string FormType { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string OriginBRSTN { get; set; }
        public string StartingSeries { get; set; }
        public string EndingSeries { get; set; }
        public string DeliveryBRSTN { get; set; }
        public int NoOfSheet { get; set; }  
    }
}
