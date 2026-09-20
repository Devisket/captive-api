<#
.SYNOPSIS
    Deploys the Captive APIs to IIS and the Orchestrator as a Windows Service.

.DESCRIPTION
    Consumes an artifact folder produced by Publish-Captive.ps1 and:
      - stops the affected IIS app pools and the Orchestrator service
      - mirrors the published output into C:\Captive\apps\<App>
      - applies the per-server config from deploy\config\
      - creates/updates the IIS app pools, site and applications
      - installs/updates the Orchestrator service via WinSW
      - starts everything and runs a smoke test

    Idempotent: safe to re-run for every deployment.
    Requires an elevated PowerShell session.

.PARAMETER ArtifactPath
    Folder produced by Publish-Captive.ps1 (contains Commands\, Query\, MdbApi\,
    Orchestrator\).

.PARAMETER InstallRoot
    Base install folder. Default C:\Captive

.PARAMETER SiteName
    IIS site name. Default 'Captive'

.PARAMETER HttpPort
    HTTP port for the IIS site. Default 8080 (avoids clashing with Default Web Site).

.PARAMETER Environment
    ASPNETCORE_ENVIRONMENT / DOTNET_ENVIRONMENT value.
    'Development' exposes Swagger and detailed errors - useful on a test server.
    'Production' hides both. Default Development.

.PARAMETER CertThumbprint
    Optional. Thumbprint of a certificate in LocalMachine\My to bind on HTTPS.
    Without it the site is HTTP-only (see README on UseHttpsRedirection).

.PARAMETER HttpsPort
    HTTPS port when CertThumbprint is supplied. Default 8443.

.PARAMETER Apps
    Subset of apps to deploy. Default all four.

.PARAMETER SkipSmokeTest
    Do not probe the endpoints after starting.

.EXAMPLE
    .\Deploy-Captive.ps1 -ArtifactPath C:\Captive\artifacts\20260825-143000

.EXAMPLE
    .\Deploy-Captive.ps1 -ArtifactPath C:\Captive\artifacts\20260825-143000 -Apps Orchestrator
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ArtifactPath,

    [string]$InstallRoot = 'C:\Captive',
    [string]$SiteName    = 'Captive',
    [int]$HttpPort       = 8080,

    [ValidateSet('Development','Production')]
    [string]$Environment = 'Development',

    [string]$CertThumbprint,
    [int]$HttpsPort = 8443,

    [ValidateSet('Commands','Query','MdbApi','Orchestrator')]
    [string[]]$Apps = @('Commands','Query','MdbApi','Orchestrator'),

    [switch]$SkipSmokeTest
)

$ErrorActionPreference = 'Stop'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

function Write-Step { param([string]$m) Write-Host "`n=== $m ===" -ForegroundColor Cyan }
function Write-Ok   { param([string]$m) Write-Host "  [ok] $m" -ForegroundColor Green }
function Write-Warn { param([string]$m) Write-Host "  [!!] $m" -ForegroundColor Yellow }
function Write-Info { param([string]$m) Write-Host "  $m" -ForegroundColor Gray }

# =============================================================================
# App definitions
# =============================================================================
# 'Enable32Bit' is set for MdbApi: it targets net8.0-windows/x86 and talks to
# Access via OleDb, which requires the 32-bit ACE provider.
$appDefs = [ordered]@{
    'Commands' = @{
        Kind        = 'iis'
        VirtualPath = '/commands'
        AppPool     = 'CaptiveCommands'
        Enable32Bit = $false
        Assembly    = 'Captive.Commands.dll'
        ConfigFile  = 'Commands.appsettings.json'
        Probe       = '/commands/swagger/index.html'
    }
    'Query' = @{
        Kind        = 'iis'
        VirtualPath = '/query'
        AppPool     = 'CaptiveQuery'
        Enable32Bit = $false
        Assembly    = 'Captive.Queries.dll'          # folder is Captive.Query, assembly is Captive.Queries
        ConfigFile  = 'Query.appsettings.json'
        Probe       = '/query/swagger/index.html'
    }
    'MdbApi' = @{
        Kind        = 'iis'
        VirtualPath = '/mdbapi'
        AppPool     = 'CaptiveMdbApi'
        Enable32Bit = $true
        Assembly    = 'Captive.MdbAPI.dll'
        ConfigFile  = 'MdbApi.appsettings.json'
        Probe       = '/mdbapi/swagger/index.html'
    }
    'Orchestrator' = @{
        Kind        = 'service'
        ServiceId   = 'CaptiveOrchestrator'
        Assembly    = 'Captive.Orchestrator.dll'
        ConfigFile  = 'Orchestrator.appsettings.json'
    }
}

# =============================================================================
# Preflight
# =============================================================================
Write-Step 'Preflight checks'

$identity  = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'This script must be run from an elevated (Administrator) PowerShell session.'
}
Write-Ok 'running elevated'

if (-not (Test-Path $ArtifactPath)) { throw "Artifact path not found: $ArtifactPath" }
Write-Ok "artifact path: $ArtifactPath"

$iisApps = $Apps | Where-Object { $appDefs[$_].Kind -eq 'iis' }
if ($iisApps) {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Ok 'WebAdministration module loaded'

    $ancm = Join-Path $env:SystemRoot 'System32\inetsrv\aspnetcorev2.dll'
    if (-not (Test-Path $ancm)) {
        throw "ASP.NET Core Module v2 not found. Install the .NET 8 Hosting Bundle, then re-run. (Install-Prerequisites.ps1 checks this.)"
    }
    Write-Ok 'ASP.NET Core Module v2 present'
}

# Validate every app has its artifact and config template before touching anything.
foreach ($app in $Apps) {
    $def = $appDefs[$app]
    $src = Join-Path $ArtifactPath $app
    if (-not (Test-Path $src)) { throw "Artifact folder missing for ${app}: $src" }
    if (-not (Test-Path (Join-Path $src $def.Assembly))) {
        throw "Expected assembly missing: $(Join-Path $src $def.Assembly)"
    }
    $cfg = Join-Path $scriptRoot "config\$($def.ConfigFile)"
    if (-not (Test-Path $cfg)) { throw "Config template missing: $cfg" }

    $raw = Get-Content $cfg -Raw
    if ($raw -match 'CHANGEME') {
        throw "Config template still contains CHANGEME placeholders: $cfg`nEdit it for this server before deploying."
    }
    try { $null = $raw | ConvertFrom-Json }
    catch { throw "Config template is not valid JSON: $cfg`n$($_.Exception.Message)" }

    Write-Ok "$app artifact and config validated"
}

if ($Apps -contains 'Orchestrator') {
    $winswSrc = Join-Path $scriptRoot 'winsw\WinSW.exe'
    if (-not (Test-Path $winswSrc)) {
        throw "WinSW.exe not found at $winswSrc. Run Install-Prerequisites.ps1 or download it manually."
    }
    Write-Ok 'WinSW.exe present'
}

# =============================================================================
# Stop
# =============================================================================
Write-Step 'Stopping targets'

foreach ($app in $iisApps) {
    $pool = $appDefs[$app].AppPool
    if (Test-Path "IIS:\AppPools\$pool") {
        $state = (Get-WebAppPoolState -Name $pool).Value
        if ($state -eq 'Started') {
            Stop-WebAppPool -Name $pool
            # Wait for w3wp to release file handles, or robocopy will fail.
            $waited = 0
            while ((Get-WebAppPoolState -Name $pool).Value -ne 'Stopped' -and $waited -lt 30) {
                Start-Sleep -Seconds 1; $waited++
            }
            Write-Ok "app pool $pool stopped"
        }
        else { Write-Info "app pool $pool already $state" }
    }
    else { Write-Info "app pool $pool does not exist yet" }
}

if ($Apps -contains 'Orchestrator') {
    $svc = Get-Service -Name $appDefs['Orchestrator'].ServiceId -ErrorAction SilentlyContinue
    if ($svc -and $svc.Status -ne 'Stopped') {
        Stop-Service -Name $svc.Name -Force
        $svc.WaitForStatus('Stopped', '00:00:45')
        Write-Ok 'CaptiveOrchestrator stopped'
    }
    elseif ($svc) { Write-Info 'CaptiveOrchestrator already stopped' }
    else { Write-Info 'CaptiveOrchestrator not installed yet' }
}

# =============================================================================
# Copy files
# =============================================================================
Write-Step 'Copying application files'

foreach ($app in $Apps) {
    $src  = Join-Path $ArtifactPath $app
    $dest = Join-Path $InstallRoot "apps\$app"

    if (-not (Test-Path $dest)) { New-Item -ItemType Directory -Path $dest -Force | Out-Null }

    # /MIR removes files left behind by previous versions. Nothing that must
    # survive a deploy lives in these folders - logs go to $InstallRoot\logs.
    & robocopy $src $dest /MIR /NFL /NDL /NJH /NJS /NP /R:3 /W:2 | Out-Null
    $rc = $LASTEXITCODE

    # Robocopy: 0-7 are success, 8+ are real failures.
    if ($rc -ge 8) { throw "robocopy failed for $app with exit code $rc" }
    Write-Ok "$app -> $dest (robocopy rc=$rc)"
}

# =============================================================================
# Apply configuration
# =============================================================================
Write-Step 'Applying server configuration'

foreach ($app in $Apps) {
    $def  = $appDefs[$app]
    $dest = Join-Path $InstallRoot "apps\$app"
    $cfg  = Join-Path $scriptRoot "config\$($def.ConfigFile)"

    if ($def.Kind -eq 'service') {
        # Orchestrator: Program.cs appends .AddJsonFile("appsettings.json") AFTER
        # the host defaults, making appsettings.json the highest-priority source.
        # An appsettings.<Environment>.json overlay would be silently ignored, so
        # the base file is replaced outright.
        $target = Join-Path $dest 'appsettings.json'
        Copy-Item $cfg $target -Force
        Write-Ok "Orchestrator: appsettings.json replaced (overlay would not win - see README)"
    }
    else {
        $target = Join-Path $dest "appsettings.$Environment.json"
        Copy-Item $cfg $target -Force
        Write-Ok "${app}: appsettings.$Environment.json written"
    }
}

# =============================================================================
# IIS: app pools
# =============================================================================
if ($iisApps) {
    Write-Step 'Configuring IIS application pools'

    foreach ($app in $iisApps) {
        $def  = $appDefs[$app]
        $pool = $def.AppPool

        if (-not (Test-Path "IIS:\AppPools\$pool")) {
            New-WebAppPool -Name $pool | Out-Null
            Write-Ok "created app pool $pool"
        }

        # "No Managed Code": ASP.NET Core does not use the .NET Framework CLR.
        # This is also what OpenTelemetry auto-instrumentation requires later.
        Set-ItemProperty "IIS:\AppPools\$pool" -Name managedRuntimeVersion -Value ''

        # 32-bit only for MdbApi (x86 + OleDb/ACE).
        Set-ItemProperty "IIS:\AppPools\$pool" -Name enable32BitAppOnWin64 -Value $def.Enable32Bit

        # Load the profile so DPAPI, temp files and COM activation behave.
        Set-ItemProperty "IIS:\AppPools\$pool" -Name processModel.loadUserProfile -Value $true

        # These are long-running backend services, not idle-able web sites.
        Set-ItemProperty "IIS:\AppPools\$pool" -Name startMode -Value 'AlwaysRunning'
        Set-ItemProperty "IIS:\AppPools\$pool" -Name processModel.idleTimeout -Value ([TimeSpan]::Zero)

        # Disable the default 29-hour scheduled recycle: it drops SignalR
        # connections at an arbitrary time of day.
        Set-ItemProperty "IIS:\AppPools\$pool" -Name recycling.periodicRestart.time -Value ([TimeSpan]::Zero)

        Write-Ok "$pool configured (32-bit: $($def.Enable32Bit), No Managed Code, AlwaysRunning)"

        # ASPNETCORE_ENVIRONMENT via app pool env vars survives republishing,
        # unlike edits to the generated web.config. Requires IIS 10 / Server 2016+.
        try {
            $envCollection = "system.applicationHost/applicationPools/add[@name='$pool']/environmentVariables"
            Clear-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter $envCollection -ErrorAction SilentlyContinue
            Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter $envCollection `
                -Value @{ name = 'ASPNETCORE_ENVIRONMENT'; value = $Environment } -ErrorAction Stop
            Write-Ok "$pool ASPNETCORE_ENVIRONMENT=$Environment"
        }
        catch {
            Write-Warn "Could not set app pool environment variables (needs IIS 10+): $($_.Exception.Message)"
            Write-Warn "Set ASPNETCORE_ENVIRONMENT=$Environment by hand, or the app runs as Production."
        }
    }
}

# =============================================================================
# IIS: site and applications
# =============================================================================
if ($iisApps) {
    Write-Step "Configuring IIS site '$SiteName'"

    $siteRoot = Join-Path $InstallRoot 'wwwroot'
    if (-not (Test-Path $siteRoot)) {
        New-Item -ItemType Directory -Path $siteRoot -Force | Out-Null
        Set-Content -Path (Join-Path $siteRoot 'index.html') `
                    -Value '<h1>Captive backend</h1><p>APIs are hosted under /commands, /query and /mdbapi.</p>' `
                    -Encoding UTF8
    }

    if (-not (Get-Website -Name $SiteName -ErrorAction SilentlyContinue)) {
        # Warn about a port clash before creating a site that cannot start.
        $conflict = Get-Website | Where-Object {
            $_.Bindings.Collection | Where-Object { $_.bindingInformation -like "*:${HttpPort}:*" }
        }
        if ($conflict) {
            Write-Warn "Port $HttpPort is already bound by site '$($conflict.Name)'. Stop it or pass -HttpPort."
        }

        New-Website -Name $SiteName -Port $HttpPort -PhysicalPath $siteRoot -ApplicationPool 'DefaultAppPool' | Out-Null
        Write-Ok "created site $SiteName on port $HttpPort"
    }
    else {
        Write-Info "site $SiteName already exists"
    }

    if ($CertThumbprint) {
        $cert = Get-ChildItem "Cert:\LocalMachine\My\$CertThumbprint" -ErrorAction SilentlyContinue
        if (-not $cert) {
            Write-Warn "Certificate $CertThumbprint not found in LocalMachine\My - skipping HTTPS binding."
        }
        else {
            $existing = Get-WebBinding -Name $SiteName -Protocol https -ErrorAction SilentlyContinue
            if (-not $existing) {
                New-WebBinding -Name $SiteName -Protocol https -Port $HttpsPort | Out-Null
            }
            # Bind the cert at the HTTP.SYS level.
            $bindingPath = "IIS:\SslBindings\0.0.0.0!$HttpsPort"
            if (Test-Path $bindingPath) { Remove-Item $bindingPath -Force }
            $cert | New-Item $bindingPath | Out-Null
            Write-Ok "HTTPS binding on port $HttpsPort using $($cert.Subject)"
        }
    }
    else {
        Write-Info 'No -CertThumbprint given: site is HTTP-only.'
        Write-Info 'UseHttpsRedirection cannot resolve an HTTPS port and will pass requests through.'
    }

    foreach ($app in $iisApps) {
        $def      = $appDefs[$app]
        $physical = Join-Path $InstallRoot "apps\$app"
        $existing = Get-WebApplication -Site $SiteName -Name $def.VirtualPath.TrimStart('/') -ErrorAction SilentlyContinue

        if ($existing) {
            # IIS: provider paths use backslashes; VirtualPath is '/commands'.
            $iisPath = "IIS:\Sites\$SiteName\" + $def.VirtualPath.TrimStart('/')
            Set-ItemProperty $iisPath -Name physicalPath    -Value $physical
            Set-ItemProperty $iisPath -Name applicationPool -Value $def.AppPool
            Write-Ok "updated application $($def.VirtualPath)"
        }
        else {
            New-WebApplication -Site $SiteName -Name $def.VirtualPath.TrimStart('/') `
                               -PhysicalPath $physical -ApplicationPool $def.AppPool | Out-Null
            Write-Ok "created application $($def.VirtualPath) -> $physical"
        }
    }
}

# =============================================================================
# Filesystem permissions
# =============================================================================
Write-Step 'Granting filesystem permissions'

$dataFolders = @(
    (Join-Path $InstallRoot 'processing'),
    (Join-Path $InstallRoot 'Reports'),
    (Join-Path $InstallRoot 'archive'),
    (Join-Path $InstallRoot 'logs')
)

foreach ($app in $iisApps) {
    $poolIdentity = "IIS AppPool\$($appDefs[$app].AppPool)"
    foreach ($folder in $dataFolders) {
        if (-not (Test-Path $folder)) { New-Item -ItemType Directory -Path $folder -Force | Out-Null }
        & icacls $folder /grant "${poolIdentity}:(OI)(CI)M" /T /Q 2>&1 | Out-Null
    }
    # Read+execute on its own binaries.
    & icacls (Join-Path $InstallRoot "apps\$app") /grant "${poolIdentity}:(OI)(CI)RX" /T /Q 2>&1 | Out-Null
    Write-Ok "granted $poolIdentity"
}

if ($Apps -contains 'Orchestrator') {
    # LocalSystem by default. Swap for the domain account if the WinSW
    # <serviceaccount> block is enabled.
    foreach ($folder in $dataFolders) {
        & icacls $folder /grant 'SYSTEM:(OI)(CI)M' /T /Q 2>&1 | Out-Null
    }
    Write-Ok 'granted SYSTEM (Orchestrator service identity)'
}

# =============================================================================
# Orchestrator Windows Service
# =============================================================================
if ($Apps -contains 'Orchestrator') {
    Write-Step 'Installing Orchestrator Windows Service'

    $svcDir  = Join-Path $InstallRoot 'apps\Orchestrator'
    $svcExe  = Join-Path $svcDir 'CaptiveOrchestrator.exe'
    $svcXml  = Join-Path $svcDir 'CaptiveOrchestrator.xml'
    $svcId   = $appDefs['Orchestrator'].ServiceId

    # WinSW locates its config by matching the XML filename to its own.
    Copy-Item (Join-Path $scriptRoot 'winsw\WinSW.exe') $svcExe -Force

    $xml = Get-Content (Join-Path $scriptRoot 'winsw\Captive.Orchestrator.xml') -Raw
    $xml = $xml.Replace('{{INSTALL_ROOT}}', $InstallRoot).Replace('{{ENVIRONMENT}}', $Environment)
    Set-Content -Path $svcXml -Value $xml -Encoding UTF8
    Write-Ok "service definition written to $svcXml"

    $existingSvc = Get-Service -Name $svcId -ErrorAction SilentlyContinue
    if ($existingSvc) {
        Write-Info "service $svcId already installed - refreshing registration"
        & $svcExe uninstall | Out-Null
        Start-Sleep -Seconds 3
    }

    & $svcExe install
    if ($LASTEXITCODE -ne 0) { throw "WinSW install failed with exit code $LASTEXITCODE" }
    Write-Ok "service $svcId installed"
}

# =============================================================================
# Start
# =============================================================================
Write-Step 'Starting'

foreach ($app in $iisApps) {
    $pool = $appDefs[$app].AppPool
    Start-WebAppPool -Name $pool
    Write-Ok "app pool $pool started"
}

if ($iisApps -and (Get-Website -Name $SiteName).State -ne 'Started') {
    Start-Website -Name $SiteName
    Write-Ok "site $SiteName started"
}

$orchestratorStartFailed = $false
if ($Apps -contains 'Orchestrator') {
    # Do not let a start failure abort the script - the log tail below is the
    # single most useful thing to see when the service will not come up.
    try {
        Start-Service -Name $appDefs['Orchestrator'].ServiceId -ErrorAction Stop
        $svc = Get-Service -Name $appDefs['Orchestrator'].ServiceId
        $svc.WaitForStatus('Running', '00:00:60')
        Write-Ok "service $($svc.Name) is $($svc.Status)"
    }
    catch {
        $orchestratorStartFailed = $true
        Write-Warn "CaptiveOrchestrator did not reach Running: $($_.Exception.Message)"
        Write-Warn "See the wrapper log at $InstallRoot\logs\Orchestrator\CaptiveOrchestrator.wrapper.log"
    }
}

# =============================================================================
# Smoke test
# =============================================================================
if (-not $SkipSmokeTest -and $iisApps) {
    Write-Step 'Smoke test'
    Write-Info 'Waiting 10s for the app pools to warm up...'
    Start-Sleep -Seconds 10

    $baseUrl = "http://localhost:$HttpPort"
    $failures = 0

    foreach ($app in $iisApps) {
        $probe = $appDefs[$app].Probe

        # Swagger is only mapped when the environment is Development.
        if ($Environment -ne 'Development') {
            $probe = $appDefs[$app].VirtualPath
        }

        $url = "$baseUrl$probe"
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 30
            Write-Ok "$app -> HTTP $($response.StatusCode) ($url)"
        }
        catch {
            # PS 5.1 exposes value__ on the enum; PS 7 wraps an HttpResponseMessage.
            $status = $null
            if ($_.Exception.Response) {
                try   { $status = [int]$_.Exception.Response.StatusCode }
                catch { $status = $null }
            }
            if ($status -and $status -lt 500) {
                # 404 from a bare virtual path is expected when Swagger is off:
                # the app started, it just has no route at /.
                Write-Ok "$app -> HTTP $status ($url) - app is responding"
            }
            else {
                Write-Warn "$app -> FAILED ($url): $($_.Exception.Message)"
                Write-Warn "  check: Event Viewer > Windows Logs > Application, source 'IIS AspNetCore Module V2'"
                $failures++
            }
        }
    }

    if ($failures -gt 0) { Write-Warn "$failures app(s) did not respond." }
    else { Write-Ok 'all IIS apps responding' }
}

if ($Apps -contains 'Orchestrator') {
    $logFile = Join-Path $InstallRoot 'logs\Orchestrator\CaptiveOrchestrator.out.log'
    if (Test-Path $logFile) {
        Write-Step 'Orchestrator log tail'
        Get-Content $logFile -Tail 20 | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
    }
    else {
        Write-Warn "No Orchestrator log yet at $logFile - check the service started cleanly."
    }

    $wrapperLog = Join-Path $InstallRoot 'logs\Orchestrator\CaptiveOrchestrator.wrapper.log'
    if ($orchestratorStartFailed -and (Test-Path $wrapperLog)) {
        Write-Step 'Orchestrator wrapper log tail'
        Get-Content $wrapperLog -Tail 20 | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
    }
}

# =============================================================================
# Summary
# =============================================================================
Write-Step 'Deployment complete'
Write-Host @"
Environment : $Environment
Install root: $InstallRoot
Artifact    : $ArtifactPath

Endpoints (HTTP port $HttpPort):
  http://localhost:$HttpPort/commands
  http://localhost:$HttpPort/query
  http://localhost:$HttpPort/mdbapi

Logs:
  IIS apps     : Event Viewer > Application (source: IIS AspNetCore Module V2)
                 stdout logging is OFF by default - see README to enable it
  Orchestrator : $InstallRoot\logs\Orchestrator\

Service control:
  Get-Service CaptiveOrchestrator
  Restart-Service CaptiveOrchestrator
"@ -ForegroundColor Gray
