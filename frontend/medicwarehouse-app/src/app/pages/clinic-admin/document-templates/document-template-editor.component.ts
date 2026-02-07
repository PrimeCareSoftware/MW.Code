import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { QuillEditorComponent } from 'ngx-quill';
import { 
  DocumentTemplate,
  CreateDocumentTemplateDto,
  UpdateDocumentTemplateDto,
  TemplateVariable,
  DocumentTemplateType,
  ProfessionalSpecialty,
  DocumentTemplateTypeLabels,
  ProfessionalSpecialtyLabels
} from '../../../models/document-template.model';
import { DocumentTemplateService } from '../../../services/document-template.service';

@Component({
  selector: 'app-document-template-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, QuillEditorComponent],
  templateUrl: './document-template-editor.component.html',
  styleUrls: ['./document-template-editor.component.scss']
})
export class DocumentTemplateEditorComponent implements OnInit {
  form!: FormGroup;
  template?: DocumentTemplate;
  isEditMode = false;
  isViewMode = false;
  loading = false;
  saving = false;
  error: string | null = null;
  success: string | null = null;
  
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

  // Common variables for all templates
  commonVariables: TemplateVariable[] = [
    { key: 'patientName', label: 'Nome do Paciente', type: 'text', isRequired: true, displayOrder: 1 },
    { key: 'patientCpf', label: 'CPF do Paciente', type: 'text', isRequired: false, displayOrder: 2 },
    { key: 'patientBirthDate', label: 'Data de Nascimento', type: 'date', isRequired: false, displayOrder: 3 },
    { key: 'patientAge', label: 'Idade do Paciente', type: 'number', isRequired: false, displayOrder: 4 },
    { key: 'consultationDate', label: 'Data da Consulta', type: 'date', isRequired: true, displayOrder: 5 },
    { key: 'professionalName', label: 'Nome do Profissional', type: 'text', isRequired: true, displayOrder: 6 },
    { key: 'professionalRegistration', label: 'Registro Profissional', type: 'text', isRequired: true, displayOrder: 7 },
    { key: 'clinicName', label: 'Nome da Clínica', type: 'text', isRequired: false, displayOrder: 8 },
    { key: 'clinicAddress', label: 'Endereço da Clínica', type: 'text', isRequired: false, displayOrder: 9 }
  ];

  constructor(
    private fb: FormBuilder,
    private templateService: DocumentTemplateService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    
    const id = this.route.snapshot.paramMap.get('id');
    const mode = this.route.snapshot.data['mode'];
    
    if (mode === 'view') {
      this.isViewMode = true;
    }
    
    if (id && id !== 'new') {
      this.isEditMode = true;
      this.loadTemplate(id);
    } else {
      // Initialize with common variables
      this.setVariables(this.commonVariables);
    }
  }

  initializeForm(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      specialty: [null, Validators.required],
      type: [null, Validators.required],
      content: ['', Validators.required],
      variables: this.fb.array([])
    });
    
    if (this.isViewMode) {
      this.form.disable();
    }
  }

  get variablesFormArray(): FormArray {
    return this.form.get('variables') as FormArray;
  }

  loadTemplate(id: string): void {
    this.loading = true;
    this.templateService.getById(id).subscribe({
      next: (template) => {
        this.template = template;
        this.form.patchValue({
          name: template.name,
          description: template.description,
          specialty: template.specialty,
          type: template.type,
          content: template.content
        });
        
        try {
          const variables: TemplateVariable[] = JSON.parse(template.variables);
          this.setVariables(variables);
        } catch (e) {
          console.error('Error parsing variables:', e);
          this.setVariables(this.commonVariables);
        }
        
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar template: ' + (err.error?.message || err.message);
        this.loading = false;
      }
    });
  }

  setVariables(variables: TemplateVariable[]): void {
    this.variablesFormArray.clear();
    variables.forEach(v => this.addVariableFromData(v));
  }

  addVariable(): void {
    const variableGroup = this.fb.group({
      key: ['', Validators.required],
      label: ['', Validators.required],
      type: ['text', Validators.required],
      description: [''],
      defaultValue: [''],
      isRequired: [false],
      displayOrder: [this.variablesFormArray.length]
    });
    
    this.variablesFormArray.push(variableGroup);
  }

  addVariableFromData(variable: TemplateVariable): void {
    const variableGroup = this.fb.group({
      key: [variable.key, Validators.required],
      label: [variable.label, Validators.required],
      type: [variable.type, Validators.required],
      description: [variable.description || ''],
      defaultValue: [variable.defaultValue || ''],
      isRequired: [variable.isRequired],
      displayOrder: [variable.displayOrder]
    });
    
    this.variablesFormArray.push(variableGroup);
  }

  removeVariable(index: number): void {
    this.variablesFormArray.removeAt(index);
  }

  insertVariable(key: string): void {
    const content = this.form.get('content');
    if (content) {
      const currentValue = content.value || '';
      const newValue = currentValue + `{{${key}}}`;
      content.setValue(newValue);
    }
  }

  resetToCommonVariables(): void {
    if (confirm('Deseja resetar para as variáveis padrão? Isso irá remover todas as variáveis personalizadas.')) {
      this.setVariables(this.commonVariables);
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.error = 'Por favor, preencha todos os campos obrigatórios.';
      return;
    }

    this.saving = true;
    this.error = null;

    const variables: TemplateVariable[] = this.variablesFormArray.value;
    const variablesJson = JSON.stringify(variables);

    if (this.isEditMode && this.template) {
      const dto: UpdateDocumentTemplateDto = {
        name: this.form.value.name,
        description: this.form.value.description,
        content: this.form.value.content,
        variables: variablesJson
      };

      this.templateService.update(this.template.id, dto).subscribe({
        next: () => {
          this.showSuccess('Template atualizado com sucesso!');
          setTimeout(() => this.router.navigate(['/clinic-admin/document-templates']), 1500);
        },
        error: (err) => {
          this.error = 'Erro ao atualizar template: ' + (err.error?.message || err.message);
          this.saving = false;
        }
      });
    } else {
      const dto: CreateDocumentTemplateDto = {
        name: this.form.value.name,
        description: this.form.value.description,
        specialty: this.form.value.specialty,
        type: this.form.value.type,
        content: this.form.value.content,
        variables: variablesJson
      };

      this.templateService.create(dto).subscribe({
        next: () => {
          this.showSuccess('Template criado com sucesso!');
          setTimeout(() => this.router.navigate(['/clinic-admin/document-templates']), 1500);
        },
        error: (err) => {
          this.error = 'Erro ao criar template: ' + (err.error?.message || err.message);
          this.saving = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/clinic-admin/document-templates']);
  }

  private showSuccess(message: string): void {
    this.success = message;
  }
}
