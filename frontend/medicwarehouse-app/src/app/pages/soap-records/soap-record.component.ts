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
    SubjectiveFormComponent,
    ObjectiveFormComponent,
    AssessmentFormComponent,
    PlanFormComponent,
    SoapSummaryComponent
  ],
  template: `
    <div class="soap-record-container">
      @if (loading) {
        <div class="loading">Carregando prontuário...</div>
      }

      @if (!loading && soapRecord) {
        <mat-card class="header-card">
          <mat-card-header>
            <mat-card-title>Prontuário SOAP</mat-card-title>
            <mat-card-subtitle>
              Data: {{ soapRecord.recordDate | date:'dd/MM/yyyy HH:mm' }}
              @if (soapRecord.isComplete) {
                <span class="locked-badge">
                  <mat-icon>lock</mat-icon>
                  Bloqueado
                </span>
              }
            </mat-card-subtitle>
          </mat-card-header>
        </mat-card>

        @if (soapRecord.isLocked) {
          <mat-card class="locked-message">
            <mat-icon>lock</mat-icon>
            <p>Este prontuário foi concluído e bloqueado. Não é mais possível editar.</p>
            <button mat-raised-button color="primary" (click)="viewSummary()">
              Ver Resumo Completo
            </button>
          </mat-card>
        } @else {
          <mat-stepper [linear]="false" #stepper (selectionChange)="onStepChange()">
            
            <!-- Step 1: Subjective -->
            <mat-step label="Subjetivo" [completed]="soapRecord.subjective !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  @if (soapRecord.subjective) {
                    <mat-icon class="step-icon complete">check_circle</mat-icon>
                  } @else {
                    <mat-icon class="step-icon">radio_button_unchecked</mat-icon>
                  }
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
                  @if (soapRecord.objective) {
                    <mat-icon class="step-icon complete">check_circle</mat-icon>
                  } @else {
                    <mat-icon class="step-icon">radio_button_unchecked</mat-icon>
                  }
                  <span>O - Objetivo</span>
                </div>
              </ng-template>
              <app-objective-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-objective-form>
              <div class="step-actions">
                <button mat-button matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 3: Assessment -->
            <mat-step label="Avaliação" [completed]="soapRecord.assessment !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  @if (soapRecord.assessment) {
                    <mat-icon class="step-icon complete">check_circle</mat-icon>
                  } @else {
                    <mat-icon class="step-icon">radio_button_unchecked</mat-icon>
                  }
                  <span>A - Avaliação</span>
                </div>
              </ng-template>
              <app-assessment-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-assessment-form>
              <div class="step-actions">
                <button mat-button matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 4: Plan -->
            <mat-step label="Plano" [completed]="soapRecord.plan !== null">
              <ng-template matStepLabel>
                <div class="step-label">
                  @if (soapRecord.plan) {
                    <mat-icon class="step-icon complete">check_circle</mat-icon>
                  } @else {
                    <mat-icon class="step-icon">radio_button_unchecked</mat-icon>
                  }
                  <span>P - Plano</span>
                </div>
              </ng-template>
              <app-plan-form 
                [soapId]="soapId" 
                (saved)="onSectionSaved(); stepper.next()">
              </app-plan-form>
              <div class="step-actions">
                <button mat-button matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

            <!-- Step 5: Summary & Complete -->
            <mat-step label="Revisar e Concluir">
              <ng-template matStepLabel>
                <div class="step-label">
                  @if (soapRecord.isComplete) {
                    <mat-icon class="step-icon complete">check_circle</mat-icon>
                  } @else {
                    <mat-icon class="step-icon">rate_review</mat-icon>
                  }
                  <span>Revisar</span>
                </div>
              </ng-template>
              <app-soap-summary [soapId]="soapId"></app-soap-summary>
              <div class="step-actions">
                <button mat-button matStepperPrevious>Voltar</button>
              </div>
            </mat-step>

          </mat-stepper>
        }
      }

      @if (!loading && !soapRecord) {
        <mat-card class="error-card">
          <mat-icon>error</mat-icon>
          <p>Prontuário não encontrado.</p>
          <button mat-raised-button color="primary" (click)="goBack()">
            Voltar
          </button>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    .soap-record-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .loading {
      text-align: center;
      padding: 40px;
      font-size: 18px;
      color: #666;
    }

    .header-card {
      margin-bottom: 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .locked-badge {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      margin-left: 16px;
      padding: 4px 12px;
      background-color: rgba(255, 255, 255, 0.2);
      border-radius: 12px;
      font-size: 14px;
    }

    .locked-badge mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
    }

    .locked-message {
      text-align: center;
      padding: 40px;
      background-color: #fff3cd;
      border: 1px solid #ffc107;
    }

    .locked-message mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #ff9800;
    }

    .locked-message p {
      font-size: 16px;
      margin: 20px 0;
    }

    .error-card {
      text-align: center;
      padding: 40px;
    }

    .error-card mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #f44336;
    }

    .step-label {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .step-icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .step-icon.complete {
      color: #4caf50;
    }

    .step-actions {
      display: flex;
      justify-content: space-between;
      padding-top: 20px;
      margin-top: 20px;
      border-top: 1px solid #e0e0e0;
    }

    :host ::ng-deep .mat-stepper-horizontal {
      background-color: transparent;
    }

    :host ::ng-deep .mat-step-header {
      padding: 16px;
    }

    :host ::ng-deep .mat-step-header.cdk-keyboard-focused,
    :host ::ng-deep .mat-step-header.cdk-program-focused,
    :host ::ng-deep .mat-step-header:hover {
      background-color: rgba(0, 0, 0, 0.04);
    }
  `]
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
      error: (error) => {
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
      error: (error) => {
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
