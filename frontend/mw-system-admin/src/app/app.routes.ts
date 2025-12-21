import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard),
    canActivate: [authGuard]
  },
  {
    path: 'clinics',
    loadComponent: () => import('./pages/clinics/clinics-list').then(m => m.ClinicsList),
    canActivate: [authGuard]
  },
  {
    path: 'clinics/create',
    loadComponent: () => import('./pages/clinics/clinic-create').then(m => m.ClinicCreate),
    canActivate: [authGuard]
  },
  {
    path: 'clinics/:id',
    loadComponent: () => import('./pages/clinics/clinic-detail').then(m => m.ClinicDetail),
    canActivate: [authGuard]
  },
  {
    path: 'plans',
    loadComponent: () => import('./pages/plans/plans-list').then(m => m.PlansList),
    canActivate: [authGuard]
  },
  {
    path: 'clinic-owners',
    loadComponent: () => import('./pages/clinic-owners/clinic-owners-list').then(m => m.ClinicOwnersList),
    canActivate: [authGuard]
  },
  {
    path: 'subdomains',
    loadComponent: () => import('./pages/subdomains/subdomains-list').then(m => m.SubdomainsList),
    canActivate: [authGuard]
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];

