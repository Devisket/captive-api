
using Captive.Data.Enums;

namespace Captive.Applications.OrderFile.Queries.GetAllOrderFiles.Model
{
    public  class BatchFileDtoResponse
    {
        public int Id { get; set; }
        public DateTime UploadDate { get; set; }
        public ICollection<OrderFileDtoResponse>? OrderFiles { get; set; }
    }
}
