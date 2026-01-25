# Procedures Management - Two Workflow Options

## Overview
The system now provides two options for managing and recording procedures performed on patients, as requested. Both options are fully integrated with the billing system to calculate procedure costs.

## Option 1: Procedures in Patient Attendance Form

### Location
- **Menu Path**: Appointments → Calendar → Select Appointment → Start Attendance
- **Direct URL**: `/appointments/{appointmentId}/attendance`

### Workflow
1. **During Patient Consultation**
   - Doctor opens the patient attendance form
   - Form includes a "Procedimentos Realizados" (Procedures Performed) section
   
2. **Adding Procedures**
   - Click "+ Adicionar Procedimento" button
   - Select procedure from dropdown (shows all active clinic procedures with prices)
   - Optionally customize the price
   - Add notes about the procedure
   - Click "Adicionar" to add procedure to the appointment

3. **Cost Calculation**
   - System automatically calculates total cost of all procedures
   - Display shows: "Total: R$ {calculated_total}"
   - Each procedure shows its individual price

4. **Closing the Account**
   - When finishing the attendance, doctor can check "Registrar que recebi o pagamento"
   - Click "Finalizar Atendimento"
   - If payment registration was selected, choose who received payment:
     - Doctor
     - Secretary/Reception
     - Other Staff
   - System marks appointment as paid and records procedures with final costs

### Features
- ✅ Add multiple procedures to a single appointment
- ✅ Customize procedure prices on-the-fly
- ✅ Add procedure-specific notes
- ✅ Automatic cost calculation
- ✅ Integrated with payment registration
- ✅ Part of complete patient medical record

### Use Cases
- Quick procedure recording during consultation
- Procedures performed as part of regular appointment
- Immediate billing during patient visit

---

## Option 2: Standalone Procedures Management

### Location
- **Menu Path**: Procedimentos → Procedimentos da Clínica
- **Direct URL**: `/procedures`

### Workflow
1. **Accessing Procedures Management**
   - Click "Procedimentos da Clínica" in the sidebar menu (separate section above Compliance)
   - View list of all procedures available at the clinic

2. **Managing Procedures**
   - **View All**: See complete list with filters and search
   - **Search**: Find procedures by code, name, or description
   - **Create New**: Click "Novo Procedimento" to add new procedure
   - **Edit**: Click edit icon to modify existing procedure
   - **Deactivate**: Click delete icon to deactivate (soft delete)

3. **Creating/Editing Procedures**
   Form includes:
   - **Basic Information**:
     - Name (required, min 3 chars)
     - Code (required, unique, cannot be changed after creation)
     - Description (required)
   - **Category & Pricing**:
     - Category (Consultation, Exam, Surgery, Therapy, etc.)
     - Price in R$ (required)
     - Duration in minutes (required)
     - Requires special materials (checkbox)

4. **Procedure Categories**
   - Consulta (Consultation)
   - Exame (Exam)
   - Cirurgia (Surgery)
   - Terapia (Therapy)
   - Vacinação (Vaccination)
   - Diagnóstico (Diagnostic)
   - Tratamento (Treatment)
   - Emergência (Emergency)
   - Prevenção (Prevention)
   - Estética (Aesthetic)
   - Retorno (Follow-up)
   - Outros (Other)

### Features
- ✅ Full CRUD operations (Create, Read, Update, Delete)
- ✅ Search and filter functionality
- ✅ Status management (Active/Inactive)
- ✅ Detailed procedure information
- ✅ Price and duration management
- ✅ Category organization

### Use Cases
- Setting up clinic procedures catalog
- Updating procedure prices
- Managing procedure availability
- Administrative procedure configuration
- Defining procedures that will be used in attendance forms

---

## Integration Between All Options

### How They Work Together

1. **Procedure Catalog Setup** (Option 2)
   - Admin/Owner creates procedures in standalone management page
   - Defines prices, categories, and details
   - Activates/deactivates procedures as needed

2. **Procedure Usage** (Option 1)
   - During patient attendance, only active procedures appear in dropdown
   - Doctor selects from pre-configured procedures
   - System uses default price or allows customization
   - Procedures are recorded against the appointment

3. **Cross-Clinic Oversight** (Option 3)
   - Owners view consolidated procedures from all their clinics
   - Monitor pricing and catalog consistency
   - Access details and navigate to specific clinics for editing
   - Maintain quality standards across clinic network

4. **Billing Integration**
   - All procedures added during attendance contribute to total cost
   - System maintains detailed record of each procedure performed
   - Payment registration links to specific procedures
   - Financial reports can track procedure-based revenue

### Technical Notes

**Frontend Components**:
- `/pages/procedures/` - Standalone management pages
- `/pages/attendance/` - Attendance form with integrated procedure selection
- Shared models and services ensure consistency

**Backend API Endpoints**:
- `GET /api/procedures` - List all procedures
- `POST /api/procedures` - Create new procedure
- `PUT /api/procedures/{id}` - Update procedure
- `DELETE /api/procedures/{id}` - Deactivate procedure
- `POST /api/procedures/appointments/{appointmentId}/procedures` - Add procedure to appointment
- `GET /api/procedures/appointments/{appointmentId}/procedures` - Get appointment procedures
- `GET /api/procedures/appointments/{appointmentId}/billing-summary` - Get billing summary

**Permissions Required**:
- `procedures.view` - View procedures
- `procedures.create` - Create new procedures
- `procedures.edit` - Edit existing procedures
- `procedures.delete` - Deactivate procedures
- `procedures.manage` - Owner-level: Manage procedures across all owned clinics (see Option 3 below)

---

## Option 3: Owner Procedures Management (Cross-Clinic View)

### Location
- **Menu Path**: Procedimentos → Gerenciar Procedimentos (Proprietário)
- **Direct URL**: `/procedures/owner-management`
- **Availability**: Only visible to clinic owners

### Overview
For clinic owners who manage multiple clinics, this option provides a consolidated view of all procedures across all owned clinics. This is a read-only view designed for oversight and monitoring.

### Workflow
1. **Accessing Owner Management**
   - Menu item appears only for users with Owner or ClinicOwner role
   - Click "Gerenciar Procedimentos (Proprietário)" in the Procedimentos section
   - System automatically fetches procedures from all clinics owned by the user

2. **Viewing Consolidated Procedures**
   - See all procedures from all owned clinics in a single view
   - Statistics dashboard shows:
     - Total procedures count
     - Active procedures count
   - Data updates automatically based on ownership relationships

3. **Filtering and Search**
   - **Search**: Real-time search by code, name, or description
   - **Category Filter**: Filter by procedure category (Consultation, Exam, Surgery, etc.)
   - **Debounced Search**: Smooth UX with 300ms delay
   - Filters work on client-side for fast response

4. **Viewing Details**
   - Click view icon to see full procedure details
   - Redirects to the procedure edit form (in the context of that specific clinic)
   - Maintains proper clinic context for editing

### Features
- ✅ Cross-clinic visibility for procedures
- ✅ Consolidated statistics and counts
- ✅ Advanced filtering by category
- ✅ Real-time search functionality
- ✅ Automatic owner detection and verification
- ✅ Permission-based menu visibility
- ✅ Read-only view mode (viewing only, editing via clinic-specific interface)

### Use Cases
- **Multi-Clinic Oversight**: View all procedures across clinic network
- **Standardization Review**: Compare procedures between clinics
- **Price Monitoring**: Check procedure pricing consistency
- **Catalog Auditing**: Verify all clinics have necessary procedures
- **Quality Assurance**: Ensure procedure definitions are consistent

### Security & Permissions
- Requires `procedures.manage` permission
- Protected by both `authGuard` and `ownerGuard`
- Backend verifies owner status via database (not just JWT claims)
- Respects clinic ownership relationships via `OwnerClinicLink` table
- Prevents permission spoofing through server-side validation

### Technical Implementation
- **Backend**: Uses `GetByOwnerAsync()` with SQL JOIN on `OwnerClinicLink`
- **Performance**: Single query with JOIN avoids N+1 problem
- **Frontend**: Angular standalone component with lazy loading
- **State Management**: Uses Angular signals for reactive updates
- **Data Flow**: Automatic role detection → owner ID → cross-clinic query

### Differences from Option 2
| Feature | Option 2 (Clinic) | Option 3 (Owner) |
|---------|------------------|------------------|
| Scope | Single clinic | All owned clinics |
| Permissions | `procedures.*` | `procedures.manage` |
| Editing | Full CRUD | View only |
| Navigation | Create, Edit, Delete | View details |
| Use Case | Daily management | Oversight & monitoring |
| User Roles | Clinic admins | Clinic owners |

### Implementation Details
For full technical documentation, see: `PR367_OWNER_PROCEDURES_IMPLEMENTATION.md`

---

## User Guide Summary

### For System/Clinic Owners
Use **Option 3** (Owner Management) to:
- View procedures across all your clinics
- Monitor pricing and catalog consistency
- Oversee procedure standardization
- Audit procedure availability across clinic network

### For Clinic Administrators
Use **Option 2** (Standalone Management) to:
- Set up your clinic's procedure catalog
- Define standard prices and details
- Manage which procedures are available
- Update procedure information periodically

### For Healthcare Providers
Use **Option 1** (Attendance Form) to:
- Record procedures performed during consultation
- Generate patient bills with procedure costs
- Document procedures in patient medical records
- Receive payment and close accounts

### For Front Desk/Secretaries
- Can view procedures through attendance records
- Can help manage procedure catalog if given permissions
- Can receive payments when doctor delegates this task

---

## Benefits of This Implementation

1. ✅ **Flexibility**: Three ways to work with procedures based on role and context
2. ✅ **Integration**: All options share same data and billing system
3. ✅ **Accuracy**: Pre-configured procedures reduce data entry errors
4. ✅ **Control**: Centralized procedure management with proper permissions
5. ✅ **Traceability**: Every procedure is recorded with patient, date, and cost
6. ✅ **Billing**: Automatic cost calculation for accurate patient billing
7. ✅ **Multi-Clinic Support**: Owners can oversee multiple clinics efficiently
8. ✅ **Security**: Proper permission checks and role-based access control

---

## Implementation Status

✅ **Completed Features**:
- Procedures management page with list and form
- CRUD operations for procedures
- Menu integration with dedicated section
- Routes and navigation configured
- Existing attendance form already has procedure integration
- Cost calculation working
- Payment registration functional
- **Owner procedures management with cross-clinic visibility**
- **Permission-based menu item visibility**
- **Automatic owner detection and verification**

✅ **All Required Options Implemented**:
1. Procedures in attendance form (already existed, now documented)
2. Standalone procedures management page (existing feature)
3. Owner cross-clinic procedures management (newly created - PR 367)

