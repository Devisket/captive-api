using Captive.Applications.CheckOrder.Command.CreateCheckOrder;
using Captive.Applications.CheckValidation.Query.ValidateCheckOrder;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        

        [HttpPost("validateCheck")]
        public async Task<IActionResult> ValidateCheckOrders([FromBody] ValidateCheckOrderCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }

    }
}
