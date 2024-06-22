using Captive.Applications.Product.Query.GetAllProductConfiguration;
using Captive.Applications.Product.Query.GetAllProductType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/[controller]/{bankId}")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductType([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new GetAllProductTypeQuery { BankId = bankId });

            return Ok(response);
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetAllProductConfiguration([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new GetAllProductConfigurationQuery() { BankId = bankId });

            return Ok(response);
        }
    }
}
