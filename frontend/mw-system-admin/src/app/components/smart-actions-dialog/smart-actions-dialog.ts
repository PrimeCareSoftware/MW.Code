import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SmartActionService } from '../../services/smart-action.service';
import {
  ImpersonateRequest,
  GrantCreditRequest,
  ApplyDiscountRequest,
  SuspendRequest,
  ExportDataRequest,
  MigratePlanRequest,
  SendCustomEmailRequest
} from '../../models/smart-action.model';

type ActionType = 'impersonate' | 'grantCredit' | 'applyDiscount' | 'suspend' | 'exportData' | 'migratePlan' | 'sendEmail';

@Component({
  selector: 'app-smart-actions-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './smart-actions-dialog.html',
  styleUrl: './smart-actions-dialog.scss'
})
export class SmartActionsDialog {
  @Input() clinicId!: string;
  @Input() clinicName?: string;
  @Output() success = new EventEmitter<{ action: string; message: string }>();
  @Output() error = new EventEmitter<{ action: string; error: string }>();
  @Output() close = new EventEmitter<void>();

  selectedAction = signal<ActionType>('impersonate');
  submitting = signal(false);
  actionError = signal<string | null>(null);

  // Form data for each action
  grantCreditForm = {
    days: 30,
    reason: ''
  };

  applyDiscountForm = {
    percentage: 10,
    months: 3
  };

  suspendForm = {
    reactivationDate: '',
    reason: ''
  };

  migratePlanForm = {
    newPlanId: '',
    proRata: true
  };

  sendEmailForm = {
    subject: '',
    body: ''
  };

  actions = [
    { id: 'impersonate' as ActionType, label: 'Impersonate', icon: '👤', description: 'Access clinic as admin' },
    { id: 'grantCredit' as ActionType, label: 'Grant Credit', icon: '💰', description: 'Add free days to subscription' },
    { id: 'applyDiscount' as ActionType, label: 'Apply Discount', icon: '🏷️', description: 'Apply temporary discount' },
    { id: 'suspend' as ActionType, label: 'Suspend', icon: '⏸️', description: 'Temporarily suspend clinic' },
    { id: 'exportData' as ActionType, label: 'Export Data', icon: '📥', description: 'Export clinic data' },
    { id: 'migratePlan' as ActionType, label: 'Migrate Plan', icon: '🔄', description: 'Change subscription plan' },
    { id: 'sendEmail' as ActionType, label: 'Send Email', icon: '📧', description: 'Send custom email' }
  ];

  constructor(private smartActionService: SmartActionService) {}

  selectAction(actionId: ActionType): void {
    this.selectedAction.set(actionId);
    this.actionError.set(null);
  }

  onClose(): void {
    this.close.emit();
  }

  executeImpersonate(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: ImpersonateRequest = { clinicId: this.clinicId };

    this.smartActionService.impersonate(request).subscribe({
      next: (result) => {
        this.submitting.set(false);
        this.success.emit({
          action: 'impersonate',
          message: `Impersonation token generated. Valid for ${result.expiresIn} seconds.`
        });
        
        // Open in new tab with the impersonation token
        const baseUrl = window.location.origin.replace('system-admin', 'app');
        window.open(`${baseUrl}?impersonate=${result.token}`, '_blank');
        
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error executing impersonation';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'impersonate', error: errorMsg });
      }
    });
  }

  executeGrantCredit(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: GrantCreditRequest = {
      clinicId: this.clinicId,
      days: this.grantCreditForm.days,
      reason: this.grantCreditForm.reason
    };

    this.smartActionService.grantCredit(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.success.emit({ action: 'grantCredit', message: response.message });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error granting credit';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'grantCredit', error: errorMsg });
      }
    });
  }

  executeApplyDiscount(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: ApplyDiscountRequest = {
      clinicId: this.clinicId,
      percentage: this.applyDiscountForm.percentage,
      months: this.applyDiscountForm.months
    };

    this.smartActionService.applyDiscount(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.success.emit({ action: 'applyDiscount', message: response.message });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error applying discount';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'applyDiscount', error: errorMsg });
      }
    });
  }

  executeSuspend(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: SuspendRequest = {
      clinicId: this.clinicId,
      reactivationDate: this.suspendForm.reactivationDate ? new Date(this.suspendForm.reactivationDate) : undefined,
      reason: this.suspendForm.reason
    };

    this.smartActionService.suspend(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.success.emit({ action: 'suspend', message: response.message });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error suspending clinic';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'suspend', error: errorMsg });
      }
    });
  }

  executeExportData(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: ExportDataRequest = { clinicId: this.clinicId };

    this.smartActionService.exportData(request).subscribe({
      next: (blob) => {
        this.submitting.set(false);
        
        // Download the file
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `clinic-${this.clinicId}-data-${Date.now()}.zip`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);

        this.success.emit({ action: 'exportData', message: 'Data exported successfully' });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error exporting data';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'exportData', error: errorMsg });
      }
    });
  }

  executeMigratePlan(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: MigratePlanRequest = {
      clinicId: this.clinicId,
      newPlanId: this.migratePlanForm.newPlanId,
      proRata: this.migratePlanForm.proRata
    };

    this.smartActionService.migratePlan(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.success.emit({ action: 'migratePlan', message: response.message });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error migrating plan';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'migratePlan', error: errorMsg });
      }
    });
  }

  executeSendEmail(): void {
    this.submitting.set(true);
    this.actionError.set(null);

    const request: SendCustomEmailRequest = {
      clinicId: this.clinicId,
      subject: this.sendEmailForm.subject,
      body: this.sendEmailForm.body
    };

    this.smartActionService.sendEmail(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.success.emit({ action: 'sendEmail', message: response.message });
        this.onClose();
      },
      error: (err) => {
        this.submitting.set(false);
        const errorMsg = err.error?.message || 'Error sending email';
        this.actionError.set(errorMsg);
        this.error.emit({ action: 'sendEmail', error: errorMsg });
      }
    });
  }

  onSubmit(): void {
    const action = this.selectedAction();
    
    switch (action) {
      case 'impersonate':
        this.executeImpersonate();
        break;
      case 'grantCredit':
        this.executeGrantCredit();
        break;
      case 'applyDiscount':
        this.executeApplyDiscount();
        break;
      case 'suspend':
        this.executeSuspend();
        break;
      case 'exportData':
        this.executeExportData();
        break;
      case 'migratePlan':
        this.executeMigratePlan();
        break;
      case 'sendEmail':
        this.executeSendEmail();
        break;
    }
  }
}
