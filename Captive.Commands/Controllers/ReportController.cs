using Captive.Applications.Reports.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost("GenerateOutput/{batchId}")]
        public async Task<IActionResult> GenerateReport([FromRoute] Guid batchId)
        {
            await _mediator.Send(new GenerateReportCommand
            {
                BatchId = batchId
            });

            return Ok();
        }
    }
}
