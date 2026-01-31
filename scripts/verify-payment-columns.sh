#!/bin/bash
# Verification script for IsPaid column fix
# This script verifies that all payment tracking columns exist in the Appointments table

set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Payment Tracking Columns Verification${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Get connection string from argument or environment variable
CONNECTION_STRING="${1:-${DATABASE_CONNECTION_STRING}}"

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${YELLOW}No connection string provided.${NC}"
    echo "Usage:"
    echo "  $0 'Host=localhost;Database=primecare;Username=postgres;Password=YourPassword'"
    echo "Or set DATABASE_CONNECTION_STRING environment variable"
    exit 1
fi

# Extract connection parameters
HOST=$(echo "$CONNECTION_STRING" | grep -oP 'Host=\K[^;]+')
DATABASE=$(echo "$CONNECTION_STRING" | grep -oP 'Database=\K[^;]+')
USERNAME=$(echo "$CONNECTION_STRING" | grep -oP 'Username=\K[^;]+')
PASSWORD=$(echo "$CONNECTION_STRING" | grep -oP 'Password=\K[^;]+')

echo -e "${GREEN}Checking database: $DATABASE on $HOST${NC}"
echo ""

# SQL query to check for columns
SQL_QUERY="
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    CASE 
        WHEN is_nullable = 'NO' THEN column_default
        ELSE 'N/A'
    END as default_value
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Appointments'
  AND column_name IN ('IsPaid', 'PaidAt', 'PaidByUserId', 'PaymentReceivedBy', 'PaymentAmount', 'PaymentMethod')
ORDER BY column_name;
"

# Execute query using psql
echo -e "${BLUE}Checking for payment tracking columns...${NC}"
echo ""

export PGPASSWORD="$PASSWORD"
RESULT=$(psql -h "$HOST" -U "$USERNAME" -d "$DATABASE" -t -c "$SQL_QUERY" 2>&1)

if [ $? -eq 0 ]; then
    # Count the number of columns found
    COLUMN_COUNT=$(echo "$RESULT" | grep -c "IsPaid\|PaidAt\|PaidByUserId\|PaymentReceivedBy\|PaymentAmount\|PaymentMethod" || true)
    
    if [ "$COLUMN_COUNT" -eq 6 ]; then
        echo -e "${GREEN}✓ All 6 payment tracking columns found!${NC}"
        echo ""
        echo "Column Details:"
        echo "$RESULT"
        echo ""
        echo -e "${GREEN}========================================${NC}"
        echo -e "${GREEN}Verification PASSED${NC}"
        echo -e "${GREEN}========================================${NC}"
        exit 0
    else
        echo -e "${RED}✗ Missing columns detected!${NC}"
        echo ""
        echo "Found $COLUMN_COUNT out of 6 expected columns:"
        echo "$RESULT"
        echo ""
        echo -e "${YELLOW}========================================${NC}"
        echo -e "${YELLOW}Action Required${NC}"
        echo -e "${YELLOW}========================================${NC}"
        echo ""
        echo "Please apply the migration to fix missing columns:"
        echo "  cd src/MedicSoft.Api"
        echo "  dotnet ef database update"
        echo ""
        echo "Or use the migration script:"
        echo "  ./run-all-migrations.sh"
        echo ""
        echo "For more information, see:"
        echo "  docs/troubleshooting/MISSING_DATABASE_COLUMNS.md"
        exit 1
    fi
else
    echo -e "${RED}✗ Failed to connect to database${NC}"
    echo ""
    echo "Error:"
    echo "$RESULT"
    echo ""
    echo "Please verify:"
    echo "  1. PostgreSQL is running"
    echo "  2. Connection string is correct"
    echo "  3. Database exists"
    echo "  4. User has proper permissions"
    exit 1
fi
