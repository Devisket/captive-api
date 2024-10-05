using Captive.Applications.Batch.Query.GetAllBatch;
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
            var response = await _mediator.Send(new GetBatchQuery
            {
                BankId = bankId,
                BatchId = batchId
            });

            return Ok(response);
        }
    }
}
