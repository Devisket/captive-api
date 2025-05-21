using Captive.Applications.TagAndMapping.Command.CreateMapping;
using Captive.Applications.TagAndMapping.Command.CreateTag;
using Captive.Applications.TagAndMapping.Command.DeleteMapping;
using Captive.Applications.TagAndMapping.Command.DeleteTag;
using Captive.Applications.TagAndMapping.Command.LockTag;
using Captive.Model.Dto;
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
        public async Task<IActionResult> CreateTag([FromRoute] Guid bankId, [FromBody] TagDto request)
        {
            await _mediator.Send(new CreateTagCommand
            {
                Id = null,
                BankId = bankId,
                SearchByAccount = request.SearchByAccount,
                TagName = request.TagName,
                isDefaultTag = request.isDefaultTag,
                SearchByBranch = request.SearchByBranch,
                SearchByFormCheck = request.SearchByFormCheck,
                SearchByProduct = request.SearchByProduct
            });
            return NoContent();
        }

        [HttpPost("{tagId}/mapping")]
        public async Task<IActionResult> AddTagMapping([FromRoute] Guid tagId, [FromBody] IList<TagMappingDto> request)
        {
            await _mediator.Send(new CreateTagMappingCommand
            {
                TagId = tagId,
                Mappings = request
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
        public async Task<IActionResult> UpdateTag([FromRoute] Guid tagId, [FromRoute] Guid bankId, [FromBody] TagDto request)
        {
            await _mediator.Send(new CreateTagCommand
            {
                Id = tagId,
                BankId = bankId,
                SearchByAccount = request.SearchByAccount,
                TagName = request.TagName,
                isDefaultTag = request.isDefaultTag,
                SearchByBranch = request.SearchByBranch,
                SearchByFormCheck = request.SearchByFormCheck,
                SearchByProduct = request.SearchByProduct
            });
            return NoContent();
        }

        [HttpPost("Lock/{tagId}")]
        public async Task<IActionResult> UpdateTag([FromRoute] Guid tagId, [FromRoute] Guid bankId)
        {
            await _mediator.Send(new LockTagCommand { Id = tagId });

            return NoContent();
        }
    }
}
