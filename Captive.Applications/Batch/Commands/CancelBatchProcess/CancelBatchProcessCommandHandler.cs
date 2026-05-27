using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.CancelBatchProcess
{
    public class CancelBatchProcessCommandHandler : IRequestHandler<CancelBatchProcessCommand>
    {
        private readonly IWriteUnitOfWork _writeUow;

        public CancelBatchProcessCommandHandler(IWriteUnitOfWork writeUow)
        {
            _writeUow = writeUow;
        }

        public async Task Handle(CancelBatchProcessCommand request, CancellationToken cancellationToken)
        {
            var job = await _writeUow.BatchJobs.GetAll()
                .Where(x => x.BatchId == request.BatchId && x.Status == BatchJobStatus.AwaitingConfirmation)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (job == null) return;

            job.Status = BatchJobStatus.Failed;
            job.ErrorMessage = "Processing cancelled by user.";
            job.UpdatedAt = DateTime.UtcNow;
            await _writeUow.Complete(cancellationToken);
        }
    }
}
