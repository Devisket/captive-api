using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Command.CheckDuplication
{
    public class CheckDuplicationCommandHandler : IRequestHandler<CheckDuplicationCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUnitOfWork;
        private readonly IWriteUnitOfWork _writeUnitOfWork;

        public CheckDuplicationCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow) 
        {
            _readUnitOfWork = readUow;
            _writeUnitOfWork = writeUow;
        }
        public async Task<Unit> Handle(CheckDuplicationCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUnitOfWork.OrderFiles.GetAll()
                .Include(x => x.FloatingCheckOrders)
                .FirstOrDefaultAsync(x => x.Id == request.OrderFileId, cancellationToken);


            if (orderFile == null) 
            {
                throw new SystemException($"Order file ID {request.OrderFileId} doesn't exist.");
            }

            if(orderFile.FloatingCheckOrders == null && !orderFile.FloatingCheckOrders.Any())
            {
                return Unit.Value;
            }

            var floatingCheckOrders = orderFile.FloatingCheckOrders.ToArray();

            var otherOrderFile = _readUnitOfWork.OrderFiles.GetAll().Where(x => x.BatchFileId == orderFile.BatchFileId && x.Id != orderFile.Id);

            var otherCheckOrder = otherOrderFile.SelectMany(p => p.FloatingCheckOrders, (parent, child)=> new {OrderFileId = parent.Id, child.AccountNo});

            foreach (var checkOrder in floatingCheckOrders) 
            {
                if (otherCheckOrder.Any(x => x.AccountNo == checkOrder.AccountNo))
                {
                    checkOrder.IsValid = false;
                    checkOrder.ErrorMessage = $"It has duplicated account from other order file.";

                    _writeUnitOfWork.FloatingCheckOrders.Update(checkOrder);
                }
            }

            return Unit.Value;
        }
    }
}
