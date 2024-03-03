
using Captive.Data.Enums;

namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model
{
    public  class OrderFileDtoResponse
    {
        public int Id { get; set; } 
        public int BatchFileId {  get; set; }   
        public required string FileName { get; set; }   
        public string FileStatus { get; set; }
        public ICollection<CheckOrderDtoResponse>? CheckOrders { get; set; } 
    }
}
