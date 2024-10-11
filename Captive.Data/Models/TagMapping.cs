
namespace Captive.Data.Models
{
    public class TagMapping
    {
        public Guid Id {  get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ProducTypeId { get; set; }
        public Guid? FormCheckId { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
