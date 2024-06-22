using Captive.Applications.OrderFile.Queries.GetAllOrderFiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/[controller]/{bankId}")]
    [ApiController]
    public class OrderFileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderFileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderFiles([FromRoute] Guid bankId) 
        {
            var response = await _mediator.Send(new GetAllOrderFilesQuery() { BankId = bankId });

            if(response == null)
            {
                return NoContent();
            }

            return Ok(response);
        }
    }
}
