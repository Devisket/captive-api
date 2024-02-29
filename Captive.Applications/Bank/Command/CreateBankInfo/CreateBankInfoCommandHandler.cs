using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Command.CreateBankInfo
{
    public class CreateBankInfoCommandHandler : IRequestHandler<CreateBankInfoCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        public CreateBankInfoCommandHandler(
            IReadUnitOfWork readUow,
            IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateBankInfoCommand request, CancellationToken cancellationToken)
        {

            if (request.Id.HasValue)
            {
                var bankInfo = await _readUow.Banks.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (bankInfo == null)
                    throw new Exception("Bank is null");

                bankInfo.BankName = request.BankName;
                bankInfo.BankDescription = request.Description;
            }
            else
            {
                var bankInfo = new BankInfo()
                {
                    BankName = request.BankName,
                    ShortName = request.ShortName,
                    BankDescription = request.Description,
                    CreatedDate = DateTime.UtcNow
                };

                await _writeUow.BankInfo.AddAsync(bankInfo, cancellationToken);
            }

            await _writeUow.Complete(cancellationToken);

            return Unit.Value;
        }
    }
}
