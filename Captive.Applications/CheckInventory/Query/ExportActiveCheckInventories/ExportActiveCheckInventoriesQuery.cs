using MediatR;

namespace Captive.Applications.CheckInventory.Query.ExportActiveCheckInventories
{
    public class ExportActiveCheckInventoriesQuery : IRequest<byte[]>
    {
        public Guid BankId { get; set; }
    }
}
