# API Proxy Configuration Fix

## Problem
Some pages were returning HTML instead of JSON when making API requests to endpoints like:
- `/api/system-admin/tickets/statistics`
- `/api/salesfunnel/incomplete`
- `/api/exam-catalog`

## Root Cause
The frontend applications (running on ports 4200 and 4201) were serving HTML for all routes including `/api/*` paths because:
1. Services use relative URLs like `/api/exam-catalog`
2. Nginx configuration had `try_files $uri $uri/ /index.html;` which catches ALL requests
3. No proxy configuration existed to forward `/api/*` requests to the backend API

## Solution

### Development Mode (npm start)
Created `proxy.conf.json` files for both frontend applications that proxy `/api/*` requests to the backend API server at `http://localhost:5293`.

**To use in development:**
```bash
cd frontend/mw-system-admin
npm start
# or
cd frontend/medicwarehouse-app
npm start
```

The Angular dev server will automatically use the proxy configuration and forward all `/api/*` requests to `http://localhost:5293/api/*`.

### Production Mode (nginx)
Updated `nginx.conf` files to add a `location /api/` block that proxies requests to the backend API server.

**Configuration:**
- Default backend URL: `http://localhost:5293`
- In Docker: This will need to be updated to point to the API container

## How It Works

### Development
When you make a request to `http://localhost:4201/api/exam-catalog`:
1. Angular dev server receives the request
2. Proxy configuration matches `/api/*`
3. Request is forwarded to `http://localhost:5293/api/exam-catalog`
4. Backend API responds with JSON
5. Response is returned to the browser

### Production
When you make a request to `http://localhost:4201/api/exam-catalog`:
1. Nginx receives the request
2. Location `/api/` block matches
3. Request is proxied to `http://localhost:5293/api/exam-catalog`
4. Backend API responds with JSON
5. Response is returned to the browser

## Docker Configuration
In Docker environments, nginx uses environment variable substitution to configure the backend API URL dynamically.

### How It Works
The nginx Docker image automatically processes template files in `/etc/nginx/templates/` and substitutes environment variables before starting. We provide two files:

1. **nginx.conf** - Used for local development/testing (hardcoded to `http://localhost:5293`)
2. **nginx.conf.template** - Used in Docker production (uses `${API_URL}` environment variable)

### Configuration
The API URL can be configured in docker-compose.yml:

```yaml
services:
  system-admin:
    build:
      context: ./frontend/mw-system-admin
      dockerfile: Dockerfile.production
      args:
        API_URL: ${API_URL:-http://api:8080}
    environment:
      - API_URL=${API_URL:-http://api:8080}
```

The default is `http://api:8080` which points to the API service within the Docker network.

## Testing
To test if the fix is working:

1. Start the backend API server:
```bash
cd src/MedicSoft.Api
dotnet run
```

2. Start the frontend application:
```bash
cd frontend/mw-system-admin
npm start
```

3. Open the browser to `http://localhost:4201` and navigate to pages that make API calls
4. Check the browser's network tab to confirm API requests return JSON instead of HTML

## Files Modified
- `frontend/mw-system-admin/proxy.conf.json` (created) - Angular dev proxy configuration
- `frontend/medicwarehouse-app/proxy.conf.json` (created) - Angular dev proxy configuration
- `frontend/mw-system-admin/angular.json` (updated to use proxy config)
- `frontend/medicwarehouse-app/angular.json` (updated to use proxy config)
- `frontend/mw-system-admin/nginx.conf` (added API proxy for local development)
- `frontend/medicwarehouse-app/nginx.conf` (added API proxy for local development)
- `frontend/mw-system-admin/nginx.conf.template` (created) - Template for Docker with env vars
- `frontend/medicwarehouse-app/nginx.conf.template` (created) - Template for Docker with env vars
- `frontend/mw-system-admin/Dockerfile.production` (updated to use template)
- `frontend/medicwarehouse-app/Dockerfile.production` (updated to use template)
