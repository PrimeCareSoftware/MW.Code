#!/bin/bash

# Script to test the JWT token generation and validation

echo "Testing JWT Token Generation and Validation"
echo "==========================================="

# Build the project
cd "/Users/igorlessarobainadesouza/Documents/MW.Code/src/MedicSoft.Api"
dotnet build -c Release --no-restore

echo ""
echo "Build completed. You can now test the API with:"
echo ""
echo "1. Login endpoint: POST /api/auth/login"
echo "   Body: {\"username\": \"admin\", \"password\": \"Admin@123\", \"tenantId\": \"demo-clinic-001\"}"
echo ""
echo "2. Validate session: POST /api/auth/validate-session"
echo "   Body: {\"token\": \"<token from login>\"}"
echo ""
echo "Check the logs for token details (Claims, Expires, etc.)"
