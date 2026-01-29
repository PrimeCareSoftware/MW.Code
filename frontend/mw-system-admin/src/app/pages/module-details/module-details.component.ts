import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ModuleConfigService } from '../../services/module-config.service';
import { ClinicModule } from '../../models/module-config.model';

@Component({
  selector: 'app-module-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatDialogModule
  ],
  templateUrl: './module-details.component.html',
  styleUrls: ['./module-details.component.scss']
})
export class ModuleDetailsComponent implements OnInit {
  moduleName: string = '';
  loading = false;
  clinics: ClinicModule[] = [];
  moduleStats: any = null;
  displayedColumns: string[] = ['clinicName', 'isEnabled', 'updatedAt', 'actions'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private moduleService: ModuleConfigService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.moduleName = this.route.snapshot.paramMap.get('moduleName') || '';
    if (this.moduleName) {
      this.loadModuleDetails();
    }
  }

  loadModuleDetails(): void {
    this.loading = true;

    Promise.all([
      this.moduleService.getClinicsWithModule(this.moduleName).toPromise(),
      this.moduleService.getModuleStats(this.moduleName).toPromise()
    ]).then(([clinics, stats]) => {
      this.clinics = clinics || [];
      this.moduleStats = stats || {};
      this.loading = false;
    }).catch(error => {
      console.error('Erro ao carregar detalhes:', error);
      this.snackBar.open('Erro ao carregar detalhes do módulo', 'Fechar', { duration: 3000 });
      this.loading = false;
    });
  }

  enableGlobally(): void {
    if (confirm(`Tem certeza que deseja habilitar o módulo ${this.moduleName} globalmente?`)) {
      this.moduleService.enableModuleGlobally(this.moduleName).subscribe({
        next: () => {
          this.snackBar.open('Módulo habilitado globalmente', 'Fechar', { duration: 3000 });
          this.loadModuleDetails();
        },
        error: (error) => {
          console.error('Erro:', error);
          this.snackBar.open('Erro ao habilitar módulo', 'Fechar', { duration: 3000 });
        }
      });
    }
  }

  disableGlobally(): void {
    if (confirm(`Tem certeza que deseja desabilitar o módulo ${this.moduleName} globalmente?`)) {
      this.moduleService.disableModuleGlobally(this.moduleName).subscribe({
        next: () => {
          this.snackBar.open('Módulo desabilitado globalmente', 'Fechar', { duration: 3000 });
          this.loadModuleDetails();
        },
        error: (error) => {
          console.error('Erro:', error);
          this.snackBar.open('Erro ao desabilitar módulo', 'Fechar', { duration: 3000 });
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/modules']);
  }

  viewClinic(clinicId: string): void {
    this.router.navigate(['/clinics', clinicId]);
  }
}
