using Captive.Model.Dto.Reports;

namespace Captive.Barcode.Base
{
    public interface IBarcodeService
    {
        public string BarcodeImplementationName { get;}
        public Task<string> GenerateBarcode(CheckOrderReport param);
        public Task<string> GenerateBarcodeAsync(CheckOrderReport param);
    }
}
