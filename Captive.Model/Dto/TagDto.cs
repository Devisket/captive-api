
namespace Captive.Model.Dto
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<TagMappingDto>? Mapping { get; set; }
    }
}
