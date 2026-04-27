namespace Captive.Model.Dto
{
    public class ImportCheckInventoryResult
    {
        public int Created { get; set; }
        public int Deprecated { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
