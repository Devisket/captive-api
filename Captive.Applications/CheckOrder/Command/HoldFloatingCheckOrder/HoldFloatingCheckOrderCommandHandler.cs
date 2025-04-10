using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Command.HoldFloatingCheckOrder
{
    public class HoldFloatingCheckOrderCommandHandler : IRequestHandler<HoldFloatingCheckOrderCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public HoldFloatingCheckOrderCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        { 
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(HoldFloatingCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var floatingCheckOrder = await _readUow.FloatingCheckOrders.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id);

            if (floatingCheckOrder == null) {
                throw new Exception($"Floating Check Order ID: {request.Id} doesn't exist.");
            }

            floatingCheckOrder.IsOnHold = true;

            _writeUow.FloatingCheckOrders.Update(floatingCheckOrder);
           
            return Unit.Value;
        }
    }
}
