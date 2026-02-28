import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatStepperModule, MatStepper } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SubjectiveFormComponent } from './components/subjective-form/subjective-form.component';
import { ObjectiveFormComponent } from './components/objective-form/objective-form.component';
import { AssessmentFormComponent } from './components/assessment-form/assessment-form.component';
import { PlanFormComponent } from './components/plan-form/plan-form.component';
import { SoapSummaryComponent } from './components/soap-summary/soap-summary.component';
import { SoapRecordService } from './services/soap-record.service';
import { SoapRecord } from './models/soap-record.model';
import { HelpButtonComponent } from '../../shared/help-button/help-button';

@Component({
  selector: 'app-soap-record',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatStepperModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    HelpButtonComponent,
    SubjectiveFormComponent,
    ObjectiveFormComponent,
    AssessmentFormComponent,
    PlanFormComponent,
    SoapSummaryComponent,
  ],
  styleUrls: ['./soap-record.component.scss'],
  template: `
    
    <div class="soap-record-container">
      <div class="page-header">
        <div class="header-content">
          <h1>Prontuário SOAP</h1>
          <p>Sistema de Registro Médico - Subjetivo, Objetivo, Avaliação e Plano</p>
        </div>
      </div>

      @if (loading) {
        <div class="loading-state">
          <span class="spinner"></span>
          <p>Carregando prontuário...</p>
        </div>
      }

      @if (!loading && soapRecord) {
        <!-- Record Info -->
        <div class="form-section">
          <div class="section-header">
            <div class="section-icon">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                <polyline points="14 2 14 8 20 8"/>
                <line x1="16" y1="13" x2="8" y2="13"/>
                <line x1="16" y1="17" x2="8" y2="17"/>
              </svg>
            </div>
            <div>
              <h3>Informações do Prontuário</h3>
              <p>Data: {{ soapRecord.recordDate | date:'dd/MM/yyyy HH:mm' }}</p>
            </div>
            @if (soapRecord.isComplete) {
              <span class="badge badge-success ml-auto">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                </svg>
                Bloqueado
              </span>
            }
          </div>
        </div>

        @if (soapRecord.isLocked) {
          <div class="alert alert-info">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
              <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
            </svg>
            <div>
              <p>Este prontuário foi concluído e bloqueado. Não é mais possível editar.</p>
              <button type="button" class="btn btn-primary" (click)="viewSummary()">
                Ver Resumo Completo
              </button>
            </div>
          </div>
        } @else {
          <mat-stepper [linear]="false" #stepper (selectionChange)="onStepChange()">
            
            <!-- Step 1: Subjective -->
            <mat-step label="Subjetivo" [completed]="soapRecord.subjective !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    @if (soapRecord.subjective) {
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                      <polyline points="22 4 12 14.01 9 11.01"/>
                    } @else {
                      <circle cx="12" cy="12" r="10"/>
                    }
                  </svg>
                  <span>S - Subjetivo</span>
                </div>
              </ng-template>
              <app-subjective-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-subjective-form>
            </mat-step>

            <!-- Step 2: Objective -->
            <mat-step label="Objetivo" [completed]="soapRecord.objective !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    @if (soapRecord.objective) {
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                      <polyline points="22 4 12 14.01 9 11.01"/>
                    } @else {
                      <circle cx="12" cy="12" r="10"/>
                    }
                  </svg>
                  <span>O - Objetivo</span>
                </div>
              </ng-template>
              <app-objective-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-objective-form>
              <div class="step-actions">
                <button type="button" class="btn btn-secondary" matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 3: Assessment -->
            <mat-step label="Avaliação" [completed]="soapRecord.assessment !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    @if (soapRecord.assessment) {
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                      <polyline points="22 4 12 14.01 9 11.01"/>
                    } @else {
                      <circle cx="12" cy="12" r="10"/>
                    }
                  </svg>
                  <span>A - Avaliação</span>
                </div>
              </ng-template>
              <app-assessment-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-assessment-form>
              <div class="step-actions">
                <button type="button" class="btn btn-secondary" matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 4: Plan -->
            <mat-step label="Plano" [completed]="soapRecord.plan !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    @if (soapRecord.plan) {
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                      <polyline points="22 4 12 14.01 9 11.01"/>
                    } @else {
                      <circle cx="12" cy="12" r="10"/>
                    }
                  </svg>
                  <span>P - Plano</span>
                </div>
              </ng-template>
              <app-plan-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-plan-form>
              <div class="step-actions">
                <button type="button" class="btn btn-secondary" matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 5: Summary & Complete -->
            <mat-step label="Revisar e Concluir">
              <ng-template matStepLabel>
                <div class="step-label">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    @if (soapRecord.isComplete) {
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                      <polyline points="22 4 12 14.01 9 11.01"/>
                    } @else {
                      <path d="M9 11l3 3L22 4"/>
                      <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/>
                    }
                  </svg>
                  <span>Revisar</span>
                </div>
              </ng-template>
              <app-soap-summary [soapId]="soapId"></app-soap-summary>
              <div class="step-actions">
                <button type="button" class="btn btn-secondary" matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

          </mat-stepper>
        }
      }

      @if (!loading && !soapRecord) {
        <div class="empty-state">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"/>
            <line x1="12" y1="8" x2="12" y2="12"/>
            <line x1="12" y1="16" x2="12.01" y2="16"/>
          </svg>
          <h3>Prontuário não encontrado</h3>
          <p>O prontuário solicitado não foi encontrado ou não existe.</p>
          <button type="button" class="btn btn-primary" (click)="goBack()">
            Voltar
          </button>
        </div>
      }
    </div>
    
    <app-help-button module="soap-records"></app-help-button>
  `
})
export class SoapRecordComponent implements OnInit {
  @ViewChild('stepper') stepper!: MatStepper;

  soapId: string = '';
  soapRecord: SoapRecord | null = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private soapService: SoapRecordService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      const attendanceId = params['attendanceId'];

      if (id) {
        this.soapId = id;
        this.loadSoapRecord();
      } else if (attendanceId) {
        this.createSoapRecord(attendanceId);
      } else {
        this.snackBar.open('ID inválido', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  createSoapRecord(attendanceId: string): void {
    this.loading = true;
    this.soapService.createSoapRecord(attendanceId).subscribe({
      next: (record) => {
        this.soapRecord = record;
        this.soapId = record.id;
        this.loading = false;
        this.snackBar.open('Prontuário SOAP criado!', 'Fechar', { duration: 3000 });
      },
      error: (error: any) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.loading = false;
      }
    });
  }

  loadSoapRecord(): void {
    this.loading = true;
    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        this.soapRecord = record;
        this.loading = false;
      },
      error: (error: any) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
        this.loading = false;
      }
    });
  }

  onSectionSaved(): void {
    // Reload the SOAP record to update completion status
    this.loadSoapRecord();
  }

  onStepChange(): void {
    // Reload when changing steps to ensure data is fresh
    if (this.soapId) {
      this.loadSoapRecord();
    }
  }

  viewSummary(): void {
    if (this.stepper) {
      this.stepper.selectedIndex = 4; // Go to summary step
    }
  }

  goBack(): void {
    this.router.navigate(['/soap-records']);
  }
}
