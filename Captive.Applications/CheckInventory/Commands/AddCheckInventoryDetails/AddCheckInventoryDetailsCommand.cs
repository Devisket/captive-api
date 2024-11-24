using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails
{
    public class AddCheckInventoryDetailsCommand:IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
        public List<CheckOrderDto> CheckOrders { get; set; }
    }
}
