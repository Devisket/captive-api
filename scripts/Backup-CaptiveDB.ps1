param(
    [string]$BackupPath = "C:\Users\helbe\captive\captive-api\sql-backup\CaptiveDB.bak"
)

$container = "captive-infra-mssql-server-1"
$backupHostPath = $BackupPath
$backupContainerDir = "/var/opt/mssql/backup"
$backupContainerPath = "/var/opt/mssql/backup/CaptiveDB.bak"
$sqlcmd = "/opt/mssql-tools18/bin/sqlcmd"
$user = "sa"
$password = "Password1"
$database = "CaptiveDB"

Write-Host "Preparing backup directory in container..."
docker exec $container mkdir -p $backupContainerDir | Out-Null

$hostDirectory = Split-Path -Parent $backupHostPath
if (-not (Test-Path $hostDirectory)) {
    New-Item -ItemType Directory -Path $hostDirectory -Force | Out-Null
}

Write-Host "Creating database backup..."
$backupSql = @"
BACKUP DATABASE [$database]
TO DISK = N'$backupContainerPath'
WITH INIT, FORMAT, COMPRESSION, STATS = 10;
"@

docker exec $container $sqlcmd -S localhost -U $user -P $password -C -Q $backupSql

Write-Host "Copying backup to host..."
docker cp "${container}:$backupContainerPath" $backupHostPath

Write-Host "Verifying backup on host..."
Get-Item $backupHostPath | Select-Object FullName, Length, LastWriteTime
