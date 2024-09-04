

namespace Captive.Messaging.Models
{
    public class FileUploadMessage
    {
        public Guid BankId { get; set; }
        public Guid BatchID { get;set; }
        public string[] Files { get; set; }
    }
}
