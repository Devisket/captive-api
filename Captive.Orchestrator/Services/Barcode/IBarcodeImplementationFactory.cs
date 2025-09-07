using Captive.Model.Dto;

namespace Captive.Orchestrator.Services.Barcode
{
    public interface IBarcodeImplementationFactory
    {
        IBarcodeImplementationService GetBarcodeImplementation(string serviceName);
        IEnumerable<IBarcodeImplementationService> GetAllImplementations();
    }
} 