using Captive.Applications.Batch.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/[controller]/{bankId}")]
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
    }
}
