using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommand:CheckInventoryDto, IRequest<Unit>
    {
    }
}
    