using Captive.Model.Dto;
using Captive.Model.Processing.Configurations;

namespace Captive.Processing.Processor.MDBFileProcessor
{
    public interface IMDBFileProcessor
    {
        IEnumerable<CheckOrderDto> Extractfile(OrderfileDto orderFile, MdbConfiguration config);
    }
}
