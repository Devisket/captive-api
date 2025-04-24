using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.Product.Command.CreateProductType
{
    public class CreateProductTypeCommandRequest
    {
        public required string ProductName { get; set; }
        public required int ProductSequence { get; set; }
        
    }
}
