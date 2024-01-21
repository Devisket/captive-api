using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class CheckAccounts
    {
        [Key]
        public int Id { get; set; }
        public string AccountNo { get; set; }
        
        public AccountInfo AccountInfo { get; set; }

        public ICollection<CheckOrders> CheckOrders { get; set; }
    }
}
