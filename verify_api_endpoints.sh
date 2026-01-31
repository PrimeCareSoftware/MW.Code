#!/bin/bash

# API Endpoint Verification Script
# This script tests the fixes for API endpoint issues

echo "======================================"
echo "API Endpoint Verification Script"
echo "======================================"
echo ""

# Configuration
BASE_URL="${API_BASE_URL:-http://localhost:5293}"
API_PATH="/api"

echo "Testing against: $BASE_URL"
echo ""

# Function to test endpoint
test_endpoint() {
    local method=$1
    local path=$2
    local description=$3
    local data=$4
    local auth=$5
    
    echo "Testing: $description"
    echo "  Method: $method"
    echo "  URL: $BASE_URL$path"
    
    if [ "$method" == "GET" ]; then
        if [ -n "$auth" ]; then
            response=$(curl -s -w "\n%{http_code}" -X GET "$BASE_URL$path" -H "Authorization: Bearer $auth" 2>&1)
        else
            response=$(curl -s -w "\n%{http_code}" -X GET "$BASE_URL$path" 2>&1)
        fi
    else
        if [ -n "$auth" ]; then
            response=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL$path" \
                -H "Content-Type: application/json" \
                -H "Authorization: Bearer $auth" \
                -d "$data" 2>&1)
        else
            response=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL$path" \
                -H "Content-Type: application/json" \
                -d "$data" 2>&1)
        fi
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | head -n-1)
    
    echo "  Status: $http_code"
    
    if [ "$http_code" == "200" ] || [ "$http_code" == "401" ]; then
        echo "  ✓ PASS - Endpoint is reachable"
    elif [ "$http_code" == "404" ]; then
        echo "  ✗ FAIL - 404 Not Found"
    elif [ "$http_code" == "400" ]; then
        echo "  ✗ FAIL - 400 Bad Request"
        echo "  Response: $(echo $body | head -c 200)"
    else
        echo "  ? Status: $http_code"
    fi
    echo ""
}

echo "======================================"
echo "Test 1: Module Config Info Endpoint"
echo "======================================"
test_endpoint "GET" "/api/module-config/info" "Module configuration info (correct URL)"

echo "======================================"
echo "Test 2: Module Config Info - Wrong URL"
echo "======================================"
test_endpoint "GET" "/api/api/module-config/info" "Module configuration info (WRONG URL with /api/api/)"

echo "======================================"
echo "Test 3: System Admin Module Usage"
echo "======================================"
test_endpoint "GET" "/api/system-admin/modules/usage" "System admin module usage (requires auth)" "" "$JWT_TOKEN"

echo "======================================"
echo "Test 4: System Admin Module Adoption"
echo "======================================"
test_endpoint "GET" "/api/system-admin/modules/adoption" "System admin module adoption (requires auth)" "" "$JWT_TOKEN"

echo "======================================"
echo "Test 5: Clinic Management Filter"
echo "======================================"
test_endpoint "POST" "/api/system-admin/clinic-management/filter" "Clinic filter with enum (requires auth)" \
    '{"page":1,"pageSize":20,"healthStatus":"AtRisk"}' "$JWT_TOKEN"

echo "======================================"
echo "Test 6: Clinic Filter - Lowercase Properties"
echo "======================================"
test_endpoint "POST" "/api/system-admin/clinic-management/filter" "Clinic filter with lowercase properties" \
    '{"page":1,"pagesize":20,"healthstatus":"AtRisk"}' "$JWT_TOKEN"

echo "======================================"
echo "Test 7: Clinic Filter - Numeric Enum"
echo "======================================"
test_endpoint "POST" "/api/system-admin/clinic-management/filter" "Clinic filter with numeric enum value" \
    '{"page":1,"pageSize":20,"healthStatus":2}' "$JWT_TOKEN"

echo "======================================"
echo "Summary"
echo "======================================"
echo ""
echo "Expected Results:"
echo "  Test 1: ✓ PASS (200 or 401 if auth required)"
echo "  Test 2: ✗ FAIL (404 Not Found)"
echo "  Test 3: ✓ PASS with auth, 401 without auth"
echo "  Test 4: ✓ PASS with auth, 401 without auth"
echo "  Test 5: ✓ PASS with auth, 401 without auth (was 400 before fix)"
echo "  Test 6: ✓ PASS (case-insensitive properties)"
echo "  Test 7: ✓ PASS (numeric enum support)"
echo ""
echo "Note: Tests 3-7 require a valid JWT token."
echo "Set JWT_TOKEN environment variable with a valid SystemAdmin token:"
echo "  export JWT_TOKEN='your-jwt-token-here'"
echo ""
echo "To run this script:"
echo "  bash verify_api_endpoints.sh"
echo ""
