using MediatR;

namespace Captive.Applications.Batch.Commands.ConfirmBatchProcess
{
    public class ConfirmBatchProcessCommand : IRequest<ConfirmBatchProcessCommandResponse>
    {
        public Guid BatchId { get; set; }
    }

    public class ConfirmBatchProcessCommandResponse
    {
        public Guid JobId { get; set; }
    }
}
