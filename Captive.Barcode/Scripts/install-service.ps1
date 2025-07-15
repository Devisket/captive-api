# Captive Barcode Service Installation Script
# Run as Administrator

param(
    [string]$ServiceName = "CaptiveBarcodeService",
    [string]$DisplayName = "Captive Barcode Service",
    [string]$Description = "Captive Barcode Generation Service for processing barcode requests",
    [string]$BinaryPath = ""
)

# Check if running as administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "This script must be run as Administrator!"
    exit 1
}

# Set default binary path if not provided
if ([string]::IsNullOrEmpty($BinaryPath)) {
    $BinaryPath = Join-Path (Get-Location) "Captive.Barcode.exe"
}

# Check if binary exists
if (-not (Test-Path $BinaryPath)) {
    Write-Error "Service binary not found at: $BinaryPath"
    Write-Host "Please build the project first or specify the correct path with -BinaryPath parameter"
    exit 1
}

try {
    # Stop service if it exists
    $existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($existingService) {
        Write-Host "Stopping existing service..."
        Stop-Service -Name $ServiceName -Force
        
        Write-Host "Removing existing service..."
        sc.exe delete $ServiceName
        Start-Sleep -Seconds 2
    }

    # Create the service
    Write-Host "Installing service: $DisplayName"
    New-Service -Name $ServiceName -BinaryPathName $BinaryPath -DisplayName $DisplayName -Description $Description -StartupType Automatic

    # Start the service
    Write-Host "Starting service..."
    Start-Service -Name $ServiceName

    # Check service status
    $service = Get-Service -Name $ServiceName
    Write-Host "Service Status: $($service.Status)"
    
    if ($service.Status -eq "Running") {
        Write-Host "✓ Service installed and started successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Service Details:"
        Write-Host "  Name: $ServiceName"
        Write-Host "  Display Name: $DisplayName"
        Write-Host "  Binary Path: $BinaryPath"
        Write-Host "  Status: $($service.Status)"
        Write-Host ""
        Write-Host "To view logs, check Windows Event Viewer under Applications and Services Logs."
    } else {
        Write-Warning "Service installed but not running. Check Windows Event Viewer for errors."
    }
}
catch {
    Write-Error "Failed to install service: $($_.Exception.Message)"
    exit 1
}

Write-Host ""
Write-Host "Useful commands:"
Write-Host "  View service status: Get-Service -Name $ServiceName"
Write-Host "  Stop service: Stop-Service -Name $ServiceName"
Write-Host "  Start service: Start-Service -Name $ServiceName"
Write-Host "  Uninstall service: .\uninstall-service.ps1" 