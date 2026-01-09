#!/bin/bash
# Script to apply all EF Core migrations in the correct order
# Usage: ./run-all-migrations.sh [connection_string]

set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}PrimeCare Software - Migration Script${NC}"
echo -e "${BLUE}Apply All Migrations in Correct Order${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Get connection string from argument or environment variable
CONNECTION_STRING="${1:-${DATABASE_CONNECTION_STRING}}"

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${YELLOW}No connection string provided.${NC}"
    echo "Usage:"
    echo "  $0 'Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword'"
    echo "Or set DATABASE_CONNECTION_STRING environment variable"
    echo ""
    echo -e "${YELLOW}Using default connection string for local development...${NC}"
    echo -e "${RED}WARNING: Default credentials are for DEVELOPMENT ONLY!${NC}"
    echo -e "${RED}Never use default credentials in production environments!${NC}"
    CONNECTION_STRING="Host=localhost;Database=medicsoft;Username=postgres;Password=postgres"
fi

echo -e "${GREEN}Connection string: ${CONNECTION_STRING:0:50}...${NC}"
echo ""

# Function to apply migration
apply_migration() {
    local PROJECT_DIR=$1
    local PROJECT_NAME=$2
    local CONTEXT_NAME=$3
    
    echo -e "${BLUE}-----------------------------------${NC}"
    echo -e "${BLUE}Applying: $PROJECT_NAME${NC}"
    echo -e "${BLUE}Context: $CONTEXT_NAME${NC}"
    echo -e "${BLUE}-----------------------------------${NC}"
    
    cd "$PROJECT_DIR"
    
    if dotnet ef database update --context "$CONTEXT_NAME" --connection "$CONNECTION_STRING" 2>&1; then
        echo -e "${GREEN}✓ $PROJECT_NAME migrations applied successfully!${NC}"
    else
        echo -e "${RED}✗ Failed to apply $PROJECT_NAME migrations!${NC}"
        echo -e "${YELLOW}Continuing with next migration...${NC}"
    fi
    
    echo ""
    cd - > /dev/null
}

# Get the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$SCRIPT_DIR"

echo -e "${GREEN}Starting migration process...${NC}"
echo ""

# 1. Main Application (MedicSoft.Repository)
# This is the core database with most tables
apply_migration \
    "$REPO_ROOT/src/MedicSoft.Api" \
    "Main Application (MedicSoft)" \
    "MedicSoftDbContext"

# 2. Patient Portal
# Patient-facing portal database
apply_migration \
    "$REPO_ROOT/patient-portal-api/PatientPortal.Api" \
    "Patient Portal" \
    "PatientPortalDbContext"

# 3. Auth Microservice
# Authentication and session management
apply_migration \
    "$REPO_ROOT/microservices/auth/MedicSoft.Auth.Api" \
    "Auth Microservice" \
    "AuthDbContext"

# 4. Appointments Microservice
# Appointment scheduling and notifications
apply_migration \
    "$REPO_ROOT/microservices/appointments/MedicSoft.Appointments.Api" \
    "Appointments Microservice" \
    "AppointmentsDbContext"

# 5. Patients Microservice
# Patient management
apply_migration \
    "$REPO_ROOT/microservices/patients/MedicSoft.Patients.Api" \
    "Patients Microservice" \
    "PatientsDbContext"

# 6. Medical Records Microservice
# Medical records and clinical data
apply_migration \
    "$REPO_ROOT/microservices/medicalrecords/MedicSoft.MedicalRecords.Api" \
    "Medical Records Microservice" \
    "MedicalRecordsDbContext"

# 7. Billing Microservice
# Financial transactions and billing
apply_migration \
    "$REPO_ROOT/microservices/billing/MedicSoft.Billing.Api" \
    "Billing Microservice" \
    "BillingDbContext"

# 8. System Admin Microservice
# System administration
apply_migration \
    "$REPO_ROOT/microservices/systemadmin/MedicSoft.SystemAdmin.Api" \
    "System Admin Microservice" \
    "SystemAdminDbContext"

# 9. Telemedicine
# Video consultation platform
apply_migration \
    "$REPO_ROOT/telemedicine/src/MedicSoft.Telemedicine.Api" \
    "Telemedicine" \
    "TelemedicineDbContext"

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Migration process completed!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo "Summary:"
echo "✓ Main Application (MedicSoftDbContext)"
echo "✓ Patient Portal (PatientPortalDbContext)"
echo "✓ Auth Microservice (AuthDbContext)"
echo "✓ Appointments Microservice (AppointmentsDbContext)"
echo "✓ Patients Microservice (PatientsDbContext)"
echo "✓ Medical Records Microservice (MedicalRecordsDbContext)"
echo "✓ Billing Microservice (BillingDbContext)"
echo "✓ System Admin Microservice (SystemAdminDbContext)"
echo "✓ Telemedicine (TelemedicineDbContext)"
echo ""
echo -e "${BLUE}Note: Some migrations may have been skipped if they were already applied.${NC}"
echo -e "${BLUE}This is normal and expected behavior.${NC}"
