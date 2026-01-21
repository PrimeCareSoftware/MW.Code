import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissBatchService } from '../../../services/tiss-batch.service';
import { TissGuideService } from '../../../services/tiss-guide.service';
import { HealthInsuranceOperatorService } from '../../../services/health-insurance-operator.service';
import { HealthInsuranceOperator, TissGuide } from '../../../models/tiss.model';

@Component({
  selector: 'app-tiss-batch-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './tiss-batch-form.html',
  styleUrl: './tiss-batch-form.scss'
})
export class TissBatchFormComponent implements OnInit {
  batchForm: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  operators = signal<HealthInsuranceOperator[]>([]);
  availableGuides = signal<TissGuide[]>([]);
  selectedGuides = signal<Set<string>>(new Set());
  isLoadingGuides = signal<boolean>(false);
  
  guideCount = signal<number>(0);
  totalAmount = signal<number>(0);

  constructor(
    private fb: FormBuilder,
    private batchService: TissBatchService,
    private guideService: TissGuideService,
    private operatorService: HealthInsuranceOperatorService,
    private router: Router
  ) {
    this.batchForm = this.fb.group({
      healthInsuranceOperatorId: ['', [Validators.required]],
      competence: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadOperators();
    this.setCurrentCompetence();
  }

  setCurrentCompetence(): void {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    this.batchForm.patchValue({ competence: `${year}-${month}` });
  }

  loadOperators(): void {
    this.operatorService.getAll().subscribe({
      next: (operators) => {
        this.operators.set(operators.filter(op => op.isActive));
      },
      error: (error) => {
        console.error('Error loading operators:', error);
        this.errorMessage.set('Erro ao carregar operadoras');
      }
    });
  }

  onOperatorChange(): void {
    const operatorId = this.batchForm.get('healthInsuranceOperatorId')?.value;
    if (operatorId) {
      this.loadUnbilledGuides(operatorId);
    }
  }

  loadUnbilledGuides(operatorId: string): void {
    this.isLoadingGuides.set(true);
    this.selectedGuides.set(new Set());
    this.updateSummary();
    
    this.guideService.getUnbilledByOperator(operatorId).subscribe({
      next: (guides) => {
        this.availableGuides.set(guides);
        this.isLoadingGuides.set(false);
      },
      error: (error) => {
        console.error('Error loading guides:', error);
        this.errorMessage.set('Erro ao carregar guias não faturadas');
        this.isLoadingGuides.set(false);
      }
    });
  }

  toggleGuide(guideId: string): void {
    const selected = this.selectedGuides();
    if (selected.has(guideId)) {
      selected.delete(guideId);
    } else {
      selected.add(guideId);
    }
    this.selectedGuides.set(new Set(selected));
    this.updateSummary();
  }

  toggleAllGuides(): void {
    const selected = this.selectedGuides();
    if (selected.size === this.availableGuides().length) {
      this.selectedGuides.set(new Set());
    } else {
      const allIds = this.availableGuides().map(g => g.id);
      this.selectedGuides.set(new Set(allIds));
    }
    this.updateSummary();
  }

  isGuideSelected(guideId: string): boolean {
    return this.selectedGuides().has(guideId);
  }

  areAllGuidesSelected(): boolean {
    return this.availableGuides().length > 0 && 
           this.selectedGuides().size === this.availableGuides().length;
  }

  updateSummary(): void {
    const selected = this.selectedGuides();
    const guides = this.availableGuides().filter(g => selected.has(g.id));
    
    this.guideCount.set(guides.length);
    const total = guides.reduce((sum, guide) => sum + guide.totalAmount, 0);
    this.totalAmount.set(total);
  }

  onSubmit(): void {
    if (this.batchForm.invalid) {
      this.batchForm.markAllAsTouched();
      return;
    }

    if (this.selectedGuides().size === 0) {
      this.errorMessage.set('Selecione pelo menos uma guia para criar o lote');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.batchForm.value;
    const batchData = {
      healthInsuranceOperatorId: formValue.healthInsuranceOperatorId,
      competence: formValue.competence,
      guideIds: Array.from(this.selectedGuides())
    };

    this.batchService.create(batchData).subscribe({
      next: (batch) => {
        this.successMessage.set('Lote criado com sucesso');
        setTimeout(() => this.router.navigate(['/tiss/batches', batch.id]), 1500);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar lote');
        this.isLoading.set(false);
        console.error('Error creating batch:', error);
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.batchForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
