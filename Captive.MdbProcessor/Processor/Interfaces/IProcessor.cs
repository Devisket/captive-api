using Captive.Model.Dto;

namespace Captive.MdbProcessor.Processor.Interfaces
{
    public interface IProcessor <in T> where T : class
    {
        IEnumerable<CheckOrderDto> Extractfile(OrderfileDto orderFile, T config);
    }
}
