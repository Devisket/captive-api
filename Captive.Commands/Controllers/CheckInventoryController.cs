using Captive.Applications.CheckInventory.Commands.AddCheckInventory;
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

        [HttpPost("AddInventory")]
        public async Task<IActionResult> AddCheckInventory(AddCheckInventoryCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
