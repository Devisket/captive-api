using Captive.Model.Dto;

namespace Captive.Orchestrator.Services.Barcode
{
    public class BarcodeImplementationFactory : IBarcodeImplementationFactory
    {
        private readonly IEnumerable<IBarcodeImplementationService> _implementations;

        public BarcodeImplementationFactory(IEnumerable<IBarcodeImplementationService> implementations)
        {
            _implementations = implementations;
        }

        public IBarcodeImplementationService GetBarcodeImplementation(string serviceName)
        {
            var implementation = _implementations.FirstOrDefault(x => 
                x.BarcodeServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            if (implementation == null)
            {
                throw new InvalidOperationException($"No barcode implementation found for service name: {serviceName}");
            }

            return implementation;
        }

        public IEnumerable<IBarcodeImplementationService> GetAllImplementations()
        {
            return _implementations;
        }
    }
} 