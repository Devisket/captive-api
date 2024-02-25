using Captive.Applications.Bank.Query.GetAllBankInfos.Model;
using MediatR;

namespace Captive.Applications.Bank.Query.GetAllBankInfos
{
    public class GetAllBankInfoQuery:IRequest<GetAllBankInfoResponse>
    {
    }

    public class GetAllBankInfoResponse
    {
        public ICollection<BankInfo>? BankInfos { get; set; }
    }
}
