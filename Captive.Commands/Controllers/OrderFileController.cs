using Captive.Applications.OrderFile.Commands.UploadOrderFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFileController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderFileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadOrderFile([FromForm] Guid bankId, [FromForm] Guid batchId, [FromForm]IEnumerable<IFormFile> files) 
        {
            if (files.Any())
            {
                await _mediator.Send(new UploadOrderFileCommand()
                {
                    BankId = bankId,
                    BatchId = batchId,
                    Files = files
                }).ConfigureAwait(false);
            }
            else
            {
                return BadRequest("Files is empty");
            }

            return Ok();
        }
    }
}
