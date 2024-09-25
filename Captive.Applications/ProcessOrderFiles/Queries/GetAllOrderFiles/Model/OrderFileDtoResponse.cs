
using Captive.Data.Enums;

namespace Captive.Applications.ProcessOrderFiles.Queries.GetAllOrderFiles.Model
{
    public  class OrderFileDtoResponse
    {
        public Guid Id { get; set; } 
        public Guid BatchFileId {  get; set; }   
        public required string FileName { get; set; }   
        public string FileStatus { get; set; }
        public ICollection<CheckOrderDtoResponse>? CheckOrders { get; set; } 
    }
}
