using Captive.Applications.Product.Command.CreateProductType;
using Captive.Applications.Product.Command.DeleteProductType;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]/bank/{bankId}")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductTypeController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewProductType([FromRoute] int bankId, [FromBody] CreateProductTypeCommandRequest request)
        {
            await _mediator.Send(new CreateProductTypeCommand
            {
                BankId = bankId,
                ProductName = request.ProductName,
            });
            return Ok();
            
        }

        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductType([FromRoute] int bankId, [FromRoute] int productTypeId,[FromBody] CreateProductTypeCommandRequest request)
        {
            await _mediator.Send(new CreateProductTypeCommand
            {
                ProductTypeId = productTypeId,
                BankId = bankId,
                ProductName = request.ProductName,
            });

            return Ok();
        }

        [HttpDelete("{productTypeId}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int bankId, [FromRoute] int productTypeId)
        {
            await _mediator.Send(new DeleteProductTypeCommand
            {
                ProductTypeId = productTypeId,
                BankId = bankId,
            });

            return Ok();
        }
    }
}
