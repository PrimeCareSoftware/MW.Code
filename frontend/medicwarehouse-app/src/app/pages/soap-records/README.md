# SOAP Medical Records Module

## Overview

This module implements a complete SOAP (Subjective-Objective-Assessment-Plan) medical record system for structured clinical documentation following international standards.

## Structure

```
soap-records/
├── components/
│   ├── subjective-form/         # S - Subjective data form
│   ├── objective-form/          # O - Objective data form  
│   ├── assessment-form/         # A - Assessment form
│   ├── plan-form/               # P - Plan form
│   ├── soap-summary/            # Summary view of complete SOAP
│   └── soap-list/               # List of SOAP records
├── models/
│   └── soap-record.model.ts     # TypeScript interfaces and types
├── services/
│   └── soap-record.service.ts   # API service for SOAP records
├── soap-record.component.ts     # Main component with stepper
├── soap-records.routes.ts       # Routing configuration
└── index.ts                     # Public exports
```

## Features

### 1. Subjective Form (S)
- Chief complaint
- History of present illness
- Current symptoms
- Allergies
- Current medications
- Medical history
- Family history
- Social history

### 2. Objective Form (O)
- **Vital Signs:**
  - Blood pressure (systolic/diastolic)
  - Heart rate
  - Respiratory rate
  - Temperature
  - Oxygen saturation (SpO2)
  - Weight & Height with **auto-calculated BMI**
  - Pain scale (0-10)

- **Physical Examination** (collapsible sections):
  - General appearance
  - Head, Eyes, Ears, Nose, Throat (HEENT)
  - Neck
  - Cardiovascular
  - Respiratory
  - Abdomen
  - Musculoskeletal
  - Neurological
  - Skin
  - Other findings

- **Exam Results:**
  - Laboratory results
  - Imaging results
  - Other complementary exams

### 3. Assessment Form (A)
- Primary diagnosis with ICD-10 code
- Differential diagnoses (multiple, with priority)
- Clinical reasoning
- Prognosis
- Evolution

### 4. Plan Form (P)
- **Prescriptions** (multiple):
  - Medication name
  - Dosage
  - Frequency
  - Duration
  - Instructions

- **Exam Requests** (multiple):
  - Exam name and type
  - Clinical indication
  - Urgency flag

- **Procedures** (multiple):
  - Procedure name
  - Description
  - Scheduled date

- **Referrals** (multiple):
  - Specialty
  - Reason
  - Priority

- **Patient Instructions:**
  - Return instructions
  - Next appointment date
  - General instructions
  - Dietary recommendations
  - Activity restrictions
  - Warning symptoms

### 5. SOAP Summary
- Read-only view of all sections
- Completion status indicators
- Complete & Lock functionality
- Visual status badges

## Usage

### Creating a new SOAP record

```typescript
// Route: /soap-records/new/:attendanceId
// Automatically creates SOAP record for an attendance
```

### Viewing existing SOAP record

```typescript
// Route: /soap-records/:id
// Loads existing SOAP record with all data
```

### Navigation Flow

The application uses Angular Material Stepper with 5 steps:

1. **S** - Subjective (required fields marked)
2. **O** - Objective (with BMI auto-calculation)
3. **A** - Assessment (with ICD-10 codes)
4. **P** - Plan (prescriptions, exams, referrals)
5. **Review** - Summary and complete

Each step:
- Can be edited independently
- Saves data on "Save and Advance"
- Shows completion status
- Allows navigation between steps

### Completing a SOAP Record

Once all 4 sections are complete:
1. Navigate to Review step
2. Verify all information
3. Click "Complete and Lock"
4. Record becomes read-only and locked

## API Integration

### Service Methods

```typescript
// Create new SOAP record
createSoapRecord(attendanceId: string): Observable<SoapRecord>

// Update sections
updateSubjective(soapId: string, data: UpdateSubjectiveCommand): Observable<SoapRecord>
updateObjective(soapId: string, data: UpdateObjectiveCommand): Observable<SoapRecord>
updateAssessment(soapId: string, data: UpdateAssessmentCommand): Observable<SoapRecord>
updatePlan(soapId: string, data: UpdatePlanCommand): Observable<SoapRecord>

// Complete and lock
completeSoapRecord(soapId: string): Observable<SoapRecord>

// Retrieve records
getSoapRecord(soapId: string): Observable<SoapRecord>
getPatientSoapRecords(patientId: string): Observable<SoapRecord[]>

// Validation
validateSoapRecord(soapId: string): Observable<SoapRecordValidation>
```

### Backend Endpoints

- `POST /api/SoapRecords/attendance/:attendanceId` - Create
- `PUT /api/SoapRecords/:id/subjective` - Update subjective
- `PUT /api/SoapRecords/:id/objective` - Update objective
- `PUT /api/SoapRecords/:id/assessment` - Update assessment
- `PUT /api/SoapRecords/:id/plan` - Update plan
- `POST /api/SoapRecords/:id/complete` - Complete and lock
- `GET /api/SoapRecords/:id` - Get by ID
- `GET /api/SoapRecords/patient/:patientId` - Get by patient
- `GET /api/SoapRecords/:id/validate` - Validate completeness

## Validation

### Required Fields

**Subjective:**
- Chief complaint
- History of present illness
- Allergies
- Current medications

**Assessment:**
- Primary diagnosis
- Primary diagnosis ICD-10
- Clinical reasoning

**Plan:**
- Return instructions
- Patient instructions

### Completion Requirements

All 4 sections (S-O-A-P) must have data before the record can be completed and locked.

## Styling

The module uses:
- Angular Material components
- Consistent color scheme (primary: #1976d2)
- Responsive grid layouts
- Status indicators with icons
- Card-based layouts
- Expansion panels for physical exam

## Benefits

1. **Structured Data**: Better quality than free text
2. **Clinical Standards**: Follows international SOAP format
3. **Completeness**: Validation ensures all required data
4. **Security**: Locked records prevent tampering
5. **Analysis Ready**: Structured data enables statistics and AI
6. **ICD-10 Integration**: Standardized diagnosis codes
7. **User Experience**: Step-by-step guided workflow

## Future Enhancements

- [ ] ICD-10 code search/autocomplete
- [ ] Template library for common conditions
- [ ] AI-assisted clinical reasoning suggestions
- [ ] Voice-to-text for faster documentation
- [ ] Print/PDF generation
- [ ] Digital signature integration
- [ ] Audit trail for changes
- [ ] Statistics and analytics dashboard

## Technical Stack

- **Angular 20**: Latest Angular features
- **Angular Material**: UI components and theming
- **Reactive Forms**: Form handling and validation
- **Standalone Components**: Modern Angular architecture
- **RxJS**: Reactive programming for API calls
- **TypeScript**: Strong typing and interfaces

## Dependencies

```json
{
  "@angular/core": "^20.x",
  "@angular/material": "^20.x",
  "@angular/forms": "^20.x",
  "@angular/common": "^20.x",
  "@angular/router": "^20.x"
}
```

## Routes Configuration

Add to your main app routes:

```typescript
{
  path: 'soap-records',
  loadChildren: () => import('./pages/soap-records/soap-records.routes')
    .then(m => m.SOAP_ROUTES),
  canActivate: [authGuard]
}
```

## Authors

- Frontend Implementation: AI Assistant
- Based on specification: docs/prompts-copilot/alta/06-prontuario-soap.md

## Last Updated

January 2026
