# Captive Barcode Service Build and Deploy Script
# Run as Administrator

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputPath = ".\publish",
    [switch]$SelfContained = $true,
    [switch]$InstallService = $false,
    [switch]$Force = $false
)

$ErrorActionPreference = "Stop"

# Colors for output
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"

function Write-Step {
    param([string]$Message)
    Write-Host "🔄 $Message" -ForegroundColor $Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor $Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor $Red
}

try {
    Write-Host "🚀 Captive Barcode Service Build & Deploy" -ForegroundColor $Green
    Write-Host "========================================" -ForegroundColor $Green
    Write-Host ""

    # Step 1: Clean previous builds
    Write-Step "Cleaning previous builds..."
    if (Test-Path $OutputPath) {
        Remove-Item $OutputPath -Recurse -Force
    }
    dotnet clean --configuration $Configuration
    Write-Success "Clean completed"

    # Step 2: Restore packages
    Write-Step "Restoring NuGet packages..."
    dotnet restore
    Write-Success "Packages restored"

    # Step 3: Build project
    Write-Step "Building project..."
    dotnet build --configuration $Configuration --no-restore
    Write-Success "Build completed"

    # Step 4: Publish project
    Write-Step "Publishing project..."
    $publishArgs = @(
        "publish"
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--output", $OutputPath
        "--no-build"
    )
    
    if ($SelfContained) {
        $publishArgs += "--self-contained"
    }
    
    & dotnet $publishArgs
    Write-Success "Publish completed to: $OutputPath"

    # Step 5: Copy configuration and scripts
    Write-Step "Copying additional files..."
    Copy-Item "appsettings.json" -Destination $OutputPath -Force
    Copy-Item "Scripts" -Destination $OutputPath -Recurse -Force
    Write-Success "Additional files copied"

    # Step 6: Install service if requested
    if ($InstallService) {
        Write-Step "Installing Windows service..."
        
        # Check if running as administrator
        if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
            Write-Error "Service installation requires Administrator privileges!"
            Write-Host "Please run this script as Administrator with -InstallService flag"
            exit 1
        }
        
        $servicePath = Join-Path $OutputPath "Captive.Barcode.exe"
        
        # Run installation script
        Push-Location $OutputPath
        & ".\Scripts\install-service.ps1" -BinaryPath $servicePath
        Pop-Location
        
        Write-Success "Service installation completed"
    }

    Write-Host ""
    Write-Host "🎉 Build and Deploy Completed Successfully!" -ForegroundColor $Green
    Write-Host ""
    Write-Host "📁 Output Location: $OutputPath"
    Write-Host "🔧 Configuration: $Configuration"
    Write-Host "🏗️ Runtime: $Runtime"
    Write-Host "📦 Self-contained: $SelfContained"
    Write-Host ""
    
    if (-not $InstallService) {
        Write-Host "Next Steps:"
        Write-Host "1. Navigate to: $OutputPath"
        Write-Host "2. Run as Administrator: .\Scripts\install-service.ps1"
        Write-Host "3. Or install directly: .\Scripts\install-service.ps1 -BinaryPath `"$(Join-Path $OutputPath "Captive.Barcode.exe")`""
    }
}
catch {
    Write-Error "Build failed: $($_.Exception.Message)"
    exit 1
}

Write-Host ""
Write-Host "Useful commands:"
Write-Host "  Build only: .\build-and-deploy.ps1"
Write-Host "  Build and install: .\build-and-deploy.ps1 -InstallService"
Write-Host "  Debug build: .\build-and-deploy.ps1 -Configuration Debug"
Write-Host "  Framework dependent: .\build-and-deploy.ps1 -SelfContained:`$false" 