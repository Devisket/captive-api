using Captive.Processing.Processor.Model;

namespace Captive.Processing.Processor
{
    public interface IFileProcessor
    {
        public void OnProcessFile(byte[] file);

        public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration);
    }
}
