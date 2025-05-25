
using Captive.Data.Models;

namespace Captive.Model.Dto
{
    public class TagDto
    {
        public Guid? Id { get; set; }
        public required string TagName { get; set; }
        public bool isDefaultTag { get; set; }
        public bool SearchByBranch { get; set; }
        public bool SearchByAccount { get; set; } 
        public bool SearchByFormCheck { get; set; }
        public bool SearchByProduct { get; set; }
        public bool isLocked { get; set; }
        public bool isActive { get; set; }
        public bool CheckInventoryInitiated { get; set; }

        public static TagDto ToDto (Tag input)
        {
            return new TagDto
            {
                Id = input.Id,
                TagName = input.TagName,
                isDefaultTag = input.isDefaultTag,
                SearchByBranch = input.SearchByBranch,
                SearchByAccount = input.SearchByAccount,
                SearchByFormCheck = input.SearchByFormCheck,
                SearchByProduct = input.SearchByProduct,
                isLocked = input.IsLock,
                isActive = input.IsActive,
                CheckInventoryInitiated = input.CheckInventoryInitiated,
            };
        }
    }
}
