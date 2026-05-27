using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using MediatR;

namespace Captive.Applications.Batch.Commands.ConfirmBatchProcess
{
    public class ConfirmBatchProcessCommandHandler : IRequestHandler<ConfirmBatchProcessCommand, ConfirmBatchProcessCommandResponse>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IProducer<BatchProcessMessage> _producer;

        public ConfirmBatchProcessCommandHandler(IWriteUnitOfWork writeUow, IProducer<BatchProcessMessage> producer)
        {
            _writeUow = writeUow;
            _producer = producer;
        }

        public async Task<ConfirmBatchProcessCommandResponse> Handle(ConfirmBatchProcessCommand request, CancellationToken cancellationToken)
        {
            var job = new BatchJob
            {
                Id = Guid.NewGuid(),
                BatchId = request.BatchId,
                Status = BatchJobStatus.Pending,
                Progress = 0,
                ForceProcess = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _writeUow.BatchJobs.AddAsync(job, cancellationToken);
            await _writeUow.Complete(cancellationToken);

            _producer.ProduceMessage(new BatchProcessMessage
            {
                JobId = job.Id,
                BatchId = request.BatchId,
                ForceProcess = true,
            });

            return new ConfirmBatchProcessCommandResponse { JobId = job.Id };
        }
    }
}
