﻿using Captive.Data.Enums;
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
        public required Guid Id { get; set; }
        public required string BrstnCode { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public required string BranchName { get; set; }
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? BranchAddress3 { get; set; }
        public string? BranchAddress4 { get; set; }
        public string? BranchAddress5 { get; set; }
        public required string BranchStatus {  get; set; }
        public Guid? MergingBranchId {  get; set; } 

    }
}
