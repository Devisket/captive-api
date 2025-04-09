using Captive.Model.Dto;
using MediatR;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventory
{
    public class GetCheckInventoryQuery : IRequest<IEnumerable<CheckInventoryDto>>
    {
        public Guid TagId { get; set; }
    }
}
