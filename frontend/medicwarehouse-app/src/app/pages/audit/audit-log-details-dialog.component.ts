import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { AuditLog, AuditService } from '../../services/audit.service';

@Component({
  selector: 'app-audit-log-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatChipsModule,
    MatDividerModule
  ],
  templateUrl: './audit-log-details-dialog.component.html',
  styleUrls: ['./audit-log-details-dialog.component.scss']
})
export class AuditLogDetailsDialogComponent {
  oldValuesJson: any;
  newValuesJson: any;
  hasChanges: boolean = false;
  parsedChangedFields: string[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public log: AuditLog,
    private dialogRef: MatDialogRef<AuditLogDetailsDialogComponent>,
    public auditService: AuditService
  ) {
    this.initializeData();
  }

  private initializeData(): void {
    // Parse JSON values
    try {
      if (this.log.oldValues) {
        this.oldValuesJson = JSON.parse(this.log.oldValues);
      }
    } catch (e) {
      console.error('Error parsing old values:', e);
      this.oldValuesJson = this.log.oldValues;
    }

    try {
      if (this.log.newValues) {
        this.newValuesJson = JSON.parse(this.log.newValues);
      }
    } catch (e) {
      console.error('Error parsing new values:', e);
      this.newValuesJson = this.log.newValues;
    }

    // Check if there are changes to display
    this.hasChanges = !!(this.log.oldValues || this.log.newValues || (this.log.changedFields && this.log.changedFields.length > 0));

    // Parse changed fields if available
    if (this.log.changedFields) {
      this.parsedChangedFields = this.log.changedFields;
    }
  }

  getActionText(action: string): string {
    return this.auditService.getActionText(action);
  }

  getActionColor(action: string): string {
    return this.auditService.getActionColor(action);
  }

  getResultIcon(result: string): string {
    return this.auditService.getResultIcon(result);
  }

  getResultColor(result: string): string {
    return this.auditService.getResultColor(result);
  }

  getSeverityColor(severity: string): string {
    return this.auditService.getSeverityColor(severity);
  }

  getDataCategoryLabel(category: string): string {
    const labels: { [key: string]: string } = {
      'PUBLIC': 'Público',
      'PERSONAL': 'Pessoal',
      'SENSITIVE': 'Sensível',
      'CONFIDENTIAL': 'Confidencial'
    };
    return labels[category] || category;
  }

  getDataCategoryColor(category: string): string {
    const colors: { [key: string]: string } = {
      'PUBLIC': '#4caf50',
      'PERSONAL': '#2196f3',
      'SENSITIVE': '#ff9800',
      'CONFIDENTIAL': '#f44336'
    };
    return colors[category] || '#9e9e9e';
  }

  getPurposeLabel(purpose: string): string {
    const labels: { [key: string]: string } = {
      'HEALTHCARE': 'Serviços de Saúde',
      'BILLING': 'Faturamento',
      'LEGAL_OBLIGATION': 'Obrigação Legal',
      'LEGITIMATE_INTEREST': 'Interesse Legítimo',
      'CONSENT': 'Consentimento'
    };
    return labels[purpose] || purpose;
  }

  close(): void {
    this.dialogRef.close();
  }

  isObject(value: any): boolean {
    return value !== null && typeof value === 'object';
  }

  formatJson(value: any): string {
    if (!value) return '';
    if (typeof value === 'string') return value;
    return JSON.stringify(value, null, 2);
  }

  getFieldValue(obj: any, field: string): any {
    if (!obj) return null;
    return obj[field];
  }

  hasFieldChanged(field: string): boolean {
    if (!this.oldValuesJson || !this.newValuesJson) return false;
    const oldVal = this.getFieldValue(this.oldValuesJson, field);
    const newVal = this.getFieldValue(this.newValuesJson, field);
    return JSON.stringify(oldVal) !== JSON.stringify(newVal);
  }
}
