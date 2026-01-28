# System Admin API Documentation

**Version:** 1.0  
**Base URL:** `{API_URL}/api/system-admin`  
**Authentication:** Required (Role: SystemAdmin)  
**Last Updated:** January 28, 2026

---

## 📊 SaaS Metrics API

### Get Dashboard Metrics

Get comprehensive SaaS dashboard metrics including MRR, ARR, churn, growth rates, and customer counts.

**Endpoint:** `GET /saas-metrics/dashboard`

**Response:**
```json
{
  "mrr": 45000.00,
  "arr": 540000.00,
  "activeCustomers": 150,
  "newCustomers": 12,
  "churnedCustomers": 3,
  "churnRate": 2.0,
  "arpu": 300.00,
  "ltv": 15000.00,
  "cac": 500.00,
  "ltvCacRatio": 30.0,
  "mrrGrowthMoM": 8.5,
  "growthRateYoY": 45.2,
  "quickRatio": 4.5,
  "mrrTrend": "up",
  "trialCustomers": 8,
  "atRiskCustomers": 5
}
```

**Metrics Explained:**
- **MRR**: Monthly Recurring Revenue (current month)
- **ARR**: Annual Recurring Revenue (MRR × 12)
- **Churn Rate**: Percentage of customers lost this month
- **ARPU**: Average Revenue Per User (MRR / Active Customers)
- **LTV**: Customer Lifetime Value
- **CAC**: Customer Acquisition Cost
- **LTV/CAC Ratio**: Lifetime Value to Customer Acquisition Cost ratio (>3 is good, >4 is excellent)
- **Quick Ratio**: (New MRR + Expansion MRR) / (Contraction MRR + Churned MRR) - should be >4

---

### Get MRR Breakdown

Get detailed breakdown of Monthly Recurring Revenue components.

**Endpoint:** `GET /saas-metrics/mrr-breakdown`

**Response:**
```json
{
  "totalMrr": 45000.00,
  "newMrr": 3600.00,
  "expansionMrr": 0.00,
  "contractionMrr": 0.00,
  "churnedMrr": 900.00,
  "netNewMrr": 2700.00
}
```

**Components:**
- **Total MRR**: Current month's recurring revenue
- **New MRR**: Revenue from new customers this month
- **Expansion MRR**: Revenue from upgrades (requires plan change tracking)
- **Contraction MRR**: Revenue lost from downgrades
- **Churned MRR**: Revenue lost from cancellations
- **Net New MRR**: New + Expansion - Contraction - Churned

---

### Get Churn Analysis

Get detailed churn analysis including revenue and customer churn rates.

**Endpoint:** `GET /saas-metrics/churn-analysis`

**Response:**
```json
{
  "revenueChurnRate": 2.0,
  "customerChurnRate": 2.0,
  "monthlyRevenueChurn": 900.00,
  "monthlyCustomerChurn": 3,
  "annualRevenueChurn": 24.0,
  "annualCustomerChurn": 24.0,
  "churnHistory": [
    {
      "month": "Aug 2025",
      "revenueChurn": 850.00,
      "customerChurn": 2,
      "churnedCount": 2
    },
    {
      "month": "Sep 2025",
      "revenueChurn": 900.00,
      "customerChurn": 3,
      "churnedCount": 3
    }
  ]
}
```

---

### Get Growth Metrics

Get growth metrics including Month-over-Month and Year-over-Year growth rates.

**Endpoint:** `GET /saas-metrics/growth`

**Response:**
```json
{
  "moMGrowthRate": 8.5,
  "yoYGrowthRate": 45.2,
  "quickRatio": 4.5,
  "trialConversionRate": 65.0,
  "growthHistory": [
    {
      "month": "Feb 2025",
      "growthRate": 6.2,
      "mrr": 38500.00
    },
    {
      "month": "Mar 2025",
      "growthRate": 7.8,
      "mrr": 41500.00
    }
  ]
}
```

---

### Get Revenue Timeline

Get MRR timeline for the specified number of months.

**Endpoint:** `GET /saas-metrics/revenue-timeline?months=12`

**Parameters:**
- `months` (query, optional): Number of months (1-36, default: 12)

**Response:**
```json
[
  {
    "month": "Feb 2025",
    "date": "2025-02-01T00:00:00Z",
    "totalMrr": 38500.00,
    "newMrr": 2800.00,
    "expansionMrr": 0.00,
    "contractionMrr": 0.00,
    "churnedMrr": 750.00,
    "activeCustomers": 128
  },
  {
    "month": "Mar 2025",
    "date": "2025-03-01T00:00:00Z",
    "totalMrr": 41500.00,
    "newMrr": 3250.00,
    "expansionMrr": 0.00,
    "contractionMrr": 0.00,
    "churnedMrr": 800.00,
    "activeCustomers": 138
  }
]
```

---

### Get Customer Breakdown

Get customer breakdown by plan and status.

**Endpoint:** `GET /saas-metrics/customer-breakdown`

**Response:**
```json
{
  "byPlan": {
    "Basic": 45,
    "Professional": 75,
    "Enterprise": 30
  },
  "byStatus": {
    "Active": 150,
    "Trial": 8,
    "Cancelled": 0
  }
}
```

---

## 🔍 Global Search API

### Search

Search across clinics, users, tickets, plans, and audit logs.

**Endpoint:** `GET /search?q={query}&maxResults=50`

**Parameters:**
- `q` (query, required): Search query (minimum 2 characters)
- `maxResults` (query, optional): Maximum results per entity type (1-100, default: 50)

**Response:**
```json
{
  "clinics": [
    {
      "id": 123,
      "name": "Clínica Exemplo",
      "document": "12.345.678/0001-90",
      "email": "contato@clinicaexemplo.com.br",
      "tenantId": "tenant-uuid",
      "isActive": true,
      "planName": "Professional",
      "status": "Active"
    }
  ],
  "users": [
    {
      "id": 456,
      "username": "dr.silva",
      "fullName": "Dr. João Silva",
      "email": "joao.silva@clinica.com.br",
      "role": "Doctor",
      "isActive": true,
      "clinicName": "Clínica Exemplo"
    }
  ],
  "tickets": [
    {
      "id": 789,
      "title": "Problema no sistema",
      "description": "Não consigo acessar o módulo de agendamentos...",
      "status": "Open",
      "priority": "High",
      "createdAt": "2026-01-28T10:30:00Z",
      "clinicName": "Clínica Exemplo"
    }
  ],
  "plans": [
    {
      "id": 1,
      "name": "Professional",
      "description": "Plano profissional com todas as funcionalidades",
      "monthlyPrice": 299.90,
      "isActive": true,
      "activeSubscriptions": 75
    }
  ],
  "auditLogs": [
    {
      "id": 12345,
      "action": "Update",
      "entityType": "Clinic",
      "entityId": "123",
      "userName": "admin@system.com",
      "timestamp": "2026-01-28T14:20:00Z"
    }
  ],
  "totalResults": 8,
  "searchDurationMs": 245.5
}
```

---

## 🔔 System Notifications API

### Get Unread Notifications

Get all unread notifications for the current system admin.

**Endpoint:** `GET /notifications/unread`

**Response:**
```json
[
  {
    "id": 1,
    "type": "critical",
    "category": "subscription",
    "title": "Assinatura Vencida",
    "message": "A assinatura da clínica XYZ venceu.",
    "actionUrl": "/clinics/123",
    "actionLabel": "Ver Clínica",
    "isRead": false,
    "createdAt": "2026-01-28T10:00:00Z",
    "readAt": null,
    "data": null
  },
  {
    "id": 2,
    "type": "warning",
    "category": "subscription",
    "title": "Trial Expirando",
    "message": "O trial da clínica ABC expira em 2 dia(s).",
    "actionUrl": "/clinics/456",
    "actionLabel": "Contatar Cliente",
    "isRead": false,
    "createdAt": "2026-01-28T09:00:00Z",
    "readAt": null,
    "data": null
  }
]
```

---

### Get All Notifications

Get all notifications with pagination.

**Endpoint:** `GET /notifications?page=1&pageSize=20`

**Parameters:**
- `page` (query, optional): Page number (minimum 1, default: 1)
- `pageSize` (query, optional): Results per page (1-100, default: 20)

**Response:** Same format as unread notifications, includes both read and unread.

---

### Get Unread Count

Get count of unread notifications.

**Endpoint:** `GET /notifications/unread/count`

**Response:**
```json
{
  "count": 5
}
```

---

### Mark Notification as Read

Mark a specific notification as read.

**Endpoint:** `POST /notifications/{id}/read`

**Parameters:**
- `id` (path, required): Notification ID

**Response:**
```json
{
  "message": "Notification marked as read"
}
```

---

### Mark All Notifications as Read

Mark all notifications as read for the current user.

**Endpoint:** `POST /notifications/read-all`

**Response:**
```json
{
  "message": "All notifications marked as read"
}
```

---

### Create Notification (System Use Only)

Create a new notification. Restricted to SystemAdmin and BackgroundJob roles.

**Endpoint:** `POST /notifications`

**Authorization:** `SystemAdmin` or `BackgroundJob` role required

**Request Body:**
```json
{
  "type": "warning",
  "category": "customer",
  "title": "Clínica Inativa",
  "message": "Clínica XYZ sem atividade há 30 dias.",
  "actionUrl": "/clinics/123",
  "actionLabel": "Ver Clínica",
  "data": null
}
```

**Response:**
```json
{
  "id": 3,
  "type": "warning",
  "category": "customer",
  "title": "Clínica Inativa",
  "message": "Clínica XYZ sem atividade há 30 dias.",
  "actionUrl": "/clinics/123",
  "actionLabel": "Ver Clínica",
  "isRead": false,
  "createdAt": "2026-01-28T15:30:00Z",
  "readAt": null,
  "data": null
}
```

---

## 🔌 SignalR Hub

### System Notifications Hub

Real-time notification delivery via WebSocket.

**Hub URL:** `/hubs/system-notifications`

**Connection Setup:**
```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('/hubs/system-notifications', {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
  })
  .withAutomaticReconnect()
  .build();

await connection.start();
```

**Event Listeners:**
```typescript
connection.on('ReceiveNotification', (notification) => {
  console.log('New notification:', notification);
  // Handle notification display
});
```

---

## 📝 Error Responses

All endpoints return standard error responses:

### 400 Bad Request
```json
{
  "message": "Query must be at least 2 characters long"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "User does not have the required role"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while processing your request",
  "details": "Error details (in development only)"
}
```

---

## 🔐 Authentication

All endpoints require authentication with a valid JWT token containing the `SystemAdmin` role.

**Request Header:**
```
Authorization: Bearer {jwt_token}
```

**Role Required:** `SystemAdmin`

---

## 📊 Rate Limiting

- **Search API**: 60 requests per minute
- **Notifications API**: 100 requests per minute
- **SaaS Metrics API**: 30 requests per minute

---

## 🧪 Testing Examples

### cURL Examples

**Get Dashboard Metrics:**
```bash
curl -X GET "https://api.primecare.com/api/system-admin/saas-metrics/dashboard" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Search:**
```bash
curl -X GET "https://api.primecare.com/api/system-admin/search?q=clinic&maxResults=10" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Mark Notification as Read:**
```bash
curl -X POST "https://api.primecare.com/api/system-admin/notifications/1/read" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### JavaScript/TypeScript Examples

```typescript
// Get SaaS Dashboard Metrics
const response = await fetch(`${API_URL}/system-admin/saas-metrics/dashboard`, {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
const metrics = await response.json();

// Search
const searchResponse = await fetch(
  `${API_URL}/system-admin/search?q=clinic&maxResults=10`,
  {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  }
);
const results = await searchResponse.json();

// Mark notification as read
await fetch(`${API_URL}/system-admin/notifications/${notificationId}/read`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

---

## 📞 Support

For API support, contact: dev@primecare.com.br

---

**Version:** 1.0  
**Last Updated:** January 28, 2026
