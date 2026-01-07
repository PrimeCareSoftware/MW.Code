export enum DocumentType {
  Prescription = 'Prescription',
  MedicalCertificate = 'MedicalCertificate',
  LabReport = 'LabReport',
  ImagingReport = 'ImagingReport',
  MedicalReport = 'MedicalReport',
  Other = 'Other'
}

export interface Document {
  id: string;
  title: string;
  documentType: string;
  description?: string;
  doctorName: string;
  issuedDate: Date;
  fileUrl?: string;
  fileName?: string;
  fileSizeFormatted?: string;
  isAvailable: boolean;
}
