namespace Captive.Messaging.Models
{
    public class BatchProcessMessage
    {
        public Guid JobId { get; set; }
        public Guid BatchId { get; set; }
        public bool ForceProcess { get; set; }
    }
}
