import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';

@Component({
  selector: 'app-supplier-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './supplier-form.component.html',
  styleUrl: './supplier-form.component.scss'
})
export class SupplierFormComponent implements OnInit {
  supplierForm: FormGroup;
  isEditMode = signal<boolean>(false);
  supplierId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private financialService: FinancialService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.supplierForm = this.fb.group({
      name: ['', [Validators.required]],
      tradeName: [''],
      documentNumber: [''],
      email: ['', [Validators.email]],
      phone: [''],
      address: [''],
      city: [''],
      state: [''],
      zipCode: [''],
      bankName: [''],
      bankAccount: [''],
      pixKey: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.supplierId.set(id);
      this.loadSupplier(id);
    }
  }

  loadSupplier(id: string): void {
    this.isLoading.set(true);
    this.financialService.getSupplierById(id).subscribe({
      next: (data) => {
        this.supplierForm.patchValue(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar fornecedor');
        this.isLoading.set(false);
        console.error('Error:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.supplierForm.valid) {
      this.isLoading.set(true);
      const formValue = this.supplierForm.value;

      if (this.isEditMode()) {
        this.financialService.updateSupplier(this.supplierId()!, formValue).subscribe({
          next: () => {
            this.successMessage.set('Fornecedor atualizado com sucesso!');
            setTimeout(() => this.router.navigate(['/financial/suppliers']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao atualizar fornecedor');
            this.isLoading.set(false);
          }
        });
      } else {
        this.financialService.createSupplier(formValue).subscribe({
          next: () => {
            this.successMessage.set('Fornecedor cadastrado com sucesso!');
            setTimeout(() => this.router.navigate(['/financial/suppliers']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao cadastrar fornecedor');
            this.isLoading.set(false);
          }
        });
      }
    }
  }
}
