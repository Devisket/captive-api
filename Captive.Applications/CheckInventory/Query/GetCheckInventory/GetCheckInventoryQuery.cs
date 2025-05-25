using Captive.Model.Dto;
using Captive.Model.Request.Interface;
using Captive.Model.Response;
using MediatR;

namespace Captive.Applications.CheckInventory.Query.GetCheckInventory
{
    public class GetCheckInventoryQuery : IPaginatedRequest, IRequest<CheckInventoryQueryResponse>
    {
        public Guid TagId { get; set; }
        public required int PageSize { get; set; }
        public required int CurrentPage { get; set; }
        public IEnumerable<Guid>? BranchIds { get; set; }
        public IEnumerable<Guid>? ProductIds { get; set; }
        public IEnumerable<string>? FormCheckType{ get; set; }
        public bool IsActive {  get; set; }
        public bool IsRepeating{ get; set; }
    }
}
