using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommand:IRequest<Unit>
    {
        public int FormCheckId { get; set; }

        public int BranchId { get; set; }
        public int Quantity { get; set; }
        public bool WithSeries { get; set; } = true;
    }
}
