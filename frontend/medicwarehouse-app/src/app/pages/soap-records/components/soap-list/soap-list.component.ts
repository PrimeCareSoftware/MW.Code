import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SoapRecordService } from '../../services/soap-record.service';
import { SoapRecord } from '../../models/soap-record.model';

@Component({
  selector: 'app-soap-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatChipsModule,
    MatSnackBarModule
  ],
  template: `
    <div class="soap-list-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Prontuários SOAP</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          @if (loading) {
            <div class="loading">Carregando prontuários...</div>
          }

          @if (!loading && soapRecords.length === 0) {
            <div class="no-records">
              <mat-icon>description</mat-icon>
              <p>Nenhum prontuário SOAP encontrado</p>
            </div>
          }

          @if (!loading && soapRecords.length > 0) {
            <div class="records-table">
              <table mat-table [dataSource]="soapRecords" class="mat-elevation-z0">
                
                <!-- Date Column -->
                <ng-container matColumnDef="date">
                  <th mat-header-cell *matHeaderCellDef>Data</th>
                  <td mat-cell *matCellDef="let record">
                    {{ record.recordDate | date:'dd/MM/yyyy HH:mm' }}
                  </td>
                </ng-container>

                <!-- Status Column -->
                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let record">
                    @if (record.isComplete) {
                      <mat-chip class="status-chip complete">
                        <mat-icon>lock</mat-icon>
                        Concluído
                      </mat-chip>
                    } @else {
                      <mat-chip class="status-chip incomplete">
                        <mat-icon>edit</mat-icon>
                        Em Andamento
                      </mat-chip>
                    }
                  </td>
                </ng-container>

                <!-- Sections Column -->
                <ng-container matColumnDef="sections">
                  <th mat-header-cell *matHeaderCellDef>Seções Preenchidas</th>
                  <td mat-cell *matCellDef="let record">
                    <div class="sections">
                      @if (record.subjective) {
                        <mat-icon class="section-icon complete">check_circle</mat-icon>
                      } @else {
                        <mat-icon class="section-icon">radio_button_unchecked</mat-icon>
                      }
                      S
                      
                      @if (record.objective) {
                        <mat-icon class="section-icon complete">check_circle</mat-icon>
                      } @else {
                        <mat-icon class="section-icon">radio_button_unchecked</mat-icon>
                      }
                      O
                      
                      @if (record.assessment) {
                        <mat-icon class="section-icon complete">check_circle</mat-icon>
                      } @else {
                        <mat-icon class="section-icon">radio_button_unchecked</mat-icon>
                      }
                      A
                      
                      @if (record.plan) {
                        <mat-icon class="section-icon complete">check_circle</mat-icon>
                      } @else {
                        <mat-icon class="section-icon">radio_button_unchecked</mat-icon>
                      }
                      P
                    </div>
                  </td>
                </ng-container>

                <!-- Actions Column -->
                <ng-container matColumnDef="actions">
                  <th mat-header-cell *matHeaderCellDef>Ações</th>
                  <td mat-cell *matCellDef="let record">
                    <button mat-icon-button 
                            (click)="viewRecord(record.id)"
                            matTooltip="Visualizar">
                      <mat-icon>visibility</mat-icon>
                    </button>
                    @if (!record.isComplete) {
                      <button mat-icon-button 
                              (click)="editRecord(record.id)"
                              matTooltip="Editar">
                        <mat-icon>edit</mat-icon>
                      </button>
                    }
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
              </table>
            </div>
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .soap-list-container {
      padding: 20px;
    }

    .loading,
    .no-records {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .no-records mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #ccc;
    }

    .records-table {
      overflow-x: auto;
    }

    table {
      width: 100%;
    }

    .status-chip {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .status-chip.complete {
      background-color: #4caf50;
      color: white;
    }

    .status-chip.incomplete {
      background-color: #ff9800;
      color: white;
    }

    .sections {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .section-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
    }

    .section-icon.complete {
      color: #4caf50;
    }
  `]
})
export class SoapListComponent implements OnInit {
  soapRecords: SoapRecord[] = [];
  loading = true;
  displayedColumns = ['date', 'status', 'sections', 'actions'];

  constructor(
    private soapService: SoapRecordService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    // For now, we'll load all records. In a real app, you'd filter by patient
    // this.loadRecords();
  }

  loadRecords(): void {
    // This would be called with a patient ID in a real scenario
    // For now, just set loading to false
    this.loading = false;
  }

  viewRecord(id: string): void {
    this.router.navigate(['/soap-records', id]);
  }

  editRecord(id: string): void {
    this.router.navigate(['/soap-records', id, 'edit']);
  }
}
