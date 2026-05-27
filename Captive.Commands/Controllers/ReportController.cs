using Captive.Applications.Reports.Commands;
using Captive.Model.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderFileNotifier _orderFileNotifier;

        public ReportController(IMediator mediator, IOrderFileNotifier orderFileNotifier)
        {
            _mediator = mediator;
            _orderFileNotifier = orderFileNotifier;
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

        [HttpPost("BatchProgress/{batchId}")]
        public async Task<IActionResult> NotifyBatchProgress([FromRoute] Guid batchId, [FromBody] string statusDetail)
        {
            await _orderFileNotifier.NotifyBatchProgress(batchId, statusDetail);
            return Ok();
        }
    }
}
