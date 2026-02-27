import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, NavigationEnd, RouterLink } from '@angular/router';
import { filter, Subscription } from 'rxjs';

interface Breadcrumb {
  label: string;
  url: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './breadcrumb.html',
  styleUrl: './breadcrumb.scss'
})
export class BreadcrumbComponent implements OnInit, OnDestroy {
  breadcrumbs: Breadcrumb[] = [];
  private subscription = new Subscription();

  private readonly labelMap: Record<string, string> = {
    dashboard: 'Dashboard',
    patients: 'Pacientes',
    new: 'Novo',
    edit: 'Editar',
    appointments: 'Agendamentos',
    calendar: 'Calendário',
    list: 'Lista',
    attendance: 'Atendimento',
    'waiting-queue': 'Fila de Espera',
    tickets: 'Chamados',
    crm: 'CRM',
    complaints: 'Reclamações',
    surveys: 'Pesquisas',
    'patient-journey': 'Jornada do Paciente',
    marketing: 'Marketing',
    admin: 'Admin',
    profiles: 'Perfis',
    analytics: 'Analytics',
    prescriptions: 'Prescrições',
    tiss: 'TISS',
    procedures: 'Procedimentos',
    settings: 'Configurações',
    company: 'Empresa',
    profile: 'Meu Perfil',
    financial: 'Financeiro',
    receivables: 'Contas a Receber',
    payables: 'Contas a Pagar',
    suppliers: 'Fornecedores',
    'cash-flow': 'Fluxo de Caixa',
    invoices: 'Notas Fiscais',
    reports: 'Relatórios',
    telemedicine: 'Telemedicina',
    anamnesis: 'Anamnese',
    'audit-logs': 'Logs de Auditoria',
    chat: 'Chat',
    referral: 'Referências',
  };

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.buildBreadcrumbs();
    this.subscription.add(
      this.router.events.pipe(filter(e => e instanceof NavigationEnd))
        .subscribe(() => this.buildBreadcrumbs())
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private buildBreadcrumbs(): void {
    const url = this.router.url.split('?')[0];
    const segments = url.split('/').filter(s => s && s !== 'site');
    const crumbs: Breadcrumb[] = [{ label: 'Início', url: '/dashboard' }];

    let accumulated = '';
    for (const segment of segments) {
      accumulated += `/${segment}`;
      const label = this.labelMap[segment] ?? this.toTitleCase(segment);
      crumbs.push({ label, url: accumulated });
    }

    // Don't show breadcrumb if only one crumb (home/dashboard itself)
    this.breadcrumbs = crumbs.length > 1 ? crumbs : [];
  }

  private toTitleCase(str: string): string {
    return str.replace(/-/g, ' ').replace(/\b\w/g, c => c.toUpperCase());
  }
}
