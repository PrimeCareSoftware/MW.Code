import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { HealthInsuranceOperatorService } from '../../../services/health-insurance-operator.service';
import { CepService } from '../../../services/cep.service';
import { CnpjMaskDirective } from '../../../directives/cnpj-mask.directive';
import { PhoneMaskDirective } from '../../../directives/phone-mask.directive';
import { CepMaskDirective } from '../../../directives/cep-mask.directive';

@Component({
  selector: 'app-health-insurance-operator-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar, CnpjMaskDirective, PhoneMaskDirective, CepMaskDirective],
  templateUrl: './health-insurance-operator-form.html',
  styleUrl: './health-insurance-operator-form.scss'
})
export class HealthInsuranceOperatorForm implements OnInit {
  operatorForm: FormGroup;
  isEditMode = signal<boolean>(false);
  operatorId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  isLoadingCep = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private operatorService: HealthInsuranceOperatorService,
    private cepService: CepService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.operatorForm = this.fb.group({
      ansCode: ['', [Validators.required]],
      tradeName: ['', [Validators.required]],
      legalName: ['', [Validators.required]],
      cnpj: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneCountryCode: ['+55', [Validators.required]],
      phoneNumber: ['', [Validators.required]],
      address: this.fb.group({
        street: ['', [Validators.required]],
        number: ['', [Validators.required]],
        complement: [''],
        neighborhood: ['', [Validators.required]],
        city: ['', [Validators.required]],
        state: ['', [Validators.required]],
        zipCode: ['', [Validators.required]],
        country: ['Brasil', [Validators.required]]
      }),
      isActive: [true]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.operatorId.set(id);
      this.loadOperator(id);
    }
  }

  loadOperator(id: string): void {
    this.isLoading.set(true);
    this.operatorService.getById(id).subscribe({
      next: (operator) => {
        this.operatorForm.patchValue({
          ansCode: operator.ansCode,
          tradeName: operator.tradeName,
          legalName: operator.legalName,
          cnpj: operator.cnpj,
          email: operator.email,
          phoneCountryCode: operator.phoneCountryCode,
          phoneNumber: operator.phoneNumber,
          address: operator.address,
          isActive: operator.isActive
        });
        if (this.isEditMode()) {
          this.operatorForm.get('ansCode')?.disable();
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar operadora');
        this.isLoading.set(false);
        console.error('Error loading operator:', error);
      }
    });
  }

  onCepChange(): void {
    const cep = this.operatorForm.get('address.zipCode')?.value?.replace(/\D/g, '');
    if (cep && cep.length === 8) {
      this.isLoadingCep.set(true);
      this.cepService.lookupCep(cep).subscribe({
        next: (data) => {
          if (data) {
            this.operatorForm.patchValue({
              address: {
                street: data.street,
                neighborhood: data.neighborhood,
                city: data.city,
                state: data.state,
                country: 'Brasil'
              }
            });
          }
          this.isLoadingCep.set(false);
        },
        error: () => {
          this.isLoadingCep.set(false);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.operatorForm.invalid) {
      this.operatorForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.operatorForm.getRawValue();

    if (this.isEditMode()) {
      const id = this.operatorId();
      if (!id) return;

      this.operatorService.update(id, formValue).subscribe({
        next: () => {
          this.successMessage.set('Operadora atualizada com sucesso');
          setTimeout(() => this.router.navigate(['/tiss/operators']), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar operadora');
          this.isLoading.set(false);
          console.error('Error updating operator:', error);
        }
      });
    } else {
      this.operatorService.create(formValue).subscribe({
        next: () => {
          this.successMessage.set('Operadora cadastrada com sucesso');
          setTimeout(() => this.router.navigate(['/tiss/operators']), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cadastrar operadora');
          this.isLoading.set(false);
          console.error('Error creating operator:', error);
        }
      });
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.operatorForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
