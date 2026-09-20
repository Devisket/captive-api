<#
.SYNOPSIS
    Publishes the Captive APIs and Orchestrator into a deployable artifact folder.

.DESCRIPTION
    Runs "dotnet publish" for the four deployed projects and drops the output
    into <ArtifactRoot>\<timestamp>\<AppName>\. Requires the .NET 8 SDK on
    whatever machine runs this - a build agent, your workstation, or the test
    server itself.

    Nothing is installed here. Deploy-Captive.ps1 consumes the folder produced.

.PARAMETER SourceRoot
    Repository root (the folder containing captive-api.sln).
    Defaults to the parent of this script's folder.

.PARAMETER ArtifactRoot
    Where to write the published output. Default C:\Captive\artifacts

.PARAMETER Configuration
    Build configuration. Default Release.

.PARAMETER Apps
    Subset of apps to publish. Default all four.

.EXAMPLE
    .\Publish-Captive.ps1
    .\Publish-Captive.ps1 -Apps Orchestrator
#>
[CmdletBinding()]
param(
    [string]$SourceRoot,
    [string]$ArtifactRoot = 'C:\Captive\artifacts',
    [ValidateSet('Release','Debug')]
    [string]$Configuration = 'Release',
    [ValidateSet('Commands','Query','MdbApi','Orchestrator')]
    [string[]]$Apps = @('Commands','Query','MdbApi','Orchestrator')
)

$ErrorActionPreference = 'Stop'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not $SourceRoot) { $SourceRoot = Split-Path -Parent $scriptRoot }

function Write-Step { param([string]$Message) Write-Host "`n=== $Message ===" -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host "  [ok] $Message" -ForegroundColor Green }

# Project layout. Note the Query project file is Captive.Queries.csproj even
# though its folder is Captive.Query - the built assembly is Captive.Queries.dll.
$projects = @{
    'Commands'     = @{ Path = 'Captive.Commands\Captive.Commands.csproj';     Runtime = 'win-x64' }
    'Query'        = @{ Path = 'Captive.Query\Captive.Queries.csproj';         Runtime = 'win-x64' }
    'MdbApi'       = @{ Path = 'Captive.MdbAPI\Captive.MdbAPI.csproj';         Runtime = 'win-x86' }  # x86: OleDb / ACE provider
    'Orchestrator' = @{ Path = 'Captive.Orchestrator\Captive.Orchestrator.csproj'; Runtime = 'win-x64' }
}

# --- Sanity checks -----------------------------------------------------------
$solution = Join-Path $SourceRoot 'captive-api.sln'
if (-not (Test-Path $solution)) {
    throw "captive-api.sln not found at $solution. Pass -SourceRoot explicitly."
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    throw 'The dotnet CLI was not found on PATH. Install the .NET 8 SDK to publish.'
}

$sdkVersions = & dotnet --list-sdks
if (-not ($sdkVersions | Where-Object { $_ -like '8.*' })) {
    Write-Warning 'No .NET 8 SDK detected. Publish may fail.'
    Write-Warning ("Installed SDKs:`n{0}" -f ($sdkVersions -join "`n"))
}

# --- Artifact folder ---------------------------------------------------------
$stamp       = Get-Date -Format 'yyyyMMdd-HHmmss'
$artifactDir = Join-Path $ArtifactRoot $stamp
New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null

Write-Step "Publishing to $artifactDir"
Write-Host "  source        : $SourceRoot"
Write-Host "  configuration : $Configuration"
Write-Host "  apps          : $($Apps -join ', ')"

# --- Restore -----------------------------------------------------------------
Write-Step 'Restoring packages'
& dotnet restore $solution | Out-Host
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed with exit code $LASTEXITCODE" }
Write-Ok 'restore complete'

# --- Publish each app --------------------------------------------------------
foreach ($app in $Apps) {
    $project     = $projects[$app]
    $projectPath = Join-Path $SourceRoot $project.Path
    $outputPath  = Join-Path $artifactDir $app

    if (-not (Test-Path $projectPath)) { throw "Project not found: $projectPath" }

    Write-Step "Publishing $app ($($project.Runtime))"

    # Framework-dependent: the Hosting Bundle supplies the runtime on the server.
    # Piped to Out-Host so dotnet's output does not land on the pipeline -
    # this script returns the artifact path, and nothing else may leak into it.
    & dotnet publish $projectPath `
        --configuration $Configuration `
        --runtime $project.Runtime `
        --self-contained false `
        --output $outputPath `
        /p:PublishSingleFile=false | Out-Host

    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $app with exit code $LASTEXITCODE" }

    $fileCount = (Get-ChildItem $outputPath -Recurse -File).Count
    Write-Ok "$app -> $outputPath ($fileCount files)"
}

# --- Post-publish checks -----------------------------------------------------
Write-Step 'Verifying published output'

$expected = @{
    'Commands'     = 'Captive.Commands.dll'
    'Query'        = 'Captive.Queries.dll'
    'MdbApi'       = 'Captive.MdbAPI.dll'
    'Orchestrator' = 'Captive.Orchestrator.dll'
}

foreach ($app in $Apps) {
    $dll = Join-Path (Join-Path $artifactDir $app) $expected[$app]
    if (Test-Path $dll) { Write-Ok "$($expected[$app]) present" }
    else                { throw "Expected assembly missing: $dll" }
}

# The three APIs need a web.config for the ASP.NET Core Module. The Web SDK
# generates one at publish time; flag its absence early rather than debugging
# a 500.19 on the server.
foreach ($app in ($Apps | Where-Object { $_ -ne 'Orchestrator' })) {
    $webConfig = Join-Path (Join-Path $artifactDir $app) 'web.config'
    if (Test-Path $webConfig) { Write-Ok "$app\web.config generated" }
    else { Write-Warning "$app\web.config was NOT generated - IIS will not be able to start this app." }
}

# Captive.Commands publishes appsettings.Development.json (CopyToPublishDirectory
# is set to Always in the csproj). Harmless when ASPNETCORE_ENVIRONMENT is
# Production, but it does ship dev connection strings to the server.
$devSettings = Join-Path (Join-Path $artifactDir 'Commands') 'appsettings.Development.json'
if (Test-Path $devSettings) {
    Write-Warning 'Commands\appsettings.Development.json was published (csproj forces it).'
    Write-Warning 'It contains development settings. Deploy-Captive.ps1 leaves it in place;'
    Write-Warning 'remove it manually if this server should never run as Development.'
}

Write-Step 'Publish complete'
Write-Host "Artifact folder: $artifactDir" -ForegroundColor Green
Write-Host "Next: .\Deploy-Captive.ps1 -ArtifactPath '$artifactDir'" -ForegroundColor Gray

# Emit the path so callers can capture it: $dir = .\Publish-Captive.ps1
return $artifactDir
