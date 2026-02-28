import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ElectronicInvoiceService } from '../../../services/electronic-invoice.service';

@Component({
  selector: 'app-invoice-form',
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './invoice-form.component.html',
  styleUrl: './invoice-form.component.scss'
})
export class InvoiceFormComponent implements OnInit {
  form: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // Tax calculations
  issRate = signal<number>(5.0);
  calculatedTaxes = signal<any>({
    issAmount: 0,
    pisAmount: 0,
    cofinsAmount: 0,
    csllAmount: 0,
    totalTaxes: 0,
    netAmount: 0
  });

  constructor(
    private fb: FormBuilder,
    private invoiceService: ElectronicInvoiceService,
    private router: Router
  ) {
    this.form = this.fb.group({
      type: ['NFSe', [Validators.required]],
      clientCpfCnpj: ['', [Validators.required]],
      clientName: ['', [Validators.required]],
      clientEmail: ['', [Validators.email]],
      clientPhone: [''],
      clientAddress: [''],
      clientCity: [''],
      clientState: [''],
      clientZipCode: [''],
      serviceDescription: ['', [Validators.required]],
      serviceCode: [''],
      serviceAmount: [0, [Validators.required, Validators.min(0.01)]],
      autoIssue: [false],
      paymentId: [null],
      appointmentId: [null]
    });
  }

  ngOnInit(): void {
    this.loadDefaultIssRate();
    
    // Subscribe to service amount changes to recalculate taxes
    this.form.get('serviceAmount')?.valueChanges.subscribe(() => {
      this.calculateTaxes();
    });
  }

  loadDefaultIssRate(): void {
    this.invoiceService.getConfiguration().subscribe({
      next: (config) => {
        this.issRate.set(config.defaultIssRate);
        this.calculateTaxes();
      },
      error: (error) => {
        console.error('Error loading configuration:', error);
        // Use default rate if config not found
        this.calculateTaxes();
      }
    });
  }

  calculateTaxes(): void {
    const serviceAmount = this.form.get('serviceAmount')?.value || 0;
    const taxes = this.invoiceService.calculateTaxes(serviceAmount, this.issRate());
    this.calculatedTaxes.set(taxes);
  }

  onCpfCnpjInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    if (value.length <= 11) {
      // CPF: 000.000.000-00
      value = value.replace(/(\d{3})(\d)/, '$1.$2');
      value = value.replace(/(\d{3})(\d)/, '$1.$2');
      value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else {
      // CNPJ: 00.000.000/0000-00
      value = value.replace(/^(\d{2})(\d)/, '$1.$2');
      value = value.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
      value = value.replace(/\.(\d{3})(\d)/, '.$1/$2');
      value = value.replace(/(\d{4})(\d)/, '$1-$2');
    }
    
    this.form.patchValue({ clientCpfCnpj: value });
  }

  onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // (00) 00000-0000
    if (value.length > 10) {
      value = value.replace(/^(\d{2})(\d{5})(\d{4}).*/, '($1) $2-$3');
    } else if (value.length > 6) {
      value = value.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, '($1) $2-$3');
    } else if (value.length > 2) {
      value = value.replace(/^(\d{2})(\d{0,5})/, '($1) $2');
    } else {
      value = value.replace(/^(\d*)/, '($1');
    }
    
    this.form.patchValue({ clientPhone: value });
  }

  onZipCodeInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // 00000-000
    value = value.replace(/^(\d{5})(\d)/, '$1-$2');
    
    this.form.patchValue({ clientZipCode: value });
  }

  saveDraft(): void {
    if (!this.form.valid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const formData = {
      ...this.form.value,
      autoIssue: false
    };

    this.invoiceService.createInvoice(formData).subscribe({
      next: (invoice) => {
        this.successMessage.set('Rascunho salvo com sucesso!');
        setTimeout(() => {
          this.router.navigate(['/financial/invoices', invoice.id]);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao salvar rascunho');
        this.isLoading.set(false);
      }
    });
  }

  issueInvoice(): void {
    if (!this.form.valid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    if (!confirm('Deseja emitir esta nota fiscal? Esta ação não pode ser desfeita.')) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const formData = {
      ...this.form.value,
      autoIssue: true
    };

    this.invoiceService.createInvoice(formData).subscribe({
      next: (invoice) => {
        this.successMessage.set('Nota fiscal emitida com sucesso!');
        setTimeout(() => {
          this.router.navigate(['/financial/invoices', invoice.id]);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao emitir nota fiscal');
        this.isLoading.set(false);
      }
    });
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(amount);
  }

  formatPercent(value: number): string {
    return `${value.toFixed(2)}%`;
  }
}
