import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { Supplier } from '../../../models/financial.model';

@Component({
  selector: 'app-suppliers-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './suppliers-list.component.html',
  styleUrl: './suppliers-list.component.scss'
})
export class SuppliersListComponent implements OnInit {
  suppliers = signal<Supplier[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private financialService: FinancialService) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.isLoading.set(true);
    this.financialService.getAllSuppliers().subscribe({
      next: (data) => {
        this.suppliers.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar fornecedores');
        this.isLoading.set(false);
        console.error('Error:', error);
      }
    });
  }

  deleteSupplier(id: string): void {
    if (confirm('Tem certeza que deseja excluir este fornecedor?')) {
      this.financialService.deleteSupplier(id).subscribe({
        next: () => this.loadSuppliers(),
        error: (error) => {
          this.errorMessage.set('Erro ao excluir fornecedor');
          console.error('Error:', error);
        }
      });
    }
  }

  toggleActive(id: string, isActive: boolean): void {
    const action = isActive ? 
      this.financialService.deactivateSupplier(id) : 
      this.financialService.activateSupplier(id);
    
    action.subscribe({
      next: () => this.loadSuppliers(),
      error: (error) => console.error('Error:', error)
    });
  }
}
