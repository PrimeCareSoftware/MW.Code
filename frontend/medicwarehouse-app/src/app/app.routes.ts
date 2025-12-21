import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  { 
    path: 'register', 
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard),
    canActivate: [authGuard]
  },
  { 
    path: 'patients', 
    loadComponent: () => import('./pages/patients/patient-list/patient-list').then(m => m.PatientList),
    canActivate: [authGuard]
  },
  { 
    path: 'patients/new', 
    loadComponent: () => import('./pages/patients/patient-form/patient-form').then(m => m.PatientForm),
    canActivate: [authGuard]
  },
  { 
    path: 'patients/:id', 
    loadComponent: () => import('./pages/patients/patient-form/patient-form').then(m => m.PatientForm),
    canActivate: [authGuard]
  },
  { 
    path: 'appointments', 
    loadComponent: () => import('./pages/appointments/appointment-list/appointment-list').then(m => m.AppointmentList),
    canActivate: [authGuard]
  },
  { 
    path: 'appointments/calendar', 
    loadComponent: () => import('./pages/appointments/appointment-calendar/appointment-calendar').then(m => m.AppointmentCalendar),
    canActivate: [authGuard]
  },
  { 
    path: 'appointments/new', 
    loadComponent: () => import('./pages/appointments/appointment-form/appointment-form').then(m => m.AppointmentForm),
    canActivate: [authGuard]
  },
  { 
    path: 'appointments/:appointmentId/attendance', 
    loadComponent: () => import('./pages/attendance/attendance').then(m => m.Attendance),
    canActivate: [authGuard]
  },
  { 
    path: 'waiting-queue', 
    loadComponent: () => import('./pages/waiting-queue/queue-management/queue-management').then(m => m.QueueManagementComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tickets', 
    loadComponent: () => import('./pages/tickets/tickets').then(m => m.Tickets),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];
