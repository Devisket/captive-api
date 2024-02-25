using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor;
using Captive.Processing.Processor.Model;
using Captive.Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Captive.Applications.OrderFile.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
    {
        private readonly IFileProcessor _fileProcessor;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        private readonly IPrinterFileReport _printerFileReport;

        public UploadOrderFileCommandHandler(IFileProcessor fileProcessor, IReadUnitOfWork readUow, IWriteUnitOfWork writeUow, IPrinterFileReport printerFileReport)
        {
            _fileProcessor = fileProcessor;
            _readUow = readUow;
            _writeUow = writeUow;
            _printerFileReport = printerFileReport;
        }

        public async Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            /*
             * TODO ON THIS FUNCTION:
             * --[] FILE PROCESSING
             * |_ [] - Support excel file interpretation
             * --[X] APPLY CHECK ORDERS INTO CHECK INVENTORY
             * --[] GENERATE REPORTS
             * |_ [X] CREATE CONFIGURATION FOR FILE PATHING
             * |_ [X] CREATE PRINTER FILE
             * |- [] CREATE SUMMARY REPORT FILE
             */


            var orders = await CreateOrderFileAsync(request.Files, cancellationToken);

            foreach(var order in orders) 
            {
                var orderFile = order.Item1;
                var file = order.Item2;

                //Get configuration
                var configuration = await GetOrderFileConfigurationAsync(orderFile, cancellationToken);

                if(configuration == null)
                {
                    await WriteOrderFileLog(orderFile, $"Order file {orderFile.FileName} configuration doesn't exist", LogType.Error, cancellationToken);

                    await SetOrderFileStatus(orderFile, OrderFilesStatus.Error ,cancellationToken);

                    continue;
                }

                /*
                 * TODO ON THIS LINE:
                 * [] - Support excel file interpretation
                 */
                //Process file
                var orderFileDatas = _fileProcessor.OnProcessFile(file, configuration.ConfigurationData);

                if (!await ValidateOrderFileData(orderFile, orderFileDatas, configuration.Bank, configuration, cancellationToken))
                {
                    await SetOrderFileStatus(orderFile, OrderFilesStatus.Error, cancellationToken);

                    continue;
                }

                var formChecks = _readUow.FormChecks.GetAll().AsNoTracking().Where(x => x.Bank == configuration.Bank);

                foreach (OrderFileData orderFileData in orderFileDatas)
                {
                    var formCheck = GetFormCheck(orderFileData, configuration, cancellationToken);

                    var checkOrder = new CheckOrders
                    {
                        AccountNo = orderFileData.AccountNumber,
                        BRSTN = orderFileData.BRSTN,
                        DeliverTo = orderFileData.DeliverTo,
                        OrderFileId = orderFile.Id,
                        FormCheckId = formCheck.Id,
                        OrderQuanity = orderFileData.Quantity,
                        AccountName = orderFileData.AccountName
                    };

                    await InsertCheckOrder(checkOrder, cancellationToken);

                    await ApplyCheckInventory(checkOrder, cancellationToken);
                }
                /*
                 * TODO ON THIS LINE: 
                 * [X] - Apply check orders into check inventory
                 * [] - Create printer file report
                 * [] - Create a summary file report
                 */
                await _printerFileReport.GenerateReport(orderFile, configuration.Bank, cancellationToken);

                await SetOrderFileStatus(orderFile, OrderFilesStatus.Completed, cancellationToken);
            }

            return Unit.Value;
        }
        private async Task<IEnumerable<Tuple<Data.Models.OrderFile, byte[]>>> CreateOrderFileAsync(IEnumerable<IFormFile> rawFiles, CancellationToken cancellationToken)
        {
            var orderFiles = new List<Tuple<Data.Models.OrderFile, byte[]>>();

            foreach (IFormFile file in rawFiles)
            {
                var newOrderFile = new Data.Models.OrderFile
                {
                    FileName = file.FileName,
                    ProcessDate = DateTime.UtcNow,
                    Status = OrderFilesStatus.Pending
                };

                await _writeUow.OrderFiles.AddAsync(newOrderFile, cancellationToken);

                var fileByte = await ExtractFile(file, cancellationToken);

                orderFiles.Add(new Tuple<Data.Models.OrderFile, byte[]>(newOrderFile, fileByte));
            }

            await _writeUow.Complete();

            return orderFiles;
        }
        private async Task<byte[]> ExtractFile(IFormFile rawFile, CancellationToken cancellationToken)
        {
            using (var fileStream = rawFile.OpenReadStream())
            {
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length, cancellationToken);
                fileStream.Close();

                return fileBytes;
            }
        }
        private async Task<OrderFileConfiguration?>GetOrderFileConfigurationAsync(Data.Models.OrderFile orderFile, CancellationToken cancellationToken)
        {
            var absoluteName = Regex.Replace(orderFile.FileName, @"[\d-]", String.Empty);
            
            absoluteName = Path.GetFileNameWithoutExtension(absoluteName);

            var config = await _readUow.OrderFileConfigurations.GetAll()
                .Include(x => x.Bank)
                .AsNoTracking()
                .Where(x => x.Name == absoluteName)
                .FirstOrDefaultAsync(cancellationToken);

            return config;
        }
        private async Task SetOrderFileStatus(Data.Models.OrderFile orderFile, OrderFilesStatus status, CancellationToken cancellationToken)
        {
            /*
             * TODO:
             * Update updated date as well
             */

            orderFile.Status = status;
           
            _writeUow.OrderFiles.Update(orderFile);

            await _writeUow.Complete(cancellationToken);
        }
        private async Task<bool> ValidateOrderFileData(Data.Models.OrderFile orderFile, ICollection<OrderFileData> orderFileDatas, BankInfo bankInfo, OrderFileConfiguration configuration, CancellationToken cancellationToken)
        {
            if(orderFileDatas == null || !orderFileDatas.Any()) 
            {
                return false;
            }

            var summaryFormCheck = orderFileDatas.GroupBy(x => new { x.FormType, x.CheckType })
                .Select(z => new
                {
                    z.Key,
                    count = z.Sum(c=> c.Quantity)
                }).ToList() ;

            for(int i =0; i< summaryFormCheck.Count(); i++)
            {
                var formCheck = await GetFormCheck(summaryFormCheck[i].Key.FormType, summaryFormCheck[i].Key.CheckType, configuration, cancellationToken);

                if (formCheck == null)
                {
                    await WriteOrderFileLog(
                        orderFile, 
                        $"Missing mapping for check-type:{summaryFormCheck[i].Key.CheckType}, form-type:{summaryFormCheck[i].Key.FormType} for file {orderFile.FileName}", 
                        LogType.Error, 
                        cancellationToken);

                    return false;
                }

                var checkInventoryCount = await _readUow.CheckInventory.GetAll()
                    .AsNoTracking()
                    .Where(x=> x.FormCheckId == formCheck.Id)
                    .CountAsync();

                if (summaryFormCheck[i].count > checkInventoryCount)
                {
                    await WriteOrderFileLog(
                        orderFile,
                        $"Check inventory quantity is not enough for  {formCheck.ProductType.ProductName} - {formCheck.Description}",
                        LogType.Error,
                        cancellationToken);

                    return false;
                }
            }

            var bankBranches = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankId == bankInfo.Id).ToListAsync(cancellationToken);
            var fileBRSTNs = orderFileDatas.Select(x => x.BRSTN).Distinct().ToList();

            if (!bankBranches.Any())
            {
                await WriteOrderFileLog(
                        orderFile,
                        $"There is no any branch for bank: ${bankInfo.BankName}",
                        LogType.Error,
                        cancellationToken);

                return false;
            }

            foreach ( var brstn in fileBRSTNs)
            {
                if(!bankBranches.Any(x => x.BRSTNCode == brstn))
                {
                    await WriteOrderFileLog(
                        orderFile,
                        $"There is no BRSNT: {brstn} for bank: {bankInfo.BankName}",
                        LogType.Error,
                        cancellationToken);

                    return false;
                }
            }

            return true;
        }
        private async Task WriteOrderFileLog(Data.Models.OrderFile orderFile, string message, LogType logType, CancellationToken cancellationToken = default)
        {
            await _writeUow.OrderFileLogs.AddAsync(new OrderFileLog
            {
                LogType = logType,
                LogDate = DateTime.UtcNow,
                LogMessage = message,
                OrderFileId = orderFile.Id,
                OrderFile = orderFile,
            }, cancellationToken);

            await _writeUow.Complete();
        }  
        
        private async Task<CheckOrders> InsertCheckOrder(CheckOrders checkOrder, CancellationToken cancellationToken) { 
        
            await _writeUow.CheckOrders.AddAsync(checkOrder,cancellationToken);
            await _writeUow.Complete();

            return checkOrder;
        }
        private async Task ApplyCheckInventory(CheckOrders checkOrder, CancellationToken cancellationToken)
        {
           for(int i = 0; i < checkOrder.OrderQuanity; i++)
            {
                var checkInventory = await _readUow.CheckInventory.GetAll()
                .Where(x => x.FormCheckId == checkOrder.FormCheckId)
                .FirstAsync(x => !x.CheckOrderId.HasValue, cancellationToken);

                checkInventory.CheckOrderId = checkOrder.Id;

                _writeUow.CheckInventory.Update(checkInventory);

                await _writeUow.Complete();

            }
        }

        private async Task<FormChecks?>GetFormCheck(OrderFileData orderFileData, OrderFileConfiguration orderFileConfiguration, CancellationToken cancellationToken)
        {
            var productConfiguration = await _readUow.ProductConfigurations
                .GetAll()
                .Where(x => x.OrderFileConfigurationId == orderFileConfiguration.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (productConfiguration == null)
                return null;


            if(productConfiguration.ProductType.FormChecks == null || !productConfiguration.ProductType.FormChecks.Any())
                return null;

            var formCheck = productConfiguration.ProductType.FormChecks
                .Where(x=>x.FormType == orderFileData.FormType && x.CheckType == orderFileData.CheckType)
                .FirstOrDefault();

            return formCheck;
        }

        private async Task<FormChecks?> GetFormCheck(string FormType, string CheckType, OrderFileConfiguration orderFileConfiguration, CancellationToken cancellationToken)
        {
            var productConfiguration = await _readUow.ProductConfigurations
                .GetAll()
                .Where(x => x.OrderFileConfigurationId == orderFileConfiguration.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (productConfiguration == null)
                return null;


            if (productConfiguration.ProductType.FormChecks == null || !productConfiguration.ProductType.FormChecks.Any())
                return null;

            var formCheck = productConfiguration.ProductType.FormChecks
                .Where(x => x.FormType == FormType && x.CheckType == CheckType)
                .FirstOrDefault();

            return formCheck;
        }
    }
}
