using Captive.Model;

namespace Captive.Processor
{
    public interface IFileProcessor
    {
        public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration);
    }
}
