import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';

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
            id: 'indice',
            title: 'Índice Geral',
            description: 'Índice completo da documentação do System Admin',
            path: '/system-admin/INDICE.md',
            icon: '📋'
          },
          {
            id: 'readme',
            title: 'README',
            description: 'Visão geral do System Admin',
            path: '/system-admin/README.md',
            icon: '📖'
          },
          {
            id: 'migration-report',
            title: 'Relatório de Migração',
            description: 'Relatório sobre migrações do sistema',
            path: '/system-admin/MIGRATION_REPORT.md',
            icon: '🔄'
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
            description: '70+ documentos sobre implementações do sistema',
            path: '/system-admin/implementacoes/',
            icon: '📝'
          }
        ]
      },
      {
        name: 'Guias',
        icon: '📖',
        docs: [
          {
            id: 'guias',
            title: 'Guias de Usuário e Desenvolvedor',
            description: '40+ guias práticos para uso e desenvolvimento',
            path: '/system-admin/guias/',
            icon: '🎓'
          }
        ]
      },
      {
        name: 'Conformidade CFM',
        icon: '⚕️',
        docs: [
          {
            id: 'cfm',
            title: 'Conformidade CFM',
            description: 'Documentação sobre conformidade com regulamentações do CFM',
            path: '/system-admin/cfm-compliance/',
            icon: '✅'
          }
        ]
      },
      {
        name: 'Frontend',
        icon: '🎨',
        docs: [
          {
            id: 'frontend',
            title: 'Documentação Frontend',
            description: 'Documentação específica do frontend',
            path: '/system-admin/frontend/',
            icon: '💻'
          }
        ]
      },
      {
        name: 'Backend',
        icon: '🔧',
        docs: [
          {
            id: 'backend',
            title: 'Documentação Backend',
            description: 'APIs e documentação do backend',
            path: '/system-admin/backend/',
            icon: '⚡'
          }
        ]
      },
      {
        name: 'Regras de Negócio',
        icon: '💼',
        docs: [
          {
            id: 'regras',
            title: 'Regras de Negócio',
            description: 'Especificações e regras de negócio do sistema',
            path: '/system-admin/regras-negocio/',
            icon: '📊'
          }
        ]
      },
      {
        name: 'Segurança',
        icon: '🔐',
        docs: [
          {
            id: 'seguranca',
            title: 'Documentação de Segurança',
            description: 'Protocolos e documentação de segurança',
            path: '/system-admin/seguranca/',
            icon: '🛡️'
          }
        ]
      },
      {
        name: 'Infraestrutura',
        icon: '🏗️',
        docs: [
          {
            id: 'infrastructure',
            title: 'Guias de Infraestrutura',
            description: 'DevOps e documentação de infraestrutura',
            path: '/system-admin/infrastructure/',
            icon: '☁️'
          }
        ]
      },
      {
        name: 'Documentação Técnica',
        icon: '📐',
        docs: [
          {
            id: 'docs',
            title: 'Documentos Técnicos',
            description: 'Migrações, schemas, testes e documentação técnica',
            path: '/system-admin/docs/',
            icon: '🔬'
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
    // Open in new tab - GitHub raw content
    const repoUrl = 'https://github.com/PrimeCareSoftware/MW.Code/blob/main';
    window.open(`${repoUrl}${path}`, '_blank');
  }

  getTotalDocs(): number {
    return this.categories().reduce((sum, cat) => sum + cat.docs.length, 0);
  }
}
