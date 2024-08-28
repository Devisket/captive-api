using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Command.DeleteBankBranch
{
    public class DeleteBankBranchCommandHandler : IRequestHandler<DeleteBankBranchCommand, Unit>
    {
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IReadUnitOfWork _readUow;

        public DeleteBankBranchCommandHandler (IWriteUnitOfWork writeUnitOfWork, IReadUnitOfWork readUow)
        {
            _writeUow = writeUnitOfWork;
            _readUow = readUow;
        }

        public async Task<Unit> Handle(DeleteBankBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _readUow.BankBranches.GetAll().FirstOrDefaultAsync(x => x.Id == request.BranchId && x.BankInfoId == request.BankId);

            if(branch == null)
            {
               throw new Exception($"BankId: {request.BankId} with BranchId:{request.BranchId} doesn't exist");
            }

            _writeUow.BankBranches.Delete(branch);

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
