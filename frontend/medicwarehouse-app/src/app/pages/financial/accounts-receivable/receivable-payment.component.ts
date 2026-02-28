import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { AccountsReceivable } from '../../../models/financial.model';

@Component({
  selector: 'app-receivable-payment',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './receivable-payment.component.html',
  styleUrl: './receivable-payment.component.scss'
})
export class ReceivablePaymentComponent implements OnInit {
  paymentForm: FormGroup;
  receivable = signal<AccountsReceivable | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private financialService: FinancialService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    const today = new Date().toISOString().split('T')[0];
    this.paymentForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0.01)]],
      paymentDate: [today, [Validators.required]],
      transactionId: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadReceivable(id);
    }
  }

  loadReceivable(id: string): void {
    this.isLoading.set(true);
    this.financialService.getReceivableById(id).subscribe({
      next: (data) => {
        this.receivable.set(data);
        // Set default amount to outstanding amount
        this.paymentForm.patchValue({
          amount: data.outstandingAmount
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar conta a receber');
        this.isLoading.set(false);
        console.error('Error loading receivable:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    const receivable = this.receivable();
    if (!receivable) return;

    const amount = this.paymentForm.value.amount;
    if (amount > receivable.outstandingAmount) {
      this.errorMessage.set('O valor do pagamento não pode ser maior que o saldo devedor');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const payment = {
      receivableId: receivable.id,
      amount: this.paymentForm.value.amount,
      paymentDate: this.paymentForm.value.paymentDate,
      transactionId: this.paymentForm.value.transactionId || undefined,
      notes: this.paymentForm.value.notes || undefined
    };

    this.financialService.addReceivablePayment(receivable.id, payment).subscribe({
      next: () => {
        this.successMessage.set('Pagamento registrado com sucesso');
        this.isLoading.set(false);
        setTimeout(() => {
          this.router.navigate(['/financial/receivables']);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao registrar pagamento');
        this.isLoading.set(false);
        console.error('Error adding payment:', error);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/financial/receivables']);
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(amount);
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  formatStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'Pendente',
      'PartiallyPaid': 'Parcialmente Pago',
      'Paid': 'Pago',
      'Overdue': 'Vencido',
      'Cancelled': 'Cancelado',
      'InNegotiation': 'Em Negociação'
    };
    return statusMap[status] || status;
  }

  getStatusBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'badge-warning',
      'PartiallyPaid': 'badge-info',
      'Paid': 'badge-success',
      'Overdue': 'badge-error',
      'Cancelled': 'badge-error',
      'InNegotiation': 'badge-info'
    };
    return statusMap[status] || 'badge-default';
  }
}
