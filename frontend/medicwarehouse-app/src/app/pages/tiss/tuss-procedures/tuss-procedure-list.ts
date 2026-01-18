import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { TussProcedureService } from '../../../services/tuss-procedure.service';
import { TussProcedure } from '../../../models/tiss.model';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-tuss-procedure-list',
  imports: [CommonModule, RouterLink, FormsModule, Navbar],
  templateUrl: './tuss-procedure-list.html',
  styleUrl: './tuss-procedure-list.scss'
})
export class TussProcedureList implements OnInit {
  procedures = signal<TussProcedure[]>([]);
  filteredProcedures = signal<TussProcedure[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';
  private searchSubject = new Subject<string>();

  constructor(
    private procedureService: TussProcedureService,
    private router: Router
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      if (term.length >= 3) {
        this.searchProcedures(term);
      } else if (term.length === 0) {
        this.loadProcedures();
      }
    });
  }

  ngOnInit(): void {
    this.loadProcedures();
  }

  loadProcedures(): void {
    this.isLoading.set(true);
    this.procedureService.getAll().subscribe({
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

  searchProcedures(term: string): void {
    this.isLoading.set(true);
    this.procedureService.search(term).subscribe({
      next: (data) => {
        this.filteredProcedures.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao buscar procedimentos');
        this.isLoading.set(false);
        console.error('Error searching procedures:', error);
      }
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  deleteProcedure(id: string): void {
    if (confirm('Tem certeza que deseja excluir este procedimento?')) {
      this.procedureService.delete(id).subscribe({
        next: () => {
          this.loadProcedures();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir procedimento');
          console.error('Error deleting procedure:', error);
        }
      });
    }
  }
}
