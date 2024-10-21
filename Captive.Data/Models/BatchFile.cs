using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public  class BatchFile
    {
        public Guid Id { get; set; }
        public required int OrderNumber { get; set; }
        public required string BatchName { get; set; }
        public required DateTime CreatedDate { get; set; }   
        public required BatchFileStatus BatchFileStatus { get; set; }
        public string ErrorMessage { get; set; }
        public ICollection<OrderFile>? OrderFiles { get; set; }
        public Guid BankInfoId { get; set; }
        public BankInfo BankInfo{ get; set; }
    }
}
