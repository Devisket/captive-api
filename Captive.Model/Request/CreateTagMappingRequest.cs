
namespace Captive.Model.Request
{
    public class CreateTagMappingRequest
    {
        public ICollection<TagMappingRequestObject> mappings { get; set; }
    }

    public class TagMappingRequestObject
    {
        public Guid? BranchId { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? FormCheckId { get; set; }
    }
}
