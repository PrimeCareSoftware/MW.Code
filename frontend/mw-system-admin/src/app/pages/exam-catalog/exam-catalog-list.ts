import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ExamCatalogService } from '../../services/exam-catalog';
import { 
  ExamCatalog, 
  CreateExamCatalogRequest, 
  UpdateExamCatalogRequest,
  ExamType,
  EXAM_TYPE_LABELS
} from '../../models/exam-catalog-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-exam-catalog-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './exam-catalog-list.html',
  styleUrl: './exam-catalog-list.scss'
})
export class ExamCatalogList implements OnInit {
  exams = signal<ExamCatalog[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingExam: ExamCatalog | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  // Filter states
  searchTerm = '';
  selectedType: number | undefined;

  // Enums for template
  ExamType = ExamType;
  examTypeLabels = EXAM_TYPE_LABELS;

  formData: CreateExamCatalogRequest = this.getEmptyFormData();

  constructor(private examCatalogService: ExamCatalogService) {}

  ngOnInit(): void {
    this.loadExams();
  }

  loadExams(): void {
    this.loading.set(true);
    this.error.set(null);

    this.examCatalogService.getAllExams(this.searchTerm, this.selectedType).subscribe({
      next: (data) => {
        this.exams.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar exames');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.loadExams();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedType = undefined;
    this.loadExams();
  }

  openCreateModal(): void {
    this.editingExam = null;
    this.formData = this.getEmptyFormData();
    this.showModal = true;
  }

  openEditModal(exam: ExamCatalog): void {
    this.editingExam = exam;
    this.formData = {
      name: exam.name,
      description: exam.description,
      examType: exam.examType,
      category: exam.category,
      preparation: exam.preparation,
      synonyms: exam.synonyms,
      tussCode: exam.tussCode
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingExam = null;
    this.modalError.set(null);
  }

  onSubmit(): void {
    this.submitting.set(true);
    this.modalError.set(null);

    if (this.editingExam) {
      // Update
      this.examCatalogService.updateExam(this.editingExam.id, this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadExams();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao atualizar exame');
          this.submitting.set(false);
        }
      });
    } else {
      // Create
      this.examCatalogService.createExam(this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadExams();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao criar exame');
          this.submitting.set(false);
        }
      });
    }
  }

  toggleStatus(exam: ExamCatalog): void {
    const action = exam.isActive ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} este exame?`)) {
      return;
    }

    const operation = exam.isActive
      ? this.examCatalogService.deactivateExam(exam.id)
      : this.examCatalogService.activateExam(exam.id);

    operation.subscribe({
      next: () => {
        this.loadExams();
      },
      error: (err) => {
        alert(err.error?.message || `Erro ao ${action} exame`);
      }
    });
  }

  getTypeLabel(type: ExamType): string {
    return this.examTypeLabels[type];
  }

  getTypeOptions(): { value: number; label: string }[] {
    return Object.keys(ExamType)
      .filter(key => !isNaN(Number(key)))
      .map(key => ({
        value: Number(key),
        label: this.examTypeLabels[Number(key) as ExamType]
      }));
  }

  private getEmptyFormData(): CreateExamCatalogRequest {
    return {
      name: '',
      description: '',
      examType: ExamType.Laboratory,
      category: '',
      preparation: '',
      synonyms: '',
      tussCode: ''
    };
  }
}
