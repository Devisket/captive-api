
namespace Captive.Model.Application
{
    public class TagMappingData
    {
        public TagMappingData()
        {
            BranchIds = new List<Guid>();
            ProductIds = new List<Guid>();
            FormCheckType = new List<string>();
        }

        public required IEnumerable<Guid> BranchIds { get; set; }
        public required IEnumerable<Guid> ProductIds { get; set; }
        public required IEnumerable<string> FormCheckType { get; set; }
    }
}
