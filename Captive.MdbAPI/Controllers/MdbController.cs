using Captive.Data.UnitOfWork.Read;
using Captive.Model;
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
    public class MdbController : ControllerBase
    {
        private readonly IMDBFileProcessor _mdbProcessor;
        private readonly IReadUnitOfWork _readUow;
        public MdbController(IMDBFileProcessor mdbProcessor, IReadUnitOfWork readUow)
        {
            _mdbProcessor = mdbProcessor;
            _readUow = readUow;
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

            _mdbProcessor.Extractfile(request, extractedConfig);

            return Ok(new List<OrderFileData>());
        }
    }
}