namespace Captive.Messaging.Models
{
    public class MdbExtractMessage
    {
        public Guid BankId { get; set; }
        public required string FileName {  get; set; }
    }
}
