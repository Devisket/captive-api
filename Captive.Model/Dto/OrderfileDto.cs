
namespace Captive.Model.Dto
{
    public class OrderfileDto
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public Guid ProductId { get; set; }
        public required string FileName {  get; set; }
        public required string FilePath { get; set; }
        public required string FileType { get; set; }
        public required string Status { get; set; }

        public ICollection<CheckOrderDto>? CheckOrders { get; set; }
    }
}
