using Captive.Applications.CheckInventory.Query.ExportActiveCheckInventories;
using Captive.Applications.CheckInventory.Query.GetCheckInventory;
using Captive.Applications.CheckInventory.Query.GetCheckInventoryDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class CheckInventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CheckInventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCheckInventory([FromRoute] Guid bankId, [FromQuery] GetCheckInventoryQuery query)
        {
            query.BankId = bankId;
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Series assigned from a check inventory (?checkInventoryId=...), or the series assigned to
        /// check orders without a check inventory (?unassigned=true).
        /// </summary>
        [HttpGet("details")]
        public async Task<ActionResult> GetCheckInventoryDetails([FromRoute] Guid bankId, [FromQuery] GetCheckInventoryDetailsQuery query)
        {
            query.BankId = bankId;
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("export")]
        public async Task<ActionResult> ExportActiveCheckInventories([FromRoute] Guid bankId)
        {
            var bytes = await _mediator.Send(new ExportActiveCheckInventoriesQuery { BankId = bankId });
            return File(bytes, "text/csv", "check-inventory-active.csv");
        }
    }
}
