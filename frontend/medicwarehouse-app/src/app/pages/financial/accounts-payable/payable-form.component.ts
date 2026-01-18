import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { PayableCategory } from '../../../models/financial.model';

@Component({
  selector: 'app-payable-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './payable-form.component.html',
  styleUrl: './payable-form.component.scss'
})
export class PayableFormComponent implements OnInit {
  payableForm: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  categories = [
    { value: PayableCategory.Rent, label: 'Aluguel' },
    { value: PayableCategory.Salaries, label: 'Salários' },
    { value: PayableCategory.Supplies, label: 'Suprimentos' },
    { value: PayableCategory.Equipment, label: 'Equipamentos' },
    { value: PayableCategory.Maintenance, label: 'Manutenção' },
    { value: PayableCategory.Utilities, label: 'Utilidades' },
    { value: PayableCategory.Other, label: 'Outro' }
  ];

  constructor(
    private fb: FormBuilder,
    private financialService: FinancialService,
    private router: Router
  ) {
    this.payableForm = this.fb.group({
      documentNumber: ['', [Validators.required]],
      category: [PayableCategory.Other, [Validators.required]],
      dueDate: ['', [Validators.required]],
      totalAmount: ['', [Validators.required, Validators.min(0.01)]],
      description: ['', [Validators.required]],
      supplierId: ['']
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.payableForm.valid) {
      this.isLoading.set(true);
      const formValue = this.payableForm.value;
      
      this.financialService.createPayable(formValue).subscribe({
        next: () => {
          this.successMessage.set('Conta a pagar cadastrada com sucesso!');
          this.isLoading.set(false);
          setTimeout(() => this.router.navigate(['/financial/payables']), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cadastrar conta a pagar');
          this.isLoading.set(false);
          console.error('Error:', error);
        }
      });
    }
  }
}
