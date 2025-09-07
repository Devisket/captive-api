
namespace Captive.Applications.Bank.Query.GetAllBankInfos.Model
{
    public class BankInfo
    {
        public required Guid Id { get; set; }
        public required string BankName { get; set; }
        public required string BankShortName { get; set; }
        public required DateTime CreatedDate { get; set; }
        public string? AccountNumberFormat {  get; set; }
    }
}
