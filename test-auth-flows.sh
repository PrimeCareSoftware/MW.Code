#!/bin/bash

# Script to test all three authentication flows
# Tests: medicwarehouse-app, system-admin, and patient-portal

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  PrimeCare Software - Authentication Flow Tests       ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

# Configuration
MAIN_API_URL="http://localhost:5293/api"
PATIENT_API_URL="http://localhost:5101/api"
TENANT_ID="demo-clinic-001"

# Test credentials
USER_USERNAME="admin"
USER_PASSWORD="Admin@123"
OWNER_USERNAME="owner"
OWNER_PASSWORD="Owner@123"
PATIENT_EMAIL="patient@example.com"
PATIENT_PASSWORD="Patient@123"

# Function to test login
test_login() {
    local system_name=$1
    local api_url=$2
    local endpoint=$3
    local credentials=$4
    local description=$5
    
    echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${YELLOW}Testing: ${description}${NC}"
    echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo ""
    
    echo -e "${BLUE}→${NC} API URL: ${api_url}${endpoint}"
    echo -e "${BLUE}→${NC} Credentials: $(echo $credentials | jq -c '{username: .username, emailOrCPF: .emailOrCPF}')"
    echo ""
    
    # Make the login request
    response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST "${api_url}${endpoint}" \
        -H "Content-Type: application/json" \
        -d "$credentials")
    
    # Extract status code
    status_code=$(echo "$response" | grep "HTTP_STATUS:" | cut -d: -f2)
    body=$(echo "$response" | sed '/HTTP_STATUS:/d')
    
    echo -e "${BLUE}HTTP Status:${NC} $status_code"
    echo ""
    
    # Check if we got 200 OK
    if [ "$status_code" = "200" ]; then
        echo -e "${GREEN}✓ Status: 200 OK${NC}"
        
        # Check if we got a token
        if echo "$body" | grep -q "token\|accessToken"; then
            echo -e "${GREEN}✓ Token received${NC}"
            
            # Extract and show token info
            token=$(echo "$body" | jq -r '.token // .accessToken // empty' 2>/dev/null)
            if [ ! -z "$token" ] && [ "$token" != "null" ]; then
                echo -e "${GREEN}✓ Token is valid${NC}"
                echo -e "${BLUE}→${NC} Token: ${token:0:50}..."
                
                # Decode JWT to show claims (without verification)
                payload=$(echo "$token" | cut -d. -f2)
                # Add padding if needed
                padded_payload=$(printf '%s' "$payload" | sed 's/-/+/g; s/_/\//g')
                case $((${#padded_payload} % 4)) in
                    2) padded_payload="${padded_payload}==" ;;
                    3) padded_payload="${padded_payload}=" ;;
                esac
                
                decoded=$(echo "$padded_payload" | base64 -d 2>/dev/null || echo "{}")
                echo ""
                echo -e "${BLUE}Token Claims:${NC}"
                echo "$decoded" | jq '.' 2>/dev/null || echo "$decoded"
                
                echo ""
                echo -e "${GREEN}✅ AUTHENTICATION SUCCESSFUL${NC}"
            else
                echo -e "${RED}✗ Token is empty or null${NC}"
                echo -e "${RED}❌ AUTHENTICATION FAILED (No token)${NC}"
            fi
        else
            echo -e "${RED}✗ No token field in response${NC}"
            echo -e "${RED}❌ AUTHENTICATION FAILED (200 OK but no token)${NC}"
        fi
        
        echo ""
        echo -e "${BLUE}Full Response:${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
    else
        echo -e "${RED}✗ Unexpected status code: $status_code${NC}"
        echo -e "${RED}❌ AUTHENTICATION FAILED${NC}"
        echo ""
        echo -e "${BLUE}Response:${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
    fi
    
    echo ""
    echo ""
}

# ===================================================
# TEST 1: MedicWarehouse App - User Login
# ===================================================
echo -e "${CYAN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  TEST 1: MedicWarehouse App - User Login              ║${NC}"
echo -e "${CYAN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

USER_CREDENTIALS="{
  \"username\": \"$USER_USERNAME\",
  \"password\": \"$USER_PASSWORD\",
  \"tenantId\": \"$TENANT_ID\"
}"

test_login "MedicWarehouse-App" "$MAIN_API_URL" "/auth/login" "$USER_CREDENTIALS" "MedicWarehouse App - Regular User Login"

# ===================================================
# TEST 2: System Admin - Owner Login
# ===================================================
echo -e "${CYAN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  TEST 2: System Admin - Owner Login                   ║${NC}"
echo -e "${CYAN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

OWNER_CREDENTIALS="{
  \"username\": \"$OWNER_USERNAME\",
  \"password\": \"$OWNER_PASSWORD\",
  \"tenantId\": \"$TENANT_ID\"
}"

test_login "System-Admin" "$MAIN_API_URL" "/auth/owner-login" "$OWNER_CREDENTIALS" "System Admin - Owner Login"

# ===================================================
# TEST 3: Patient Portal - Patient Login
# ===================================================
echo -e "${CYAN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  TEST 3: Patient Portal - Patient Login               ║${NC}"
echo -e "${CYAN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

PATIENT_CREDENTIALS="{
  \"emailOrCPF\": \"$PATIENT_EMAIL\",
  \"password\": \"$PATIENT_PASSWORD\"
}"

test_login "Patient-Portal" "$PATIENT_API_URL" "/auth/login" "$PATIENT_CREDENTIALS" "Patient Portal - Patient Login"

# ===================================================
# SUMMARY
# ===================================================
echo -e "${GREEN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  All Tests Completed!                                  ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}Summary:${NC}"
echo -e "  • MedicWarehouse App uses: ${MAIN_API_URL}/auth/login"
echo -e "  • System Admin uses: ${MAIN_API_URL}/auth/owner-login"
echo -e "  • Patient Portal uses: ${PATIENT_API_URL}/auth/login"
echo ""
echo -e "${YELLOW}Notes:${NC}"
echo -e "  • Main API returns: { token, username, tenantId, role, ... }"
echo -e "  • Patient API returns: { accessToken, refreshToken, user, ... }"
echo -e "  • Both return 200 OK on successful authentication"
echo ""
echo -e "${YELLOW}If tests pass but frontend still doesn't authenticate:${NC}"
echo -e "  1. Check frontend is using correct API URL (environment.ts)"
echo -e "  2. Check token is stored in localStorage"
echo -e "  3. Check HTTP interceptor adds Authorization header"
echo -e "  4. Check browser console for errors"
echo ""
