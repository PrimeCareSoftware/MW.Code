export interface SubscriptionPlan {
  id: string;
  name: string;
  description: string;
  monthlyPrice: number;
  trialDays: number;
  maxUsers: number;
  maxPatients: number;
  hasReports: boolean;
  hasWhatsAppIntegration: boolean;
  hasSMSNotifications: boolean;
  hasTissExport: boolean;
  isActive: boolean;
  type: PlanType;
  features: string[];
  isRecommended?: boolean;
}

export enum PlanType {
  Trial = 0,
  Basic = 1,
  Standard = 2,
  Premium = 3,
  Enterprise = 4
}

export const AVAILABLE_PLANS: SubscriptionPlan[] = [
  {
    id: 'basic-plan',
    name: 'Básico',
    description: 'Perfeito para consultórios que estão começando',
    monthlyPrice: 190,
    trialDays: 15,
    maxUsers: 2,
    maxPatients: 100,
    hasReports: false,
    hasWhatsAppIntegration: false,
    hasSMSNotifications: false,
    hasTissExport: false,
    isActive: true,
    type: PlanType.Basic,
    features: [
      'Até 2 usuários',
      'Até 100 pacientes',
      'Agenda de consultas',
      'Cadastro de pacientes',
      'Prontuário médico digital',
      'Suporte por email'
    ]
  },
  {
    id: 'standard-plan',
    name: 'Médio',
    description: 'O mais indicado para consultórios em crescimento',
    monthlyPrice: 240,
    trialDays: 15,
    maxUsers: 3,
    maxPatients: 300,
    hasReports: true,
    hasWhatsAppIntegration: true,
    hasSMSNotifications: false,
    hasTissExport: false,
    isActive: true,
    type: PlanType.Standard,
    isRecommended: true,
    features: [
      'Até 3 usuários',
      'Até 300 pacientes',
      'Todas as funcionalidades do Básico',
      'Integração WhatsApp',
      'Relatórios gerenciais',
      'Lembretes de consulta',
      'Suporte prioritário'
    ]
  },
  {
    id: 'premium-plan',
    name: 'Premium',
    description: 'Para clínicas que precisam de recursos avançados',
    monthlyPrice: 320,
    trialDays: 15,
    maxUsers: 5,
    maxPatients: 1000,
    hasReports: true,
    hasWhatsAppIntegration: true,
    hasSMSNotifications: true,
    hasTissExport: true,
    isActive: true,
    type: PlanType.Premium,
    features: [
      'Até 5 usuários',
      'Pacientes ilimitados',
      'Todas as funcionalidades do Médio',
      'Notificações por SMS',
      'Exportação TISS',
      'Dashboard avançado',
      'API de integração',
      'Suporte 24/7'
    ]
  },
  {
    id: 'custom-plan',
    name: 'Personalizado',
    description: 'Solução customizada para suas necessidades',
    monthlyPrice: 0,
    trialDays: 15,
    maxUsers: 999,
    maxPatients: 999999,
    hasReports: true,
    hasWhatsAppIntegration: true,
    hasSMSNotifications: true,
    hasTissExport: true,
    isActive: true,
    type: PlanType.Enterprise,
    features: [
      'Usuários ilimitados',
      'Pacientes ilimitados',
      'Todos os recursos Premium',
      'Desenvolvimento de funcionalidades específicas',
      'Treinamento personalizado',
      'Gerente de conta dedicado',
      'SLA garantido'
    ]
  }
];
