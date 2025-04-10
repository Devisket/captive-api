using Captive.Applications.CheckInventory.Commands.AddCheckInventory;
using Captive.Applications.CheckInventory.Commands.AddCheckInventoryDetails;
using Captive.Applications.CheckInventory.Commands.DeleteCheckInventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CheckInventoryController(IMediator mediator) 
        { 
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckInventory([FromBody]AddCheckInventoryCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPut("{checkInventoryId}")]
        public async Task<IActionResult> UpdateCheckInventory([FromRoute]Guid checkInventoryId,  [FromBody]AddCheckInventoryCommand request)
        {
            request.Id = checkInventoryId;
            await _mediator.Send(request);
            return Ok();
        }

        [HttpDelete("{checkInventoryId}")]
        public async Task<IActionResult> DeleteCheckInventory([FromRoute] Guid checkInventoryId)
        {
            await _mediator.Send(new DeleteCheckInventoryCommand() { Id = checkInventoryId });
            return Ok();
        }


        [HttpPost("ApplyCheckInventoryDetails")]
        public async Task<IActionResult>ApplyCheckInventoryDetails([FromBody]ApplyCheckInventoryDetailsCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
