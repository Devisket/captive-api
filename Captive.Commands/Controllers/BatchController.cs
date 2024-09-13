using Captive.Applications.Batch.Commands.CreateBatchFile;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/{bankId}/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BatchController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<CreateBatchFileResponse> CreateBatch([FromRoute] Guid bankId)
        {
            var response = await _mediator.Send(new CreateBatchFileCommand { BankInfoId = bankId });

            return response;
        }

    }
}
