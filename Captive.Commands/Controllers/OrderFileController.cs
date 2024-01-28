using Captive.Applications.OrderFiles.Commands.UploadOrderFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Captive.Commands.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFileController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderFileController(IMediator mediator) 
        {
            _mediator = mediator;    
        }

        [HttpPost("upload")]
        public async Task<IActionResult>UploadOrderFile([FromForm]IFormFile file) 
        {
            if (file.Length < 0)
                return BadRequest();

            using(var fileStream = file.OpenReadStream())
            {
                byte[] fileBytes = new byte[file.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);

                await _mediator.Send( new UploadOrderFileCommand() 
                    { 
                        OrderFile = fileBytes
                    });

                return Ok();
            }
        }

        
    }
}
