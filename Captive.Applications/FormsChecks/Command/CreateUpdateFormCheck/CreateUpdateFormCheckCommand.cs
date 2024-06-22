
using MediatR;

namespace Captive.Applications.FormChecks.Command.CreateUpdateFormCheck
{
    public class CreateUpdateFormCheckCommand : IRequest<Unit>
    {
        public Guid BankId { get; set; }
        public Guid FormCheckId { get; set; }
        
        public required CreateUpdateFormCheckCommandRequest Detail { get; set; }
    }
}
