using Captive.Applications.Batch.Commands.CreateBatchFile;
using Captive.Applications.Batch.Commands.DeleteBatchFile;
using Captive.Applications.Batch.Commands.ProcessBatch;
using Captive.Applications.Batch.Commands.ValidateBatch;
using Captive.Applications.Batch.Hubs;
using Captive.Applications.Batch.Services;
using Captive.Model.Request;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Captive.Commands.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BatchController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<CreateBatchFileResponse> CreateBatch([FromRoute] Guid bankId, [FromBody] CreateBatchRequest request)
        {
            var response = await _mediator.Send(new CreateBatchFileCommand { BankInfoId = bankId, DeliveryDate = request.DeliveryDate, BatchName = request.BatchName});

            return response;
        }

        [HttpDelete("{batchId}")]
        public async Task<IActionResult> DeleteBatch([FromRoute] Guid batchId)
        {
            await _mediator.Send(new DeleteBatchFileCommand{ Id = batchId});

            return NoContent();
        }

        [HttpPost("{batchId}/validate")]
        public async Task<IActionResult> ValidateBatch([FromRoute]Guid batchId)
        {
            var response = await _mediator.Send(new ValidateBatchCommand { BatchId = batchId });
            return Ok(response);
        }

        [HttpPost("{batchId}/process")]
        public async Task<IActionResult> ProcessBatch([FromRoute] Guid batchId)
        {
            var response = await _mediator.Send(new ProcessBatchCommand { BatchId = batchId });
            return Ok(response);
        }

    }
}
