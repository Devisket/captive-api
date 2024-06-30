using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.Models
{
    internal class CheckInventoryDetail
    {
        public Guid Id { get; set; }
        public Guid CheckInventoryId { get; set; }

        public Guid? CheckOrderId { get; set; }

        public string? StarSeries { get; set; }
        public string? EndSeries { get; set; }

        public required int Quantity { get; set; }
    }
}
