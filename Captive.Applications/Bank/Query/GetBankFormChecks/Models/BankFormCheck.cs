
namespace Captive.Applications.Bank.Query.GetBankFormChecks.Models
{
    public class BankFormCheck
    {
        public required Guid Id { get; set; }
        public required Guid ProductId { get; set; }
        public string? Description { get; set; }
        public required string CheckType { get; set; }
        public required string FormType { get; set; }
        public required int Quanitity { get; set; }
    }
}
