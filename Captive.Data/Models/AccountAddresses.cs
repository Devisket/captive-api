using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    public  class AccountAddresses
    {
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
       
        public AccountInfo AccountInfo { get; set; }
    }
}
