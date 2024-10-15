using Captive.Applications.CheckValidation.Command;
using Captive.Model.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class CheckValidationController : ControllerBase
    {
        private readonly IMediator _mediator;
        CheckValidationController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckValidation([FromRoute] Guid bankId, CreateCheckValidationRequest request) 
        {
            await _mediator.Send(new CreateCheckValidationCommand
            {
                BankId = bankId,
                Name = request.Name,
                ValidationType = request.ValidationType,
            });

            return NoContent();
        }

        [HttpPut("{checkValidationId}")]
        public async Task<IActionResult> UpdateCheckValidation([FromRoute] Guid bankId, [FromRoute] Guid checkValidationId,CreateCheckValidationRequest request)
        {
            await _mediator.Send(new CreateCheckValidationCommand
            {
                BankId = bankId,
                Id = checkValidationId,
                Name = request.Name,
                ValidationType = request.ValidationType,
            });

            return NoContent();
        }
    }
}
