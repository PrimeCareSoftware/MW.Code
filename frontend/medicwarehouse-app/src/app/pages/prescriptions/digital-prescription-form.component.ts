import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { DigitalPrescriptionService } from '../../services/prescriptions/digital-prescription.service';
import { 
  CreateDigitalPrescription, 
  DigitalPrescription, 
  DigitalPrescriptionItem,
  PrescriptionType,
  PRESCRIPTION_TYPES,
  PrescriptionTypeInfo
} from '../../models/prescriptions/digital-prescription.model';
import { PrescriptionTypeSelectorComponent } from './prescription-type-selector.component';

@Component({
  selector: 'app-digital-prescription-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PrescriptionTypeSelectorComponent],
  template: `
    <div class="digital-prescription-form">
      <h2>Prescrição Digital</h2>
      <p class="subtitle">Conforme CFM 1.643/2002 e Portaria ANVISA 344/1998</p>

      @if (errorMessage()) {
        <div class="alert alert-error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert alert-success">{{ successMessage() }}</div>
      }

      @if (!selectedType()) {
        <app-prescription-type-selector 
          (typeSelected)="onTypeSelected($event)">
        </app-prescription-type-selector>
      } @else {
        <div class="selected-type-info">
          <div class="type-badge" [style.background-color]="getTypeColor()">
            {{ getTypeName() }}
          </div>
          <button 
            type="button" 
            class="btn-change-type" 
            (click)="changeType()"
            [disabled]="isSaving()"
          >
            Alterar Tipo
          </button>
        </div>

        <form [formGroup]="prescriptionForm" (ngSubmit)="onSubmit()">
          <!-- Doctor Information (Auto-filled) -->
          <div class="section">
            <h3>Informações do Médico</h3>
            <div class="form-row">
              <div class="form-group col-md-6">
                <label for="doctorName">Nome do Médico *</label>
                <input
                  type="text"
                  id="doctorName"
                  formControlName="doctorName"
                  readonly
                />
              </div>

              <div class="form-group col-md-3">
                <label for="doctorCRM">CRM *</label>
                <input
                  type="text"
                  id="doctorCRM"
                  formControlName="doctorCRM"
                  readonly
                />
              </div>

              <div class="form-group col-md-3">
                <label for="doctorCRMState">UF *</label>
                <input
                  type="text"
                  id="doctorCRMState"
                  formControlName="doctorCRMState"
                  readonly
                />
              </div>
            </div>
          </div>

          <!-- Patient Information (Auto-filled) -->
          <div class="section">
            <h3>Informações do Paciente</h3>
            <div class="form-row">
              <div class="form-group col-md-8">
                <label for="patientName">Nome do Paciente *</label>
                <input
                  type="text"
                  id="patientName"
                  formControlName="patientName"
                  readonly
                />
              </div>

              <div class="form-group col-md-4">
                <label for="patientDocument">CPF *</label>
                <input
                  type="text"
                  id="patientDocument"
                  formControlName="patientDocument"
                  readonly
                />
              </div>
            </div>
          </div>

          <!-- Medications -->
          <div class="section">
            <h3>Medicamentos Prescritos</h3>
            
            @if (items().length === 0) {
              <div class="empty-items">
                <p>Nenhum medicamento adicionado. Clique no botão abaixo para adicionar.</p>
              </div>
            }

            @for (item of items(); track $index) {
              <div class="medication-item">
                <div class="item-header">
                  <span class="item-number">{{ $index + 1 }}.</span>
                  <h4>{{ item.medicationName }}</h4>
                  <button
                    type="button"
                    class="btn-delete"
                    (click)="removeItem($index)"
                    [disabled]="isSaving()"
                  >
                    🗑️
                  </button>
                </div>
                <div class="item-details">
                  <p><strong>Dosagem:</strong> {{ item.dosage }}</p>
                  <p><strong>Forma Farmacêutica:</strong> {{ item.pharmaceuticalForm }}</p>
                  <p><strong>Frequência:</strong> {{ item.frequency }}</p>
                  <p><strong>Duração:</strong> {{ item.durationDays }} dias</p>
                  <p><strong>Quantidade:</strong> {{ item.quantity }}</p>
                  @if (item.instructions) {
                    <p><strong>Instruções:</strong> {{ item.instructions }}</p>
                  }
                  @if (item.isControlledSubstance && item.controlledList) {
                    <div class="controlled-badge">
                      🔒 Substância Controlada - Lista {{ item.controlledList }}
                    </div>
                  }
                </div>
              </div>
            }

            @if (showItemForm()) {
              <div class="item-form" formGroupName="currentItem">
                <h4>{{ editingItemIndex() !== null ? 'Editar' : 'Adicionar' }} Medicamento</h4>
                
                <div class="form-row">
                  <div class="form-group col-md-8">
                    <label for="medicationName">Nome do Medicamento *</label>
                    <input
                      type="text"
                      id="medicationName"
                      formControlName="medicationName"
                      placeholder="Ex: Paracetamol"
                      [class.invalid]="isItemFieldInvalid('medicationName')"
                    />
                    @if (isItemFieldInvalid('medicationName')) {
                      <small class="error-text">Nome do medicamento é obrigatório</small>
                    }
                  </div>

                  <div class="form-group col-md-4">
                    <label for="quantity">Quantidade *</label>
                    <input
                      type="number"
                      id="quantity"
                      formControlName="quantity"
                      min="1"
                      placeholder="Ex: 20"
                      [class.invalid]="isItemFieldInvalid('quantity')"
                    />
                    @if (isItemFieldInvalid('quantity')) {
                      <small class="error-text">Quantidade é obrigatória</small>
                    }
                  </div>
                </div>

                <div class="form-row">
                  <div class="form-group col-md-6">
                    <label for="dosage">Dosagem *</label>
                    <input
                      type="text"
                      id="dosage"
                      formControlName="dosage"
                      placeholder="Ex: 750mg"
                      [class.invalid]="isItemFieldInvalid('dosage')"
                    />
                    @if (isItemFieldInvalid('dosage')) {
                      <small class="error-text">Dosagem é obrigatória</small>
                    }
                  </div>

                  <div class="form-group col-md-6">
                    <label for="pharmaceuticalForm">Forma Farmacêutica *</label>
                    <input
                      type="text"
                      id="pharmaceuticalForm"
                      formControlName="pharmaceuticalForm"
                      placeholder="Ex: Comprimido"
                      [class.invalid]="isItemFieldInvalid('pharmaceuticalForm')"
                    />
                    @if (isItemFieldInvalid('pharmaceuticalForm')) {
                      <small class="error-text">Forma farmacêutica é obrigatória</small>
                    }
                  </div>
                </div>

                <div class="form-row">
                  <div class="form-group col-md-6">
                    <label for="frequency">Frequência *</label>
                    <input
                      type="text"
                      id="frequency"
                      formControlName="frequency"
                      placeholder="Ex: 1 comprimido 8/8h"
                      [class.invalid]="isItemFieldInvalid('frequency')"
                    />
                    @if (isItemFieldInvalid('frequency')) {
                      <small class="error-text">Frequência é obrigatória</small>
                    }
                  </div>

                  <div class="form-group col-md-6">
                    <label for="durationDays">Duração (dias) *</label>
                    <input
                      type="number"
                      id="durationDays"
                      formControlName="durationDays"
                      min="1"
                      placeholder="Ex: 7"
                      [class.invalid]="isItemFieldInvalid('durationDays')"
                    />
                    @if (isItemFieldInvalid('durationDays')) {
                      <small class="error-text">Duração é obrigatória</small>
                    }
                  </div>
                </div>

                <div class="form-group">
                  <label for="instructions">Instruções de Uso</label>
                  <textarea
                    id="instructions"
                    formControlName="instructions"
                    rows="2"
                    placeholder="Ex: Tomar após as refeições com água"
                  ></textarea>
                </div>

                @if (requiresControlledInfo()) {
                  <div class="controlled-section">
                    <h5>⚠️ Informações de Substância Controlada</h5>
                    
                    <div class="form-group">
                      <label class="checkbox-label">
                        <input
                          type="checkbox"
                          formControlName="isControlledSubstance"
                        />
                        <span>Esta é uma substância controlada</span>
                      </label>
                    </div>

                    @if (currentItemForm.get('isControlledSubstance')?.value) {
                      <div class="form-group">
                        <label for="controlledList">Lista de Controle ANVISA *</label>
                        <select
                          id="controlledList"
                          formControlName="controlledList"
                          [class.invalid]="isItemFieldInvalid('controlledList')"
                        >
                          <option value="">Selecione...</option>
                          <option value="A1">A1 - Entorpecentes</option>
                          <option value="A2">A2 - Entorpecentes</option>
                          <option value="A3">A3 - Psicotrópicos</option>
                          <option value="B1">B1 - Psicotrópicos</option>
                          <option value="B2">B2 - Anorexígenos</option>
                          <option value="C1">C1 - Outras Substâncias</option>
                          <option value="C2">C2 - Retinóides</option>
                          <option value="C3">C3 - Imunossupressores</option>
                          <option value="C4">C4 - Antirretrovirais</option>
                          <option value="C5">C5 - Anabolizantes</option>
                        </select>
                        @if (isItemFieldInvalid('controlledList')) {
                          <small class="error-text">Lista de controle é obrigatória para substâncias controladas</small>
                        }
                      </div>
                    }
                  </div>
                }

                <div class="form-actions">
                  <button
                    type="button"
                    class="btn btn-secondary"
                    (click)="cancelItemEdit()"
                  >
                    Cancelar
                  </button>
                  <button
                    type="button"
                    class="btn btn-primary"
                    (click)="saveItem()"
                    [disabled]="currentItemForm.invalid"
                  >
                    {{ editingItemIndex() !== null ? 'Atualizar' : 'Adicionar' }} Medicamento
                  </button>
                </div>
              </div>
            }

            @if (!showItemForm()) {
              <button
                type="button"
                class="btn btn-add-item"
                (click)="addNewItem()"
                [disabled]="isSaving()"
              >
                ➕ Adicionar Medicamento
              </button>
            }
          </div>

          <!-- Observations -->
          <div class="section">
            <h3>Observações Gerais</h3>
            <div class="form-group">
              <textarea
                id="notes"
                formControlName="notes"
                rows="4"
                placeholder="Observações adicionais sobre a prescrição..."
              ></textarea>
            </div>
          </div>

          <!-- Actions -->
          <div class="form-actions-main">
            <button
              type="button"
              class="btn btn-secondary"
              (click)="onCancel()"
              [disabled]="isSaving()"
            >
              Cancelar
            </button>
            <button
              type="button"
              class="btn btn-outline"
              (click)="onPreview()"
              [disabled]="items().length === 0 || isSaving()"
            >
              👁️ Visualizar
            </button>
            <button
              type="submit"
              class="btn btn-primary"
              [disabled]="prescriptionForm.invalid || items().length === 0 || isSaving()"
            >
              {{ isSaving() ? 'Salvando...' : 'Salvar Prescrição' }}
            </button>
          </div>
        </form>
      }
    </div>
  `,
  styles: [`
    .digital-prescription-form {
      padding: 1.5rem;
    }

    h2 {
      color: #333;
      margin-bottom: 0.5rem;
    }

    .subtitle {
      color: #666;
      font-size: 0.9rem;
      margin-bottom: 2rem;
    }

    .selected-type-info {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .type-badge {
      padding: 0.5rem 1rem;
      border-radius: 6px;
      color: white;
      font-weight: 500;
      font-size: 1rem;
    }

    .btn-change-type {
      padding: 0.5rem 1rem;
      background: white;
      border: 1px solid #ddd;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.9rem;
      transition: all 0.2s;
    }

    .btn-change-type:hover:not(:disabled) {
      background: #f8f9fa;
      border-color: #007bff;
    }

    .btn-change-type:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .section {
      margin-bottom: 2rem;
      padding: 1.5rem;
      background: white;
      border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .section h3 {
      margin-bottom: 1.5rem;
      color: #333;
      font-size: 1.2rem;
      border-bottom: 2px solid #007bff;
      padding-bottom: 0.5rem;
    }

    .form-row {
      display: flex;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .form-group {
      flex: 1;
      margin-bottom: 0;
    }

    .form-group.col-md-3 {
      flex: 0 0 calc(25% - 0.75rem);
    }

    .form-group.col-md-4 {
      flex: 0 0 calc(33.333% - 0.667rem);
    }

    .form-group.col-md-6 {
      flex: 0 0 calc(50% - 0.5rem);
    }

    .form-group.col-md-8 {
      flex: 0 0 calc(66.666% - 0.5rem);
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
      font-size: 0.9rem;
    }

    .form-group input,
    .form-group select,
    .form-group textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-family: inherit;
      font-size: 0.95rem;
    }

    .form-group input:read-only {
      background: #f8f9fa;
      color: #666;
    }

    .form-group input.invalid,
    .form-group select.invalid,
    .form-group textarea.invalid {
      border-color: #dc3545;
      background: #fff5f5;
    }

    .empty-items {
      padding: 2rem;
      text-align: center;
      background: #f8f9fa;
      border-radius: 6px;
      margin-bottom: 1rem;
    }

    .empty-items p {
      color: #666;
      margin: 0;
    }

    .medication-item {
      padding: 1rem;
      margin-bottom: 1rem;
      background: #f8f9fa;
      border: 1px solid #ddd;
      border-radius: 6px;
    }

    .item-header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 0.75rem;
    }

    .item-number {
      font-size: 1.2rem;
      font-weight: bold;
      color: #007bff;
    }

    .item-header h4 {
      flex: 1;
      margin: 0;
      color: #333;
      font-size: 1.1rem;
    }

    .btn-delete {
      background: #dc3545;
      color: white;
      border: none;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.9rem;
      transition: background-color 0.2s;
    }

    .btn-delete:hover:not(:disabled) {
      background: #c82333;
    }

    .btn-delete:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .item-details p {
      margin: 0.25rem 0;
      font-size: 0.9rem;
      color: #555;
    }

    .controlled-badge {
      margin-top: 0.5rem;
      padding: 0.5rem;
      background: #fff3cd;
      border: 1px solid #ffc107;
      border-radius: 4px;
      color: #856404;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .item-form {
      padding: 1.5rem;
      background: #fff;
      border: 2px solid #007bff;
      border-radius: 6px;
      margin-bottom: 1rem;
    }

    .item-form h4 {
      margin-bottom: 1rem;
      color: #007bff;
    }

    .controlled-section {
      margin-top: 1rem;
      padding: 1rem;
      background: #fff3cd;
      border-radius: 6px;
    }

    .controlled-section h5 {
      margin-bottom: 1rem;
      color: #856404;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      font-weight: 500;
    }

    .checkbox-label input[type="checkbox"] {
      width: auto;
      margin: 0;
    }

    .btn-add-item {
      width: 100%;
      padding: 1rem;
      background: #28a745;
      color: white;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-add-item:hover:not(:disabled) {
      background: #218838;
    }

    .btn-add-item:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .form-actions,
    .form-actions-main {
      margin-top: 1.5rem;
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-primary {
      background: #007bff;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #0056b3;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
    }

    .btn-outline {
      background: white;
      color: #007bff;
      border: 1px solid #007bff;
    }

    .btn-outline:hover:not(:disabled) {
      background: #007bff;
      color: white;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .error-text {
      display: block;
      margin-top: 0.25rem;
      color: #dc3545;
      font-size: 0.85rem;
    }

    .alert {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      border-radius: 4px;
    }

    .alert-error {
      background: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .alert-success {
      background: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    @media (max-width: 768px) {
      .form-row {
        flex-direction: column;
      }

      .form-group.col-md-3,
      .form-group.col-md-4,
      .form-group.col-md-6,
      .form-group.col-md-8 {
        flex: 1;
      }
    }
  `]
})
export class DigitalPrescriptionFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Input() patientId!: string;
  @Input() doctorId!: string;
  @Input() patientName!: string;
  @Input() patientDocument!: string;
  @Input() doctorName!: string;
  @Input() doctorCRM!: string;
  @Input() doctorCRMState!: string;
  
  @Output() prescriptionSaved = new EventEmitter<DigitalPrescription>();
  @Output() cancelled = new EventEmitter<void>();
  @Output() previewRequested = new EventEmitter<CreateDigitalPrescription>();

  prescriptionForm: FormGroup;
  currentItemForm!: FormGroup;
  
  selectedType = signal<PrescriptionType | null>(null);
  items = signal<DigitalPrescriptionItem[]>([]);
  showItemForm = signal(false);
  editingItemIndex = signal<number | null>(null);
  isSaving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private fb: FormBuilder,
    private prescriptionService: DigitalPrescriptionService
  ) {
    this.prescriptionForm = this.fb.group({
      doctorName: ['', Validators.required],
      doctorCRM: ['', Validators.required],
      doctorCRMState: ['', Validators.required],
      patientName: ['', Validators.required],
      patientDocument: ['', Validators.required],
      notes: [''],
      currentItem: this.createItemFormGroup()
    });
  }

  ngOnInit() {
    // Auto-fill doctor and patient information
    this.prescriptionForm.patchValue({
      doctorName: this.doctorName,
      doctorCRM: this.doctorCRM,
      doctorCRMState: this.doctorCRMState,
      patientName: this.patientName,
      patientDocument: this.patientDocument
    });

    this.currentItemForm = this.prescriptionForm.get('currentItem') as FormGroup;
  }

  createItemFormGroup(): FormGroup {
    return this.fb.group({
      medicationId: ['temp-' + Date.now()],
      medicationName: ['', Validators.required],
      dosage: ['', Validators.required],
      pharmaceuticalForm: ['', Validators.required],
      frequency: ['', Validators.required],
      durationDays: [null, [Validators.required, Validators.min(1)]],
      quantity: [null, [Validators.required, Validators.min(1)]],
      instructions: [''],
      isControlledSubstance: [false],
      controlledList: ['']
    });
  }

  onTypeSelected(type: PrescriptionType) {
    this.selectedType.set(type);
  }

  changeType() {
    if (this.items().length > 0) {
      if (!confirm('Alterar o tipo de receita removerá todos os medicamentos adicionados. Deseja continuar?')) {
        return;
      }
      this.items.set([]);
    }
    this.selectedType.set(null);
  }

  getTypeName(): string {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.selectedType());
    return type?.name || '';
  }

  getTypeColor(): string {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.selectedType());
    const colorMap: Record<string, string> = {
      'primary': '#007bff',
      'warn': '#ff9800',
      'accent': '#e91e63'
    };
    return colorMap[type?.color || 'primary'] || '#007bff';
  }

  requiresControlledInfo(): boolean {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.selectedType());
    return type?.requiresSNGPC || false;
  }

  addNewItem() {
    this.showItemForm.set(true);
    this.editingItemIndex.set(null);
    this.currentItemForm.reset({
      medicationId: 'temp-' + Date.now(),
      isControlledSubstance: false
    });
  }

  saveItem() {
    if (this.currentItemForm.invalid) {
      Object.keys(this.currentItemForm.controls).forEach(key => {
        this.currentItemForm.get(key)?.markAsTouched();
      });
      return;
    }

    const itemData: DigitalPrescriptionItem = {
      ...this.currentItemForm.value,
      controlledList: this.currentItemForm.value.isControlledSubstance 
        ? this.currentItemForm.value.controlledList 
        : undefined
    };

    if (this.editingItemIndex() !== null) {
      // Update existing item
      const updatedItems = [...this.items()];
      updatedItems[this.editingItemIndex()!] = itemData;
      this.items.set(updatedItems);
    } else {
      // Add new item
      this.items.update(items => [...items, itemData]);
    }

    this.cancelItemEdit();
  }

  removeItem(index: number) {
    if (confirm('Deseja remover este medicamento?')) {
      this.items.update(items => items.filter((_, i) => i !== index));
    }
  }

  cancelItemEdit() {
    this.showItemForm.set(false);
    this.editingItemIndex.set(null);
    this.currentItemForm.reset({
      medicationId: 'temp-' + Date.now(),
      isControlledSubstance: false
    });
  }

  isItemFieldInvalid(fieldName: string): boolean {
    const field = this.currentItemForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  onPreview() {
    const prescriptionData = this.buildPrescriptionData();
    this.previewRequested.emit(prescriptionData);
  }

  onSubmit() {
    if (this.prescriptionForm.invalid || this.items().length === 0) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios e adicione pelo menos um medicamento.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const prescriptionData = this.buildPrescriptionData();

    this.prescriptionService.create(prescriptionData).subscribe({
      next: (prescription) => {
        this.successMessage.set('Prescrição criada com sucesso!');
        this.prescriptionSaved.emit(prescription);
        this.isSaving.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar prescrição: ' + (error.error?.message || 'Erro desconhecido'));
        this.isSaving.set(false);
      }
    });
  }

  private buildPrescriptionData(): CreateDigitalPrescription {
    const formValue = this.prescriptionForm.value;
    
    return {
      medicalRecordId: this.medicalRecordId,
      patientId: this.patientId,
      doctorId: this.doctorId,
      type: this.selectedType()!,
      doctorName: formValue.doctorName,
      doctorCRM: formValue.doctorCRM,
      doctorCRMState: formValue.doctorCRMState,
      patientName: formValue.patientName,
      patientDocument: formValue.patientDocument,
      notes: formValue.notes || undefined,
      items: this.items()
    };
  }

  onCancel() {
    if (this.items().length > 0) {
      if (!confirm('Tem certeza que deseja cancelar? Todos os medicamentos adicionados serão perdidos.')) {
        return;
      }
    }
    this.cancelled.emit();
  }
}
