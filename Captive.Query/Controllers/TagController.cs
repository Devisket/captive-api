using Captive.Applications.TagAndMapping.Query.GetTagAndMapping;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Queries.Controllers
{
    [Route("api/{bankId}/[controller]{tagId}")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TagController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTagAndMapping([FromRoute] Guid tagId) 
        {
            var response = await _mediator.Send(new GetTagAndMappingQuery
            {
                Id = tagId
            });
        
            return Ok(response);
        }
    }
}
