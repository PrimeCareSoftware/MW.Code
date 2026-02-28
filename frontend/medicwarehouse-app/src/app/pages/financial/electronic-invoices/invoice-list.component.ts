import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ElectronicInvoiceService } from '../../../services/electronic-invoice.service';
import { 
  ElectronicInvoiceListItem, 
  ElectronicInvoiceStatistics 
} from '../../../models/electronic-invoice.model';

@Component({
  selector: 'app-invoice-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './invoice-list.component.html',
  styleUrl: './invoice-list.component.scss'
})
export class InvoiceListComponent implements OnInit {
  invoices = signal<ElectronicInvoiceListItem[]>([]);
  filteredInvoices = signal<ElectronicInvoiceListItem[]>([]);
  statistics = signal<ElectronicInvoiceStatistics | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // Filters
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  startDate = signal<string>('');
  endDate = signal<string>('');

  constructor(
    private invoiceService: ElectronicInvoiceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.setDefaultDates();
    this.loadInvoices();
    this.loadStatistics();
  }

  setDefaultDates(): void {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    this.startDate.set(this.formatDateForInput(firstDay));
    this.endDate.set(this.formatDateForInput(today));
  }

  formatDateForInput(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  loadInvoices(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    const startDate = this.startDate() ? new Date(this.startDate()) : undefined;
    const endDate = this.endDate() ? new Date(this.endDate()) : undefined;
    const status = this.statusFilter() !== 'all' ? this.statusFilter() : undefined;

    this.invoiceService.getInvoices(startDate, endDate, status).subscribe({
      next: (data) => {
        this.invoices.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar notas fiscais');
        this.isLoading.set(false);
        console.error('Error loading invoices:', error);
      }
    });
  }

  loadStatistics(): void {
    const startDate = this.startDate() ? new Date(this.startDate()) : undefined;
    const endDate = this.endDate() ? new Date(this.endDate()) : undefined;

    this.invoiceService.getStatistics(startDate, endDate).subscribe({
      next: (data) => this.statistics.set(data),
      error: (error) => console.error('Error loading statistics:', error)
    });
  }

  applyFilters(): void {
    let filtered = this.invoices();
    
    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(inv => 
        inv.number.toLowerCase().includes(term) ||
        inv.clientName.toLowerCase().includes(term) ||
        inv.accessKey?.toLowerCase().includes(term)
      );
    }

    this.filteredInvoices.set(filtered);
  }

  onSearchChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.applyFilters();
  }

  onStatusFilterChange(event: Event): void {
    const status = (event.target as HTMLSelectElement)?.value || 'all';
    this.statusFilter.set(status);
    this.loadInvoices();
    this.loadStatistics();
  }

  onDateChange(): void {
    this.loadInvoices();
    this.loadStatistics();
  }

  downloadPdf(id: string, event: Event): void {
    event.stopPropagation();
    this.invoiceService.downloadPdf(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `nfse-${id}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar PDF');
        console.error('Error downloading PDF:', error);
      }
    });
  }

  downloadXml(id: string, event: Event): void {
    event.stopPropagation();
    this.invoiceService.downloadXml(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `nfse-${id}.xml`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar XML');
        console.error('Error downloading XML:', error);
      }
    });
  }

  sendEmail(id: string, event: Event): void {
    event.stopPropagation();
    const email = prompt('Digite o e-mail de destino:');
    if (email) {
      this.invoiceService.sendByEmail(id, { email }).subscribe({
        next: () => {
          this.successMessage.set('E-mail enviado com sucesso');
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao enviar e-mail');
          console.error('Error sending email:', error);
        }
      });
    }
  }

  cancelInvoice(id: string, event: Event): void {
    event.stopPropagation();
    const reason = prompt('Informe o motivo do cancelamento:');
    if (reason) {
      if (confirm('Tem certeza que deseja cancelar esta nota fiscal?')) {
        this.invoiceService.cancelInvoice(id, { reason }).subscribe({
          next: () => {
            this.successMessage.set('Nota fiscal cancelada com sucesso');
            setTimeout(() => this.successMessage.set(''), 3000);
            this.loadInvoices();
            this.loadStatistics();
          },
          error: (error) => {
            this.errorMessage.set('Erro ao cancelar nota fiscal');
            console.error('Error cancelling invoice:', error);
          }
        });
      }
    }
  }

  viewDetails(id: string): void {
    this.router.navigate(['/financial/invoices', id]);
  }

  getStatusBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Draft': 'badge-default',
      'Pending': 'badge-warning',
      'Authorized': 'badge-success',
      'Cancelled': 'badge-error',
      'Error': 'badge-error'
    };
    return statusMap[status] || 'badge-default';
  }

  formatStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Draft': 'Rascunho',
      'Pending': 'Pendente',
      'Authorized': 'Autorizada',
      'Cancelled': 'Cancelada',
      'Error': 'Erro'
    };
    return statusMap[status] || status;
  }

  formatType(type: string): string {
    const typeMap: { [key: string]: string } = {
      'NFSe': 'NFS-e',
      'NFe': 'NF-e',
      'NFCe': 'NFC-e'
    };
    return typeMap[type] || type;
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(amount);
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }
}
