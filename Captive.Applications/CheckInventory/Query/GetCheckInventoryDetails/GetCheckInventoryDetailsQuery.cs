using Captive.Model.Request.Interface;
using Captive.Model.Response;
using MediatR;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventoryDetails
{
    /// <summary>
    /// Returns the series assigned from a check inventory (<see cref="CheckInventoryId"/>), or, when
    /// <see cref="Unassigned"/> is true, the series that were assigned to check orders without a
    /// check inventory (manually provided starting/ending series).
    /// </summary>
    public class GetCheckInventoryDetailsQuery : IPaginatedRequest, IRequest<CheckInventoryDetailsQueryResponse>
    {
        public Guid BankId { get; set; }
        public Guid? CheckInventoryId { get; set; }
        public bool Unassigned { get; set; }
        public string? Search { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
    }
}
