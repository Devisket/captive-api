using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Command.CreateBankBranches
{
    public class CreateBankBranchCommandHandler : IRequestHandler<CreateBankBranchCommand, Unit>
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public CreateBankBranchCommandHandler(
            IReadUnitOfWork readUow, 
            IWriteUnitOfWork writeUow)
        {
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task<Unit> Handle(CreateBankBranchCommand request, CancellationToken cancellationToken)
        {
            var bank = await _readUow.Banks.GetAll().FirstOrDefaultAsync(x => x.Id == request.BankId, cancellationToken);

            if(bank == null)
            {
               throw new Exception($"Bank Id: {request.BankId} doesn't exist.");
            }

            if(request.BranchId != Guid.Empty)
            {
               var branch = await _readUow.BankBranches.GetAll().FirstOrDefaultAsync(x => x.Id == request.BranchId, cancellationToken);

               if(branch == null)
                   throw new Exception($"Branch Id: {request.BranchId} doesnt exist.");

               if (await _readUow.BankBranches.GetAll().AsNoTracking().AnyAsync(x => x.BankInfoId == request.BankId && 
                       x.BRSTNCode == request.BrstnCode && x.Id != request.BranchId, cancellationToken))
                   throw new Exception($"BRSTN Code: {request.BrstnCode} for bank ${bank.BankName} is conflicting.");

               branch.BranchName = request.BranchName;
               branch.BRSTNCode = request.BrstnCode;
               branch.BranchAddress1 = request.BranchAddress1;
               branch.BranchAddress2 = request.BranchAddress2;
               branch.BranchAddress3 = request.BranchAddress3;
               branch.BranchAddress4 = request.BranchAddress4;
               branch.BranchAddress5 = request.BranchAddress5;

               _writeUow.BankBranches.Update(branch);
            }
            else
            {
               if (await _readUow.BankBranches.GetAll().AsNoTracking().AnyAsync(x => x.BankInfoId == request.BankId && x.BRSTNCode == request.BrstnCode, cancellationToken))
                   throw new Exception($"BRSTN Code: {request.BrstnCode} for bank ${bank.BankName} is conflicting.");

               await _writeUow.BankBranches.AddAsync(new BankBranches
               {
                   BankInfoId = request.BankId,
                   BranchAddress1 = request.BranchAddress1,
                   BranchAddress2 = request.BranchAddress2,
                   BranchAddress3 = request.BranchAddress3,
                   BranchAddress4 = request.BranchAddress4,
                   BranchAddress5 = request.BranchAddress5,
                   BRSTNCode = request.BrstnCode,
                   BranchName = request.BranchName,
                   BranchStatus = BranchStatus.Active,

               },cancellationToken);
            }

            await _writeUow.Complete();

            return Unit.Value;
        }
    }
}
