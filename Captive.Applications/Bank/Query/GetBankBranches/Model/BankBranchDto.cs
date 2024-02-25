using Captive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Bank.Query.GetBankBranches.Model
{
    public  class BankBranchDto
    {
        public required int Id { get; set; }
        public required string BRSTN { get; set; }
        public required string BranchName { get; set; }
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
    }
}
