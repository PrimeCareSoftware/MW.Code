import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { SubscriptionService } from '../../services/subscription';
import { CartService } from '../../services/cart';
import { RegistrationRequest } from '../../models/registration.model';
import { SubscriptionPlan } from '../../models/subscription-plan.model';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private subscriptionService = inject(SubscriptionService);
  private cartService = inject(CartService);
  
  selectedPlan?: SubscriptionPlan;
  currentStep = 1;
  
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

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['plan']) {
        const plan = this.subscriptionService.getPlanById(params['plan']);
        if (plan) {
          this.selectedPlan = plan;
          this.model.planId = plan.id;
          this.cartService.clearCart();
          this.cartService.addToCart(plan);
        }
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
  }

  nextStep(): void {
    if (this.validateStep(this.currentStep)) {
      this.currentStep++;
    }
  }

  prevStep(): void {
    this.currentStep--;
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
        this.router.navigate(['/checkout'], { 
          queryParams: { 
            success: true,
            clinicId: response.clinicId 
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
}
