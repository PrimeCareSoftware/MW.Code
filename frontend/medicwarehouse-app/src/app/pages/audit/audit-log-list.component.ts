import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { 
  AuditService, 
  AuditLog, 
  AuditFilter, 
  AuditAction, 
  OperationResult, 
  AuditSeverity,
  PagedAuditLogs 
} from '../../services/audit.service';
import { AuditLogDetailsDialogComponent } from './audit-log-details-dialog.component';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule,
    Navbar
  ],
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss']
})
export class AuditLogListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  auditLogs: AuditLog[] = [];
  displayedColumns: string[] = [
    'timestamp',
    'userName',
    'action',
    'entityType',
    'result',
    'ipAddress',
    'details'
  ];
  
  filterForm: FormGroup;
  loading = false;
  error: string | null = null;
  
  totalCount = 0;
  pageSize = 50;
  pageNumber = 1;

  // Enum values for dropdowns
  actions = Object.values(AuditAction);
  results = Object.values(OperationResult);
  severities = Object.values(AuditSeverity);

  constructor(
    private auditService: AuditService,
    private fb: FormBuilder,
    private dialog: MatDialog
  ) {
    this.filterForm = this.fb.group({
      startDate: [null],
      endDate: [null],
      action: [''],
      result: [''],
      severity: [''],
      entityType: ['']
    });
  }

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.loading = true;
    this.error = null;

    const filter: AuditFilter = {
      startDate: this.formatDate(this.filterForm.value.startDate),
      endDate: this.formatDate(this.filterForm.value.endDate),
      action: this.filterForm.value.action || undefined,
      result: this.filterForm.value.result || undefined,
      severity: this.filterForm.value.severity || undefined,
      entityType: this.filterForm.value.entityType || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    this.auditService.queryLogs(filter).subscribe({
      next: (response: PagedAuditLogs) => {
        this.auditLogs = response.data;
        this.totalCount = response.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar logs de auditoria';
        this.loading = false;
        console.error('Error loading audit logs:', err);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadAuditLogs();
  }

  clearFilters(): void {
    this.filterForm.reset({
      startDate: null,
      endDate: null,
      action: '',
      result: '',
      severity: '',
      entityType: ''
    });
    this.pageNumber = 1;
    this.loadAuditLogs();
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.loadAuditLogs();
  }

  viewDetails(log: AuditLog): void {
    this.dialog.open(AuditLogDetailsDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      data: log
    });
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

  private formatDate(date: Date | null): string | undefined {
    if (!date) return undefined;
    return date.toISOString();
  }
}
