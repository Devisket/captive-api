using Captive.Messaging.Models;
using RabbitMQ.Client;

namespace Captive.Messaging.Producers.Messages
{
    public class FileUploadProducerMessage : BaseProducer<FileUploadMessage>
    {
        public FileUploadProducerMessage(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public override string queueName { get => ""; }

    }
}
