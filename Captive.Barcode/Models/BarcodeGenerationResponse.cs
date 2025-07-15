namespace Captive.Barcode.Models
{
    public class BarcodeGenerationResponse
    {
        public string RequestId { get; set; } = string.Empty;
        
        public string CorrelationId { get; set; } = string.Empty;
        
        public bool Success { get; set; }
        
        public string? Barcode { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public string? ErrorCode { get; set; }
        
        public DateTime ProcessedTime { get; set; } = DateTime.UtcNow;
        
        public TimeSpan ProcessingDuration { get; set; }
        
        public string BarcodeImplementation { get; set; } = string.Empty;
        
        public Dictionary<string, object>? AdditionalData { get; set; }
        
        public static BarcodeGenerationResponse Success(string requestId, string correlationId, string barcode, string implementation, TimeSpan duration)
        {
            return new BarcodeGenerationResponse
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Success = true,
                Barcode = barcode,
                BarcodeImplementation = implementation,
                ProcessingDuration = duration
            };
        }
        
        public static BarcodeGenerationResponse Error(string requestId, string correlationId, string errorMessage, string? errorCode = null)
        {
            return new BarcodeGenerationResponse
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }
} 