import { ComponentFixture, TestBed } from '@angular/core/testing';
import {
  FormErrorSummaryComponent,
  FieldErrorComponent,
  AccessibleFieldComponent,
  ValidationError
} from './form-validation.components';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';

describe('FormErrorSummaryComponent', () => {
  let component: FormErrorSummaryComponent;
  let fixture: ComponentFixture<FormErrorSummaryComponent>;

  const mockErrors: ValidationError[] = [
    { field: 'email', message: 'E-mail é obrigatório' },
    { field: 'password', message: 'Senha deve ter no mínimo 8 caracteres' }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FormErrorSummaryComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(FormErrorSummaryComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Accessibility Attributes', () => {
    beforeEach(() => {
      component.errors = mockErrors;
      fixture.detectChanges();
    });

    it('should have role="alert"', () => {
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary.nativeElement.getAttribute('role')).toBe('alert');
    });

    it('should have aria-live="assertive"', () => {
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary.nativeElement.getAttribute('aria-live')).toBe('assertive');
    });

    it('should have aria-atomic="true"', () => {
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary.nativeElement.getAttribute('aria-atomic')).toBe('true');
    });

    it('should be focusable with tabindex="-1"', () => {
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary.nativeElement.getAttribute('tabindex')).toBe('-1');
    });
  });

  describe('Error Display', () => {
    it('should not render when no errors', () => {
      component.errors = [];
      fixture.detectChanges();
      
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary).toBeFalsy();
    });

    it('should render when errors exist', () => {
      component.errors = mockErrors;
      fixture.detectChanges();
      
      const summary = fixture.debugElement.query(By.css('.error-summary'));
      expect(summary).toBeTruthy();
    });

    it('should display correct error count for single error', () => {
      component.errors = [mockErrors[0]];
      fixture.detectChanges();
      
      const description = fixture.debugElement.query(By.css('.error-summary-description'));
      expect(description.nativeElement.textContent).toContain('Foi encontrado 1 erro');
    });

    it('should display correct error count for multiple errors', () => {
      component.errors = mockErrors;
      fixture.detectChanges();
      
      const description = fixture.debugElement.query(By.css('.error-summary-description'));
      expect(description.nativeElement.textContent).toContain('Foram encontrados 2 erros');
    });

    it('should list all errors', () => {
      component.errors = mockErrors;
      fixture.detectChanges();
      
      const errorItems = fixture.debugElement.queryAll(By.css('.error-item'));
      expect(errorItems.length).toBe(2);
    });
  });

  describe('Error Links', () => {
    beforeEach(() => {
      component.errors = mockErrors;
      fixture.detectChanges();
    });

    it('should have links to field IDs', () => {
      const links = fixture.debugElement.queryAll(By.css('.error-link'));
      expect(links[0].nativeElement.getAttribute('href')).toBe('#field-email');
      expect(links[1].nativeElement.getAttribute('href')).toBe('#field-password');
    });

    it('should focus field on link click', () => {
      // Create mock field element
      const mockField = document.createElement('input');
      mockField.id = 'field-email';
      document.body.appendChild(mockField);
      spyOn(mockField, 'focus');
      spyOn(mockField, 'scrollIntoView');
      
      const link = fixture.debugElement.query(By.css('.error-link'));
      const event = new Event('click');
      spyOn(event, 'preventDefault');
      
      link.nativeElement.dispatchEvent(event);
      
      expect(event.preventDefault).toHaveBeenCalled();
      expect(mockField.focus).toHaveBeenCalled();
      
      document.body.removeChild(mockField);
    });

    it('should emit errorFocused event', () => {
      spyOn(component.errorFocused, 'emit');
      
      const link = fixture.debugElement.query(By.css('.error-link'));
      link.nativeElement.click();
      
      expect(component.errorFocused.emit).toHaveBeenCalledWith('email');
    });
  });
});

describe('FieldErrorComponent', () => {
  let component: FieldErrorComponent;
  let fixture: ComponentFixture<FieldErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FieldErrorComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(FieldErrorComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not render when no error', () => {
    component.error = undefined;
    fixture.detectChanges();
    
    const errorDiv = fixture.debugElement.query(By.css('.field-error'));
    expect(errorDiv).toBeFalsy();
  });

  it('should render when error exists', () => {
    component.error = 'Campo obrigatório';
    fixture.detectChanges();
    
    const errorDiv = fixture.debugElement.query(By.css('.field-error'));
    expect(errorDiv).toBeTruthy();
    expect(errorDiv.nativeElement.textContent).toContain('Campo obrigatório');
  });

  it('should have role="alert"', () => {
    component.error = 'Erro';
    fixture.detectChanges();
    
    const errorDiv = fixture.debugElement.query(By.css('.field-error'));
    expect(errorDiv.nativeElement.getAttribute('role')).toBe('alert');
  });

  it('should have aria-live when live validation is enabled', () => {
    component.error = 'Erro';
    component.live = true;
    fixture.detectChanges();
    
    const errorDiv = fixture.debugElement.query(By.css('.field-error'));
    expect(errorDiv.nativeElement.getAttribute('aria-live')).toBe('polite');
  });

  it('should have error ID attribute', () => {
    component.error = 'Erro';
    component.errorId = 'email-error';
    fixture.detectChanges();
    
    const errorDiv = fixture.debugElement.query(By.css('.field-error'));
    expect(errorDiv.nativeElement.getAttribute('id')).toBe('email-error');
  });
});

describe('AccessibleFieldComponent', () => {
  let component: AccessibleFieldComponent;
  let fixture: ComponentFixture<AccessibleFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AccessibleFieldComponent, FieldErrorComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(AccessibleFieldComponent);
    component = fixture.componentInstance;
    component.label = 'E-mail';
    component.fieldId = 'email';
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Label', () => {
    it('should render label with correct for attribute', () => {
      fixture.detectChanges();
      
      const label = fixture.debugElement.query(By.css('label'));
      expect(label.nativeElement.getAttribute('for')).toBe('email');
      expect(label.nativeElement.textContent).toContain('E-mail');
    });

    it('should show required indicator when required', () => {
      component.required = true;
      fixture.detectChanges();
      
      const indicator = fixture.debugElement.query(By.css('.required-indicator'));
      expect(indicator).toBeTruthy();
      expect(indicator.nativeElement.getAttribute('aria-label')).toBe('obrigatório');
    });

    it('should not show required indicator when not required', () => {
      component.required = false;
      fixture.detectChanges();
      
      const indicator = fixture.debugElement.query(By.css('.required-indicator'));
      expect(indicator).toBeFalsy();
    });
  });

  describe('Help Text', () => {
    it('should render help text when provided', () => {
      component.helpText = 'Digite seu e-mail';
      fixture.detectChanges();
      
      const helpDiv = fixture.debugElement.query(By.css('.help-text'));
      expect(helpDiv).toBeTruthy();
      expect(helpDiv.nativeElement.textContent).toContain('Digite seu e-mail');
    });

    it('should have correct ID for ARIA association', () => {
      component.helpText = 'Help text';
      fixture.detectChanges();
      
      const helpDiv = fixture.debugElement.query(By.css('.help-text'));
      expect(helpDiv.nativeElement.getAttribute('id')).toBe('email-help');
    });
  });

  describe('Error Display', () => {
    it('should render error when provided', () => {
      component.error = 'E-mail inválido';
      fixture.detectChanges();
      
      const errorComponent = fixture.debugElement.query(By.directive(FieldErrorComponent));
      expect(errorComponent).toBeTruthy();
    });

    it('should add has-error class when error exists', () => {
      component.error = 'Erro';
      fixture.detectChanges();
      
      const fieldDiv = fixture.debugElement.query(By.css('.form-field'));
      expect(fieldDiv.nativeElement.classList.contains('has-error')).toBe(true);
    });

    it('should not add has-error class when no error', () => {
      component.error = '';
      fixture.detectChanges();
      
      const fieldDiv = fixture.debugElement.query(By.css('.form-field'));
      expect(fieldDiv.nativeElement.classList.contains('has-error')).toBe(false);
    });
  });
});
