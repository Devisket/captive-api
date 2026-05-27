using Captive.Applications.Batch.Query.GetAllBatch;
using Captive.Applications.Batch.Query.GetBatchById;
using Captive.Applications.Batch.Query.GetBatchJobStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllBatch([FromRoute]Guid bankId) 
        {
            var response = await _mediator.Send(new GetBatchQuery
            {
                BankId = bankId
            });

            return Ok(response);
        }

        [HttpGet("{batchId}")]
        public async Task<IActionResult> GetBatchById([FromRoute] Guid bankId, [FromRoute] Guid batchId)
        {
            var response = await _mediator.Send(new GetBatchByIdQuery
            {
                BankId = bankId,
                BatchId = batchId
            });

            return Ok(response);
        }

        [HttpGet("{batchId}/job")]
        public async Task<IActionResult> GetBatchJobStatus([FromRoute] Guid batchId)
        {
            var response = await _mediator.Send(new GetBatchJobStatusQuery { BatchId = batchId });
            return Ok(response);
        }
    }
}
