import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { HelpButtonComponent } from '../../shared/help-button/help-button';
import { ConsultationFormConfigurationService } from '../../services/consultation-form-configuration.service';
import { ConsultationFormConfigurationDto, CustomFieldDto, CustomFieldType } from '../../models/consultation-form-configuration.model';
import { ProfessionalSpecialty } from '../../models/appointment.model';

interface SpecialtyOption {
  value: ProfessionalSpecialty;
  label: string;
  description: string;
}

@Component({
  selector: 'app-custom-fields-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar, HelpButtonComponent],
  templateUrl: './custom-fields-management.component.html',
  styleUrls: ['./custom-fields-management.component.scss']
})
export class CustomFieldsManagementComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly configService = inject(ConsultationFormConfigurationService);

  specialties = signal<SpecialtyOption[]>([
    { value: ProfessionalSpecialty.Medico, label: 'Médico', description: 'Consultas médicas gerais' },
    { value: ProfessionalSpecialty.Psicologo, label: 'Psicólogo', description: 'Sessões de psicoterapia' },
    { value: ProfessionalSpecialty.Nutricionista, label: 'Nutricionista', description: 'Consultas nutricionais' },
    { value: ProfessionalSpecialty.Fisioterapeuta, label: 'Fisioterapeuta', description: 'Sessões de fisioterapia' },
    { value: ProfessionalSpecialty.Dentista, label: 'Dentista', description: 'Consultas odontológicas' },
    { value: ProfessionalSpecialty.Enfermeiro, label: 'Enfermeiro', description: 'Atendimentos de enfermagem' },
    { value: ProfessionalSpecialty.TerapeutaOcupacional, label: 'Terapeuta Ocupacional', description: 'Sessões de terapia ocupacional' },
    { value: ProfessionalSpecialty.Fonoaudiologo, label: 'Fonoaudiólogo', description: 'Sessões de fonoaudiologia' },
    { value: ProfessionalSpecialty.Veterinario, label: 'Veterinário', description: 'Consultas veterinárias' }
  ]);

  selectedSpecialty = signal<ProfessionalSpecialty | null>(null);
  configuration = signal<ConsultationFormConfigurationDto | null>(null);
  customFieldsForm!: FormGroup;
  
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  fieldTypes = [
    { value: CustomFieldType.TextoCurto, label: 'Texto Curto' },
    { value: CustomFieldType.TextoLongo, label: 'Texto Longo' },
    { value: CustomFieldType.Numero, label: 'Número' },
    { value: CustomFieldType.Data, label: 'Data' },
    { value: CustomFieldType.SelecaoUnica, label: 'Seleção Única' },
    { value: CustomFieldType.SelecaoMultipla, label: 'Seleção Múltipla' },
    { value: CustomFieldType.CheckBox, label: 'Checkbox' },
    { value: CustomFieldType.SimNao, label: 'Sim/Não' }
  ];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.customFieldsForm = this.fb.group({
      fields: this.fb.array([])
    });
  }

  get fields(): FormArray {
    return this.customFieldsForm.get('fields') as FormArray;
  }

  selectSpecialty(specialty: ProfessionalSpecialty): void {
    this.selectedSpecialty.set(specialty);
    this.loadConfiguration(specialty);
  }

  loadConfiguration(specialty: ProfessionalSpecialty): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    // TODO: Implement loading configuration by specialty
    // For now, we'll initialize with empty fields
    this.initializeForm();
    this.isLoading.set(false);
  }

  addField(): void {
    const fieldForm = this.fb.group({
      fieldKey: ['', Validators.required],
      label: ['', Validators.required],
      fieldType: [CustomFieldType.TextoCurto, Validators.required],
      isRequired: [false],
      displayOrder: [this.fields.length + 1],
      placeholder: [''],
      defaultValue: [''],
      helpText: [''],
      options: [[]]
    });

    this.fields.push(fieldForm);
  }

  removeField(index: number): void {
    this.fields.removeAt(index);
    this.reorderFields();
  }

  moveFieldUp(index: number): void {
    if (index === 0) return;
    
    const field = this.fields.at(index);
    this.fields.removeAt(index);
    this.fields.insert(index - 1, field);
    this.reorderFields();
  }

  moveFieldDown(index: number): void {
    if (index === this.fields.length - 1) return;
    
    const field = this.fields.at(index);
    this.fields.removeAt(index);
    this.fields.insert(index + 1, field);
    this.reorderFields();
  }

  reorderFields(): void {
    this.fields.controls.forEach((control, index) => {
      control.patchValue({ displayOrder: index + 1 });
    });
  }

  needsOptions(fieldType: CustomFieldType): boolean {
    return fieldType === CustomFieldType.SelecaoUnica || 
           fieldType === CustomFieldType.SelecaoMultipla;
  }

  addOption(fieldIndex: number): void {
    const field = this.fields.at(fieldIndex);
    const options = field.get('options')?.value || [];
    options.push('');
    field.patchValue({ options });
  }

  removeOption(fieldIndex: number, optionIndex: number): void {
    const field = this.fields.at(fieldIndex);
    const options = field.get('options')?.value || [];
    options.splice(optionIndex, 1);
    field.patchValue({ options });
  }

  updateOption(fieldIndex: number, optionIndex: number, value: string): void {
    const field = this.fields.at(fieldIndex);
    const options = field.get('options')?.value || [];
    options[optionIndex] = value;
    field.patchValue({ options });
  }

  saveConfiguration(): void {
    if (this.customFieldsForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    // TODO: Implement save configuration
    console.log('Saving configuration:', this.customFieldsForm.value);
    
    setTimeout(() => {
      this.isLoading.set(false);
      this.successMessage.set('Configuração salva com sucesso!');
      
      setTimeout(() => {
        this.successMessage.set('');
      }, 3000);
    }, 1000);
  }

  getFieldTypeName(type: CustomFieldType): string {
    const fieldType = this.fieldTypes.find(ft => ft.value === type);
    return fieldType ? fieldType.label : 'Desconhecido';
  }

  backToSpecialtySelection(): void {
    this.selectedSpecialty.set(null);
    this.configuration.set(null);
    this.initializeForm();
  }
}
