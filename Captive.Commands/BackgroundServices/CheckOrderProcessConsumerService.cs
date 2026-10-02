using Captive.Applications.CheckInventory.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Orderfiles.Services;
using Captive.Data.Enums;
using Captive.Model.Notifications;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Captive.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Commands.BackgroundServices
{
    public class CheckOrderProcessConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly ILogger<CheckOrderProcessConsumerService> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public CheckOrderProcessConsumerService(
            IServiceScopeFactory scopeFactory,
            IRabbitConnectionManager rabbitConnManager,
            ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _rabbitConnManager = rabbitConnManager;
            _logger = loggerFactory.CreateLogger<CheckOrderProcessConsumerService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnManager.GetRabbitMQConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(queue: "CaptiveCheckOrderProcess", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var channel = _channel;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonConvert.DeserializeObject<CheckOrderProcessMessage>(Encoding.UTF8.GetString(body));

                    if (message != null)
                        await ProcessCheckOrderAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing check order message");
                }

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("CaptiveCheckOrderProcess", false, consumer, stoppingToken);
        }

        private async Task ProcessCheckOrderAsync(CheckOrderProcessMessage message, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var readUow = scope.ServiceProvider.GetRequiredService<IReadUnitOfWork>();
            var writeUow = scope.ServiceProvider.GetRequiredService<IWriteUnitOfWork>();
            var checkOrderService = scope.ServiceProvider.GetRequiredService<ICheckOrderService>();
            var checkInventoryService = scope.ServiceProvider.GetRequiredService<ICheckInventoryService>();
            var orderFileService = scope.ServiceProvider.GetRequiredService<IOrderFileService>();
            var orderFileNotifier = scope.ServiceProvider.GetRequiredService<IOrderFileNotifier>();
            var reportGenerator = scope.ServiceProvider.GetRequiredService<IReportGenerator>();

            try
            {
                var orderFile = await readUow.OrderFiles.GetAll()
                    .Include(x => x.BatchFile)
                        .ThenInclude(x => x.BankInfo)
                    .Include(x => x.FloatingCheckOrders)
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == message.OrderFileId, cancellationToken);

                if (orderFile == null)
                {
                    _logger.LogError("Order file {OrderFileId} not found", message.OrderFileId);
                    return;
                }

                await orderFileNotifier.NotifyOrderFileProgress(orderFile.BatchFileId, orderFile.Id, "Creating check orders", cancellationToken);
                await checkOrderService.CreateCheckOrder(orderFile, cancellationToken);

                await orderFileNotifier.NotifyOrderFileProgress(orderFile.BatchFileId, orderFile.Id, "Applying inventory", cancellationToken);
                await checkInventoryService.ApplyCheckInventory(orderFile, cancellationToken);

                await orderFileService.UpdateOrderFileStatus(orderFile.Id, OrderFilesStatus.GeneratingReport, cancellationToken);
                await writeUow.Complete(cancellationToken);

                await orderFileNotifier.NotifyOrderFileProgress(orderFile.BatchFileId, orderFile.Id, "Generating barcodes", cancellationToken);
                await reportGenerator.GenerateBarcode(orderFile.BatchFile!.BankInfo!, orderFile.BatchFileId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error processing check order {OrderFileId}", message.OrderFileId);

                try
                {
                    var orderFileService2 = scope.ServiceProvider.GetRequiredService<IOrderFileService>();
                    var writeUow2 = scope.ServiceProvider.GetRequiredService<IWriteUnitOfWork>();
                    await orderFileService2.UpdateOrderFileStatus(message.OrderFileId, ex.Message, cancellationToken);
                    await writeUow2.Complete(cancellationToken);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Failed to set error status for order file {OrderFileId}", message.OrderFileId);
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
