

namespace Captive.Model.Dto
{
    public class CheckValidationDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ValidationType { get; set; }
        public ICollection<TagDto>? Tags { get; set; }
    }
}
