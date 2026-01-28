import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatMenuModule } from '@angular/material/menu';
import { DashboardService } from '../../services/dashboard.service';
import { CustomDashboard, DashboardWidget } from '../../models/system-admin.model';
import { DashboardWidgetComponent } from '../../components/dashboard-widget/dashboard-widget.component';

@Component({
  selector: 'app-dashboard-editor',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatMenuModule,
    DashboardWidgetComponent
  ],
  template: `
    <mat-toolbar color="primary">
      <mat-form-field appearance="outline" class="dashboard-name">
        <input matInput [(ngModel)]="dashboardName" placeholder="Nome do Dashboard">
      </mat-form-field>
      
      <span class="spacer"></span>
      
      <button mat-button (click)="addWidget()">
        <mat-icon>add</mat-icon>
        Adicionar Widget
      </button>
      
      <button mat-button (click)="toggleEditMode()">
        <mat-icon>{{ editMode() ? 'lock_open' : 'lock' }}</mat-icon>
        {{ editMode() ? 'Bloquear' : 'Editar' }}
      </button>
      
      <button mat-raised-button color="accent" (click)="saveDashboard()">
        <mat-icon>save</mat-icon>
        Salvar
      </button>
      
      <button mat-button (click)="cancel()">
        <mat-icon>close</mat-icon>
        Cancelar
      </button>
    </mat-toolbar>

    <div class="editor-container">
      <div class="widgets-grid">
        @for (widget of widgets(); track widget.id) {
          <div class="widget-wrapper" 
               [style.grid-column]="'span ' + widget.gridWidth"
               [style.grid-row]="'span ' + widget.gridHeight">
            <app-dashboard-widget [widget]="widget"></app-dashboard-widget>
            @if (editMode()) {
              <div class="widget-controls">
                <button mat-icon-button (click)="removeWidget(widget.id)">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
            }
          </div>
        }
      </div>

      @if (widgets().length === 0) {
        <div class="empty-state">
          <mat-icon>widgets</mat-icon>
          <h2>Nenhum widget adicionado</h2>
          <p>Adicione widgets para personalizar seu dashboard</p>
          <button mat-raised-button color="primary" (click)="addWidget()">
            <mat-icon>add</mat-icon>
            Adicionar Widget
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    mat-toolbar {
      gap: 16px;
    }

    .dashboard-name {
      width: 300px;
    }

    .spacer {
      flex: 1;
    }

    .editor-container {
      padding: 24px;
      min-height: calc(100vh - 64px);
      background: #f5f5f5;
    }

    .widgets-grid {
      display: grid;
      grid-template-columns: repeat(12, 1fr);
      gap: 16px;
      auto-rows: 200px;
    }

    .widget-wrapper {
      position: relative;
    }

    .widget-controls {
      position: absolute;
      top: 8px;
      right: 8px;
      background: rgba(255, 255, 255, 0.9);
      border-radius: 4px;
      padding: 4px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 80px 24px;
      text-align: center;
    }

    .empty-state mat-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #ccc;
      margin-bottom: 16px;
    }

    .empty-state h2 {
      margin: 0 0 8px 0;
      font-size: 24px;
      font-weight: 600;
    }

    .empty-state p {
      color: #666;
      margin: 0 0 24px 0;
    }
  `]
})
export class DashboardEditorComponent implements OnInit {
  dashboardId?: number;
  dashboardName = '';
  widgets = signal<DashboardWidget[]>([]);
  editMode = signal(true);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.dashboardId = parseInt(id, 10);
      this.loadDashboard();
    }
  }

  loadDashboard(): void {
    if (this.dashboardId) {
      this.dashboardService.getDashboard(this.dashboardId).subscribe({
        next: (dashboard) => {
          this.dashboardName = dashboard.name;
          this.widgets.set(dashboard.widgets);
        },
        error: (err) => console.error('Erro ao carregar dashboard:', err)
      });
    }
  }

  toggleEditMode(): void {
    this.editMode.set(!this.editMode());
  }

  addWidget(): void {
    console.log('Add widget functionality to be implemented');
  }

  removeWidget(widgetId: number): void {
    this.widgets.update(widgets => widgets.filter(w => w.id !== widgetId));
  }

  saveDashboard(): void {
    const dto = {
      name: this.dashboardName,
      description: '',
      layout: JSON.stringify({ widgets: this.widgets() })
    };

    if (this.dashboardId) {
      this.dashboardService.updateDashboard(this.dashboardId, dto).subscribe({
        next: () => this.router.navigate(['/custom-dashboards']),
        error: (err) => console.error('Erro ao atualizar dashboard:', err)
      });
    } else {
      this.dashboardService.createDashboard(dto).subscribe({
        next: () => this.router.navigate(['/custom-dashboards']),
        error: (err) => console.error('Erro ao criar dashboard:', err)
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/custom-dashboards']);
  }
}
