using Captive.Applications.Bank.Query.GetAllBankInfos;
using Captive.Applications.Bank.Query.GetBankBranches;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("{bankId}/branches")]
        public async Task<IActionResult> GetAllBankBranches(int bankId)
        {
            var response = await _mediator.Send(new GetBankBranchesQuery { BankId = bankId });

            if (response.Branches == null || !response.Branches.Any())
            {
                return NoContent();
            }

            return Ok( response);
        }
    }
}
