using Captive.Applications.Bank.Query.GetBankFormChecks;
using Captive.Applications.FormsChecks.Queries.GetAllFormChecks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllFormChecks([FromRoute] Guid productId)
        {
            var response = await _mediator.Send(
                new GetAllFormChecksQuery
                { 
                    ProductId = productId
                });

            return Ok(response);
        }
    }
}
