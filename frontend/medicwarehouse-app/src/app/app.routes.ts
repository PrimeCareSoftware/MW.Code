import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { ownerGuard } from './guards/owner-guard';
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
        path: 'clinics',
        loadComponent: () => import('./pages/site/clinics/clinic-search').then(m => m.ClinicSearchComponent)
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

  // Error pages - no authentication required
  { 
    path: '401', 
    loadComponent: () => import('./pages/errors/unauthorized').then(m => m.Unauthorized)
  },
  { 
    path: '403', 
    loadComponent: () => import('./pages/errors/forbidden').then(m => m.Forbidden)
  },
  { 
    path: '404', 
    loadComponent: () => import('./pages/errors/not-found').then(m => m.NotFound)
  },

  // Login route - accessible only via URL (not redirected to by guards)
  { 
    path: 'login', 
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  { 
    path: 'register', 
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },

  // Homepage - redirect to dashboard (will be protected by authGuard)
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },

  // Main application routes - require authentication
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
    path: 'appointments/:id/edit', 
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
  
  // TISS/TUSS routes
  { 
    path: 'tiss/operators', 
    loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operators-list').then(m => m.HealthInsuranceOperatorsList),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/operators/new', 
    loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operator-form').then(m => m.HealthInsuranceOperatorForm),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/operators/edit/:id', 
    loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operator-form').then(m => m.HealthInsuranceOperatorForm),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/guides', 
    loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-list').then(m => m.TissGuideList),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/guides/new', 
    loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-form').then(m => m.TissGuideFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/guides/edit/:id', 
    loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-form').then(m => m.TissGuideFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/batches', 
    loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-list').then(m => m.TissBatchList),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/batches/new', 
    loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-form').then(m => m.TissBatchFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/batches/:id', 
    loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-detail').then(m => m.TissBatchDetailComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/authorizations', 
    loadComponent: () => import('./pages/tiss/authorization-requests/authorization-request-list').then(m => m.AuthorizationRequestList),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/authorizations/new', 
    loadComponent: () => import('./pages/tiss/authorization-requests/authorization-request-form').then(m => m.AuthorizationRequestFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/patient-insurance/:patientId', 
    loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-list').then(m => m.PatientInsuranceList),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/patient-insurance/:patientId/new', 
    loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-form').then(m => m.PatientInsuranceFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/patient-insurance/:patientId/edit/:id', 
    loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-form').then(m => m.PatientInsuranceFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'tiss/procedures', 
    loadComponent: () => import('./pages/tiss/tuss-procedures/tuss-procedure-list').then(m => m.TussProcedureList),
    canActivate: [authGuard]
  },
  
  // Financial Module routes
  { 
    path: 'financial/receivables', 
    loadComponent: () => import('./pages/financial/accounts-receivable/receivables-list.component').then(m => m.ReceivablesListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/receivables/new', 
    loadComponent: () => import('./pages/financial/accounts-receivable/receivable-form.component').then(m => m.ReceivableFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/receivables/:id/payment', 
    loadComponent: () => import('./pages/financial/accounts-receivable/receivable-payment.component').then(m => m.ReceivablePaymentComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/receivables/:id', 
    loadComponent: () => import('./pages/financial/accounts-receivable/receivable-form.component').then(m => m.ReceivableFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/payables', 
    loadComponent: () => import('./pages/financial/accounts-payable/payables-list.component').then(m => m.PayablesListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/payables/new', 
    loadComponent: () => import('./pages/financial/accounts-payable/payable-form.component').then(m => m.PayableFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/payables/:id/payment', 
    loadComponent: () => import('./pages/financial/accounts-payable/payable-payment.component').then(m => m.PayablePaymentComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/payables/:id', 
    loadComponent: () => import('./pages/financial/accounts-payable/payable-form.component').then(m => m.PayableFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/suppliers', 
    loadComponent: () => import('./pages/financial/suppliers/suppliers-list.component').then(m => m.SuppliersListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/suppliers/new', 
    loadComponent: () => import('./pages/financial/suppliers/supplier-form.component').then(m => m.SupplierFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/suppliers/:id', 
    loadComponent: () => import('./pages/financial/suppliers/supplier-form.component').then(m => m.SupplierFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/cash-flow', 
    loadComponent: () => import('./pages/financial/cash-flow/cash-flow-dashboard.component').then(m => m.CashFlowDashboardComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/cash-flow/entries', 
    loadComponent: () => import('./pages/financial/cash-flow/cash-flow-list.component').then(m => m.CashFlowListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/closures', 
    loadComponent: () => import('./pages/financial/financial-closures/closures-list.component').then(m => m.ClosuresListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/closures/new', 
    loadComponent: () => import('./pages/financial/financial-closures/closure-form.component').then(m => m.ClosureFormComponent),
    canActivate: [authGuard]
  },
  
  // Electronic Invoices routes
  { 
    path: 'financial/invoices', 
    loadComponent: () => import('./pages/financial/electronic-invoices/invoice-list.component').then(m => m.InvoiceListComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/invoices/config', 
    loadComponent: () => import('./pages/financial/electronic-invoices/invoice-config.component').then(m => m.InvoiceConfigComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/invoices/new', 
    loadComponent: () => import('./pages/financial/electronic-invoices/invoice-form.component').then(m => m.InvoiceFormComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/invoices/:id', 
    loadComponent: () => import('./pages/financial/electronic-invoices/invoice-details.component').then(m => m.InvoiceDetailsComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'financial/closures/:id', 
    loadComponent: () => import('./pages/financial/financial-closures/closure-form.component').then(m => m.ClosureFormComponent),
    canActivate: [authGuard]
  },
  
  // Telemedicine routes
  { 
    path: 'telemedicine', 
    loadComponent: () => import('./pages/telemedicine/session-list/session-list').then(m => m.SessionList),
    canActivate: [authGuard]
  },
  { 
    path: 'telemedicine/new', 
    loadComponent: () => import('./pages/telemedicine/session-form/session-form').then(m => m.SessionForm),
    canActivate: [authGuard]
  },
  { 
    path: 'telemedicine/room/:id', 
    loadComponent: () => import('./pages/telemedicine/video-room/video-room').then(m => m.VideoRoom),
    canActivate: [authGuard]
  },
  { 
    path: 'telemedicine/details/:id', 
    loadComponent: () => import('./pages/telemedicine/session-details/session-details').then(m => m.SessionDetails),
    canActivate: [authGuard]
  },
  { 
    path: 'telemedicine/consent', 
    loadComponent: () => import('./pages/telemedicine/consent-form/consent-form').then(m => m.ConsentForm),
    canActivate: [authGuard]
  },
  
  ...CLINIC_ADMIN_ROUTES,

  // Wildcard route - redirect to 404 page for unknown routes
  { path: '**', redirectTo: '/404' }
];
