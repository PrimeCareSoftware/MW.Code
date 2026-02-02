# Schedule Blocking and Recurring Appointments - User Guide

## Overview

This feature allows healthcare professionals to:
- Block time slots in their schedule (for breaks, meetings, unavailability, etc.)
- Create recurring appointments for patients (weekly therapy sessions, monthly checkups, etc.)
- Create recurring blocked slots (daily lunch break, weekly team meetings, etc.)

## API Endpoints

### Blocked Time Slots

#### Create a Blocked Time Slot
```http
POST /api/blocked-time-slots
Authorization: Bearer {token}
Content-Type: application/json

{
  "clinicId": "guid",
  "professionalId": "guid",  // optional - null means clinic-wide block
  "date": "2026-02-15",
  "startTime": "12:00:00",
  "endTime": "13:00:00",
  "type": 1,  // 1=Break, 2=Unavailable, 3=Maintenance, 4=Training, 5=Meeting, 6=Other
  "reason": "Lunch break"  // optional
}
```

#### Get Blocked Slots for a Date
```http
GET /api/blocked-time-slots?date=2026-02-15&clinicId=guid&professionalId=guid
Authorization: Bearer {token}
```

#### Update a Blocked Slot
```http
PUT /api/blocked-time-slots/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "startTime": "12:30:00",
  "endTime": "13:30:00",
  "type": 1,
  "reason": "Extended lunch break"
}
```

#### Delete a Blocked Slot
```http
DELETE /api/blocked-time-slots/{id}
Authorization: Bearer {token}
```

### Recurring Appointments

#### Create Recurring Weekly Appointments
```http
POST /api/recurring-appointments
Authorization: Bearer {token}
Content-Type: application/json

{
  "patientId": "guid",
  "clinicId": "guid",
  "professionalId": "guid",  // optional
  "frequency": 2,  // 1=Daily, 2=Weekly, 3=Biweekly, 4=Monthly
  "daysOfWeek": 2,  // Flags: 1=Sunday, 2=Monday, 4=Tuesday, 8=Wednesday, 16=Thursday, 32=Friday, 64=Saturday
  "startDate": "2026-02-17",
  "endDate": "2026-08-17",  // optional - can use occurrencesCount instead
  "occurrencesCount": 20,  // optional - alternative to endDate
  "startTime": "14:00:00",
  "durationMinutes": 60,
  "appointmentType": 1,  // 1=Regular, 2=Emergency, 3=FollowUp, 4=Consultation
  "notes": "Weekly therapy session"  // optional
}
```

#### Create Recurring Monthly Blocked Slots
```http
POST /api/recurring-appointments/blocked-slots
Authorization: Bearer {token}
Content-Type: application/json

{
  "clinicId": "guid",
  "professionalId": "guid",  // optional
  "frequency": 4,  // Monthly
  "interval": 1,
  "dayOfMonth": 15,  // 15th of each month
  "startDate": "2026-02-15",
  "endDate": "2026-12-15",
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "blockedSlotType": 5,  // Meeting
  "notes": "Monthly team meeting"
}
```

#### Get Recurring Patterns for a Clinic
```http
GET /api/recurring-appointments/clinic/{clinicId}
Authorization: Bearer {token}
```

#### Get Recurring Patterns for a Professional
```http
GET /api/recurring-appointments/professional/{professionalId}
Authorization: Bearer {token}
```

#### Get Recurring Patterns for a Patient
```http
GET /api/recurring-appointments/patient/{patientId}
Authorization: Bearer {token}
```

#### Deactivate a Recurring Pattern
```http
POST /api/recurring-appointments/{id}/deactivate
Authorization: Bearer {token}
```

## Usage Examples

### Example 1: Psychologist with Weekly Recurring Sessions
A psychologist wants to schedule a patient for therapy every Monday at 2:00 PM for 3 months.

```json
POST /api/recurring-appointments
{
  "patientId": "patient-guid",
  "clinicId": "clinic-guid",
  "professionalId": "psychologist-guid",
  "frequency": 2,
  "daysOfWeek": 2,
  "startDate": "2026-02-17",
  "occurrencesCount": 12,
  "startTime": "14:00:00",
  "durationMinutes": 60,
  "appointmentType": 4,
  "notes": "Cognitive behavioral therapy session"
}
```

### Example 2: Doctor Blocking Lunch Break Daily
A doctor wants to block their lunch break every day from 12:00 PM to 1:00 PM.

```json
POST /api/recurring-appointments/blocked-slots
{
  "clinicId": "clinic-guid",
  "professionalId": "doctor-guid",
  "frequency": 1,
  "interval": 1,
  "startDate": "2026-02-10",
  "endDate": "2026-12-31",
  "startTime": "12:00:00",
  "endTime": "13:00:00",
  "blockedSlotType": 1,
  "notes": "Lunch break"
}
```

### Example 3: Clinic-Wide Maintenance Block
Block the entire clinic for equipment maintenance on the 1st Saturday of each month.

```json
POST /api/blocked-time-slots
{
  "clinicId": "clinic-guid",
  "date": "2026-03-01",
  "startTime": "08:00:00",
  "endTime": "12:00:00",
  "type": 3,
  "reason": "Monthly equipment maintenance"
}
```

## Enum Reference

### BlockedTimeSlotType
- `1` - Break (lunch, coffee break)
- `2` - Unavailable (personal time, vacation)
- `3` - Maintenance (equipment, facility)
- `4` - Training (staff training, courses)
- `5` - Meeting (team meetings, consultations)
- `6` - Other

### RecurrenceFrequency
- `1` - Daily
- `2` - Weekly
- `3` - Biweekly (every 2 weeks)
- `4` - Monthly
- `5` - Custom

### RecurrenceDays (Flags)
- `1` - Sunday
- `2` - Monday
- `4` - Tuesday
- `8` - Wednesday
- `16` - Thursday
- `32` - Friday
- `64` - Saturday

To specify multiple days, add the values:
- Monday + Wednesday + Friday = 2 + 8 + 32 = `42`
- Weekdays (Mon-Fri) = 2 + 4 + 8 + 16 + 32 = `62`

### AppointmentType
- `1` - Regular
- `2` - Emergency
- `3` - Follow-up
- `4` - Consultation

## Important Notes

1. **Conflict Detection**: The system automatically checks for conflicts with existing appointments and blocked slots when scheduling.

2. **Professional vs Clinic-Wide**: 
   - If `professionalId` is provided, the block only affects that professional's schedule
   - If `professionalId` is null, the block affects the entire clinic

3. **Maximum Occurrences**: Recurring patterns are limited to 1000 occurrences for safety. For longer patterns, create multiple patterns.

4. **Deactivating Patterns**: Deactivating a pattern stops generating future occurrences but doesn't delete already created appointments/blocks.

5. **Permissions Required**:
   - `appointments.create` - Create blocked slots or recurring patterns
   - `appointments.view` - View blocked slots or patterns
   - `appointments.edit` - Update blocked slots
   - `appointments.delete` - Delete blocked slots or deactivate patterns

## Frontend Integration

The frontend can use these endpoints to:
1. Display blocked time slots on the calendar (grayed out)
2. Show recurring appointment indicators
3. Prevent users from booking during blocked times
4. Allow professionals to manage their availability easily
5. Show recurring pattern details when clicking on an appointment

## Database Schema

Two new tables were added:
- `BlockedTimeSlots` - Individual blocked time slots
- `RecurringAppointmentPatterns` - Patterns for recurring appointments/blocks

Both tables have proper indexes on `ClinicId`, `Date`, and `TenantId` for efficient querying.
