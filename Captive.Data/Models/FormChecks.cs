﻿namespace Captive.Data.Models
{
    public class FormChecks
    {
        public int Id { get; set; }

        public required string CheckType { get; set; }

        public required string FormType { get; set; }
                
        public string? Description { get; set; }

        public required int Quantity { get; set; }

        public int BankId { get; set; }
        public BankInfo Bank { get; set; }

        public ICollection<CheckInventory>? CheckInventory{ get; set; }
    }
}
