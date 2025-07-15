using Captive.Barcode.Base;
using Captive.Barcode.Models;
using Captive.Model.Dto.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace Captive.Barcode.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly BarcodeServiceSettings _barcodeSettings;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQConsumerService(
            ILogger<RabbitMQConsumerService> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMQSettings> rabbitMQSettings,
            IOptions<BarcodeServiceSettings> barcodeSettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMQSettings = rabbitMQSettings.Value;
            _barcodeSettings = barcodeSettings.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service starting...");
            
            try
            {
                await InitializeRabbitMQ();
                _logger.LogInformation("RabbitMQ Consumer Service started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start RabbitMQ Consumer Service");
                throw;
            }
            
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service is running and listening for messages...");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Connection health check
                    if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
                    {
                        _logger.LogWarning("RabbitMQ connection lost. Attempting to reconnect...");
                        await InitializeRabbitMQ();
                    }
                    
                    await Task.Delay(5000, stoppingToken); // Check every 5 seconds
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in RabbitMQ Consumer Service execution");
                    await Task.Delay(_rabbitMQSettings.ReconnectDelay, stoppingToken);
                }
            }
        }

        private async Task InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.HostName,
                    Port = _rabbitMQSettings.Port,
                    UserName = _rabbitMQSettings.UserName,
                    Password = _rabbitMQSettings.Password,
                    VirtualHost = _rabbitMQSettings.VirtualHost
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(
                    exchange: _rabbitMQSettings.ExchangeName,
                    type: ExchangeType.Direct,
                    durable: _rabbitMQSettings.Durable);

                // Declare queue
                _channel.QueueDeclare(
                    queue: _rabbitMQSettings.QueueName,
                    durable: _rabbitMQSettings.Durable,
                    exclusive: _rabbitMQSettings.Exclusive,
                    autoDelete: _rabbitMQSettings.AutoDelete,
                    arguments: null);

                // Bind queue to exchange
                _channel.QueueBind(
                    queue: _rabbitMQSettings.QueueName,
                    exchange: _rabbitMQSettings.ExchangeName,
                    routingKey: _rabbitMQSettings.RoutingKey);

                // Set QoS
                _channel.BasicQos(prefetchSize: 0, prefetchCount: (ushort)_rabbitMQSettings.PrefetchCount, global: false);

                // Create consumer
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    await ProcessMessage(ea);
                };

                // Start consuming
                _channel.BasicConsume(
                    queue: _rabbitMQSettings.QueueName,
                    autoAck: _rabbitMQSettings.AutoAck,
                    consumer: consumer);

                _logger.LogInformation("RabbitMQ initialized successfully. Listening on queue: {QueueName}", _rabbitMQSettings.QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
                throw;
            }
        }

        private async Task ProcessMessage(BasicDeliverEventArgs ea)
        {
            var stopwatch = Stopwatch.StartNew();
            BarcodeGenerationRequest? request = null;
            
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                _logger.LogDebug("Received message: {Message}", message);
                
                request = JsonConvert.DeserializeObject<BarcodeGenerationRequest>(message);
                
                if (request == null)
                {
                    _logger.LogError("Failed to deserialize message");
                    _channel?.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                _logger.LogInformation("Processing barcode generation request: {RequestId}", request.RequestId);

                // Process the barcode generation
                var response = await GenerateBarcode(request);
                stopwatch.Stop();
                response.ProcessingDuration = stopwatch.Elapsed;

                // Send response if reply-to queue is specified
                if (!string.IsNullOrEmpty(request.ReplyToQueue) && _channel != null)
                {
                    await SendResponse(request.ReplyToQueue, response, request.CorrelationId);
                }

                // Acknowledge message
                _channel?.BasicAck(ea.DeliveryTag, false);
                
                _logger.LogInformation("Successfully processed request {RequestId} in {Duration}ms", 
                    request.RequestId, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing message for request {RequestId}", request?.RequestId);
                
                // Send error response if possible
                if (request != null && !string.IsNullOrEmpty(request.ReplyToQueue))
                {
                    var errorResponse = BarcodeGenerationResponse.Error(
                        request.RequestId, 
                        request.CorrelationId ?? "", 
                        ex.Message, 
                        "PROCESSING_ERROR");
                    errorResponse.ProcessingDuration = stopwatch.Elapsed;
                    
                    await SendResponse(request.ReplyToQueue, errorResponse, request.CorrelationId);
                }
                
                // Reject message (don't requeue to avoid infinite loops)
                _channel?.BasicNack(ea.DeliveryTag, false, false);
            }
        }

        private async Task<BarcodeGenerationResponse> GenerateBarcode(BarcodeGenerationRequest request)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var barcodeFactory = scope.ServiceProvider.GetRequiredService<IBarcodeServiceFactory>();
                
                var barcodeService = barcodeFactory.GetBarcodeService(request.BarcodeImplementation);
                
                // Create a mock CheckOrderReport from the request
                var checkOrderReport = new CheckOrderReport
                {
                    ProductTypeName = "Default",
                    CheckType = "Standard",
                    FormType = "Default",
                    CheckOrder = new Captive.Data.Models.CheckOrders
                    {
                        AccountNo = request.AccountNo,
                        AccountName = "",
                        BRSTN = request.BRSTN,
                        OrderQuanity = 1
                    },
                    StartSeries = request.StartSeries,
                    EndSeries = request.EndSeries,
                    FileInitial = "DEFAULT",
                    BankBranch = new Captive.Data.Models.BankBranches
                    {
                        BranchName = "Default",
                        BRSTNCode = request.BRSTN,
                        BranchStatus = Captive.Data.Enums.BranchStatus.Active
                    },
                    FormCheckType = Captive.Data.Enums.FormCheckType.Personal
                };

                var barcode = await barcodeService.GenerateBarcode(checkOrderReport);
                
                return BarcodeGenerationResponse.Success(
                    request.RequestId,
                    request.CorrelationId ?? "",
                    barcode,
                    request.BarcodeImplementation,
                    TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode for request {RequestId}", request.RequestId);
                return BarcodeGenerationResponse.Error(
                    request.RequestId,
                    request.CorrelationId ?? "",
                    ex.Message,
                    "BARCODE_GENERATION_ERROR");
            }
        }

        private async Task SendResponse(string replyToQueue, BarcodeGenerationResponse response, string? correlationId)
        {
            try
            {
                if (_channel == null)
                {
                    _logger.LogError("Cannot send response: RabbitMQ channel is null");
                    return;
                }

                var responseJson = JsonConvert.SerializeObject(response);
                var responseBody = Encoding.UTF8.GetBytes(responseJson);

                var properties = _channel.CreateBasicProperties();
                properties.CorrelationId = correlationId ?? response.CorrelationId;
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: replyToQueue,
                    basicProperties: properties,
                    body: responseBody);

                _logger.LogDebug("Sent response to queue {ReplyToQueue} for request {RequestId}", 
                    replyToQueue, response.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending response to queue {ReplyToQueue}", replyToQueue);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service stopping...");
            
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connections");
            }
            
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ Consumer Service stopped");
        }
    }
} 