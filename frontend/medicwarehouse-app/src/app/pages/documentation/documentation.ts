import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { environment } from '../../../environments/environment';

interface DocCategory {
  name: string;
  icon: string;
  docs: DocItem[];
}

interface DocItem {
  id: string;
  title: string;
  description: string;
  path: string;
  icon: string;
}

@Component({
  selector: 'app-documentation',
  standalone: true,
  imports: [CommonModule, RouterModule, Navbar],
  templateUrl: './documentation.html',
  styleUrl: './documentation.scss'
})
export class Documentation implements OnInit {
  categories = signal<DocCategory[]>([]);
  searchQuery = signal('');
  filteredCategories = signal<DocCategory[]>([]);
  private readonly repositoryUrl = environment.documentation.repositoryUrl;

  ngOnInit(): void {
    this.loadDocumentation();
  }

  loadDocumentation(): void {
    const docs: DocCategory[] = [
      {
        name: 'Documentação Geral',
        icon: '📚',
        docs: [
          {
            id: 'readme',
            title: 'README Principal',
            description: 'Visão geral do MedicWarehouse',
            path: '/README.md',
            icon: '📖'
          },
          {
            id: 'changelog',
            title: 'Changelog',
            description: 'Histórico de mudanças e atualizações',
            path: '/CHANGELOG.md',
            icon: '📝'
          }
        ]
      },
      {
        name: 'Implementações',
        icon: '⚙️',
        docs: [
          {
            id: 'implementacoes',
            title: 'Documentação de Implementações',
            description: '59+ documentos sobre implementações do sistema',
            path: '/system-admin/implementacoes/',
            icon: '📝'
          },
          {
            id: 'implementacoes-indice',
            title: 'Índice de Implementações',
            description: 'Lista completa de todas as implementações ativas',
            path: '/system-admin/implementacoes/INDEX.md',
            icon: '📋'
          }
        ]
      },
      {
        name: 'Guias do Usuário',
        icon: '📖',
        docs: [
          {
            id: 'guias',
            title: 'Guias de Usuário e Desenvolvedor',
            description: '40+ guias práticos para uso e desenvolvimento',
            path: '/system-admin/guias/',
            icon: '🎓'
          },
          {
            id: 'onboarding',
            title: 'Guia de Onboarding',
            description: 'Guia inicial para novos usuários',
            path: '/ONBOARDING_GUIDE.md',
            icon: '🚀'
          }
        ]
      },
      {
        name: 'Portal do Paciente',
        icon: '👤',
        docs: [
          {
            id: 'patient-portal',
            title: 'Portal do Paciente',
            description: 'Documentação do portal de autoatendimento',
            path: '/PATIENT_PORTAL_GUIDE.md',
            icon: '🏥'
          },
          {
            id: 'patient-portal-impl',
            title: 'Implementação do Portal',
            description: 'Relatório de implementação do portal do paciente',
            path: '/RELATORIO_IMPLEMENTACAO_PORTAL_PACIENTE.md',
            icon: '📊'
          }
        ]
      },
      {
        name: 'Telemedicina',
        icon: '💻',
        docs: [
          {
            id: 'telemedicine',
            title: 'Sistema de Telemedicina',
            description: 'Documentação do módulo de telemedicina',
            path: '/telemedicine/README.md',
            icon: '🎥'
          },
          {
            id: 'telemedicine-impl',
            title: 'Implementação Telemedicina',
            description: 'Resumo de implementação do sistema de telemedicina',
            path: '/system-admin/implementacoes/TELEMEDICINA_IMPLEMENTATION_SUMMARY.md',
            icon: '⚕️'
          }
        ]
      },
      {
        name: 'Funcionalidades Clínicas',
        icon: '🏥',
        docs: [
          {
            id: 'anamnesis',
            title: 'Anamnese',
            description: 'Sistema de anamnese e templates',
            path: '/system-admin/implementacoes/ANAMNESIS_IMPLEMENTATION_COMPLETE.md',
            icon: '📋'
          },
          {
            id: 'soap',
            title: 'Prontuário SOAP',
            description: 'Sistema de prontuário eletrônico SOAP',
            path: '/system-admin/implementacoes/SOAP_IMPLEMENTATION_SUMMARY.md',
            icon: '📄'
          },
          {
            id: 'prescriptions',
            title: 'Prescrições Digitais',
            description: 'Sistema de prescrições digitais',
            path: '/system-admin/implementacoes/DIGITAL_PRESCRIPTION_FINALIZATION_COMPLETE.md',
            icon: '💊'
          },
          {
            id: 'sngpc',
            title: 'SNGPC',
            description: 'Sistema Nacional de Gerenciamento de Produtos Controlados',
            path: '/system-admin/implementacoes/SNGPC_IMPLEMENTATION_STATUS_2026.md',
            icon: '🔐'
          }
        ]
      },
      {
        name: 'Gestão e CRM',
        icon: '💼',
        docs: [
          {
            id: 'crm',
            title: 'Sistema CRM',
            description: 'Gestão de relacionamento com clientes',
            path: '/CRM_IMPLEMENTATION_SUMMARY.md',
            icon: '📊'
          },
          {
            id: 'fila-espera',
            title: 'Fila de Espera',
            description: 'Sistema de gestão de fila de espera',
            path: '/RELATORIO_IMPLEMENTACAO_FILA_ESPERA.md',
            icon: '⏱️'
          },
          {
            id: 'campaigns',
            title: 'Campanhas',
            description: 'Sistema de gestão de campanhas e marketing',
            path: '/CAMPAIGN_MANAGEMENT_IMPLEMENTATION_SUMMARY.md',
            icon: '📢'
          }
        ]
      },
      {
        name: 'Financeiro e Fiscal',
        icon: '💰',
        docs: [
          {
            id: 'payment',
            title: 'Sistema de Pagamentos',
            description: 'Guia do sistema de pagamentos',
            path: '/PAYMENT_SYSTEM_GUIDE.md',
            icon: '💳'
          },
          {
            id: 'gestao-fiscal',
            title: 'Gestão Fiscal',
            description: 'Sistema de gestão fiscal e notas fiscais',
            path: '/GESTAO_FISCAL_FASE4_COMPLETA.md',
            icon: '📑'
          },
          {
            id: 'tiss',
            title: 'TISS/TUSS',
            description: 'Padrão de Troca de Informações na Saúde Suplementar',
            path: '/system-admin/implementacoes/TISS_IMPLEMENTATION_STATUS.md',
            icon: '🏢'
          }
        ]
      },
      {
        name: 'Analytics e BI',
        icon: '📊',
        docs: [
          {
            id: 'analytics',
            title: 'Analytics e BI',
            description: 'Sistema de analytics e business intelligence',
            path: '/RELATORIO_FINAL_BI_ANALYTICS.md',
            icon: '📈'
          },
          {
            id: 'dashboards',
            title: 'Dashboards',
            description: 'Guia de criação de dashboards personalizados',
            path: '/DASHBOARD_CREATION_GUIDE.md',
            icon: '📉'
          }
        ]
      },
      {
        name: 'Segurança e Compliance',
        icon: '🔐',
        docs: [
          {
            id: 'lgpd',
            title: 'Conformidade LGPD',
            description: 'Guia de conformidade com LGPD',
            path: '/LGPD_COMPLIANCE_GUIDE.md',
            icon: '🛡️'
          },
          {
            id: '2fa',
            title: 'Autenticação 2FA',
            description: 'Sistema de autenticação de dois fatores',
            path: '/GUIA_USUARIO_2FA.md',
            icon: '🔑'
          },
          {
            id: 'security',
            title: 'Melhores Práticas de Segurança',
            description: 'Guia de segurança e melhores práticas',
            path: '/SECURITY_BEST_PRACTICES_GUIDE.md',
            icon: '🔒'
          },
          {
            id: 'cfm',
            title: 'Conformidade CFM',
            description: 'Documentação sobre conformidade com regulamentações do CFM',
            path: '/system-admin/cfm-compliance/',
            icon: '⚕️'
          }
        ]
      },
      {
        name: 'Acessibilidade',
        icon: '♿',
        docs: [
          {
            id: 'accessibility-guide',
            title: 'Guia de Acessibilidade',
            description: 'Guia de implementação de acessibilidade',
            path: '/ACCESSIBILITY_GUIDE.md',
            icon: '📘'
          },
          {
            id: 'accessibility-testing',
            title: 'Testes de Acessibilidade',
            description: 'Guia de testes de acessibilidade',
            path: '/ACCESSIBILITY_TESTING_GUIDE.md',
            icon: '🧪'
          },
          {
            id: 'wcag',
            title: 'Conformidade WCAG',
            description: 'Declaração de conformidade WCAG',
            path: '/WCAG_COMPLIANCE_STATEMENT.md',
            icon: '✅'
          }
        ]
      },
      {
        name: 'Assinatura Digital',
        icon: '✍️',
        docs: [
          {
            id: 'digital-signature',
            title: 'Assinatura Digital',
            description: 'Documentação técnica de assinatura digital',
            path: '/ASSINATURA_DIGITAL_DOCUMENTACAO_TECNICA.md',
            icon: '📝'
          },
          {
            id: 'digital-signature-guide',
            title: 'Guia do Usuário',
            description: 'Guia do usuário para assinatura digital',
            path: '/ASSINATURA_DIGITAL_GUIA_USUARIO.md',
            icon: '📖'
          },
          {
            id: 'digital-signature-integration',
            title: 'Guia de Integração',
            description: 'Guia de integração de assinatura digital',
            path: '/GUIA_INTEGRACAO_ASSINATURA_DIGITAL.md',
            icon: '🔗'
          }
        ]
      }
    ];

    this.categories.set(docs);
    this.filteredCategories.set(docs);
  }

  onSearch(event: Event): void {
    const query = (event.target as HTMLInputElement).value.toLowerCase();
    this.searchQuery.set(query);

    if (!query) {
      this.filteredCategories.set(this.categories());
      return;
    }

    const filtered = this.categories()
      .map(category => ({
        ...category,
        docs: category.docs.filter(doc =>
          doc.title.toLowerCase().includes(query) ||
          doc.description.toLowerCase().includes(query)
        )
      }))
      .filter(category => category.docs.length > 0);

    this.filteredCategories.set(filtered);
  }

  openDocumentation(path: string): void {
    // Validate path to prevent XSS attacks
    const sanitizedPath = this.sanitizePath(path);
    if (!sanitizedPath) {
      console.error('Invalid documentation path');
      return;
    }
    
    // Open in new tab - GitHub content
    const url = `${this.repositoryUrl}${sanitizedPath}`;
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  private sanitizePath(path: string): string | null {
    // Ensure path starts with a valid prefix
    const validPrefixes = ['/system-admin/', '/README.md', '/CHANGELOG.md', '/telemedicine/'];
    const hasValidPrefix = validPrefixes.some(prefix => path.startsWith(prefix));
    
    // Also allow root-level markdown files
    if (!hasValidPrefix && !path.match(/^\/[A-Z_]+\.md$/)) {
      return null;
    }
    
    // Remove any potentially dangerous characters
    const sanitized = path.replace(/[<>'"]/g, '');
    
    // Prevent path traversal attacks
    if (sanitized.includes('..')) {
      return null;
    }
    
    return sanitized;
  }

  getTotalDocs(): number {
    return this.categories().reduce((sum, cat) => sum + cat.docs.length, 0);
  }
}
