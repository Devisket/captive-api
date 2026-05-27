using MediatR;

namespace Captive.Applications.Batch.Query.GetBatchJobStatus
{
    public class GetBatchJobStatusQuery : IRequest<GetBatchJobStatusQueryResponse>
    {
        public Guid BatchId { get; set; }
    }

    public class GetBatchJobStatusQueryResponse
    {
        public Guid? JobId { get; set; }
        public string Status { get; set; } = "None";
        public int Progress { get; set; }
        public string? CurrentStep { get; set; }
        public List<string> Warnings { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
