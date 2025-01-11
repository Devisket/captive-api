using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckOrder.Command.CreateCheckOrder
{
    public class CreateFloatingCheckOrderCommand:IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
        public List<CheckOrderDto> CheckOrders { get; set; }
    }
}
