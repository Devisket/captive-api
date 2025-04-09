using Captive.Applications.Bank.Command.CreateBankBranches;
using Captive.Applications.Bank.Command.CreateBankInfo;
using Captive.Applications.Bank.Command.DeleteBankBranch;
using Captive.Applications.Bank.Command.DeleteBankInfo;
using Captive.Applications.Bank.Query.GetBankBranches.Model;
using Captive.Data.Enums;
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
        public async Task<IActionResult> CreateBranch([FromBody] BankBranchDto request, [FromRoute] Guid id)
        {
            await _mediator.Send(new CreateBankBranchCommand
            {
                BankId = id,
                BranchId = null,
                BranchName = request.BranchName,
                BranchStatus = request.BranchStatus,
                BrstnCode = request.BrstnCode,
                BranchCode = request.BranchCode,
                MergingBranchId = request.MergingBranchId,
                BranchAddress1 = request.BranchAddress1,
                BranchAddress2 = request.BranchAddress2,
                BranchAddress3 = request.BranchAddress3,
                BranchAddress4 = request.BranchAddress4,
                BranchAddress5 = request.BranchAddress5,
            });
            return NoContent();
        }

        [HttpPut("{bankId}/branch/{branchId}")]
        public async Task<IActionResult> UpdateBranch([FromBody] BankBranchDto request, [FromRoute] Guid bankId, [FromRoute] Guid branchId)
        {
            await _mediator.Send(new CreateBankBranchCommand
            {
                BankId = bankId,
                BranchId = branchId,
                BranchName = request.BranchName,
                BranchStatus = request.BranchStatus,
                BrstnCode = request.BrstnCode,
                BranchCode = request.BranchCode,
                MergingBranchId = request.MergingBranchId,
                BranchAddress1 = request.BranchAddress1,
                BranchAddress2 = request.BranchAddress2,
                BranchAddress3 = request.BranchAddress3,
                BranchAddress4 = request.BranchAddress4,
                BranchAddress5 = request.BranchAddress5,
            });

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
