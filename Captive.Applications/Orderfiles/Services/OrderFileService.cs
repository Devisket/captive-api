using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Orderfiles.Services
{
    public interface IOrderFileService
    {
        Task UpdateOrderFileStatus(Guid orderFileID, string errorMessage, CancellationToken cancellationToken);
        Task UpdateOrderFileStatus(Guid orderFileID, OrderFilesStatus orderFileStatus, CancellationToken cancellationToken);
    }

    public class OrderFileService : IOrderFileService
    {
        public IWriteUnitOfWork _writeUow;
        public IReadUnitOfWork _readUow;

        public OrderFileService(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow)
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }
        public async Task UpdateOrderFileStatus(Guid orderFileID, string errorMessage, CancellationToken cancellationToken)
        {
            await ChangeOrderFileStatus(orderFileID, errorMessage, OrderFilesStatus.Error, cancellationToken);
        }

        public async Task UpdateOrderFileStatus(Guid orderFileID, OrderFilesStatus orderFileStatus, CancellationToken cancellationToken)
        {
            await ChangeOrderFileStatus(orderFileID, string.Empty, orderFileStatus, cancellationToken);
        }

        private async Task ChangeOrderFileStatus(Guid orderFileId, string errorMessage, OrderFilesStatus status, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == orderFileId);

            if (orderFile == null) {
                throw new SystemException($"Order file ID: {orderFileId} doesn't exist");
            }

            orderFile.Status = status;

            if(!string.IsNullOrEmpty(errorMessage))
                orderFile.ErrorMessage = errorMessage;

            _writeUow.OrderFiles.Update(orderFile);

        }
    }
}
