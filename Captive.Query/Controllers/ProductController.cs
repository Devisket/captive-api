using Captive.Applications.Product.Query.GetProductConfiguration;
using Captive.Applications.Product.Query.GetAllProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Captive.Applications.Bank.Query.GetBankFormChecks;

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

        [HttpGet("{productId}/formChecks")]
        public async Task<IActionResult> GetProductFormChecks([FromRoute] Guid productId)
        {
            var response = await _mediator.Send(
                new GetProductFormCheckQuery
                {
                    ProductId = productId
                });

            return Ok(response);
        }
    }
}