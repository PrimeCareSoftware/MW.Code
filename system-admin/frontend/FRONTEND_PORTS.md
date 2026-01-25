# Frontend Port Configuration

## 📋 Overview

This document describes the port configuration for all frontend projects in the PrimeCare Software repository. Each project has been assigned a unique port to prevent conflicts when running multiple frontend applications simultaneously during development.

## 🔌 Port Assignments

| Project | Port | URL | Description |
|---------|------|-----|-------------|
| **medicwarehouse-app** | 4200 | `http://localhost:4200` | Main unified application (Clinic Dashboard, System Admin routes, Marketing Site) |
| **mw-system-admin** | 4201 | `http://localhost:4201` | Standalone System Admin application |
| **patient-portal** | 4202 | `http://localhost:4202` | Patient Portal (external patient access) |
| **mw-docs** | 4203 | `http://localhost:4203` | Technical Documentation Portal |

## 🚀 Running All Projects Simultaneously

You can run all frontend projects at the same time without port conflicts:

```bash
# Terminal 1 - Main Application
cd frontend/medicwarehouse-app
npm install --legacy-peer-deps
npm start
# Available at http://localhost:4200

# Terminal 2 - System Admin (Standalone)
cd frontend/mw-system-admin
npm install --legacy-peer-deps
npm start
# Available at http://localhost:4201

# Terminal 3 - Patient Portal
cd frontend/patient-portal
npm install --legacy-peer-deps
npm start
# Available at http://localhost:4202

# Terminal 4 - Documentation Portal
cd frontend/mw-docs
npm install --legacy-peer-deps
npm start
# Available at http://localhost:4203
```

## 📝 Configuration Files

Port configurations are defined in each project's `angular.json` file under:
```json
{
  "projects": {
    "[project-name]": {
      "architect": {
        "serve": {
          "options": {
            "port": [PORT_NUMBER]
          }
        }
      }
    }
  }
}
```

## ⚠️ Important Notes

- **medicwarehouse-app** (port 4200) is the main unified application that includes:
  - Clinic Dashboard: `http://localhost:4200/dashboard`
  - System Admin (route): `http://localhost:4200/system-admin`
  - Marketing Site: `http://localhost:4200/site`

- **mw-system-admin** (port 4201) is a standalone version that can be run independently

- All projects use Angular's dev server and can be configured to use different ports if needed

## 🔧 Changing Ports

If you need to use different ports, edit the `angular.json` file in each project:

1. Navigate to the project directory
2. Open `angular.json`
3. Find the `"serve"` section under `"architect"`
4. Update the `"port"` value in the `"options"` object
5. Save and restart the development server

## 📚 Related Documentation

- [Frontend API Configuration](FRONTEND_API_CONFIGURATION.md)
- [Frontend Consolidation Guide](FRONTEND_CONSOLIDATION_GUIDE.md)
- [Patient Portal Guide](PATIENT_PORTAL_GUIDE.md)
- [Quick Start Guide](GUIA_INICIO_RAPIDO_LOCAL.md)
