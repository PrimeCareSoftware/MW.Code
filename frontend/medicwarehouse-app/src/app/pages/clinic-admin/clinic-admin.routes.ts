import { Routes } from '@angular/router';
import { authGuard } from '../../guards/auth-guard';
import { ownerGuard } from '../../guards/owner-guard';

export const CLINIC_ADMIN_ROUTES: Routes = [
  {
    path: 'clinic-admin',
    canActivate: [authGuard, ownerGuard],
    children: [
      {
        path: 'info',
        loadComponent: () => import('./clinic-info/clinic-info.component').then(m => m.ClinicInfoComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./user-management/user-management.component').then(m => m.UserManagementComponent)
      },
      {
        path: 'customization',
        loadComponent: () => import('./customization/customization-editor.component').then(m => m.CustomizationEditorComponent)
      },
      {
        path: 'subscription',
        loadComponent: () => import('./subscription/subscription-info.component').then(m => m.SubscriptionInfoComponent)
      },
      {
        path: 'tiss-config',
        loadComponent: () => import('./tiss-config/tiss-config.component').then(m => m.TissConfigComponent)
      },
      {
        path: 'public-display',
        loadComponent: () => import('./public-display/public-display.component').then(m => m.PublicDisplayComponent)
      },
      {
        path: 'modules',
        loadComponent: () => import('./modules/clinic-modules.component').then(m => m.ClinicModulesComponent)
      },
      {
        path: 'business-configuration',
        loadComponent: () => import('./business-configuration/business-configuration.component').then(m => m.BusinessConfigurationComponent)
      },
      {
        path: 'template-editor',
        loadComponent: () => import('./template-editor/template-editor.component').then(m => m.TemplateEditorComponent)
      },
      {
        path: '',
        redirectTo: 'info',
        pathMatch: 'full'
      }
    ]
  }
];
