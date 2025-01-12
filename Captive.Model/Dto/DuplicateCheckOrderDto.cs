
namespace Captive.Model.Dto
{
    public class DuplicateCheckOrderDto
    {
        public Guid DuplicateOrderFileId { get; set; }
        public Guid DuplicatedCheckOrder { get; set; }
        public string DuplicateOrderFileName {  get; set; }
    }
}
