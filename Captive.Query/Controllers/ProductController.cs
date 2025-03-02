using Captive.Applications.Product.Query.GetProductConfiguration;
using Captive.Applications.Product.Query.GetAllProductType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductType([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new GetAllProductTypeQuery { BankId = bankId });

            return Ok(response);
        }

        [HttpGet("{productId}/configuration")]
        public async Task<IActionResult> GetProductConfiguration([FromRoute] Guid productId)
        {
            var response = await _mediator.Send(new GetProductConfigurationQuery() { ProductId = productId });

            return Ok(response);
        }
    }
}