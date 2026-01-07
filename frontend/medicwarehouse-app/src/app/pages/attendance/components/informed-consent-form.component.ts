import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { InformedConsentService } from '../../../services/informed-consent.service';
import { CreateInformedConsent, InformedConsent } from '../../../models/medical-record.model';

@Component({
  selector: 'app-informed-consent-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="informed-consent-form">
      <h3>Consentimento Informado (CFM 1.821/2007)</h3>
      <p class="info-text">
        O consentimento informado é obrigatório para procedimentos médicos e deve ser registrado no prontuário.
      </p>

      @if (errorMessage()) {
        <div class="alert alert-error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert alert-success">{{ successMessage() }}</div>
      }

      @if (existingConsents().length > 0) {
        <div class="existing-consents">
          <h4>Consentimentos Registrados</h4>
          @for (consent of existingConsents(); track consent.id) {
            <div class="consent-item" [class.accepted]="consent.isAccepted">
              <div class="consent-header">
                <span class="consent-date">{{ consent.createdAt | date: 'dd/MM/yyyy HH:mm' }}</span>
                <span class="consent-status" [class.accepted]="consent.isAccepted">
                  {{ consent.isAccepted ? 'Aceito' : 'Pendente' }}
                </span>
              </div>
              <div class="consent-text">{{ consent.consentText }}</div>
              @if (consent.isAccepted && consent.acceptedAt) {
                <div class="consent-acceptance">
                  <small>Aceito em: {{ consent.acceptedAt | date: 'dd/MM/yyyy HH:mm' }}</small>
                  @if (consent.ipAddress) {
                    <small>IP: {{ consent.ipAddress }}</small>
                  }
                </div>
              }
            </div>
          }
        </div>
      }

      <form [formGroup]="consentForm" (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label for="consentText">Texto do Consentimento *</label>
          <textarea
            id="consentText"
            formControlName="consentText"
            rows="8"
            placeholder="Digite o texto do consentimento informado que será apresentado ao paciente..."
            [class.invalid]="consentForm.get('consentText')?.invalid && consentForm.get('consentText')?.touched"
          ></textarea>
          @if (consentForm.get('consentText')?.invalid && consentForm.get('consentText')?.touched) {
            <small class="error-text">O texto do consentimento deve ter no mínimo 50 caracteres</small>
          }
        </div>

        <div class="form-group">
          <label class="checkbox-label">
            <input
              type="checkbox"
              formControlName="acceptImmediately"
            />
            <span>Registrar aceite imediato do paciente</span>
          </label>
          <small class="help-text">
            Marque se o paciente já aceitou o consentimento verbalmente ou por escrito
          </small>
        </div>

        @if (consentForm.get('acceptImmediately')?.value) {
          <div class="form-group">
            <label for="ipAddress">Endereço IP (opcional)</label>
            <input
              type="text"
              id="ipAddress"
              formControlName="ipAddress"
              placeholder="Ex: 192.168.1.1"
            />
            <small class="help-text">Para rastreabilidade do aceite</small>
          </div>
        }

        <div class="form-actions">
          <button
            type="submit"
            class="btn btn-primary"
            [disabled]="consentForm.invalid || isSaving()"
          >
            {{ isSaving() ? 'Salvando...' : 'Registrar Consentimento' }}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .informed-consent-form {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .info-text {
      color: #666;
      font-size: 0.9rem;
      margin-bottom: 1.5rem;
      padding: 0.75rem;
      background: #f8f9fa;
      border-left: 3px solid #007bff;
      border-radius: 4px;
    }

    .existing-consents {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .existing-consents h4 {
      margin-bottom: 1rem;
      color: #333;
    }

    .consent-item {
      background: white;
      padding: 1rem;
      margin-bottom: 0.75rem;
      border-radius: 6px;
      border: 1px solid #ddd;
    }

    .consent-item.accepted {
      border-color: #28a745;
      background: #f8fff9;
    }

    .consent-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .consent-date {
      font-size: 0.85rem;
      color: #666;
    }

    .consent-status {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.8rem;
      background: #ffc107;
      color: #000;
    }

    .consent-status.accepted {
      background: #28a745;
      color: white;
    }

    .consent-text {
      color: #333;
      line-height: 1.6;
      margin-bottom: 0.5rem;
    }

    .consent-acceptance {
      display: flex;
      gap: 1rem;
      padding-top: 0.5rem;
      border-top: 1px solid #eee;
      margin-top: 0.5rem;
    }

    .consent-acceptance small {
      color: #666;
      font-size: 0.8rem;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
    }

    .form-group textarea,
    .form-group input[type="text"] {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-family: inherit;
      font-size: 0.95rem;
    }

    .form-group textarea.invalid,
    .form-group input.invalid {
      border-color: #dc3545;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
    }

    .checkbox-label input[type="checkbox"] {
      width: auto;
      margin: 0;
    }

    .help-text {
      display: block;
      margin-top: 0.25rem;
      color: #666;
      font-size: 0.85rem;
    }

    .error-text {
      display: block;
      margin-top: 0.25rem;
      color: #dc3545;
      font-size: 0.85rem;
    }

    .form-actions {
      margin-top: 2rem;
      display: flex;
      justify-content: flex-end;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-primary {
      background: #007bff;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #0056b3;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .alert {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      border-radius: 4px;
    }

    .alert-error {
      background: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .alert-success {
      background: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }
  `]
})
export class InformedConsentFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Input() patientId!: string;
  @Output() consentCreated = new EventEmitter<InformedConsent>();

  consentForm: FormGroup;
  existingConsents = signal<InformedConsent[]>([]);
  isSaving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private fb: FormBuilder,
    private consentService: InformedConsentService
  ) {
    this.consentForm = this.fb.group({
      consentText: ['', [Validators.required, Validators.minLength(50)]],
      acceptImmediately: [false],
      ipAddress: ['']
    });
  }

  ngOnInit() {
    this.loadExistingConsents();
  }

  loadExistingConsents() {
    if (!this.medicalRecordId) return;

    this.consentService.getByMedicalRecord(this.medicalRecordId).subscribe({
      next: (consents) => {
        this.existingConsents.set(consents);
      },
      error: (error) => {
        console.error('Error loading consents:', error);
      }
    });
  }

  onSubmit() {
    if (this.consentForm.invalid) return;

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.consentForm.value;
    const createData: CreateInformedConsent = {
      medicalRecordId: this.medicalRecordId,
      patientId: this.patientId,
      consentText: formValue.consentText.trim()
    };

    this.consentService.create(createData).subscribe({
      next: (consent) => {
        if (formValue.acceptImmediately) {
          // Auto-accept the consent
          this.consentService.accept(consent.id, {
            ipAddress: formValue.ipAddress || undefined
          }).subscribe({
            next: (acceptedConsent) => {
              this.successMessage.set('Consentimento registrado e aceito com sucesso!');
              this.existingConsents.update(consents => [...consents, acceptedConsent]);
              this.consentCreated.emit(acceptedConsent);
              this.resetForm();
              this.isSaving.set(false);
            },
            error: (error) => {
              this.errorMessage.set('Erro ao aceitar consentimento: ' + (error.error?.message || 'Erro desconhecido'));
              this.isSaving.set(false);
            }
          });
        } else {
          this.successMessage.set('Consentimento registrado com sucesso! Aguardando aceite do paciente.');
          this.existingConsents.update(consents => [...consents, consent]);
          this.consentCreated.emit(consent);
          this.resetForm();
          this.isSaving.set(false);
        }
      },
      error: (error) => {
        this.errorMessage.set('Erro ao registrar consentimento: ' + (error.error?.message || 'Erro desconhecido'));
        this.isSaving.set(false);
      }
    });
  }

  resetForm() {
    this.consentForm.reset({
      consentText: '',
      acceptImmediately: false,
      ipAddress: ''
    });
  }
}
