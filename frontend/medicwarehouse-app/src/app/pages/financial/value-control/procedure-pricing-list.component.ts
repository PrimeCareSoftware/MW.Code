import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { ProcedurePricingConfiguration } from '../../../models/financial.model';

@Component({
  selector: 'app-procedure-pricing-list',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './procedure-pricing-list.component.html',
  styleUrl: './procedure-pricing-list.component.scss'
})
export class ProcedurePricingListComponent implements OnInit {
  configurations = signal<ProcedurePricingConfiguration[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  clinicId = signal<string>('');

  constructor(
    private financialService: FinancialService
  ) {}

  ngOnInit(): void {
    const clinicId = localStorage.getItem('selectedClinicId');
    if (clinicId) {
      this.clinicId.set(clinicId);
      this.loadConfigurations();
    }
  }

  loadConfigurations(): void {
    const clinicId = this.clinicId();
    if (!clinicId) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.financialService.getProcedurePricingConfigurationsByClinic(clinicId).subscribe({
      next: (configs) => {
        this.configurations.set(configs);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar configurações: ' + error.message);
        this.isLoading.set(false);
      }
    });
  }

  deleteConfiguration(id: string): void {
    if (!confirm('Tem certeza que deseja excluir esta configuração?')) {
      return;
    }

    this.financialService.deleteProcedurePricingConfiguration(id).subscribe({
      next: () => {
        this.loadConfigurations();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao excluir configuração: ' + error.message);
      }
    });
  }

  getPolicyLabel(policy?: number): string {
    switch (policy) {
      case 1:
        return 'Cobrar Consulta';
      case 2:
        return 'Desconto na Consulta';
      case 3:
        return 'Não Cobrar Consulta';
      default:
        return 'Padrão da Clínica';
    }
  }
}
