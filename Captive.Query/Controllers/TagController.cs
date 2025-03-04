using Captive.Applications.TagAndMapping.Query.GetTagAndMapping;
using Captive.Applications.TagAndMapping.Query.GetTagAndMappingByTagId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TagController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTagAndMapping([FromRoute] Guid bankId) 
        {
            var response = await _mediator.Send(new GetAllTagAndMappingQuery
            {
                BankId = bankId
            });
        
            return Ok(response);
        }

        [HttpGet("{tagId}")]
        public async Task<IActionResult> GetTagAndMappingByTagId([FromRoute] Guid tagId)
        {
            var response = await _mediator.Send(new GetTagAndMappingByTagIdQuery
            {
                Id = tagId
            });

            return Ok(response);
        }
    }
}
    