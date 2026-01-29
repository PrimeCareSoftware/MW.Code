import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Components
import { SkipToContentComponent } from './components/skip-to-content.component';
import { AccessibleBreadcrumbsComponent } from './components/accessible-breadcrumbs.component';
import { AccessibleTableComponent } from './components/accessible-table.component';
import {
  FormErrorSummaryComponent,
  FieldErrorComponent,
  AccessibleFieldComponent
} from './components/form-validation.components';

// Directives
import { FocusTrapDirective } from './directives/focus-trap.directive';

// Services
import { KeyboardNavigationService } from './hooks/keyboard-navigation.hook';
import { ScreenReaderService } from './hooks/screen-reader.service';

/**
 * Accessibility Module
 * 
 * Provides WCAG 2.1 Level AA compliant components, directives, and services
 * for building accessible Angular applications.
 * 
 * @example
 * ```typescript
 * import { AccessibilityModule } from '@shared/accessibility/accessibility.module';
 * 
 * @NgModule({
 *   imports: [AccessibilityModule]
 * })
 * export class MyModule { }
 * ```
 * 
 * Features:
 * - Keyboard navigation support
 * - Screen reader compatibility (NVDA, JAWS, VoiceOver)
 * - Focus management
 * - ARIA live regions
 * - Accessible form validation
 * - Semantic HTML components
 * - WCAG 2.1 AA compliant color contrasts
 */
@NgModule({
  declarations: [
    // Empty - all components are standalone
  ],
  imports: [
    CommonModule,
    // Standalone Components
    SkipToContentComponent,
    AccessibleBreadcrumbsComponent,
    AccessibleTableComponent,
    FormErrorSummaryComponent,
    FieldErrorComponent,
    AccessibleFieldComponent,
    
    // Standalone Directives
    FocusTrapDirective
  ],
  exports: [
    // Components
    SkipToContentComponent,
    AccessibleBreadcrumbsComponent,
    AccessibleTableComponent,
    FormErrorSummaryComponent,
    FieldErrorComponent,
    AccessibleFieldComponent,
    
    // Directives
    FocusTrapDirective
  ],
  providers: [
    KeyboardNavigationService,
    ScreenReaderService
  ]
})
export class AccessibilityModule { }
