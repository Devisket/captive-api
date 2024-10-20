
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.CheckOrder.Command.CreateCheckOrder
{
    public class CreateCheckOrderCommandHandler : IRequestHandler<CreateCheckOrderCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateCheckOrderCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public Task<Unit> Handle(CreateCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var orderFile = _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null) {
                throw new Exception("Orderfile ID doesn't exist");
            }

            var checkOrders = request.CheckOrderDto.Select(x => new CheckOrders
            {
                AccountNo = x.AccountNumber,
                AccountName = string.Concat(x.AccountName1, x.AccountName2),
                BRSTN = x.BRSTN,
                OrderQuanity = x.Quantity,
                FormCheckId = request.FormCheckId,
                DeliverTo = x.DeliverTo,
                Concode = x.Concode,
                InputEnable = false,
                
            }).ToArray();

            _writeUow.CheckOrders.AddRange(checkOrders,cancellationToken);

            throw new NotImplementedException();
        }
    }
}
