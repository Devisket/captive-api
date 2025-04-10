using Captive.Applications.CheckOrder.Command.CheckDuplication;
using Captive.Applications.CheckOrder.Command.CreateCheckOrder;
using Captive.Applications.CheckOrder.Command.DeleteFloatingCheckOrder;
using Captive.Applications.CheckOrder.Command.HoldFloatingCheckOrder;
using Captive.Applications.CheckOrder.Command.ReleaseFloatingCheckOrder;
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


        [HttpPut("floating")]
        public async Task<IActionResult> UpdateFloatingCheckOrder([FromBody] CreateFloatingCheckOrderCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpDelete("floating/{floatingCheckOrderId}")]
        public async Task<IActionResult> DeleteFloatingCheckOrder([FromRoute] Guid floatingCheckOrderId)
        {
            await _mediator.Send(new DeleteFloatingCheckOrderCommand { FloatingCheckOrderId = floatingCheckOrderId });
            return Ok();
        }

        [HttpPost("{orderFileId}/duplicationcheck")]
        public async Task<IActionResult> CheckDuplication([FromRoute] Guid orderFileId)
        {
            await _mediator.Send(new CheckDuplicationCommand { OrderFileId = orderFileId});
            return NoContent();
        }

        [HttpPost("floating/hold/{floatingCheckOrderId}")]
        public async Task<IActionResult> HoldFloatingCheckOrder([FromRoute] Guid floatingCheckOrderId)
        {
            await _mediator.Send(new HoldFloatingCheckOrderCommand { Id = floatingCheckOrderId });
            return NoContent();
        }

        [HttpPost("floating/release/{floatingCheckOrderId}")]
        public async Task<IActionResult> ReleaseFloatingCheckOrder([FromRoute] Guid floatingCheckOrderId)
        {
            await _mediator.Send(new ReleaseFloatingCheckOrderCommand { Id = floatingCheckOrderId });
            return NoContent();
        }
    }
}
