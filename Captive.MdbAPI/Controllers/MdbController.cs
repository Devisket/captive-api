using Captive.Data.UnitOfWork.Read;
using Captive.Processing.Processor.MDBFileProcessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("batchId")]
        public async Task<IActionResult> ExtractMdb([FromQuery]Guid batchId, [FromBody] string fileName)
        {  
            var config = await _readUow.ProductConfigurations.GetAll().FirstOrDefaultAsync(x => fileName.Contains(x.FileName));
            
            if (config == null) 
            {
                return Problem(detail:$"Can't find configuration for {fileName}", statusCode: 500);
            }

            return Ok();
        }
    }
}
