using Captive.Data;
using Captive.Processing.Processor;
using MediatR;

namespace Captive.Applications.OrderFiles.Commands.UploadOrderFile
{
    public class UploadOrderFileCommandHandler : IRequestHandler<UploadOrderFileCommand,Unit>
    {
        private readonly IFileProcessor _fileProcessor;
        private readonly IReadUnitOfWork _uow;

        public UploadOrderFileCommandHandler(IFileProcessor fileProcessor, IReadUnitOfWork uow)
        {
            _fileProcessor = fileProcessor;
            _uow = uow;
        }


        public Task<Unit> Handle(UploadOrderFileCommand request, CancellationToken cancellationToken)
        {
            var orderfileConfiguration = _uow.OrderFileConfigurations.GetAll().FirstOrDefault(x=> String.Equals(x.Name, request.FileName));

            if(orderfileConfiguration == null) 
            {
                throw new ArgumentException("File is empty");
            }

            var file = request.OrderFile;

            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("File is empty");
            }

            var checkOrders = _fileProcessor.OnProcessFile(file, orderfileConfiguration.ConfigurationData);




            return Task.FromResult(Unit.Value);
        }
    }
}
