using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails
{
    public class ApplyCheckInventoryDetailsCommand:IRequest<Unit>
    {
        public Guid OrderFileId { get; set; }
    }
}
