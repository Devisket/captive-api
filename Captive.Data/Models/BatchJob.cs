using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class BatchJob
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public BatchJobStatus Status { get; set; }
        public int Progress { get; set; }
        public string? CurrentStep { get; set; }
        public string? Warnings { get; set; }
        public string? ErrorMessage { get; set; }
        public bool ForceProcess { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BatchFile BatchFile { get; set; } = null!;
    }
}
