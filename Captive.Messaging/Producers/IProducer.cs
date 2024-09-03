
namespace Captive.Messaging.Producers
{
    public interface IProducer<T>
    {
        void ProduceMessage(T message);
    }
}
