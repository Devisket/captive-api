using Captive.Applications.Product.Command.CreateProductType;
using Captive.Applications.Product.Command.DeleteProductType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/bank/{bankId}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewProductType([FromRoute] Guid bankId, [FromBody] CreateProductTypeCommandRequest request)
        {
            await _mediator.Send(new CreateProductTypeCommand
            {
                BankId = bankId,
                ProductName = request.ProductName,
                ProductSequence = request.ProductSequence,
            });
            return Ok();
            
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductType([FromRoute] Guid bankId, [FromRoute] Guid productId,[FromBody] CreateProductTypeCommandRequest request)
        {
            await _mediator.Send(new CreateProductTypeCommand
            {
                ProductId = productId,
                BankId = bankId,
                ProductName = request.ProductName,
                ProductSequence = request.ProductSequence,
            });

            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] Guid productId)
        {
            await _mediator.Send(new DeleteProductTypeCommand
            {
                ProductId = productId,
            });

            return Ok();
        }
    }
}
