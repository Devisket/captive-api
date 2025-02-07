﻿using Captive.Messaging.Base;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;

namespace Captive.Messaging.Producers.Messages
{
    public class DbfProducerMessage : BaseProducer<DbfGenerateMessage>
    {
        public DbfProducerMessage(IRabbitConnectionManager connectionFactory) : base(connectionFactory) { }
        public override string queueName { get => "DbfGenerate"; }
    }
}
