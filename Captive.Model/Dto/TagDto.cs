
namespace Captive.Model.Dto
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public Guid BankId { get; set; }
        public string TagName { get; set; }
        public bool isDefaultTag { get; set; } = false;
        public bool SearchByBranch { get; set; } = false;
        public bool SearchByAccount { get; set; } = false;
        public bool SearchByFormCheck { get; set; } = false;
        public bool SearchByProduct { get; set; } = false;
        public ICollection<TagMappingDto>? Mapping { get; set; }
    }
}
