using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class OrderFile
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required OrderFilesStatus Status { get; set; }
        public DateTime ProcessDate { get; set; }
        public string? ErrorMessage { get; set; }

        
        public ICollection<CheckOrders>? CheckOrders { get; set; }
        public ICollection<OrderFileLog>? OrderFileLogs { get; set; }

        public Guid BatchFileId { get; set; }
        public BatchFile? BatchFile { get; set; }
        
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
