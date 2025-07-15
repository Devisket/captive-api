# Captive Barcode Service Uninstallation Script
# Run as Administrator

param(
    [string]$ServiceName = "CaptiveBarcodeService"
)

# Check if running as administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "This script must be run as Administrator!"
    exit 1
}

try {
    # Check if service exists
    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if (-not $service) {
        Write-Warning "Service '$ServiceName' not found."
        exit 0
    }

    Write-Host "Found service: $($service.DisplayName)"
    Write-Host "Current Status: $($service.Status)"

    # Stop the service if it's running
    if ($service.Status -eq "Running") {
        Write-Host "Stopping service..."
        Stop-Service -Name $ServiceName -Force
        
        # Wait for service to stop
        $timeout = 30
        $elapsed = 0
        while ($service.Status -ne "Stopped" -and $elapsed -lt $timeout) {
            Start-Sleep -Seconds 1
            $elapsed++
            $service = Get-Service -Name $ServiceName
        }
        
        if ($service.Status -ne "Stopped") {
            Write-Warning "Service did not stop within $timeout seconds. Forcing removal..."
        } else {
            Write-Host "✓ Service stopped successfully"
        }
    }

    # Remove the service
    Write-Host "Removing service..."
    sc.exe delete $ServiceName
    
    # Verify removal
    Start-Sleep -Seconds 2
    $serviceCheck = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($serviceCheck) {
        Write-Error "Failed to remove service. You may need to restart the computer."
        exit 1
    } else {
        Write-Host "✓ Service removed successfully!" -ForegroundColor Green
    }
}
catch {
    Write-Error "Failed to uninstall service: $($_.Exception.Message)"
    exit 1
}

Write-Host ""
Write-Host "Service '$ServiceName' has been completely removed from the system."
Write-Host "You can reinstall it using: .\install-service.ps1" 