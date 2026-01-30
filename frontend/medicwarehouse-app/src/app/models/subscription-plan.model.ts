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
  hasFinancialModule: boolean;
  isActive: boolean;
  type: PlanType;
  features: string[];
  isRecommended?: boolean;
  isMvp?: boolean;
  earlyAdopterPrice?: number;
  futurePrice?: number;
  savingsPercentage?: number;
  featuresInDevelopment?: string[];
  earlyAdopterBenefits?: string[];
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
    id: 'starter-mvp-plan',
    name: 'Starter',
    description: 'MVP Básico - Ideal para médicos autônomos',
    monthlyPrice: 49,
    earlyAdopterPrice: 49,
    futurePrice: 149,
    savingsPercentage: 67,
    trialDays: 14,
    maxUsers: 1,
    maxPatients: 50,
    hasReports: true,
    hasWhatsAppIntegration: false,
    hasSMSNotifications: false,
    hasTissExport: false,
    hasFinancialModule: false,
    isActive: true,
    isMvp: true,
    type: PlanType.Basic,
    features: [
      'Até 1 usuário',
      'Até 50 pacientes',
      'Agenda de consultas básica',
      'Cadastro de pacientes',
      'Prontuário médico digital simples',
      'Relatórios básicos',
      'Suporte por email (48h)'
    ],
    featuresInDevelopment: [
      'Integração WhatsApp Business',
      'Lembretes automáticos',
      'Backup automático diário'
    ],
    earlyAdopterBenefits: [
      'Preço fixo vitalício de R$ 49/mês',
      'R$ 100 em créditos de serviço',
      'Acesso beta a novos recursos',
      'Badge de Cliente Fundador'
    ]
  },
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
    hasFinancialModule: false,
    isActive: false,
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
    id: 'professional-mvp-plan',
    name: 'Professional',
    description: 'MVP Intermediário - Ideal para consultórios pequenos',
    monthlyPrice: 89,
    earlyAdopterPrice: 89,
    futurePrice: 239,
    savingsPercentage: 63,
    trialDays: 14,
    maxUsers: 2,
    maxPatients: 200,
    hasReports: true,
    hasWhatsAppIntegration: false,
    hasSMSNotifications: false,
    hasTissExport: false,
    hasFinancialModule: true,
    isActive: true,
    isMvp: true,
    type: PlanType.Standard,
    isRecommended: true,
    features: [
      'Até 2 usuários',
      'Até 200 pacientes',
      'Todos os recursos do Starter',
      'Agenda avançada (múltiplos profissionais)',
      'Prontuário médico completo',
      'Módulo Financeiro básico',
      'Relatórios gerenciais',
      'Portal do Paciente (básico)',
      'Suporte prioritário (24h)'
    ],
    featuresInDevelopment: [
      'Integração WhatsApp API',
      'Notificações por SMS',
      'Assinatura digital (ICP-Brasil)',
      'Exportação TISS',
      'Dashboard Analytics',
      'API de Integração'
    ],
    earlyAdopterBenefits: [
      'Preço fixo vitalício de R$ 89/mês',
      'R$ 100 em créditos de serviço',
      'Acesso beta a novos recursos',
      'Treinamento personalizado (2h)',
      'Badge de Cliente Fundador'
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
    hasFinancialModule: true,
    isActive: false,
    type: PlanType.Standard,
    isRecommended: false,
    features: [
      'Até 3 usuários',
      'Até 300 pacientes',
      'Todas as funcionalidades do Básico',
      'Integração WhatsApp',
      'Relatórios gerenciais',
      'Módulo Financeiro',
      'Lembretes de consulta',
      'Suporte prioritário'
    ]
  },
  {
    id: 'enterprise-mvp-plan',
    name: 'Enterprise',
    description: 'MVP Avançado - Ideal para clínicas estabelecidas',
    monthlyPrice: 149,
    earlyAdopterPrice: 149,
    futurePrice: 389,
    savingsPercentage: 62,
    trialDays: 14,
    maxUsers: 5,
    maxPatients: 999999,
    hasReports: true,
    hasWhatsAppIntegration: false,
    hasSMSNotifications: false,
    hasTissExport: false,
    hasFinancialModule: true,
    isActive: true,
    isMvp: true,
    type: PlanType.Premium,
    features: [
      'Até 5 usuários',
      'Pacientes ilimitados',
      'Todos os recursos do Professional',
      'Módulo Financeiro completo',
      'Gestão de estoque',
      'Fila de espera',
      'Telemedicina (básica)',
      'Portal do Paciente completo',
      'Relatórios avançados',
      'Conformidade LGPD',
      'Suporte 24/7'
    ],
    featuresInDevelopment: [
      'Assinatura digital (ICP-Brasil)',
      'Exportação TISS completa',
      'BI e Analytics avançado',
      'CRM para gestão de leads',
      'Automação de workflows',
      'Integração com laboratórios',
      'Agendamento online',
      'Marketing automation'
    ],
    earlyAdopterBenefits: [
      'Preço fixo vitalício de R$ 149/mês',
      'R$ 100 em créditos de serviço',
      'Acesso beta a novos recursos',
      'Treinamento personalizado (2h)',
      'Gerente de sucesso dedicado (3 meses)',
      'Badge de Cliente Fundador',
      'Voto no roadmap de desenvolvimento'
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
    hasFinancialModule: true,
    isActive: false,
    type: PlanType.Premium,
    features: [
      'Até 5 usuários',
      'Pacientes ilimitados',
      'Todas as funcionalidades do Médio',
      'Notificações por SMS',
      'Exportação TISS',
      'Módulo Financeiro Completo',
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
    hasFinancialModule: true,
    isActive: true,
    type: PlanType.Enterprise,
    features: [
      'Usuários ilimitados',
      'Pacientes ilimitados',
      'Todos os recursos Premium',
      'Módulo Financeiro Completo',
      'Desenvolvimento de funcionalidades específicas',
      'Treinamento personalizado',
      'Gerente de conta dedicado',
      'SLA garantido'
    ]
  }
];
