using Captive.Data.Enums;
using Captive.Fileprocessor.Services.CheckOrderService;
using Captive.Messaging.Models;
using Captive.Model.Dto;
using Captive.Utility;

namespace Captive.Fileprocessor.Services.FileProcessOrchestrator.cs
{
    class FileProcessOrchestratorService : IFileProcessOrchestratorService
    {
        private readonly ICheckOrderService _checkOrderService;
        public FileProcessOrchestratorService(ICheckOrderService checkOrderService)
        {
            _checkOrderService = checkOrderService;
        }

        public async Task ProcessFile(FileUploadMessage message)
        {
            var files = message.Files;
            foreach (var file in files)
            {
                try
                {
                    List<CheckOrderDto> checkOrders = new List<CheckOrderDto>();

                    var fileExtension = Path.GetExtension(file.FileName);

                    fileExtension = fileExtension.SanitizeFileName();

                    switch (fileExtension)
                    {
                        case ".txt":
                            break;
                        case ".xlsx":
                            break;
                        case ".mdb":
                            checkOrders = await _checkOrderService.ExtractMdb(file);
                            break;
                        default:
                            break;
                    }

                    if (checkOrders == null || !checkOrders.Any())
                    {
                        await _checkOrderService.SendOrderFileStatus(file.Id, "Can't map the check orders for this file", OrderFilesStatus.Error);
                        continue;
                    }

                    //Create Floating Check Order
                    await _checkOrderService.CreateFloatingCheckOrder(file.Id, message.BankId, checkOrders);

                    await _checkOrderService.SendOrderFileStatus(file.Id, OrderFilesStatus.Pending);
                }
                catch (Exception ex)
                {
                    await _checkOrderService.SendOrderFileStatus(file.Id, ex.Message, OrderFilesStatus.Error);
                    continue;
                }
            }
            //await _checkOrderService.GenerateReport(message.BatchID);
        }
    }
}
