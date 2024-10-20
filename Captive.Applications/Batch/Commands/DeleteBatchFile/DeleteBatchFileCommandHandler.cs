using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Captive.Applications.Batch.Commands.DeleteBatchFile
{
    public class DeleteBatchFileCommandHandler : IRequestHandler<DeleteBatchFileCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public DeleteBatchFileCommandHandler(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(DeleteBatchFileCommand request, CancellationToken cancellationToken)
        {
            var batch = await _readUow.BatchFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);

            if (batch == null) {
                throw new Exception($"Batch ID:{request.Id} doesn't exist");
            }

            _writeUow.BatchFiles.Delete(batch);           

            return Unit.Value;
        }
    }
}
