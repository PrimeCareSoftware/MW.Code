import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SoapRecordService } from '../../services/soap-record.service';
import { UpdateAssessmentCommand, DifferentialDiagnosis } from '../../models/soap-record.model';

@Component({
  selector: 'app-assessment-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>A - Avaliação</mat-card-title>
        <mat-card-subtitle>Interpretação médica e diagnósticos</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="save()">
          
          <!-- Primary Diagnosis -->
          <h3>Diagnóstico Principal</h3>
          <div class="primary-diagnosis">
            <mat-form-field class="full-width">
              <mat-label>Diagnóstico Principal *</mat-label>
              <input matInput 
                     formControlName="primaryDiagnosis"
                     placeholder="Digite o diagnóstico principal"
                     required>
              @if (form.get('primaryDiagnosis')?.hasError('required') && form.get('primaryDiagnosis')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Código CID-10 *</mat-label>
              <input matInput 
                     formControlName="primaryDiagnosisIcd10"
                     placeholder="Ex: J00, I10, E11"
                     required>
              <mat-hint>Digite o código CID-10</mat-hint>
              @if (form.get('primaryDiagnosisIcd10')?.hasError('required') && form.get('primaryDiagnosisIcd10')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>
          </div>

          <!-- Differential Diagnoses -->
          <h3>Diagnósticos Diferenciais</h3>
          <div formArrayName="differentialDiagnoses">
            @for (diagnosis of differentialDiagnoses.controls; track diagnosis; let i = $index) {
              <mat-card class="differential-card" [formGroupName]="i">
                <div class="differential-header">
                  <h4>Diagnóstico Diferencial {{ i + 1 }}</h4>
                  <button mat-icon-button 
                          type="button"
                          (click)="removeDifferentialDiagnosis(i)"
                          [disabled]="differentialDiagnoses.length === 0">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>

                <div class="differential-form">
                  <mat-form-field>
                    <mat-label>Prioridade</mat-label>
                    <input matInput 
                           formControlName="priority"
                           type="number"
                           min="1"
                           placeholder="1">
                    <mat-hint>1 = mais provável</mat-hint>
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Diagnóstico</mat-label>
                    <input matInput 
                           formControlName="diagnosis"
                           placeholder="Digite o diagnóstico">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Código CID-10</mat-label>
                    <input matInput 
                           formControlName="icd10Code"
                           placeholder="Ex: J00">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Justificativa</mat-label>
                    <textarea matInput 
                              formControlName="justification"
                              rows="2"
                              placeholder="Por que considerar este diagnóstico?"></textarea>
                  </mat-form-field>
                </div>
              </mat-card>
            }
          </div>

          <button mat-raised-button 
                  type="button"
                  (click)="addDifferentialDiagnosis()">
            <mat-icon>add</mat-icon>
            Adicionar Diagnóstico Diferencial
          </button>

          <!-- Clinical Reasoning -->
          <h3 class="section-title">Raciocínio Clínico</h3>
          <div class="clinical-reasoning">
            <mat-form-field class="full-width">
              <mat-label>Raciocínio Clínico *</mat-label>
              <textarea matInput 
                        formControlName="clinicalReasoning"
                        rows="4"
                        placeholder="Descreva o raciocínio que levou ao diagnóstico"
                        required></textarea>
              <mat-hint>Explique como chegou ao diagnóstico com base nos achados</mat-hint>
              @if (form.get('clinicalReasoning')?.hasError('required') && form.get('clinicalReasoning')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Prognóstico</mat-label>
              <textarea matInput 
                        formControlName="prognosis"
                        rows="2"
                        placeholder="Expectativa de evolução do quadro"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Evolução do Quadro</mat-label>
              <textarea matInput 
                        formControlName="evolution"
                        rows="2"
                        placeholder="Como o quadro tem evoluído"></textarea>
            </mat-form-field>
          </div>

          <div class="actions">
            <button mat-raised-button 
                    color="primary" 
                    type="submit"
                    [disabled]="saving || form.invalid">
              {{ saving ? 'Salvando...' : 'Salvar e Avançar' }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .primary-diagnosis {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
    }

    .differential-card {
      margin: 16px 0;
      background-color: #f5f5f5;
    }

    .differential-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .differential-header h4 {
      margin: 0;
      color: #1976d2;
    }

    .differential-form {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .clinical-reasoning {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
    }

    .section-title {
      margin-top: 32px;
      margin-bottom: 16px;
      color: #1976d2;
    }

    .full-width {
      width: 100%;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      padding-top: 20px;
      gap: 12px;
    }

    mat-card {
      margin-bottom: 20px;
    }

    h3 {
      margin-top: 24px;
      margin-bottom: 12px;
    }
  `]
})
export class AssessmentFormComponent implements OnInit {
  @Input() soapId!: string;
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private soapService: SoapRecordService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadData();
  }

  initForm(): void {
    this.form = this.fb.group({
      primaryDiagnosis: ['', Validators.required],
      primaryDiagnosisIcd10: ['', Validators.required],
      differentialDiagnoses: this.fb.array([]),
      clinicalReasoning: ['', Validators.required],
      prognosis: [''],
      evolution: ['']
    });
  }

  get differentialDiagnoses(): FormArray {
    return this.form.get('differentialDiagnoses') as FormArray;
  }

  createDifferentialDiagnosisGroup(diagnosis?: DifferentialDiagnosis): FormGroup {
    return this.fb.group({
      diagnosis: [diagnosis?.diagnosis || ''],
      icd10Code: [diagnosis?.icd10Code || ''],
      justification: [diagnosis?.justification || ''],
      priority: [diagnosis?.priority || 1]
    });
  }

  addDifferentialDiagnosis(): void {
    this.differentialDiagnoses.push(this.createDifferentialDiagnosisGroup());
  }

  removeDifferentialDiagnosis(index: number): void {
    this.differentialDiagnoses.removeAt(index);
  }

  loadData(): void {
    if (!this.soapId) return;

    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        if (record.assessment) {
          const assessment = record.assessment;
          
          this.form.patchValue({
            primaryDiagnosis: assessment.primaryDiagnosis,
            primaryDiagnosisIcd10: assessment.primaryDiagnosisIcd10,
            clinicalReasoning: assessment.clinicalReasoning,
            prognosis: assessment.prognosis,
            evolution: assessment.evolution
          });

          // Load differential diagnoses
          if (assessment.differentialDiagnoses && assessment.differentialDiagnoses.length > 0) {
            assessment.differentialDiagnoses.forEach(dd => {
              this.differentialDiagnoses.push(this.createDifferentialDiagnosisGroup(dd));
            });
          }
        }
      },
      error: (error: any) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.snackBar.open('Por favor, preencha todos os campos obrigatórios', 'Fechar', { duration: 3000 });
      return;
    }

    this.saving = true;
    const data: UpdateAssessmentCommand = this.form.value;

    this.soapService.updateAssessment(this.soapId, data).subscribe({
      next: () => {
        this.snackBar.open('Avaliação salva com sucesso!', 'Fechar', { duration: 3000 });
        this.saving = false;
        this.saved.emit();
      },
      error: (error: any) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.saving = false;
      }
    });
  }
}
