using Captive.Applications.Bank.Command.CreateBankBranches;
using Captive.Applications.Bank.Command.CreateBankInfo;
using Captive.Applications.Bank.Command.DeleteBankBranch;
using Captive.Applications.Bank.Command.DeleteBankInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankInfoController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BankInfoController(IMediator mediator)
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
        public async Task<IActionResult> OnUpdate([FromBody] CreateBankInfoCommand request, [FromRoute] Guid? id)
        {
            request.Id = id ?? null;

            await _mediator.Send(request);

            return NoContent();
        }

        [HttpDelete("id/{id}")]
        public async Task<IActionResult> OnDelete([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteBankInfoCommand(id));
            return NoContent();
        }

        [HttpPost("{id}/branch")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBankBranchCommand request, [FromRoute] Guid id)
        {
            request.BankId = id;

            await _mediator.Send(request);
            return NoContent();
        }

        [HttpPut("{bankId}/branch/{branchId}")]
        public async Task<IActionResult> UpdateBranch([FromBody] CreateBankBranchCommand request, [FromRoute] Guid bankId, [FromRoute] Guid branchId)
        {
            request.BankId = bankId;
            request.BranchId = branchId;

            await _mediator.Send(request);
            return NoContent();
        }

        [HttpDelete("{bankId}/branch/{branchId}")]
        public async Task<IActionResult> DeleteBranch([FromRoute] Guid bankId, [FromRoute] Guid branchId)
        {
            await _mediator.Send(new DeleteBankBranchCommand
            {
                BankId = bankId,
                BranchId = branchId
            });

            return NoContent();
        }
    }
}
