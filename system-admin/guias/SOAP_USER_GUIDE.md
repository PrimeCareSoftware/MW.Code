# SOAP Medical Record System - User Guide

## 📋 Table of Contents

- [Introduction](#introduction)
- [What is SOAP?](#what-is-soap)
- [Getting Started](#getting-started)
- [Creating a SOAP Record](#creating-a-soap-record)
- [The Four SOAP Sections](#the-four-soap-sections)
- [Completing and Locking Records](#completing-and-locking-records)
- [Viewing Patient History](#viewing-patient-history)
- [Best Practices](#best-practices)
- [FAQ](#faq)

## Introduction

The SOAP Medical Record system provides a **structured approach** to documenting patient encounters following the international SOAP (Subjective-Objective-Assessment-Plan) standard. This ensures:

✅ **Consistency** - Standardized format across all records  
✅ **Completeness** - Ensures all critical information is captured  
✅ **Quality** - Improves clinical documentation quality  
✅ **Searchability** - Structured data enables better searching and reporting  
✅ **Compliance** - Meets regulatory requirements (CFM 1.821)  
✅ **AI-Ready** - Structured data prepares for future AI analysis

## What is SOAP?

SOAP is an internationally recognized method for documenting patient encounters:

### 🔵 **S - Subjective**
Patient-reported information:
- Chief complaint ("What brought you here today?")
- History of present illness
- Symptoms and duration
- Allergies
- Current medications

### 🟢 **O - Objective**
Clinical observations and measurements:
- Vital signs (BP, HR, Temperature, SpO2, Weight, Height, BMI)
- Physical examination findings (by body system)
- Lab and imaging results

### 🟡 **A - Assessment**
Clinical interpretation:
- Primary diagnosis (with ICD-10 code)
- Differential diagnoses
- Clinical reasoning
- Prognosis

### 🔴 **P - Plan**
Treatment and follow-up:
- Prescriptions
- Exam requests
- Procedures
- Referrals
- Patient instructions

## Getting Started

### Accessing SOAP Records

1. Navigate to **Appointments** in the main menu
2. Select an appointment with status "In Progress" or "Completed"
3. Click **"Create SOAP Record"** button
4. The system will create a new SOAP record linked to the appointment

### Prerequisites

- The appointment must exist in the system
- You must have appropriate permissions (Doctor, Nurse, or Admin)
- Patient demographic information must be complete

## Creating a SOAP Record

### Step-by-Step Workflow

The SOAP record interface uses a **4-step wizard** (Material Stepper):

```
Step 1: Subjective → Step 2: Objective → Step 3: Assessment → Step 4: Plan → Review & Complete
```

Each step can be:
- ✅ **Completed** - All required fields filled
- ⏳ **In Progress** - Partially filled
- ⚠️ **Incomplete** - Missing required fields

### Saving Your Work

- Click **"Save"** button at the bottom of each section
- Data is saved immediately to the server
- You can navigate between steps without losing data
- The record remains editable until you complete and lock it

## The Four SOAP Sections

### 📝 Section 1: Subjective (Patient-Reported Data)

**Required Fields:**
- **Chief Complaint** - Main reason for visit (minimum 10 characters)
- **History of Present Illness (HDA)** - Detailed description (minimum 50 characters)

**Optional Fields:**
- Current Symptoms
- Symptom Duration  
- Aggravating Factors (what makes it worse)
- Relieving Factors (what makes it better)
- Review of Systems
- Allergies
- Current Medications
- Past Medical History
- Family History
- Social History (lifestyle habits)

**Tips:**
- Use patient's own words for chief complaint
- Include onset, duration, severity, and characteristics
- Document both positive and negative findings

### 🔬 Section 2: Objective (Clinical Observations)

**Vital Signs:**

| Measurement | Unit | Normal Range |
|-------------|------|--------------|
| Systolic BP | mmHg | 90-120 |
| Diastolic BP | mmHg | 60-80 |
| Heart Rate | bpm | 60-100 |
| Respiratory Rate | rpm | 12-20 |
| Temperature | °C | 36.1-37.2 |
| SpO2 | % | 95-100 |
| Weight | kg | Variable |
| Height | cm | Variable |
| **BMI** | calculated | 18.5-24.9 (normal) |

**Physical Examination Sections:**
- General Appearance
- Head, Eyes, Ears, Nose, Throat (HEENT)
- Neck
- Cardiovascular System
- Respiratory System
- Abdomen
- Musculoskeletal System
- Neurological System
- Skin

**Lab and Imaging Results:**
- Laboratory test results
- Imaging study results
- Other exam findings

**Tips:**
- BMI is auto-calculated from weight and height
- Use standard medical terminology
- Document "normal" or "unremarkable" when appropriate
- Note any abnormal findings with details

### 🔍 Section 3: Assessment (Clinical Interpretation)

**Required:**
- **Primary Diagnosis** - Main clinical diagnosis

**Recommended:**
- **ICD-10 Code** - International disease classification code
- **Differential Diagnoses** - Alternative possible diagnoses (with justification and priority)
- **Clinical Reasoning** - Explanation of diagnostic thought process
- **Prognosis** - Expected outcome
- **Evolution** - Disease progression notes

**Differential Diagnoses:**
- Add multiple alternative diagnoses
- Set priority (1 = most likely)
- Include ICD-10 codes when available
- Justify each differential

**Tips:**
- Be specific with diagnoses
- Always use ICD-10 codes for billing and statistics
- Document uncertainty when appropriate
- Explain clinical reasoning for educational purposes

### 📋 Section 4: Plan (Treatment and Follow-up)

**Prescriptions:**

For each medication, specify:
- Medication Name (required)
- Dosage (e.g., "500mg")
- Frequency (e.g., "2x/day")
- Duration (e.g., "7 days")
- Special Instructions

**Exam Requests:**

For each exam, specify:
- Exam Name (required)
- Exam Type (Lab, Imaging, etc.)
- Clinical Indication
- Urgency (routine or urgent)

**Procedures:**

For each procedure:
- Procedure Name (required)
- Description
- Scheduled Date (optional)

**Referrals:**

For each referral:
- Specialty Name (required)
- Reason for referral
- Priority (Routine, Urgent, Emergency)

**Patient Instructions:**
- Return Instructions
- Next Appointment Date
- General Patient Instructions
- Dietary Recommendations
- Activity Restrictions
- Warning Symptoms (when to seek immediate care)

**Tips:**
- Be clear and specific with all instructions
- Use layman's terms for patient instructions
- Document all prescriptions even if e-prescribed separately
- Include follow-up timeline

## Completing and Locking Records

### Validation Requirements

Before completing a SOAP record, the system validates:

✅ **Subjective:** Chief Complaint AND History of Present Illness  
✅ **Objective:** Vital Signs OR Physical Examination findings  
✅ **Assessment:** Primary Diagnosis  
✅ **Plan:** At least ONE of: Prescription, Exam Request, or Patient Instructions

### How to Complete

1. Fill out all four SOAP sections
2. Click **"Review"** to see the summary
3. Verify all information is correct
4. Click **"Complete and Lock"** button
5. Confirm the action in the dialog

### Locked Status

Once locked:
- ✅ Record becomes **read-only**
- ✅ Completion date is recorded
- ✅ Record is marked as complete
- ❌ **Cannot be edited** (ensures data integrity)

### Unlocking Records

**Only administrators** can unlock records:

1. Open the locked SOAP record
2. Click **"Unlock"** button (admin only)
3. Confirm the action
4. Record returns to editable state

**Important:** Unlocking should only be done for legitimate corrections, not routine changes.

## Viewing Patient History

### SOAP Records List

Access from:
- **Patient Profile** → SOAP Records tab
- **Appointments** → View Appointment → SOAP Records
- **Main Menu** → SOAP Records

**List Features:**
- Filter by Patient
- Filter by Doctor
- Sort by Date
- Search by Diagnosis
- Status indicators (Complete, In Progress, Locked)

### Record Details

Click any record to view:
- Complete SOAP documentation
- Completion status
- Creation and completion dates
- Associated appointment details
- Doctor information

## Best Practices

### Documentation Quality

✅ **DO:**
- Document promptly after patient encounter
- Be thorough but concise
- Use standard medical terminology
- Include both positive and negative findings
- Specify measurements with units
- Date and sign (system does this automatically)

❌ **DON'T:**
- Copy-paste from previous records without updating
- Leave required fields blank
- Use unclear abbreviations
- Document subjective opinions as objective facts
- Complete records before patient leaves

### Clinical Best Practices

1. **Subjective:** Let patient tell their story in their own words
2. **Objective:** Focus on measurable, observable data
3. **Assessment:** Base diagnosis on objective evidence
4. **Plan:** Ensure plan addresses the assessment
5. **Follow-up:** Always specify when patient should return

### Time Management

- Use templates for common conditions (future feature)
- Complete SOAP while patient details are fresh
- Review and complete before end of day
- Lock records promptly to prevent accidental edits

### Legal and Compliance

- SOAP records are **legal medical documents**
- All entries are **timestamped and attributed**
- Locked records ensure **data integrity**
- System maintains **complete audit trail**
- Complies with **CFM 1.821** regulations

## FAQ

### General Questions

**Q: Can I edit a SOAP record after creating it?**  
A: Yes, until you click "Complete and Lock". After locking, only admins can unlock for corrections.

**Q: What happens if I don't complete a section?**  
A: The record is saved with whatever data you entered. You can complete it later, but it won't pass validation for completion.

**Q: Can I create multiple SOAP records for one appointment?**  
A: No, there's a one-to-one relationship. One appointment = one SOAP record.

**Q: What if the patient has no current medications or allergies?**  
A: Document "None" or "No known allergies" to show you asked.

### Technical Questions

**Q: Is data saved automatically?**  
A: No, you must click the "Save" button for each section. However, navigation between steps is safe once saved.

**Q: Can I access SOAP records offline?**  
A: No, SOAP records require server connection for data integrity and security.

**Q: What if the system crashes while I'm documenting?**  
A: Your last saved data is preserved. Unsaved changes in the current section may be lost.

**Q: How is BMI calculated?**  
A: BMI = Weight (kg) / [Height (m)]². The system calculates this automatically when you enter weight and height.

### Workflow Questions

**Q: Should I document before or after seeing the patient?**  
A: Document subjective data during the encounter. Complete objective, assessment, and plan immediately after.

**Q: What if I'm interrupted during documentation?**  
A: Save your current section and continue later. The record remains editable until locked.

**Q: Can I print SOAP records?**  
A: Yes (future feature), a formatted PDF will be available from the summary view.

**Q: How do I search for a specific diagnosis?**  
A: Use the SOAP Records list page and filter by ICD-10 code or diagnosis text.

### Clinical Questions

**Q: What if I have multiple diagnoses?**  
A: List the most important as Primary Diagnosis, add others as Differential Diagnoses with priorities.

**Q: How detailed should physical exam documentation be?**  
A: Document all relevant findings. For normal systems, "WNL" (Within Normal Limits) is acceptable.

**Q: What if patient refuses treatment?**  
A: Document this in the Plan section under Patient Instructions, including patient's reasoning.

**Q: How do I document emergency situations?**  
A: Complete critical information immediately (vital signs, chief complaint, immediate plan). Complete full SOAP when situation stabilizes.

## Support

For technical support or questions about the SOAP system:

📧 **Email:** support@medicwarehouse.com  
📞 **Phone:** (11) 1234-5678  
📚 **Documentation:** docs.medicwarehouse.com  
💬 **Help Desk:** support.medicwarehouse.com

---

**Version:** 1.0  
**Last Updated:** January 2026  
**System:** PrimeCare Medical Records
