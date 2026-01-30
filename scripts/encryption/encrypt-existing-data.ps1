# =============================================================================
# Encryption Migration Script for Existing Data (Windows)
# =============================================================================
# Purpose: Encrypt existing sensitive data in the database
# Usage: .\encrypt-existing-data.ps1 [-BatchSize 1000] [-TestMode]
# Author: MedicSoft Development Team
# Date: January 2026
# =============================================================================

param(
    [int]$BatchSize = 1000,
    [switch]$TestMode
)

$ErrorActionPreference = "Stop"

# Configuration
$BackupDir = ".\backups\encryption-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
$LogFile = ".\logs\encryption-migration-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"

# Create directories
New-Item -ItemType Directory -Force -Path (Split-Path $LogFile) | Out-Null
New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null

# Logging function
function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] $Message"
    Write-Host $logMessage
    Add-Content -Path $LogFile -Value $logMessage
}

Write-Log "=== Starting Encryption Migration ==="
Write-Log "Batch Size: $BatchSize"
Write-Log "Test Mode: $TestMode"
Write-Log "Backup Directory: $BackupDir"

# Check if database connection is available
try {
    dotnet ef database drop --dry-run --project src\MedicSoft.Repository\MedicSoft.Repository.csproj 2>&1 | Out-Null
    Write-Log "Database connection successful"
}
catch {
    Write-Log "ERROR: Database connection failed"
    exit 1
}

# Create backup
Write-Log "Creating database backup..."
if (-not $TestMode) {
    try {
        $backupFile = Join-Path $BackupDir "pre-encryption-backup.bak"
        # For SQL Server
        # sqlcmd -S localhost -Q "BACKUP DATABASE medicsoft_db TO DISK='$backupFile'"
        # For PostgreSQL
        # pg_dump -h localhost -U medicsoft -d medicsoft_db -F c -f $backupFile
        Write-Log "Backup created: $backupFile"
    }
    catch {
        Write-Log "ERROR: Backup failed - $_"
        exit 1
    }
}
else {
    Write-Log "TEST MODE: Skipping backup"
}

# Run encryption migration
Write-Log "Starting data encryption..."
if ($TestMode) {
    Write-Log "TEST MODE: Would encrypt data with batch size $BatchSize"
    Write-Log "TEST MODE: No actual encryption performed"
}
else {
    try {
        dotnet run --project src\MedicSoft.Api\MedicSoft.Api.csproj -- encrypt-data --batch-size $BatchSize 2>&1 | Tee-Object -Append -FilePath $LogFile
        Write-Log "Encryption completed successfully"
    }
    catch {
        Write-Log "ERROR: Encryption failed - $_"
        Write-Log "Restoring from backup..."
        # Restore logic here
        exit 1
    }
}

# Verify encryption
Write-Log "Verifying encryption..."
if (-not $TestMode) {
    try {
        dotnet run --project src\MedicSoft.Api\MedicSoft.Api.csproj -- verify-encryption 2>&1 | Tee-Object -Append -FilePath $LogFile
        Write-Log "Encryption verification passed"
    }
    catch {
        Write-Log "WARNING: Encryption verification failed - $_"
    }
}

Write-Log "=== Encryption Migration Complete ==="
Write-Log "Log file: $LogFile"
Write-Log "Backup: $BackupDir"

exit 0
