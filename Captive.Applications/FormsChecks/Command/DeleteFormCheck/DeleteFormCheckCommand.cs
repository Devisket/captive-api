using MediatR;

namespace Captive.Applications.FormsChecks.Command.DeleteFormCheck
{
    public class DeleteFormCheckCommand : IRequest<Unit>
    {
        public Guid FormCheckId { get; set; }
    }
}
