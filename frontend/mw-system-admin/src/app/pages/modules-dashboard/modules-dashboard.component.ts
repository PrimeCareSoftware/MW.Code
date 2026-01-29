import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { forkJoin } from 'rxjs';
import { ModuleConfigService } from '../../services/module-config.service';
import { ModuleUsage, ModuleAdoption } from '../../models/module-config.model';

@Component({
  selector: 'app-modules-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatGridListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './modules-dashboard.component.html',
  styleUrls: ['./modules-dashboard.component.scss']
})
export class ModulesDashboardComponent implements OnInit {
  loading = false;
  moduleUsage: ModuleUsage[] = [];
  moduleAdoption: ModuleAdoption[] = [];
  totalModules = 0;
  averageAdoption = 0;

  constructor(private moduleService: ModuleConfigService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    // Carregar dados em paralelo usando forkJoin
    forkJoin({
      usage: this.moduleService.getGlobalModuleUsage(),
      adoption: this.moduleService.getModuleAdoption()
    }).subscribe({
      next: ({ usage, adoption }) => {
        this.moduleUsage = usage || [];
        this.moduleAdoption = adoption || [];
        this.totalModules = this.moduleUsage.length;
        this.averageAdoption = this.calculateAverageAdoption();
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar dados:', error);
        this.loading = false;
      }
    });
  }

  calculateAverageAdoption(): number {
    if (this.moduleUsage.length === 0) return 0;
    const sum = this.moduleUsage.reduce((acc, m) => acc + m.adoptionRate, 0);
    return sum / this.moduleUsage.length;
  }

  getCategoryColor(category: string): string {
    const colors: { [key: string]: string } = {
      'Core': '#4CAF50',
      'Advanced': '#2196F3',
      'Premium': '#FF9800',
      'Analytics': '#9C27B0'
    };
    return colors[category] || '#757575';
  }

  getAdoptionClass(rate: number): string {
    if (rate >= 75) return 'high-adoption';
    if (rate >= 50) return 'medium-adoption';
    return 'low-adoption';
  }

  getMostUsedModule(): string {
    if (this.moduleUsage.length === 0) return '-';
    const sorted = [...this.moduleUsage].sort((a, b) => b.adoptionRate - a.adoptionRate);
    return sorted[0]?.displayName || '-';
  }

  getLeastUsedModule(): string {
    if (this.moduleUsage.length === 0) return '-';
    const sorted = [...this.moduleUsage].sort((a, b) => a.adoptionRate - b.adoptionRate);
    return sorted[0]?.displayName || '-';
  }
}
