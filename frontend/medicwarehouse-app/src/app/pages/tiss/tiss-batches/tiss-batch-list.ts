import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TissBatchService } from '../../../services/tiss-batch.service';
import { TissBatch, BatchStatus } from '../../../models/tiss.model';

@Component({
  selector: 'app-tiss-batch-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './tiss-batch-list.html',
  styleUrl: './tiss-batch-list.scss'
})
export class TissBatchList implements OnInit {
  batches = signal<TissBatch[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  BatchStatus = BatchStatus;

  constructor(
    private batchService: TissBatchService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBatches();
  }

  loadBatches(): void {
    this.isLoading.set(true);
    this.batchService.getAll().subscribe({
      next: (data) => {
        this.batches.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar lotes');
        this.isLoading.set(false);
        console.error('Error loading batches:', error);
      }
    });
  }

  generateXml(id: string): void {
    this.isLoading.set(true);
    this.batchService.generateXml(id).subscribe({
      next: () => {
        this.successMessage.set('XML gerado com sucesso');
        this.loadBatches();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao gerar XML');
        this.isLoading.set(false);
        console.error('Error generating XML:', error);
      }
    });
  }

  downloadXml(id: string): void {
    this.batchService.downloadXml(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `lote-${id}.xml`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar XML');
        console.error('Error downloading XML:', error);
      }
    });
  }

  viewDetails(id: string): void {
    this.router.navigate(['/tiss/batches', id]);
  }

  getStatusClass(status: BatchStatus): string {
    const statusMap: Record<BatchStatus, string> = {
      [BatchStatus.Draft]: 'badge-secondary',
      [BatchStatus.Generated]: 'badge-info',
      [BatchStatus.Sent]: 'badge-primary',
      [BatchStatus.Processing]: 'badge-warning',
      [BatchStatus.Approved]: 'badge-success',
      [BatchStatus.PartiallyApproved]: 'badge-warning',
      [BatchStatus.Rejected]: 'badge-danger',
      [BatchStatus.Paid]: 'badge-success'
    };
    return statusMap[status] || 'badge-secondary';
  }

  getStatusLabel(status: BatchStatus): string {
    const labelMap: Record<BatchStatus, string> = {
      [BatchStatus.Draft]: 'Rascunho',
      [BatchStatus.Generated]: 'Gerado',
      [BatchStatus.Sent]: 'Enviado',
      [BatchStatus.Processing]: 'Processando',
      [BatchStatus.Approved]: 'Aprovado',
      [BatchStatus.PartiallyApproved]: 'Parcialmente Aprovado',
      [BatchStatus.Rejected]: 'Rejeitado',
      [BatchStatus.Paid]: 'Pago'
    };
    return labelMap[status] || status;
  }
}
