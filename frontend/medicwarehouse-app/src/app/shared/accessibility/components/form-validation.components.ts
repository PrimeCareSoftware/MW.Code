import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ValidationError {
  field: string;
  message: string;
}

/**
 * Accessible form error summary component following WCAG 2.1 AA guidelines
 * - ARIA live region for dynamic errors
 * - Keyboard navigable error list
 * - Links to form fields with errors
 * - Screen reader announcements
 */
@Component({
  selector: 'app-form-error-summary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      *ngIf="errors.length > 0"
      class="error-summary"
      role="alert"
      aria-live="assertive"
      aria-atomic="true"
      tabindex="-1"
      [id]="errorSummaryId"
    >
      <h2 class="error-summary-title">
        <span class="error-icon" aria-hidden="true">⚠</span>
        {{ title }}
      </h2>
      
      <p class="error-summary-description">
        {{ errors.length === 1 ? 'Foi encontrado 1 erro' : 'Foram encontrados ' + errors.length + ' erros' }} no formulário:
      </p>
      
      <ul class="error-list">
        <li *ngFor="let error of errors" class="error-item">
          <a
            [href]="'#' + getFieldId(error.field)"
            (click)="onErrorClick($event, error.field)"
            class="error-link"
          >
            {{ error.message }}
          </a>
        </li>
      </ul>
    </div>
  `,
  styles: [`
    .error-summary {
      background-color: #fdecea;
      border: 3px solid #d32f2f;
      border-radius: 4px;
      padding: 20px;
      margin-bottom: 24px;
    }

    .error-summary:focus {
      outline: 3px solid #2196f3;
      outline-offset: 2px;
    }

    .error-summary-title {
      color: #d32f2f;
      font-size: 1.25rem;
      font-weight: bold;
      margin: 0 0 12px 0;
      display: flex;
      align-items: center;
    }

    .error-icon {
      margin-right: 8px;
      font-size: 1.5rem;
    }

    .error-summary-description {
      color: #333;
      margin: 0 0 16px 0;
      font-size: 1rem;
    }

    .error-list {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .error-item {
      margin-bottom: 8px;
    }

    .error-link {
      color: #d32f2f;
      text-decoration: underline;
      font-weight: 500;
      cursor: pointer;
    }

    .error-link:hover {
      color: #b71c1c;
      background-color: rgba(211, 47, 47, 0.1);
    }

    .error-link:focus-visible {
      outline: 3px solid #2196f3;
      outline-offset: 2px;
      background-color: rgba(33, 150, 243, 0.1);
    }

    .error-link:visited {
      color: #d32f2f;
    }
  `]
})
export class FormErrorSummaryComponent {
  @Input() errors: ValidationError[] = [];
  @Input() title = 'Erros no formulário';
  @Input() errorSummaryId = 'form-error-summary';
  @Output() errorFocused = new EventEmitter<string>();

  getFieldId(fieldName: string): string {
    return `field-${fieldName}`;
  }

  onErrorClick(event: Event, fieldName: string): void {
    event.preventDefault();
    
    // Find and focus the field
    const fieldElement = document.getElementById(this.getFieldId(fieldName));
    if (fieldElement) {
      fieldElement.focus();
      fieldElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
    
    this.errorFocused.emit(fieldName);
  }
}

/**
 * Inline field error component with proper ARIA attributes
 */
@Component({
  selector: 'app-field-error',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      *ngIf="error"
      class="field-error"
      role="alert"
      [id]="errorId"
      [attr.aria-live]="live ? 'polite' : null"
    >
      <span class="error-icon" aria-hidden="true">✕</span>
      {{ error }}
    </div>
  `,
  styles: [`
    .field-error {
      color: #d32f2f;
      font-size: 0.875rem;
      margin-top: 4px;
      display: flex;
      align-items: center;
    }

    .error-icon {
      margin-right: 6px;
      font-weight: bold;
    }
  `]
})
export class FieldErrorComponent {
  @Input() error?: string;
  @Input() errorId = '';
  @Input() live = false; // Set to true for dynamic inline validation
}

/**
 * Accessible form field wrapper with proper labels and associations
 */
@Component({
  selector: 'app-accessible-field',
  standalone: true,
  imports: [CommonModule, FieldErrorComponent],
  template: `
    <div class="form-field" [class.has-error]="error">
      <label
        [for]="fieldId"
        [class.required]="required"
      >
        {{ label }}
        <span *ngIf="required" class="required-indicator" aria-label="obrigatório">*</span>
      </label>
      
      <div
        *ngIf="helpText"
        [id]="helpId"
        class="help-text"
      >
        {{ helpText }}
      </div>
      
      <ng-content></ng-content>
      
      <app-field-error
        [error]="error"
        [errorId]="errorId"
        [live]="liveValidation"
      ></app-field-error>
    </div>
  `,
  styles: [`
    .form-field {
      margin-bottom: 20px;
    }

    .form-field.has-error label {
      color: #d32f2f;
    }

    label {
      display: block;
      font-weight: 500;
      margin-bottom: 6px;
      color: #333;
    }

    label.required {
      font-weight: 600;
    }

    .required-indicator {
      color: #d32f2f;
      margin-left: 4px;
    }

    .help-text {
      font-size: 0.875rem;
      color: #666;
      margin-bottom: 6px;
    }
  `]
})
export class AccessibleFieldComponent {
  @Input() label = '';
  @Input() fieldId = '';
  @Input() helpText = '';
  @Input() error = '';
  @Input() required = false;
  @Input() liveValidation = false;

  get helpId(): string {
    return `${this.fieldId}-help`;
  }

  get errorId(): string {
    return `${this.fieldId}-error`;
  }
}
