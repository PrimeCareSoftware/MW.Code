import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { AnamnesisService } from '../../../services/anamnesis.service';
import { AnamnesisTemplate, SPECIALTY_NAMES } from '../../../models/anamnesis.model';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-template-management',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './template-management.html',
  styleUrl: './template-management.scss'
})
export class TemplateManagementComponent implements OnInit {
  templates = signal<AnamnesisTemplate[]>([]);
  filteredTemplates = signal<AnamnesisTemplate[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';
  specialtyNames = SPECIALTY_NAMES;
  private searchSubject = new Subject<string>();

  constructor(
    private anamnesisService: AnamnesisService,
    private router: Router
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.filterTemplates(term);
    });
  }

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.isLoading.set(true);
    this.anamnesisService.getAllTemplates().subscribe({
      next: (data) => {
        this.templates.set(data);
        this.filteredTemplates.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar templates de anamnese');
        this.isLoading.set(false);
        console.error('Error loading templates:', error);
      }
    });
  }

  filterTemplates(term: string): void {
    if (!term) {
      this.filteredTemplates.set(this.templates());
      return;
    }

    const searchLower = term.toLowerCase();
    const filtered = this.templates().filter(template => 
      template.name.toLowerCase().includes(searchLower) ||
      template.description?.toLowerCase().includes(searchLower) ||
      this.specialtyNames[template.specialty].toLowerCase().includes(searchLower)
    );
    this.filteredTemplates.set(filtered);
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  deleteTemplate(id: string): void {
    if (confirm('Tem certeza que deseja excluir este template de anamnese?')) {
      this.anamnesisService.deleteTemplate(id).subscribe({
        next: () => {
          this.loadTemplates();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir template');
          console.error('Error deleting template:', error);
        }
      });
    }
  }

  navigateToNew(): void {
    this.router.navigate(['/anamnesis/templates/new']);
  }

  navigateToEdit(id: string): void {
    this.router.navigate(['/anamnesis/templates/edit', id]);
  }
}
