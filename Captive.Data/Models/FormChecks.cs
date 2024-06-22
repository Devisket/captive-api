namespace Captive.Data.Models
{
    public class FormChecks
    {
        public Guid Id { get; set; }

        public required string CheckType { get; set; }

        public required string FormType { get; set; }
                
        public string? Description { get; set; }

        public required int Quantity { get; set; }

        public Guid BankId { get; set; }
        public BankInfo Bank { get; set; }

        public Guid ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        public required string FileInitial { get; set; }

        public ICollection<CheckInventory>? CheckInventory{ get; set; }
        public ICollection<CheckOrders>? CheckOrders { get; set; }
    }
}
