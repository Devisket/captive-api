using Captive.Model.Dto;

namespace Captive.Model.Request
{
    public class UpdateCheckOrderBarCodeRequest
    {
        public Guid BatchId { get; set; }
        public IEnumerable<UpdateCheckOrderBarcodeDto> CheckOrdersToUpdate { get; set; }
    }
}
