
using Captive.Applications.FormsChecks.Services;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

            var newFloatingCheckOrder = request.CheckOrders.Select(x => new FloatingCheckOrder
            {
                Id = Guid.NewGuid(),
                AccountName = x.MainAccountName ?? string.Empty,
                AccountName1 = x.AccountName1 ?? string.Empty,
                AccountName2 = x.AccountName2 ?? string.Empty,
                IsValid = false,
                ErrorMessage = string.Empty,
                BRSTN = x.BRSTN,
                FormType = x.FormType,
                CheckType = x.CheckType,
                Concode = x.Concode,
                BranchCode = x.BranchCode,
                AccountNo = x.AccountNumber,
                OrderFileId = request.OrderFileId,
                PreEndingSeries = x.StartingSeries,
                PreStartingSeries = x.EndingSeries,
                Quantity = x.Quantity,
                DeliverTo = x.DeliverTo,
            }).ToArray();

            await _writeUow.FloatingCheckOrders.AddRange(newFloatingCheckOrder, cancellationToken);

            return Unit.Value;
        }
    }
}
