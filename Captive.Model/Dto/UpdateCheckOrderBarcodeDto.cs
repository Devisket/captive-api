using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Model.Dto
{
    public class UpdateCheckOrderBarcodeDto
    {
        public Guid CheckOrderId { get; set; }
        public Guid CheckInventoryDetailId { get; set; }
        public string BarcodeValue {  get; set; }
    }
}
