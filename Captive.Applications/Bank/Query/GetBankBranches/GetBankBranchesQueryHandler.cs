using Captive.Applications.Bank.Query.GetBankBranches.Model;
using Captive.Data.UnitOfWork.Read;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Captive.Applications.Bank.Query.GetBankBranches
{
    public class GetBankBranchesQueryHandler : IRequestHandler<GetBankBranchesQuery, GetBankBranchesQueryResponse>
    {

        private readonly IReadUnitOfWork _readUow;

        public GetBankBranchesQueryHandler(IReadUnitOfWork readUnitOfWork) 
        { 
            _readUow = readUnitOfWork;
        }
        public async Task<GetBankBranchesQueryResponse> Handle(GetBankBranchesQuery request, CancellationToken cancellationToken)
        {
           
            if(!await _readUow.Banks.GetAll().AsNoTracking().AnyAsync(x=> x.Id == request.BankId, cancellationToken)) 
            {
                throw new Exception("Bank doesn't exist");
            }

            var bankBranch = await _readUow.BankBranches.GetAll().Where(x => x.BankId == request.BankId).ToListAsync(cancellationToken);

            return new GetBankBranchesQueryResponse
            {
                BankId = request.BankId,
                Branches = bankBranch.Select(x => new BankBranchDto
                {
                    Id = x.Id,
                    BranchName = x.BranchName,
                    BRSTN = x.BRSTNCode,
                    BranchAddress1 = x.BranchAddress1,
                    BranchAddress2 = x.BranchAddress2,
                    BranchAddress3 = x.BranchAddress3,
                    BranchAddress4 = x.BranchAddress4,
                    BranchAddress5 = x.BranchAddress5    
                }).ToList()
            };
        }
    }
}
