using Captive.Applications.Batch.Commands.CreateBatchFile;
using Captive.Applications.Batch.Commands.DeleteBatchFile;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BatchController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<CreateBatchFileResponse> CreateBatch([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new CreateBatchFileCommand { BankInfoId = bankId });

            return response;
        }
        //api/bankId/batch/batchId
        [HttpDelete("{batchId}")]
        public async Task<IActionResult> DeleteBatch([FromRoute] Guid batchId)
        {
            await _mediator.Send(new DeleteBatchFileCommand{ Id = batchId});

            return NoContent();
        }

    }
}
