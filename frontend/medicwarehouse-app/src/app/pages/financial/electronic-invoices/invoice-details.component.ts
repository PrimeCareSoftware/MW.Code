import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ElectronicInvoiceService } from '../../../services/electronic-invoice.service';
import { ElectronicInvoice } from '../../../models/electronic-invoice.model';

@Component({
  selector: 'app-invoice-details',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './invoice-details.component.html',
  styleUrl: './invoice-details.component.scss'
})
export class InvoiceDetailsComponent implements OnInit {
  invoice = signal<ElectronicInvoice | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  showCancelForm = signal<boolean>(false);
  cancellationReason = signal<string>('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private invoiceService: ElectronicInvoiceService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadInvoice(id);
    }
  }

  loadInvoice(id: string): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.invoiceService.getInvoiceById(id).subscribe({
      next: (data) => {
        this.invoice.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar nota fiscal');
        this.isLoading.set(false);
        console.error('Error loading invoice:', error);
      }
    });
  }

  issueInvoice(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    if (!confirm('Deseja emitir esta nota fiscal? Esta ação não pode ser desfeita.')) {
      return;
    }

    this.isLoading.set(true);
    this.invoiceService.issueInvoice(invoice.id).subscribe({
      next: (updated) => {
        this.invoice.set(updated);
        this.successMessage.set('Nota fiscal emitida com sucesso!');
        this.isLoading.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao emitir nota fiscal');
        this.isLoading.set(false);
      }
    });
  }

  toggleCancelForm(): void {
    this.showCancelForm.set(!this.showCancelForm());
    this.cancellationReason.set('');
  }

  cancelInvoice(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    const reason = this.cancellationReason();
    if (!reason.trim()) {
      this.errorMessage.set('Informe o motivo do cancelamento');
      return;
    }

    if (!confirm('Tem certeza que deseja cancelar esta nota fiscal?')) {
      return;
    }

    this.isLoading.set(true);
    this.invoiceService.cancelInvoice(invoice.id, { reason }).subscribe({
      next: (updated) => {
        this.invoice.set(updated);
        this.showCancelForm.set(false);
        this.successMessage.set('Nota fiscal cancelada com sucesso!');
        this.isLoading.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao cancelar nota fiscal');
        this.isLoading.set(false);
      }
    });
  }

  replaceInvoice(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    const reason = prompt('Informe o motivo da substituição:');
    if (!reason) return;

    if (!confirm('Deseja cancelar esta nota e criar uma nova?')) {
      return;
    }

    this.isLoading.set(true);
    this.invoiceService.replaceInvoice(invoice.id, { reason }).subscribe({
      next: (newInvoice) => {
        this.successMessage.set('Nota fiscal substituída com sucesso!');
        setTimeout(() => {
          this.router.navigate(['/financial/invoices', newInvoice.id]);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao substituir nota fiscal');
        this.isLoading.set(false);
      }
    });
  }

  downloadPdf(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    this.invoiceService.downloadPdf(invoice.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `nfse-${invoice.number}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar PDF');
        console.error('Error downloading PDF:', error);
      }
    });
  }

  downloadXml(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    this.invoiceService.downloadXml(invoice.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `nfse-${invoice.number}.xml`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar XML');
        console.error('Error downloading XML:', error);
      }
    });
  }

  sendEmail(): void {
    const invoice = this.invoice();
    if (!invoice) return;

    const email = prompt('Digite o e-mail de destino:', invoice.clientEmail || '');
    if (!email) return;

    this.invoiceService.sendByEmail(invoice.id, { email }).subscribe({
      next: () => {
        this.successMessage.set('E-mail enviado com sucesso!');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao enviar e-mail');
        console.error('Error sending email:', error);
      }
    });
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

  formatPercent(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  formatDate(dateString: string | Date | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  formatDateTime(dateString: string | Date | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleString('pt-BR');
  }
}
