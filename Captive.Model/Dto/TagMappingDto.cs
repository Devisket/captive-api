
using Captive.Data.Models;

namespace Captive.Model.Dto
{
    public class TagMappingDto
    { 
        public Guid? Id { get; set; }
        
        public Guid TagId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? FormCheckId { get; set; }

        public static TagMappingDto ToDto (TagMapping input)
        {
            return new TagMappingDto
            {
                Id = input.Id,
                TagId = input.TagId,
                BranchId = input.BranchId,
                ProductId = input.ProductId,
                FormCheckId = input.FormCheckId,
            };
        }
    }
}
