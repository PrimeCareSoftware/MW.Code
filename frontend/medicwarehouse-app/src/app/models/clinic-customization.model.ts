export interface ClinicCustomizationDto {
  id: string;
  clinicId: string;
  logoUrl?: string;
  backgroundImageUrl?: string;
  primaryColor?: string;
  secondaryColor?: string;
  fontColor?: string;
  tenantId: string;
  createdAt: string;
  updatedAt: string;
}

export interface ClinicCustomizationPublicDto {
  clinicName: string;
  logoUrl?: string;
  backgroundImageUrl?: string;
  primaryColor?: string;
  secondaryColor?: string;
  fontColor?: string;
}

export interface UpdateClinicCustomizationRequest {
  primaryColor?: string;
  secondaryColor?: string;
  fontColor?: string;
}
