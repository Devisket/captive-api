
namespace Captive.Messaging.Interfaces {
    public interface IConsumer<T> {
        public void OnConsume(T message);
    }
}
