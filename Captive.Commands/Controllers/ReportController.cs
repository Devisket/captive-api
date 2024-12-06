﻿using Captive.Applications.Reports.Commands;
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

        [HttpPost]
        public async Task<IActionResult> GenerateReport([FromBody]GenerateReportCommand request)
        {
            await _mediator.Send(request);

            return Ok();
        }
    }
}
