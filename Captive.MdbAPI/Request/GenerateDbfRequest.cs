namespace Captive.MdbAPI.Request
{
    public class GenerateDbfRequest
    {
        public Guid batchId { get; set; }
        public string? outputDirectory { get; set; }
    }
}
