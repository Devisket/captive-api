
namespace Captive.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set;}
        public Guid BankId { get; set; }
        public bool isDefaultTag { get; set;} = false;
        public bool SearchByBranch { get; set; } = false;
        public bool SearchByAccount { get; set; } = false;
        public bool SearchByFormCheck { get; set; } = false;
        public bool SearchByProduct { get; set; } = false;
        public ICollection<TagMapping> Mapping { get; set; }
        public ICollection<CheckInventory>? CheckInventories { get; set; }
        public bool IsLock { get; set; }
        public bool IsActive { get; set; }
    }
}
