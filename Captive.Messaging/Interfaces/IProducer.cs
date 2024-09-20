namespace Captive.Messaging.Interfaces
{
    public interface IProducer<T>
    {
        void ProduceMessage(T message);
    }
}
