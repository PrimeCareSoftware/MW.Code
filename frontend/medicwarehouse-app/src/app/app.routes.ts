import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { ownerGuard } from './guards/owner-guard';
import { systemAdminGuard } from './guards/system-admin-guard';
import { CLINIC_ADMIN_ROUTES } from './pages/clinic-admin/clinic-admin.routes';

export const routes: Routes = [
  // Public site routes (marketing, registration) - no authentication required
  { 
    path: 'site', 
    children: [
      { 
        path: '', 
        loadComponent: () => import('./pages/site/home/home').then(m => m.HomeComponent)
      },
      { 
        path: 'pricing', 
        loadComponent: () => import('./pages/site/pricing/pricing').then(m => m.PricingComponent)
      },
      { 
        path: 'contact', 
        loadComponent: () => import('./pages/site/contact/contact').then(m => m.ContactComponent)
      },
      { 
        path: 'testimonials', 
        loadComponent: () => import('./pages/site/testimonials/testimonials').then(m => m.TestimonialsComponent)
      },
      { 
        path: 'register', 
        loadComponent: () => import('./pages/site/register/register').then(m => m.RegisterComponent)
      },
      { 
        path: 'cart', 
        loadComponent: () => import('./pages/site/cart/cart').then(m => m.CartComponent)
      },
      { 
        path: 'checkout', 
        loadComponent: () => import('./pages/site/checkout/checkout').then(m => m.CheckoutComponent)
      },
      { 
        path: 'privacy', 
        loadComponent: () => import('./pages/site/privacy/privacy').then(m => m.PrivacyComponent)
      },
      { 
        path: 'terms', 
        loadComponent: () => import('./pages/site/terms/terms').then(m => m.TermsComponent)
      }
    ]
  },
  
  // System Admin routes - requires system owner authentication
  { 
    path: 'system-admin', 
    children: [
      {
        path: 'login',
        loadComponent: () => import('./pages/system-admin/login/login').then(m => m.Login)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
        canActivate: [systemAdminGuard]
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/system-admin/dashboard/dashboard').then(m => m.Dashboard),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'clinics',
        loadComponent: () => import('./pages/system-admin/clinics/clinics-list').then(m => m.ClinicsList),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'clinics/create',
        loadComponent: () => import('./pages/system-admin/clinics/clinic-create').then(m => m.ClinicCreate),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'clinics/:id',
        loadComponent: () => import('./pages/system-admin/clinics/clinic-detail').then(m => m.ClinicDetail),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'plans',
        loadComponent: () => import('./pages/system-admin/plans/plans-list').then(m => m.PlansList),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'clinic-owners',
        loadComponent: () => import('./pages/system-admin/clinic-owners/clinic-owners-list').then(m => m.ClinicOwnersList),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'subdomains',
        loadComponent: () => import('./pages/system-admin/subdomains/subdomains-list').then(m => m.SubdomainsList),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'tickets',
        loadComponent: () => import('./pages/system-admin/tickets/tickets').then(m => m.TicketsPage),
        canActivate: [systemAdminGuard]
      },
      {
        path: 'sales-metrics',
        loadComponent: () => import('./pages/system-admin/sales-metrics/sales-metrics').then(m => m.SalesMetrics),
        canActivate: [systemAdminGuard]
      }
    ]
  },

  // Default redirect to dashboard for authenticated users, or to site for public
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
  { 
    path: 'admin/profiles', 
    loadComponent: () => import('./pages/admin/profiles/profile-list.component').then(m => m.ProfileListComponent),
    canActivate: [authGuard, ownerGuard]
  },
  { 
    path: 'admin/profiles/new', 
    loadComponent: () => import('./pages/admin/profiles/profile-form.component').then(m => m.ProfileFormComponent),
    canActivate: [authGuard, ownerGuard]
  },
  { 
    path: 'admin/profiles/edit/:id', 
    loadComponent: () => import('./pages/admin/profiles/profile-form.component').then(m => m.ProfileFormComponent),
    canActivate: [authGuard, ownerGuard]
  },
  { 
    path: 'analytics', 
    loadComponent: () => import('./pages/analytics/analytics-dashboard').then(m => m.AnalyticsDashboard),
    canActivate: [authGuard]
  },
  { 
    path: 'prescriptions/new/:medicalRecordId', 
    loadComponent: () => import('./pages/prescriptions/digital-prescription-form.component').then(m => m.DigitalPrescriptionFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'prescriptions/view/:id', 
    loadComponent: () => import('./pages/prescriptions/digital-prescription-view.component').then(m => m.DigitalPrescriptionViewComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'sngpc/dashboard', 
    loadComponent: () => import('./pages/prescriptions/sngpc-dashboard.component').then(m => m.SNGPCDashboardComponent),
    canActivate: [authGuard]
  },
  ...CLINIC_ADMIN_ROUTES,
  { path: '**', redirectTo: '/dashboard' }
];
