
using MediatR;

namespace Captive.Applications.FormChecks.Command.CreateUpdateFormCheck
{
    public class CreateUpdateFormCheckCommand : IRequest<Unit>
    {
        public int BankId { get; set; }
        public int FormCheckId { get; set; }
        
        public required CreateUpdateFormCheckCommandRequest Detail { get; set; }
    }
}
