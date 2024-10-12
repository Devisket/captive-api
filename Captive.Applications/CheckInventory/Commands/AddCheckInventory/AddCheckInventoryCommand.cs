using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommand:IRequest<Unit>
    {
        public int Quantity { get; set; }
        public bool WithSeries { get; set; } = true;
    }
}
