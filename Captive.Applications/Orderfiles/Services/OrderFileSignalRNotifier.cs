using Captive.Applications.Orderfiles.Hubs;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Model.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Orderfiles.Services
{
    public class OrderFileSignalRNotifier : IOrderFileNotifier
    {
        private readonly IHubContext<OrderFileHub> _hubContext;
        private readonly IReadUnitOfWork _readUow;

        public OrderFileSignalRNotifier(IHubContext<OrderFileHub> hubContext, IReadUnitOfWork readUow)
        {
            _hubContext = hubContext;
            _readUow = readUow;
        }

        public async Task NotifyBatchProgress(Guid batchId, string statusDetail, CancellationToken cancellationToken = default)
        {
            var orderFiles = await _readUow.OrderFiles
                .GetAll()
                .Where(x => x.BatchFileId == batchId &&
                    (x.Status == OrderFilesStatus.Processing || x.Status == OrderFilesStatus.GeneratingReport))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var orderFile in orderFiles)
                await BroadcastProgress(batchId, orderFile.Id, orderFile, statusDetail, cancellationToken);
        }

        public async Task NotifyOrderFileProgress(Guid batchId, Guid orderFileId, string statusDetail, CancellationToken cancellationToken = default)
        {
            var orderFile = await _readUow.OrderFiles
                .GetAll()
                .Where(x => x.Id == orderFileId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (orderFile == null) return;

            await BroadcastProgress(batchId, orderFileId, orderFile, statusDetail, cancellationToken);
        }

        private async Task BroadcastProgress(Guid batchId, Guid orderFileId, Data.Models.OrderFile orderFile, string statusDetail, CancellationToken cancellationToken)
        {
            var dto = new OrderfileDto
            {
                Id = orderFile.Id,
                BatchId = orderFile.BatchFileId,
                ProductId = orderFile.ProductId,
                FileName = orderFile.FileName,
                FilePath = orderFile.FilePath,
                FileType = Path.GetExtension(orderFile.FileName).TrimStart('.'),
                Status = orderFile.Status.ToString(),
                IsValidated = orderFile.IsValidated,
                PersonalQuantity = orderFile.PersonalQuantity,
                CommercialQuantity = orderFile.CommercialQuantity,
                ErrorMessage = orderFile.ErrorMessage,
                StatusDetail = statusDetail,
            };

            await _hubContext.Clients
                .Group($"batch-{batchId}")
                .SendAsync("orderFileStatusUpdate", dto, cancellationToken);
        }
    }
}
