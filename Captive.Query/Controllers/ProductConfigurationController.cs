using Captive.Applications.Product.Query.GetProductConfiguration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{productId}/[controller]")]
    [ApiController]
    public class ProductConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductConfigurationController(IMediator mediator) { 
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductConfiguration([FromRoute]Guid productId)
        {
            var response = await _mediator.Send(new GetProductConfigurationQuery() { ProductId = productId });

            return Ok(response);
        }
    }
}
