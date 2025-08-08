using Captive.Applications.Orderfiles.Services;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Orderfiles.Command.UpdateOrderFile
{
    public class UpdateOrderFileCommandHandler : IRequestHandler<UpdateOrderFileCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IOrderFileService _orderFileService;
        public UpdateOrderFileCommandHandler(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow,
            IOrderFileService orderFileService) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
            _orderFileService = orderFileService;
        }
        public async Task<Unit> Handle(UpdateOrderFileCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);

            if (orderFile == null)
                throw new HttpRequestException($"Order file id: {request.Id.ToString()} doesn't exist");

            // Update error message first if provided
            if (!String.IsNullOrEmpty(request.ErrorMessage))
            {
                orderFile.ErrorMessage = request.ErrorMessage;
                _writeUow.OrderFiles.Update(orderFile);
                await _orderFileService.UpdateOrderFileStatus(request.Id, request.ErrorMessage, cancellationToken);
            }
            else
            {
                // Update status through the service to trigger SignalR notifications
                await _orderFileService.UpdateOrderFileStatus(request.Id, request.Status, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
