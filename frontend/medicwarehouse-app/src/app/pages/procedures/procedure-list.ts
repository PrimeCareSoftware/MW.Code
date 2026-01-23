import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../shared/navbar/navbar';
import { ProcedureService } from '../../services/procedure';
import { Procedure, ProcedureCategoryLabels } from '../../models/procedure.model';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-procedure-list',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './procedure-list.html',
  styleUrl: './procedure-list.scss'
})
export class ProcedureList implements OnInit {
  procedures = signal<Procedure[]>([]);
  filteredProcedures = signal<Procedure[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';
  procedureCategoryLabels = ProcedureCategoryLabels;
  private searchSubject = new Subject<string>();

  constructor(
    private procedureService: ProcedureService,
    private router: Router
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.filterProcedures(term);
    });
  }

  ngOnInit(): void {
    this.loadProcedures();
  }

  loadProcedures(): void {
    this.isLoading.set(true);
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

  filterProcedures(term: string): void {
    if (!term || term.length === 0) {
      this.filteredProcedures.set(this.procedures());
      return;
    }

    const searchLower = term.toLowerCase();
    const filtered = this.procedures().filter(proc => 
      proc.name.toLowerCase().includes(searchLower) ||
      proc.code.toLowerCase().includes(searchLower) ||
      proc.description.toLowerCase().includes(searchLower)
    );
    this.filteredProcedures.set(filtered);
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  deleteProcedure(id: string): void {
    if (confirm('Tem certeza que deseja desativar este procedimento?')) {
      this.procedureService.delete(id).subscribe({
        next: () => {
          this.loadProcedures();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao desativar procedimento');
          console.error('Error deleting procedure:', error);
        }
      });
    }
  }

  navigateToNew(): void {
    this.router.navigate(['/procedures/new']);
  }

  navigateToEdit(id: string): void {
    this.router.navigate(['/procedures/edit', id]);
  }
}
