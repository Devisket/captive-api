using Captive.Messaging.Base;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;

namespace Captive.Messaging.Producers.Messages
{
    public class GenerateBarcodeProducerMessage : BaseProducer<GenerateBarcodeMessage>
    {
        public GenerateBarcodeProducerMessage(IRabbitConnectionManager connectionFactory):base(connectionFactory) { }
        public override string queueName => "GenerateBarcode";
    }
}
