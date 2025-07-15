

using Captive.Model.Dto;

namespace Captive.Messaging.Models
{
    public class GenerateBarcodeMessage
    {
        public Guid BankId { get; set; }
        public Guid BatchId { get; set; }
        public string BarcodeService{get;set;}
        public IEnumerable<CheckOrderBarcodeDto> CheckOrderBarcode { get; set; }
    }
}
