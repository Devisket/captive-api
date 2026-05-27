using Captive.Applications.Orderfiles.Hubs;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<OrderFileHub> _hubContext;

        public OrderFileService(IWriteUnitOfWork writeUow, IReadUnitOfWork readUow, IHubContext<OrderFileHub> hubContext)
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _hubContext = hubContext;
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

            if (orderFile == null)
                throw new SystemException($"Order file ID: {orderFileId} doesn't exist");

            orderFile.Status = status;

            if (!string.IsNullOrEmpty(errorMessage))
                orderFile.ErrorMessage = errorMessage;

            _writeUow.OrderFiles.Update(orderFile);

            // Broadcast status change in real-time via SignalR (before DB commit is fine — the commit follows immediately via pipeline)
            var dto = new OrderfileDto
            {
                Id = orderFile.Id,
                BatchId = orderFile.BatchFileId,
                ProductId = orderFile.ProductId,
                FileName = orderFile.FileName,
                FilePath = orderFile.FilePath,
                FileType = Path.GetExtension(orderFile.FileName).TrimStart('.'),
                Status = status.ToString(),
                IsValidated = orderFile.IsValidated,
                PersonalQuantity = orderFile.PersonalQuantity,
                CommercialQuantity = orderFile.CommercialQuantity,
                ErrorMessage = string.IsNullOrEmpty(errorMessage) ? orderFile.ErrorMessage : errorMessage,
                StatusDetail = null,
            };

            await _hubContext.Clients
                .Group($"batch-{orderFile.BatchFileId}")
                .SendAsync("orderFileStatusUpdate", dto, cancellationToken);
        }
    }
}
