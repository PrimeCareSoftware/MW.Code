import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { QuillEditorComponent } from 'ngx-quill';
import {
  GlobalDocumentTemplate,
  CreateGlobalTemplateDto,
  UpdateGlobalTemplateDto,
  DocumentTemplateType,
  ProfessionalSpecialty,
  DocumentTemplateTypeLabels,
  ProfessionalSpecialtyLabels,
  TemplateVariable
} from '../../models/global-document-template.model';
import { GlobalDocumentTemplateService } from '../../services/global-document-template.service';

@Component({
  selector: 'app-global-template-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, QuillEditorComponent],
  templateUrl: './global-template-editor.component.html',
  styleUrls: ['./global-template-editor.component.scss']
})
export class GlobalTemplateEditorComponent implements OnInit {
  form!: FormGroup;
  templateId = signal<string | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);
  isEditMode = signal(false);

  // Enums for template
  DocumentTemplateType = DocumentTemplateType;
  ProfessionalSpecialty = ProfessionalSpecialty;
  DocumentTemplateTypeLabels = DocumentTemplateTypeLabels;
  ProfessionalSpecialtyLabels = ProfessionalSpecialtyLabels;

  // Arrays for dropdowns
  specialties = Object.keys(ProfessionalSpecialty)
    .filter(key => !isNaN(Number(key)))
    .map(key => ({
      value: Number(key) as ProfessionalSpecialty,
      label: ProfessionalSpecialtyLabels[Number(key) as ProfessionalSpecialty]
    }));

  templateTypes = Object.keys(DocumentTemplateType)
    .filter(key => !isNaN(Number(key)))
    .map(key => ({
      value: Number(key) as DocumentTemplateType,
      label: DocumentTemplateTypeLabels[Number(key) as DocumentTemplateType]
    }));

  // Available variables for the sidebar
  availableVariables: TemplateVariable[] = [
    { key: 'patientName', label: 'Nome do Paciente', type: 'text', isRequired: false, displayOrder: 1 },
    { key: 'patientAge', label: 'Idade do Paciente', type: 'number', isRequired: false, displayOrder: 2 },
    { key: 'patientCPF', label: 'CPF do Paciente', type: 'text', isRequired: false, displayOrder: 3 },
    { key: 'professionalName', label: 'Nome do Profissional', type: 'text', isRequired: false, displayOrder: 4 },
    { key: 'professionalRegistration', label: 'Registro do Profissional', type: 'text', isRequired: false, displayOrder: 5 },
    { key: 'clinicName', label: 'Nome da Clínica', type: 'text', isRequired: false, displayOrder: 6 },
    { key: 'clinicAddress', label: 'Endereço da Clínica', type: 'text', isRequired: false, displayOrder: 7 },
    { key: 'currentDate', label: 'Data Atual', type: 'date', isRequired: false, displayOrder: 8 },
    { key: 'appointmentDate', label: 'Data da Consulta', type: 'date', isRequired: false, displayOrder: 9 },
    { key: 'diagnosis', label: 'Diagnóstico', type: 'text', isRequired: false, displayOrder: 10 },
  ];

  // Quill editor configuration
  quillModules = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'align': [] }],
      ['clean']
    ]
  };

  constructor(
    private fb: FormBuilder,
    private templateService: GlobalDocumentTemplateService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.templateId.set(id);
      this.isEditMode.set(true);
      this.loadTemplate(id);
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      type: [DocumentTemplateType.MedicalRecord, Validators.required],
      specialty: [ProfessionalSpecialty.Medico, Validators.required],
      content: ['', Validators.required],
      isActive: [true]
    });
  }

  private loadTemplate(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.templateService.getById(id).subscribe({
      next: (template) => {
        this.form.patchValue({
          name: template.name,
          description: template.description,
          type: template.type,
          specialty: template.specialty,
          content: template.content,
          isActive: template.isActive
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar template: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  insertVariable(variable: TemplateVariable): void {
    const currentContent = this.form.get('content')?.value || '';
    const variableText = `{{${variable.key}}}`;
    this.form.patchValue({
      content: currentContent + variableText
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      this.showError('Por favor, preencha todos os campos obrigatórios corretamente.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const formValue = this.form.value;
    const variablesJson = JSON.stringify(this.availableVariables);

    if (this.isEditMode()) {
      const updateDto: UpdateGlobalTemplateDto = {
        name: formValue.name,
        description: formValue.description,
        content: formValue.content,
        variables: variablesJson,
        isActive: formValue.isActive
      };

      this.templateService.update(this.templateId()!, updateDto).subscribe({
        next: () => {
          this.showSuccess('Template atualizado com sucesso!');
          this.loading.set(false);
          setTimeout(() => this.router.navigate(['/global-templates']), 1500);
        },
        error: (err) => {
          this.showError('Erro ao atualizar template: ' + (err.error?.message || err.message));
          this.loading.set(false);
        }
      });
    } else {
      const createDto: CreateGlobalTemplateDto = {
        name: formValue.name,
        description: formValue.description,
        type: formValue.type,
        specialty: formValue.specialty,
        content: formValue.content,
        variables: variablesJson
      };

      this.templateService.create(createDto).subscribe({
        next: () => {
          this.showSuccess('Template criado com sucesso!');
          this.loading.set(false);
          setTimeout(() => this.router.navigate(['/global-templates']), 1500);
        },
        error: (err) => {
          this.showError('Erro ao criar template: ' + (err.error?.message || err.message));
          this.loading.set(false);
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/global-templates']);
  }

  private showSuccess(message: string): void {
    this.success.set(message);
    setTimeout(() => this.success.set(null), 3000);
  }

  private showError(message: string): void {
    this.error.set(message);
    setTimeout(() => this.error.set(null), 5000);
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (field?.touched && field?.errors) {
      if (field.errors['required']) {
        return 'Este campo é obrigatório';
      }
      if (field.errors['maxlength']) {
        return `Máximo de ${field.errors['maxlength'].requiredLength} caracteres`;
      }
    }
    return '';
  }
}
