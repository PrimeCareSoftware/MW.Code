#!/bin/bash
# =============================================================================
# Encryption Migration Script for Existing Data
# =============================================================================
# Purpose: Encrypt existing sensitive data in the database
# Usage: ./encrypt-existing-data.sh [--batch-size 1000] [--test]
# Author: MedicSoft Development Team
# Date: January 2026
# =============================================================================

set -euo pipefail

# Configuration
BATCH_SIZE=1000
TEST_MODE=false
BACKUP_DIR="./backups/encryption-$(date +%Y%m%d-%H%M%S)"
LOG_FILE="./logs/encryption-migration-$(date +%Y%m%d-%H%M%S).log"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --batch-size)
            BATCH_SIZE="$2"
            shift 2
            ;;
        --test)
            TEST_MODE=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Create directories
mkdir -p "$(dirname "$LOG_FILE")"
mkdir -p "$BACKUP_DIR"

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

log "=== Starting Encryption Migration ==="
log "Batch Size: $BATCH_SIZE"
log "Test Mode: $TEST_MODE"
log "Backup Directory: $BACKUP_DIR"

# Check if database connection is available
if ! dotnet ef database drop --dry-run --project src/MedicSoft.Repository/MedicSoft.Repository.csproj > /dev/null 2>&1; then
    log "ERROR: Database connection failed"
    exit 1
fi

log "Database connection successful"

# Create backup
log "Creating database backup..."
if [ "$TEST_MODE" = false ]; then
    pg_dump -h localhost -U medicsoft -d medicsoft_db -F c -f "$BACKUP_DIR/pre-encryption-backup.dump" 2>> "$LOG_FILE"
    log "Backup created: $BACKUP_DIR/pre-encryption-backup.dump"
else
    log "TEST MODE: Skipping backup"
fi

# Run encryption migration command
log "Starting data encryption..."
if [ "$TEST_MODE" = true ]; then
    log "TEST MODE: Would encrypt data with batch size $BATCH_SIZE"
    log "TEST MODE: No actual encryption performed"
else
    # Execute encryption through .NET application
    dotnet run --project src/MedicSoft.Api/MedicSoft.Api.csproj -- encrypt-data --batch-size "$BATCH_SIZE" 2>&1 | tee -a "$LOG_FILE"
    
    if [ ${PIPESTATUS[0]} -eq 0 ]; then
        log "Encryption completed successfully"
    else
        log "ERROR: Encryption failed"
        log "Restoring from backup..."
        pg_restore -h localhost -U medicsoft -d medicsoft_db -c "$BACKUP_DIR/pre-encryption-backup.dump" 2>> "$LOG_FILE"
        log "Backup restored"
        exit 1
    fi
fi

# Verify encryption
log "Verifying encryption..."
if [ "$TEST_MODE" = false ]; then
    dotnet run --project src/MedicSoft.Api/MedicSoft.Api.csproj -- verify-encryption 2>&1 | tee -a "$LOG_FILE"
    
    if [ ${PIPESTATUS[0]} -eq 0 ]; then
        log "Encryption verification passed"
    else
        log "WARNING: Encryption verification failed"
    fi
fi

log "=== Encryption Migration Complete ==="
log "Log file: $LOG_FILE"
log "Backup: $BACKUP_DIR"

exit 0
