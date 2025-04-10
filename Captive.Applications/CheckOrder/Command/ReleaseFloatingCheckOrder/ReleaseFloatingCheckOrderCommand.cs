using MediatR;
namespace Captive.Applications.CheckOrder.Command.ReleaseFloatingCheckOrder
{
    public class ReleaseFloatingCheckOrderCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
