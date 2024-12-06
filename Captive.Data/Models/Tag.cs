
namespace Captive.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set;}
        public Guid CheckValidationId { get; set;}       
        public CheckValidation CheckValidation { get; set;}
        public ICollection<TagMapping> Mapping { get; set; }
    }
}
