import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SoapRecordService } from '../../services/soap-record.service';
import { UpdatePlanCommand, Prescription, ExamRequest, Procedure, Referral } from '../../models/soap-record.model';

@Component({
  selector: 'app-plan-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>P - Plano</mat-card-title>
        <mat-card-subtitle>Condutas e orientações</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="save()">
          
          <!-- Prescriptions -->
          <h3>Prescrições</h3>
          <div formArrayName="prescriptions">
            @for (prescription of prescriptions.controls; track prescription; let i = $index) {
              <mat-card class="item-card" [formGroupName]="i">
                <div class="item-header">
                  <h4>Medicamento {{ i + 1 }}</h4>
                  <button mat-icon-button 
                          type="button"
                          (click)="removePrescription(i)">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>

                <div class="item-form">
                  <mat-form-field class="full-width">
                    <mat-label>Nome do Medicamento</mat-label>
                    <input matInput 
                           formControlName="medicationName"
                           placeholder="Ex: Dipirona">
                  </mat-form-field>

                  <mat-form-field>
                    <mat-label>Dosagem</mat-label>
                    <input matInput 
                           formControlName="dosage"
                           placeholder="Ex: 500mg">
                  </mat-form-field>

                  <mat-form-field>
                    <mat-label>Frequência</mat-label>
                    <input matInput 
                           formControlName="frequency"
                           placeholder="Ex: 8/8h">
                  </mat-form-field>

                  <mat-form-field>
                    <mat-label>Duração</mat-label>
                    <input matInput 
                           formControlName="duration"
                           placeholder="Ex: 7 dias">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Instruções</mat-label>
                    <textarea matInput 
                              formControlName="instructions"
                              rows="2"
                              placeholder="Instruções de uso"></textarea>
                  </mat-form-field>
                </div>
              </mat-card>
            }
          </div>

          <button mat-raised-button 
                  type="button"
                  (click)="addPrescription()">
            <mat-icon>add</mat-icon>
            Adicionar Medicamento
          </button>

          <!-- Exam Requests -->
          <h3 class="section-title">Solicitação de Exames</h3>
          <div formArrayName="examRequests">
            @for (exam of examRequests.controls; track exam; let i = $index) {
              <mat-card class="item-card" [formGroupName]="i">
                <div class="item-header">
                  <h4>Exame {{ i + 1 }}</h4>
                  <button mat-icon-button 
                          type="button"
                          (click)="removeExamRequest(i)">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>

                <div class="item-form">
                  <mat-form-field class="full-width">
                    <mat-label>Nome do Exame</mat-label>
                    <input matInput 
                           formControlName="examName"
                           placeholder="Ex: Hemograma Completo">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Tipo de Exame</mat-label>
                    <input matInput 
                           formControlName="examType"
                           placeholder="Ex: Laboratorial, Imagem, etc.">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Indicação Clínica</mat-label>
                    <textarea matInput 
                              formControlName="clinicalIndication"
                              rows="2"
                              placeholder="Justificativa para o exame"></textarea>
                  </mat-form-field>

                  <mat-checkbox formControlName="isUrgent">
                    Urgente
                  </mat-checkbox>
                </div>
              </mat-card>
            }
          </div>

          <button mat-raised-button 
                  type="button"
                  (click)="addExamRequest()">
            <mat-icon>add</mat-icon>
            Adicionar Exame
          </button>

          <!-- Procedures -->
          <h3 class="section-title">Procedimentos</h3>
          <div formArrayName="procedures">
            @for (procedure of procedures.controls; track procedure; let i = $index) {
              <mat-card class="item-card" [formGroupName]="i">
                <div class="item-header">
                  <h4>Procedimento {{ i + 1 }}</h4>
                  <button mat-icon-button 
                          type="button"
                          (click)="removeProcedure(i)">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>

                <div class="item-form">
                  <mat-form-field class="full-width">
                    <mat-label>Nome do Procedimento</mat-label>
                    <input matInput 
                           formControlName="procedureName"
                           placeholder="Ex: Curetagem">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Descrição</mat-label>
                    <textarea matInput 
                              formControlName="description"
                              rows="2"
                              placeholder="Detalhes do procedimento"></textarea>
                  </mat-form-field>

                  <mat-form-field>
                    <mat-label>Data Agendada</mat-label>
                    <input matInput 
                           [matDatepicker]="picker"
                           formControlName="scheduledDate">
                    <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                    <mat-datepicker #picker></mat-datepicker>
                  </mat-form-field>
                </div>
              </mat-card>
            }
          </div>

          <button mat-raised-button 
                  type="button"
                  (click)="addProcedure()">
            <mat-icon>add</mat-icon>
            Adicionar Procedimento
          </button>

          <!-- Referrals -->
          <h3 class="section-title">Encaminhamentos</h3>
          <div formArrayName="referrals">
            @for (referral of referrals.controls; track referral; let i = $index) {
              <mat-card class="item-card" [formGroupName]="i">
                <div class="item-header">
                  <h4>Encaminhamento {{ i + 1 }}</h4>
                  <button mat-icon-button 
                          type="button"
                          (click)="removeReferral(i)">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>

                <div class="item-form">
                  <mat-form-field class="full-width">
                    <mat-label>Especialidade</mat-label>
                    <input matInput 
                           formControlName="specialtyName"
                           placeholder="Ex: Cardiologia">
                  </mat-form-field>

                  <mat-form-field class="full-width">
                    <mat-label>Motivo</mat-label>
                    <textarea matInput 
                              formControlName="reason"
                              rows="2"
                              placeholder="Motivo do encaminhamento"></textarea>
                  </mat-form-field>

                  <mat-form-field>
                    <mat-label>Prioridade</mat-label>
                    <input matInput 
                           formControlName="priority"
                           placeholder="Ex: Rotina, Urgente, Emergência">
                  </mat-form-field>
                </div>
              </mat-card>
            }
          </div>

          <button mat-raised-button 
                  type="button"
                  (click)="addReferral()">
            <mat-icon>add</mat-icon>
            Adicionar Encaminhamento
          </button>

          <!-- Patient Instructions -->
          <h3 class="section-title">Orientações ao Paciente</h3>
          <div class="instructions-section">
            <mat-form-field class="full-width">
              <mat-label>Instruções de Retorno *</mat-label>
              <textarea matInput 
                        formControlName="returnInstructions"
                        rows="2"
                        placeholder="Quando o paciente deve retornar"
                        required></textarea>
              @if (form.get('returnInstructions')?.hasError('required') && form.get('returnInstructions')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>Data do Próximo Retorno</mat-label>
              <input matInput 
                     [matDatepicker]="returnPicker"
                     formControlName="nextAppointmentDate">
              <mat-datepicker-toggle matSuffix [for]="returnPicker"></mat-datepicker-toggle>
              <mat-datepicker #returnPicker></mat-datepicker>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Orientações Gerais *</mat-label>
              <textarea matInput 
                        formControlName="patientInstructions"
                        rows="3"
                        placeholder="Orientações gerais ao paciente"
                        required></textarea>
              @if (form.get('patientInstructions')?.hasError('required') && form.get('patientInstructions')?.touched) {
                <mat-error>Campo obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Recomendações Dietéticas</mat-label>
              <textarea matInput 
                        formControlName="dietaryRecommendations"
                        rows="2"
                        placeholder="Orientações sobre alimentação"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Restrições de Atividade</mat-label>
              <textarea matInput 
                        formControlName="activityRestrictions"
                        rows="2"
                        placeholder="Restrições ou recomendações de atividade física"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Sinais de Alerta</mat-label>
              <textarea matInput 
                        formControlName="warningSymptoms"
                        rows="3"
                        placeholder="Sintomas que exigem retorno imediato"></textarea>
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
    .item-card {
      margin: 16px 0;
      background-color: #f5f5f5;
    }

    .item-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .item-header h4 {
      margin: 0;
      color: #1976d2;
    }

    .item-form {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .section-title {
      margin-top: 32px;
      margin-bottom: 16px;
      color: #1976d2;
    }

    .instructions-section {
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

    h3 {
      margin-top: 24px;
      margin-bottom: 12px;
    }
  `]
})
export class PlanFormComponent implements OnInit {
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
      prescriptions: this.fb.array([]),
      examRequests: this.fb.array([]),
      procedures: this.fb.array([]),
      referrals: this.fb.array([]),
      returnInstructions: ['', Validators.required],
      nextAppointmentDate: [null],
      patientInstructions: ['', Validators.required],
      dietaryRecommendations: [''],
      activityRestrictions: [''],
      warningSymptoms: ['']
    });
  }

  get prescriptions(): FormArray {
    return this.form.get('prescriptions') as FormArray;
  }

  get examRequests(): FormArray {
    return this.form.get('examRequests') as FormArray;
  }

  get procedures(): FormArray {
    return this.form.get('procedures') as FormArray;
  }

  get referrals(): FormArray {
    return this.form.get('referrals') as FormArray;
  }

  createPrescriptionGroup(prescription?: Prescription): FormGroup {
    return this.fb.group({
      medicationName: [prescription?.medicationName || ''],
      dosage: [prescription?.dosage || ''],
      frequency: [prescription?.frequency || ''],
      duration: [prescription?.duration || ''],
      instructions: [prescription?.instructions || '']
    });
  }

  createExamRequestGroup(exam?: ExamRequest): FormGroup {
    return this.fb.group({
      examName: [exam?.examName || ''],
      examType: [exam?.examType || ''],
      clinicalIndication: [exam?.clinicalIndication || ''],
      isUrgent: [exam?.isUrgent || false]
    });
  }

  createProcedureGroup(procedure?: Procedure): FormGroup {
    return this.fb.group({
      procedureName: [procedure?.procedureName || ''],
      description: [procedure?.description || ''],
      scheduledDate: [procedure?.scheduledDate ? new Date(procedure.scheduledDate) : null]
    });
  }

  createReferralGroup(referral?: Referral): FormGroup {
    return this.fb.group({
      specialtyName: [referral?.specialtyName || ''],
      reason: [referral?.reason || ''],
      priority: [referral?.priority || '']
    });
  }

  addPrescription(): void {
    this.prescriptions.push(this.createPrescriptionGroup());
  }

  removePrescription(index: number): void {
    this.prescriptions.removeAt(index);
  }

  addExamRequest(): void {
    this.examRequests.push(this.createExamRequestGroup());
  }

  removeExamRequest(index: number): void {
    this.examRequests.removeAt(index);
  }

  addProcedure(): void {
    this.procedures.push(this.createProcedureGroup());
  }

  removeProcedure(index: number): void {
    this.procedures.removeAt(index);
  }

  addReferral(): void {
    this.referrals.push(this.createReferralGroup());
  }

  removeReferral(index: number): void {
    this.referrals.removeAt(index);
  }

  loadData(): void {
    if (!this.soapId) return;

    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        if (record.plan) {
          const plan = record.plan;
          
          this.form.patchValue({
            returnInstructions: plan.returnInstructions,
            nextAppointmentDate: plan.nextAppointmentDate ? new Date(plan.nextAppointmentDate) : null,
            patientInstructions: plan.patientInstructions,
            dietaryRecommendations: plan.dietaryRecommendations,
            activityRestrictions: plan.activityRestrictions,
            warningSymptoms: plan.warningSymptoms
          });

          // Load arrays
          if (plan.prescriptions && plan.prescriptions.length > 0) {
            plan.prescriptions.forEach(p => {
              this.prescriptions.push(this.createPrescriptionGroup(p));
            });
          }

          if (plan.examRequests && plan.examRequests.length > 0) {
            plan.examRequests.forEach(e => {
              this.examRequests.push(this.createExamRequestGroup(e));
            });
          }

          if (plan.procedures && plan.procedures.length > 0) {
            plan.procedures.forEach(p => {
              this.procedures.push(this.createProcedureGroup(p));
            });
          }

          if (plan.referrals && plan.referrals.length > 0) {
            plan.referrals.forEach(r => {
              this.referrals.push(this.createReferralGroup(r));
            });
          }
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
    const formValue = this.form.value;
    const data: UpdatePlanCommand = {
      ...formValue,
      nextAppointmentDate: formValue.nextAppointmentDate ? formValue.nextAppointmentDate.toISOString() : null
    };

    this.soapService.updatePlan(this.soapId, data).subscribe({
      next: () => {
        this.snackBar.open('Plano salvo com sucesso!', 'Fechar', { duration: 3000 });
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
