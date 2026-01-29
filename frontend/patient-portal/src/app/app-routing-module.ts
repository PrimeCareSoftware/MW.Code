import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./pages/auth/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./pages/auth/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'appointments',
    loadComponent: () => import('./pages/appointments/appointments.component').then(m => m.AppointmentsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'appointments/book',
    loadComponent: () => import('./pages/appointments/appointment-booking/appointment-booking.component').then(m => m.AppointmentBookingComponent),
    canActivate: [authGuard]
  },
  {
    path: 'documents',
    loadComponent: () => import('./pages/documents/documents.component').then(m => m.DocumentsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    loadComponent: () => import('./pages/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard]
  },
  {
    path: 'privacy',
    loadComponent: () => import('./pages/privacy/PrivacyCenter.component').then(m => m.PrivacyCenterComponent),
    canActivate: [authGuard]
  },
  {
    path: 'privacy/data-viewer',
    loadComponent: () => import('./pages/privacy/DataViewer.component').then(m => m.DataViewerComponent),
    canActivate: [authGuard]
  },
  {
    path: 'privacy/data-portability',
    loadComponent: () => import('./pages/privacy/DataPortability.component').then(m => m.DataPortabilityComponent),
    canActivate: [authGuard]
  },
  {
    path: 'privacy/consent-manager',
    loadComponent: () => import('./pages/privacy/ConsentManager.component').then(m => m.ConsentManagerComponent),
    canActivate: [authGuard]
  },
  {
    path: 'privacy/deletion-request',
    loadComponent: () => import('./pages/privacy/DeletionRequest.component').then(m => m.DeletionRequestComponent),
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
