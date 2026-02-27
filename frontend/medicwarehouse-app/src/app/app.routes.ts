import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { ownerGuard } from './guards/owner-guard';
import { careFeatureGuard } from './guards/care-feature-guard';
import { adminGuard } from './guards/admin.guard';
import { CLINIC_ADMIN_ROUTES } from './pages/clinic-admin/clinic-admin.routes';
import { SiteLayoutComponent } from './layout/site-layout/site-layout';
import { CoreLayoutComponent } from './layout/core-layout/core-layout';

export const routes: Routes = [
  // Public site routes (marketing, registration) - SiteLayout
  { 
    path: 'site', 
    component: SiteLayoutComponent,
    children: [
      { 
        path: '', 
        loadComponent: () => import('./pages/site/home/home').then(m => m.HomeComponent)
      },
      {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then(m => m.Login)
      },
      {
        path: 'clinics',
        loadComponent: () => import('./pages/site/clinics/clinic-search').then(m => m.ClinicSearchComponent)
      },
      {
        path: 'clinics/:id/schedule',
        loadComponent: () => import('./pages/site/appointment-booking/appointment-booking.component').then(m => m.AppointmentBookingComponent)
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
      },
      { 
        path: 'blog', 
        loadComponent: () => import('./pages/site/blog/blog.component').then(m => m.BlogComponent)
      },
      { 
        path: 'blog/:slug', 
        loadComponent: () => import('./pages/site/blog/blog-article.component').then(m => m.BlogArticleComponent)
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

  // Legacy login route redirect
  { 
    path: 'login', 
    redirectTo: '/site/login',
    pathMatch: 'full'
  },
  { 
    path: 'register', 
    redirectTo: '/site/register',
    pathMatch: 'full'
  },

  // Homepage - redirect to site
  { path: '', redirectTo: '/site', pathMatch: 'full' },

  // Onboarding wizard - requires authentication (standalone, no core layout)
  { 
    path: 'onboarding', 
    loadComponent: () => import('./pages/onboarding/onboarding-wizard.component').then(m => m.OnboardingWizardComponent),
    canActivate: [authGuard]
  },

  // Fila de Espera (Queue System) - Public access for totem and TV panel
  {
    path: 'fila-espera/totem/:clinicId/:filaId',
    loadComponent: () => import('./pages/fila-espera/totem/totem.component').then(m => m.TotemComponent)
  },
  {
    path: 'fila-espera/gerar-senha/:clinicId/:filaId',
    loadComponent: () => import('./pages/fila-espera/totem/gerar-senha.component').then(m => m.GerarSenhaComponent)
  },
  {
    path: 'fila-espera/consultar/:clinicId/:filaId',
    loadComponent: () => import('./pages/fila-espera/totem/consultar-senha.component').then(m => m.ConsultarSenhaComponent)
  },
  {
    path: 'fila-espera/painel-tv/:clinicId/:filaId',
    loadComponent: () => import('./pages/fila-espera/painel-tv/painel-tv.component').then(m => m.PainelTvComponent)
  },

  // Authenticated routes with CoreLayout
  {
    path: '',
    component: CoreLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard) },
      { path: 'referral', loadComponent: () => import('./pages/referral/referral-dashboard.component').then(m => m.ReferralDashboardComponent), canActivate: [careFeatureGuard] },
      { path: 'patients', loadComponent: () => import('./pages/patients/patient-list/patient-list').then(m => m.PatientList) },
      { path: 'patients/new', loadComponent: () => import('./pages/patients/patient-form/patient-form').then(m => m.PatientForm) },
      { path: 'patients/:id', loadComponent: () => import('./pages/patients/patient-form/patient-form').then(m => m.PatientForm) },
      { path: 'appointments', redirectTo: '/appointments/calendar', pathMatch: 'full' },
      { path: 'appointments/list', loadComponent: () => import('./pages/appointments/appointment-list/appointment-list').then(m => m.AppointmentList), canActivate: [careFeatureGuard] },
      { path: 'appointments/calendar', loadComponent: () => import('./pages/appointments/appointment-calendar/appointment-calendar').then(m => m.AppointmentCalendar), canActivate: [careFeatureGuard] },
      { path: 'appointments/new', loadComponent: () => import('./pages/appointments/appointment-form/appointment-form').then(m => m.AppointmentForm), canActivate: [careFeatureGuard] },
      { path: 'appointments/:id/edit', loadComponent: () => import('./pages/appointments/appointment-form/appointment-form').then(m => m.AppointmentForm), canActivate: [careFeatureGuard] },
      { path: 'appointments/:appointmentId/attendance', loadComponent: () => import('./pages/attendance/attendance').then(m => m.Attendance), canActivate: [careFeatureGuard] },
      { path: 'waiting-queue', loadComponent: () => import('./pages/waiting-queue/queue-management/queue-management').then(m => m.QueueManagementComponent), canActivate: [careFeatureGuard] },
      { path: 'tickets', loadComponent: () => import('./pages/tickets/tickets').then(m => m.Tickets) },
      
      // CRM routes
      { path: 'crm/complaints', loadComponent: () => import('./pages/crm/complaints/complaint-list').then(m => m.ComplaintList) },
      { path: 'crm/surveys', loadComponent: () => import('./pages/crm/surveys/survey-list').then(m => m.SurveyList) },
      { path: 'crm/patient-journey', loadComponent: () => import('./pages/crm/patient-journey/patient-journey').then(m => m.PatientJourney) },
      { path: 'crm/marketing', loadComponent: () => import('./pages/crm/marketing/marketing-automation').then(m => m.MarketingAutomation) },
      
      // Admin routes
      { path: 'admin/profiles', loadComponent: () => import('./pages/admin/profiles/profile-list.component').then(m => m.ProfileListComponent), canActivate: [ownerGuard] },
      { path: 'admin/profiles/new', loadComponent: () => import('./pages/admin/profiles/profile-form.component').then(m => m.ProfileFormComponent), canActivate: [ownerGuard] },
      { path: 'admin/profiles/edit/:id', loadComponent: () => import('./pages/admin/profiles/profile-form.component').then(m => m.ProfileFormComponent), canActivate: [ownerGuard] },
      
      // Analytics
      { path: 'analytics', loadComponent: () => import('./pages/analytics/analytics-dashboard').then(m => m.AnalyticsDashboard) },
      { path: 'analytics/dashboard-clinico', loadComponent: () => import('./pages/analytics/dashboard-clinico/dashboard-clinico.component').then(m => m.DashboardClinicoComponent) },
      { path: 'analytics/dashboard-financeiro', loadComponent: () => import('./pages/analytics/dashboard-financeiro/dashboard-financeiro.component').then(m => m.DashboardFinanceiroComponent) },
      
      // Prescriptions
      { path: 'prescriptions/new/:medicalRecordId', loadComponent: () => import('./pages/prescriptions/digital-prescription-form.component').then(m => m.DigitalPrescriptionFormComponent) },
      { path: 'prescriptions/view/:id', loadComponent: () => import('./pages/prescriptions/digital-prescription-view.component').then(m => m.DigitalPrescriptionViewComponent) },
      { path: 'sngpc/dashboard', loadComponent: () => import('./pages/prescriptions/sngpc-dashboard.component').then(m => m.SNGPCDashboardComponent) },
      { path: 'consultation-forms/custom-fields', loadComponent: () => import('./pages/consultation-forms/custom-fields-management.component').then(m => m.CustomFieldsManagementComponent) },
      
      // TISS/TUSS routes
      { path: 'tiss/operators', loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operators-list').then(m => m.HealthInsuranceOperatorsList) },
      { path: 'tiss/operators/new', loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operator-form').then(m => m.HealthInsuranceOperatorForm) },
      { path: 'tiss/operators/edit/:id', loadComponent: () => import('./pages/tiss/health-insurance-operators/health-insurance-operator-form').then(m => m.HealthInsuranceOperatorForm) },
      { path: 'tiss/guides', loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-list').then(m => m.TissGuideList) },
      { path: 'tiss/guides/new', loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-form').then(m => m.TissGuideFormComponent) },
      { path: 'tiss/guides/edit/:id', loadComponent: () => import('./pages/tiss/tiss-guides/tiss-guide-form').then(m => m.TissGuideFormComponent) },
      { path: 'tiss/batches', loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-list').then(m => m.TissBatchList) },
      { path: 'tiss/batches/new', loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-form').then(m => m.TissBatchFormComponent) },
      { path: 'tiss/batches/:id', loadComponent: () => import('./pages/tiss/tiss-batches/tiss-batch-detail').then(m => m.TissBatchDetailComponent) },
      { path: 'tiss/authorizations', loadComponent: () => import('./pages/tiss/authorization-requests/authorization-request-list').then(m => m.AuthorizationRequestList) },
      { path: 'tiss/authorizations/new', loadComponent: () => import('./pages/tiss/authorization-requests/authorization-request-form').then(m => m.AuthorizationRequestFormComponent) },
      { path: 'tiss/patient-insurance/:patientId', loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-list').then(m => m.PatientInsuranceList) },
      { path: 'tiss/patient-insurance/:patientId/new', loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-form').then(m => m.PatientInsuranceFormComponent) },
      { path: 'tiss/patient-insurance/:patientId/edit/:id', loadComponent: () => import('./pages/tiss/patient-insurance/patient-insurance-form').then(m => m.PatientInsuranceFormComponent) },
      { path: 'tiss/procedures', loadComponent: () => import('./pages/tiss/tuss-procedures/tuss-procedure-list').then(m => m.TussProcedureList) },
      { path: 'tiss/dashboards/glosas', loadComponent: () => import('./pages/tiss/dashboards/glosas-dashboard').then(m => m.GlosasDashboard) },
      { path: 'tiss/dashboards/performance', loadComponent: () => import('./pages/tiss/dashboards/performance-dashboard').then(m => m.PerformanceDashboard) },
      { path: 'tiss/reports', loadComponent: () => import('./pages/tiss/reports/tiss-reports').then(m => m.TissReports) },
      
      // Procedures routes
      { path: 'procedures', loadComponent: () => import('./pages/procedures/procedure-list').then(m => m.ProcedureList), canActivate: [careFeatureGuard] },
      { path: 'procedures/new', loadComponent: () => import('./pages/procedures/procedure-form').then(m => m.ProcedureForm), canActivate: [careFeatureGuard] },
      { path: 'procedures/edit/:id', loadComponent: () => import('./pages/procedures/procedure-form').then(m => m.ProcedureForm), canActivate: [careFeatureGuard] },
      { path: 'procedures/owner-management', loadComponent: () => import('./pages/procedures/owner-procedure-management').then(m => m.OwnerProcedureManagement), canActivate: [ownerGuard] },
      
      // Settings routes
      { path: 'settings/company', loadComponent: () => import('./pages/settings/company-info').then(m => m.CompanyInfo), canActivate: [ownerGuard] },
      
      // User Profile route
      { path: 'profile', loadComponent: () => import('./pages/user-profile/user-profile.component').then(m => m.UserProfileComponent) },
      
      // Financial Module routes
      { path: 'financial/receivables', loadComponent: () => import('./pages/financial/accounts-receivable/receivables-list.component').then(m => m.ReceivablesListComponent) },
      { path: 'financial/receivables/new', loadComponent: () => import('./pages/financial/accounts-receivable/receivable-form.component').then(m => m.ReceivableFormComponent) },
      { path: 'financial/receivables/:id/payment', loadComponent: () => import('./pages/financial/accounts-receivable/receivable-payment.component').then(m => m.ReceivablePaymentComponent) },
      { path: 'financial/receivables/:id', loadComponent: () => import('./pages/financial/accounts-receivable/receivable-form.component').then(m => m.ReceivableFormComponent) },
      { path: 'financial/payables', loadComponent: () => import('./pages/financial/accounts-payable/payables-list.component').then(m => m.PayablesListComponent) },
      { path: 'financial/payables/new', loadComponent: () => import('./pages/financial/accounts-payable/payable-form.component').then(m => m.PayableFormComponent) },
      { path: 'financial/payables/:id/payment', loadComponent: () => import('./pages/financial/accounts-payable/payable-payment.component').then(m => m.PayablePaymentComponent) },
      { path: 'financial/payables/:id', loadComponent: () => import('./pages/financial/accounts-payable/payable-form.component').then(m => m.PayableFormComponent) },
      { path: 'financial/suppliers', loadComponent: () => import('./pages/financial/suppliers/suppliers-list.component').then(m => m.SuppliersListComponent) },
      { path: 'financial/suppliers/new', loadComponent: () => import('./pages/financial/suppliers/supplier-form.component').then(m => m.SupplierFormComponent) },
      { path: 'financial/suppliers/:id', loadComponent: () => import('./pages/financial/suppliers/supplier-form.component').then(m => m.SupplierFormComponent) },
      { path: 'financial/cash-flow', loadComponent: () => import('./pages/financial/cash-flow/cash-flow-dashboard.component').then(m => m.CashFlowDashboardComponent) },
      { path: 'financial/cash-flow/entries', loadComponent: () => import('./pages/financial/cash-flow/cash-flow-list.component').then(m => m.CashFlowListComponent) },
      { path: 'financial/closures', loadComponent: () => import('./pages/financial/financial-closures/closures-list.component').then(m => m.ClosuresListComponent) },
      { path: 'financial/closures/new', loadComponent: () => import('./pages/financial/financial-closures/closure-form.component').then(m => m.ClosureFormComponent) },
      { path: 'financial/invoices', loadComponent: () => import('./pages/financial/electronic-invoices/invoice-list.component').then(m => m.InvoiceListComponent) },
      { path: 'financial/invoices/config', loadComponent: () => import('./pages/financial/electronic-invoices/invoice-config.component').then(m => m.InvoiceConfigComponent) },
      { path: 'financial/invoices/new', loadComponent: () => import('./pages/financial/electronic-invoices/invoice-form.component').then(m => m.InvoiceFormComponent) },
      { path: 'financial/invoices/:id', loadComponent: () => import('./pages/financial/electronic-invoices/invoice-details.component').then(m => m.InvoiceDetailsComponent) },
      { path: 'financial/fiscal-dashboard', loadComponent: () => import('./pages/financial/dashboards/fiscal-dashboard').then(m => m.FiscalDashboard) },
      { path: 'financial/tax-dashboard', loadComponent: () => import('./pages/financial/tax-dashboard/tax-dashboard').then(m => m.TaxDashboard) },
      { path: 'financial/closures/:id', loadComponent: () => import('./pages/financial/financial-closures/closure-form.component').then(m => m.ClosureFormComponent) },
      { path: 'financial/reports/dre', loadComponent: () => import('./pages/financial/reports/dre-report.component').then(m => m.DREReportComponent) },
      { path: 'financial/reports/cash-flow-forecast', loadComponent: () => import('./pages/financial/reports/cash-flow-forecast.component').then(m => m.CashFlowForecastComponent) },
      { path: 'financial/reports/profitability', loadComponent: () => import('./pages/financial/reports/profitability-analysis.component').then(m => m.ProfitabilityAnalysisComponent) },
      
      // Telemedicine routes
      { path: 'telemedicine', loadComponent: () => import('./pages/telemedicine/session-list/session-list').then(m => m.SessionList), canActivate: [careFeatureGuard] },
      { path: 'telemedicine/new', loadComponent: () => import('./pages/telemedicine/session-form/session-form').then(m => m.SessionForm), canActivate: [careFeatureGuard] },
      { path: 'telemedicine/room/:id', loadComponent: () => import('./pages/telemedicine/video-room/video-room').then(m => m.VideoRoom), canActivate: [careFeatureGuard] },
      { path: 'telemedicine/details/:id', loadComponent: () => import('./pages/telemedicine/session-details/session-details').then(m => m.SessionDetails), canActivate: [careFeatureGuard] },
      { path: 'telemedicine/consent', loadComponent: () => import('./pages/telemedicine/consent-form/consent-form').then(m => m.ConsentForm), canActivate: [careFeatureGuard] },
      
      // SOAP Records routes
      { path: 'soap-records', loadChildren: () => import('./pages/soap-records/soap-records.routes').then(m => m.SOAP_ROUTES), canActivate: [careFeatureGuard] },
      
      // Anamnesis routes
      { path: 'anamnesis/templates', loadComponent: () => import('./pages/anamnesis/template-selector/template-selector').then(m => m.TemplateSelectorComponent), canActivate: [careFeatureGuard] },
      { path: 'anamnesis/templates/manage', loadComponent: () => import('./pages/anamnesis/template-management/template-management').then(m => m.TemplateManagementComponent), canActivate: [careFeatureGuard] },
      { path: 'anamnesis/templates/new', loadComponent: () => import('./pages/anamnesis/template-form/template-form').then(m => m.TemplateFormComponent), canActivate: [careFeatureGuard] },
      { path: 'anamnesis/templates/edit/:id', loadComponent: () => import('./pages/anamnesis/template-form/template-form').then(m => m.TemplateFormComponent), canActivate: [careFeatureGuard] },
      { path: 'anamnesis/questionnaire/:appointmentId', loadComponent: () => import('./pages/anamnesis/questionnaire/questionnaire').then(m => m.QuestionnaireComponent), canActivate: [careFeatureGuard] },
      { path: 'anamnesis/history/:patientId', loadComponent: () => import('./pages/anamnesis/history/history').then(m => m.AnamnesisHistoryComponent), canActivate: [careFeatureGuard] },
      
      // Audit Logs route
      { path: 'audit-logs', loadComponent: () => import('./pages/audit/audit-log-list.component').then(m => m.AuditLogListComponent), canActivate: [ownerGuard] },
      
      // Chat system
      { path: 'chat', loadComponent: () => import('./pages/chat/chat.component').then(m => m.ChatComponent) },

      ...CLINIC_ADMIN_ROUTES,
    ]
  },

  // Wildcard route - redirect to 404 page for unknown routes
  { path: '**', redirectTo: '/404' }
];

