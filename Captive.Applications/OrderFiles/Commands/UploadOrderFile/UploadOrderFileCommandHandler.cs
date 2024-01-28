using Captive.Processing.Processor;
using MediatR;

namespace Captive.Applications.OrderFiles.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
    {
        private readonly IFileProcessor _fileProcessor;

        public UploadOrderFileCommandHandler(IFileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        public Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            var file = request.OrderFile;

            if(file == null || file.Length <= 0) 
            {
                throw new ArgumentException("File is empty");
            }

            _fileProcessor.OnProcessFile(file);

            return Task.FromResult(Unit.Value);
        }
    }
}
