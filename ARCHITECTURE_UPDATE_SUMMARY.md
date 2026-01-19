# Architecture Update Summary - Frontend Separation

## Overview
This document summarizes the architectural changes made to separate the System Admin application from the main MedicWarehouse application, improving maintainability and reducing complexity.

## Previous Architecture (Deprecated as of January 2026)
- **Frontend**: Single unified Angular application
  - `frontend/medicwarehouse-app` - Main unified application serving:
    - Main clinic interface at `/`
    - System admin at `/system-admin/*`
    - Marketing site at `/site/*`
  - Port: 4200

## New Architecture (Current)
- **Frontend**: Separated applications for better organization
  - `frontend/medicwarehouse-app` - Main clinic application serving:
    - Main clinic interface at `/`
    - Marketing site at `/site/*`
    - Port: 4200
  
  - `frontend/mw-system-admin` - Standalone system admin application
    - System admin interface at `/`
    - Port: 4201
    - Independent deployment and development
  
- **Other Applications** (Still Available):
  - `frontend/mw-docs` - Documentation site
  - `frontend/patient-portal` - Patient portal

## Changes Made

### 1. New System Admin Application
Created a new standalone Angular application at `frontend/mw-system-admin` with:
- All system admin pages (dashboard, clinics, plans, clinic-owners, subdomains, tickets, sales-metrics)
- Dedicated services (Auth, SystemAdminService, TicketService)
- Dedicated models (auth.model.ts, system-admin.model.ts, ticket.model.ts)
- System admin guard for route protection
- Independent routing without `/system-admin` prefix
- Port: 4201

### 2. Cleaned Up Main Application
From `frontend/medicwarehouse-app`, removed:
- `/pages/system-admin/*` directory and all subdirectories
- `services/system-admin.ts`
- `models/system-admin.model.ts`
- `guards/system-admin-guard.ts`
- All system admin routes from `app.routes.ts`
- System admin login path logic from Auth service

### 3. Updated Docker/Podman Compose Files
Updated 4 compose files to include the new system-admin service:

1. **docker-compose.yml**
   - Added `system-admin` service on port 4201
   - Uses `./frontend/mw-system-admin` context

2. **docker-compose.production.yml**
   - Added `system-admin` service with production build
   - Resource limits: 128M memory, 0.25 CPU
   - Port mapping: 4201:80

3. **podman-compose.yml**
   - Added `system-admin` service on port 4201
   - Uses `./frontend/mw-system-admin` context

4. **podman-compose.production.yml**
   - Added `system-admin` service with production build
   - Same resource limits as docker version

## Migration Benefits

### For Developers
- ✅ Clear separation of concerns between clinic app and system admin
- ✅ Easier to maintain and understand each application
- ✅ Reduced bundle size for each application
- ✅ Independent development and deployment cycles
- ✅ Simpler debugging and testing

### For DevOps
- ✅ Can scale each application independently
- ✅ Can deploy updates to one without affecting the other
- ✅ Better resource allocation (system admin used less frequently)
- ✅ Easier to implement different security policies per application

### For End Users
- ✅ Better performance - smaller application bundles
- ✅ Clearer separation between clinic and admin interfaces
- ✅ Independent availability (clinic app remains available if admin app has issues)

## How to Use

### Development Environment

#### Using Docker Compose:
```bash
# Start all services including both frontend applications
docker compose up -d

# Start only specific services
docker compose up -d postgres api frontend system-admin

# View logs
docker compose logs -f frontend
docker compose logs -f system-admin
```

#### Using Podman Compose:
```bash
# Start all services
podman-compose up -d

# Start only specific services
podman-compose up -d postgres api frontend system-admin

# View logs
podman-compose logs -f frontend
podman-compose logs -f system-admin
```

#### Local Development (without containers):
```bash
# Terminal 1 - Main Application
cd frontend/medicwarehouse-app
npm install
npm start
# Access at http://localhost:4200

# Terminal 2 - System Admin
cd frontend/mw-system-admin
npm install
npm start
# Access at http://localhost:4201
```

### Production Environment

#### Using Docker Compose:
```bash
# Build and start
docker compose -f docker-compose.production.yml up -d

# View logs
docker compose -f docker-compose.production.yml logs -f

# Stop
docker compose -f docker-compose.production.yml down
```

#### Using Podman Compose:
```bash
# Build and start
podman-compose -f podman-compose.production.yml up -d

# View logs
podman-compose -f podman-compose.production.yml logs -f

# Stop
podman-compose -f podman-compose.production.yml down
```

## URL Structure

### Development
- Main Application: http://localhost:4200
  - Clinic Interface: http://localhost:4200/
  - Marketing Site: http://localhost:4200/site
- System Admin: http://localhost:4201
  - Dashboard: http://localhost:4201/dashboard
  - Clinics: http://localhost:4201/clinics
  - Plans: http://localhost:4201/plans
- API: http://localhost:5000

### Production
- Main Application: Port 4200 (80 inside container)
- System Admin: Port 4201 (80 inside container)
- API: Port 5000 (8080 inside container)

## Architecture History

1. **Pre-2024**: Multiple separate applications
   - `frontend/primecare-app` - Main clinic application
   - `frontend/mw-system-admin` - System admin application
   - Other standalone apps

2. **Mid-2024 (PR #212)**: Unified architecture
   - Merged everything into `frontend/medicwarehouse-app`
   - Single application serving all routes

3. **January 2026 (Current)**: Separated architecture
   - Split system-admin back into separate application
   - Kept main clinic app and site together
   - Better balance between unified and modular approaches

## Migration Notes

### For Existing Deployments
1. Deploy the new `mw-system-admin` application alongside existing `medicwarehouse-app`
2. Update nginx/load balancer configuration to route requests appropriately
3. Update environment variables if needed
4. Test both applications independently
5. Update any hardcoded URLs that referenced `/system-admin` paths

### Breaking Changes
- System admin is no longer accessible at `/system-admin/*` in the main application
- System admin must be accessed via the new standalone application on port 4201
- Auth service no longer includes system admin specific login redirect logic

## Security Considerations
- Both applications share the same backend API
- Authentication tokens are valid across both applications
- System admin guard ensures only users with `isSystemOwner` flag can access admin routes
- Consider using separate domains or subdomains in production (e.g., `admin.medicwarehouse.com`)

## Support
For questions or issues related to this architectural change, please contact the development team or create an issue in the repository.
