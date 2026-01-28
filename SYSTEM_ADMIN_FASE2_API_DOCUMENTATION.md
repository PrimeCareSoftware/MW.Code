# System Admin API Documentation - Phase 2: Client Management

## Overview

This document describes the new API endpoints added in Phase 2 of the System Admin improvements, which transform the basic clinic management into a comprehensive CRM system with advanced analytics, health scoring, cross-tenant user management, and tagging capabilities.

## Table of Contents

1. [Clinic Management Endpoints](#clinic-management-endpoints)
2. [Cross-Tenant User Management](#cross-tenant-user-management)
3. [Tag Management](#tag-management)
4. [Data Models](#data-models)

---

## Clinic Management Endpoints

Base URL: `/api/system-admin/clinic-management`

### Get Clinic Detail

Get comprehensive clinic information including subscription, users, tickets, and tags.

**Endpoint:** `GET /api/system-admin/clinic-management/{id}/detail`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Clinic ID

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Clínica Exemplo",
  "tradeName": "Clínica Exemplo LTDA",
  "document": "12345678000190",
  "phone": "(11) 98765-4321",
  "email": "contato@clinicaexemplo.com.br",
  "address": "Rua Exemplo, 123",
  "isActive": true,
  "subdomain": "exemplo",
  "createdAt": "2024-01-15T10:30:00Z",
  "currentSubscription": {
    "planName": "Profissional",
    "status": "Active",
    "startDate": "2024-01-15T00:00:00Z",
    "endDate": null,
    "trialEndDate": null,
    "currentPrice": 299.90
  },
  "totalUsers": 5,
  "activeUsers": 4,
  "openTickets": 1,
  "totalTickets": 12,
  "tags": [
    {
      "id": "tag-id",
      "name": "High Value",
      "category": "value",
      "color": "#10B981"
    }
  ]
}
```

---

### Calculate Health Score

Calculate and retrieve the health score for a clinic based on multiple factors.

**Endpoint:** `GET /api/system-admin/clinic-management/{id}/health-score`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Clinic ID

**Response:**
```json
{
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "usageScore": 25,
  "userEngagementScore": 20,
  "supportScore": 15,
  "paymentScore": 25,
  "totalScore": 85,
  "healthStatus": "Healthy",
  "calculatedAt": "2024-01-20T14:30:00Z",
  "lastActivity": "2024-01-19T18:45:00Z",
  "daysSinceActivity": 1,
  "activeUsersCount": 4,
  "totalUsersCount": 5,
  "openTicketsCount": 1,
  "hasPaymentIssues": false
}
```

**Health Score Breakdown:**

1. **Usage Score (0-30 points)**
   - <= 1 day since activity: 30 points
   - <= 7 days: 25 points
   - <= 14 days: 20 points
   - <= 30 days: 10 points
   - > 30 days: 0 points

2. **User Engagement Score (0-25 points)**
   - Based on percentage of active users in last 30 days
   - Formula: 25 * (active_users / total_users)

3. **Support Score (0-20 points)**
   - 0 open tickets: 20 points
   - 1 open ticket: 15 points
   - 2 open tickets: 10 points
   - 3 open tickets: 5 points
   - 4+ open tickets: 0 points

4. **Payment Score (0-25 points)**
   - No payment issues: 25 points
   - Has payment issues: 0 points

**Health Status:**
- **Healthy**: 80-100 points
- **Needs Attention**: 50-79 points
- **At Risk**: 0-49 points

---

### Get Timeline

Retrieve timeline events for a clinic including subscriptions, tickets, and user changes.

**Endpoint:** `GET /api/system-admin/clinic-management/{id}/timeline`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Clinic ID
- `limit` (int, query, optional) - Maximum number of events (default: 50)

**Response:**
```json
[
  {
    "type": "subscription",
    "title": "Plano Profissional",
    "description": "Active",
    "date": "2024-01-15T10:00:00Z",
    "icon": "card_membership"
  },
  {
    "type": "user",
    "title": "Usuário criado: João Silva",
    "description": "Role: Doctor",
    "date": "2024-01-16T14:30:00Z",
    "icon": "person_add"
  },
  {
    "type": "ticket",
    "title": "Ticket #123",
    "description": "Problema com agendamento",
    "date": "2024-01-18T09:15:00Z",
    "icon": "support"
  }
]
```

---

### Get Usage Metrics

Get detailed usage statistics for a clinic over a specific period.

**Endpoint:** `GET /api/system-admin/clinic-management/{id}/usage-metrics`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Clinic ID
- `periodStart` (datetime, query, optional) - Start date (default: 30 days ago)
- `periodEnd` (datetime, query, optional) - End date (default: now)

**Response:**
```json
{
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "last7DaysLogins": 42,
  "last30DaysLogins": 156,
  "lastLoginDate": "2024-01-19T18:45:00Z",
  "appointmentsCreated": 87,
  "patientsRegistered": 23,
  "documentsGenerated": 45,
  "metricsPeriodStart": "2023-12-20T00:00:00Z",
  "metricsPeriodEnd": "2024-01-20T00:00:00Z"
}
```

---

### Filter Clinics

Advanced filtering and searching of clinics with multiple criteria.

**Endpoint:** `POST /api/system-admin/clinic-management/filter`

**Authorization:** SystemAdmin role required

**Request Body:**
```json
{
  "searchTerm": "exemplo",
  "isActive": true,
  "tags": ["High Value", "At Risk"],
  "healthStatus": "NeedsAttention",
  "subscriptionStatus": "Active",
  "createdAfter": "2024-01-01T00:00:00Z",
  "createdBefore": "2024-12-31T23:59:59Z",
  "page": 1,
  "pageSize": 20,
  "sortBy": "name",
  "sortDescending": false
}
```

**Response:**
```json
{
  "data": [
    {
      "id": "...",
      "name": "...",
      // ... clinic details
    }
  ],
  "totalCount": 156,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

---

### Get Clinics by Segment

Quick access to predefined clinic segments.

**Endpoint:** `GET /api/system-admin/clinic-management/segment/{segment}`

**Authorization:** SystemAdmin role required

**Parameters:**
- `segment` (string, path) - Segment name: `new`, `trial`, `at-risk`, `needs-attention`, `healthy`, `inactive`

**Response:**
```json
{
  "segment": "at-risk",
  "data": [
    // ... clinic details
  ],
  "totalCount": 12
}
```

**Available Segments:**
- `new` - Clinics created in last 30 days
- `trial` - Clinics on trial subscriptions
- `at-risk` - Health score < 50
- `needs-attention` - Health score 50-79
- `healthy` - Health score >= 80
- `inactive` - Deactivated clinics

---

## Cross-Tenant User Management

Base URL: `/api/system-admin/users`

### Filter Users

Get users across all clinics with filtering capabilities.

**Endpoint:** `POST /api/system-admin/users/filter`

**Authorization:** SystemAdmin role required

**Request Body:**
```json
{
  "searchTerm": "joão",
  "role": "Doctor",
  "isActive": true,
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "page": 1,
  "pageSize": 20
}
```

**Response:**
```json
{
  "data": [
    {
      "id": "user-id",
      "name": "João Silva",
      "email": "joao@example.com",
      "phone": "(11) 98765-4321",
      "role": "Doctor",
      "isActive": true,
      "createdAt": "2024-01-15T10:00:00Z",
      "clinicId": "clinic-id",
      "clinicName": "Clínica Exemplo",
      "clinicSubdomain": "exemplo"
    }
  ],
  "totalCount": 234,
  "page": 1,
  "pageSize": 20,
  "totalPages": 12
}
```

---

### Get User by ID

Get detailed information about a specific user.

**Endpoint:** `GET /api/system-admin/users/{id}`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - User ID

**Response:**
```json
{
  "id": "user-id",
  "name": "João Silva",
  "email": "joao@example.com",
  "phone": "(11) 98765-4321",
  "role": "Doctor",
  "isActive": true,
  "createdAt": "2024-01-15T10:00:00Z",
  "clinicId": "clinic-id",
  "clinicName": "Clínica Exemplo",
  "clinicSubdomain": "exemplo"
}
```

---

### Reset Password

Reset a user's password.

**Endpoint:** `POST /api/system-admin/users/{id}/reset-password`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - User ID

**Request Body:**
```json
{
  "newPassword": "newSecurePassword123"
}
```

**Response:**
```json
{
  "message": "Password reset successfully"
}
```

---

### Toggle User Activation

Activate or deactivate a user account.

**Endpoint:** `POST /api/system-admin/users/{id}/toggle-activation`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - User ID

**Response:**
```json
{
  "message": "User activation toggled successfully"
}
```

---

## Tag Management

Base URL: `/api/system-admin/tags`

### Get All Tags

Retrieve all available tags, ordered by category and name.

**Endpoint:** `GET /api/system-admin/tags`

**Authorization:** SystemAdmin role required

**Response:**
```json
[
  {
    "id": "tag-id",
    "name": "High Value",
    "description": "Clinics with MRR > R$ 1000",
    "category": "value",
    "color": "#10B981",
    "isAutomatic": true,
    "order": 1,
    "createdAt": "2024-01-10T00:00:00Z"
  }
]
```

**Tag Categories:**
- `type` - Business type (dental, medical, veterinary, etc.)
- `region` - Geographic location
- `value` - Revenue/value segmentation
- `status` - Lifecycle status (new, at-risk, churned, etc.)
- `custom` - Custom categorization

---

### Create Tag

Create a new tag for clinic categorization.

**Endpoint:** `POST /api/system-admin/tags`

**Authorization:** SystemAdmin role required

**Request Body:**
```json
{
  "name": "VIP Customer",
  "description": "Premium tier customers",
  "category": "value",
  "color": "#F59E0B",
  "isAutomatic": false,
  "automationRules": null,
  "order": 0
}
```

**Response:**
```json
{
  "id": "new-tag-id",
  "name": "VIP Customer",
  // ... tag details
}
```

---

### Update Tag

Update an existing tag.

**Endpoint:** `PUT /api/system-admin/tags/{id}`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Tag ID

**Request Body:**
```json
{
  "name": "VIP Customer Updated",
  "description": "Updated description",
  "category": "value",
  "color": "#F59E0B",
  "order": 1
}
```

---

### Delete Tag

Delete a tag and remove all its associations.

**Endpoint:** `DELETE /api/system-admin/tags/{id}`

**Authorization:** SystemAdmin role required

**Parameters:**
- `id` (guid, path) - Tag ID

**Response:**
```json
{
  "message": "Tag deleted successfully"
}
```

---

### Assign Tag to Clinics

Assign a tag to one or more clinics.

**Endpoint:** `POST /api/system-admin/tags/assign`

**Authorization:** SystemAdmin role required

**Request Body:**
```json
{
  "tagId": "tag-id",
  "clinicIds": [
    "clinic-id-1",
    "clinic-id-2",
    "clinic-id-3"
  ]
}
```

**Response:**
```json
{
  "message": "Tag assigned to 3 clinic(s)"
}
```

---

### Remove Tag from Clinics

Remove a tag from one or more clinics.

**Endpoint:** `POST /api/system-admin/tags/remove`

**Authorization:** SystemAdmin role required

**Request Body:**
```json
{
  "tagId": "tag-id",
  "clinicIds": [
    "clinic-id-1",
    "clinic-id-2"
  ]
}
```

**Response:**
```json
{
  "message": "Tag removed from 2 clinic(s)"
}
```

---

### Get Tags by Clinic

Get all tags assigned to a specific clinic.

**Endpoint:** `GET /api/system-admin/tags/clinic/{clinicId}`

**Authorization:** SystemAdmin role required

**Parameters:**
- `clinicId` (guid, path) - Clinic ID

**Response:**
```json
[
  {
    "id": "tag-id",
    "name": "High Value",
    "category": "value",
    "color": "#10B981"
  }
]
```

---

### Apply Automatic Tags

Trigger the automatic tag application based on rules.

**Endpoint:** `POST /api/system-admin/tags/apply-automatic`

**Authorization:** SystemAdmin role required

**Response:**
```json
{
  "message": "Automatic tags applied successfully"
}
```

**Automatic Tag Rules:**

1. **At Risk Tag**
   - Applied to clinics with no activity in last 30 days
   - Auto-assigned by system

2. **High Value Tag**
   - Applied to clinics with MRR >= R$ 1000
   - Auto-assigned by system

3. **New Tag**
   - Applied to clinics created in last 30 days
   - Auto-assigned by system

---

## Data Models

### HealthStatus Enum
```csharp
enum HealthStatus
{
    Healthy = 0,        // 80-100 points
    NeedsAttention = 1, // 50-79 points
    AtRisk = 2          // 0-49 points
}
```

### ExportFormat Enum
```csharp
enum ExportFormat
{
    Csv = 0,
    Excel = 1,
    Pdf = 2
}
```

---

## Error Responses

All endpoints follow a consistent error response format:

**404 Not Found:**
```json
{
  "message": "Clinic with ID {id} not found"
}
```

**400 Bad Request:**
```json
{
  "message": "At least one clinic ID is required"
}
```

**401 Unauthorized:**
```json
{
  "message": "Authorization token required"
}
```

**403 Forbidden:**
```json
{
  "message": "SystemAdmin role required"
}
```

---

## Best Practices

1. **Health Score Calculation**
   - Cache health scores for performance
   - Recalculate periodically (e.g., daily via background job)
   - Use health status for quick filtering

2. **Cross-Tenant Queries**
   - Always use `IgnoreQueryFilters()` when querying across tenants
   - Verify SystemAdmin role before allowing cross-tenant access
   - Limit result sets with pagination

3. **Tagging Strategy**
   - Use automatic tags for objective criteria
   - Use manual tags for subjective categorization
   - Run automatic tag application periodically
   - Keep tag names consistent and meaningful

4. **Performance Optimization**
   - Use pagination for all list endpoints
   - Cache frequently accessed data (tags, health scores)
   - Limit timeline events to recent history
   - Consider background jobs for expensive calculations

---

## Migration Notes

When deploying Phase 2, ensure:

1. Run database migrations to create `Tag` and `ClinicTag` tables
2. Register new services in dependency injection
3. Seed initial tags if desired
4. Set up background job for automatic tag application
5. Update API documentation in Swagger/OpenAPI

---

## Support

For questions or issues:
- Email: suporte@medicwarehouse.com.br
- Documentation: https://docs.medicwarehouse.com.br
- GitHub: https://github.com/PrimeCareSoftware/MW.Code
