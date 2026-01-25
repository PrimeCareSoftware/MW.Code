import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../shared/navbar/navbar';
import { ProcedureService } from '../../services/procedure';
import { Procedure, ProcedureCategoryLabels } from '../../models/procedure.model';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-owner-procedure-management',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './owner-procedure-management.html',
  styleUrl: './owner-procedure-management.scss'
})
export class OwnerProcedureManagement implements OnInit {
  procedures = signal<Procedure[]>([]);
  filteredProcedures = signal<Procedure[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';
  selectedCategory = '';
  procedureCategoryLabels = ProcedureCategoryLabels;
  private searchSubject = new Subject<string>();

  constructor(
    private procedureService: ProcedureService,
    private router: Router
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.filterProcedures();
    });
  }

  ngOnInit(): void {
    this.loadProcedures();
  }

  loadProcedures(): void {
    this.isLoading.set(true);
    // The backend will automatically return procedures from all owned clinics for ClinicOwner role
    this.procedureService.getAll(false).subscribe({
      next: (data) => {
        this.procedures.set(data);
        this.filteredProcedures.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar procedimentos');
        this.isLoading.set(false);
        console.error('Error loading procedures:', error);
      }
    });
  }

  filterProcedures(): void {
    let filtered = this.procedures();

    // Filter by search term
    if (this.searchTerm) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(proc => 
        proc.name.toLowerCase().includes(searchLower) ||
        proc.code.toLowerCase().includes(searchLower) ||
        proc.description.toLowerCase().includes(searchLower)
      );
    }

    // Filter by category
    if (this.selectedCategory) {
      filtered = filtered.filter(proc => proc.category === this.selectedCategory);
    }

    this.filteredProcedures.set(filtered);
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onCategoryChange(): void {
    this.filterProcedures();
  }

  getCategoryOptions(): string[] {
    return Object.keys(ProcedureCategoryLabels);
  }

  navigateToView(id: string): void {
    this.router.navigate(['/procedures/edit', id]);
  }

  getUniqueClinicCount(): number {
    const clinicIds = new Set(this.filteredProcedures().map(p => p.tenantId));
    return clinicIds.size;
  }
}
