using Captive.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    public  class BatchFile
    {
        public required int Id { get; set; } 
        public DateTime UploadDate { get; set; }   

        public BatchFileStatus BatchFileStatus { get; set; }

        public ICollection<OrderFile>? OrderFiles { get; set; }

        public int BankInfoId { get; set; }
        public BankInfo? BankInfo{ get; set; }
        

    }
}
