using Captive.Applications.Orderfiles.Command.UpdateOrderFile;
using Captive.Applications.ProcessOrderFiles.Commands.UploadOrderFile;
using Captive.Data.Enums;
using Captive.Model.Request;
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

        [HttpPost("{id}/updateStatus")]
        public async Task<IActionResult> UpdateOrderfile([FromRoute] Guid bankId, [FromRoute] Guid Id, [FromBody] UpdateOrderFileRequest request)
        {
            await _mediator.Send(new UpdateOrderFileCommand
            {
                Id = Id,
                Status = (OrderFilesStatus) Enum.Parse(typeof(OrderFilesStatus), request.Status),
                ErrorMessage = request.ErrorMessage,
            });

            return Ok();
        }
    }
}
