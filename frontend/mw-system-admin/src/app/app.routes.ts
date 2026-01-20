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
  
  // Wildcard route - redirect to dashboard
  { path: '**', redirectTo: '/dashboard' }
];
