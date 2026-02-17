import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DashboardService } from '../../services/dashboard.service';
import { CustomDashboard } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-custom-dashboards',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    Navbar
  ],
  templateUrl: './custom-dashboards.component.html',
  styleUrls: ['./custom-dashboards.component.scss']
})
export class CustomDashboardsComponent implements OnInit {
  dashboards = signal<CustomDashboard[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private dashboardService: DashboardService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadDashboards();
  }

  loadDashboards(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboardService.getAllDashboards().subscribe({
      next: (data) => {
        this.dashboards.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar dashboards');
        this.loading.set(false);
      }
    });
  }

  createDashboard(): void {
    this.router.navigate(['/custom-dashboards', 'new', 'edit']);
  }

  openDashboard(id: number): void {
    this.router.navigate(['/custom-dashboards', id]);
  }

  editDashboard(id: number): void {
    this.router.navigate(['/custom-dashboards', id, 'edit']);
  }

  duplicateDashboard(id: number): void {
    const dashboard = this.dashboards().find(d => d.id === id);
    if (dashboard) {
      this.dashboardService.createDashboard({
        name: `${dashboard.name} (Cópia)`,
        description: dashboard.description,
        layout: dashboard.layout
      }).subscribe({
        next: () => this.loadDashboards(),
        error: (err) => console.error('Erro ao duplicar dashboard:', err)
      });
    }
  }

  exportDashboard(id: number): void {
    this.dashboardService.exportDashboard(id, 'pdf').subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `dashboard-${id}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Erro ao exportar dashboard:', err)
    });
  }

  deleteDashboard(id: number): void {
    if (confirm('Tem certeza que deseja excluir este dashboard?')) {
      this.dashboardService.deleteDashboard(id).subscribe({
        next: () => this.loadDashboards(),
        error: (err) => console.error('Erro ao excluir dashboard:', err)
      });
    }
  }
}
