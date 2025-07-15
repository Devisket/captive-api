using System.ComponentModel.DataAnnotations;

namespace Captive.Barcode.Models
{
    public class BarcodeGenerationRequest
    {
        [Required]
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string AccountNo { get; set; } = string.Empty;
        
        [Required]
        public string BRSTN { get; set; } = string.Empty;
        
        public string StartSeries { get; set; } = string.Empty;
        
        public string EndSeries { get; set; } = string.Empty;
        
        public string BarcodeImplementation { get; set; } = "MbtcBarcode";
        
        public string? ReplyToQueue { get; set; }
        
        public string? CorrelationId { get; set; }
        
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        
        public int Priority { get; set; } = 0;
        
        public Dictionary<string, object>? AdditionalProperties { get; set; }
    }
} 