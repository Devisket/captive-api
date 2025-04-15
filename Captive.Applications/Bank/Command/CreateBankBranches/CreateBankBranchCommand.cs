using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Bank.Command.CreateBankBranches
{
    public class CreateBankBranchCommand:IRequest<Unit>
    {
        public Guid BankId { get; set; }
        public Guid? BranchId { get; set; }
        public required string BranchName { get; set; }
        public required string BrstnCode { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }
        public required string BranchStatus { get; set; }
        public Guid? MergingBranchId { get; set; }
    }
}
