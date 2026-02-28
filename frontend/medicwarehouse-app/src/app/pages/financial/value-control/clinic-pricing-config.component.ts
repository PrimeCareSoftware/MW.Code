import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import {
  ClinicPricingConfiguration,
  ProcedureConsultationPolicy,
  CreateClinicPricingConfiguration
} from '../../../models/financial.model';

@Component({
  selector: 'app-clinic-pricing-config',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './clinic-pricing-config.component.html',
  styleUrl: './clinic-pricing-config.component.scss'
})
export class ClinicPricingConfigComponent implements OnInit {
  pricingForm: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  configuration = signal<ClinicPricingConfiguration | null>(null);
  clinicId = signal<string>(''); // Should be set from clinic selection context

  policyOptions = [
    { value: ProcedureConsultationPolicy.ChargeConsultation, label: 'Cobrar Consulta Integral + Procedimento' },
    { value: ProcedureConsultationPolicy.DiscountOnConsultation, label: 'Aplicar Desconto na Consulta' },
    { value: ProcedureConsultationPolicy.NoCharge, label: 'Não Cobrar Consulta (Apenas Procedimento)' }
  ];

  constructor(
    private fb: FormBuilder,
    private financialService: FinancialService,
    private router: Router
  ) {
    this.pricingForm = this.fb.group({
      defaultConsultationPrice: [0, [Validators.required, Validators.min(0)]],
      followUpConsultationPrice: [null, [Validators.min(0)]],
      telemedicineConsultationPrice: [null, [Validators.min(0)]],
      defaultProcedurePolicy: [ProcedureConsultationPolicy.ChargeConsultation, [Validators.required]],
      consultationDiscountPercentage: [null, [Validators.min(0), Validators.max(100)]],
      consultationDiscountFixedAmount: [null, [Validators.min(0)]]
    });

    // Watch policy changes to enable/disable discount fields
    this.pricingForm.get('defaultProcedurePolicy')?.valueChanges.subscribe(policy => {
      this.updateDiscountValidators(policy);
    });
  }

  ngOnInit(): void {
    // Get clinic ID from localStorage (set by clinic selection context)
    // In production, this would be part of a clinic context service
    const clinicId = localStorage.getItem('selectedClinicId');
    if (clinicId) {
      this.clinicId.set(clinicId);
      this.loadConfiguration();
    }
  }

  updateDiscountValidators(policy: ProcedureConsultationPolicy): void {
    const percentageControl = this.pricingForm.get('consultationDiscountPercentage');
    const amountControl = this.pricingForm.get('consultationDiscountFixedAmount');

    if (policy === ProcedureConsultationPolicy.DiscountOnConsultation) {
      // At least one discount should be set
      percentageControl?.enable();
      amountControl?.enable();
    } else {
      // Clear and disable discount fields
      percentageControl?.setValue(null);
      amountControl?.setValue(null);
      percentageControl?.disable();
      amountControl?.disable();
    }
  }

  loadConfiguration(): void {
    const clinicId = this.clinicId();
    if (!clinicId) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.financialService.getClinicPricingConfiguration(clinicId).subscribe({
      next: (config) => {
        this.configuration.set(config);
        this.pricingForm.patchValue({
          defaultConsultationPrice: config.defaultConsultationPrice,
          followUpConsultationPrice: config.followUpConsultationPrice,
          telemedicineConsultationPrice: config.telemedicineConsultationPrice,
          defaultProcedurePolicy: config.defaultProcedurePolicy,
          consultationDiscountPercentage: config.consultationDiscountPercentage,
          consultationDiscountFixedAmount: config.consultationDiscountFixedAmount
        });
        this.updateDiscountValidators(config.defaultProcedurePolicy);
        this.isLoading.set(false);
      },
      error: (error) => {
        // Configuration not found is OK, we'll create a new one
        if (error.status === 404) {
          this.isLoading.set(false);
        } else {
          this.errorMessage.set('Erro ao carregar configuração: ' + error.message);
          this.isLoading.set(false);
        }
      }
    });
  }

  onSubmit(): void {
    if (this.pricingForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    const clinicId = this.clinicId();
    if (!clinicId) {
      this.errorMessage.set('Clínica não selecionada.');
      return;
    }

    const policy = this.pricingForm.value.defaultProcedurePolicy;
    
    // Validate discount when policy requires it
    if (policy === ProcedureConsultationPolicy.DiscountOnConsultation) {
      const hasDiscount = 
        this.pricingForm.value.consultationDiscountPercentage != null ||
        this.pricingForm.value.consultationDiscountFixedAmount != null;
      
      if (!hasDiscount) {
        this.errorMessage.set('É necessário informar um desconto (percentual ou valor fixo) quando a política é "Aplicar Desconto".');
        return;
      }
    }

    const data: CreateClinicPricingConfiguration = {
      clinicId: clinicId,
      ...this.pricingForm.value
    };

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.financialService.createOrUpdateClinicPricingConfiguration(data).subscribe({
      next: (config) => {
        this.configuration.set(config);
        this.successMessage.set('Configuração salva com sucesso!');
        this.isLoading.set(false);
        
        // Clear success message after 3 seconds
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao salvar configuração: ' + (error.error?.message || error.message));
        this.isLoading.set(false);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/financial']);
  }

  getPolicyLabel(policy: ProcedureConsultationPolicy): string {
    const option = this.policyOptions.find(opt => opt.value === policy);
    return option?.label || '';
  }
}
