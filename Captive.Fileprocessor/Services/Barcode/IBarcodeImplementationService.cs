﻿using Captive.Model.Dto;

namespace Captive.Fileprocessor.Services.Barcode
{
    public interface IBarcodeImplementationService
    {
        public string BarcodeServiceName { get; }
        public Task<IEnumerable<UpdateCheckOrderBarcodeDto>> GenerateBarcode(Guid bankid, Guid batchId, IEnumerable<CheckOrderBarcodeDto> checkOrders);
    }
}
