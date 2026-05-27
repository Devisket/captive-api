using MediatR;

namespace Captive.Applications.Batch.Commands.ProcessBatch
{
    public class ProcessBatchCommand : IRequest<ProcessBatchCommandResponse>
    {
        public Guid BatchId { get; set; }
    }

    public class ProcessBatchCommandResponse
    {
        public Guid JobId { get; set; }
    }
}
