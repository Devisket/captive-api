
namespace Captive.Applications.Bank.Query.GetAllBankInfos.Model
{
    public class BankInfo
    {
        public required int Id { get; set; }
        public required string BankName { get; set; }
        public required string BankDescription { get; set; }
        public required DateTime CreatedDate { get; set; }
    }
}
