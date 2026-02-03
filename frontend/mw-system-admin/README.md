# MedicWarehouse System Admin

This is the standalone System Administration application for MedicWarehouse. It provides a dedicated interface for system administrators to manage clinics, subscription plans, clinic owners, subdomains, and support tickets.

## Overview

The System Admin application is a separate Angular application that has been extracted from the main medicwarehouse-app to provide better separation of concerns and easier maintenance.

## Features

- **Dashboard**: System-wide analytics and metrics
- **Clinic Management**: Create, view, and manage clinics
- **Subscription Plans**: Manage subscription plans and pricing
- **Clinic Owners**: Manage clinic owner accounts
- **Subdomains**: Configure and manage clinic subdomains
- **Support Tickets**: View and manage support tickets
- **Sales Metrics**: Track sales performance and revenue

## Prerequisites

- Node.js 20.x or higher
- npm 10.x or higher

## Installation

```bash
npm install
```

## Development

To start the development server:

```bash
npm start
```

The application will be available at `http://localhost:4201` (or the configured port).

## Building

To build the application for production:

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

## Testing

To run the unit tests:

```bash
npm test
```

## Project Structure

```
src/
├── app/
│   ├── guards/
│   │   └── system-admin-guard.ts    # Route guard for system admin authentication
│   ├── models/
│   │   ├── auth.model.ts            # Authentication models
│   │   ├── system-admin.model.ts    # System admin data models
│   │   └── ticket.model.ts          # Support ticket models
│   ├── pages/
│   │   ├── clinic-owners/           # Clinic owners management
│   │   ├── clinics/                 # Clinic management
│   │   ├── dashboard/               # Main dashboard
│   │   ├── errors/                  # Error pages
│   │   ├── login/                   # Login page
│   │   ├── plans/                   # Subscription plans
│   │   ├── sales-metrics/           # Sales analytics
│   │   ├── subdomains/              # Subdomain management
│   │   └── tickets/                 # Support tickets
│   ├── services/
│   │   ├── auth.ts                  # Authentication service
│   │   ├── system-admin.ts          # System admin API service
│   │   └── ticket.service.ts        # Ticket service
│   ├── shared/                      # Shared components
│   ├── app.routes.ts                # Application routes
│   └── app.ts                       # Root component
└── environments/                    # Environment configurations
```

## Authentication

The application requires system administrator authentication. Only users with the `isSystemOwner` flag can access the system admin routes.

## API Integration

The application communicates with the backend API at the configured `apiUrl` in the environment files. All system admin endpoints are prefixed with `/system-admin`.

## License

Proprietary - Omni Care Software
