using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Batch.Commands.UpdateBatchFileStatus
{
    public class UpdateBatchFileStatusCommandHandler : IRequestHandler<UpdateBatchFileStatusCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUnitOfWork;
        private readonly IWriteUnitOfWork _writeUnitOfWork;

        public UpdateBatchFileStatusCommandHandler (IReadUnitOfWork readUnitOfWork, IWriteUnitOfWork writeUnitOfWork)
        {
            _readUnitOfWork = readUnitOfWork;
            _writeUnitOfWork = writeUnitOfWork;
        }

        public async Task<Unit> Handle(UpdateBatchFileStatusCommand request, CancellationToken cancellationToken)
        {
            var batch = await _readUnitOfWork.BatchFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.BatchId, cancellationToken);

            if (batch == null) 
            {
                throw new Exception($"Batch ID: {request.BatchId} doesn't exist");
            }

            batch.BatchFileStatus = (BatchFileStatus)Enum.Parse(typeof(BatchFileStatus), request.BatchFileStatus);
            batch.ErrorMessage = request.ErrorMessage;

            _writeUnitOfWork.BatchFiles.Update(batch);

            return Unit.Value;
        }
    }
}
