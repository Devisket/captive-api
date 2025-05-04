using Captive.Data.UnitOfWork.Read;
using Captive.MdbAPI.Request;
using Captive.MdbProcessor.Processor.DbfGenerator;
using Captive.MdbProcessor.Processor.DbfProcessor;
using Captive.MdbProcessor.Processor.Interfaces;
using Captive.Model.Dto;
using Captive.Model.Processing.Configurations;
using Captive.Processing.Processor.MDBFileProcessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Captive.MdbAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorController : ControllerBase
    {
        private readonly IProcessor<MdbConfiguration> _mdbProcessor;
        private readonly IProcessor<DbfConfiguration> _dbfProcessor;
        private readonly IReadUnitOfWork _readUow;
        private readonly IDbfGenerator _dbfGenerator;

        public ProcessorController(IProcessor<MdbConfiguration> mdbProcessor, IProcessor<DbfConfiguration> dbfProcessor, IReadUnitOfWork readUow, IDbfGenerator dbfGenerator)
        {
            _mdbProcessor = mdbProcessor;
            _readUow = readUow;
            _dbfGenerator = dbfGenerator;

            _dbfProcessor = dbfProcessor;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<CheckOrderDto>>> ExtractMdb([FromBody] OrderfileDto request)
        {  
            var config = await _readUow.ProductConfigurations.GetAll().FirstOrDefaultAsync(x => request.FileName.Contains(x.FileName));

            if (config == null)
                throw new Exception("Null configuration");

            var extractedConfig = JsonConvert.DeserializeObject<MdbConfiguration>(config.ConfigurationData);

            if (extractedConfig == null)
                throw new Exception("Can't extract configuration");
            
            if (config == null) 
            {
                return Problem(detail:$"Can't find configuration for {request.FileName}", statusCode: 500);
            }

            var response = _mdbProcessor.Extractfile(request, extractedConfig);

            return Ok(response);
        }


        [HttpPost("dbf")]
        public async Task<ActionResult<IEnumerable<CheckOrderDto>>> ExtractDbf([FromBody] OrderfileDto request)
        {
            var config = await _readUow.ProductConfigurations.GetAll().FirstOrDefaultAsync(x => request.FileName.Contains(x.FileName));

            if (config == null)
                throw new Exception("Null configuration");

            var extractedConfig = JsonConvert.DeserializeObject<DbfConfiguration>(config.ConfigurationData);

            if (extractedConfig == null)
                throw new Exception("Can't extract configuration");

            if (config == null)
            {
                return Problem(detail: $"Can't find configuration for {request.FileName}", statusCode: 500);
            }

            var response = _dbfProcessor.Extractfile(request, extractedConfig);

            return Ok(response);
        }

        [HttpPost("GenerateDbf")]
        public async Task<ActionResult<IEnumerable<CheckOrderDto>>> GenerateDbf([FromBody] GenerateDbfRequest request, CancellationToken cancellationToken)
        {
            var orderFiles = await _readUow.OrderFiles.GetAll()
                    .Include(x => x.BatchFile)
                    .Include(x => x.CheckOrders)                        
                    .Include(x => x.CheckOrders)
                        .ThenInclude(x => x.CheckInventoryDetail)
                    .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => x.BatchFileId == request.batchId)
                .ToListAsync();

            if (orderFiles == null)
            {
                throw new SystemException($"Order file for batchID:{request.batchId} doens't exist");
            }

            await _dbfGenerator.GenerateDbf(orderFiles, cancellationToken);

            return Ok();
        }
    }
}