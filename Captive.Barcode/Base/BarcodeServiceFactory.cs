using System;
using System.Collections.Generic;
using System.Linq;

namespace Captive.Barcode.Base
{
    public class BarcodeServiceFactory : IBarcodeServiceFactory
    {
        private readonly IEnumerable<IBarcodeService> _barcodeServices;

        public BarcodeServiceFactory(IEnumerable<IBarcodeService> barcodeServices)
        {
            _barcodeServices = barcodeServices;
        }

        public IBarcodeService GetBarcodeService(string barcodeImplementationName)
        {
            var service = _barcodeServices.FirstOrDefault(x => x.BarcodeImplementationName.Equals(barcodeImplementationName, StringComparison.OrdinalIgnoreCase));
            
            if (service == null)
                throw new ArgumentException($"Barcode implementation '{barcodeImplementationName}' not found. Available implementations: {string.Join(", ", GetAvailableImplementations())}");
            
            return service;
        }

        public IEnumerable<string> GetAvailableImplementations()
        {
            return _barcodeServices.Select(x => x.BarcodeImplementationName);
        }
    }
} 