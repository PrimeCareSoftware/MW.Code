import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HealthInsuranceOperatorService } from '../../../services/health-insurance-operator.service';
import { HealthInsuranceOperator } from '../../../models/tiss.model';

@Component({
  selector: 'app-health-insurance-operators-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './health-insurance-operators-list.html',
  styleUrl: './health-insurance-operators-list.scss'
})
export class HealthInsuranceOperatorsList implements OnInit {
  operators = signal<HealthInsuranceOperator[]>([]);
  filteredOperators = signal<HealthInsuranceOperator[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';

  constructor(
    private operatorService: HealthInsuranceOperatorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOperators();
  }

  loadOperators(): void {
    this.isLoading.set(true);
    this.operatorService.getAll().subscribe({
      next: (data) => {
        this.operators.set(data);
        this.filteredOperators.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar operadoras');
        this.isLoading.set(false);
        console.error('Error loading operators:', error);
      }
    });
  }

  onSearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredOperators.set(this.operators());
      return;
    }

    const filtered = this.operators().filter(op =>
      op.tradeName.toLowerCase().includes(term) ||
      op.legalName.toLowerCase().includes(term) ||
      op.ansCode.includes(term) ||
      op.cnpj.includes(term)
    );
    this.filteredOperators.set(filtered);
  }

  deleteOperator(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta operadora?')) {
      this.operatorService.delete(id).subscribe({
        next: () => {
          this.loadOperators();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir operadora');
          console.error('Error deleting operator:', error);
        }
      });
    }
  }

  viewDetails(id: string): void {
    this.router.navigate(['/tiss/operators', id]);
  }
}
