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
            var bankBranch = _readUow.BankBranches.GetAll()
                .Where(x => x.BankInfoId == request.BankId);

            if(request.BranchId.HasValue)
                bankBranch = bankBranch.Where(x => x.Id == request.BranchId);

            return new GetBankBranchesQueryResponse
            {
                Branches = await bankBranch.Select(x => new BankBranchDto
                {
                    Id = x.Id,
                    BranchName = x.BranchName,
                    BrstnCode = x.BRSTNCode,
                    BranchAddress1 = x.BranchAddress1,
                    BranchAddress2 = x.BranchAddress2,
                    BranchAddress3 = x.BranchAddress3,
                    BranchAddress4 = x.BranchAddress4,
                    BranchAddress5 = x.BranchAddress5,
                    BranchStatus = x.BranchStatus.ToString(),
                    MergingBranchId = x.MergingBranchId,
                }).ToListAsync(cancellationToken)
            };
        }
    }
}
