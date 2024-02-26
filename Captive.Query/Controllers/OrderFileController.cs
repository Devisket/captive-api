using Captive.Applications.OrderFile.Queries.GetAllOrderFiles;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAllOrderFiles([FromRoute] int bankId) 
        {
            var response = await _mediator.Send(new GetAllOrderFilesQuery() { BankId = bankId });
            return Ok();
        }
    }
}
