
namespace Captive.Data.Models
{
    public class TagMapping
    {
        public Guid Id {  get; set; }
        public required string TagMappingData {  get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
