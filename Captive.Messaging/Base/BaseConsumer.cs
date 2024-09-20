
using Captive.Messaging.Interfaces;

namespace Captive.Messaging.Base
{
    public abstract class BaseConsumer<T> : IConsumer<T>
    {
        public BaseConsumer() { }
        public void OnConsume(T message)
        {
            
        }
    }
}
