using Captive.Messaging.Base;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;

namespace Captive.Messaging.Producers.Messages
{
    public class BatchProcessProducerMessage : BaseProducer<BatchProcessMessage>
    {
        public BatchProcessProducerMessage(IRabbitConnectionManager connectionFactory) : base(connectionFactory) { }
        public override string queueName { get => "CaptiveBatchProcess"; }
    }
}
