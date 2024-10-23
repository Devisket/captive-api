using Captive.Applications.Bank.Query.GetBankFormChecks.Models;
using MediatR;

namespace Captive.Applications.Bank.Query.GetBankFormChecks
{
    public class GetProductFormCheckQuery:IRequest<GetProductFormCheckQueryResponse>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductFormCheckQueryResponse
    {
        public ICollection<BankFormCheck> FormChecks { get; set; }
    }
}
