
namespace Captive.Model.Dto
{
    public class CheckInventoryMappingData
    {
        public CheckInventoryMappingData()
        {
            BranchIds = new List<Guid>();
            ProductIds = new List<Guid>();
            FormCheckIds = new List<Guid>();
        }
        public CheckInventoryMappingData(IEnumerable<Guid> branchIds, IEnumerable<Guid> productIds, IEnumerable<Guid> formCheckIds)
        {
            BranchIds = branchIds;
            ProductIds = productIds;
            FormCheckIds = formCheckIds;
        }

        public IEnumerable<Guid> BranchIds { get; set; }
        public IEnumerable<Guid> ProductIds { get; set; }
        public IEnumerable<Guid> FormCheckIds { get; set; }
    }
}
