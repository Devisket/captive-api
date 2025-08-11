
using Captive.Applications.FormsChecks.Services;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Captive.Applications.CheckOrder.Command.CreateCheckOrder
{
    public class CreateFloatingCheckOrderCommandHandler : IRequestHandler<CreateFloatingCheckOrderCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IFormsChecksService _formCheckService;

        public CreateFloatingCheckOrderCommandHandler(IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IFormsChecksService formCheckService)
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _formCheckService = formCheckService;
        }

        public async Task<Unit> Handle(CreateFloatingCheckOrderCommand request, CancellationToken cancellationToken)
        {
            var orderFile = await _readUow.OrderFiles.GetAll().FirstOrDefaultAsync(x => x.Id == request.OrderFileId);

            if (orderFile == null)
            {
                throw new Exception("Orderfile ID doesn't exist");
            }

            foreach(var floatingCheckOrder in request.CheckOrders)
            {
                if (floatingCheckOrder.Id.HasValue)
                {
                    var existingFloatingCheckOrder = await _readUow.FloatingCheckOrders.GetAll().FirstOrDefaultAsync(x => x.Id == floatingCheckOrder.Id.Value, cancellationToken);

                    if (existingFloatingCheckOrder == null)
                        throw new Exception($"Floating Check Order ID:{floatingCheckOrder.Id} doesn't exist.");

                    existingFloatingCheckOrder.AccountName = String.Format("{0} {1}", floatingCheckOrder.AccountName1, floatingCheckOrder.AccountName2); ;
                    existingFloatingCheckOrder.AccountName1 = floatingCheckOrder.AccountName1 ?? string.Empty;
                    existingFloatingCheckOrder.AccountName2 = floatingCheckOrder.AccountName2 ?? string.Empty;
                    existingFloatingCheckOrder.ErrorMessage = string.Empty;
                    existingFloatingCheckOrder.BRSTN = floatingCheckOrder.BRSTN;
                    existingFloatingCheckOrder.FormType = floatingCheckOrder.FormType;
                    existingFloatingCheckOrder.CheckType = floatingCheckOrder.CheckType;
                    existingFloatingCheckOrder.AccountNo = floatingCheckOrder.AccountNumber;
                    existingFloatingCheckOrder.PreEndingSeries = floatingCheckOrder.StartingSeries;
                    existingFloatingCheckOrder.PreStartingSeries = floatingCheckOrder.EndingSeries;
                    existingFloatingCheckOrder.Quantity = floatingCheckOrder.Quantity;
                    existingFloatingCheckOrder.DeliverTo = floatingCheckOrder.DeliverTo;
                    existingFloatingCheckOrder.IsValid = false;


                    _writeUow.FloatingCheckOrders.Update(existingFloatingCheckOrder);
                }
                else {
                    var newFloatingCheckOrder = new FloatingCheckOrder
                    {
                        Id = Guid.NewGuid(),
                        AccountName = floatingCheckOrder.MainAccountName ?? string.Empty,
                        AccountName1 = floatingCheckOrder.AccountName1 ?? string.Empty,
                        AccountName2 = floatingCheckOrder.AccountName2 ?? string.Empty,
                        IsValid = false,
                        ErrorMessage = string.Empty,
                        BRSTN = floatingCheckOrder.BRSTN,
                        FormType = floatingCheckOrder.FormType,
                        CheckType = floatingCheckOrder.CheckType,
                        Concode = floatingCheckOrder.Concode,
                        BranchCode = floatingCheckOrder.BranchCode,
                        AccountNo = floatingCheckOrder.AccountNumber,
                        OrderFileId = request.OrderFileId,
                        PreEndingSeries = floatingCheckOrder.StartingSeries,
                        PreStartingSeries = floatingCheckOrder.EndingSeries,
                        Quantity = floatingCheckOrder.Quantity,
                        DeliverTo = floatingCheckOrder.DeliverTo,
                    };

                    await _writeUow.FloatingCheckOrders.AddAsync(newFloatingCheckOrder, cancellationToken);
                }
            }

            //var newFloatingCheckOrder = request.CheckOrders.Select(x => new FloatingCheckOrder
            //{
            //    Id = Guid.NewGuid(),
            //    AccountName = x.MainAccountName ?? string.Empty,
            //    AccountName1 = x.AccountName1 ?? string.Empty,
            //    AccountName2 = x.AccountName2 ?? string.Empty,
            //    IsValid = false,
            //    ErrorMessage = string.Empty,
            //    BRSTN = x.BRSTN,
            //    FormType = x.FormType,
            //    CheckType = x.CheckType,
            //    Concode = x.Concode,
            //    BranchCode = x.BranchCode,
            //    AccountNo = x.AccountNumber,
            //    OrderFileId = request.OrderFileId,
            //    PreEndingSeries = x.StartingSeries,
            //    PreStartingSeries = x.EndingSeries,
            //    Quantity = x.Quantity,
            //    DeliverTo = x.DeliverTo,
            //}).ToArray();

            //await _writeUow.FloatingCheckOrders.AddRange(newFloatingCheckOrder, cancellationToken);

            return Unit.Value;
        }
    }
}
