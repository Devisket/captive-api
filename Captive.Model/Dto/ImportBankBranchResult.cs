namespace Captive.Model.Dto
{
    public class ImportBankBranchResult
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
