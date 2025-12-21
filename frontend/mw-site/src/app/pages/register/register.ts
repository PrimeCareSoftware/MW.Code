import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { SubscriptionService } from '../../services/subscription';
import { CartService } from '../../services/cart';
import { FormPersistenceService } from '../../services/form-persistence';
import { CepService } from '../../services/cep.service';
import { RegistrationRequest } from '../../models/registration.model';
import { SubscriptionPlan } from '../../models/subscription-plan.model';
import { CpfMaskDirective } from '../../directives/cpf-mask.directive';
import { CnpjMaskDirective } from '../../directives/cnpj-mask.directive';
import { PhoneMaskDirective } from '../../directives/phone-mask.directive';
import { CepMaskDirective } from '../../directives/cep-mask.directive';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink, CpfMaskDirective, CnpjMaskDirective, PhoneMaskDirective, CepMaskDirective],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private subscriptionService = inject(SubscriptionService);
  private cartService = inject(CartService);
  private formPersistence = inject(FormPersistenceService);
  private cepService = inject(CepService);
  
  selectedPlan?: SubscriptionPlan;
  allPlans: SubscriptionPlan[] = [];
  currentStep = 1;
  showDataConsentDialog = false;
  hasSavedData = false;
  
  model: RegistrationRequest = {
    clinicName: '',
    clinicCNPJ: '',
    clinicPhone: '',
    clinicEmail: '',
    street: '',
    number: '',
    complement: '',
    neighborhood: '',
    city: '',
    state: '',
    zipCode: '',
    ownerName: '',
    ownerCPF: '',
    ownerPhone: '',
    ownerEmail: '',
    username: '',
    password: '',
    planId: '',
    acceptTerms: false,
    useTrial: true
  };
  
  passwordConfirm = '';
  isSubmitting = false;
  submitError = '';
  isLoadingCep = false;
  private autoSaveInterval?: number;

  ngOnInit(): void {
    // Load all plans for the plan selection step
    this.subscriptionService.getPlans().subscribe({
      next: (plans) => {
        this.allPlans = plans;
      },
      error: (error) => {
        console.error('Error loading plans:', error);
      }
    });

    // Check if user has saved data
    this.hasSavedData = this.formPersistence.hasSavedData();

    // Check for plan from URL
    this.route.queryParams.subscribe(params => {
      if (params['plan']) {
        this.subscriptionService.getPlanById(params['plan']).subscribe(plan => {
          if (plan) {
            this.selectedPlan = plan;
            this.model.planId = plan.id;
            this.cartService.clearCart();
            this.cartService.addToCart(plan);
          }
        });
      }
    });

    // If no plan selected, try to get from cart
    if (!this.selectedPlan) {
      const cart = this.cartService.getCart()();
      if (cart.items.length > 0) {
        this.selectedPlan = cart.items[0].plan;
        this.model.planId = this.selectedPlan.id;
      }
    }

    // Show data consent dialog if there's saved data and no consent
    if (this.hasSavedData && !this.formPersistence.hasConsent()) {
      this.showDataConsentDialog = true;
    } else if (this.formPersistence.hasConsent()) {
      // Load saved data if consent is granted
      this.loadSavedData();
    }

    // Start auto-save interval (every 30 seconds)
    this.autoSaveInterval = window.setInterval(() => {
      this.autoSaveFormData();
    }, 30000);
  }

  ngOnDestroy(): void {
    // Clear auto-save interval
    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
    }
  }

  /**
   * Accept data consent and load saved data
   */
  acceptDataConsent(): void {
    this.formPersistence.grantConsent();
    this.loadSavedData();
    this.showDataConsentDialog = false;
  }

  /**
   * Decline data consent and clear saved data
   */
  declineDataConsent(): void {
    this.formPersistence.revokeConsent();
    this.hasSavedData = false;
    this.showDataConsentDialog = false;
  }

  /**
   * Load saved form data
   */
  private loadSavedData(): void {
    const savedData = this.formPersistence.loadFormData();
    if (savedData) {
      Object.assign(this.model, savedData);
      
      // Reload selected plan if planId is saved
      if (this.model.planId) {
        this.subscriptionService.getPlanById(this.model.planId).subscribe(plan => {
          if (plan) {
            this.selectedPlan = plan;
          }
        });
      }
    }
  }

  /**
   * Auto-save form data
   */
  private autoSaveFormData(): void {
    if (this.formPersistence.hasConsent() && this.currentStep > 1) {
      this.formPersistence.saveFormData(this.model);
    }
  }

  /**
   * Clear saved data (called on successful registration)
   */
  private clearSavedData(): void {
    this.formPersistence.clearFormData();
  }

  nextStep(): void {
    if (this.validateStep(this.currentStep)) {
      this.currentStep++;
      // Auto-save when moving to next step
      this.autoSaveFormData();
    }
  }

  prevStep(): void {
    this.currentStep--;
  }

  /**
   * Change the selected plan
   */
  changePlan(plan: SubscriptionPlan): void {
    this.selectedPlan = plan;
    this.model.planId = plan.id;
    this.cartService.clearCart();
    this.cartService.addToCart(plan);
    // Save the new selection
    this.autoSaveFormData();
  }

  validateStep(step: number): boolean {
    switch (step) {
      case 1:
        return !!(this.model.clinicName && this.model.clinicCNPJ && 
                  this.model.clinicPhone && this.model.clinicEmail);
      case 2:
        return !!(this.model.street && this.model.number && this.model.neighborhood && 
                  this.model.city && this.model.state && this.model.zipCode);
      case 3:
        return !!(this.model.ownerName && this.model.ownerCPF && 
                  this.model.ownerPhone && this.model.ownerEmail);
      case 4:
        return !!(this.model.username && this.model.password && 
                  this.passwordConfirm && this.model.password === this.passwordConfirm);
      case 5:
        return !!(this.selectedPlan && this.model.planId);
      default:
        return true;
    }
  }

  onSubmit(): void {
    if (!this.model.acceptTerms) {
      this.submitError = 'Você deve aceitar os termos de uso.';
      return;
    }

    if (!this.selectedPlan) {
      this.submitError = 'Por favor, selecione um plano.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = '';

    this.subscriptionService.register(this.model).subscribe({
      next: (response) => {
        this.cartService.clearCart();
        this.clearSavedData(); // Clear saved data on success
        this.router.navigate(['/checkout'], { 
          queryParams: { 
            success: true,
            clinicId: response.clinicId,
            userId: response.userId,
            tenantId: response.tenantId,
            subdomain: response.subdomain,
            clinicName: response.clinicName,
            ownerName: response.ownerName,
            ownerEmail: response.ownerEmail,
            username: response.username
          } 
        });
      },
      error: (error) => {
        this.submitError = error.error?.message || 'Erro ao processar cadastro. Tente novamente.';
        this.isSubmitting = false;
        console.error('Registration error:', error);
      }
    });
  }

  formatCNPJ(value: string): void {
    let cnpj = value.replace(/\D/g, '');
    if (cnpj.length > 14) cnpj = cnpj.substring(0, 14);
    this.model.clinicCNPJ = cnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  }

  formatCPF(value: string): void {
    let cpf = value.replace(/\D/g, '');
    if (cpf.length > 11) cpf = cpf.substring(0, 11);
    this.model.ownerCPF = cpf.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, '$1.$2.$3-$4');
  }

  formatZipCode(value: string): void {
    let zip = value.replace(/\D/g, '');
    if (zip.length > 8) zip = zip.substring(0, 8);
    this.model.zipCode = zip.replace(/^(\d{5})(\d{3})$/, '$1-$2');
  }

  /**
   * Look up CEP and auto-fill address fields
   */
  onCepBlur(): void {
    const cep = this.model.zipCode;
    if (!cep || cep.replace(/\D/g, '').length !== 8) {
      return;
    }

    this.isLoadingCep = true;
    this.cepService.lookupCep(cep).subscribe({
      next: (addressData) => {
        this.isLoadingCep = false;
        if (addressData) {
          // Auto-fill address fields
          this.model.street = addressData.street;
          this.model.neighborhood = addressData.neighborhood;
          this.model.city = addressData.city;
          this.model.state = addressData.state;
          if (addressData.complement) {
            this.model.complement = addressData.complement;
          }
        }
      },
      error: (error) => {
        this.isLoadingCep = false;
        console.error('Error looking up CEP:', error);
      }
    });
  }
}
