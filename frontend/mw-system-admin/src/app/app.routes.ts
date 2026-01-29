import { Routes } from '@angular/router';
import { systemAdminGuard } from './guards/system-admin-guard';

export const routes: Routes = [
  // Login route - accessible without authentication
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  
  // Error pages
  {
    path: '403',
    loadComponent: () => import('./pages/errors/forbidden').then(m => m.Forbidden)
  },
  
  // Root redirect to dashboard
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  
  // Dashboard - requires system admin authentication
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard),
    canActivate: [systemAdminGuard]
  },
  
  // Clinics Management
  {
    path: 'clinics',
    loadComponent: () => import('./pages/clinics/clinics-list').then(m => m.ClinicsList),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'clinics/create',
    loadComponent: () => import('./pages/clinics/clinic-create').then(m => m.ClinicCreate),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'clinics/:id',
    loadComponent: () => import('./pages/clinics/clinic-detail').then(m => m.ClinicDetail),
    canActivate: [systemAdminGuard]
  },
  
  // Subscription Plans Management
  {
    path: 'plans',
    loadComponent: () => import('./pages/plans/plans-list').then(m => m.PlansList),
    canActivate: [systemAdminGuard]
  },
  
  // Clinic Owners Management
  {
    path: 'clinic-owners',
    loadComponent: () => import('./pages/clinic-owners/clinic-owners-list').then(m => m.ClinicOwnersList),
    canActivate: [systemAdminGuard]
  },
  
  // Subdomains Management
  {
    path: 'subdomains',
    loadComponent: () => import('./pages/subdomains/subdomains-list').then(m => m.SubdomainsList),
    canActivate: [systemAdminGuard]
  },
  
  // Support Tickets
  {
    path: 'tickets',
    loadComponent: () => import('./pages/tickets/tickets').then(m => m.TicketsPage),
    canActivate: [systemAdminGuard]
  },
  
  // Sales Metrics
  {
    path: 'sales-metrics',
    loadComponent: () => import('./pages/sales-metrics/sales-metrics').then(m => m.SalesMetrics),
    canActivate: [systemAdminGuard]
  },
  
  // Medications Management
  {
    path: 'medications',
    loadComponent: () => import('./pages/medications/medications-list').then(m => m.MedicationsList),
    canActivate: [systemAdminGuard]
  },
  
  // Exam Catalog Management
  {
    path: 'exam-catalog',
    loadComponent: () => import('./pages/exam-catalog/exam-catalog-list').then(m => m.ExamCatalogList),
    canActivate: [systemAdminGuard]
  },
  
  // Audit Logs
  {
    path: 'audit-logs',
    loadComponent: () => import('./pages/audit-logs/audit-logs').then(m => m.AuditLogs),
    canActivate: [systemAdminGuard]
  },
  
  // Documentation
  {
    path: 'documentation',
    loadComponent: () => import('./pages/documentation/documentation').then(m => m.Documentation),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 3: Analytics & BI - Custom Dashboards
  {
    path: 'custom-dashboards',
    loadComponent: () => import('./pages/custom-dashboards/custom-dashboards.component').then(m => m.CustomDashboardsComponent),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'custom-dashboards/:id/edit',
    loadComponent: () => import('./pages/custom-dashboards/dashboard-editor.component').then(m => m.DashboardEditorComponent),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 3: Analytics & BI - Reports
  {
    path: 'reports',
    loadComponent: () => import('./pages/reports/reports.component').then(m => m.ReportsComponent),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'reports/wizard',
    loadComponent: () => import('./pages/reports/report-wizard.component').then(m => m.ReportWizardComponent),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 3: Analytics & BI - Cohort Analysis
  {
    path: 'cohort-analysis',
    loadComponent: () => import('./pages/cohort-analysis/cohort-analysis.component').then(m => m.CohortAnalysisComponent),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 4: Workflow Automation - Workflows
  {
    path: 'workflows',
    loadComponent: () => import('./pages/workflows/workflows-list').then(m => m.WorkflowsList),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'workflows/create',
    loadComponent: () => import('./pages/workflows/workflow-editor').then(m => m.WorkflowEditor),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'workflows/:id/edit',
    loadComponent: () => import('./pages/workflows/workflow-editor').then(m => m.WorkflowEditor),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'workflows/:id/executions',
    loadComponent: () => import('./pages/workflows/workflow-executions').then(m => m.WorkflowExecutions),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 4: Workflow Automation - Webhooks
  {
    path: 'webhooks',
    loadComponent: () => import('./pages/webhooks/webhooks-list').then(m => m.WebhooksList),
    canActivate: [systemAdminGuard]
  },
  {
    path: 'webhooks/:id/deliveries',
    loadComponent: () => import('./pages/webhooks/webhook-deliveries').then(m => m.WebhookDeliveries),
    canActivate: [systemAdminGuard]
  },
  
  // Phase 5: Help Center
  {
    path: 'help',
    loadComponent: () => import('./shared/components/help-center/help-center.component').then(m => m.HelpCenterComponent),
    canActivate: [systemAdminGuard]
  },
  
  // LGPD - Consent Management
  {
    path: 'lgpd/consents',
    loadComponent: () => import('./pages/lgpd/consents/consent-management').then(m => m.ConsentManagement),
    canActivate: [systemAdminGuard]
  },
  
  // LGPD - Deletion Requests
  {
    path: 'lgpd/deletion-requests',
    loadComponent: () => import('./pages/lgpd/deletion-requests/deletion-requests').then(m => m.DeletionRequests),
    canActivate: [systemAdminGuard]
  },
  
  // LGPD - Dashboard
  {
    path: 'lgpd/dashboard',
    loadComponent: () => import('./pages/lgpd/dashboard/lgpd-dashboard').then(m => m.LgpdDashboard),
    canActivate: [systemAdminGuard]
  },
  
  // Module Configuration Management
  {
    path: 'modules',
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/modules-dashboard/modules-dashboard.component').then(m => m.ModulesDashboardComponent),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'plans',
        loadComponent: () => import('./pages/plan-modules/plan-modules.component').then(m => m.PlanModulesComponent),
        canActivate: [systemAdminGuard]
      },
      {
        path: ':moduleName',
        loadComponent: () => import('./pages/module-details/module-details.component').then(m => m.ModuleDetailsComponent),
        canActivate: [systemAdminGuard]
      }
    ]
  },
  
  // Wildcard route - redirect to dashboard
  { path: '**', redirectTo: '/dashboard' }
];
