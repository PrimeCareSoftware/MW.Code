#!/bin/bash
# Script to apply ticket system migration
# Usage: ./run-ticket-migration.sh [connection_string]

set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Get the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MIGRATION_FILE="$SCRIPT_DIR/migrations/20251221_add_ticket_system.sql"

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Ticket System Migration Script${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

# Check if migration file exists
if [ ! -f "$MIGRATION_FILE" ]; then
    echo -e "${RED}ERROR: Migration file not found: $MIGRATION_FILE${NC}"
    exit 1
fi

# Get connection string from argument or environment variable
CONNECTION_STRING="${1:-${DATABASE_CONNECTION_STRING}}"

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${YELLOW}No connection string provided.${NC}"
    echo "Usage:"
    echo "  $0 'Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword'"
    echo "Or set DATABASE_CONNECTION_STRING environment variable"
    exit 1
fi

echo "Connection string: ${CONNECTION_STRING:0:50}..."
echo ""

# Parse connection string to extract psql parameters
# This is a simple parser - adjust as needed for your connection string format
HOST=$(echo "$CONNECTION_STRING" | grep -oP 'Host=\K[^;]+' || echo "localhost")
DATABASE=$(echo "$CONNECTION_STRING" | grep -oP 'Database=\K[^;]+' || echo "medicsoft")
USERNAME=$(echo "$CONNECTION_STRING" | grep -oP 'Username=\K[^;]+' || echo "postgres")
PASSWORD=$(echo "$CONNECTION_STRING" | grep -oP 'Password=\K[^;]+' || echo "")
PORT=$(echo "$CONNECTION_STRING" | grep -oP 'Port=\K[^;]+' || echo "5432")

echo "Database Host: $HOST"
echo "Database Name: $DATABASE"
echo "Database User: $USERNAME"
echo "Database Port: $PORT"
echo ""

# Check if psql is available
if ! command -v psql &> /dev/null; then
    echo -e "${RED}ERROR: psql command not found. Please install PostgreSQL client.${NC}"
    exit 1
fi

# Check if the Tickets table already exists
echo "Checking if migration has already been applied..."
EXPORT PGPASSWORD="$PASSWORD"
TABLE_EXISTS=$(psql -h "$HOST" -p "$PORT" -U "$USERNAME" -d "$DATABASE" -t -c \
    "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Tickets');" 2>/dev/null || echo "f")

if [ "$TABLE_EXISTS" = " t" ]; then
    echo -e "${YELLOW}WARNING: Tickets table already exists. Migration may have been applied already.${NC}"
    read -p "Do you want to continue anyway? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Migration cancelled."
        exit 0
    fi
fi

# Apply the migration
echo -e "${GREEN}Applying migration...${NC}"
if PGPASSWORD="$PASSWORD" psql -h "$HOST" -p "$PORT" -U "$USERNAME" -d "$DATABASE" -f "$MIGRATION_FILE"; then
    echo ""
    echo -e "${GREEN}✓ Migration applied successfully!${NC}"
    echo ""
    echo "Created tables:"
    echo "  - Tickets"
    echo "  - TicketComments"
    echo "  - TicketAttachments"
    echo "  - TicketHistory"
    echo ""
    echo "With indexes for optimal query performance."
else
    echo ""
    echo -e "${RED}✗ Migration failed!${NC}"
    exit 1
fi

unset PGPASSWORD
