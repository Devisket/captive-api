
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
                    var formCheckId = await GetFormCheckId(orderFile.ProductId, checkOrder.FormType, checkOrder.CheckType, cancellationToken);

                    newCheckOrders.Add(new CheckOrders
                    {
                        AccountNo = checkOrder.AccountNumber,
                        AccountName = string.Concat(checkOrder.AccountName1, checkOrder.AccountName2),
                        BRSTN = checkOrder.BRSTN,
                        OrderQuanity = checkOrder.Quantity,
                        FormCheckId = formCheckId ?? null,
                        DeliverTo = checkOrder.DeliverTo,
                        Concode = checkOrder.Concode,
                        ErrorMessage = formCheckId.HasValue ? string.Empty : "Cannot find formcheck",
                        IsValid = formCheckId.HasValue,
                        InputEnable = false,
                    });
                }
                else
                {
                    newCheckOrders.Add(new CheckOrders
                    {
                        AccountNo = checkOrder.AccountNumber,
                        AccountName = string.Concat(checkOrder.AccountName1, checkOrder.AccountName2),
                        BRSTN = checkOrder.BRSTN,
                        OrderQuanity = checkOrder.Quantity,
                        FormCheckId = null,
                        DeliverTo = checkOrder.DeliverTo,
                        Concode = checkOrder.Concode,
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

        private async Task<Guid?> GetFormCheckId(Guid productId, string formType, string checkType, CancellationToken cancellationToken)
        {
            var formCheck = await _readUow.FormChecks.GetAll().FirstOrDefaultAsync(x => x.CheckType == checkType && x.FormType == formType && x.ProductId == productId, cancellationToken);

            return formCheck != null ?  formCheck.Id : null;
        }
    }
}
