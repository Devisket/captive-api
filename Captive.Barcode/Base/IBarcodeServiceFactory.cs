using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Barcode.Base
{
    public interface IBarcodeServiceFactory
    {
        IBarcodeService GetBarcodeService(string barcodeImplementationName);
        IEnumerable<string> GetAvailableImplementations();
    }
} 