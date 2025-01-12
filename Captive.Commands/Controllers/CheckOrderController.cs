using Captive.Applications.CheckOrder.Command.CheckDuplication;
using Captive.Applications.CheckOrder.Command.CreateCheckOrder;
using Captive.Applications.Orderfiles.Command.ValidateOrderFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class CheckOrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CheckOrderController(IMediator mediator) 
        { 
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckOrder([FromBody] CreateCheckOrderCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("floating")]
        public async Task<IActionResult> CreateFloatingCheckOrder([FromBody] CreateFloatingCheckOrderCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("{orderFileId}/duplicationcheck")]
        public async Task<IActionResult> CheckDuplication([FromRoute] Guid orderFileId)
        {
            await _mediator.Send(new CheckDuplicationCommand { OrderFileId = orderFileId});
            return NoContent();
        }
    }
}
