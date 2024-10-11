using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class CheckValidation
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ValidationType ValidationType {get;set;}
        public ICollection<Tag>? Tags { get; set; }
        public ICollection<CheckInventory>? CheckInventory { get; set; }
    }
}
