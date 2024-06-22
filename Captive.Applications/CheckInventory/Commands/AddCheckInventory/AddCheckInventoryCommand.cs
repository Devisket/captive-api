using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommand:IRequest<Unit>
    {
        public Guid FormCheckId { get; set; }

        public Guid BranchId { get; set; }
        public int Quantity { get; set; }
        public bool WithSeries { get; set; } = true;
    }
}
