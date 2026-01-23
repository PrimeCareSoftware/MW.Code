import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { ProcedureService } from '../../services/procedure';
import { Procedure, ProcedureCategory, ProcedureCategoryLabels } from '../../models/procedure.model';

@Component({
  selector: 'app-procedure-form',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './procedure-form.html',
  styleUrl: './procedure-form.scss'
})
export class ProcedureForm implements OnInit {
  procedureForm: FormGroup;
  procedureId = signal<string | null>(null);
  procedure = signal<Procedure | null>(null);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  procedureCategories = Object.values(ProcedureCategory).filter(v => typeof v === 'number') as ProcedureCategory[];
  procedureCategoryLabels = ProcedureCategoryLabels;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private procedureService: ProcedureService
  ) {
    this.procedureForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      code: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', Validators.required],
      category: [ProcedureCategory.Other, Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      durationMinutes: [30, [Validators.required, Validators.min(1)]],
      requiresMaterials: [false]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.procedureId.set(id);
      this.loadProcedure(id);
    }
  }

  loadProcedure(id: string): void {
    this.isLoading.set(true);
    this.procedureService.getById(id).subscribe({
      next: (procedure) => {
        this.procedure.set(procedure);
        this.procedureForm.patchValue({
          name: procedure.name,
          code: procedure.code,
          description: procedure.description,
          category: procedure.category,
          price: procedure.price,
          durationMinutes: procedure.durationMinutes,
          requiresMaterials: procedure.requiresMaterials
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading procedure:', error);
        this.errorMessage.set('Erro ao carregar procedimento');
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.procedureForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios corretamente.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.procedureForm.value;
    
    if (this.procedureId()) {
      // Update existing procedure
      this.procedureService.update(this.procedureId()!, {
        name: formValue.name,
        description: formValue.description,
        category: formValue.category,
        price: formValue.price,
        durationMinutes: formValue.durationMinutes,
        requiresMaterials: formValue.requiresMaterials
      }).subscribe({
        next: () => {
          this.successMessage.set('Procedimento atualizado com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => {
            this.router.navigate(['/procedures']);
          }, 1500);
        },
        error: (error) => {
          console.error('Error updating procedure:', error);
          this.errorMessage.set('Erro ao atualizar procedimento');
          this.isSaving.set(false);
        }
      });
    } else {
      // Create new procedure
      this.procedureService.create({
        name: formValue.name,
        code: formValue.code,
        description: formValue.description,
        category: formValue.category,
        price: formValue.price,
        durationMinutes: formValue.durationMinutes,
        requiresMaterials: formValue.requiresMaterials
      }).subscribe({
        next: () => {
          this.successMessage.set('Procedimento criado com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => {
            this.router.navigate(['/procedures']);
          }, 1500);
        },
        error: (error) => {
          console.error('Error creating procedure:', error);
          this.errorMessage.set('Erro ao criar procedimento');
          this.isSaving.set(false);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/procedures']);
  }
}
