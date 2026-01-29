import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ReportService } from '../../services/report.service';
import { ReportTemplate, ReportResult } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-report-wizard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NgApexchartsModule,
    Navbar
  ],
  template: `
    <app-navbar></app-navbar>
    
    <div class="page-container">
      <h1>Assistente de Relatórios</h1>

      <mat-stepper [linear]="true" #stepper>
        <mat-step [stepControl]="templateForm">
          <ng-template matStepLabel>Selecionar Template</ng-template>
          <form [formGroup]="templateForm">
            <div class="step-content">
              <mat-form-field appearance="outline">
                <mat-label>Template</mat-label>
                <mat-select formControlName="templateId" (selectionChange)="onTemplateSelected($event.value)">
                  @for (template of templates(); track template.id) {
                    <mat-option [value]="template.id">
                      {{ template.name }}
                    </mat-option>
                  }
                </mat-select>
              </mat-form-field>
              
              @if (selectedTemplate()) {
                <div class="template-info">
                  <h3>{{ selectedTemplate()?.name }}</h3>
                  <p>{{ selectedTemplate()?.description }}</p>
                </div>
              }
            </div>
            <div class="stepper-actions">
              <button mat-raised-button color="primary" matStepperNext [disabled]="!templateForm.valid">
                Próximo
              </button>
            </div>
          </form>
        </mat-step>

        <mat-step [stepControl]="parametersForm">
          <ng-template matStepLabel>Configurar Parâmetros</ng-template>
          <form [formGroup]="parametersForm">
            <div class="step-content">
              <mat-form-field appearance="outline">
                <mat-label>Data Inicial</mat-label>
                <input matInput [matDatepicker]="startPicker" formControlName="startDate">
                <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
                <mat-datepicker #startPicker></mat-datepicker>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Data Final</mat-label>
                <input matInput [matDatepicker]="endPicker" formControlName="endDate">
                <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
                <mat-datepicker #endPicker></mat-datepicker>
              </mat-form-field>
            </div>
            <div class="stepper-actions">
              <button mat-button matStepperPrevious>Voltar</button>
              <button mat-raised-button color="primary" (click)="generatePreview()" [disabled]="!parametersForm.valid">
                Gerar Preview
              </button>
            </div>
          </form>
        </mat-step>

        <mat-step>
          <ng-template matStepLabel>Preview e Exportar</ng-template>
          <div class="step-content">
            @if (generating()) {
              <div class="loading-container">
                <mat-spinner></mat-spinner>
                <p>Gerando relatório...</p>
              </div>
            } @else if (reportResult()) {
              <div class="report-preview">
                <h2>{{ reportResult()?.title }}</h2>
                <p class="generated-date">Gerado em: {{ reportResult()?.generatedAt | date:'medium' }}</p>

                @for (chart of reportResult()?.charts; track $index) {
                  <div class="chart-container">
                    <h3>{{ chart.title }}</h3>
                    <apx-chart
                      [series]="chart.series"
                      [chart]="getChartConfig(chart)"
                    ></apx-chart>
                  </div>
                }

                @if (reportResult()?.data && reportResult()!.data.length > 0) {
                  <div class="table-container">
                    <h3>Dados</h3>
                    <table mat-table [dataSource]="reportResult()!.data">
                      @for (column of getDataColumns(); track column) {
                        <ng-container [matColumnDef]="column">
                          <th mat-header-cell *matHeaderCellDef>{{ column }}</th>
                          <td mat-cell *matCellDef="let element">{{ element[column] }}</td>
                        </ng-container>
                      }
                      <tr mat-header-row *matHeaderRowDef="getDataColumns()"></tr>
                      <tr mat-row *matRowDef="let row; columns: getDataColumns()"></tr>
                    </table>
                  </div>
                }
              </div>
            }
          </div>
          <div class="stepper-actions">
            <button mat-button matStepperPrevious>Voltar</button>
            <button mat-raised-button (click)="exportReport('pdf')" [disabled]="!reportResult()">
              <mat-icon>picture_as_pdf</mat-icon>
              Exportar PDF
            </button>
            <button mat-button (click)="exportReport('excel')" [disabled]="!reportResult()">
              <mat-icon>table_chart</mat-icon>
              Exportar Excel
            </button>
          </div>
        </mat-step>
      </mat-stepper>
    </div>
  `,
  styles: [`
    .page-container {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    h1 {
      margin: 0 0 32px 0;
      font-size: 32px;
      font-weight: 600;
    }

    .step-content {
      padding: 24px 0;
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .stepper-actions {
      display: flex;
      gap: 8px;
      padding: 16px 0;
    }

    .template-info {
      padding: 16px;
      background: #f5f5f5;
      border-radius: 8px;
    }

    .template-info h3 {
      margin: 0 0 8px 0;
    }

    .template-info p {
      margin: 0;
      color: #666;
    }

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 80px 24px;
      gap: 16px;
    }

    .report-preview {
      padding: 24px 0;
    }

    .report-preview h2 {
      margin: 0 0 8px 0;
    }

    .generated-date {
      color: #666;
      margin: 0 0 32px 0;
    }

    .chart-container {
      margin: 32px 0;
    }

    .table-container {
      margin: 32px 0;
      overflow: auto;
    }

    table {
      width: 100%;
    }
  `]
})
export class ReportWizardComponent implements OnInit {
  templates = signal<ReportTemplate[]>([]);
  selectedTemplate = signal<ReportTemplate | null>(null);
  reportResult = signal<ReportResult | null>(null);
  generating = signal(false);

  templateForm: FormGroup;
  parametersForm: FormGroup;

  constructor(
    private reportService: ReportService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.templateForm = this.fb.group({
      templateId: ['', Validators.required]
    });

    this.parametersForm = this.fb.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadTemplates();
    
    const templateId = this.route.snapshot.queryParamMap.get('templateId');
    if (templateId) {
      this.templateForm.patchValue({ templateId: parseInt(templateId, 10) });
    }
  }

  loadTemplates(): void {
    this.reportService.getAvailableReports().subscribe({
      next: (data) => this.templates.set(data),
      error: (err) => console.error('Erro ao carregar templates:', err)
    });
  }

  onTemplateSelected(templateId: number): void {
    const template = this.templates().find(t => t.id === templateId);
    this.selectedTemplate.set(template || null);
  }

  generatePreview(): void {
    if (!this.parametersForm.valid) return;

    this.generating.set(true);
    const templateId = this.templateForm.value.templateId;
    const parameters = {
      startDate: this.parametersForm.value.startDate.toISOString(),
      endDate: this.parametersForm.value.endDate.toISOString()
    };

    this.reportService.generateReport(templateId, parameters).subscribe({
      next: (result) => {
        this.reportResult.set(result);
        this.generating.set(false);
      },
      error: (err) => {
        console.error('Erro ao gerar relatório:', err);
        this.generating.set(false);
      }
    });
  }

  exportReport(format: 'pdf' | 'excel'): void {
    const result = this.reportResult();
    if (!result) return;

    this.reportService.exportReport(result.id, format).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `report-${result.id}.${format}`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Erro ao exportar relatório:', err)
    });
  }

  getDataColumns(): string[] {
    const data = this.reportResult()?.data;
    if (!data || data.length === 0) return [];
    return Object.keys(data[0]);
  }

  getChartConfig(chart: any): any {
    return { type: chart.type, height: 350 };
  }
}
