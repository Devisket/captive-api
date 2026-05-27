using MediatR;

namespace Captive.Applications.Batch.Commands.CancelBatchProcess
{
    public class CancelBatchProcessCommand : IRequest
    {
        public Guid BatchId { get; set; }
    }
}
