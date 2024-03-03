using Captive.Applications.FormChecks.Command.CreateUpdateFormCheck;
using Captive.Applications.FormsChecks.Command.DeleteFormCheck;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]/Bank/{bankId}")]
    [ApiController]
    public class FormChecksController : ControllerBase
    {

        private readonly IMediator _mediator;
        public FormChecksController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFormCheck([FromRoute]int bankId,  [FromBody] CreateUpdateFormCheckCommandRequest request)
        {
            await _mediator.Send(new CreateUpdateFormCheckCommand
            {
                BankId = bankId,
                Detail = request
            });
            
            return Created();
        }

        [HttpPut("{formCheckId}")]
        public async Task<IActionResult> UpdateFormCheck([FromRoute] int bankId,[FromRoute] int formCheckId, [FromBody] CreateUpdateFormCheckCommandRequest request)
        {
            await _mediator.Send(new CreateUpdateFormCheckCommand
            {
                FormCheckId = formCheckId,
                BankId = bankId,
                Detail = request
            });

            return Ok();
        }

        [HttpDelete("{formCheckId}")]
        public async Task<IActionResult> Delete([FromRoute] int bankId, [FromRoute] int formCheckId)
        {
            await _mediator.Send(new DeleteFormCheckCommand
            {
                BankId = bankId,
                FormCheckId = formCheckId
            });

            return Ok();
        }
    }
}
