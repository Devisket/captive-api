

namespace Captive.Model.Dto
{
    public class CheckInventoryMappingData
    {
        public CheckInventoryMappingData()
        {
            BranchIds = new List<Guid>();
            ProductIds = new List<Guid>();
            FormCheckType = new List<string>();
        }
        public CheckInventoryMappingData(IEnumerable<Guid> branchIds, IEnumerable<Guid> productIds, IEnumerable<string> formCheckIds)
        {
            BranchIds = branchIds;
            ProductIds = productIds;
            FormCheckType = formCheckIds;
        }

        public IEnumerable<Guid> BranchIds { get; set; }
        public IEnumerable<Guid> ProductIds { get; set; }
        public IEnumerable<string> FormCheckType { get; set; }
    }
}
