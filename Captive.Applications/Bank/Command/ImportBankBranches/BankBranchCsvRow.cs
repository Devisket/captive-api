namespace Captive.Applications.Bank.Command.ImportBankBranches
{
    public class BankBranchCsvRow
    {
        public string BRSTNCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }
        public string? BranchStatus { get; set; }
    }
}
