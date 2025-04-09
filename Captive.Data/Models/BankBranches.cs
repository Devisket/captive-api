using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class BankBranches
    {
        public Guid Id { get; set; }
        public required string BranchName { get; set; }
        public required string BRSTNCode { get; set; }
        public string BranchCode { get; set; }
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }
        
        public Guid? MergingBranchId { get; set; }
        public required BranchStatus BranchStatus { get; set; }

        public Guid BankInfoId { get; set; }
        public BankInfo BankInfo { get; set; }
    }
}
