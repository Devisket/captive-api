using Captive.Model.Dto;

namespace Captive.Fileprocessor.Services.Barcode
{
    public interface IBarcodeImplementationFactory
    {
        IBarcodeImplementationService GetBarcodeImplementation(string serviceName);
        IEnumerable<IBarcodeImplementationService> GetAllImplementations();
    }
} 