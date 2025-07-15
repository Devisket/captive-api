# Captive Barcode Windows Service

## Overview
The Captive Barcode Service is a Windows service that handles barcode generation requests by calling external console applications. It runs in the background and can be managed through Windows Service Manager. The service now includes **RabbitMQ integration** for receiving barcode generation requests via message queues.

## Features
- ✅ Runs as a Windows service
- ✅ Configurable through appsettings.json
- ✅ Supports multiple barcode implementations
- ✅ Calls external console applications for barcode generation
- ✅ **NEW: RabbitMQ consumer for message-based requests**
- ✅ **NEW: Automatic message acknowledgment and error handling**
- ✅ **NEW: Response queue support for async communication**
- ✅ Comprehensive logging to Windows Event Log
- ✅ Automatic startup on system boot
- ✅ Easy installation/uninstallation scripts

## Prerequisites
- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Runtime
- Administrator privileges for installation
- **RabbitMQ Server** (for message queue functionality)
- COM components for barcode generation (if using legacy components)

## Building the Service

### 1. Build the Project
```powershell
dotnet build Captive.Barcode.csproj --configuration Release
```

### 2. Publish for Deployment
```powershell
dotnet publish Captive.Barcode.csproj -c Release -r win-x64 --self-contained
```

## Installation

### 1. Using PowerShell Scripts (Recommended)
```powershell
# Run as Administrator
cd path\to\Captive.Barcode
.\Scripts\install-service.ps1
```

### 2. Manual Installation
```powershell
# Run as Administrator
sc.exe create CaptiveBarcodeService binpath="C:\path\to\Captive.Barcode.exe" displayname="Captive Barcode Service" start=auto
net start CaptiveBarcodeService
```

## Configuration

### appsettings.json
```json
{
  "BarcodeService": {
    "HeartbeatIntervalSeconds": 30,
    "DefaultBarcodeImplementation": "MbtcBarcode",
    "ConsoleAppPath": "C:\\BarcodeGenerator\\BarcodeGenerator.exe",
    "WorkingDirectory": "C:\\BarcodeGenerator",
    "ProcessTimeoutSeconds": 30,
    "ShowConsoleWindow": false,
    "EnableDetailedLogging": false,
    "TempDirectory": "C:\\Temp\\BarcodeService"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "QueueName": "barcode-generation-queue",
    "ExchangeName": "barcode-exchange",
    "RoutingKey": "barcode.generate",
    "Durable": true,
    "AutoDelete": false,
    "Exclusive": false,
    "PrefetchCount": 1,
    "AutoAck": false,
    "ReconnectDelay": 5000,
    "MaxRetries": 3
  }
}
```

### Configuration Options

#### BarcodeService Section
- **HeartbeatIntervalSeconds**: Service heartbeat interval
- **DefaultBarcodeImplementation**: Default barcode service to use
- **ConsoleAppPath**: Path to the console application for barcode generation
- **WorkingDirectory**: Working directory for the console application
- **ProcessTimeoutSeconds**: Timeout for console application execution
- **ShowConsoleWindow**: Show console window (for debugging)
- **EnableDetailedLogging**: Enable detailed logging
- **TempDirectory**: Temporary directory for service operations

#### RabbitMQ Section
- **HostName**: RabbitMQ server hostname
- **Port**: RabbitMQ server port (default: 5672)
- **UserName/Password**: RabbitMQ credentials
- **VirtualHost**: RabbitMQ virtual host
- **QueueName**: Queue name for barcode generation requests
- **ExchangeName**: Exchange name for routing messages
- **RoutingKey**: Routing key for message routing
- **Durable**: Whether queues/exchanges survive server restart
- **PrefetchCount**: Number of unacknowledged messages per consumer
- **AutoAck**: Automatic message acknowledgment
- **ReconnectDelay**: Delay before reconnection attempts (ms)
- **MaxRetries**: Maximum retry attempts for failed operations

## RabbitMQ Integration

### Message Format

#### Request Message
```json
{
  "RequestId": "550e8400-e29b-41d4-a716-446655440000",
  "AccountNo": "123456789012",
  "BRSTN": "010123456",
  "StartSeries": "1000001",
  "EndSeries": "1000001",
  "BarcodeImplementation": "MbtcBarcode",
  "ReplyToQueue": "barcode-response-queue",
  "CorrelationId": "correlation-123",
  "RequestTime": "2024-01-15T10:30:00.000Z",
  "Priority": 0,
  "AdditionalProperties": {}
}
```

#### Response Message
```json
{
  "RequestId": "550e8400-e29b-41d4-a716-446655440000",
  "CorrelationId": "correlation-123",
  "Success": true,
  "Barcode": "generated-barcode-data",
  "ErrorMessage": null,
  "ErrorCode": null,
  "ProcessedTime": "2024-01-15T10:30:05.123Z",
  "ProcessingDuration": "00:00:05.123",
  "BarcodeImplementation": "MbtcBarcode",
  "AdditionalData": {}
}
```

### Publishing Messages

#### Using PowerShell Test Script
```powershell
# Send a single test message
.\Examples\TestRabbitMQMessage.ps1

# Send multiple test messages
.\Examples\TestRabbitMQMessage.ps1 -MessageCount 5

# Use custom RabbitMQ server
.\Examples\TestRabbitMQMessage.ps1 -RabbitMQHost "production-server" -Username "admin" -Password "password"
```

#### Using .NET Code
```csharp
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

var factory = new ConnectionFactory() 
{ 
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var request = new BarcodeGenerationRequest
{
    RequestId = Guid.NewGuid().ToString(),
    AccountNo = "123456789012",
    BRSTN = "010123456",
    StartSeries = "1000001",
    EndSeries = "1000001",
    BarcodeImplementation = "MbtcBarcode",
    ReplyToQueue = "barcode-response-queue",
    CorrelationId = Guid.NewGuid().ToString()
};

var message = JsonConvert.SerializeObject(request);
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(
    exchange: "barcode-exchange",
    routingKey: "barcode.generate",
    basicProperties: null,
    body: body);
```

## Service Management

### Start/Stop Service
```powershell
# Start service
Start-Service -Name CaptiveBarcodeService

# Stop service
Stop-Service -Name CaptiveBarcodeService

# Check status
Get-Service -Name CaptiveBarcodeService
```

### View Service Logs
1. Open Windows Event Viewer
2. Navigate to Applications and Services Logs
3. Look for "Captive Barcode Service" entries

### Monitor RabbitMQ
- **Management UI**: http://localhost:15672 (guest/guest)
- **Queue Monitoring**: Check message rates and queue lengths
- **Exchange Monitoring**: Monitor message routing

### Uninstall Service
```powershell
# Run as Administrator
.\Scripts\uninstall-service.ps1
```

## Usage

### Direct Service Call
```csharp
// Get the barcode service factory
var barcodeServiceFactory = serviceProvider.GetService<IBarcodeServiceFactory>();

// Get specific barcode implementation
var barcodeService = barcodeServiceFactory.GetBarcodeService("MbtcBarcode");

// Generate barcode
var barcode = await barcodeService.GenerateBarcode(checkOrderReport);
```

### RabbitMQ Message-Based Call
1. Publish a message to the `barcode-generation-queue`
2. Service automatically processes the message
3. Response is sent to the specified `ReplyToQueue`

### Available Implementations
- **MbtcBarcode**: MBTC barcode implementation using console application

## Console Application Requirements

Your console application should:
1. Accept command line arguments in this order:
   - AccountNo
   - BRSTN
   - StartSeries
   - EndSeries

2. Return barcode via `Console.WriteLine(barcode)`

3. Use proper exit codes:
   - 0: Success
   - Non-zero: Error

4. Write errors to `Console.Error`

### Example Console App
```csharp
static void Main(string[] args)
{
    try
    {
        if (args.Length < 4)
        {
            Console.Error.WriteLine("Usage: BarcodeGenerator.exe <AccountNo> <BRSTN> <StartSeries> <EndSeries>");
            Environment.Exit(1);
        }

        string accountNo = args[0];
        string brstn = args[1];
        string startSeries = args[2];
        string endSeries = args[3];

        // Generate barcode
        string barcode = GenerateBarcode(accountNo, brstn, startSeries, endSeries);
        Console.WriteLine(barcode);
        Environment.Exit(0);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
}
```

## Troubleshooting

### Common Issues

1. **Service won't start**
   - Check Windows Event Viewer for errors
   - Verify .NET 8.0 Runtime is installed
   - Ensure proper file permissions

2. **Console application not found**
   - Verify the `ConsoleAppPath` in appsettings.json
   - Check file permissions for the console application

3. **RabbitMQ connection issues**
   - Verify RabbitMQ server is running
   - Check connection settings in appsettings.json
   - Ensure RabbitMQ credentials are correct
   - Check network connectivity and firewall settings

4. **Messages not being processed**
   - Check queue and exchange configuration
   - Verify routing key matches
   - Monitor RabbitMQ Management UI for stuck messages
   - Check service logs for processing errors

5. **Service stops unexpectedly**
   - Check Event Viewer for exceptions
   - Verify console application is working independently
   - Check timeout settings
   - Monitor RabbitMQ connection stability

### Debug Mode
Set `ShowConsoleWindow = true` in appsettings.json to see console output during development.

### RabbitMQ Setup
```bash
# Enable RabbitMQ Management Plugin
rabbitmq-plugins enable rabbitmq_management

# Create custom user (optional)
rabbitmqctl add_user barcodeuser password123
rabbitmqctl set_permissions -p / barcodeuser ".*" ".*" ".*"
```

### Logging
- Service logs to Windows Event Log
- Check Event Viewer under Applications and Services Logs
- Enable detailed logging with `EnableDetailedLogging = true`
- Monitor RabbitMQ logs for connection issues

## Performance Tuning

### Message Processing
- Adjust `PrefetchCount` for throughput vs. load balancing
- Set `AutoAck = false` for reliability vs. `true` for performance
- Monitor queue lengths and processing times

### Console Application
- Optimize console app startup time
- Consider connection pooling for external resources
- Monitor process timeout settings

## Support
For issues and questions:
1. Check Windows Event Log for service errors
2. Monitor RabbitMQ Management UI for queue status
3. Verify configuration settings
4. Check console application functionality independently
5. Review network connectivity for RabbitMQ

The service includes comprehensive error handling and logging for troubleshooting both barcode generation and message queue operations. 