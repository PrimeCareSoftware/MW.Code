import { 
  ClinicSummary, 
  ClinicDetail, 
  PaginatedClinics, 
  SystemAnalytics,
  SystemOwner 
} from '../models/system-admin.model';

export const MOCK_CLINICS: ClinicSummary[] = [
  {
    id: 'clinic1',
    name: 'Clínica Saúde Total',
    document: '12.345.678/0001-90',
    email: 'contato@clinicasaudetotal.com.br',
    phone: '+55 11 3456-7890',
    address: 'Rua das Clínicas, 100 - São Paulo, SP',
    isActive: true,
    tenantId: 'clinic1',
    createdAt: '2024-01-15T10:00:00Z',
    subscriptionStatus: 'Active',
    planName: 'Plano Professional',
    nextBillingDate: '2024-12-15'
  },
  {
    id: 'clinic2',
    name: 'Clínica Bem Estar',
    document: '98.765.432/0001-10',
    email: 'contato@clinicabemestar.com.br',
    phone: '+55 11 2345-6789',
    address: 'Av. Paulista, 2000 - São Paulo, SP',
    isActive: true,
    tenantId: 'clinic2',
    createdAt: '2024-02-20T14:00:00Z',
    subscriptionStatus: 'Trial',
    planName: 'Plano Basic',
    nextBillingDate: '2024-11-20'
  },
  {
    id: 'clinic3',
    name: 'Clínica Vida Plena',
    document: '11.222.333/0001-44',
    email: 'contato@clinicavidaplena.com.br',
    phone: '+55 11 4567-8901',
    address: 'Rua Augusta, 500 - São Paulo, SP',
    isActive: false,
    tenantId: 'clinic3',
    createdAt: '2024-03-10T09:00:00Z',
    subscriptionStatus: 'Suspended',
    planName: 'Plano Basic'
  }
];

export const MOCK_CLINIC_DETAIL: ClinicDetail = {
  ...MOCK_CLINICS[0],
  planPrice: 299.00,
  trialEndsAt: undefined,
  totalUsers: 8,
  activeUsers: 7
};

export const MOCK_PAGINATED_CLINICS: PaginatedClinics = {
  totalCount: 3,
  page: 1,
  pageSize: 20,
  totalPages: 1,
  clinics: MOCK_CLINICS
};

export const MOCK_SYSTEM_ANALYTICS: SystemAnalytics = {
  totalClinics: 3,
  activeClinics: 2,
  inactiveClinics: 1,
  totalUsers: 24,
  activeUsers: 20,
  totalPatients: 150,
  monthlyRecurringRevenue: 1098.00,
  subscriptionsByStatus: {
    'Active': 1,
    'Trial': 1,
    'Suspended': 1
  },
  subscriptionsByPlan: {
    'Plano Basic': 2,
    'Plano Professional': 1
  }
};

export const MOCK_SYSTEM_OWNERS: SystemOwner[] = [
  {
    id: '1',
    username: 'admin',
    email: 'admin@medicwarehouse.com',
    fullName: 'Administrador Sistema',
    phone: '+55 11 99999-9999',
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    lastLoginAt: '2024-11-08T08:00:00Z'
  },
  {
    id: '2',
    username: 'support',
    email: 'support@medicwarehouse.com',
    fullName: 'Suporte Técnico',
    phone: '+55 11 88888-8888',
    isActive: true,
    createdAt: '2024-01-15T00:00:00Z',
    lastLoginAt: '2024-11-07T16:30:00Z'
  }
];
