using Captive.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Fileprocessor.Services.GenerateBarcodeService
{
    public interface IGenerateBarcodeService
    {
        public Task GenerateBarcode(Guid bankId, Guid batchId, string barcodeServiceName, IEnumerable<CheckOrderBarcodeDto> checkOrders);
    }
}
