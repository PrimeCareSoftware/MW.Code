export interface ModuleInfo {
  name: string;
  displayName: string;
  description: string;
  category: 'Core' | 'Advanced' | 'Premium' | 'Analytics';
  icon: string;
  isCore: boolean;
  requiredModules: string[];
  minimumPlan: string;
}

export interface ModuleConfig {
  moduleName: string;
  displayName: string;
  description: string;
  category: string;
  icon: string;
  isEnabled: boolean;
  isAvailableInPlan: boolean;
  isCore: boolean;
  requiredModules: string[];
  configuration?: string;
  updatedAt?: Date;
}

export interface ModuleUsage {
  moduleName: string;
  displayName: string;
  totalClinics: number;
  clinicsWithModuleEnabled: number;
  adoptionRate: number;
  category: string;
}

export interface ModuleAdoption {
  moduleName: string;
  displayName: string;
  adoptionRate: number;
  enabledCount: number;
}

export interface ModuleUsageByPlan {
  planName: string;
  moduleName: string;
  clinicsCount: number;
  usagePercentage: number;
}

export interface ClinicModule {
  clinicId: string;
  clinicName: string;
  isEnabled: boolean;
  configuration?: string;
  updatedAt?: Date;
}

export interface ModuleConfigHistory {
  id: string;
  moduleName: string;
  action: 'Enabled' | 'Disabled' | 'ConfigUpdated';
  changedBy: string;
  changedAt: Date;
  reason?: string;
  previousConfiguration?: string;
  newConfiguration?: string;
}

export interface ValidationResponse {
  isValid: boolean;
  errorMessage?: string;
}
