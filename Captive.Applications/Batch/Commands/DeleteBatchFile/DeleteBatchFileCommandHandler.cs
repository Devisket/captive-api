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
            var batch = await _readUow.BatchFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (batch == null)
                throw new Exception($"Batch ID:{request.Id} doesn't exist");

            var affectedInventoryIds = await _readUow.CheckInventoryDetails.GetAll()
                .Where(x => x.CheckOrderId != null && x.CheckOrder!.OrderFile.BatchFileId == request.Id)
                .Select(x => x.CheckInventoryId)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var inventoryId in affectedInventoryIds)
            {
                var inventory = await _writeUow.CheckInventory.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == inventoryId, cancellationToken);

                if (inventory == null) continue;

                var remainingMax = await _readUow.CheckInventoryDetails.GetAll()
                    .Where(x => x.CheckInventoryId == inventoryId &&
                                (x.CheckOrderId == null || x.CheckOrder!.OrderFile.BatchFileId != request.Id))
                    .OrderByDescending(x => x.EndingNumber)
                    .Select(x => (long?)x.EndingNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                inventory.CurrentSeries = remainingMax ?? inventory.StartingSeries;
                _writeUow.CheckInventory.Update(inventory);
            }

            _writeUow.BatchFiles.Delete(batch);

            return Unit.Value;
        }
    }
}
