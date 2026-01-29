import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatSlideToggleModule, MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ModuleConfigService } from '../../../services/module-config.service';
import { ModuleConfig } from '../../../models/module-config.model';
import { ModuleConfigDialogComponent } from './module-config-dialog/module-config-dialog.component';

/**
 * Component for managing clinic modules
 * Allows clinic administrators to enable/disable and configure modules
 */
@Component({
  selector: 'app-clinic-modules',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './clinic-modules.component.html',
  styleUrls: ['./clinic-modules.component.scss']
})
export class ClinicModulesComponent implements OnInit {
  modules: ModuleConfig[] = [];
  loading = false;

  constructor(
    private moduleService: ModuleConfigService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadModules();
  }

  /**
   * Load all modules for the clinic
   */
  loadModules(): void {
    this.loading = true;
    this.moduleService.getModules().subscribe({
      next: (modules) => {
        this.modules = modules;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar módulos:', error);
        this.snackBar.open('Erro ao carregar módulos', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  /**
   * Handle module toggle (enable/disable)
   */
  toggleModule(module: ModuleConfig, event: MatSlideToggleChange): void {
    const isEnabled = event.checked;

    // Validate before enabling
    if (isEnabled) {
      this.moduleService.validateModule(module.moduleName).subscribe({
        next: (validation) => {
          if (validation.isValid) {
            this.enableModule(module);
          } else {
            // Revert toggle
            event.source.checked = false;
            this.snackBar.open(validation.errorMessage || 'Não é possível habilitar este módulo', 
                              'Fechar', { duration: 5000 });
          }
        },
        error: (error) => {
          event.source.checked = false;
          this.snackBar.open('Erro ao validar módulo', 'Fechar', { duration: 3000 });
        }
      });
    } else {
      this.disableModule(module);
    }
  }

  /**
   * Enable a module
   */
  enableModule(module: ModuleConfig): void {
    this.moduleService.enableModule(module.moduleName).subscribe({
      next: () => {
        module.isEnabled = true;
        this.snackBar.open(`${module.displayName} habilitado com sucesso`, 'Fechar', 
                          { duration: 3000 });
      },
      error: (error) => {
        console.error('Erro ao habilitar módulo:', error);
        module.isEnabled = false; // Revert state on error
        this.snackBar.open('Erro ao habilitar módulo', 'Fechar', { duration: 3000 });
        // Force refresh to sync UI with backend state
        this.loadModules();
      }
    });
  }

  /**
   * Disable a module
   */
  disableModule(module: ModuleConfig): void {
    this.moduleService.disableModule(module.moduleName).subscribe({
      next: () => {
        module.isEnabled = false;
        this.snackBar.open(`${module.displayName} desabilitado`, 'Fechar', 
                          { duration: 3000 });
      },
      error: (error) => {
        console.error('Erro ao desabilitar módulo:', error);
        module.isEnabled = true; // Revert state on error
        this.snackBar.open('Erro ao desabilitar módulo', 'Fechar', { duration: 3000 });
        // Force refresh to sync UI with backend state
        this.loadModules();
      }
    });
  }

  /**
   * Open configuration dialog for a module
   */
  openConfigDialog(module: ModuleConfig): void {
    const dialogRef = this.dialog.open(ModuleConfigDialogComponent, {
      width: '600px',
      data: { module }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadModules(); // Reload modules
      }
    });
  }

  /**
   * Get modules for a specific category
   */
  getCategoryModules(category: string): ModuleConfig[] {
    return this.modules.filter(m => m.category === category);
  }

  /**
   * Get all unique categories
   */
  getCategories(): string[] {
    return [...new Set(this.modules.map(m => m.category))];
  }

  /**
   * Get icon for a category
   */
  getCategoryIcon(category: string): string {
    const icons: { [key: string]: string } = {
      'Core': 'star',
      'Advanced': 'build',
      'Premium': 'workspace_premium',
      'Analytics': 'analytics'
    };
    return icons[category] || 'extension';
  }

  /**
   * Get color for a category
   */
  getCategoryColor(category: string): string {
    const colors: { [key: string]: string } = {
      'Core': '#4CAF50',
      'Advanced': '#2196F3',
      'Premium': '#FF9800',
      'Analytics': '#9C27B0'
    };
    return colors[category] || '#757575';
  }
}
