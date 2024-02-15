using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor;
using Captive.Processing.Processor.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Captive.Applications.OrderFiles.Commands.UploadOrderFile
{


    /**
     * STEPS
     * 1. Upload file
     *[x]2. Interpret file into OrderFileData DTO
     *[x] 3. Check it's CheckType and FromType?
     *[x] 4. Check available check inventory?
     * []5. Applied series
     * []6. Print printer file
     **/
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
    {
        private readonly IFileProcessor _fileProcessor;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public UploadOrderFileCommandHandler(IFileProcessor fileProcessor, IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _fileProcessor = fileProcessor;
            _readUow = readUow;
            _writeUow = writeUow;
        }


        public async Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            var orderfileConfiguration = _readUow.OrderFileConfigurations.GetAll().FirstOrDefault(x=> String.Equals(x.Name, request.FileName));

            if(orderfileConfiguration == null) 
            {
                throw new ArgumentException("File is empty");
            }

            var file = request.OrderFile;

            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("File is empty");
            }

            var checkOrders = _fileProcessor.OnProcessFile(file, orderfileConfiguration.ConfigurationData);


            if (await ValidateOrderFileData(checkOrders))
            {

                await _writeUow.OrderFiles.Add(new Data.Models.OrderFiles
                {
                    BatchName = request.FileName,
                    ProcessDate = DateTime.UtcNow,
                });

                await _writeUow.Complete();

                var batchFile = _readUow.OrderFiles.GetAll().Where(x=> x.BatchName ==  request.FileName).FirstOrDefault();

                if(batchFile == null) 
                {
                    throw new Exception($"Batch file doesn't exists");
                }

                foreach (OrderFileData fileData in checkOrders)
                {
                    await _writeUow.CheckOrders.Add(new Data.Models.CheckOrders
                    {
                        AccountNo = fileData.AccountNumber,
                        BRSTN = fileData.BRSTN,
                        DeliverTo = fileData.DeliverTo,
                        OrderFileId = batchFile.Id
                    });

                    await _writeUow.Complete();

                }
            }

            return Unit.Value;
        }


        private async Task<bool> ValidateOrderFileData(ICollection<OrderFileData> orderFileDatas)
        {
            if(orderFileDatas == null || !orderFileDatas.Any()) 
            {
                return false;
            }

            var summary = orderFileDatas.GroupBy(x => new { x.FormType, x.CheckType })
                .Select(z => new
                {
                    z.Key,
                    count = z.Sum(c=> c.Quantity)
                }).ToList() ;

            for(int i =0; i< summary.Count(); i++)
            {
                var formCheck = _readUow.FormChecks.Find(x => x.CheckType == summary[i].Key.CheckType && x.FormType == summary[i].Key.FormType).FirstOrDefault();

                if (formCheck == null)
                {
                    throw new Exception($"Missing Form/Check type for: CHECKTYPE ({summary[i].Key.CheckType}) FORMTYPE({summary[i].Key.FormType})");
                }

                var checkInventoryCount = await _readUow.CheckInventory.GetAll().Where(x=> x.FormCheckId == formCheck.Id).CountAsync();

                if (summary[i].count > checkInventoryCount)
                {
                    throw new Exception($"Check inventory quantity is not enough for {formCheck.Description}");
                }
            }

            foreach(OrderFileData fileData in orderFileDatas)
            {
                var formCheck  = _readUow.FormChecks.Find(x=> x.CheckType == fileData.CheckType && x.FormType == fileData.FormType).FirstOrDefault();

                if (formCheck == null)
                {
                    throw new Exception($"Missing Form/Check type for: CHECKTYPE ({fileData.CheckType}) FORMTYPE({fileData.FormType})");
                }

                if (!await _readUow.BankBranches.GetAll().AnyAsync(x => x.BRSTNCode == fileData.BRSTN))
                {
                    throw new Exception($"BRSTN: {fileData.BRSTN} doesn't exists");
                }
                
            }

            return true;
        }
    }
}
