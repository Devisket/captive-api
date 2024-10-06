using Captive.Applications.Bank.Query.GetAllBankInfos;
using Captive.Applications.Bank.Query.GetBankBranches;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllBankInfos()
        {
            var response = await _mediator.Send(new GetAllBankInfoQuery());

            return Ok(response);
        }

        [HttpGet("{bankId}/branch")]
        public async Task<IActionResult> GetAllBankBranches(Guid bankId)
        {
            var response = await _mediator.Send(new GetBankBranchesQuery { BankId = bankId });

            return Ok(response);
        }

        [HttpGet("{bankId}/branch/{branchId}")]
        public async Task<IActionResult> GetSpecificBranch([FromRoute] Guid bankId, [FromRoute] Guid branchId)
        {
            var response = await _mediator.Send(new GetBankBranchesQuery { BankId = bankId, BranchId = branchId });

            return Ok(response);
        }
    }
}
