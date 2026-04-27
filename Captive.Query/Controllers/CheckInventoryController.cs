using Captive.Applications.CheckInventory.Query.GetCheckInventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class CheckInventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CheckInventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCheckInventory([FromRoute] Guid bankId, [FromQuery] GetCheckInventoryQuery query)
        {
            query.BankId = bankId;
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
