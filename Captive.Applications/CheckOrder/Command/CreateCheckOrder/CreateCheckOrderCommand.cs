using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckOrder.Command.CreateCheckOrder
{
    public class CreateCheckOrderCommand:IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
        public IList<CheckOrderDto> CheckOrders { get; set; }
    }
}
