
namespace Captive.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set;}
        public Guid CheckValidationId { get; set;}
        public bool isDefaultTag { get; set;} = false;
        public CheckValidation CheckValidation { get; set;}
        public ICollection<TagMapping> Mapping { get; set; }

        public Guid CheckInventoryId { get; set; }
        public CheckInventory CheckInventory { get; set; }
    }
}
