using MediatR;
namespace Captive.Applications.CheckOrder.Command.HoldFloatingCheckOrder
{
    public class HoldFloatingCheckOrderCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
