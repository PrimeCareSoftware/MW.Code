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
          id: 'api-quick-guide',
          title: 'API_QUICK_GUIDE.md',
          category: 'Guias',
          path: 'API_QUICK_GUIDE.md',
          description: 'Guia rápido dos endpoints da API com exemplos',
          idealFor: 'Desenvolvedores de integração, testes'
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
      name: '📚 Índice e Referências',
      icon: '📚',
      docs: [
        {
          id: 'index',
          title: 'INDEX.md',
          category: 'Referência',
          path: 'docs/INDEX.md',
          description: 'Índice completo da documentação com jornadas de leitura',
          idealFor: 'Todos os usuários'
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
