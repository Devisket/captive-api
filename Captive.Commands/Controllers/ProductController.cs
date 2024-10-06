using Captive.Applications.Product.Command.CreateProductConfiguration;
using Captive.Applications.Product.Command.CreateProductConfiguration;
using Captive.Applications.Product.Command.CreateProductType;
using Captive.Applications.Product.Command.DeleteProductType;
using Captive.Data.Models;
using Captive.Model.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

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
            });
            return Ok();
            
        }

        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductType([FromRoute] Guid bankId, [FromRoute] Guid productTypeId,[FromBody] CreateProductTypeCommandRequest request)
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
        public async Task<IActionResult> DeleteProductType([FromRoute] Guid bankId, [FromRoute] Guid productTypeId)
        {
            await _mediator.Send(new DeleteProductTypeCommand
            {
                ProductTypeId = productTypeId,
                BankId = bankId,
            });

            return Ok();
        }

        [HttpPost("{productId}/configuration")]
        public async Task<IActionResult> AddProductConfiguration([FromRoute] Guid productId, [FromBody] ProductConfigurationRequest request)
        {
            await _mediator.Send(new CreateProductConfigurationCommand
            {
                ProductId = productId,
                FileName = request.FileName,
                ConfigurationData = request.ConfigurationData,
                ConfigurationType = request.ConfigurationType
            });

            return NoContent();
        }
        [HttpPut("/product/{productId}/configuration/{productConfigurationId}")]
        public async Task<IActionResult> AddProductConfiguration([FromRoute] Guid productId, [FromRoute] Guid productConfigurationId, [FromBody] ProductConfigurationRequest requestBody)
        {
            await _mediator.Send(new CreateProductConfigurationCommand
            {
                Id = productConfigurationId,
                ProductId = productId,
                FileName = requestBody.FileName,
                ConfigurationData = requestBody.ConfigurationData,
                ConfigurationType = requestBody.ConfigurationType,                               
            });

            return NoContent();
        }
    }
}
