using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Models;
using Captive.Messaging.Producers;
using Captive.Processing.Processor.ExcelFileProcessor;
using Captive.Processing.Processor.Model;
using Captive.Processing.Processor.TextFileProcessor;
using Captive.Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading;

namespace Captive.Applications.OrderFile.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
    {
        //private readonly ITextFileProcessor _textFileProcessor;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;
        //private readonly IReportGenerator _reportGenerator;
        //private readonly IExcelFileProcessor _excelFileProcessor;

        //public UploadOrderFileCommandHandler(
        //    ITextFileProcessor fileProcessor,
        //    IExcelFileProcessor excelFileProcessor,
        //    IReadUnitOfWork readUow, 
        //    IWriteUnitOfWork writeUow, 
        //    IReportGenerator reportGenerator)
        //{
        //    _textFileProcessor = fileProcessor;
        //    _readUow = readUow;
        //    _writeUow = writeUow;
        //    _reportGenerator = reportGenerator;
        //    _excelFileProcessor = excelFileProcessor;
        //}
        
        private readonly IProducer<FileUploadMessage> _producer;

        public UploadOrderFileCommandHandler(
            IReadUnitOfWork readUow, 
            IWriteUnitOfWork writeUow,  
            IProducer<FileUploadMessage> producer) 
        {
            _readUow = readUow;
            _writeUow = writeUow;
            _producer = producer;            
        }


        /*
         * TODO: 
         *  a. Handle file upload
         *  b. Check if the file is encrypted or not 
         */
        public async Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            var bankInfo = await _readUow.Banks.GetAll().FirstOrDefaultAsync(x => x.Id == request.BankId);

            if (bankInfo == null)
            {
                throw new Exception($"the bankId: {request.BankId} doesn't exist");
            }

            var batch = new BatchFile
            {
                Id = Guid.NewGuid(),
                BatchName = "sample", // <- Should we generate batch name? If yes what should be the format?
                BankInfoId = bankInfo.Id,
                BankInfo = bankInfo,
                BatchFileStatus = BatchFileStatus.Pending,
            };

            _producer.ProduceMessage(new FileUploadMessage
            {
                BankId = request.BankId,
                BatchID = Guid.NewGuid(),
                Files = new string[] { "sample" }
            });
            //var bankInfo = await GetBankInfo(request.BankId);

            //if (bankInfo == null)
            //    throw new Exception($"Bank Id {request.BankId} doesn't exist");

            //var bankBranches = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankId == bankInfo.Id).ToListAsync(cancellationToken);

            //var batchFile = await CreateBatchFile(bankInfo, cancellationToken);

            //var orders = await CreateOrderFileAsync(request.Files, batchFile, cancellationToken);

            //foreach(var order in orders) 
            //{
            //    var orderFile = order.Item1;
            //    var file = order.Item2;

            //    //Get configuration
            //    var configuration = await GetOrderFileConfigurationAsync(orderFile,bankInfo.ShortName, cancellationToken);
                
            //    if(configuration == null)
            //    {
            //        await WriteOrderFileLog(orderFile, $"Order file {orderFile.FileName} configuration doesn't exist", LogType.Error, cancellationToken);

            //        await SetOrderFileStatus(orderFile, OrderFilesStatus.Error ,cancellationToken);

            //        continue;
            //    }

            //    var formChecks = await GetFormCheck(configuration, cancellationToken);

            //    //Process file
            //    var orderFileDatas =  configuration.ConfigurationType == ConfigurationType.TextConfiguration ? _textFileProcessor.OnProcessFile(file, configuration.ConfigurationData) : 
            //        _excelFileProcessor.OnProcessFile(file,configuration.ConfigurationData);

            //    if (!await ValidateOrderFileData(orderFile, orderFileDatas, bankInfo,formChecks, configuration.ConfigurationType, cancellationToken))
            //    {
            //        await SetOrderFileStatus(orderFile, OrderFilesStatus.Error, cancellationToken);

            //        continue;
            //    }
            //    //TODO
            //    //Create a check inventory for those file data that has starting series
            //    foreach (OrderFileData orderFileData in orderFileDatas)
            //    {
            //        var formCheck = formChecks.First(x => x.FormType == orderFileData.FormType && x.CheckType == orderFileData.CheckType);

            //        var bankBranch = bankInfo.BankBranches.First(x => x.BRSTNCode == orderFileData.BRSTN);

            //        var checkOrder = new CheckOrders
            //        {
            //            AccountNo = orderFileData.AccountNumber,
            //            BRSTN = orderFileData.BRSTN,
            //            DeliverTo = orderFileData.DeliverTo,
            //            OrderFileId = orderFile.Id,
            //            Concode = orderFileData.Concode,
            //            FormCheckId = formCheck.Id,
            //            OrderQuanity = orderFileData.Quantity,
            //            AccountName = orderFileData.AccountName
            //        };

            //        await InsertCheckOrder(checkOrder, cancellationToken);
            //        if (!String.IsNullOrEmpty(orderFileData.StartingSeries))
            //        {
            //            await InsertSeriesCheck(formCheck, bankBranch, checkOrder, orderFileData.StartingSeries, cancellationToken);
            //        }
            //        else
            //        {
            //            await ApplyCheckInventory(checkOrder, bankBranch, cancellationToken);
            //        }
                    
            //    }
                
            //    await SetOrderFileStatus(orderFile, OrderFilesStatus.Completed, cancellationToken);
            //}

            //await UpdateBatchFile(batchFile, BatchFileStatus.Success);

            //await _reportGenerator.OnGenerateReport(batchFile.Id, cancellationToken);

            return Unit.Value;
        }
        //private async Task ApplyCheckInventory(CheckOrders checkOrder, BankBranches branch, CancellationToken cancellationToken, string startingSeries = "")
        //{
        //    if(!String.IsNullOrEmpty(startingSeries))
        //    {
               
        //    }

        //    for (int i = 0; i < checkOrder.OrderQuanity; i++)
        //    {
        //        var checkInventory = await _readUow.CheckInventory.GetAll()
        //        .Where(x => x.FormCheckId == checkOrder.FormCheckId && x.BranchId == branch.Id)
        //        .FirstAsync(x => !x.CheckOrderId.HasValue, cancellationToken);

        //        checkInventory.CheckOrderId = checkOrder.Id;

        //        _writeUow.CheckInventory.Update(checkInventory);

        //        await _writeUow.Complete();

        //    }
        //}
        //private async Task<BankInfo?>GetBankInfo(Guid bankId)
        //{
        //    return await _readUow.Banks.GetAll()
        //        .Include(x => x.BankBranches)
        //        .FirstOrDefaultAsync(x => x.Id == bankId);
        //}

        //private async Task<ICollection<Captive.Data.Models.FormChecks>> GetFormCheck(ProductConfiguration productConfiguration, CancellationToken cancellationToken)
        //{
        //    var formChecks = await _readUow.FormChecks.GetAll()
        //        .Include(x => x.ProductType)
        //        .Where(x => x.ProductTypeId == productConfiguration.ProductTypeId).ToListAsync();

        //    return formChecks;
        //}

        //private async Task<IEnumerable<Tuple<Data.Models.OrderFile, byte[]>>> CreateOrderFileAsync(IEnumerable<IFormFile> rawFiles, BatchFile batchFile, CancellationToken cancellationToken)
        //{
        //    var orderFiles = new List<Tuple<Data.Models.OrderFile, byte[]>>();

        //    foreach (IFormFile file in rawFiles)
        //    {
        //        var newOrderFile = new Data.Models.OrderFile
        //        {
        //            BatchFile = batchFile,
        //            BatchFileId = batchFile.Id,
        //            FileName = file.FileName,
        //            ProcessDate = DateTime.UtcNow,
        //            Status = OrderFilesStatus.Pending
        //        };

        //        await _writeUow.OrderFiles.AddAsync(newOrderFile, cancellationToken);

        //        var fileByte = await ExtractFile(file, cancellationToken);

        //        orderFiles.Add(new Tuple<Data.Models.OrderFile, byte[]>(newOrderFile, fileByte));
        //    }

        //    await _writeUow.Complete();

        //    return orderFiles;
        //}

        //private async Task<byte[]> ExtractFile(IFormFile rawFile, CancellationToken cancellationToken)
        //{
        //    using (var fileStream = rawFile.OpenReadStream())
        //    {
        //        byte[] fileBytes = new byte[fileStream.Length];
        //        await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length, cancellationToken);
        //        fileStream.Close();

        //        return fileBytes;
        //    }
        //}

        //private async Task<ProductConfiguration?>GetOrderFileConfigurationAsync(Data.Models.OrderFile orderFile, string bankShortName, CancellationToken cancellationToken)
        //{ 
        //    var absoluteName = Path.GetFileNameWithoutExtension(orderFile.FileName);

        //    absoluteName = Regex.Replace(absoluteName, @"[\d-_.]", String.Empty);

        //    absoluteName = absoluteName.Replace(bankShortName, string.Empty);

        //    absoluteName = absoluteName.Replace("Order", string.Empty);

        //    var config = await _readUow.ProductConfigurations.GetAll()
        //        .Where(x => !string.IsNullOrEmpty(x.FileName))
        //        .Where(x => x.FileName == absoluteName || EF.Functions.Like(x.FileName, $"%{absoluteName}%"))
        //        .FirstOrDefaultAsync(cancellationToken);

        //    return config;
        //}

        //private async Task SetOrderFileStatus(Data.Models.OrderFile orderFile, OrderFilesStatus status, CancellationToken cancellationToken)
        //{
        //    /*
        //     * TODO:
        //     * Update updated date as well
        //     */

        //    orderFile.Status = status;
           
        //    _writeUow.OrderFiles.Update(orderFile);

        //    await _writeUow.Complete(cancellationToken);
        //}

        //private async Task<bool> ValidateOrderFileData(Data.Models.OrderFile orderFile, ICollection<OrderFileData> orderFileDatas, BankInfo bankInfo, ICollection<Captive.Data.Models.FormChecks> formChecks, 
        //    ConfigurationType configurationType, CancellationToken cancellationToken)
        //{
        //    var validationStatus = true;
        //    var bankBranches = await _readUow.BankBranches.GetAll().AsNoTracking().Where(x => x.BankId == bankInfo.Id).ToListAsync(cancellationToken);

        //    var fileBRSTNs = orderFileDatas.Select(x => x.BRSTN).Distinct().ToList();

        //    if (orderFileDatas == null || !orderFileDatas.Any())
        //    {
        //        return false;
        //    }

        //    //Check if there is any bank branches
        //    if (!bankBranches.Any())
        //    {
        //        await WriteOrderFileLog(
        //                orderFile,
        //                $"There is no any branch for bank: ${bankInfo.BankName}",
        //                LogType.Error,
        //                cancellationToken);

        //        return false;
        //    }

        //   //Validate BRSTN
        //    foreach (var brstn in fileBRSTNs)
        //    {
        //        if (!bankBranches.Any(x => x.BRSTNCode == brstn))
        //        {
        //            await WriteOrderFileLog(
        //                orderFile,
        //                $"There is no BRSNT: {brstn} for bank: {bankInfo.BankName}",
        //                LogType.Error,
        //                cancellationToken);

        //            validationStatus = false;
        //        }
        //    }

        //    var orderFileGroups = orderFileDatas.GroupBy(x => new { x.FormType, x.CheckType })
        //        .Select(z => new
        //        {
        //            z.Key,
        //            count = z.Sum(c=> c.Quantity)
        //        }).ToList() ;

        //    //Check form and check type mapping
        //    for(int i =0; i< orderFileGroups.Count(); i++)
        //    {
        //        var formCheck = formChecks
        //            .FirstOrDefault(x=> x.FormType == orderFileGroups[i].Key.FormType 
        //            && x.CheckType == orderFileGroups[i].Key.CheckType);

        //        if (formCheck == null)
        //        {
        //            await WriteOrderFileLog(
        //                orderFile, 
        //                $"Missing mapping for check-type:{orderFileGroups[i].Key.CheckType}, form-type:{orderFileGroups[i].Key.FormType} for file {orderFile.FileName}", 
        //                LogType.Error, 
        //                cancellationToken);

        //            validationStatus = false;
        //        }
        //    }

        //    var orderFileInvGroup = orderFileDatas.GroupBy(x => new { x.FormType, x.CheckType, x.BRSTN })
        //        .Select(z => new
        //        {
        //            z.Key,
        //            count = z.Sum(c => c.Quantity)
        //        }).ToList();



        //    if(configurationType == ConfigurationType.ExcelConfiguration)
        //    {
        //        return validationStatus;
        //    }

        //    //Check inventory availability
        //    foreach (var invGroup in orderFileInvGroup)
        //    {
        //        var formCheck = formChecks
        //            .FirstOrDefault(x => x.FormType == invGroup.Key.FormType
        //            && x.CheckType == invGroup.Key.CheckType);

        //        var branch = bankBranches.FirstOrDefault(x => x.BRSTNCode == invGroup.Key.BRSTN);

        //        var checkInventoryCount = await _readUow.CheckInventory.GetAll()
        //            .AsNoTracking()
        //            .Where(x => x.FormChecks == formCheck && x.BankBranch == branch && !x.CheckOrderId.HasValue)
        //            .CountAsync();

        //        if (invGroup.count > checkInventoryCount)
        //        {
        //            await WriteOrderFileLog(
        //                orderFile,
        //                $"Check inventory quantity is not enough for  {formCheck?.ProductType.ProductName} - {formCheck?.Description} for BRSTN: {invGroup.Key.BRSTN}",
        //                LogType.Error,
        //                cancellationToken);

        //            validationStatus = false;
        //        }
        //    }

        //    return validationStatus;
        //}

        //private async Task WriteOrderFileLog(Data.Models.OrderFile orderFile, string message, LogType logType, CancellationToken cancellationToken = default)
        //{
        //    await _writeUow.OrderFileLogs.AddAsync(new OrderFileLog
        //    {
        //        LogType = logType,
        //        LogDate = DateTime.UtcNow,
        //        LogMessage = message,
        //        OrderFileId = orderFile.Id,
        //        OrderFile = orderFile,
        //    }, cancellationToken);

        //    await _writeUow.Complete();
        //}  

        //private async Task<CheckOrders> InsertCheckOrder(CheckOrders checkOrder, CancellationToken cancellationToken) { 
        
        //    await _writeUow.CheckOrders.AddAsync(checkOrder,cancellationToken);
        //    await _writeUow.Complete();

        //    return checkOrder;
        //}

        //private async Task<BatchFile> CreateBatchFile(BankInfo bankInfo, CancellationToken cancellationToken)
        //{
        //    int orderNumber = 0;

        //    var prevBatch = await _readUow.BatchFiles
        //        .GetAll()
        //        .Where(x => x.UploadDate.Date == DateTime.UtcNow.Date)
        //        .OrderBy(x => x.OrderNumber)
        //        .LastOrDefaultAsync();

        //    if (prevBatch == null)
        //        orderNumber = 1;
        //    else
        //        orderNumber = prevBatch.OrderNumber + 1;

        //    var batchFile = new BatchFile
        //    {
        //        BatchName = string.Format("{0}_{1}_{2}", bankInfo.ShortName, DateTime.UtcNow.ToString("MM-dd-yy"), orderNumber),
        //        OrderNumber = orderNumber,
        //        BankInfoId = bankInfo.Id,
        //        BatchFileStatus = BatchFileStatus.Pending,
        //        UploadDate = DateTime.UtcNow
        //    };

        //    if(bankInfo.BatchFiles == null)
        //        bankInfo.BatchFiles = new List<BatchFile> { batchFile };
        //    else
        //        bankInfo.BatchFiles.Add(batchFile);

        //    _writeUow.BankInfo.Update(bankInfo);

        //    await _writeUow.Complete(cancellationToken);

        //    return batchFile;
        //}

        //private async Task UpdateBatchFile(BatchFile batchFile, BatchFileStatus status )
        //{
        //    batchFile.BatchFileStatus = status;

        //    _writeUow.BatchFiles.Update(batchFile);

        //    await _writeUow.Complete();
        //}

        //private async Task InsertSeriesCheck(Captive.Data.Models.FormChecks formCheck, BankBranches bankBranch, CheckOrders checkOrder, string startingSeries, CancellationToken cancellationToken)
        //{

        //    var nextSeries = int.Parse(startingSeries);

        //    for(var i = 0; i < checkOrder.OrderQuanity; i++)
        //    {
        //        var endingSeries = nextSeries + formCheck.Quantity;
        //        endingSeries -= 1;

        //        var checkInventory = new Data.Models.CheckInventory
        //        {
        //            StarSeries = String.Format("{0:D8}", nextSeries),
        //            EndSeries = String.Format("{0:D8}", endingSeries),
        //            FormCheckId = formCheck.Id,
        //            Quantity = formCheck.Quantity,
        //            CheckOrderId = checkOrder.Id,
        //            BranchId = bankBranch.Id,
        //        };

        //        await _writeUow.CheckInventory.AddAsync(checkInventory, cancellationToken);
        //        nextSeries = endingSeries + 1;
        //    }
        //}
    }
}
