/**
 * MVP Features Configuration
 * 
 * This file controls which features are enabled during MVP phase.
 * Set mvpMode to false when transitioning to production.
 */

export interface FeatureConfig {
  enabled: boolean;
  inDevelopment: boolean;
  availableFrom?: Date;
}

export interface MvpConfig {
  mode: 'mvp' | 'production';
  earlyAdopterProgramActive: boolean;
  maxEarlyAdoptersPerPlan: number;
  features: {
    // Core Features (Always Available)
    patientManagement: FeatureConfig;
    appointmentScheduling: FeatureConfig;
    digitalMedicalRecords: FeatureConfig;
    basicReports: FeatureConfig;
    lgpdCompliance: FeatureConfig;
    
    // MVP Phase 1 (Current)
    patientPortalBasic: FeatureConfig;
    financialModuleBasic: FeatureConfig;
    queueManagement: FeatureConfig;
    inventoryManagement: FeatureConfig;
    telemedicineBasic: FeatureConfig;
    
    // MVP Phase 2 (In Development)
    whatsappBusinessIntegration: FeatureConfig;
    automaticReminders: FeatureConfig;
    automaticBackup: FeatureConfig;
    
    // MVP Phase 3 (Planned - 5-7 months)
    whatsappApi: FeatureConfig;
    smsNotifications: FeatureConfig;
    digitalSignatureICPBrasil: FeatureConfig;
    tissExport: FeatureConfig;
    analyticsBasic: FeatureConfig;
    customizableReports: FeatureConfig;
    
    // MVP Phase 4 (Planned - 8-10 months)
    digitalSignatureComplete: FeatureConfig;
    tissExportComplete: FeatureConfig;
    crmIntegrated: FeatureConfig;
    marketingAutomation: FeatureConfig;
    publicApi: FeatureConfig;
    
    // MVP Phase 5 (Planned - 11-12 months)
    advancedBiAnalytics: FeatureConfig;
    machineLearning: FeatureConfig;
    workflowAutomation: FeatureConfig;
    laboratoryIntegration: FeatureConfig;
    onlineScheduling: FeatureConfig;
  };
}

export const MVP_CONFIG: MvpConfig = {
  mode: 'mvp',
  earlyAdopterProgramActive: true,
  maxEarlyAdoptersPerPlan: 100,
  
  features: {
    // Core Features (Always Available)
    patientManagement: {
      enabled: true,
      inDevelopment: false
    },
    appointmentScheduling: {
      enabled: true,
      inDevelopment: false
    },
    digitalMedicalRecords: {
      enabled: true,
      inDevelopment: false
    },
    basicReports: {
      enabled: true,
      inDevelopment: false
    },
    lgpdCompliance: {
      enabled: true,
      inDevelopment: false
    },
    
    // MVP Phase 1 (Current)
    patientPortalBasic: {
      enabled: true,
      inDevelopment: false
    },
    financialModuleBasic: {
      enabled: true,
      inDevelopment: false
    },
    queueManagement: {
      enabled: true,
      inDevelopment: false
    },
    inventoryManagement: {
      enabled: true,
      inDevelopment: false
    },
    telemedicineBasic: {
      enabled: true,
      inDevelopment: false
    },
    
    // MVP Phase 2 (In Development)
    whatsappBusinessIntegration: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-04-01')
    },
    automaticReminders: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-04-01')
    },
    automaticBackup: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-03-15')
    },
    
    // MVP Phase 3 (Planned - 5-7 months)
    whatsappApi: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-06-01')
    },
    smsNotifications: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-06-01')
    },
    digitalSignatureICPBrasil: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-07-01')
    },
    tissExport: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-07-01')
    },
    analyticsBasic: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-06-15')
    },
    customizableReports: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-06-15')
    },
    
    // MVP Phase 4 (Planned - 8-10 months)
    digitalSignatureComplete: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-09-01')
    },
    tissExportComplete: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-09-01')
    },
    crmIntegrated: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-08-01')
    },
    marketingAutomation: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-10-01')
    },
    publicApi: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-10-01')
    },
    
    // MVP Phase 5 (Planned - 11-12 months)
    advancedBiAnalytics: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-11-01')
    },
    machineLearning: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-11-15')
    },
    workflowAutomation: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-11-01')
    },
    laboratoryIntegration: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-12-01')
    },
    onlineScheduling: {
      enabled: false,
      inDevelopment: true,
      availableFrom: new Date('2026-12-01')
    }
  }
};

/**
 * Check if a feature is enabled
 */
export function isFeatureEnabled(feature: keyof MvpConfig['features']): boolean {
  return MVP_CONFIG.features[feature].enabled;
}

/**
 * Check if a feature is in development
 */
export function isFeatureInDevelopment(feature: keyof MvpConfig['features']): boolean {
  return MVP_CONFIG.features[feature].inDevelopment;
}

/**
 * Get the expected availability date for a feature
 */
export function getFeatureAvailabilityDate(feature: keyof MvpConfig['features']): Date | undefined {
  return MVP_CONFIG.features[feature].availableFrom;
}

/**
 * Check if we are in MVP mode
 */
export function isMvpMode(): boolean {
  return MVP_CONFIG.mode === 'mvp';
}

/**
 * Check if early adopter program is active
 */
export function isEarlyAdopterProgramActive(): boolean {
  return MVP_CONFIG.earlyAdopterProgramActive;
}

/**
 * Get maximum number of early adopters per plan
 */
export function getMaxEarlyAdoptersPerPlan(): number {
  return MVP_CONFIG.maxEarlyAdoptersPerPlan;
}
