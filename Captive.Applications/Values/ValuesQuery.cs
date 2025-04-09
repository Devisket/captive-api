using Captive.Model.Dto.ValuesDto;
using MediatR;

namespace Captive.Applications.Values
{
    public class ValuesQuery : IRequest<ValuesDto>
    {
        public ValuesQuery(Guid bankId) { 
            this.BankId = bankId;
        }
        public Guid BankId { get; set; }
    }
}
