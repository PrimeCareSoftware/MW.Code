import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { HelpButtonComponent } from '../../shared/help-button/help-button';
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
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-digital-prescription-form',
  standalone: true,
  imports: [HelpButtonComponent, CommonModule, ReactiveFormsModule, PrescriptionTypeSelectorComponent, Navbar],
  templateUrl: './digital-prescription-form.component.html',
  styleUrl: './digital-prescription-form.component.scss'})
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
