import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  GlobalDocumentTemplate,
  DocumentTemplateType,
  ProfessionalSpecialty,
  DocumentTemplateTypeLabels,
  ProfessionalSpecialtyLabels,
  GlobalTemplateFilter
} from '../../models/global-document-template.model';
import { GlobalDocumentTemplateService } from '../../services/global-document-template.service';

@Component({
  selector: 'app-global-template-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './global-template-list.component.html',
  styleUrls: ['./global-template-list.component.scss']
})
export class GlobalTemplateListComponent implements OnInit {
  templates = signal<GlobalDocumentTemplate[]>([]);
  filteredTemplates = signal<GlobalDocumentTemplate[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  // Filter options
  filterSpecialty = signal<ProfessionalSpecialty | null>(null);
  filterType = signal<DocumentTemplateType | null>(null);
  filterActive = signal<boolean | null>(null);
  searchText = signal('');

  // Enums for template
  DocumentTemplateType = DocumentTemplateType;
  ProfessionalSpecialty = ProfessionalSpecialty;
  DocumentTemplateTypeLabels = DocumentTemplateTypeLabels;
  ProfessionalSpecialtyLabels = ProfessionalSpecialtyLabels;

  // Arrays for dropdowns
  specialties = Object.keys(ProfessionalSpecialty)
    .filter(key => !isNaN(Number(key)))
    .map(key => ({
      value: Number(key) as ProfessionalSpecialty,
      label: ProfessionalSpecialtyLabels[Number(key) as ProfessionalSpecialty]
    }));

  templateTypes = Object.keys(DocumentTemplateType)
    .filter(key => !isNaN(Number(key)))
    .map(key => ({
      value: Number(key) as DocumentTemplateType,
      label: DocumentTemplateTypeLabels[Number(key) as DocumentTemplateType]
    }));

  constructor(
    private templateService: GlobalDocumentTemplateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading.set(true);
    this.error.set(null);

    this.templateService.getAll().subscribe({
      next: (templates) => {
        this.templates.set(templates);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar templates: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    const templates = this.templates();
    const filterSpecialty = this.filterSpecialty();
    const filterType = this.filterType();
    const filterActive = this.filterActive();
    const searchText = this.searchText();

    const filtered = templates.filter(template => {
      // Filter by specialty
      if (filterSpecialty !== null && template.specialty !== filterSpecialty) {
        return false;
      }

      // Filter by type
      if (filterType !== null && template.type !== filterType) {
        return false;
      }

      // Filter by active status
      if (filterActive !== null) {
        if (template.isActive !== filterActive) {
          return false;
        }
      }

      // Filter by search text
      if (searchText) {
        const search = searchText.toLowerCase();
        return template.name.toLowerCase().includes(search) ||
               template.description.toLowerCase().includes(search);
      }

      return true;
    });

    this.filteredTemplates.set(filtered);
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.filterSpecialty.set(null);
    this.filterType.set(null);
    this.filterActive.set(null);
    this.searchText.set('');
    this.applyFilters();
  }

  createNew(): void {
    this.router.navigate(['/global-templates/new']);
  }

  editTemplate(id: string): void {
    this.router.navigate(['/global-templates/edit', id]);
  }

  toggleActive(template: GlobalDocumentTemplate, event: Event): void {
    event.stopPropagation();

    const newStatus = !template.isActive;
    this.templateService.setActiveStatus(template.id, newStatus).subscribe({
      next: () => {
        template.isActive = newStatus;
        this.showSuccess(`Template ${template.isActive ? 'ativado' : 'desativado'} com sucesso!`);
        this.applyFilters();
      },
      error: (err) => {
        this.showError('Erro ao alterar status: ' + (err.error?.message || err.message));
      }
    });
  }

  deleteTemplate(template: GlobalDocumentTemplate, event: Event): void {
    event.stopPropagation();

    if (!confirm(`Tem certeza que deseja excluir o template "${template.name}"?`)) {
      return;
    }

    this.templateService.delete(template.id).subscribe({
      next: () => {
        const updatedTemplates = this.templates().filter(t => t.id !== template.id);
        this.templates.set(updatedTemplates);
        this.applyFilters();
        this.showSuccess('Template excluído com sucesso!');
      },
      error: (err) => {
        this.showError('Erro ao excluir template: ' + (err.error?.message || err.message));
      }
    });
  }

  private showSuccess(message: string): void {
    this.success.set(message);
    setTimeout(() => this.success.set(null), 3000);
  }

  private showError(message: string): void {
    this.error.set(message);
    setTimeout(() => this.error.set(null), 5000);
  }

  getTypeLabel(type: DocumentTemplateType): string {
    return DocumentTemplateTypeLabels[type] || 'Desconhecido';
  }

  getSpecialtyLabel(specialty: ProfessionalSpecialty): string {
    return ProfessionalSpecialtyLabels[specialty] || 'Desconhecido';
  }
}
