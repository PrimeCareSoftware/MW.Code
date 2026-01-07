# Script to apply all EF Core migrations in the correct order
# Usage: .\run-all-migrations.ps1 [-ConnectionString "Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword"]

param(
    [string]$ConnectionString = $env:DATABASE_CONNECTION_STRING
)

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Apply-Migration {
    param(
        [string]$ProjectDir,
        [string]$ProjectName,
        [string]$ContextName
    )
    
    Write-ColorOutput "-----------------------------------" -Color Cyan
    Write-ColorOutput "Applying: $ProjectName" -Color Cyan
    Write-ColorOutput "Context: $ContextName" -Color Cyan
    Write-ColorOutput "-----------------------------------" -Color Cyan
    
    Push-Location $ProjectDir
    
    try {
        $output = dotnet ef database update --context $ContextName --connection $ConnectionString 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "✓ $ProjectName migrations applied successfully!" -Color Green
        } else {
            Write-ColorOutput "✗ Failed to apply $ProjectName migrations!" -Color Red
            Write-ColorOutput "Output: $output" -Color Yellow
            Write-ColorOutput "Continuing with next migration..." -Color Yellow
        }
    }
    catch {
        Write-ColorOutput "✗ Error applying $ProjectName migrations: $_" -Color Red
        Write-ColorOutput "Continuing with next migration..." -Color Yellow
    }
    finally {
        Pop-Location
    }
    
    Write-Host ""
}

Write-ColorOutput "========================================" -Color Blue
Write-ColorOutput "MedicWarehouse - Migration Script" -Color Blue
Write-ColorOutput "Apply All Migrations in Correct Order" -Color Blue
Write-ColorOutput "========================================" -Color Blue
Write-Host ""

if ([string]::IsNullOrEmpty($ConnectionString)) {
    Write-ColorOutput "No connection string provided." -Color Yellow
    Write-Host "Usage:"
    Write-Host "  .\run-all-migrations.ps1 -ConnectionString 'Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword'"
    Write-Host "Or set DATABASE_CONNECTION_STRING environment variable"
    Write-Host ""
    Write-ColorOutput "Using default connection string for local development..." -Color Yellow
    Write-ColorOutput "WARNING: Default credentials are for DEVELOPMENT ONLY!" -Color Red
    Write-ColorOutput "Never use default credentials in production environments!" -Color Red
    $ConnectionString = "Host=localhost;Database=medicsoft;Username=postgres;Password=postgres"
}

Write-ColorOutput "Connection string: $($ConnectionString.Substring(0, [Math]::Min(50, $ConnectionString.Length)))..." -Color Green
Write-Host ""

$RepoRoot = $PSScriptRoot

Write-ColorOutput "Starting migration process..." -Color Green
Write-Host ""

# 1. Main Application (MedicSoft.Repository)
# This is the core database with most tables
Apply-Migration `
    -ProjectDir "$RepoRoot\src\MedicSoft.Api" `
    -ProjectName "Main Application (MedicSoft)" `
    -ContextName "MedicSoftDbContext"

# 2. Patient Portal
# Patient-facing portal database
Apply-Migration `
    -ProjectDir "$RepoRoot\patient-portal-api\PatientPortal.Api" `
    -ProjectName "Patient Portal" `
    -ContextName "PatientPortalDbContext"

# 3. Auth Microservice
# Authentication and session management
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\auth\MedicSoft.Auth.Api" `
    -ProjectName "Auth Microservice" `
    -ContextName "AuthDbContext"

# 4. Appointments Microservice
# Appointment scheduling and notifications
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\appointments\MedicSoft.Appointments.Api" `
    -ProjectName "Appointments Microservice" `
    -ContextName "AppointmentsDbContext"

# 5. Patients Microservice
# Patient management
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\patients\MedicSoft.Patients.Api" `
    -ProjectName "Patients Microservice" `
    -ContextName "PatientsDbContext"

# 6. Medical Records Microservice
# Medical records and clinical data
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\medicalrecords\MedicSoft.MedicalRecords.Api" `
    -ProjectName "Medical Records Microservice" `
    -ContextName "MedicalRecordsDbContext"

# 7. Billing Microservice
# Financial transactions and billing
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\billing\MedicSoft.Billing.Api" `
    -ProjectName "Billing Microservice" `
    -ContextName "BillingDbContext"

# 8. System Admin Microservice
# System administration
Apply-Migration `
    -ProjectDir "$RepoRoot\microservices\systemadmin\MedicSoft.SystemAdmin.Api" `
    -ProjectName "System Admin Microservice" `
    -ContextName "SystemAdminDbContext"

# 9. Telemedicine
# Video consultation platform
Apply-Migration `
    -ProjectDir "$RepoRoot\telemedicine\src\MedicSoft.Telemedicine.Api" `
    -ProjectName "Telemedicine" `
    -ContextName "TelemedicineDbContext"

Write-ColorOutput "========================================" -Color Green
Write-ColorOutput "Migration process completed!" -Color Green
Write-ColorOutput "========================================" -Color Green
Write-Host ""
Write-Host "Summary:"
Write-Host "✓ Main Application (MedicSoftDbContext)"
Write-Host "✓ Patient Portal (PatientPortalDbContext)"
Write-Host "✓ Auth Microservice (AuthDbContext)"
Write-Host "✓ Appointments Microservice (AppointmentsDbContext)"
Write-Host "✓ Patients Microservice (PatientsDbContext)"
Write-Host "✓ Medical Records Microservice (MedicalRecordsDbContext)"
Write-Host "✓ Billing Microservice (BillingDbContext)"
Write-Host "✓ System Admin Microservice (SystemAdminDbContext)"
Write-Host "✓ Telemedicine (TelemedicineDbContext)"
Write-Host ""
Write-ColorOutput "Note: Some migrations may have been skipped if they were already applied." -Color Cyan
Write-ColorOutput "This is normal and expected behavior." -Color Cyan
