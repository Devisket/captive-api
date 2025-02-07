using MediatR;

namespace Captive.Applications.Batch.Commands.ProcessBatch
{
    public class ProcessBatchCommand : IRequest<Unit>
    {
        public Guid BatchId { get; set; }
    }
}
