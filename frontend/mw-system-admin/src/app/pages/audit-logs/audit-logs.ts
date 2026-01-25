import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditService, AuditLog, AuditFilter } from '../../services/audit.service';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './audit-logs.html',
  styleUrl: './audit-logs.scss'
})
export class AuditLogs implements OnInit {
  logs = signal<AuditLog[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  error = signal<string | null>(null);
  selectedLog = signal<AuditLog | null>(null);
  
  // Filter state
  filter: AuditFilter = {
    pageNumber: 1,
    pageSize: 50
  };
  
  // Date filter helpers
  startDate = '';
  endDate = '';
  
  // Available filter options
  availableActions: string[] = [];
  availableResults: string[] = [];
  availableSeverities: string[] = [];
  
  // Pagination
  currentPage = signal(1);
  totalPages = signal(1);
  
  // Search and filter expand state
  filtersExpanded = signal(true);
  
  constructor(public auditService: AuditService) {}

  ngOnInit(): void {
    // Load available filter options
    this.availableActions = this.auditService.getAvailableActions();
    this.availableResults = this.auditService.getAvailableResults();
    this.availableSeverities = this.auditService.getAvailableSeverities();
    
    // Set default date range (last 7 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 7);
    
    this.startDate = startDate.toISOString().split('T')[0];
    this.endDate = endDate.toISOString().split('T')[0];
    
    this.filter.startDate = startDate.toISOString();
    this.filter.endDate = endDate.toISOString();
    
    // Load initial data
    this.loadLogs();
  }

  loadLogs(): void {
    this.loading.set(true);
    this.error.set(null);
    
    // Convert date inputs to ISO format
    if (this.startDate) {
      const start = new Date(this.startDate);
      start.setHours(0, 0, 0, 0);
      this.filter.startDate = start.toISOString();
    }
    
    if (this.endDate) {
      const end = new Date(this.endDate);
      end.setHours(23, 59, 59, 999);
      this.filter.endDate = end.toISOString();
    }
    
    this.auditService.queryAuditLogs(this.filter).subscribe({
      next: (response) => {
        this.logs.set(response.data);
        this.totalCount.set(response.totalCount);
        this.currentPage.set(response.pageNumber);
        this.totalPages.set(Math.ceil(response.totalCount / response.pageSize));
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading audit logs:', err);
        this.error.set('Erro ao carregar logs de auditoria. Por favor, tente novamente.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.filter.pageNumber = 1;
    this.loadLogs();
  }

  clearFilters(): void {
    // Reset to default (last 7 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 7);
    
    this.startDate = startDate.toISOString().split('T')[0];
    this.endDate = endDate.toISOString().split('T')[0];
    
    this.filter = {
      pageNumber: 1,
      pageSize: 50,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    
    this.loadLogs();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.filter.pageNumber = page;
      this.loadLogs();
    }
  }

  nextPage(): void {
    this.goToPage(this.currentPage() + 1);
  }

  previousPage(): void {
    this.goToPage(this.currentPage() - 1);
  }

  viewDetails(log: AuditLog): void {
    this.selectedLog.set(log);
  }

  closeDetails(): void {
    this.selectedLog.set(null);
  }

  toggleFilters(): void {
    this.filtersExpanded.set(!this.filtersExpanded());
  }

  formatTimestamp(timestamp: string): string {
    return this.auditService.formatTimestamp(timestamp);
  }

  getSeverityClass(severity: string): string {
    return this.auditService.getSeverityClass(severity);
  }

  getResultClass(result: string): string {
    return this.auditService.getResultClass(result);
  }

  getPageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    
    if (total <= 7) {
      // Show all pages if 7 or less
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      // Always show first page
      pages.push(1);
      
      // Calculate range around current page
      let start = Math.max(2, current - 2);
      let end = Math.min(total - 1, current + 2);
      
      // Add ellipsis if needed
      if (start > 2) {
        pages.push(-1); // -1 represents ellipsis
      }
      
      // Add pages around current
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      // Add ellipsis if needed
      if (end < total - 1) {
        pages.push(-1); // -1 represents ellipsis
      }
      
      // Always show last page
      pages.push(total);
    }
    
    return pages;
  }

  exportToJson(): void {
    const dataStr = JSON.stringify(this.logs(), null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    const url = URL.createObjectURL(dataBlob);
    const link = document.createElement('a');
    link.href = url;
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    link.download = `audit-logs-${timestamp}.json`;
    link.click();
    URL.revokeObjectURL(url);
  }

  exportToCsv(): void {
    const logs = this.logs();
    if (logs.length === 0) return;
    
    // Helper function to sanitize CSV cells and prevent injection
    const sanitizeCsvCell = (value: any): string => {
      if (value === null || value === undefined) return '';
      const str = String(value);
      // Prevent CSV injection by escaping cells that start with dangerous characters
      if (str.match(/^[=+\-@\t]/)) {
        return `'${str}`;
      }
      return str;
    };
    
    // CSV headers
    const headers = [
      'Timestamp', 'User', 'Email', 'Action', 'Description',
      'Entity Type', 'Entity ID', 'Result', 'Severity',
      'IP Address', 'HTTP Method', 'Path'
    ];
    
    // CSV rows
    const rows = logs.map(log => [
      this.formatTimestamp(log.timestamp),
      log.userName || '',
      log.userEmail || '',
      log.action || '',
      log.actionDescription || '',
      log.entityType || '',
      log.entityId || '',
      log.result || '',
      log.severity || '',
      log.ipAddress || '',
      log.httpMethod || '',
      log.requestPath || ''
    ]);
    
    // Combine headers and rows with proper escaping
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.map(cell => `"${sanitizeCsvCell(cell).replace(/"/g, '""')}"`).join(','))
    ].join('\n');
    
    // Download
    const dataBlob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(dataBlob);
    const link = document.createElement('a');
    link.href = url;
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    link.download = `audit-logs-${timestamp}.csv`;
    link.click();
    URL.revokeObjectURL(url);
  }

  getActionIcon(action: string): string {
    switch (action.toUpperCase()) {
      case 'CREATE':
        return '➕';
      case 'READ':
        return '👁️';
      case 'UPDATE':
        return '✏️';
      case 'DELETE':
        return '🗑️';
      case 'LOGIN':
        return '🔓';
      case 'LOGOUT':
        return '🔒';
      case 'LOGIN_FAILED':
        return '❌';
      case 'ACCESS_DENIED':
        return '🚫';
      case 'EXPORT':
      case 'DOWNLOAD':
        return '📥';
      case 'PRINT':
        return '🖨️';
      default:
        return '📝';
    }
  }
}
