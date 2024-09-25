using Captive.Messaging.Models;

namespace Captive.Fileprocessor.Services.FileProcessOrchestrator.cs
{
    public interface IFileProcessOrchestratorService
    {
        Task ProcessFile(FileUploadMessage message);
    }
}
