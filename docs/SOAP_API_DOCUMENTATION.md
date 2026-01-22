# SOAP Records API Documentation

## Base URL

```
/api/SoapRecords
```

## Authentication

All endpoints require authentication using Bearer token:

```http
Authorization: Bearer {jwt_token}
```

## Tenant Isolation

All requests are automatically scoped to the authenticated user's tenant. The `TenantId` is extracted from the JWT token and used for all database operations.

## Endpoints

### 1. Create SOAP Record

Creates a new SOAP record for an appointment.

**Endpoint:** `POST /api/SoapRecords/appointment/{appointmentId}`

**Parameters:**
- `appointmentId` (path, required): GUID of the appointment

**Request Body:** None

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "appointmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "recordDate": "2026-01-22T17:00:00Z",
  "subjective": null,
  "objective": null,
  "assessment": null,
  "plan": null,
  "isComplete": false,
  "completionDate": null,
  "isLocked": false
}
```

**Errors:**
- `400 Bad Request` - Invalid appointment ID
- `401 Unauthorized` - Not authenticated
- `404 Not Found` - Appointment not found
- `409 Conflict` - SOAP record already exists for this appointment

---

### 2. Update Subjective Section

Updates the Subjective (S) section of the SOAP record.

**Endpoint:** `PUT /api/SoapRecords/{id}/subjective`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:**

```json
{
  "chiefComplaint": "Severe headache for 3 days",
  "historyOfPresentIllness": "Patient reports constant throbbing headache starting 3 days ago, primarily in frontal region. Pain rated 7/10. No associated vision changes or vomiting. Worse in morning, improves slightly with rest.",
  "currentSymptoms": "Headache, mild nausea, photophobia",
  "symptomDuration": "3 days",
  "aggravatingFactors": "Bright lights, loud noises, physical activity",
  "relievingFactors": "Rest, dark room, cold compress",
  "reviewOfSystems": "Negative for fever, neck stiffness, vision changes, neurological symptoms",
  "allergies": "Penicillin (rash)",
  "currentMedications": "Lisinopril 10mg daily for hypertension, Metformin 500mg twice daily",
  "pastMedicalHistory": "Hypertension (diagnosed 2020), Type 2 Diabetes (diagnosed 2021)",
  "familyHistory": "Mother has migraines, Father has hypertension",
  "socialHistory": "Non-smoker, occasional alcohol use (1-2 drinks/week), office worker"
}
```

**Response:** `200 OK` - Returns updated SoapRecordDto

**Errors:**
- `400 Bad Request` - Invalid data or locked record
- `404 Not Found` - SOAP record not found

---

### 3. Update Objective Section

Updates the Objective (O) section of the SOAP record.

**Endpoint:** `PUT /api/SoapRecords/{id}/objective`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:**

```json
{
  "vitalSigns": {
    "systolicBP": 128,
    "diastolicBP": 82,
    "heartRate": 76,
    "respiratoryRate": 16,
    "temperature": 36.8,
    "oxygenSaturation": 98,
    "weight": 72.5,
    "height": 170,
    "bmi": 25.1,
    "pain": 7
  },
  "physicalExam": {
    "generalAppearance": "Alert and oriented, appears uncomfortable due to pain",
    "head": "Normocephalic, atraumatic",
    "eyes": "PERRLA, no papilledema, extraocular movements intact",
    "ears": "Tympanic membranes normal bilaterally",
    "nose": "No discharge, mucosa normal",
    "throat": "Oropharynx clear, no lesions",
    "neck": "Supple, no nuchal rigidity, no lymphadenopathy",
    "cardiovascular": "Regular rate and rhythm, no murmurs",
    "respiratory": "Clear to auscultation bilaterally, no wheezes",
    "abdomen": "Soft, non-tender, normal bowel sounds",
    "musculoskeletal": "Normal gait, full range of motion",
    "neurological": "Cranial nerves II-XII intact, normal strength and sensation",
    "skin": "No rashes or lesions",
    "otherFindings": "Mild tenderness on palpation over frontal sinuses"
  },
  "labResults": "Pending: CBC, metabolic panel",
  "imagingResults": "CT head ordered - pending",
  "otherExamResults": "None"
}
```

**Response:** `200 OK` - Returns updated SoapRecordDto

**Notes:**
- BMI is calculated automatically by the backend if weight and height are provided
- All vital signs fields are optional
- Physical exam sections are all optional

---

### 4. Update Assessment Section

Updates the Assessment (A) section of the SOAP record.

**Endpoint:** `PUT /api/SoapRecords/{id}/assessment`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:**

```json
{
  "primaryDiagnosis": "Tension-type headache",
  "primaryDiagnosisIcd10": "G44.209",
  "differentialDiagnoses": [
    {
      "diagnosis": "Migraine without aura",
      "icd10Code": "G43.009",
      "justification": "Photophobia and throbbing nature suggest migraine, but duration and lack of other migraine features make this less likely",
      "priority": 1
    },
    {
      "diagnosis": "Hypertensive headache",
      "icd10Code": "G44.81",
      "justification": "BP slightly elevated but not in hypertensive crisis range",
      "priority": 2
    },
    {
      "diagnosis": "Sinusitis",
      "icd10Code": "J32.9",
      "justification": "Frontal tenderness present but no other typical sinus symptoms",
      "priority": 3
    }
  ],
  "clinicalReasoning": "Given the constant, non-pulsatile nature of the headache, duration of 3 days, bilateral frontal location, and lack of associated neurological symptoms, tension-type headache is most likely. Physical exam shows no concerning findings. Patient's stress from work may be contributing factor. CT ordered to rule out structural pathology given severity and persistence.",
  "prognosis": "Good with appropriate treatment and stress management",
  "evolution": "New onset headache, progressively improving slightly over past 24 hours"
}
```

**Response:** `200 OK` - Returns updated SoapRecordDto

**Notes:**
- Only `primaryDiagnosis` is required
- ICD-10 codes are recommended but not required
- Differential diagnoses are optional but recommended
- Priority starts at 1 (most likely)

---

### 5. Update Plan Section

Updates the Plan (P) section of the SOAP record.

**Endpoint:** `PUT /api/SoapRecords/{id}/plan`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:**

```json
{
  "prescriptions": [
    {
      "medicationName": "Ibuprofen",
      "dosage": "400mg",
      "frequency": "Every 6 hours as needed",
      "duration": "7 days",
      "instructions": "Take with food to reduce stomach upset"
    },
    {
      "medicationName": "Cyclobenzaprine",
      "dosage": "10mg",
      "frequency": "At bedtime",
      "duration": "5 days",
      "instructions": "May cause drowsiness, do not drive or operate machinery"
    }
  ],
  "examRequests": [
    {
      "examName": "CT Head without contrast",
      "examType": "Imaging",
      "clinicalIndication": "Rule out structural pathology in patient with severe persistent headache",
      "isUrgent": false
    },
    {
      "examName": "Complete Blood Count (CBC)",
      "examType": "Lab",
      "clinicalIndication": "Baseline evaluation",
      "isUrgent": false
    }
  ],
  "procedures": [],
  "referrals": [
    {
      "specialtyName": "Neurology",
      "reason": "Persistent headaches despite treatment, for further evaluation and management",
      "priority": "Routine"
    }
  ],
  "returnInstructions": "Return in 7 days for follow-up. Return immediately if headache worsens, vision changes, fever, neck stiffness, or neurological symptoms develop.",
  "nextAppointmentDate": "2026-01-29T10:00:00Z",
  "patientInstructions": "Take medications as prescribed. Apply warm compress to neck and shoulders. Reduce screen time and take frequent breaks from work. Stay well hydrated. Maintain regular sleep schedule.",
  "dietaryRecommendations": "Avoid caffeine, alcohol, and processed foods. Maintain regular meals.",
  "activityRestrictions": "Avoid strenuous exercise for 3 days. May continue normal daily activities as tolerated.",
  "warningSymptoms": "Sudden severe headache (thunderclap), vision changes, confusion, weakness, numbness, difficulty speaking, fever with stiff neck"
}
```

**Response:** `200 OK` - Returns updated SoapRecordDto

**Notes:**
- At least one of prescriptions, examRequests, or patientInstructions is recommended
- All arrays can be empty
- Priority for referrals: "Routine", "Urgent", or "Emergency"

---

### 6. Complete SOAP Record

Validates and marks the SOAP record as complete, then locks it.

**Endpoint:** `POST /api/SoapRecords/{id}/complete`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:** None

**Response:** `200 OK` - Returns updated SoapRecordDto with `isComplete=true` and `isLocked=true`

**Validation Rules:**
- ✅ Subjective section must have ChiefComplaint AND HistoryOfPresentIllness
- ✅ Objective section must have VitalSigns OR PhysicalExam
- ✅ Assessment section must have PrimaryDiagnosis
- ✅ Plan section must have at least one of: Prescriptions, ExamRequests, or PatientInstructions

**Errors:**
- `400 Bad Request` - Validation failed (returns list of missing fields)
- `404 Not Found` - SOAP record not found
- `409 Conflict` - Already completed

**Example Error Response:**

```json
{
  "message": "Prontuário incompleto",
  "missingFields": [
    "Subjective section (Chief Complaint and History of Present Illness are required)",
    "Assessment section (Primary Diagnosis required)"
  ]
}
```

---

### 7. Unlock SOAP Record

Unlocks a previously completed SOAP record for editing.

**Endpoint:** `POST /api/SoapRecords/{id}/unlock`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Request Body:** None

**Response:** `200 OK` - Returns updated SoapRecordDto with `isComplete=false` and `isLocked=false`

**Permissions:** Requires admin privileges

**Errors:**
- `400 Bad Request` - Record not locked
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - SOAP record not found

---

### 8. Get SOAP Record by ID

Retrieves a SOAP record by its ID.

**Endpoint:** `GET /api/SoapRecords/{id}`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Response:** `200 OK` - Returns SoapRecordDto

**Errors:**
- `404 Not Found` - SOAP record not found

---

### 9. Get SOAP Record by Appointment

Retrieves a SOAP record by appointment ID.

**Endpoint:** `GET /api/SoapRecords/appointment/{appointmentId}`

**Parameters:**
- `appointmentId` (path, required): GUID of the appointment

**Response:** `200 OK` - Returns SoapRecordDto

**Errors:**
- `404 Not Found` - SOAP record not found for this appointment

---

### 10. Get Patient's SOAP Records

Retrieves all SOAP records for a specific patient.

**Endpoint:** `GET /api/SoapRecords/patient/{patientId}`

**Parameters:**
- `patientId` (path, required): GUID of the patient

**Response:** `200 OK` - Returns array of SoapRecordDto

```json
[
  {
    "id": "...",
    "recordDate": "2026-01-22T17:00:00Z",
    "isComplete": true,
    "isLocked": true,
    ...
  },
  {
    "id": "...",
    "recordDate": "2026-01-15T14:30:00Z",
    "isComplete": true,
    "isLocked": true,
    ...
  }
]
```

**Notes:**
- Results are sorted by `recordDate` descending (newest first)
- Empty array returned if no records found

---

### 11. Get Doctor's SOAP Records

Retrieves all SOAP records created by a specific doctor.

**Endpoint:** `GET /api/SoapRecords/doctor/{doctorId}`

**Parameters:**
- `doctorId` (path, required): GUID of the doctor/user

**Response:** `200 OK` - Returns array of SoapRecordDto

**Notes:**
- Results are sorted by `recordDate` descending (newest first)
- Empty array returned if no records found

---

### 12. Validate SOAP Record

Checks if a SOAP record is complete and ready to be locked.

**Endpoint:** `GET /api/SoapRecords/{id}/validate`

**Parameters:**
- `id` (path, required): GUID of the SOAP record

**Response:** `200 OK`

```json
{
  "isValid": true,
  "hasSubjective": true,
  "hasObjective": true,
  "hasAssessment": true,
  "hasPlan": true,
  "missingFields": [],
  "warnings": []
}
```

**Example Invalid Response:**

```json
{
  "isValid": false,
  "hasSubjective": true,
  "hasObjective": false,
  "hasAssessment": false,
  "hasPlan": true,
  "missingFields": [
    "Objective section (Vital Signs or Physical Examination required)",
    "Assessment section (Primary Diagnosis required)"
  ],
  "warnings": []
}
```

**Errors:**
- `404 Not Found` - SOAP record not found

---

## Data Models

### SoapRecordDto

```typescript
interface SoapRecordDto {
  id: string;
  appointmentId: string;
  patientId: string;
  doctorId: string;
  recordDate: string; // ISO 8601
  subjective: SubjectiveDataDto | null;
  objective: ObjectiveDataDto | null;
  assessment: AssessmentDataDto | null;
  plan: PlanDataDto | null;
  isComplete: boolean;
  completionDate: string | null; // ISO 8601
  isLocked: boolean;
}
```

### SubjectiveDataDto

```typescript
interface SubjectiveDataDto {
  chiefComplaint: string; // Required
  historyOfPresentIllness: string; // Required
  currentSymptoms?: string;
  symptomDuration?: string;
  aggravatingFactors?: string;
  relievingFactors?: string;
  reviewOfSystems?: string;
  allergies?: string;
  currentMedications?: string;
  pastMedicalHistory?: string;
  familyHistory?: string;
  socialHistory?: string;
}
```

### ObjectiveDataDto

```typescript
interface ObjectiveDataDto {
  vitalSigns?: VitalSignsDto;
  physicalExam?: PhysicalExaminationDto;
  labResults?: string;
  imagingResults?: string;
  otherExamResults?: string;
}
```

### VitalSignsDto

```typescript
interface VitalSignsDto {
  systolicBP?: number; // mmHg (0-300)
  diastolicBP?: number; // mmHg (0-200)
  heartRate?: number; // bpm (0-300)
  respiratoryRate?: number; // rpm (0-100)
  temperature?: number; // °C (32-45)
  oxygenSaturation?: number; // % (0-100)
  weight?: number; // kg (0-500)
  height?: number; // cm (0-300)
  bmi?: number; // auto-calculated
  pain?: number; // 0-10 scale
}
```

### PhysicalExaminationDto

```typescript
interface PhysicalExaminationDto {
  generalAppearance?: string;
  head?: string;
  eyes?: string;
  ears?: string;
  nose?: string;
  throat?: string;
  neck?: string;
  cardiovascular?: string;
  respiratory?: string;
  abdomen?: string;
  musculoskeletal?: string;
  neurological?: string;
  skin?: string;
  otherFindings?: string;
}
```

### AssessmentDataDto

```typescript
interface AssessmentDataDto {
  primaryDiagnosis?: string;
  primaryDiagnosisIcd10?: string;
  differentialDiagnoses: DifferentialDiagnosisDto[];
  clinicalReasoning?: string;
  prognosis?: string;
  evolution?: string;
}
```

### DifferentialDiagnosisDto

```typescript
interface DifferentialDiagnosisDto {
  diagnosis: string; // Required
  icd10Code?: string;
  justification?: string;
  priority: number; // 1 = most likely
}
```

### PlanDataDto

```typescript
interface PlanDataDto {
  prescriptions: SoapPrescriptionDto[];
  examRequests: SoapExamRequestDto[];
  procedures: SoapProcedureDto[];
  referrals: SoapReferralDto[];
  returnInstructions?: string;
  nextAppointmentDate?: string; // ISO 8601
  patientInstructions?: string;
  dietaryRecommendations?: string;
  activityRestrictions?: string;
  warningSymptoms?: string;
}
```

### SoapPrescriptionDto

```typescript
interface SoapPrescriptionDto {
  medicationName: string; // Required
  dosage?: string;
  frequency?: string;
  duration?: string;
  instructions?: string;
}
```

### SoapExamRequestDto

```typescript
interface SoapExamRequestDto {
  examName: string; // Required
  examType?: string; // Lab, Imaging, etc.
  clinicalIndication?: string;
  isUrgent: boolean;
}
```

### SoapProcedureDto

```typescript
interface SoapProcedureDto {
  procedureName: string; // Required
  description?: string;
  scheduledDate?: string; // ISO 8601
}
```

### SoapReferralDto

```typescript
interface SoapReferralDto {
  specialtyName: string; // Required
  reason?: string;
  priority?: string; // "Routine", "Urgent", "Emergency"
}
```

### SoapRecordValidationDto

```typescript
interface SoapRecordValidationDto {
  isValid: boolean;
  hasSubjective: boolean;
  hasObjective: boolean;
  hasAssessment: boolean;
  hasPlan: boolean;
  missingFields: string[];
  warnings: string[];
}
```

---

## Error Responses

All error responses follow this format:

```json
{
  "message": "Error description",
  "details": "Additional details (optional)"
}
```

### Common HTTP Status Codes

- `200 OK` - Request succeeded
- `400 Bad Request` - Invalid request data or validation failed
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource already exists
- `500 Internal Server Error` - Server error

---

## Rate Limiting

Currently no rate limiting is implemented. Future versions may include:
- 100 requests per minute per user
- 1000 requests per hour per tenant

---

## Changelog

### Version 1.0 (January 2026)
- Initial API release
- 12 endpoints implemented
- Complete SOAP workflow support
- Tenant isolation
- Permission-based access control

---

## Support

For API support:

📧 **Email:** api-support@medicwarehouse.com  
📚 **Documentation:** docs.medicwarehouse.com/api  
💬 **Developer Portal:** developers.medicwarehouse.com

---

**API Version:** 1.0  
**Last Updated:** January 2026
