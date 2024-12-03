
using Captive.Applications.FormsChecks.Services;
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
        private readonly IFormsChecksService _formCheckService;

        public CreateCheckOrderCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IFormsChecksService formCheckService)
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _formCheckService = formCheckService;
        }

        public async Task<Unit> Handle(CreateCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null) {
                throw new Exception("Orderfile ID doesn't exist");
            }

            List<Captive.Data.Models.CheckOrders> newCheckOrders = new List<CheckOrders>();


            foreach (var checkOrder in request.CheckOrders){
                
                if (checkOrder.IsValid)
                {
                    var formCheck = await _formCheckService.GetCheckOrderFormCheck(checkOrder, cancellationToken);

                    newCheckOrders.Add(new CheckOrders
                    {
                        Id = Guid.Empty,
                        AccountNo = checkOrder.AccountNumber,
                        ProductId = checkOrder.ProductId,
                        AccountName = string.Concat(checkOrder.AccountName1, checkOrder.AccountName2),
                        BRSTN = checkOrder.BRSTN,
                        OrderQuanity = checkOrder.Quantity,
                        FormCheckId = formCheck?.Id,
                        DeliverTo = checkOrder.DeliverTo,
                        Concode = checkOrder.Concode,
                        OrderFileId = request.OrderFileId,
                        ErrorMessage = formCheck != null ? string.Empty : "Cannot find formcheck",
                        IsValid = formCheck != null,
                        InputEnable = false,
                    });
                }
                else
                {
                    newCheckOrders.Add(new CheckOrders
                    {
                        Id = Guid.Empty,
                        ProductId = checkOrder.ProductId,
                        AccountNo = checkOrder.AccountNumber,
                        AccountName = string.Concat(checkOrder.AccountName1, checkOrder.AccountName2),
                        BRSTN = checkOrder.BRSTN,
                        OrderQuanity = checkOrder.Quantity,
                        FormCheckId = null,
                        DeliverTo = checkOrder.DeliverTo,
                        Concode = checkOrder.Concode,
                        OrderFileId = request.OrderFileId,
                        ErrorMessage = checkOrder.ErrorMessage ?? string.Empty,
                        IsValid = checkOrder.IsValid,
                        InputEnable = false,
                    });
                }
            }

            if (newCheckOrders == null || !newCheckOrders.Any())
                throw new Exception("Cannot create check order");

            await _writeUow.CheckOrders.AddRange(newCheckOrders.ToArray(), cancellationToken);

            return Unit.Value;
        }
    }
}
