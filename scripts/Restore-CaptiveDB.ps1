param(
    [string]$BackupPath = "C:\Users\helbe\captive\captive-api\sql-backup\CaptiveDB.bak"
)

$container = "captive-infra-mssql-server-1"
$backupHostPath = $BackupPath
$backupContainerPath = "/var/opt/mssql/backup/CaptiveDB.bak"
$sqlcmd = "/opt/mssql-tools18/bin/sqlcmd"
$user = "sa"
$password = "Password1"
$database = "CaptiveDB"

if (-not (Test-Path $backupHostPath)) {
    throw "Backup file does not exist: $backupHostPath"
}

Write-Host "Copying backup into container..."
docker exec $container mkdir -p /var/opt/mssql/backup | Out-Null
docker cp $backupHostPath "${container}:$backupContainerPath"

Write-Host "Reading logical file names from backup..."
$fileList = docker exec $container $sqlcmd -S localhost -U $user -P $password -C -W -s "|" -Q "RESTORE FILELISTONLY FROM DISK = N'$backupContainerPath'"

if (-not $fileList) {
    throw "Could not read backup file list."
}

$dataLogical = $null
$logLogical = $null

foreach ($line in $fileList) {
    if ($line -match "^\s*LogicalName\|") { continue }
    if ($line -match "^-+\|") { continue }

    $parts = $line -split "\|"
    if ($parts.Length -lt 3) { continue }

    $logicalName = $parts[0].Trim()
    $type = $parts[2].Trim()

    if ($type -eq "D" -and -not $dataLogical) { $dataLogical = $logicalName }
    if ($type -eq "L" -and -not $logLogical) { $logLogical = $logicalName }
}

if (-not $dataLogical -or -not $logLogical) {
    throw "Could not determine logical data/log file names from backup."
}

Write-Host "Data logical name: $dataLogical"
Write-Host "Log logical name:  $logLogical"

$restoreSql = @"
IF DB_ID(N'$database') IS NOT NULL
BEGIN
    ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END;

RESTORE DATABASE [$database]
FROM DISK = N'$backupContainerPath'
WITH
    MOVE N'$dataLogical' TO N'/var/opt/mssql/data/$database.mdf',
    MOVE N'$logLogical' TO N'/var/opt/mssql/data/${database}_log.ldf',
    REPLACE,
    RECOVERY;

ALTER DATABASE [$database] SET MULTI_USER;
"@

Write-Host "Restoring database..."
docker exec $container $sqlcmd -S localhost -U $user -P $password -C -Q $restoreSql

Write-Host "Verifying database..."
docker exec $container $sqlcmd -S localhost -U $user -P $password -C -Q "SELECT name FROM sys.databases WHERE name = N'$database';"
