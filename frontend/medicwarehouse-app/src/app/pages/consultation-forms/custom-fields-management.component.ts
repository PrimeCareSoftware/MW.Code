import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { HelpButtonComponent } from '../../shared/help-button/help-button';
import { ConsultationFormProfileService, ConsultationFormProfileDto, CustomFieldDto, CustomFieldType } from '../../services/consultation-form-profile.service';
import { Auth } from '../../services/auth';
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
  private readonly profileService = inject(ConsultationFormProfileService);
  private readonly auth = inject(Auth);

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
  selectedProfile = signal<ConsultationFormProfileDto | null>(null);
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
    
    this.profileService.getProfilesBySpecialty(specialty).subscribe({
      next: (profiles) => {
        // Get the system default or first profile for this specialty
        const profile = profiles.find(p => p.isSystemDefault) || profiles[0];
        
        if (profile) {
          this.selectedProfile.set(profile);
          this.populateForm(profile);
        } else {
          // No profile exists, initialize empty form
          this.initializeForm();
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading profiles:', error);
        this.errorMessage.set('Erro ao carregar configuração. Por favor, tente novamente.');
        this.isLoading.set(false);
        this.initializeForm();
      }
    });
  }

  populateForm(profile: ConsultationFormProfileDto): void {
    this.initializeForm();
    
    if (profile.customFields && profile.customFields.length > 0) {
      profile.customFields
        .sort((a, b) => a.displayOrder - b.displayOrder)
        .forEach(field => {
          this.fields.push(this.createFieldFormGroup(field));
        });
    }
  }

  createFieldFormGroup(field: CustomFieldDto): FormGroup {
    return this.fb.group({
      fieldKey: [field.fieldKey, Validators.required],
      label: [field.label, Validators.required],
      fieldType: [field.fieldType, Validators.required],
      isRequired: [field.isRequired],
      displayOrder: [field.displayOrder],
      placeholder: [field.placeholder || ''],
      defaultValue: [field.defaultValue || ''],
      helpText: [field.helpText || ''],
      options: [field.options || []]
    });
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

    const customFields: CustomFieldDto[] = this.fields.controls.map((control, index) => ({
      fieldKey: control.get('fieldKey')?.value,
      label: control.get('label')?.value,
      fieldType: control.get('fieldType')?.value,
      isRequired: control.get('isRequired')?.value,
      displayOrder: index + 1,
      placeholder: control.get('placeholder')?.value,
      defaultValue: control.get('defaultValue')?.value,
      helpText: control.get('helpText')?.value,
      options: control.get('options')?.value
    }));

    const profile = this.selectedProfile();
    if (profile) {
      // Update existing profile
      const updateDto = {
        name: profile.name,
        description: profile.description,
        showChiefComplaint: profile.showChiefComplaint,
        showHistoryOfPresentIllness: profile.showHistoryOfPresentIllness,
        showPastMedicalHistory: profile.showPastMedicalHistory,
        showFamilyHistory: profile.showFamilyHistory,
        showLifestyleHabits: profile.showLifestyleHabits,
        showCurrentMedications: profile.showCurrentMedications,
        requireChiefComplaint: profile.requireChiefComplaint,
        requireHistoryOfPresentIllness: profile.requireHistoryOfPresentIllness,
        requirePastMedicalHistory: profile.requirePastMedicalHistory,
        requireFamilyHistory: profile.requireFamilyHistory,
        requireLifestyleHabits: profile.requireLifestyleHabits,
        requireCurrentMedications: profile.requireCurrentMedications,
        requireClinicalExamination: profile.requireClinicalExamination,
        requireDiagnosticHypothesis: profile.requireDiagnosticHypothesis,
        requireInformedConsent: profile.requireInformedConsent,
        requireTherapeuticPlan: profile.requireTherapeuticPlan,
        customFields
      };

      this.profileService.updateProfile(profile.id, updateDto).subscribe({
        next: (updatedProfile) => {
          this.selectedProfile.set(updatedProfile);
          this.isLoading.set(false);
          this.successMessage.set('Configuração salva com sucesso!');
          
          setTimeout(() => {
            this.successMessage.set('');
          }, 3000);
        },
        error: (error) => {
          console.error('Error saving profile:', error);
          this.errorMessage.set('Erro ao salvar configuração. Por favor, tente novamente.');
          this.isLoading.set(false);
        }
      });
    } else {
      // Create new profile
      const specialty = this.selectedSpecialty();
      if (specialty === null) {
        this.errorMessage.set('Especialidade não selecionada.');
        this.isLoading.set(false);
        return;
      }

      const specialtyLabel = this.specialties().find(s => s.value === specialty)?.label || 'Perfil';
      const createDto = {
        name: `Perfil ${specialtyLabel}`,
        description: `Configuração de campos personalizados para ${specialtyLabel}`,
        specialty,
        customFields
      };

      this.profileService.createProfile(createDto).subscribe({
        next: (newProfile) => {
          this.selectedProfile.set(newProfile);
          this.isLoading.set(false);
          this.successMessage.set('Configuração criada com sucesso!');
          
          setTimeout(() => {
            this.successMessage.set('');
          }, 3000);
        },
        error: (error) => {
          console.error('Error creating profile:', error);
          this.errorMessage.set('Erro ao criar configuração. Por favor, tente novamente.');
          this.isLoading.set(false);
        }
      });
    }
  }

  getFieldTypeName(type: CustomFieldType): string {
    const fieldType = this.fieldTypes.find(ft => ft.value === type);
    return fieldType ? fieldType.label : 'Desconhecido';
  }

  backToSpecialtySelection(): void {
    this.selectedSpecialty.set(null);
    this.selectedProfile.set(null);
    this.initializeForm();
  }
}
