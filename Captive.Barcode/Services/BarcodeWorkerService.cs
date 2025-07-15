using Captive.Barcode.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Captive.Barcode.Services
{
    public class BarcodeWorkerService : BackgroundService
    {
        private readonly ILogger<BarcodeWorkerService> _logger;
        private readonly IBarcodeServiceFactory _barcodeServiceFactory;
        private readonly BarcodeServiceSettings _settings;

        public BarcodeWorkerService(
            ILogger<BarcodeWorkerService> logger,
            IBarcodeServiceFactory barcodeServiceFactory,
            IOptions<BarcodeServiceSettings> settings)
        {
            _logger = logger;
            _barcodeServiceFactory = barcodeServiceFactory;
            _settings = settings.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Captive Barcode Service starting at: {time}", DateTimeOffset.Now);
            
            // Log available barcode implementations
            var implementations = _barcodeServiceFactory.GetAvailableImplementations();
            _logger.LogInformation("Available barcode implementations: {implementations}", 
                string.Join(", ", implementations));
            
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Captive Barcode Service is running");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Service heartbeat
                    _logger.LogDebug("Barcode service heartbeat at: {time}", DateTimeOffset.Now);
                    
                    // Here you can add periodic tasks like:
                    // - Checking for queued barcode generation requests
                    // - Monitoring console applications
                    // - Cleaning up temporary files
                    // - Health checks
                    
                    await Task.Delay(_settings.HeartbeatIntervalSeconds * 1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in barcode service execution");
                    await Task.Delay(5000, stoppingToken); // Wait before retrying
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Captive Barcode Service stopping at: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }
    }
} 