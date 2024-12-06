using MediatR;

namespace Captive.Applications.CheckInventory.Commands.AddCheckInventory
{
    public class AddCheckInventoryCommand:IRequest<Unit>
    {
        public Guid? Id { get; set; }
        public Guid CheckValidationId { get; set; }
        public string SeriesPattern{  get; set; }
        public bool WithSeries { get; set; } = true;
    }
}
    