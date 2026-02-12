#!/bin/bash
# Script to verify IsException column exists in BlockedTimeSlots table
# Usage: ./verify-isexception-column.sh [database_connection_string]

set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}IsException Column Verification${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Get connection string from argument or environment variable
CONNECTION_STRING="${1:-${DATABASE_CONNECTION_STRING}}"

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${RED}ERROR: No connection string provided.${NC}"
    echo ""
    echo "Usage:"
    echo "  $0 'Host=localhost;Database=primecare;Username=postgres;Password=YourPassword'"
    echo ""
    echo "Or set DATABASE_CONNECTION_STRING environment variable"
    exit 1
fi

# Parse connection string to get psql parameters
# Note: This is a simple parser and may not handle all cases
HOST=$(echo "$CONNECTION_STRING" | grep -oP '(?<=Host=)[^;]+' || echo "localhost")
DATABASE=$(echo "$CONNECTION_STRING" | grep -oP '(?<=Database=)[^;]+' || echo "primecare")
USERNAME=$(echo "$CONNECTION_STRING" | grep -oP '(?<=Username=)[^;]+' || echo "postgres")
PASSWORD=$(echo "$CONNECTION_STRING" | grep -oP '(?<=Password=)[^;]+' || echo "")

echo -e "${BLUE}Connection Details:${NC}"
echo "  Host: $HOST"
echo "  Database: $DATABASE"
echo "  Username: $USERNAME"
echo ""

# Check if psql is available
if ! command -v psql &> /dev/null; then
    echo -e "${RED}ERROR: psql command not found.${NC}"
    echo "Please install PostgreSQL client tools."
    echo ""
    echo "On Ubuntu/Debian: sudo apt-get install postgresql-client"
    echo "On macOS: brew install postgresql"
    exit 1
fi

echo -e "${YELLOW}Checking if IsException column exists...${NC}"
echo ""

# SQL query to check if column exists
SQL_QUERY="
SELECT 
    column_name, 
    data_type, 
    is_nullable, 
    column_default
FROM information_schema.columns 
WHERE table_name = 'BlockedTimeSlots' 
  AND column_name = 'IsException';
"

# Execute query
RESULT=$(PGPASSWORD="$PASSWORD" psql -h "$HOST" -U "$USERNAME" -d "$DATABASE" -t -A -F',' -c "$SQL_QUERY" 2>&1)
EXIT_CODE=$?

if [ $EXIT_CODE -ne 0 ]; then
    echo -e "${RED}ERROR: Failed to connect to database or execute query.${NC}"
    echo ""
    echo "$RESULT"
    exit 1
fi

# Check if column exists
if [ -z "$RESULT" ] || [ "$RESULT" = "" ]; then
    echo -e "${RED}❌ FAILED: IsException column does NOT exist in BlockedTimeSlots table${NC}"
    echo ""
    echo -e "${YELLOW}Action Required:${NC}"
    echo "1. Apply migrations using: ./run-all-migrations.sh \"$CONNECTION_STRING\""
    echo "2. Or manually run: cd src/MedicSoft.Api && dotnet ef database update"
    echo "3. See DEPLOYMENT_CHECKLIST_ISEXCEPTION.md for detailed instructions"
    echo ""
    exit 1
else
    echo -e "${GREEN}✓ SUCCESS: IsException column exists!${NC}"
    echo ""
    echo -e "${BLUE}Column Details:${NC}"
    
    # Parse and display result
    IFS=',' read -r COL_NAME DATA_TYPE IS_NULLABLE COL_DEFAULT <<< "$RESULT"
    echo "  Column Name:    $COL_NAME"
    echo "  Data Type:      $DATA_TYPE"
    echo "  Is Nullable:    $IS_NULLABLE"
    echo "  Default Value:  $COL_DEFAULT"
    echo ""
    
    # Verify expected values
    if [ "$DATA_TYPE" != "boolean" ]; then
        echo -e "${YELLOW}⚠️  WARNING: Expected data type 'boolean', got '$DATA_TYPE'${NC}"
    fi
    
    if [ "$IS_NULLABLE" != "NO" ]; then
        echo -e "${YELLOW}⚠️  WARNING: Column should be NOT NULL, got nullable=${IS_NULLABLE}${NC}"
    fi
    
    if [[ "$COL_DEFAULT" != *"false"* ]]; then
        echo -e "${YELLOW}⚠️  WARNING: Expected default value 'false', got '$COL_DEFAULT'${NC}"
    fi
    
    echo -e "${GREEN}All checks passed! The IsException column is properly configured.${NC}"
fi

echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}Verification Complete${NC}"
echo -e "${BLUE}========================================${NC}"
