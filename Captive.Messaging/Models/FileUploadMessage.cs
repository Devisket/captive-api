using Captive.Model.Dto;

namespace Captive.Messaging.Models
{
    public class FileUploadMessage
    {
        public required Guid BankId { get; set; }
        public required Guid BatchID { get;set; }
        public required string BatchDirectory { get; set; }
        public required IEnumerable<OrderfileDto> Files { get; set; }
    }
}
