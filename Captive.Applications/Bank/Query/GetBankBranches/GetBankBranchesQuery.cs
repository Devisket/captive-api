using Captive.Applications.Bank.Query.GetBankBranches.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Bank.Query.GetBankBranches
{
    public class GetBankBranchesQuery:IRequest<GetBankBranchesQueryResponse>
    {
        public required Guid BankId { get; set; }
        public Guid? BranchId { get; set; }
    }
    public class GetBankBranchesQueryResponse
    {
        public ICollection<BankBranchDto>? Branches { get; set; }
    }
}
