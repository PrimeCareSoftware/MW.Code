import { ModuleUsage, ModuleAdoption } from '../models/module-config.model';

export const MOCK_MODULE_USAGE: ModuleUsage[] = [
  {
    moduleName: 'PatientManagement',
    displayName: 'Gestão de Pacientes',
    totalClinics: 10,
    clinicsWithModuleEnabled: 10,
    adoptionRate: 100,
    category: 'Core'
  },
  {
    moduleName: 'AppointmentScheduling',
    displayName: 'Agendamento de Consultas',
    totalClinics: 10,
    clinicsWithModuleEnabled: 9,
    adoptionRate: 90,
    category: 'Core'
  },
  {
    moduleName: 'MedicalRecords',
    displayName: 'Prontuário Eletrônico',
    totalClinics: 10,
    clinicsWithModuleEnabled: 10,
    adoptionRate: 100,
    category: 'Core'
  },
  {
    moduleName: 'Prescriptions',
    displayName: 'Prescrições Médicas',
    totalClinics: 10,
    clinicsWithModuleEnabled: 8,
    adoptionRate: 80,
    category: 'Core'
  },
  {
    moduleName: 'FinancialManagement',
    displayName: 'Gestão Financeira',
    totalClinics: 10,
    clinicsWithModuleEnabled: 7,
    adoptionRate: 70,
    category: 'Core'
  },
  {
    moduleName: 'UserManagement',
    displayName: 'Gestão de Usuários',
    totalClinics: 10,
    clinicsWithModuleEnabled: 10,
    adoptionRate: 100,
    category: 'Core'
  },
  {
    moduleName: 'Reports',
    displayName: 'Relatórios Avançados',
    totalClinics: 10,
    clinicsWithModuleEnabled: 6,
    adoptionRate: 60,
    category: 'Analytics'
  },
  {
    moduleName: 'WhatsAppIntegration',
    displayName: 'Integração WhatsApp',
    totalClinics: 10,
    clinicsWithModuleEnabled: 4,
    adoptionRate: 40,
    category: 'Advanced'
  },
  {
    moduleName: 'SMSNotifications',
    displayName: 'Notificações SMS',
    totalClinics: 10,
    clinicsWithModuleEnabled: 3,
    adoptionRate: 30,
    category: 'Advanced'
  },
  {
    moduleName: 'TissExport',
    displayName: 'Exportação TISS',
    totalClinics: 10,
    clinicsWithModuleEnabled: 2,
    adoptionRate: 20,
    category: 'Premium'
  },
  {
    moduleName: 'InventoryManagement',
    displayName: 'Gestão de Estoque',
    totalClinics: 10,
    clinicsWithModuleEnabled: 5,
    adoptionRate: 50,
    category: 'Advanced'
  },
  {
    moduleName: 'WaitingQueue',
    displayName: 'Fila de Espera',
    totalClinics: 10,
    clinicsWithModuleEnabled: 6,
    adoptionRate: 60,
    category: 'Advanced'
  },
  {
    moduleName: 'DoctorFieldsConfig',
    displayName: 'Configuração de Campos',
    totalClinics: 10,
    clinicsWithModuleEnabled: 3,
    adoptionRate: 30,
    category: 'Premium'
  },
  {
    moduleName: 'Chat',
    displayName: 'Chat Interno',
    totalClinics: 10,
    clinicsWithModuleEnabled: 7,
    adoptionRate: 70,
    category: 'Core'
  }
];

export const MOCK_MODULE_ADOPTION: ModuleAdoption[] = [
  {
    moduleName: 'PatientManagement',
    displayName: 'Gestão de Pacientes',
    adoptionRate: 100,
    enabledCount: 10
  },
  {
    moduleName: 'AppointmentScheduling',
    displayName: 'Agendamento de Consultas',
    adoptionRate: 90,
    enabledCount: 9
  },
  {
    moduleName: 'MedicalRecords',
    displayName: 'Prontuário Eletrônico',
    adoptionRate: 100,
    enabledCount: 10
  },
  {
    moduleName: 'Prescriptions',
    displayName: 'Prescrições Médicas',
    adoptionRate: 80,
    enabledCount: 8
  },
  {
    moduleName: 'FinancialManagement',
    displayName: 'Gestão Financeira',
    adoptionRate: 70,
    enabledCount: 7
  },
  {
    moduleName: 'UserManagement',
    displayName: 'Gestão de Usuários',
    adoptionRate: 100,
    enabledCount: 10
  },
  {
    moduleName: 'Reports',
    displayName: 'Relatórios Avançados',
    adoptionRate: 60,
    enabledCount: 6
  },
  {
    moduleName: 'WhatsAppIntegration',
    displayName: 'Integração WhatsApp',
    adoptionRate: 40,
    enabledCount: 4
  },
  {
    moduleName: 'SMSNotifications',
    displayName: 'Notificações SMS',
    adoptionRate: 30,
    enabledCount: 3
  },
  {
    moduleName: 'TissExport',
    displayName: 'Exportação TISS',
    adoptionRate: 20,
    enabledCount: 2
  },
  {
    moduleName: 'InventoryManagement',
    displayName: 'Gestão de Estoque',
    adoptionRate: 50,
    enabledCount: 5
  },
  {
    moduleName: 'WaitingQueue',
    displayName: 'Fila de Espera',
    adoptionRate: 60,
    enabledCount: 6
  },
  {
    moduleName: 'DoctorFieldsConfig',
    displayName: 'Configuração de Campos',
    adoptionRate: 30,
    enabledCount: 3
  },
  {
    moduleName: 'Chat',
    displayName: 'Chat Interno',
    adoptionRate: 70,
    enabledCount: 7
  }
];
