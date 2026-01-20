import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MedicationService } from '../../services/medication';
import { 
  Medication, 
  CreateMedicationRequest, 
  UpdateMedicationRequest,
  MedicationCategory,
  ControlledSubstanceList,
  MEDICATION_CATEGORY_LABELS,
  CONTROLLED_SUBSTANCE_LABELS
} from '../../models/medication-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-medications-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './medications-list.html',
  styleUrl: './medications-list.scss'
})
export class MedicationsList implements OnInit {
  medications = signal<Medication[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingMedication: Medication | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  // Filter states
  searchTerm = '';
  selectedCategory: number | undefined;

  // Enums for template
  MedicationCategory = MedicationCategory;
  ControlledSubstanceList = ControlledSubstanceList;
  medicationCategoryLabels = MEDICATION_CATEGORY_LABELS;
  controlledSubstanceLabels = CONTROLLED_SUBSTANCE_LABELS;

  formData: CreateMedicationRequest = this.getEmptyFormData();

  constructor(private medicationService: MedicationService) {}

  ngOnInit(): void {
    this.loadMedications();
  }

  loadMedications(): void {
    this.loading.set(true);
    this.error.set(null);

    this.medicationService.getAllMedications(this.searchTerm, this.selectedCategory).subscribe({
      next: (data) => {
        this.medications.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar medicações');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.loadMedications();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = undefined;
    this.loadMedications();
  }

  openCreateModal(): void {
    this.editingMedication = null;
    this.formData = this.getEmptyFormData();
    this.showModal = true;
  }

  openEditModal(medication: Medication): void {
    this.editingMedication = medication;
    this.formData = {
      name: medication.name,
      genericName: medication.genericName,
      manufacturer: medication.manufacturer,
      activeIngredient: medication.activeIngredient,
      dosage: medication.dosage,
      pharmaceuticalForm: medication.pharmaceuticalForm,
      concentration: medication.concentration,
      administrationRoute: medication.administrationRoute,
      category: medication.category,
      requiresPrescription: medication.requiresPrescription,
      isControlled: medication.isControlled,
      controlledList: medication.controlledList,
      anvisaRegistration: medication.anvisaRegistration,
      barcode: medication.barcode,
      description: medication.description
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingMedication = null;
    this.modalError.set(null);
  }

  onSubmit(): void {
    this.submitting.set(true);
    this.modalError.set(null);

    if (this.editingMedication) {
      // Update
      this.medicationService.updateMedication(this.editingMedication.id, this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadMedications();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao atualizar medicação');
          this.submitting.set(false);
        }
      });
    } else {
      // Create
      this.medicationService.createMedication(this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadMedications();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao criar medicação');
          this.submitting.set(false);
        }
      });
    }
  }

  toggleStatus(medication: Medication): void {
    const action = medication.isActive ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} esta medicação?`)) {
      return;
    }

    const operation = medication.isActive
      ? this.medicationService.deactivateMedication(medication.id)
      : this.medicationService.activateMedication(medication.id);

    operation.subscribe({
      next: () => {
        this.loadMedications();
      },
      error: (err) => {
        alert(err.error?.message || `Erro ao ${action} medicação`);
      }
    });
  }

  getCategoryLabel(category: MedicationCategory): string {
    return this.medicationCategoryLabels[category];
  }

  getControlledListLabel(list: ControlledSubstanceList | undefined): string {
    return list ? this.controlledSubstanceLabels[list] : 'Não controlado';
  }

  getCategoryOptions(): { value: number; label: string }[] {
    return Object.keys(MedicationCategory)
      .filter(key => !isNaN(Number(key)))
      .map(key => ({
        value: Number(key),
        label: this.medicationCategoryLabels[Number(key) as MedicationCategory]
      }));
  }

  getControlledListOptions(): { value: number; label: string }[] {
    return Object.keys(ControlledSubstanceList)
      .filter(key => !isNaN(Number(key)))
      .map(key => ({
        value: Number(key),
        label: this.controlledSubstanceLabels[Number(key) as ControlledSubstanceList]
      }));
  }

  private getEmptyFormData(): CreateMedicationRequest {
    return {
      name: '',
      genericName: '',
      manufacturer: '',
      activeIngredient: '',
      dosage: '',
      pharmaceuticalForm: '',
      concentration: '',
      administrationRoute: '',
      category: MedicationCategory.Other,
      requiresPrescription: false,
      isControlled: false,
      controlledList: ControlledSubstanceList.None,
      anvisaRegistration: '',
      barcode: '',
      description: ''
    };
  }
}
