using Captive.Data.Models;
using Captive.Model.Application;
using Newtonsoft.Json;

namespace Captive.Model.Dto
{
    public class TagMappingDto
    {
        public Guid Id { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag{ get; set; }
        public TagMappingData? Mappings { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public static TagMappingDto ToDto(TagMapping data)
        {
            return new TagMappingDto
            {
                Id = data.Id,
                TagId = data.TagId,
                Mappings = JsonConvert.DeserializeObject<TagMappingData>(data.TagMappingData) ?? null,
                CreatedDate = data.CreatedDate,
                UpdatedDate = data.UpdatedDate,
            };  
        }
    }
}
