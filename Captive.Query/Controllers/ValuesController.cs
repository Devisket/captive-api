using Captive.Applications.Bank.Query.GetAllBankInfos.Model;
using Captive.Applications.Values;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ValuesController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetValues([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new ValuesQuery(bankId));
            return Ok(response);
        }

    }
}
