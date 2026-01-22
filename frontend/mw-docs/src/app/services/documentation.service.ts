import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DocItem, DocCategory } from '../models/doc-item.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentationService {
  private readonly docsBasePath = 'assets/docs/';

  private readonly documentStructure: DocCategory[] = [
    {
      name: '📱 Interface e Experiência do Usuário',
      icon: '📱',
      docs: [
        {
          id: 'screens-documentation',
          title: 'SCREENS_DOCUMENTATION.md',
          category: 'Interface',
          path: 'SCREENS_DOCUMENTATION.md',
          description: 'Documentação completa de todas as 8 telas do sistema com mockups ASCII e diagramas de fluxo',
          size: '40KB / 813 linhas',
          idealFor: 'Desenvolvedores frontend, designers, analistas de UX'
        },
        {
          id: 'visual-flow-summary',
          title: 'VISUAL_FLOW_SUMMARY.md',
          category: 'Interface',
          path: 'docs/VISUAL_FLOW_SUMMARY.md',
          description: 'Resumo visual rápido com diagramas Mermaid interativos dos principais fluxos',
          size: '12KB / 387 linhas',
          idealFor: 'Quick reference, reuniões, apresentações'
        }
      ]
    },
    {
      name: '📋 Regras de Negócio e Requisitos',
      icon: '📋',
      docs: [
        {
          id: 'business-rules',
          title: 'BUSINESS_RULES.md',
          category: 'Negócio',
          path: 'BUSINESS_RULES.md',
          description: 'Regras de negócio detalhadas do sistema incluindo multi-tenancy, vínculos, privacidade',
          idealFor: 'Analistas de negócio, product owners, stakeholders'
        }
      ]
    },
    {
      name: '🔧 Implementação Técnica',
      icon: '🔧',
      docs: [
        {
          id: 'technical-implementation',
          title: 'TECHNICAL_IMPLEMENTATION.md',
          category: 'Técnica',
          path: 'TECHNICAL_IMPLEMENTATION.md',
          description: 'Arquitetura técnica, fluxos de dados, configurações do EF Core, segurança',
          idealFor: 'Desenvolvedores backend, arquitetos, DevOps'
        },
        {
          id: 'implementation',
          title: 'IMPLEMENTATION.md',
          category: 'Técnica',
          path: 'IMPLEMENTATION.md',
          description: 'Implementação original do sistema de atendimento ao paciente',
          idealFor: 'Desenvolvedores, referência histórica'
        }
      ]
    },
    {
      name: '🚀 Guias de Uso',
      icon: '🚀',
      docs: [
        {
          id: 'readme',
          title: 'README.md',
          category: 'Guias',
          path: 'README.md',
          description: 'Visão geral do projeto, funcionalidades, como executar, tecnologias',
          idealFor: 'Todos os usuários, primeira leitura'
        },
        {
          id: 'guia-execucao',
          title: 'GUIA_EXECUCAO.md',
          category: 'Guias',
          path: 'GUIA_EXECUCAO.md',
          description: 'Guia passo a passo para executar o projeto localmente',
          idealFor: 'Desenvolvedores, configuração inicial'
        },
        {
          id: 'system-setup-guide',
          title: 'SYSTEM_SETUP_GUIDE.md',
          category: 'Guias',
          path: 'SYSTEM_SETUP_GUIDE.md',
          description: 'Guia completo de configuração e setup do sistema passo a passo',
          size: '18KB / 556 linhas',
          idealFor: 'Desenvolvedores, administradores, configuração inicial'
        },
        {
          id: 'api-quick-guide',
          title: 'API_QUICK_GUIDE.md',
          category: 'Guias',
          path: 'API_QUICK_GUIDE.md',
          description: 'Guia rápido dos endpoints da API com exemplos',
          idealFor: 'Desenvolvedores de integração, testes'
        },
        {
          id: 'authentication-guide',
          title: 'AUTHENTICATION_GUIDE.md',
          category: 'Guias',
          path: 'AUTHENTICATION_GUIDE.md',
          description: 'Guia completo de autenticação JWT, endpoints de login e validação de tokens',
          size: '8.7KB / 392 linhas',
          idealFor: 'Desenvolvedores, integração de autenticação'
        }
      ]
    },
    {
      name: '🔄 CI/CD e Qualidade',
      icon: '🔄',
      docs: [
        {
          id: 'ci-cd-documentation',
          title: 'CI_CD_DOCUMENTATION.md',
          category: 'CI/CD',
          path: 'CI_CD_DOCUMENTATION.md',
          description: 'Documentação do pipeline de CI/CD com GitHub Actions',
          idealFor: 'DevOps, engenheiros de CI/CD'
        },
        {
          id: 'test-summary',
          title: 'TEST_SUMMARY.md',
          category: 'CI/CD',
          path: 'TEST_SUMMARY.md',
          description: 'Resumo dos testes unitários e cobertura de código',
          idealFor: 'QA, desenvolvedores'
        },
        {
          id: 'security-validations',
          title: 'SECURITY_VALIDATIONS.md',
          category: 'CI/CD',
          path: 'SECURITY_VALIDATIONS.md',
          description: 'Validações de segurança implementadas no sistema',
          idealFor: 'Security engineers, auditores'
        },
        {
          id: 'sonarcloud-setup',
          title: 'SONARCLOUD_SETUP.md',
          category: 'CI/CD',
          path: 'SONARCLOUD_SETUP.md',
          description: 'Configuração do SonarCloud para análise de código',
          idealFor: 'DevOps, qualidade de código'
        }
      ]
    },
    {
      name: '📝 Resumos de Implementação',
      icon: '📝',
      docs: [
        {
          id: 'implementation-summary',
          title: 'IMPLEMENTATION_SUMMARY.md',
          category: 'Implementação',
          path: 'IMPLEMENTATION_SUMMARY.md',
          description: 'Resumo geral das implementações',
          idealFor: 'Visão geral rápida'
        },
        {
          id: 'implementation-new-features',
          title: 'IMPLEMENTATION_NEW_FEATURES.md',
          category: 'Implementação',
          path: 'IMPLEMENTATION_NEW_FEATURES.md',
          description: 'Novas funcionalidades implementadas',
          idealFor: 'Product managers, changelog'
        },
        {
          id: 'implementation-summary-business-rules',
          title: 'IMPLEMENTATION_SUMMARY_BUSINESS_RULES.md',
          category: 'Implementação',
          path: 'IMPLEMENTATION_SUMMARY_BUSINESS_RULES.md',
          description: 'Resumo da implementação das regras de negócio',
          idealFor: 'Analistas de negócio'
        },
        {
          id: 'migration-implementation-summary',
          title: 'MIGRATION_IMPLEMENTATION_SUMMARY.md',
          category: 'Implementação',
          path: 'MIGRATION_IMPLEMENTATION_SUMMARY.md',
          description: 'Resumo das migrações de banco de dados',
          idealFor: 'DBAs, DevOps'
        }
      ]
    },
    {
      name: '🔐 Segurança',
      icon: '🔐',
      docs: [
        {
          id: 'security-guide',
          title: 'SECURITY_GUIDE.md',
          category: 'Segurança',
          path: 'SECURITY_GUIDE.md',
          description: 'Guia completo de segurança do sistema',
          idealFor: 'Security engineers, desenvolvedores'
        },
        {
          id: 'security-implementation-summary',
          title: 'SECURITY_IMPLEMENTATION_SUMMARY.md',
          category: 'Segurança',
          path: 'SECURITY_IMPLEMENTATION_SUMMARY.md',
          description: 'Resumo da implementação de segurança',
          idealFor: 'Security engineers, auditores'
        }
      ]
    },
    {
      name: '💰 Sistema de Pagamentos',
      icon: '💰',
      docs: [
        {
          id: 'implementation-payment-system',
          title: 'IMPLEMENTATION_PAYMENT_SYSTEM.md',
          category: 'Pagamentos',
          path: 'IMPLEMENTATION_PAYMENT_SYSTEM.md',
          description: 'Implementação do sistema de pagamentos',
          idealFor: 'Desenvolvedores, arquitetos'
        },
        {
          id: 'payment-flow',
          title: 'PAYMENT_FLOW.md',
          category: 'Pagamentos',
          path: 'PAYMENT_FLOW.md',
          description: 'Fluxo de pagamentos do sistema',
          idealFor: 'Analistas, desenvolvedores'
        }
      ]
    },
    {
      name: '📊 Gestão Financeira',
      icon: '📊',
      docs: [
        {
          id: 'financial-reports-documentation',
          title: 'FINANCIAL_REPORTS_DOCUMENTATION.md',
          category: 'Financeiro',
          path: 'FINANCIAL_REPORTS_DOCUMENTATION.md',
          description: 'Sistema completo de gestão financeira, relatórios e controle de despesas',
          size: '11KB / 497 linhas',
          idealFor: 'Gestores financeiros, contadores, administradores'
        }
      ]
    },
    {
      name: '💳 Sistema de Assinaturas',
      icon: '💳',
      docs: [
        {
          id: 'subscription-system',
          title: 'SUBSCRIPTION_SYSTEM.md',
          category: 'Assinaturas',
          path: 'SUBSCRIPTION_SYSTEM.md',
          description: 'Sistema SaaS de assinaturas com planos, permissões e controle de acesso',
          size: '14KB / 612 linhas',
          idealFor: 'Product managers, desenvolvedores, administradores'
        }
      ]
    },
    {
      name: '🤖 WhatsApp AI Agent',
      icon: '🤖',
      docs: [
        {
          id: 'whatsapp-ai-agent-documentation',
          title: 'WHATSAPP_AI_AGENT_DOCUMENTATION.md',
          category: 'WhatsApp AI',
          path: 'WHATSAPP_AI_AGENT_DOCUMENTATION.md',
          description: 'Documentação completa do agente de IA para agendamento via WhatsApp',
          size: '15KB / 448 linhas',
          idealFor: 'Desenvolvedores, product managers'
        },
        {
          id: 'implementation-whatsapp-ai-agent',
          title: 'IMPLEMENTATION_WHATSAPP_AI_AGENT.md',
          category: 'WhatsApp AI',
          path: 'IMPLEMENTATION_WHATSAPP_AI_AGENT.md',
          description: 'Resumo da implementação do WhatsApp AI Agent (Fase 1 completa)',
          size: '12KB / 408 linhas',
          idealFor: 'Desenvolvedores, arquitetos'
        },
        {
          id: 'whatsapp-ai-agent-security',
          title: 'WHATSAPP_AI_AGENT_SECURITY.md',
          category: 'WhatsApp AI',
          path: 'WHATSAPP_AI_AGENT_SECURITY.md',
          description: 'Guia de segurança do WhatsApp AI Agent com proteção contra prompt injection',
          size: '12KB / 436 linhas',
          idealFor: 'Security engineers, desenvolvedores'
        }
      ]
    },
    {
      name: '🔔 Notificações',
      icon: '🔔',
      docs: [
        {
          id: 'notification-routines-documentation',
          title: 'NOTIFICATION_ROUTINES_DOCUMENTATION.md',
          category: 'Notificações',
          path: 'NOTIFICATION_ROUTINES_DOCUMENTATION.md',
          description: 'Documentação completa das rotinas de notificação',
          idealFor: 'Desenvolvedores, analistas'
        },
        {
          id: 'implementation-notification-routines',
          title: 'IMPLEMENTATION_NOTIFICATION_ROUTINES.md',
          category: 'Notificações',
          path: 'IMPLEMENTATION_NOTIFICATION_ROUTINES.md',
          description: 'Implementação das rotinas de notificação',
          idealFor: 'Desenvolvedores'
        },
        {
          id: 'notification-routines-example',
          title: 'NOTIFICATION_ROUTINES_EXAMPLE.md',
          category: 'Notificações',
          path: 'NOTIFICATION_ROUTINES_EXAMPLE.md',
          description: 'Exemplos de uso das rotinas de notificação',
          idealFor: 'Desenvolvedores'
        }
      ]
    },
    {
      name: '👨‍👩‍👧 Recursos Especiais',
      icon: '👨‍👩‍👧',
      docs: [
        {
          id: 'implementation-guardian-child',
          title: 'IMPLEMENTATION_GUARDIAN_CHILD.md',
          category: 'Recursos',
          path: 'IMPLEMENTATION_GUARDIAN_CHILD.md',
          description: 'Implementação do sistema de responsável/dependente',
          idealFor: 'Desenvolvedores, analistas de negócio'
        }
      ]
    },
    {
      name: '🌐 MW.Site - Marketing',
      icon: '🌐',
      docs: [
        {
          id: 'mw-site-documentation',
          title: 'MW_SITE_DOCUMENTATION.md',
          category: 'Marketing',
          path: 'MW_SITE_DOCUMENTATION.md',
          description: 'Documentação completa do site de marketing',
          idealFor: 'Desenvolvedores frontend, marketing'
        },
        {
          id: 'mw-site-implementation-summary',
          title: 'MW_SITE_IMPLEMENTATION_SUMMARY.md',
          category: 'Marketing',
          path: 'MW_SITE_IMPLEMENTATION_SUMMARY.md',
          description: 'Resumo da implementação do site de marketing',
          idealFor: 'Product managers, desenvolvedores'
        }
      ]
    },
    {
      name: '🧪 Guias de Configuração e Testes',
      icon: '🧪',
      docs: [
        {
          id: 'testes-config-index',
          title: 'Índice Geral de Testes',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/README.md',
          description: 'Guia completo de configuração e testes do PrimeCare Software - Centro de testes com 200+ cenários',
          size: '11KB / 315 linhas',
          idealFor: 'Testadores, QA, desenvolvedores, administradores'
        },
        {
          id: 'cadastro-paciente-test',
          title: '01 - Cadastro de Paciente',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/01-CADASTRO-PACIENTE.md',
          description: 'Guia completo para testar cadastro de pacientes: validações, convênios, busca e filtros - 25+ cenários',
          size: '14KB / 450 linhas',
          idealFor: 'Testadores, QA, secretárias, administradores'
        },
        {
          id: 'atendimento-consulta-test',
          title: '02 - Atendimento e Consulta',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/02-ATENDIMENTO-CONSULTA.md',
          description: 'Testes completos de agendamento, sala de espera, atendimento, prontuário e prescrições - 30+ cenários',
          size: '16KB / 520 linhas',
          idealFor: 'Testadores, QA, médicos, secretárias'
        },
        {
          id: 'modulo-financeiro-test',
          title: '03 - Módulo Financeiro',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/03-MODULO-FINANCEIRO.md',
          description: 'Testes de contas a receber/pagar, fluxo de caixa, fornecedores e relatórios - 25+ cenários',
          size: '18KB / 580 linhas',
          idealFor: 'Testadores, QA, gestores financeiros, contadores'
        },
        {
          id: 'tiss-padrao-test',
          title: '04 - TISS Padrão ANS',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/04-TISS-PADRAO.md',
          description: 'Configuração e testes TISS: geração de guias, lotes, processamento e glosas - 20+ cenários',
          size: '17KB / 540 linhas',
          idealFor: 'Testadores, QA, faturistas, administradores'
        },
        {
          id: 'tuss-tabela-test',
          title: '05 - TUSS Tabela de Procedimentos',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/05-TUSS-TABELA.md',
          description: 'Importação e gestão da tabela TUSS: busca, precificação e integração - 18+ cenários',
          size: '15KB / 480 linhas',
          idealFor: 'Testadores, QA, gestores, administradores'
        },
        {
          id: 'telemedicina-test',
          title: '06 - Telemedicina',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/06-TELEMEDICINA.md',
          description: 'Testes completos de telemedicina: videoconsulta, gravação, prescrição digital - 22+ cenários',
          size: '17KB / 550 linhas',
          idealFor: 'Testadores, QA, médicos, administradores'
        },
        {
          id: 'cenarios-completos-test',
          title: '07 - Cenários Completos',
          category: 'Testes e Configuração',
          path: 'docs/testes-configuracao/07-CENARIOS-COMPLETOS.md',
          description: 'Consolidação de TODOS os cenários de teste: fluxos completos, integrações, segurança - 200+ cenários',
          size: '14KB / 450 linhas',
          idealFor: 'Testadores, QA, gerentes de projeto, arquitetos'
        }
      ]
    },
    {
      name: '📚 Índice e Referências',
      icon: '📚',
      docs: [
        {
          id: 'documentation-index',
          title: 'DOCUMENTATION_INDEX.md',
          category: 'Referência',
          path: 'DOCUMENTATION_INDEX.md',
          description: 'Índice completo de navegação com 31+ documentos organizados por categoria e fluxos de leitura recomendados',
          size: '11KB / 290 linhas',
          idealFor: 'Todos os usuários, navegação da documentação'
        },
        {
          id: 'index',
          title: 'INDEX.md',
          category: 'Referência',
          path: 'docs/INDEX.md',
          description: 'Índice alternativo da documentação com jornadas de leitura',
          idealFor: 'Todos os usuários'
        },
        {
          id: 'glossario-termos-empresariais',
          title: 'GLOSSARIO_TERMOS_EMPRESARIAIS.md',
          category: 'Referência',
          path: 'GLOSSARIO_TERMOS_EMPRESARIAIS.md',
          description: 'Glossário completo de termos empresariais: SaaS, MRR, Churn, CAC, LTV, ROI e muito mais',
          size: '19KB / 822 linhas',
          idealFor: 'Empreendedores, donos de negócio, estudantes, todos os usuários'
        }
      ]
    }
  ];

  constructor(private http: HttpClient) {}

  getCategories(): DocCategory[] {
    return this.documentStructure;
  }

  getAllDocs(): DocItem[] {
    return this.documentStructure.flatMap(category => category.docs);
  }

  getDocContent(path: string): Observable<string> {
    return this.http.get(`${this.docsBasePath}${path}`, { responseType: 'text' });
  }

  searchDocs(query: string): DocItem[] {
    const lowerQuery = query.toLowerCase();
    return this.getAllDocs().filter(doc =>
      doc.title.toLowerCase().includes(lowerQuery) ||
      doc.description.toLowerCase().includes(lowerQuery) ||
      doc.category.toLowerCase().includes(lowerQuery)
    );
  }
}
