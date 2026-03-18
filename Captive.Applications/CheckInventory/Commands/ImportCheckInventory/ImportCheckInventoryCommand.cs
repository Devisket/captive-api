using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckInventory.Commands.ImportCheckInventory
{
    public class ImportCheckInventoryCommand : IRequest<ImportCheckInventoryResult>
    {
        public Guid BankId { get; set; }
        public Stream FileStream { get; set; } = null!;
    }
}
