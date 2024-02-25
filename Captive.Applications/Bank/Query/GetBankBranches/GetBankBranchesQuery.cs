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
        public required int BankId { get; set; }
    }
    public class GetBankBranchesQueryResponse
    {
        public required int BankId { get; set; }
        public ICollection<BankBranchDto>? Branches { get; set; }
    }
}
