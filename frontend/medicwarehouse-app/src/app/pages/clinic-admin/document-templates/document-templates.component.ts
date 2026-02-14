import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { 
  DocumentTemplate, 
  DocumentTemplateType, 
  ProfessionalSpecialty,
  DocumentTemplateTypeLabels,
  ProfessionalSpecialtyLabels
} from '../../../models/document-template.model';
import { GlobalDocumentTemplate } from '../../../models/global-document-template.model';
import { DocumentTemplateService } from '../../../services/document-template.service';

@Component({
  selector: 'app-document-templates',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './document-templates.component.html',
  styleUrls: ['./document-templates.component.scss']
})
export class DocumentTemplatesComponent implements OnInit {
  templates: DocumentTemplate[] = [];
  filteredTemplates: DocumentTemplate[] = [];
  globalTemplates: GlobalDocumentTemplate[] = [];
  filteredGlobalTemplates: GlobalDocumentTemplate[] = [];
  showGlobalTemplates = false;
  loading = false;
  loadingGlobal = false;
  error: string | null = null;
  success: string | null = null;
  
  // Filter options
  filterSpecialty: ProfessionalSpecialty | null = null;
  filterType: DocumentTemplateType | null = null;
  filterActive: boolean | null = null;
  searchText = '';
  
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
    private templateService: DocumentTemplateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading = true;
    this.error = null;
    
    this.templateService.getAll().subscribe({
      next: (templates) => {
        this.templates = templates;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar templates: ' + (err.error?.message || err.message);
        this.loading = false;
      }
    });
  }

  toggleGlobalTemplates(): void {
    this.showGlobalTemplates = !this.showGlobalTemplates;
    if (this.showGlobalTemplates && this.globalTemplates.length === 0) {
      this.loadGlobalTemplates();
    }
  }

  loadGlobalTemplates(): void {
    this.loadingGlobal = true;
    this.error = null;

    this.templateService.getGlobalTemplates().subscribe({
      next: (templates) => {
        this.globalTemplates = templates;
        this.applyGlobalFilters();
        this.loadingGlobal = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar templates globais: ' + (err.error?.message || err.message);
        this.loadingGlobal = false;
      }
    });
  }

  applyGlobalFilters(): void {
    this.filteredGlobalTemplates = this.globalTemplates.filter(template => {
      // Filter by specialty
      if (this.filterSpecialty !== null && template.specialty !== this.filterSpecialty) {
        return false;
      }
      
      // Filter by type
      if (this.filterType !== null && template.type !== this.filterType) {
        return false;
      }
      
      // Filter by search text
      if (this.searchText) {
        const search = this.searchText.toLowerCase();
        return template.name.toLowerCase().includes(search) ||
               template.description.toLowerCase().includes(search);
      }
      
      return true;
    });
  }

  useGlobalTemplate(globalTemplate: any): void {
    if (!confirm(`Deseja criar um template baseado em "${globalTemplate.name}"?`)) {
      return;
    }

    this.loading = true;
    this.templateService.createFromGlobalTemplate(globalTemplate.id).subscribe({
      next: (newTemplate) => {
        this.showSuccess('Template criado com sucesso a partir do template global!');
        this.templates = [newTemplate, ...this.templates];
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.showError('Erro ao criar template: ' + (err.error?.message || err.message));
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredTemplates = this.templates.filter(template => {
      // Filter by specialty
      if (this.filterSpecialty !== null && template.specialty !== this.filterSpecialty) {
        return false;
      }
      
      // Filter by type
      if (this.filterType !== null && template.type !== this.filterType) {
        return false;
      }
      
      // Filter by active status
      if (this.filterActive !== null && template.isActive !== this.filterActive) {
        return false;
      }
      
      // Filter by search text
      if (this.searchText) {
        const search = this.searchText.toLowerCase();
        return template.name.toLowerCase().includes(search) ||
               template.description.toLowerCase().includes(search);
      }
      
      return true;
    });

    // Also apply filters to global templates if shown
    if (this.showGlobalTemplates) {
      this.applyGlobalFilters();
    }
  }

  clearFilters(): void {
    this.filterSpecialty = null;
    this.filterType = null;
    this.filterActive = null;
    this.searchText = '';
    this.applyFilters();
  }

  createNew(): void {
    this.router.navigate(['/clinic-admin/document-templates/new']);
  }

  editTemplate(id: string): void {
    this.router.navigate(['/clinic-admin/document-templates/edit', id]);
  }

  viewTemplate(id: string): void {
    this.router.navigate(['/clinic-admin/document-templates/view', id]);
  }

  toggleActive(template: DocumentTemplate, event: Event): void {
    event.stopPropagation();
    
    const action = template.isActive ? 'deactivate' : 'activate';
    const service$ = template.isActive 
      ? this.templateService.deactivate(template.id)
      : this.templateService.activate(template.id);
    
    service$.subscribe({
      next: () => {
        template.isActive = !template.isActive;
        this.showSuccess(`Template ${template.isActive ? 'ativado' : 'desativado'} com sucesso!`);
      },
      error: (err) => {
        this.showError('Erro ao alterar status: ' + (err.error?.message || err.message));
      }
    });
  }

  deleteTemplate(template: DocumentTemplate, event: Event): void {
    event.stopPropagation();
    
    if (template.isSystem) {
      this.showError('Templates do sistema não podem ser excluídos');
      return;
    }
    
    if (!confirm(`Tem certeza que deseja excluir o template "${template.name}"?`)) {
      return;
    }
    
    this.templateService.delete(template.id).subscribe({
      next: () => {
        this.templates = this.templates.filter(t => t.id !== template.id);
        this.applyFilters();
        this.showSuccess('Template excluído com sucesso!');
      },
      error: (err) => {
        this.showError('Erro ao excluir template: ' + (err.error?.message || err.message));
      }
    });
  }

  private showSuccess(message: string): void {
    this.success = message;
    setTimeout(() => this.success = null, 3000);
  }

  private showError(message: string): void {
    this.error = message;
    setTimeout(() => this.error = null, 5000);
  }

  getTypeLabel(type: DocumentTemplateType): string {
    return DocumentTemplateTypeLabels[type] || 'Desconhecido';
  }

  getSpecialtyLabel(specialty: ProfessionalSpecialty): string {
    return ProfessionalSpecialtyLabels[specialty] || 'Desconhecido';
  }
}
