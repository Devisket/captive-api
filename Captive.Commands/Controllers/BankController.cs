using Captive.Applications.Bank.Command.CreateBankInfo;
using Captive.Applications.Bank.Command.DeleteBankInfo;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BankController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> OnPost([FromBody] CreateBankInfoCommand request)
        {
            await _mediator.Send(request);

            return Created();
        }

        [HttpPut("id/{id}")]
        public async Task<IActionResult> OnUpdate([FromBody] CreateBankInfoCommand request, [FromRoute] int? id)
        {
            request.Id = id ?? null;

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpDelete("id/{id}")]
        public async Task<IActionResult> OnDelete([FromRoute]int id)
        {
            await _mediator.Send(new DeleteBankInfoCommand(id));
            return NoContent();
        }
    }
}
