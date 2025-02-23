﻿using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class CheckValidation
    {
        public Guid Id { get; set; }        
        public required string Name { get; set; }
        public ICollection<Tag>? Tags { get; set; }     
        public Guid BankInfoId { get; set; }
        public BankInfo BankInfo { get; set; }
        public ICollection<ProductConfiguration> ProductConfigurations { get; set; }
    }
}
