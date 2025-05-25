using Captive.Applications.CheckInventory.Query.GetCheckInventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{tagId}/[controller]")]
    [ApiController]
    public class CheckInventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CheckInventoryController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCheckInventory([FromRoute] Guid tagId, [FromQuery]GetCheckInventoryQuery query)
        {
            var response = await _mediator.Send(query);

            return Ok(response);
        }
    }
}
