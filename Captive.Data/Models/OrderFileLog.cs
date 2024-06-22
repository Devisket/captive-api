using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class OrderFileLog
    {
        public int Id { get; set; }
        public required string LogMessage { get; set; }

        public LogType LogType { get;set; }

        public DateTime LogDate { get; set; }

        public Guid OrderFileId { get; set; }
        public OrderFile? OrderFile { get; set; }
    }
}
