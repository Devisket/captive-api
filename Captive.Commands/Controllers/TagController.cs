using Captive.Applications.TagAndMapping.Command.CreateMapping;
using Captive.Applications.TagAndMapping.Command.CreateTag;
using Captive.Applications.TagAndMapping.Command.DeleteMapping;
using Captive.Applications.TagAndMapping.Command.DeleteTag;
using Captive.Model.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
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

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            await _mediator.Send(new CreateTagMappingCommand
            {
                CheckValidationId = request.CheckValidationId,
                TagName = request.TagName,
            });
            return NoContent();
        }

        [HttpPost("{tagId}/mapping")]
        public async Task<IActionResult> AddTagMapping([FromRoute] Guid tagId, [FromBody] CreateTagMappingRequest request)
        {
            await _mediator.Send(new CreateMappingCommand
            {
                TagId = tagId,
                Mappings = request.mappings.Select(x => new Model.Dto.TagMappingDto
                {
                    BranchId = x.BranchId,
                    FormCheckId = x.FormCheckId,
                    ProductId = x.ProductId,
                }).ToList()
            });

            return NoContent();
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteTag([FromRoute] Guid tagId)
        {
            await _mediator.Send(new DeleteTagCommand()
            {
                Id = tagId
            });

            return NoContent();
        }

        [HttpDelete("{tagId}/mapping/{mappingId}")]
        public async Task<IActionResult> DeleteTagMapping([FromRoute] Guid tagId, [FromRoute] Guid mappingId)
        {
            await _mediator.Send(new DeleteMappingCommand
            {
                Id = mappingId,
            });

            return NoContent();
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> UpdateTag([FromRoute] Guid tagId, [FromBody] CreateTagRequest request)
        {
            await _mediator.Send(new CreateTagMappingCommand
            {
                Id = tagId, 
                CheckValidationId = request.CheckValidationId,
                TagName = request.TagName,
            });

            return NoContent();
        }
    }
}
