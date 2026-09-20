<#
.SYNOPSIS
    One-time preparation of a Windows Server host for the Captive backend.

.DESCRIPTION
    Installs the IIS features required by ASP.NET Core, verifies the .NET 8
    Hosting Bundle is present (both x64 and x86 runtimes), creates the
    C:\Captive folder tree, and downloads WinSW for the Orchestrator service.

    Run once per server, as Administrator. Safe to re-run.

.PARAMETER InstallRoot
    Base folder for the deployed applications. Default C:\Captive

.PARAMETER SkipWinSW
    Skip downloading WinSW (use when the server has no internet access and you
    have copied WinSW-x64.exe into deploy\winsw\ by hand).

.EXAMPLE
    .\Install-Prerequisites.ps1
#>
[CmdletBinding()]
param(
    [string]$InstallRoot = 'C:\Captive',
    [switch]$SkipWinSW
)

$ErrorActionPreference = 'Stop'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

function Write-Step { param([string]$Message) Write-Host "`n=== $Message ===" -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host "  [ok] $Message" -ForegroundColor Green }
function Write-Warn { param([string]$Message) Write-Host "  [!!] $Message" -ForegroundColor Yellow }

# --- Administrator check -----------------------------------------------------
$identity  = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'This script must be run from an elevated (Administrator) PowerShell session.'
}

# --- IIS features ------------------------------------------------------------
Write-Step 'Installing IIS features'

$features = @(
    'IIS-WebServerRole',
    'IIS-WebServer',
    'IIS-CommonHttpFeatures',
    'IIS-DefaultDocument',
    'IIS-HttpErrors',
    'IIS-StaticContent',
    'IIS-HttpLogging',
    'IIS-RequestFiltering',
    'IIS-Security',
    'IIS-WebSockets',                 # required by SignalR in Captive.Commands
    'IIS-ApplicationInit',            # lets app pools warm up before first request
    'IIS-ManagementConsole'
)

foreach ($feature in $features) {
    $state = (Get-WindowsOptionalFeature -Online -FeatureName $feature -ErrorAction SilentlyContinue).State
    if ($state -eq 'Enabled') {
        Write-Ok "$feature already enabled"
    }
    else {
        Write-Host "  installing $feature ..."
        Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart | Out-Null
        Write-Ok "$feature enabled"
    }
}

Import-Module WebAdministration -ErrorAction Stop
Write-Ok 'WebAdministration module loaded'

# --- .NET 8 Hosting Bundle ---------------------------------------------------
Write-Step 'Checking .NET 8 Hosting Bundle'

# The ASP.NET Core Module v2 is the reliable signal that the Hosting Bundle
# (as opposed to a bare SDK/runtime install) is present.
$ancmPath = Join-Path $env:SystemRoot 'System32\inetsrv\aspnetcorev2.dll'
if (Test-Path $ancmPath) {
    Write-Ok "ASP.NET Core Module v2 found ($ancmPath)"
}
else {
    Write-Warn 'ASP.NET Core Module v2 NOT found.'
    Write-Warn 'Install the .NET 8 Hosting Bundle before deploying:'
    Write-Warn '  https://dotnet.microsoft.com/download/dotnet/8.0  ->  "Hosting Bundle"'
    Write-Warn 'Install it AFTER IIS. If you install the .NET SDK later, repair the'
    Write-Warn 'Hosting Bundle afterwards or the module registration is lost.'
}

# Captive.MdbAPI runs in a 32-bit app pool, so the x86 runtime must exist too.
# The Hosting Bundle ships both; a plain SDK install ships only its own bitness.
$runtimeChecks = @(
    @{ Label = 'x64 shared runtime'; Path = 'C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App' },
    @{ Label = 'x86 shared runtime'; Path = 'C:\Program Files (x86)\dotnet\shared\Microsoft.AspNetCore.App' }
)

foreach ($check in $runtimeChecks) {
    if (Test-Path $check.Path) {
        $versions = Get-ChildItem $check.Path -Directory |
                    Where-Object { $_.Name -like '8.*' } |
                    Select-Object -ExpandProperty Name
        if ($versions) {
            Write-Ok ("{0}: {1}" -f $check.Label, ($versions -join ', '))
        }
        else {
            Write-Warn ("{0} present but no 8.x version found" -f $check.Label)
        }
    }
    else {
        Write-Warn ("{0} NOT found at {1}" -f $check.Label, $check.Path)
        if ($check.Label -like 'x86*') {
            Write-Warn 'Captive.MdbAPI needs this. Install the Hosting Bundle (it includes x86).'
        }
    }
}

# --- Folder tree -------------------------------------------------------------
Write-Step "Creating folder tree under $InstallRoot"

$folders = @(
    "$InstallRoot",
    "$InstallRoot\apps",
    "$InstallRoot\apps\Commands",
    "$InstallRoot\apps\Query",
    "$InstallRoot\apps\MdbApi",
    "$InstallRoot\apps\Orchestrator",
    "$InstallRoot\apps\BarcodeGenerator",
    "$InstallRoot\logs",
    "$InstallRoot\logs\Orchestrator",
    "$InstallRoot\processing",
    "$InstallRoot\Reports",
    "$InstallRoot\Reports\output",
    "$InstallRoot\archive",
    "$InstallRoot\artifacts"
)

foreach ($folder in $folders) {
    if (Test-Path $folder) {
        Write-Ok "$folder exists"
    }
    else {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Ok "created $folder"
    }
}

# --- WinSW -------------------------------------------------------------------
Write-Step 'Fetching WinSW (Orchestrator service wrapper)'

$winswDir = Join-Path $scriptRoot 'winsw'
$winswExe = Join-Path $winswDir 'WinSW.exe'

if (Test-Path $winswExe) {
    Write-Ok "WinSW already present at $winswExe"
}
elseif ($SkipWinSW) {
    Write-Warn "Skipped. Place WinSW-x64.exe at $winswExe before running Deploy-Captive.ps1"
}
else {
    if (-not (Test-Path $winswDir)) { New-Item -ItemType Directory -Path $winswDir -Force | Out-Null }
    $url = 'https://github.com/winsw/winsw/releases/download/v2.12.0/WinSW-x64.exe'
    try {
        Write-Host "  downloading $url ..."
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -Uri $url -OutFile $winswExe -UseBasicParsing
        Write-Ok "WinSW downloaded to $winswExe"
    }
    catch {
        Write-Warn "Download failed: $($_.Exception.Message)"
        Write-Warn 'Download WinSW-x64.exe manually from https://github.com/winsw/winsw/releases'
        Write-Warn "and save it as $winswExe"
    }
}

# --- Summary -----------------------------------------------------------------
Write-Step 'Prerequisites complete'
Write-Host @"
Next steps:
  1. Edit the config templates in deploy\config\ for this server
     (connection strings, RabbitMQ host, barcode exe path).
  2. Run Publish-Captive.ps1 to produce an artifact folder.
  3. Run Deploy-Captive.ps1 to install the IIS sites and the Orchestrator service.

Still to do by hand on this server (not automated here):
  - Install SQL Server and RabbitMQ, or point the config at existing hosts.
  - Register the 32-bit barcode COM component:
      C:\Windows\SysWOW64\regsvr32.exe $InstallRoot\apps\BarcodeGenerator\BcConfig.dll
  - Copy BarcodeGenerator.exe plus tMasDigits.ini, acct_mapping.txt and
    serial_mapping.txt into $InstallRoot\apps\BarcodeGenerator\
    (the two .txt files are NOT in source control - see README).
"@ -ForegroundColor Gray
