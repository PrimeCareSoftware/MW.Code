import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AnamnesisService } from '../../../services/anamnesis.service';
import { MedicalSpecialty, SPECIALTY_NAMES, QuestionType } from '../../../models/anamnesis.model';

@Component({
  selector: 'app-template-form',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './template-form.html',
  styleUrl: './template-form.scss'
})
export class TemplateFormComponent implements OnInit {
  templateForm: FormGroup;
  templateId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  specialties = Object.keys(MedicalSpecialty).filter(k => isNaN(Number(k))).map(key => ({
    value: MedicalSpecialty[key as keyof typeof MedicalSpecialty],
    label: SPECIALTY_NAMES[MedicalSpecialty[key as keyof typeof MedicalSpecialty]]
  }));

  questionTypes = Object.keys(QuestionType).filter(k => isNaN(Number(k))).map(key => ({
    value: QuestionType[key as keyof typeof QuestionType],
    label: key
  }));

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private anamnesisService: AnamnesisService
  ) {
    this.templateForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      specialty: [MedicalSpecialty.GeneralMedicine, Validators.required],
      description: [''],
      isDefault: [false],
      sections: this.fb.array([])
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.templateId.set(id);
      this.loadTemplate(id);
    } else {
      // Add one empty section by default for new templates
      this.addSection();
    }
  }

  get sections(): FormArray {
    return this.templateForm.get('sections') as FormArray;
  }

  getQuestions(sectionIndex: number): FormArray {
    return this.sections.at(sectionIndex).get('questions') as FormArray;
  }

  loadTemplate(id: string): void {
    this.isLoading.set(true);
    this.anamnesisService.getTemplateById(id).subscribe({
      next: (template) => {
        this.templateForm.patchValue({
          name: template.name,
          specialty: template.specialty,
          description: template.description,
          isDefault: template.isDefault
        });

        // Clear and rebuild sections
        this.sections.clear();
        template.sections.forEach(section => {
          const sectionGroup = this.createSectionGroup();
          sectionGroup.patchValue({
            sectionName: section.sectionName,
            order: section.order
          });
          
          const questionsArray = sectionGroup.get('questions') as FormArray;
          section.questions.forEach(question => {
            questionsArray.push(this.fb.group({
              questionText: [question.questionText, Validators.required],
              type: [question.type, Validators.required],
              isRequired: [question.isRequired],
              options: [question.options?.join(', ') || ''],
              order: [question.order],
              helpText: [question.helpText || '']
            }));
          });

          this.sections.push(sectionGroup);
        });

        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading template:', error);
        this.errorMessage.set('Erro ao carregar template');
        this.isLoading.set(false);
      }
    });
  }

  createSectionGroup(): FormGroup {
    return this.fb.group({
      sectionName: ['', Validators.required],
      order: [0],
      questions: this.fb.array([])
    });
  }

  addSection(): void {
    const section = this.createSectionGroup();
    section.patchValue({ order: this.sections.length });
    this.sections.push(section);
    // Add one empty question to new section
    this.addQuestion(this.sections.length - 1);
  }

  removeSection(index: number): void {
    this.sections.removeAt(index);
  }

  addQuestion(sectionIndex: number): void {
    const questions = this.getQuestions(sectionIndex);
    questions.push(this.fb.group({
      questionText: ['', Validators.required],
      type: [QuestionType.Text, Validators.required],
      isRequired: [false],
      options: [''],
      order: [questions.length],
      helpText: ['']
    }));
  }

  removeQuestion(sectionIndex: number, questionIndex: number): void {
    const questions = this.getQuestions(sectionIndex);
    questions.removeAt(questionIndex);
  }

  onSubmit(): void {
    if (this.templateForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios corretamente.');
      Object.keys(this.templateForm.controls).forEach(key => {
        const control = this.templateForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.templateForm.value;
    
    // Process sections to convert options string to array
    const sections = formValue.sections.map((section: any) => ({
      ...section,
      questions: section.questions.map((q: any) => ({
        ...q,
        options: q.options ? q.options.split(',').map((opt: string) => opt.trim()).filter((opt: string) => opt) : []
      }))
    }));

    const request = {
      name: formValue.name,
      specialty: formValue.specialty,
      description: formValue.description,
      isDefault: formValue.isDefault,
      sections: sections
    };
    
    if (this.templateId()) {
      // Update existing template
      this.anamnesisService.updateTemplate(this.templateId()!, request).subscribe({
        next: () => {
          this.successMessage.set('Template atualizado com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => {
            this.router.navigate(['/anamnesis/templates/manage']);
          }, 1500);
        },
        error: (error) => {
          console.error('Error updating template:', error);
          this.errorMessage.set('Erro ao atualizar template');
          this.isSaving.set(false);
        }
      });
    } else {
      // Create new template
      this.anamnesisService.createTemplate(request).subscribe({
        next: () => {
          this.successMessage.set('Template criado com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => {
            this.router.navigate(['/anamnesis/templates/manage']);
          }, 1500);
        },
        error: (error) => {
          console.error('Error creating template:', error);
          this.errorMessage.set('Erro ao criar template');
          this.isSaving.set(false);
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/anamnesis/templates/manage']);
  }
}
