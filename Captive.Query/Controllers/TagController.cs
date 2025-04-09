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
        public async Task<IActionResult> GetAllTag([FromRoute] Guid bankId) 
        {
            var response = await _mediator.Send(new GetAllTagQuery
            {
                BankId = bankId
            });
        
            return Ok(response);
        }

        [HttpGet("{tagId}")]
        public async Task<IActionResult> GetTagMapping([FromRoute] Guid tagId)
        {
            var response = await _mediator.Send(new GetTagMappingByTagIdQuery
            {
                TagId = tagId
            });

            return Ok(response);
        }
    }
}
    