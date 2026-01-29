import { Component, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TelemedicineComplianceService, SessionComplianceValidation } from '../../../services/telemedicine-compliance.service';

interface ComplianceCheckItem {
  label: string;
  status: 'valid' | 'invalid' | 'checking';
  message: string;
  required: boolean;
  actionLink?: string;
  actionLabel?: string;
}

@Component({
  selector: 'app-session-compliance-checker',
  imports: [CommonModule],
  templateUrl: './session-compliance-checker.html',
  styleUrl: './session-compliance-checker.scss'
})
export class SessionComplianceChecker implements OnInit {
  @Input() sessionId!: string;
  @Input() tenantId: string = 'default-tenant';
  @Input() autoCheck: boolean = true;
  
  isChecking = signal<boolean>(false);
  isCompliant = signal<boolean>(false);
  canStartSession = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  checks = signal<ComplianceCheckItem[]>([]);

  constructor(
    private complianceService: TelemedicineComplianceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.sessionId) {
      this.errorMessage.set('Session ID não fornecido');
      return;
    }

    if (this.autoCheck) {
      this.performComplianceCheck();
    }
  }

  performComplianceCheck(): void {
    this.isChecking.set(true);
    this.errorMessage.set('');

    // Initialize checking state
    this.checks.set([
      {
        label: 'Consentimento do Paciente',
        status: 'checking',
        message: 'Verificando consentimento...',
        required: true
      },
      {
        label: 'Identidade do Médico',
        status: 'checking',
        message: 'Verificando identidade...',
        required: true
      },
      {
        label: 'Identidade do Paciente',
        status: 'checking',
        message: 'Verificando identidade...',
        required: true
      }
    ]);

    this.complianceService.validateSessionCompliance(this.sessionId, this.tenantId).subscribe({
      next: (validation) => {
        this.processValidationResult(validation);
        this.isChecking.set(false);
      },
      error: (error) => {
        console.error('Error validating compliance:', error);
        this.errorMessage.set(
          error.error?.message || 
          'Erro ao validar conformidade. Por favor, tente novamente.'
        );
        this.isChecking.set(false);
        
        // Set all checks to invalid on error
        this.checks.update(checks => 
          checks.map(check => ({
            ...check,
            status: 'invalid' as const,
            message: 'Falha na verificação'
          }))
        );
      }
    });
  }

  private processValidationResult(validation: SessionComplianceValidation): void {
    this.isCompliant.set(validation.isCompliant);
    this.canStartSession.set(validation.canStart);

    const updatedChecks: ComplianceCheckItem[] = [
      {
        label: 'Consentimento do Paciente',
        status: validation.compliance.patientConsent.isValid ? 'valid' : 'invalid',
        message: validation.compliance.patientConsent.message,
        required: validation.compliance.patientConsent.required,
        actionLink: validation.compliance.patientConsent.isValid ? undefined : '/telemedicine/consent',
        actionLabel: 'Registrar Consentimento'
      },
      {
        label: 'Identidade do Médico',
        status: validation.compliance.providerIdentity.isVerified ? 'valid' : 'invalid',
        message: validation.compliance.providerIdentity.message,
        required: validation.compliance.providerIdentity.required,
        actionLink: validation.compliance.providerIdentity.isVerified ? undefined : '/telemedicine/identity-verification',
        actionLabel: 'Enviar Documentos'
      },
      {
        label: 'Identidade do Paciente',
        status: validation.compliance.patientIdentity.isVerified ? 'valid' : 'invalid',
        message: validation.compliance.patientIdentity.message,
        required: validation.compliance.patientIdentity.required,
        actionLink: validation.compliance.patientIdentity.isVerified ? undefined : '/telemedicine/identity-verification',
        actionLabel: 'Enviar Documentos'
      }
    ];

    this.checks.set(updatedChecks);
  }

  navigateToAction(actionLink: string | undefined): void {
    if (actionLink) {
      this.router.navigate([actionLink], {
        queryParams: { sessionId: this.sessionId }
      });
    }
  }

  retryCheck(): void {
    this.performComplianceCheck();
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'valid':
        return 'fa-check-circle';
      case 'invalid':
        return 'fa-times-circle';
      case 'checking':
        return 'fa-spinner fa-spin';
      default:
        return 'fa-question-circle';
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'valid':
        return 'status-valid';
      case 'invalid':
        return 'status-invalid';
      case 'checking':
        return 'status-checking';
      default:
        return '';
    }
  }

  get allChecksValid(): boolean {
    const checksList = this.checks();
    return checksList.length > 0 && 
           checksList.every(check => !check.required || check.status === 'valid');
  }

  get hasInvalidChecks(): boolean {
    return this.checks().some(check => check.status === 'invalid' && check.required);
  }
}
