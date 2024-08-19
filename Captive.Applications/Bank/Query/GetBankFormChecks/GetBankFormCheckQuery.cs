using Captive.Applications.Bank.Query.GetBankFormChecks.Models;
using MediatR;

namespace Captive.Applications.Bank.Query.GetBankFormChecks
{
    public class GetBankFormCheckQuery:IRequest<GetBankFormCheckQueryResponse>
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class GetBankFormCheckQueryResponse
    {
        public ICollection<BankFormCheck> BankFormChecks { get; set; }
    }
}
