using Captive.Model.Dto;

namespace Captive.Model.Response
{
    public class CheckInventoryQueryResponse
    {
        public IEnumerable<CheckInventoryDto>? CheckInventories { get; set; }
        public int TotalCount {  get; set; }
    }
}
