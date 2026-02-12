#!/bin/bash

# Script para testar a melhoria de performance do Swagger
# Test script for Swagger performance improvement

echo "=========================================="
echo "Swagger Performance Test"
echo "=========================================="
echo ""

# Check if API is running
API_URL="${1:-http://localhost:5000}"
SWAGGER_JSON_URL="$API_URL/swagger/v1/swagger.json"

echo "Testing API URL: $API_URL"
echo "Swagger JSON URL: $SWAGGER_JSON_URL"
echo ""

# Function to measure load time
measure_time() {
    local iteration=$1
    echo "Test #$iteration: Loading Swagger JSON..."
    
    # Use curl with timing information
    TIME_OUTPUT=$(curl -w "\nTime: %{time_total}s\nHTTP Code: %{http_code}\n" \
                       -o /dev/null \
                       -s \
                       "$SWAGGER_JSON_URL")
    
    echo "$TIME_OUTPUT"
    echo ""
}

# Function to check cache headers
check_cache_headers() {
    echo "Checking Cache-Control headers..."
    HEADERS=$(curl -I -s "$SWAGGER_JSON_URL" | grep -i "cache-control")
    
    if [ -z "$HEADERS" ]; then
        echo "❌ No Cache-Control headers found"
    else
        echo "✅ Cache-Control header found:"
        echo "   $HEADERS"
    fi
    echo ""
}

echo "=========================================="
echo "First Load (expecting ~10-15 seconds)"
echo "=========================================="
measure_time 1

echo "=========================================="
echo "Second Load (expecting <1 second - cached)"
echo "=========================================="
measure_time 2

echo "=========================================="
echo "Third Load (should also be fast - cached)"
echo "=========================================="
measure_time 3

echo "=========================================="
echo "Cache Headers Verification"
echo "=========================================="
check_cache_headers

echo "=========================================="
echo "Test Complete!"
echo "=========================================="
echo ""
echo "Expected Results:"
echo "- First load: ~10-15 seconds (generation)"
echo "- Subsequent loads: <1 second (cached)"
echo "- Cache-Control header: 'public, max-age=86400'"
echo ""
echo "If the second and third loads are significantly faster,"
echo "the caching is working correctly! ✅"
