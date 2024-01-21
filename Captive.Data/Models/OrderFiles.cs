using System.ComponentModel.DataAnnotations;

namespace Captive.Data.Models
{
    public class OrderFiles
    {
        [Key]
        public int Id { get; set; }
        public string BatchName { get; set; }
        public DateTime ProcessDate { get; set; }
        public ICollection<CheckOrders> CheckOrders { get; set; }
    }
}
