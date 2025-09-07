using Captive.Messaging.Models;

namespace Captive.Orchestrator.Services.FileProcessOrchestrator.cs
{
    public interface IFileProcessOrchestratorService
    {
        Task ProcessFile(FileUploadMessage message);
    }
}
