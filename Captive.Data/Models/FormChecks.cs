namespace Captive.Data.Models
{
    public class FormChecks
    {
        public int Id { get; set; }

        public required string CheckType { get; set; }

        public required string FormType { get; set; }
                
        public string? Description { get; set; }

        public required int Quantity { get; set; }

        public int BankId { get; set; }
        public required BankInfo Bank { get; set; }

        public int ProductTypeId { get; set; }
        public required ProductType ProductType { get; set; }

        public ICollection<CheckInventory>? CheckInventory{ get; set; }
        public ICollection<CheckOrders>? CheckOrders { get; set; }
    }
}
