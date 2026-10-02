using Captive.Applications.Orderfiles.Services;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Reports.Commands
{
    public class ReportGenerationFailedCommandHandler : IRequestHandler<ReportGenerationFailedCommand, Unit>
    {
        private const string DefaultMessage = "Report generation failed.";
        private const int MaxErrorMessageLength = 1000;

        private readonly IReadUnitOfWork _readUow;
        private readonly IOrderFileService _orderFileService;

        public ReportGenerationFailedCommandHandler(IReadUnitOfWork readUow, IOrderFileService orderFileService)
        {
            _readUow = readUow;
            _orderFileService = orderFileService;
        }

        public async Task<Unit> Handle(ReportGenerationFailedCommand request, CancellationToken cancellationToken)
        {
            var orderFileIds = await _readUow.OrderFiles.GetAll()
                .AsNoTracking()
                .Where(x => x.BatchFileId == request.BatchId && x.Status == OrderFilesStatus.GeneratingReport)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var errorMessage = string.IsNullOrWhiteSpace(request.ErrorMessage)
                ? DefaultMessage
                : request.ErrorMessage.Trim();

            if (errorMessage.Length > MaxErrorMessageLength)
                errorMessage = errorMessage.Substring(0, MaxErrorMessageLength);

            // Sets status = Error + error message and broadcasts "orderFileStatusUpdate" via SignalR.
            // Changes are committed by the DatabasePipeline once the handler returns.
            foreach (var orderFileId in orderFileIds)
                await _orderFileService.UpdateOrderFileStatus(orderFileId, errorMessage, cancellationToken);

            return Unit.Value;
        }
    }
}
