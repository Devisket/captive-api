# Test RabbitMQ Message Sender for Captive Barcode Service
# This script sends test messages to the RabbitMQ queue

param(
    [string]$RabbitMQHost = "localhost",
    [int]$RabbitMQPort = 15672,
    [string]$Username = "guest",
    [string]$Password = "guest",
    [string]$VHost = "%2F",
    [string]$ExchangeName = "barcode-exchange",
    [string]$RoutingKey = "barcode.generate",
    [int]$MessageCount = 1
)

$ApiUrl = "http://$RabbitMQHost`:$RabbitMQPort/api"

# Create authorization header
$auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$Username`:$Password"))
$headers = @{
    "Authorization" = "Basic $auth"
    "Content-Type" = "application/json"
}

function Send-BarcodeMessage {
    param(
        [string]$AccountNo,
        [string]$BRSTN,
        [string]$StartSeries = "",
        [string]$EndSeries = "",
        [string]$BarcodeImplementation = "MbtcBarcode"
    )
    
    $requestId = [System.Guid]::NewGuid().ToString()
    $correlationId = [System.Guid]::NewGuid().ToString()
    
    $message = @{
        RequestId = $requestId
        AccountNo = $AccountNo
        BRSTN = $BRSTN
        StartSeries = $StartSeries
        EndSeries = $EndSeries
        BarcodeImplementation = $BarcodeImplementation
        ReplyToQueue = "barcode-response-queue"
        CorrelationId = $correlationId
        RequestTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        Priority = 0
    } | ConvertTo-Json
    
    # Encode message to base64
    $messageBytes = [System.Text.Encoding]::UTF8.GetBytes($message)
    $messageBase64 = [System.Convert]::ToBase64String($messageBytes)
    
    $publishBody = @{
        properties = @{
            delivery_mode = 2
            correlation_id = $correlationId
        }
        routing_key = $RoutingKey
        payload = $messageBase64
        payload_encoding = "base64"
    } | ConvertTo-Json -Depth 3
    
    try {
        $response = Invoke-RestMethod -Uri "$ApiUrl/exchanges/$VHost/$ExchangeName/publish" -Method Post -Headers $headers -Body $publishBody
        
        if ($response.routed) {
            Write-Host "✓ Message sent successfully!" -ForegroundColor Green
            Write-Host "  Request ID: $requestId"
            Write-Host "  Correlation ID: $correlationId"
            Write-Host "  Account: $AccountNo"
            Write-Host "  BRSTN: $BRSTN"
        } else {
            Write-Host "⚠ Message sent but not routed!" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "❌ Failed to send message: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test RabbitMQ connection
try {
    Write-Host "🔄 Testing RabbitMQ connection..." -ForegroundColor Yellow
    $testResponse = Invoke-RestMethod -Uri "$ApiUrl/overview" -Method Get -Headers $headers
    Write-Host "✓ Connected to RabbitMQ: $($testResponse.rabbitmq_version)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Failed to connect to RabbitMQ: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure RabbitMQ Management plugin is enabled and the server is running."
    exit 1
}

Write-Host ""
Write-Host "🚀 Sending test barcode generation messages..." -ForegroundColor Green
Write-Host ""

# Send test messages
for ($i = 1; $i -le $MessageCount; $i++) {
    Write-Host "Sending message $i of $MessageCount..." -ForegroundColor Blue
    
    $accountNo = "123456789$(Get-Random -Minimum 100 -Maximum 999)"
    $brstn = "010$(Get-Random -Minimum 100000 -Maximum 999999)"
    $startSeries = "$(Get-Random -Minimum 1000000 -Maximum 9999999)"
    $endSeries = "$(Get-Random -Minimum 1000000 -Maximum 9999999)"
    
    Send-BarcodeMessage -AccountNo $accountNo -BRSTN $brstn -StartSeries $startSeries -EndSeries $endSeries
    
    if ($i -lt $MessageCount) {
        Start-Sleep -Seconds 1
    }
}

Write-Host ""
Write-Host "📋 Summary:" -ForegroundColor Cyan
Write-Host "  Host: $RabbitMQHost"
Write-Host "  Exchange: $ExchangeName"
Write-Host "  Routing Key: $RoutingKey"
Write-Host "  Messages Sent: $MessageCount"
Write-Host ""
Write-Host "💡 Tips:"
Write-Host "  - Check RabbitMQ Management UI at http://$RabbitMQHost`:15672"
Write-Host "  - Monitor Windows Event Viewer for service logs"
Write-Host "  - Check the barcode-response-queue for responses" 