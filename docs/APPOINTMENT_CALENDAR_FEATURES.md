# Dynamic Appointment Calendar and Notification System

This document describes the new features implemented for the MedicWarehouse appointment system.

## Features

### 1. Dynamic Weekly Calendar View

A Teams-like calendar view has been added to visualize and manage appointments more efficiently.

#### Location
- **Route**: `/appointments/calendar`
- **Component**: `AppointmentCalendar`
- **Files**: 
  - `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-calendar/`

#### Features
- **Weekly Time Slot Grid**: Shows a full week with 30-minute time slots from 8:00 AM to 6:00 PM
- **Visual Appointment Display**: Appointments are displayed as colored blocks in their respective time slots
- **Status Indicators**: Different colors for appointment statuses (Scheduled, Confirmed, InProgress, Completed)
- **Click-to-Schedule**: Click on any available time slot to create a new appointment
- **Week Navigation**: Navigate between weeks with Previous/Next buttons, or jump to today
- **Today Highlighting**: Current day is highlighted for easy reference
- **Doctor Filtering**: Support for filtering appointments by doctor (prepared for future role-based filtering)

#### Usage
1. Navigate to "Agendamentos" in the main menu
2. Click "Calendário Semanal" button to switch to calendar view
3. Click on any empty time slot to create a new appointment
4. Click on an existing appointment to view/edit it
5. Use navigation buttons to move between weeks

### 2. Quick Appointment Creation from Calendar

When clicking on a time slot in the calendar, the system automatically pre-fills the appointment form.

#### Features
- **Auto-filled Date and Time**: The selected date and time are automatically populated
- **Patient Selection**: Choose from available patients
- **Duration Setting**: Set appointment duration (default 30 minutes)
- **Appointment Type**: Specify type (Regular, Follow-up, etc.)

### 3. Secretary Notification System

A real-time notification system that allows doctors to notify the secretary when they finish a consultation.

#### Location
- **Component**: `NotificationPanel` (integrated in navbar)
- **Service**: `NotificationService`
- **Files**: 
  - `frontend/medicwarehouse-app/src/app/shared/notification-panel/`
  - `frontend/medicwarehouse-app/src/app/services/notification.service.ts`

#### Features
- **Real-time Notifications**: Notifications appear immediately in the navbar bell icon
- **Unread Badge**: Visual indicator showing number of unread notifications
- **Notification Panel**: Dropdown panel showing all recent notifications
- **Browser Notifications**: Optional browser notifications (requires permission)
- **Sound Alerts**: Audio notification for new alerts (can be customized)
- **Mark as Read**: Individual or bulk marking notifications as read
- **Delete Notifications**: Remove notifications you no longer need

#### Notification Types
- **Appointment Completed**: When a doctor finishes a consultation
- **Next Patient Info**: Shows who the next patient is when available
- **Custom Notifications**: Extensible for future notification types

#### Usage for Doctors
1. Complete a consultation in the attendance page
2. Click "Finalizar e Notificar Secretaria" button
3. Secretary receives immediate notification
4. Confirmation message appears on screen

#### Usage for Secretary
1. Notification bell icon shows unread count
2. Click bell icon to open notification panel
3. View appointment completion notifications
4. See next patient information if available
5. Mark notifications as read or delete them

### 4. Doctor-Specific Appointment Views

The system now supports filtering appointments by doctor for role-based views.

#### Backend Changes
- **AppointmentEntity**: Added `PatientName`, `ClinicName`, `DoctorName` fields
- **AppointmentDto**: Updated to include name fields for display
- **Calendar Component**: Prepared to filter by `doctorId` when in doctor role

#### Future Enhancement
When user roles are fully implemented, doctors will automatically see only their own patients' appointments.

## Backend API Endpoints

### Notifications

#### GET `/api/notifications`
Get all notifications for the current tenant.

**Query Parameters:**
- `unreadOnly` (boolean): Only return unread notifications

**Response:** Array of `NotificationDto`

#### POST `/api/notifications/appointment-completed`
Send notification when appointment is completed.

**Body:** `AppointmentCompletedNotificationDto`
```json
{
  "appointmentId": "guid",
  "doctorName": "string",
  "patientName": "string",
  "completedAt": "datetime",
  "nextPatientId": "guid (optional)",
  "nextPatientName": "string (optional)"
}
```

#### PUT `/api/notifications/{id}/read`
Mark a notification as read.

#### PUT `/api/notifications/read-all`
Mark all notifications as read.

#### DELETE `/api/notifications/{id}`
Delete a notification.

### Appointments (Updated)

The appointment endpoints now return additional fields:
- `patientName`
- `clinicName`
- `doctorId`
- `doctorName`

## Database Changes

New table: `Notifications`
```sql
- Id (Guid, PK)
- Type (string)
- Title (string)
- Message (string)
- DataJson (string, nullable)
- IsRead (boolean)
- TenantId (string)
- UserId (Guid, nullable)
- CreatedAt (datetime)
- ReadAt (datetime, nullable)
```

Updated table: `Appointments`
```sql
+ PatientName (string)
+ ClinicName (string)
+ DoctorName (string, nullable)
```

## Technical Details

### Frontend Technologies
- **Angular 20.3**: Standalone components
- **Signals**: For reactive state management
- **RxJS**: For observable streams
- **SCSS**: For styling with CSS variables

### Backend Technologies
- **ASP.NET Core**: Microservice architecture
- **Entity Framework Core**: ORM
- **PostgreSQL**: Database
- **JWT Authentication**: Shared authentication library

### State Management
- Signals for local component state
- Services for shared state across components
- Observables for async operations

### Real-time Updates
Currently implemented as polling-based notifications. For true real-time updates, consider implementing:
- SignalR for server push notifications
- WebSockets for bidirectional communication

## Browser Compatibility

- Modern browsers (Chrome, Firefox, Edge, Safari)
- Browser notifications require user permission
- Audio notifications may be blocked by browser policies

## Accessibility

- Keyboard navigation supported
- ARIA labels on interactive elements
- Screen reader friendly
- Color contrast compliant

## Future Enhancements

1. **Real-time Updates**: Implement SignalR/WebSockets for instant notifications
2. **Role-Based Filtering**: Automatically filter appointments by logged-in doctor
3. **Drag-and-Drop**: Allow dragging appointments to reschedule
4. **Multi-clinic Support**: Filter calendar by clinic
5. **Appointment Conflicts**: Visual warnings for overlapping appointments
6. **Recurring Appointments**: Support for creating recurring appointment series
7. **Calendar Export**: Export appointments to iCal format
8. **Mobile Optimization**: Responsive calendar for mobile devices

## Testing

To test the new features:

1. **Calendar View**:
   - Navigate to `/appointments/calendar`
   - Click on different time slots
   - Verify pre-filled form data
   - Create appointments and verify they appear

2. **Notifications**:
   - Start an attendance session
   - Complete the consultation
   - Check notification appears in bell icon
   - Verify notification panel functionality
   - Test mark as read/delete operations

3. **Backend APIs**:
   - Use the Swagger UI at `/swagger`
   - Test notification endpoints
   - Verify appointment endpoints return new fields

## Troubleshooting

### Calendar not loading appointments
- Check browser console for errors
- Verify API connection to appointments microservice
- Check tenant ID is correctly set

### Notifications not appearing
- Verify backend notification service is running
- Check browser console for errors
- Ensure CORS is configured correctly

### Browser notifications not working
- Check browser permission settings
- Some browsers block notifications by default
- Try clicking "Ativar notificações do navegador" in notification panel

## Support

For issues or questions, please create a GitHub issue in the repository.
