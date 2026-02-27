import { Routes } from '@angular/router';
import { SiteLayoutComponent } from './layout/site-layout/site-layout';

export const routes: Routes = [
  {
    path: '',
    component: SiteLayoutComponent,
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
      },
      {
        path: 'cases',
        loadComponent: () => import('./pages/site/cases/cases').then(m => m.CasesComponent)
      }
    ]
  },
  { path: '**', redirectTo: '/' }
];
