using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Command.DeleteBankInfo
{
    public class DeleteBankInfoCommandHandler : IRequestHandler<DeleteBankInfoCommand, Unit>
    {

        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public DeleteBankInfoCommandHandler (
            IReadUnitOfWork readUow,
            IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }
        public async Task<Unit> Handle(DeleteBankInfoCommand request, CancellationToken cancellationToken)
        {
            var bankInfo = await _readUow.Banks.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if(bankInfo == null) 
            {
                throw new Exception("Bank doesn't exist");
            }

            _writeUow.BankInfo.Delete(bankInfo);

            await _writeUow.Complete(cancellationToken);

            return Unit.Value;
        }
    }
}
