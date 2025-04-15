using Captive.Model.Dto.ValuesDto;
using MediatR;

namespace Captive.Applications.Values
{
    public class ValuesQuery : IRequest<ValuesQueryResponse>
    {
        public ValuesQuery(Guid bankId) { 
            this.BankId = bankId;
        }
        public Guid BankId { get; set; }
    }


    public class ValuesQueryResponse
    {
        public IList<ValuesDto> BranchValues { get; set; }
        public IList<ValuesDto> ProductValues { get; set; }
        public IList<ValuesDto> FormCheckValues { get; set; }
    }
}
