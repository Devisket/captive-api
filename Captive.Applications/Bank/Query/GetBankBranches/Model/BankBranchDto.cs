
using Captive.Data.Enums;

namespace Captive.Applications.Bank.Query.GetBankBranches.Model
{
    public  class BankBranchDto
    {
        public Guid? Id { get; set; }
        public required string BrstnCode { get; set; }
        public string? BranchCode { get; set; }
        public required string BranchName { get; set; }
        public required string BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }
        public required BranchStatus BranchStatus {  get; set; }
        public Guid? MergingBranchId {  get; set; } 
    }
}
