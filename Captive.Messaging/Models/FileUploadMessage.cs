using Microsoft.AspNetCore.Http;

namespace Captive.Messaging.Models
{
    public class FileUploadMessage
    {
        public required Guid BankId { get; set; }
        public required Guid BatchID { get;set; }
        public required IEnumerable<string> Files { get; set; }
    }
}
