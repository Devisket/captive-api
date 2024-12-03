﻿using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class CheckValidation
    {
        public Guid Id { get; set; }        
        public required string Name { get; set; }
        public ValidationType ValidationType {get;set;}
        public ICollection<Tag>? Tags { get; set; }        
              
        public bool ValidateByProduct { get; set; }
        public bool ValidateByBranch { get; set; }
        public bool ValidateByFormCheck { get; set; }

        public Guid BankInfoId { get; set; }
        public BankInfo BankInfo { get; set; }

        public Guid CheckInventoryId { get; set; }
        public CheckInventory CheckInventory { get; set; }
    }
}
