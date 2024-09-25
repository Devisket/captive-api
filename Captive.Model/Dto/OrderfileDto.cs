
namespace Captive.Model.Dto
{
    public class OrderfileDto
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public required string FileName {  get; set; }
        public required string FilePath { get; set; }
    }
}
