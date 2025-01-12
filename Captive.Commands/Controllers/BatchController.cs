using Captive.Applications.Batch.Commands.CreateBatchFile;
using Captive.Applications.Batch.Commands.DeleteBatchFile;
using Captive.Applications.Batch.Hubs;
using Captive.Applications.Batch.Services;
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
        private readonly IHubContext<BatchHub> _hubContext;
        private readonly IBatchService _batchService;
        public BatchController(IMediator mediator, IHubContext<BatchHub> hubContext, IBatchService batchService) {
            _mediator = mediator;
            _hubContext = hubContext;
            _batchService = batchService;
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
