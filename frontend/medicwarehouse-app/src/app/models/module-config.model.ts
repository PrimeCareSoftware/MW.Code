/**
 * Module Configuration Models
 * Used for managing system modules configuration at clinic level
 */

export interface ModuleInfo {
  moduleName: string;
  displayName: string;
  description: string;
  category: string;
  icon: string;
  isCore: boolean;
  requiredModules: string[];
  availableInPlans: string[];
}

export interface ModuleConfig {
  moduleName: string;
  displayName: string;
  description: string;
  category: string;
  icon: string;
  isEnabled: boolean;
  isCore: boolean;
  isAvailableInPlan: boolean;
  requiredModules: string[];
  configuration?: string;
  updatedAt?: Date;
  requiresConfiguration?: boolean;
  configurationType?: string;
  configurationExample?: string;
  configurationHelp?: string;
}

export interface ValidationResponse {
  isValid: boolean;
  errorMessage?: string;
  missingRequirements?: string[];
  planUpgradeRequired?: boolean;
}

export interface ModuleHistoryEntry {
  id: string;
  moduleName: string;
  action: string;
  performedBy: string;
  performedAt: Date;
  details: string;
}

export interface ModuleEnableRequest {
  reason?: string;
}

export interface ModuleConfigUpdateRequest {
  configuration: string;
}
