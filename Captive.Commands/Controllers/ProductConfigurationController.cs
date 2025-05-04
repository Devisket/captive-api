using Captive.Applications.Product.Command.CreateProductConfiguration;
using Captive.Data.Enums;
using Captive.Model.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{productId}/[controller]")]
    [ApiController]
    public class ProductConfigurationController : ControllerBase
    {

        private readonly IMediator _mediator;
        public ProductConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductConfiguration([FromRoute] Guid productId, [FromBody] ProductConfigurationRequest request)
        {
            await _mediator.Send(new CreateProductConfigurationCommand()
            {
                Id = null,
                ProductId = productId,
                FileName = request.FileName,
                ConfigurationType = (ConfigurationType) Enum.Parse(typeof(ConfigurationType), request.ConfigurationType),
                ConfigurationData = request.ConfigurationData,
                FileType = request.FileType,
                IsChangeFileType = request.IsChangeFileType,
            });

            return Ok();
        }

        [HttpPut("{productConfigurationId}")]
        public async Task<IActionResult> UpdateProductConfiguration([FromRoute] Guid productId, [FromRoute] Guid productConfigurationId, [FromBody] ProductConfigurationRequest request)
        {
            await _mediator.Send(new CreateProductConfigurationCommand()
            {
                ProductId = productId,
                Id = request.Id,        
                ConfigurationData = request.ConfigurationData,
                ConfigurationType = (ConfigurationType) Enum.Parse(typeof(ConfigurationType), request.ConfigurationType),
                FileName = request.FileName,
                FileType = request.FileType,
                IsChangeFileType = request.IsChangeFileType,
            });

            return Ok();
        }
    }
}
