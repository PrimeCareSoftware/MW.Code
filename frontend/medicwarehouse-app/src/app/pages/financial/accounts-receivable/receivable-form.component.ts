import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { AccountsReceivable, ReceivableType } from '../../../models/financial.model';

@Component({
  selector: 'app-receivable-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './receivable-form.component.html',
  styleUrl: './receivable-form.component.scss'
})
export class ReceivableFormComponent implements OnInit {
  receivableForm: FormGroup;
  isEditMode = signal<boolean>(false);
  receivableId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  receivable = signal<AccountsReceivable | null>(null);

  receivableTypes = [
    { value: ReceivableType.Consultation, label: 'Consulta' },
    { value: ReceivableType.Procedure, label: 'Procedimento' },
    { value: ReceivableType.Exam, label: 'Exame' },
    { value: ReceivableType.HealthInsurance, label: 'Convênio' },
    { value: ReceivableType.Other, label: 'Outro' }
  ];

  constructor(
    private fb: FormBuilder,
    private financialService: FinancialService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.receivableForm = this.fb.group({
      documentNumber: ['', [Validators.required]],
      type: [ReceivableType.Consultation, [Validators.required]],
      dueDate: ['', [Validators.required]],
      totalAmount: ['', [Validators.required, Validators.min(0.01)]],
      description: [''],
      patientId: [''],
      appointmentId: [''],
      healthInsuranceOperatorId: [''],
      installmentNumber: [1],
      totalInstallments: [1]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.receivableId.set(id);
      this.loadReceivable(id);
    }
  }

  loadReceivable(id: string): void {
    this.isLoading.set(true);
    this.financialService.getReceivableById(id).subscribe({
      next: (data) => {
        this.receivable.set(data);
        this.receivableForm.patchValue({
          documentNumber: data.documentNumber,
          type: this.getTypeValue(data.type),
          dueDate: new Date(data.dueDate).toISOString().split('T')[0],
          totalAmount: data.totalAmount,
          description: data.description,
          installmentNumber: data.installmentNumber,
          totalInstallments: data.totalInstallments
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

  getTypeValue(typeString: string): number {
    const typeMap: { [key: string]: number } = {
      'Consultation': ReceivableType.Consultation,
      'Procedure': ReceivableType.Procedure,
      'Exam': ReceivableType.Exam,
      'HealthInsurance': ReceivableType.HealthInsurance,
      'Other': ReceivableType.Other
    };
    return typeMap[typeString] || ReceivableType.Other;
  }

  onSubmit(): void {
    if (this.receivableForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      const formValue = this.receivableForm.value;
      const createData = {
        documentNumber: formValue.documentNumber,
        type: formValue.type,
        dueDate: formValue.dueDate,
        totalAmount: parseFloat(formValue.totalAmount),
        description: formValue.description,
        patientId: formValue.patientId || undefined,
        appointmentId: formValue.appointmentId || undefined,
        healthInsuranceOperatorId: formValue.healthInsuranceOperatorId || undefined,
        installmentNumber: formValue.installmentNumber,
        totalInstallments: formValue.totalInstallments
      };

      this.financialService.createReceivable(createData).subscribe({
        next: () => {
          this.successMessage.set('Conta a receber cadastrada com sucesso!');
          this.isLoading.set(false);
          setTimeout(() => this.router.navigate(['/financial/receivables']), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cadastrar conta a receber');
          this.isLoading.set(false);
          console.error('Error creating receivable:', error);
        }
      });
    }
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
}
