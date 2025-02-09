using Captive.Applications.Batch.Query.GetBatchById;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Model.Dto;
using Captive.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mysqlx.Crud;
using Newtonsoft.Json;

namespace Captive.Applications.Batch.Services
{
    public interface IBatchService
    {
        Task<GetBatchByIdQueryResponse> GetBatchDetailById(Guid batchId);
    }
    public class BatchService : IBatchService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IConfiguration _configuration;

        public BatchService(IReadUnitOfWork readUow, IConfiguration configuration)
        {
            _readUow = readUow;
            _configuration = configuration;
        }

        public async Task<GetBatchByIdQueryResponse> GetBatchDetailById(Guid batchId)
        {
            var batch = await _readUow.BatchFiles.GetAll()
                .Where(x => x.Id == batchId)
                .Select(x => new GetBatchByIdQueryResponse()
                {
                    Id = x.Id,
                    BatchFileStatus = x.BatchFileStatus,
                    BatchName = x.BatchName,
                    CreatedDate = x.CreatedDate,
                    OrderNumber = x.OrderNumber,
                    OrderFiles = x.OrderFiles != null && x.OrderFiles.Any() ? x.OrderFiles.Select(x => new OrderfileDto
                    {
                        Id = x.Id,
                        BatchId = x.BatchFileId,
                        FileName = x.FileName,
                        FileType = Path.GetExtension(x.FileName).SanitizeFileName(),
                        FilePath = x.FilePath,
                        Status = x.Status.ToString(),
                        CheckOrders = x.FloatingCheckOrders != null && x.FloatingCheckOrders.Any() ? x.FloatingCheckOrders.Select(c => new CheckOrderDto
                        {
                            Id = c.Id,
                            BRSTN = c.BRSTN,
                            AccountNumber = c.AccountNo,
                            MainAccountName = c.AccountName,
                            DeliverTo = c.DeliverTo,
                            FormType = c.FormType,
                            CheckType = c.CheckType,
                            Quantity = c.Quantity,
                        }).ToList() : null
                    }).ToList() : null
                }).FirstOrDefaultAsync();

            if (batch == null)
                throw new SystemException($"Batch ID {batchId} doesn't exist");

            return batch;
        }
    }
}
