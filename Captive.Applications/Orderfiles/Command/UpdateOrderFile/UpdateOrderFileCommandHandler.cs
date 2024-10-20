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
        public UpdateOrderFileCommandHandler(
            IWriteUnitOfWork writeUow,
            IReadUnitOfWork readUow) 
        {
            _writeUow = writeUow;
            _readUow = readUow;
        }
        public async Task<Unit> Handle(UpdateOrderFileCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);

            if (orderFile == null)
                throw new HttpRequestException($"Order file id: {request.Id.ToString()} doesn't exist");

            if (!String.IsNullOrEmpty(request.ErrorMessage))
                orderFile.ErrorMessage = request.ErrorMessage;
            
            orderFile.Status = request.Status;

            _writeUow.OrderFiles.Update(orderFile);

            return Unit.Value;
        }
    }
}
