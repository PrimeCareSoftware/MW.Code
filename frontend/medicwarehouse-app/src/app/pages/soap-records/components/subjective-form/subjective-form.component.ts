import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SoapRecordService } from '../../services/soap-record.service';
import { UpdateSubjectiveCommand } from '../../models/soap-record.model';

@Component({
  selector: 'app-subjective-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>S - Dados Subjetivos</mat-card-title>
        <mat-card-subtitle>Informações relatadas pelo paciente</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="save()">
          <div class="form-container">
            <mat-form-field class="full-width">
              <mat-label>Queixa Principal *</mat-label>
              <textarea matInput 
                        formControlName="chiefComplaint" 
                        rows="2"
                        placeholder="Principal motivo da consulta"
                        required></textarea>
              @if (form.get('chiefComplaint')?.hasError('required') && form.get('chiefComplaint')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>História da Doença Atual (HDA) *</mat-label>
              <textarea matInput 
                        formControlName="historyOfPresentIllness" 
                        rows="4"
                        placeholder="Descreva o início, evolução e características dos sintomas"
                        required></textarea>
              @if (form.get('historyOfPresentIllness')?.hasError('required') && form.get('historyOfPresentIllness')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Sintomas Atuais</mat-label>
              <textarea matInput 
                        formControlName="currentSymptoms" 
                        rows="3"
                        placeholder="Liste os sintomas atuais"></textarea>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Duração dos Sintomas</mat-label>
              <input matInput 
                     formControlName="symptomDuration"
                     placeholder="Ex: 3 dias, 1 semana">
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Fatores de Piora</mat-label>
              <textarea matInput 
                        formControlName="aggravatingFactors" 
                        rows="2"
                        placeholder="O que piora os sintomas?"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Fatores de Melhora</mat-label>
              <textarea matInput 
                        formControlName="relievingFactors" 
                        rows="2"
                        placeholder="O que melhora os sintomas?"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Revisão de Sistemas</mat-label>
              <textarea matInput 
                        formControlName="reviewOfSystems" 
                        rows="3"
                        placeholder="Revisão por sistemas (cardiovascular, respiratório, etc.)"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Alergias *</mat-label>
              <textarea matInput 
                        formControlName="allergies" 
                        rows="2"
                        placeholder="Liste alergias conhecidas ou 'Nega alergias'"
                        required></textarea>
              @if (form.get('allergies')?.hasError('required') && form.get('allergies')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Medicamentos em Uso *</mat-label>
              <textarea matInput 
                        formControlName="currentMedications" 
                        rows="2"
                        placeholder="Liste medicamentos atuais ou 'Não faz uso de medicamentos'"
                        required></textarea>
              @if (form.get('currentMedications')?.hasError('required') && form.get('currentMedications')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>História Médica Pregressa</mat-label>
              <textarea matInput 
                        formControlName="pastMedicalHistory" 
                        rows="3"
                        placeholder="Doenças anteriores, cirurgias, hospitalizações"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>História Familiar</mat-label>
              <textarea matInput 
                        formControlName="familyHistory" 
                        rows="2"
                        placeholder="Doenças na família"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>História Social (Hábitos)</mat-label>
              <textarea matInput 
                        formControlName="socialHistory" 
                        rows="2"
                        placeholder="Tabagismo, etilismo, atividade física, etc."></textarea>
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
    .form-container {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
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
  `]
})
export class SubjectiveFormComponent implements OnInit {
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
      chiefComplaint: ['', Validators.required],
      historyOfPresentIllness: ['', Validators.required],
      currentSymptoms: [''],
      symptomDuration: [''],
      aggravatingFactors: [''],
      relievingFactors: [''],
      reviewOfSystems: [''],
      allergies: ['', Validators.required],
      currentMedications: ['', Validators.required],
      pastMedicalHistory: [''],
      familyHistory: [''],
      socialHistory: ['']
    });
  }

  loadData(): void {
    if (!this.soapId) return;

    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        if (record.subjective) {
          this.form.patchValue(record.subjective);
        }
      },
      error: (error) => {
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
    const data: UpdateSubjectiveCommand = this.form.value;

    this.soapService.updateSubjective(this.soapId, data).subscribe({
      next: () => {
        this.snackBar.open('Dados subjetivos salvos com sucesso!', 'Fechar', { duration: 3000 });
        this.saving = false;
        this.saved.emit();
      },
      error: (error) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.saving = false;
      }
    });
  }
}
