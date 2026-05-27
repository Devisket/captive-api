using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.ConfirmBatchProcess
{
    public class ConfirmBatchProcessCommandHandler : IRequestHandler<ConfirmBatchProcessCommand, ConfirmBatchProcessCommandResponse>
    {
        private readonly IWriteUnitOfWork _writeUow;

        public ConfirmBatchProcessCommandHandler(IWriteUnitOfWork writeUow)
        {
            _writeUow = writeUow;
        }

        public async Task<ConfirmBatchProcessCommandResponse> Handle(ConfirmBatchProcessCommand request, CancellationToken cancellationToken)
        {
            var job = await _writeUow.BatchJobs.GetAll()
                .Where(x => x.BatchId == request.BatchId && x.Status == BatchJobStatus.AwaitingConfirmation)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (job == null)
                throw new InvalidOperationException($"No pending confirmation found for batch {request.BatchId}.");

            job.ForceProcess = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _writeUow.Complete(cancellationToken);

            return new ConfirmBatchProcessCommandResponse { JobId = job.Id };
        }
    }
}
