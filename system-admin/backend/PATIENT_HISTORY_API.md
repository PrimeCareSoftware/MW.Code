# Patient History API Documentation

## Overview

This document describes the new API endpoints for retrieving patient appointment and procedure history, including payment information and medical records.

## Endpoints

### Get Patient Appointment History

Retrieves the complete appointment history for a patient, including payment information and optionally medical records.

**Endpoint:** `GET /api/patients/{patientId}/appointment-history`

**Query Parameters:**
- `includeMedicalRecords` (boolean, optional): Whether to include medical record information. Default: `false`
  - Medical records are only included if the user has `medical-records.view` permission
  - For users without permission, this parameter is ignored

**Response:**
```json
{
  "patientId": "uuid",
  "patientName": "John Doe",
  "appointments": [
    {
      "appointmentId": "uuid",
      "scheduledDate": "2024-12-20T00:00:00Z",
      "scheduledTime": "10:00:00",
      "status": "Completed",
      "type": "Regular",
      "doctorName": "Dr. Maria Silva",
      "doctorSpecialty": "Cardiologia",
      "doctorProfessionalId": "CRM-SP 123456",
      "checkInTime": "2024-12-20T10:05:00Z",
      "checkOutTime": "2024-12-20T10:45:00Z",
      "payment": {
        "paymentId": "uuid",
        "amount": 150.00,
        "method": "Pix",
        "status": "Paid",
        "paymentDate": "2024-12-20T10:50:00Z",
        "pixKey": "email@example.com"
      },
      "medicalRecord": {
        "medicalRecordId": "uuid",
        "diagnosis": "Hipertensão arterial",
        "consultationDurationMinutes": 40,
        "createdAt": "2024-12-20T10:45:00Z"
      }
    }
  ],
  "procedures": []
}
```

**Status Codes:**
- `200 OK`: Success
- `404 Not Found`: Patient not found
- `401 Unauthorized`: User not authenticated
- `403 Forbidden`: User doesn't have permission to view medical records (when requested)

---

### Get Patient Procedure History

Retrieves the complete procedure history for a patient, including payment information.

**Endpoint:** `GET /api/patients/{patientId}/procedure-history`

**Response:**
```json
[
  {
    "procedureId": "uuid",
    "appointmentId": "uuid",
    "procedureName": "Consulta Cardiológica",
    "procedureCode": "PROC001",
    "procedureCategory": "Consultation",
    "priceCharged": 150.00,
    "performedAt": "2024-12-20T10:00:00Z",
    "notes": "Procedimento realizado com sucesso",
    "doctorName": "Dr. Maria Silva",
    "doctorSpecialty": "Cardiologia",
    "payment": {
      "paymentId": "uuid",
      "amount": 150.00,
      "method": "CreditCard",
      "status": "Paid",
      "paymentDate": "2024-12-20T10:50:00Z",
      "cardLastFourDigits": "1234"
    }
  }
]
```

**Status Codes:**
- `200 OK`: Success
- `404 Not Found`: Patient not found
- `401 Unauthorized`: User not authenticated

---

## Payment Methods

The following payment methods are supported:

| Method | Portuguese | Description |
|--------|-----------|-------------|
| `Cash` | Dinheiro | Cash payment |
| `CreditCard` | Cartão de Crédito | Credit card payment |
| `DebitCard` | Cartão de Débito | Debit card payment |
| `Pix` | PIX | Brazilian instant payment system |
| `BankTransfer` | Transferência Bancária | Bank transfer |
| `Check` | Cheque | Check payment |

---

## Payment Status

| Status | Portuguese | Description |
|--------|-----------|-------------|
| `Pending` | Pendente | Payment awaiting processing |
| `Processing` | Processando | Payment is being processed |
| `Paid` | Pago | Payment completed successfully |
| `Failed` | Falhou | Payment failed |
| `Refunded` | Reembolsado | Payment was refunded |
| `Cancelled` | Cancelado | Payment was cancelled |

---

## Appointment Status

| Status | Portuguese | Description |
|--------|-----------|-------------|
| `Scheduled` | Agendado | Appointment scheduled |
| `Confirmed` | Confirmado | Appointment confirmed |
| `InProgress` | Em Andamento | Appointment in progress |
| `Completed` | Concluído | Appointment completed |
| `Cancelled` | Cancelado | Appointment cancelled |
| `NoShow` | Faltou | Patient didn't show up |

---

## Permissions

### Medical Records Access

Medical records are **only visible** to users with the `medical-records.view` permission. This typically includes:

- **Médico (Medical)** profile: Full access to medical records
- **Proprietário (Owner)** profile: Full access to medical records
- **Recepção (Reception)** profile: Read-only access to medical records
- **Financeiro (Financial)** profile: No access to medical records

When a user without permission requests appointment history with `includeMedicalRecords=true`, the medical record information will be excluded from the response.

---

## Example Usage

### Get Appointment History (with medical records)

```bash
curl -X GET \
  'https://api.medicwarehouse.com/api/patients/123e4567-e89b-12d3-a456-426614174000/appointment-history?includeMedicalRecords=true' \
  -H 'Authorization: Bearer {token}'
```

### Get Appointment History (without medical records)

```bash
curl -X GET \
  'https://api.medicwarehouse.com/api/patients/123e4567-e89b-12d3-a456-426614174000/appointment-history' \
  -H 'Authorization: Bearer {token}'
```

### Get Procedure History

```bash
curl -X GET \
  'https://api.medicwarehouse.com/api/patients/123e4567-e89b-12d3-a456-426614174000/procedure-history' \
  -H 'Authorization: Bearer {token}'
```

---

## Frontend Integration

### Patient Form Tabs

The patient registration form has been enhanced with a tabbed interface:

1. **Cadastro Básico** (Basic Registration): Original patient registration form with CEP lookup
2. **Histórico de Atendimentos** (Appointment History): Shows all appointments with payment info and optional medical records
3. **Histórico de Procedimentos** (Procedure History): Shows all procedures performed with payment info

The tabs are only visible when editing an existing patient. For new patient registration, only the basic registration tab is shown.

### Permission Checking

```typescript
// Check if user has permission to view medical records
const canViewMedicalRecords = this.auth.currentUser()?.permissions?.includes('medical-records.view') || false;

// Fetch appointment history with medical records if permitted
this.patientService.getAppointmentHistory(patientId, canViewMedicalRecords).subscribe(...);
```

---

## Security Considerations

1. **Medical Records Privacy**: Medical record information is protected by permission checks
2. **Tenant Isolation**: All queries are scoped to the user's tenant to ensure data isolation
3. **Payment Information**: Sensitive payment data (full card numbers, etc.) is never exposed
4. **Audit Trail**: All API calls are logged for compliance purposes

---

## Future Enhancements

- [ ] Add doctor information tracking to appointments
- [ ] Add filtering and sorting options for history
- [ ] Add pagination for large history datasets
- [ ] Add export functionality (PDF, CSV)
- [ ] Add more detailed payment transaction history
