import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomFieldDto, CustomFieldType } from '../../../models/consultation-form-configuration.model';

@Component({
  selector: 'app-custom-fields-renderer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './custom-fields-renderer.component.html',
  styleUrl: './custom-fields-renderer.component.scss'
})
export class CustomFieldsRendererComponent {
  @Input() customFields: CustomFieldDto[] = [];
  @Input() formGroup!: FormGroup;
  @Input() sectionTitle: string = 'Campos Personalizados';
  
  CustomFieldType = CustomFieldType; // Expose enum to template
  
  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.initializeFormControls();
  }

  ngOnChanges() {
    // Re-initialize form controls when custom fields change
    if (this.formGroup && this.customFields) {
      this.initializeFormControls();
    }
  }

  /**
   * Initialize form controls for each custom field
   */
  private initializeFormControls(): void {
    if (!this.formGroup || !this.customFields) return;

    this.customFields.forEach(field => {
      // Only add control if it doesn't exist
      if (!this.formGroup.contains(field.fieldKey)) {
        const validators = field.isRequired ? [Validators.required] : [];
        const defaultValue = field.defaultValue || this.getDefaultValueForType(field.fieldType);
        this.formGroup.addControl(field.fieldKey, this.fb.control(defaultValue, validators));
      }
    });
  }

  /**
   * Get default value based on field type
   */
  private getDefaultValueForType(fieldType: CustomFieldType): any {
    switch (fieldType) {
      case CustomFieldType.Numero:
        return null;
      case CustomFieldType.CheckBox:
      case CustomFieldType.SimNao:
        return false;
      case CustomFieldType.SelecaoMultipla:
        return [];
      case CustomFieldType.Data:
        return null;
      default:
        return '';
    }
  }

  /**
   * Get sorted custom fields by display order
   */
  getSortedFields(): CustomFieldDto[] {
    return [...this.customFields].sort((a, b) => a.displayOrder - b.displayOrder);
  }

  /**
   * Handle checkbox change for multiple selection
   */
  onMultiSelectChange(fieldKey: string, option: string, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    const control = this.formGroup.get(fieldKey);
    
    if (!control) return;

    const currentValue = control.value || [];
    
    if (checkbox.checked) {
      control.setValue([...currentValue, option]);
    } else {
      control.setValue(currentValue.filter((v: string) => v !== option));
    }
  }

  /**
   * Check if an option is selected in multi-select
   */
  isMultiSelectOptionChecked(fieldKey: string, option: string): boolean {
    const control = this.formGroup.get(fieldKey);
    if (!control) return false;
    
    const value = control.value || [];
    return value.includes(option);
  }

  /**
   * Get field type label for display
   */
  getFieldTypeLabel(fieldType: CustomFieldType): string {
    const labels: { [key in CustomFieldType]: string } = {
      [CustomFieldType.TextoCurto]: 'Texto Curto',
      [CustomFieldType.TextoLongo]: 'Texto Longo',
      [CustomFieldType.Numero]: 'Número',
      [CustomFieldType.Data]: 'Data',
      [CustomFieldType.SelecaoUnica]: 'Seleção Única',
      [CustomFieldType.SelecaoMultipla]: 'Seleção Múltipla',
      [CustomFieldType.CheckBox]: 'Checkbox',
      [CustomFieldType.SimNao]: 'Sim/Não'
    };
    return labels[fieldType] || 'Desconhecido';
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldKey: string): string {
    const control = this.formGroup.get(fieldKey);
    if (!control || !control.errors) return '';

    if (control.errors['required']) {
      return 'Este campo é obrigatório';
    }
    if (control.errors['minlength']) {
      return `Mínimo de ${control.errors['minlength'].requiredLength} caracteres`;
    }
    if (control.errors['maxlength']) {
      return `Máximo de ${control.errors['maxlength'].requiredLength} caracteres`;
    }
    if (control.errors['min']) {
      return `Valor mínimo: ${control.errors['min'].min}`;
    }
    if (control.errors['max']) {
      return `Valor máximo: ${control.errors['max'].max}`;
    }
    if (control.errors['pattern']) {
      return 'Formato inválido';
    }
    
    return 'Valor inválido';
  }
}
