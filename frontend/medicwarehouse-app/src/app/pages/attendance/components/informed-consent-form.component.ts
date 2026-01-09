import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { InformedConsentService } from '../../../services/informed-consent.service';
import { CreateInformedConsent, InformedConsent } from '../../../models/medical-record.model';

@Component({
  selector: 'app-informed-consent-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './informed-consent-form.component.html',
  styleUrl: './informed-consent-form.component.scss'})
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
