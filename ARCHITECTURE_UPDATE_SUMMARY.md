# Architecture Update Summary

## Overview
This document summarizes the changes made to align all environment setup scripts with the new frontend architecture introduced in PR #212.

## Previous Architecture (Deprecated)
- **Frontend**: Multiple separate applications
  - `frontend/primecare-app` - Main clinic application
  - `frontend/mw-system-admin` - System admin application
  - Other standalone apps

## New Architecture (Current)
- **Frontend**: Single unified Angular application
  - `frontend/medicwarehouse-app` - Main unified application serving:
    - Main clinic interface at `/`
    - System admin at `/system-admin/*`
    - Marketing site at `/site/*`
  - Port: 4200
  
- **Separate Applications** (Still Available):
  - `frontend/mw-system-admin` - Standalone system admin (port 4201)
  - `frontend/mw-docs` - Documentation site
  - `frontend/mw-site` - Marketing site
  - `frontend/patient-portal` - Patient portal

## Files Updated

### Docker/Podman Compose Files (5 files)
All compose files were updated to reference `frontend/medicwarehouse-app` instead of `frontend/primecare-app`:

1. **docker-compose.yml**
   - Development environment
   - Frontend service now builds from `./frontend/medicwarehouse-app`

2. **docker-compose.production.yml**
   - Production environment with resource limits
   - Frontend service now builds from `./frontend/medicwarehouse-app`
   - Uses `Dockerfile.production` for optimized builds

3. **docker-compose.microservices.yml**
   - Microservices architecture (legacy)
   - Frontend service now builds from `./frontend/medicwarehouse-app`

4. **podman-compose.yml**
   - Podman development environment
   - Frontend service now builds from `./frontend/medicwarehouse-app`

5. **podman-compose.production.yml**
   - Podman production environment
   - Frontend service now builds from `./frontend/medicwarehouse-app`

### Setup Scripts (Already Correct)
The following setup scripts already referenced the correct directories and did not need changes:
- `setup-macos.sh` - macOS setup script
- `setup-windows.ps1` - Windows setup script

## Migration Benefits

### For Developers
- ✅ Single codebase to maintain instead of multiple apps
- ✅ Shared components and services reduce duplication
- ✅ Consistent UI/UX across all sections
- ✅ Simplified dependency management

### For DevOps
- ✅ Reduced container image size (one frontend instead of multiple)
- ✅ Simplified deployment process
- ✅ Lower memory and CPU requirements
- ✅ Easier to maintain and update

### For End Users
- ✅ Seamless navigation between sections
- ✅ Consistent user experience
- ✅ Faster page transitions (SPA routing)
- ✅ Better performance

## How to Use

### Development Environment

#### Using Docker Compose:
```bash
# Start all services
docker compose up -d

# Start only specific services
docker compose up -d postgres api frontend

# View logs
docker compose logs -f frontend
```

#### Using Podman Compose:
```bash
# Start all services
podman-compose up -d

# Start only specific services
podman-compose up -d postgres api frontend

# View logs
podman-compose logs -f frontend
```

### Production Environment

#### Using Docker Compose:
```bash
# Build and start
docker compose -f docker-compose.production.yml up -d

# Scale frontend if needed
docker compose -f docker-compose.production.yml up -d --scale frontend=2
```

#### Using Podman Compose:
```bash
# Build and start
podman-compose -f podman-compose.production.yml up -d
```

### Local Development (Without Containers)

#### Backend:
```bash
cd src/MedicSoft.Api
dotnet run
```

#### Frontend:
```bash
cd frontend/medicwarehouse-app
npm install
npm start
```

Access:
- Main App: http://localhost:4200
- System Admin: http://localhost:4200/system-admin
- Marketing Site: http://localhost:4200/site
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## Verification

All changes have been verified:
- ✅ Docker Compose syntax validation passed
- ✅ All referenced directories exist
- ✅ Dockerfiles are properly configured
- ✅ nginx.conf exists for production builds
- ✅ No old references to `primecare-app` remain
- ✅ Code review completed with no issues
- ✅ Security scan completed (no code changes)

## Troubleshooting

### Issue: "Cannot find Dockerfile"
**Solution:** Ensure you're in the project root directory when running compose commands.

### Issue: "Port already in use"
**Solution:** Stop any running containers or services using the same ports:
```bash
# Docker
docker compose down

# Podman
podman-compose down
```

### Issue: "Build fails with npm errors"
**Solution:** Clear npm cache and rebuild:
```bash
docker compose build --no-cache frontend
# or
podman-compose build --no-cache frontend
```

## Next Steps

1. ✅ All environment setup scripts updated
2. ✅ Documentation updated
3. ✅ CI/CD pipelines will work with new architecture
4. ⏭️ Deploy to staging environment for testing
5. ⏭️ Deploy to production

## Related Documentation

- [README.md](README.md) - Main project documentation
- [REFACTORING_SUMMARY.md](REFACTORING_SUMMARY.md) - Frontend refactoring details
- [PR_SUMMARY.md](PR_SUMMARY.md) - Recent changes summary
- [docs/GUIA_INICIO_RAPIDO_LOCAL.md](docs/GUIA_INICIO_RAPIDO_LOCAL.md) - Quick start guide

## Support

If you encounter any issues with the new architecture:
1. Check the [Troubleshooting](#troubleshooting) section above
2. Review the [documentation](docs/)
3. Open an issue on GitHub with details about your problem

---

**Date:** January 2026  
**PR:** #212 (Frontend Migration) + Current Changes  
**Status:** ✅ Complete
