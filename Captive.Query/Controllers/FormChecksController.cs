﻿using Captive.Applications.Bank.Query.GetBankFormChecks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormChecksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FormChecksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("bank/{bankId}")]
        public async Task<IActionResult> GetBankFormChecks([FromRoute] Guid bankId, [FromQuery] Guid productId)
        {
            var response = await _mediator.Send(
                new GetBankFormCheckQuery { 
                    Id = bankId, 
                    ProductId = productId
                });

            return Ok(response);
        }
    }
}
