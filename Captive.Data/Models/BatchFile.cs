using Captive.Data.Enums;

namespace Captive.Data.Models
{
    public  class BatchFile
    {
        public int Id { get; set; } 
        public DateTime UploadDate { get; set; }   

        public BatchFileStatus BatchFileStatus { get; set; }

        public ICollection<OrderFile>? OrderFiles { get; set; }

        public int BankInfoId { get; set; }
        public BankInfo? BankInfo{ get; set; }

        public required string BatchName { get; set; }

        public int OrderNumber { get; set; }
    }
}
