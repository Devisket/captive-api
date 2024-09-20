using Captive.Messaging.Base;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;

namespace Captive.Messaging.Producers.Messages
{
    public class FileUploadProducerMessage : BaseProducer<FileUploadMessage>
    {
        public FileUploadProducerMessage(IRabbitConnectionManager connectionFactory) : base(connectionFactory) { }
        public override string queueName { get => "CaptiveFileUpload"; }
    }
}
