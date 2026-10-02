using Captive.Model.Dto;

namespace Captive.Model.Response
{
    public class CheckInventoryDetailsQueryResponse
    {
        public IEnumerable<CheckInventoryDetailsDto> Details { get; set; } = new List<CheckInventoryDetailsDto>();
        public int TotalCount { get; set; }
    }
}
