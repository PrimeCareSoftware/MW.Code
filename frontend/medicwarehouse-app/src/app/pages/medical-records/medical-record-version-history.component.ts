import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MedicalRecordService } from '../../services/medical-record';
import { MedicalRecordVersion } from '../../models/medical-record.model';

@Component({
  selector: 'app-medical-record-version-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './medical-record-version-history.component.html',
  styleUrls: ['./medical-record-version-history.component.scss']
})
export class MedicalRecordVersionHistoryComponent implements OnInit {
  @Input() medicalRecordId!: string;
  
  versions: MedicalRecordVersion[] = [];
  loading = false;
  error: string | null = null;

  constructor(private medicalRecordService: MedicalRecordService) {}

  ngOnInit(): void {
    if (!this.medicalRecordId) {
      this.error = 'ID do prontuário não fornecido';
      return;
    }
    this.loadVersionHistory();
  }

  loadVersionHistory(): void {
    this.loading = true;
    this.error = null;

    this.medicalRecordService.getVersionHistory(this.medicalRecordId).subscribe({
      next: (versions) => {
        this.versions = versions;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar histórico de versões';
        this.loading = false;
        console.error('Error loading version history:', err);
      }
    });
  }

  getChangeTypeIcon(changeType: string): string {
    const iconMap: Record<string, string> = {
      'Created': 'history',
      'Updated': 'edit',
      'Closed': 'lock',
      'Reopened': 'lock_open'
    };
    return iconMap[changeType] || 'info';
  }

  getChangeTypeColor(changeType: string): string {
    const colorMap: Record<string, string> = {
      'Created': 'primary',
      'Updated': 'accent',
      'Closed': 'warn',
      'Reopened': 'warn'
    };
    return colorMap[changeType] || '';
  }
}
