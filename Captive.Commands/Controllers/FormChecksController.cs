using Captive.Applications.FormChecks.Command.CreateUpdateFormCheck;
using Captive.Applications.FormsChecks.Command.DeleteFormCheck;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{productId}/[controller]")]
    [ApiController]
    public class FormChecksController : ControllerBase
    {

        private readonly IMediator _mediator;
        public FormChecksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFormCheck([FromRoute] Guid productId, [FromBody] CreateUpdateFormCheckCommandRequest request)
        {
            await _mediator.Send(new CreateUpdateFormCheckCommand
            {
                ProductId = productId,
                Detail = request,
            });
            
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFormCheck([FromRoute] Guid productId, [FromBody] CreateUpdateFormCheckCommandRequest request)
        {
            await _mediator.Send(new CreateUpdateFormCheckCommand
            {
                ProductId = productId,
                Detail = request
            });

            return Ok();
        }

        [HttpDelete("{formCheckId}")]
        public async Task<IActionResult> Delete( [FromRoute] Guid formCheckId)
        {
            await _mediator.Send(new DeleteFormCheckCommand
            {
                FormCheckId = formCheckId
            });

            return Ok();
        }
    }
}
