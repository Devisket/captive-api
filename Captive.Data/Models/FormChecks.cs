﻿using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public class FormChecks
    {
        public Guid Id { get; set; }
        public required string CheckType { get; set; }
        public required string FormType { get; set; }
        public string? Description { get; set; }
        public required int Quantity { get; set; }
        public required string FileInitial { get; set; }
        public FormCheckType FormCheckType { get; set; }
        public bool HasBranchCodeInSeries { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }             
    }
}
