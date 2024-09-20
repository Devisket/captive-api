using Captive.Processing.Processor.Model;

namespace Captive.Processing.Processor.MDBFileProcessor
{
    public interface IMDBFileProcessor
    {
        IEnumerable<OrderFileData> Extractfile(Guid batchId, string fileName, string config);
    }
}
