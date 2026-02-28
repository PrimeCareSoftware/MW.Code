#!/bin/bash
# Safe Migration Script - Ensures no concurrent migrations
# Run this instead of dotnet run to apply migrations safely

set -e  # Exit on error

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_DIR="$SCRIPT_DIR/src/MedicSoft.Api"
LOCK_FILE="/tmp/medicsoft_migration.lock"

echo "🔒 Safe Migration Script for MedicSoft"
echo "======================================="
echo ""

# Function to clean up lock file on exit
cleanup() {
    if [ -f "$LOCK_FILE" ]; then
        echo "🧹 Cleaning up lock file..."
        rm -f "$LOCK_FILE"
    fi
}

trap cleanup EXIT

# Check if another migration is in progress
if [ -f "$LOCK_FILE" ]; then
    echo "❌ ERROR: Another migration is already in progress!"
    echo "   Lock file exists: $LOCK_FILE"
    echo "   If you're sure no migration is running, delete this file manually:"
    echo "   rm -f $LOCK_FILE"
    exit 1
fi

# Create lock file
echo $$ > "$LOCK_FILE"
echo "✅ Lock acquired (PID: $$)"
echo ""

# Check for running MedicSoft instances
echo "🔍 Checking for running MedicSoft instances..."
RUNNING_INSTANCES=$(ps aux | grep -i "dotnet.*MedicSoft" | grep -v grep | wc -l)

if [ "$RUNNING_INSTANCES" -gt 0 ]; then
    echo "⚠️  WARNING: Found $RUNNING_INSTANCES running instance(s)"
    echo ""
    ps aux | grep -i "dotnet.*MedicSoft" | grep -v grep
    echo ""
    read -p "Do you want to kill these instances? (y/N): " -n 1 -r
    echo ""
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "🛑 Killing instances..."
        pkill -f "dotnet.*MedicSoft" || true
        sleep 2
        echo "✅ Instances killed"
    else
        echo "❌ Cannot proceed with running instances. Exiting."
        exit 1
    fi
else
    echo "✅ No running instances found"
fi

echo ""
echo "📊 Database Connection Info:"
echo "   You can check active connections with:"
echo "   psql -d your_database -f check_db_locks.sql"
echo ""

read -p "Continue with migration? (y/N): " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Migration cancelled by user"
    exit 0
fi

echo ""
echo "🚀 Starting migration..."
echo "   Working directory: $PROJECT_DIR"
echo "   Time: $(date)"
echo ""

cd "$PROJECT_DIR"

# Run the application (which will apply migrations)
dotnet run --no-build

EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "✅ Migration completed successfully!"
else
    echo ""
    echo "❌ Migration failed with exit code: $EXIT_CODE"
    echo "   Check the logs at: src/MedicSoft.Api/Logs/"
fi

exit $EXIT_CODE
