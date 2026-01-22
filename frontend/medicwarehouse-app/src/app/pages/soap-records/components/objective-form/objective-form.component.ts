import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SoapRecordService } from '../../services/soap-record.service';
import { UpdateObjectiveCommand } from '../../models/soap-record.model';

@Component({
  selector: 'app-objective-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatExpansionModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>O - Dados Objetivos</mat-card-title>
        <mat-card-subtitle>Dados coletados pelo médico</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="save()">
          
          <!-- Vital Signs Section -->
          <h3>Sinais Vitais</h3>
          <div class="vital-signs-grid">
            <mat-form-field>
              <mat-label>PA Sistólica</mat-label>
              <input matInput 
                     formControlName="systolicBP" 
                     type="number"
                     placeholder="120">
              <span matSuffix>mmHg</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>PA Diastólica</mat-label>
              <input matInput 
                     formControlName="diastolicBP" 
                     type="number"
                     placeholder="80">
              <span matSuffix>mmHg</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Frequência Cardíaca</mat-label>
              <input matInput 
                     formControlName="heartRate" 
                     type="number"
                     placeholder="72">
              <span matSuffix>bpm</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Frequência Respiratória</mat-label>
              <input matInput 
                     formControlName="respiratoryRate" 
                     type="number"
                     placeholder="16">
              <span matSuffix>rpm</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Temperatura</mat-label>
              <input matInput 
                     formControlName="temperature" 
                     type="number" 
                     step="0.1"
                     placeholder="36.5">
              <span matSuffix>°C</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>SpO2</mat-label>
              <input matInput 
                     formControlName="oxygenSaturation" 
                     type="number"
                     placeholder="98">
              <span matSuffix>%</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Peso</mat-label>
              <input matInput 
                     formControlName="weight" 
                     type="number" 
                     step="0.1"
                     placeholder="70"
                     (input)="calculateBMI()">
              <span matSuffix>kg</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Altura</mat-label>
              <input matInput 
                     formControlName="height" 
                     type="number"
                     placeholder="170"
                     (input)="calculateBMI()">
              <span matSuffix>cm</span>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Escala de Dor (0-10)</mat-label>
              <input matInput 
                     formControlName="pain" 
                     type="number"
                     min="0"
                     max="10"
                     placeholder="0">
            </mat-form-field>

            <div class="bmi-display">
              <strong>IMC:</strong> 
              <span class="bmi-value">{{ getBMI() }}</span>
              <span class="bmi-classification">{{ getBMIClassification() }}</span>
            </div>
          </div>

          <!-- Physical Examination Section -->
          <h3 class="section-title">Exame Físico</h3>
          <mat-accordion>
            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Aparência Geral</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="generalAppearance" 
                          rows="2"
                          placeholder="Estado geral, nível de consciência, hidratação"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Cabeça</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="head" 
                          rows="2"
                          placeholder="Inspeção e palpação da cabeça"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Olhos</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="eyes" 
                          rows="2"
                          placeholder="Pupilas, conjuntivas, escleras"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Ouvidos</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="ears" 
                          rows="2"
                          placeholder="Otoscopia"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Nariz</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="nose" 
                          rows="2"
                          placeholder="Inspeção nasal"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Garganta</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="throat" 
                          rows="2"
                          placeholder="Orofaringe, amígdalas"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Pescoço</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="neck" 
                          rows="2"
                          placeholder="Tireoide, linfonodos, jugulares"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Cardiovascular</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="cardiovascular" 
                          rows="2"
                          placeholder="Bulhas, sopros, pulsos periféricos"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Respiratório</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="respiratory" 
                          rows="2"
                          placeholder="Inspeção, palpação, percussão, ausculta"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Abdome</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="abdomen" 
                          rows="2"
                          placeholder="Inspeção, ausculta, percussão, palpação"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Musculoesquelético</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="musculoskeletal" 
                          rows="2"
                          placeholder="Articulações, amplitude de movimento, força muscular"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Neurológico</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="neurological" 
                          rows="2"
                          placeholder="Consciência, nervos cranianos, motricidade, sensibilidade, reflexos"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Pele</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="skin" 
                          rows="2"
                          placeholder="Cor, turgor, lesões, erupções"></textarea>
              </mat-form-field>
            </mat-expansion-panel>

            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>Outros Achados</mat-panel-title>
              </mat-expansion-panel-header>
              <mat-form-field class="full-width">
                <textarea matInput 
                          formControlName="otherFindings" 
                          rows="2"
                          placeholder="Achados adicionais relevantes"></textarea>
              </mat-form-field>
            </mat-expansion-panel>
          </mat-accordion>

          <!-- Lab Results Section -->
          <h3 class="section-title">Resultados de Exames</h3>
          <div class="exam-results">
            <mat-form-field class="full-width">
              <mat-label>Resultados Laboratoriais</mat-label>
              <textarea matInput 
                        formControlName="labResults" 
                        rows="3"
                        placeholder="Resultados de exames de laboratório"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Resultados de Imagem</mat-label>
              <textarea matInput 
                        formControlName="imagingResults" 
                        rows="3"
                        placeholder="Resultados de exames de imagem (RX, TC, RM, US)"></textarea>
            </mat-form-field>

            <mat-form-field class="full-width">
              <mat-label>Outros Exames</mat-label>
              <textarea matInput 
                        formControlName="otherExamResults" 
                        rows="2"
                        placeholder="ECG, espirometria, outros exames complementares"></textarea>
            </mat-form-field>
          </div>

          <div class="actions">
            <button mat-raised-button 
                    color="primary" 
                    type="submit"
                    [disabled]="saving">
              {{ saving ? 'Salvando...' : 'Salvar e Avançar' }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .vital-signs-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
      padding: 20px 0;
    }

    .bmi-display {
      display: flex;
      flex-direction: column;
      justify-content: center;
      padding: 16px;
      background-color: #f5f5f5;
      border-radius: 4px;
    }

    .bmi-value {
      font-size: 24px;
      font-weight: bold;
      color: #1976d2;
      margin-left: 8px;
    }

    .bmi-classification {
      font-size: 12px;
      color: #666;
      margin-top: 4px;
    }

    .section-title {
      margin-top: 32px;
      margin-bottom: 16px;
      color: #1976d2;
    }

    .exam-results {
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

    mat-accordion {
      margin-bottom: 20px;
    }
  `]
})
export class ObjectiveFormComponent implements OnInit {
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
      // Vital Signs
      systolicBP: [null],
      diastolicBP: [null],
      heartRate: [null],
      respiratoryRate: [null],
      temperature: [null],
      oxygenSaturation: [null],
      weight: [null],
      height: [null],
      pain: [null],
      // Physical Examination
      generalAppearance: [''],
      head: [''],
      eyes: [''],
      ears: [''],
      nose: [''],
      throat: [''],
      neck: [''],
      cardiovascular: [''],
      respiratory: [''],
      abdomen: [''],
      musculoskeletal: [''],
      neurological: [''],
      skin: [''],
      otherFindings: [''],
      // Exam Results
      labResults: [''],
      imagingResults: [''],
      otherExamResults: ['']
    });
  }

  loadData(): void {
    if (!this.soapId) return;

    this.soapService.getSoapRecord(this.soapId).subscribe({
      next: (record) => {
        if (record.objective) {
          const vitalSigns = record.objective.vitalSigns || {};
          const physicalExam = record.objective.physicalExam || {};
          
          this.form.patchValue({
            ...vitalSigns,
            ...physicalExam,
            labResults: record.objective.labResults,
            imagingResults: record.objective.imagingResults,
            otherExamResults: record.objective.otherExamResults
          });
        }
      },
      error: (error: any) => {
        this.snackBar.open(error.message, 'Fechar', { duration: 5000 });
      }
    });
  }

  calculateBMI(): void {
    const weight = this.form.get('weight')?.value;
    const height = this.form.get('height')?.value;

    if (weight && height && height > 0) {
      const heightInMeters = height / 100;
      const bmi = weight / (heightInMeters * heightInMeters);
      // Store calculated BMI for display
      this.form.patchValue({ bmi }, { emitEvent: false });
    }
  }

  getBMI(): string {
    const weight = this.form.get('weight')?.value;
    const height = this.form.get('height')?.value;

    if (weight && height && height > 0) {
      const heightInMeters = height / 100;
      const bmi = weight / (heightInMeters * heightInMeters);
      return bmi.toFixed(1);
    }
    return '--';
  }

  getBMIClassification(): string {
    const weight = this.form.get('weight')?.value;
    const height = this.form.get('height')?.value;

    if (!weight || !height || height <= 0) return '';

    const heightInMeters = height / 100;
    const bmi = weight / (heightInMeters * heightInMeters);

    if (bmi < 18.5) return '(Abaixo do peso)';
    if (bmi < 25) return '(Peso normal)';
    if (bmi < 30) return '(Sobrepeso)';
    if (bmi < 35) return '(Obesidade Grau I)';
    if (bmi < 40) return '(Obesidade Grau II)';
    return '(Obesidade Grau III)';
  }

  save(): void {
    this.saving = true;
    
    const formValue = this.form.value;
    const data: UpdateObjectiveCommand = {
      vitalSigns: {
        systolicBP: formValue.systolicBP,
        diastolicBP: formValue.diastolicBP,
        heartRate: formValue.heartRate,
        respiratoryRate: formValue.respiratoryRate,
        temperature: formValue.temperature,
        oxygenSaturation: formValue.oxygenSaturation,
        weight: formValue.weight,
        height: formValue.height,
        bmi: null, // Will be calculated by backend
        pain: formValue.pain
      },
      physicalExam: {
        generalAppearance: formValue.generalAppearance,
        head: formValue.head,
        eyes: formValue.eyes,
        ears: formValue.ears,
        nose: formValue.nose,
        throat: formValue.throat,
        neck: formValue.neck,
        cardiovascular: formValue.cardiovascular,
        respiratory: formValue.respiratory,
        abdomen: formValue.abdomen,
        musculoskeletal: formValue.musculoskeletal,
        neurological: formValue.neurological,
        skin: formValue.skin,
        otherFindings: formValue.otherFindings
      },
      labResults: formValue.labResults,
      imagingResults: formValue.imagingResults,
      otherExamResults: formValue.otherExamResults
    };

    this.soapService.updateObjective(this.soapId, data).subscribe({
      next: () => {
        this.snackBar.open('Dados objetivos salvos com sucesso!', 'Fechar', { duration: 3000 });
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
