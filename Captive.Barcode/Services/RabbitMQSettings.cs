namespace Captive.Barcode.Services
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string QueueName { get; set; } = "barcode-generation-queue";
        public string ExchangeName { get; set; } = "barcode-exchange";
        public string RoutingKey { get; set; } = "barcode.generate";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public bool Exclusive { get; set; } = false;
        public int PrefetchCount { get; set; } = 1;
        public bool AutoAck { get; set; } = false;
        public int ReconnectDelay { get; set; } = 5000;
        public int MaxRetries { get; set; } = 3;
    }
} 