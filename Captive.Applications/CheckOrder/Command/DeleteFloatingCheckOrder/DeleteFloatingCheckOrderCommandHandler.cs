using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Command.DeleteFloatingCheckOrder
{
    internal class DeleteFloatingCheckOrderCommandHandler : IRequestHandler<DeleteFloatingCheckOrderCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        public DeleteFloatingCheckOrderCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) { 
            _readUow = readUow;
            _writeUow = writeUow;
        }
        public async Task<Unit> Handle(DeleteFloatingCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var floatingCheckOrder = await _readUow.FloatingCheckOrders.GetAll().FirstOrDefaultAsync(x => x.Id == request.FloatingCheckOrderId, cancellationToken);

            if (floatingCheckOrder == null)
                throw new Exception($"Floating Check Order ID: {request.FloatingCheckOrderId} doesn't exist.");

            _writeUow.FloatingCheckOrders.Delete(floatingCheckOrder);

            return Unit.Value;
        }
    }
}
