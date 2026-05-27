using Captive.Messaging.Base;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;

namespace Captive.Messaging.Producers.Messages
{
    public class CheckOrderProcessProducerMessage : BaseProducer<CheckOrderProcessMessage>
    {
        public CheckOrderProcessProducerMessage(IRabbitConnectionManager connectionFactory) : base(connectionFactory) { }
        public override string queueName { get => "CaptiveCheckOrderProcess"; }
    }
}
