using Captive.Applications.CheckOrder.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{orderFileId}/[controller]")]
    [ApiController]
    public class CheckOrderController : ControllerBase
    {

        private readonly IMediator _mediator;

        public CheckOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> GetCheckOrder([FromRoute] Guid orderFileId)
        {
            var response = await _mediator.Send(new GetFloatingCheckOrderQuery() { OrderFileId = orderFileId });

            return Ok(response);
        } 
    }
}
