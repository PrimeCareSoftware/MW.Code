import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { TelemedicineComplianceService } from '../../../services/telemedicine-compliance.service';

@Component({
  selector: 'app-consent-form',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './consent-form.html',
  styleUrl: './consent-form.scss'
})
export class ConsentForm implements OnInit {
  consentForm: FormGroup;
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  consentText = signal<string>('');
  sessionId: string | null = null;
  patientId: string | null = null;
  tenantId: string = 'default-tenant'; // TODO: Get from auth service

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private complianceService: TelemedicineComplianceService
  ) {
    this.consentForm = this.fb.group({
      patientName: ['', Validators.required],
      patientCpf: ['', Validators.required],
      acceptTerms: [false, Validators.requiredTrue],
      acceptDataProcessing: [false, Validators.requiredTrue],
      acceptRecording: [false],
      signature: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.sessionId = params['sessionId'];
      this.patientId = params['patientId'];
    });

    // Load consent text
    this.loadConsentText();
  }

  loadConsentText(): void {
    this.complianceService.getConsentText(false).subscribe({
      next: (response) => {
        this.consentText.set(response.consentText);
      },
      error: (error) => {
        console.error('Error loading consent text:', error);
        this.errorMessage.set('Erro ao carregar texto do consentimento');
      }
    });
  }

  onSubmit(): void {
    if (this.consentForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios e aceite os termos');
      return;
    }

    if (!this.patientId) {
      this.errorMessage.set('ID do paciente não fornecido');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');

    const consentRequest = {
      patientId: this.patientId,
      appointmentId: this.sessionId || undefined,
      acceptsRecording: this.consentForm.value.acceptRecording,
      acceptsDataSharing: this.consentForm.value.acceptDataProcessing,
      digitalSignature: this.consentForm.value.signature
    };

    this.complianceService.recordConsent(consentRequest, this.tenantId).subscribe({
      next: (response) => {
        this.successMessage.set('Consentimento registrado com sucesso!');
        this.isSaving.set(false);
        
        setTimeout(() => {
          if (this.sessionId) {
            this.router.navigate(['/telemedicine/room', this.sessionId]);
          } else {
            this.router.navigate(['/telemedicine']);
          }
        }, 1500);
      },
      error: (error) => {
        console.error('Error recording consent:', error);
        this.errorMessage.set(
          error.error?.message || 
          'Erro ao registrar consentimento. Por favor, tente novamente.'
        );
        this.isSaving.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/telemedicine']);
  }
}
