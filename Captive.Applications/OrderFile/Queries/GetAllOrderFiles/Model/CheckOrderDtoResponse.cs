
namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model
{
    public class CheckOrderDtoResponse
    {
        public Guid Id {  get; set; }
        public string? AccountName { get; set; } 
        public required string BRSTN { get; set; }   
        public string? DeliveringBRSTN { get; set; }       
        public int Quantity { get; set; }
    }
}
