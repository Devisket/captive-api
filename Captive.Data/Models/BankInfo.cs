namespace Captive.Data.Models
{
    public class BankInfo
    {
        public Guid Id { get; set; }   
        public required string BankName { get; set; }
        public required string ShortName { get; set; }
        public string? BankDescription { get; set;}
        public string? AccountNumberFormat { get; set; }
        public required DateTime CreatedDate { get; set; }
        public ICollection<BankBranches>? BankBranches { get; set; }
        public ICollection<BatchFile>? BatchFiles { get;set; }
        public ICollection<Product>? Products { get; set; }
        
    }
}
