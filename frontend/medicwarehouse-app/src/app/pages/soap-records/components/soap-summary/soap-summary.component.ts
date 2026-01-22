import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { SoapRecordService } from '../../services/soap-record.service';
import { SoapRecord } from '../../models/soap-record.model';

@Component({
  selector: 'app-soap-summary',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatSnackBarModule
  ],
  template: `
    @if (loading) {
      <div class="loading">Carregando...</div>
    }

    @if (soapRecord && !loading) {
      <div class="summary-container">
        <mat-card class="status-card">
          <mat-card-header>
            <mat-card-title>Status do Prontuário SOAP</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="status-chips">
              <mat-chip [class.complete]="soapRecord.subjective !== null">
                <mat-icon>{{ soapRecord.subjective ? 'check_circle' : 'circle' }}</mat-icon>
                Subjetivo
              </mat-chip>
              <mat-chip [class.complete]="soapRecord.objective !== null">
                <mat-icon>{{ soapRecord.objective ? 'check_circle' : 'circle' }}</mat-icon>
                Objetivo
              </mat-chip>
              <mat-chip [class.complete]="soapRecord.assessment !== null">
                <mat-icon>{{ soapRecord.assessment ? 'check_circle' : 'circle' }}</mat-icon>
                Avaliação
              </mat-chip>
              <mat-chip [class.complete]="soapRecord.plan !== null">
                <mat-icon>{{ soapRecord.plan ? 'check_circle' : 'circle' }}</mat-icon>
                Plano
              </mat-chip>
            </div>

            @if (soapRecord.isComplete) {
              <div class="completion-status">
                <mat-icon color="primary">lock</mat-icon>
                <span>Prontuário concluído e bloqueado em {{ soapRecord.completionDate | date:'dd/MM/yyyy HH:mm' }}</span>
              </div>
            }
          </mat-card-content>
        </mat-card>

        <!-- Subjective Summary -->
        @if (soapRecord.subjective) {
          <mat-card>
            <mat-card-header>
              <mat-card-title>S - Subjetivo</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="summary-item">
                <strong>Queixa Principal:</strong>
                <p>{{ soapRecord.subjective.chiefComplaint }}</p>
              </div>
              <div class="summary-item">
                <strong>História da Doença Atual:</strong>
                <p>{{ soapRecord.subjective.historyOfPresentIllness }}</p>
              </div>
              @if (soapRecord.subjective.currentSymptoms) {
                <div class="summary-item">
                  <strong>Sintomas Atuais:</strong>
                  <p>{{ soapRecord.subjective.currentSymptoms }}</p>
                </div>
              }
              <div class="summary-item">
                <strong>Alergias:</strong>
                <p>{{ soapRecord.subjective.allergies }}</p>
              </div>
              <div class="summary-item">
                <strong>Medicamentos em Uso:</strong>
                <p>{{ soapRecord.subjective.currentMedications }}</p>
              </div>
            </mat-card-content>
          </mat-card>
        }

        <!-- Objective Summary -->
        @if (soapRecord.objective) {
          <mat-card>
            <mat-card-header>
              <mat-card-title>O - Objetivo</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="summary-item">
                <strong>Sinais Vitais:</strong>
                <div class="vital-signs">
                  @if (soapRecord.objective.vitalSigns.systolicBP) {
                    <span>PA: {{ soapRecord.objective.vitalSigns.systolicBP }}/{{ soapRecord.objective.vitalSigns.diastolicBP }} mmHg</span>
                  }
                  @if (soapRecord.objective.vitalSigns.heartRate) {
                    <span>FC: {{ soapRecord.objective.vitalSigns.heartRate }} bpm</span>
                  }
                  @if (soapRecord.objective.vitalSigns.temperature) {
                    <span>Temp: {{ soapRecord.objective.vitalSigns.temperature }}°C</span>
                  }
                  @if (soapRecord.objective.vitalSigns.oxygenSaturation) {
                    <span>SpO2: {{ soapRecord.objective.vitalSigns.oxygenSaturation }}%</span>
                  }
                  @if (soapRecord.objective.vitalSigns.weight) {
                    <span>Peso: {{ soapRecord.objective.vitalSigns.weight }} kg</span>
                  }
                  @if (soapRecord.objective.vitalSigns.height) {
                    <span>Altura: {{ soapRecord.objective.vitalSigns.height }} cm</span>
                  }
                  @if (soapRecord.objective.vitalSigns.bmi) {
                    <span>IMC: {{ soapRecord.objective.vitalSigns.bmi | number:'1.1-1' }}</span>
                  }
                </div>
              </div>
              @if (hasPhysicalExam()) {
                <div class="summary-item">
                  <strong>Exame Físico:</strong>
                  <div class="physical-exam">
                    @if (soapRecord.objective.physicalExam.generalAppearance) {
                      <p><strong>Aparência Geral:</strong> {{ soapRecord.objective.physicalExam.generalAppearance }}</p>
                    }
                    @if (soapRecord.objective.physicalExam.cardiovascular) {
                      <p><strong>Cardiovascular:</strong> {{ soapRecord.objective.physicalExam.cardiovascular }}</p>
                    }
                    @if (soapRecord.objective.physicalExam.respiratory) {
                      <p><strong>Respiratório:</strong> {{ soapRecord.objective.physicalExam.respiratory }}</p>
                    }
                  </div>
                </div>
              }
            </mat-card-content>
          </mat-card>
        }

        <!-- Assessment Summary -->
        @if (soapRecord.assessment) {
          <mat-card>
            <mat-card-header>
              <mat-card-title>A - Avaliação</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="summary-item">
                <strong>Diagnóstico Principal:</strong>
                <p>{{ soapRecord.assessment.primaryDiagnosis }} ({{ soapRecord.assessment.primaryDiagnosisIcd10 }})</p>
              </div>
              @if (soapRecord.assessment.differentialDiagnoses.length > 0) {
                <div class="summary-item">
                  <strong>Diagnósticos Diferenciais:</strong>
                  <ul>
                    @for (dd of soapRecord.assessment.differentialDiagnoses; track dd) {
                      <li>{{ dd.diagnosis }} ({{ dd.icd10Code }}) - Prioridade: {{ dd.priority }}</li>
                    }
                  </ul>
                </div>
              }
              <div class="summary-item">
                <strong>Raciocínio Clínico:</strong>
                <p>{{ soapRecord.assessment.clinicalReasoning }}</p>
              </div>
            </mat-card-content>
          </mat-card>
        }

        <!-- Plan Summary -->
        @if (soapRecord.plan) {
          <mat-card>
            <mat-card-header>
              <mat-card-title>P - Plano</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (soapRecord.plan.prescriptions.length > 0) {
                <div class="summary-item">
                  <strong>Prescrições:</strong>
                  <ul>
                    @for (rx of soapRecord.plan.prescriptions; track rx) {
                      <li>
                        {{ rx.medicationName }} - {{ rx.dosage }} - {{ rx.frequency }} - {{ rx.duration }}
                        @if (rx.instructions) {
                          <br><em>{{ rx.instructions }}</em>
                        }
                      </li>
                    }
                  </ul>
                </div>
              }
              @if (soapRecord.plan.examRequests.length > 0) {
                <div class="summary-item">
                  <strong>Exames Solicitados:</strong>
                  <ul>
                    @for (exam of soapRecord.plan.examRequests; track exam) {
                      <li>
                        {{ exam.examName }} ({{ exam.examType }})
                        @if (exam.isUrgent) {
                          <mat-chip class="urgent-chip">URGENTE</mat-chip>
                        }
                      </li>
                    }
                  </ul>
                </div>
              }
              <div class="summary-item">
                <strong>Orientações ao Paciente:</strong>
                <p>{{ soapRecord.plan.patientInstructions }}</p>
              </div>
              <div class="summary-item">
                <strong>Retorno:</strong>
                <p>{{ soapRecord.plan.returnInstructions }}</p>
              </div>
            </mat-card-content>
          </mat-card>
        }

        <div class="actions">
          @if (!soapRecord.isComplete) {
            <button mat-raised-button 
                    color="primary"
                    (click)="completeSoap()"
                    [disabled]="completing || !canComplete()">
              <mat-icon>lock</mat-icon>
              {{ completing ? 'Concluindo...' : 'Concluir e Bloquear Prontuário' }}
            </button>
          }
          <button mat-stroked-button (click)="goBack()">
            Voltar
          </button>
        </div>
      </div>
    }
  `,
  styles: [`
    .loading {
      text-align: center;
      padding: 40px;
      font-size: 18px;
      color: #666;
    }

    .summary-container {
      display: flex;
      flex-direction: column;
      gap: 20px;
      padding: 20px;
    }

    .status-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .status-chips {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
      margin: 20px 0;
    }

    .status-chips mat-chip {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 16px;
      background-color: rgba(255, 255, 255, 0.2);
      color: white;
    }

    .status-chips mat-chip.complete {
      background-color: rgba(76, 175, 80, 0.8);
    }

    .completion-status {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-top: 20px;
      padding: 12px;
      background-color: rgba(255, 255, 255, 0.2);
      border-radius: 4px;
    }

    .summary-item {
      margin-bottom: 20px;
    }

    .summary-item strong {
      display: block;
      color: #1976d2;
      margin-bottom: 8px;
    }

    .summary-item p {
      margin: 0;
      line-height: 1.6;
      white-space: pre-wrap;
    }

    .vital-signs {
      display: flex;
      flex-wrap: wrap;
      gap: 16px;
    }

    .vital-signs span {
      padding: 8px 12px;
      background-color: #f5f5f5;
      border-radius: 4px;
      font-weight: 500;
    }

    .physical-exam p {
      margin: 8px 0;
    }

    .urgent-chip {
      background-color: #f44336 !important;
      color: white !important;
      font-size: 10px;
      padding: 2px 8px;
      margin-left: 8px;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      padding-top: 20px;
    }

    mat-card {
      margin-bottom: 20px;
    }

    ul {
      margin: 8px 0;
      padding-left: 24px;
    }

    li {
      margin: 8px 0;
      line-height: 1.6;
    }
  `]
})
export class SoapSummaryComponent implements OnInit {
  @Input() soapId!: string;

  soapRecord: SoapRecord | null = null;
  loading = true;
  completing = false;

  constructor(
    private soapService: SoapRecordService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSoapRecord();
  }

  loadSoapRecord(): void {
    this.loading = true;
    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        this.soapRecord = record;
        this.loading = false;
      },
      error: (error) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.loading = false;
      }
    });
  }

  canComplete(): boolean {
    if (!this.soapRecord) return false;
    return this.soapRecord.subjective !== null &&
           this.soapRecord.objective !== null &&
           this.soapRecord.assessment !== null &&
           this.soapRecord.plan !== null;
  }

  hasPhysicalExam(): boolean {
    if (!this.soapRecord?.objective?.physicalExam) return false;
    const exam = this.soapRecord.objective.physicalExam;
    return !!(exam.generalAppearance || exam.cardiovascular || exam.respiratory || 
              exam.abdomen || exam.neurological || exam.musculoskeletal || exam.skin);
  }

  completeSoap(): void {
    if (!this.canComplete()) {
      this.snackBar.open('Complete todas as seções antes de finalizar', 'Fechar', { duration: 3000 });
      return;
    }

    this.completing = true;
    this.soapService.completeSoapRecord(this.soapId).subscribe({
      next: (record) => {
        this.soapRecord = record;
        this.snackBar.open('Prontuário SOAP concluído e bloqueado com sucesso!', 'Fechar', { duration: 3000 });
        this.completing = false;
      },
      error: (error) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.completing = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/soap-records']);
  }
}
