using Captive.Applications.CheckValidation.Query;
using Captive.Applications.CheckValidation.Query.GetAllCheckValidation;
using Captive.Applications.CheckValidation.Query.GetCheckValidationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api{bankId}/[controller]")]
    [ApiController]
    public class CheckValidationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CheckValidationController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCheckValidation([FromRoute] Guid bankId, [FromRoute] Guid checkValidationId)
        {
            var response = await _mediator.Send(new GetAllCheckValidationQuery { BankInfoId = bankId});

            return Ok(response);
        }

        [HttpGet("{checkValidationId}")]
        public async Task<IActionResult> GetCheckValidationById([FromRoute]Guid bankId, [FromRoute]Guid checkValidationId) 
        {
            var response = await _mediator.Send(new GetCheckValidationByIdQuery { BankInfoId = bankId, Id = checkValidationId});

            return Ok(response);
        }
    }
}
