
namespace Captive.Model.Dto.ValuesDto
{
    public class ValuesDto
    {
        public IDictionary<Guid, string> BranchValues { get; set; }
        public IDictionary<Guid, string> ProductValues { get; set; }
        public IDictionary<Guid, string> FormCheckValues { get; set; }
    }
}
