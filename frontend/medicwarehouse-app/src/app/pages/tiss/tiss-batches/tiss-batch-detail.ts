import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissBatchService } from '../../../services/tiss-batch.service';
import { TissBatch, BatchStatus } from '../../../models/tiss.model';

@Component({
  selector: 'app-tiss-batch-detail',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './tiss-batch-detail.html',
  styleUrl: './tiss-batch-detail.scss'
})
export class TissBatchDetailComponent implements OnInit {
  batch = signal<TissBatch | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  isGeneratingXml = signal<boolean>(false);
  isDownloadingXml = signal<boolean>(false);
  expandedGuides = signal<Set<string>>(new Set());

  constructor(
    private batchService: TissBatchService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadBatch(id);
    }
  }

  loadBatch(id: string): void {
    this.isLoading.set(true);
    this.batchService.getById(id).subscribe({
      next: (batch) => {
        this.batch.set(batch);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar lote');
        this.isLoading.set(false);
        console.error('Error loading batch:', error);
      }
    });
  }

  onGenerateXml(): void {
    const batch = this.batch();
    if (!batch) return;

    this.isGeneratingXml.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.batchService.generateXml(batch.id).subscribe({
      next: () => {
        this.successMessage.set('XML gerado com sucesso');
        this.loadBatch(batch.id);
        this.isGeneratingXml.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao gerar XML');
        this.isGeneratingXml.set(false);
        console.error('Error generating XML:', error);
      }
    });
  }

  onDownloadXml(): void {
    const batch = this.batch();
    if (!batch || !batch.xmlFilePath) return;

    this.isDownloadingXml.set(true);
    this.errorMessage.set('');

    this.batchService.downloadXml(batch.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `lote-${batch.batchNumber}.xml`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.isDownloadingXml.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao baixar XML');
        this.isDownloadingXml.set(false);
        console.error('Error downloading XML:', error);
      }
    });
  }

  toggleGuideExpansion(guideId: string): void {
    const expanded = this.expandedGuides();
    if (expanded.has(guideId)) {
      expanded.delete(guideId);
    } else {
      expanded.add(guideId);
    }
    this.expandedGuides.set(new Set(expanded));
  }

  isGuideExpanded(guideId: string): boolean {
    return this.expandedGuides().has(guideId);
  }

  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Draft': 'status-draft',
      'Generated': 'status-generated',
      'Sent': 'status-sent',
      'Processing': 'status-processing',
      'Approved': 'status-approved',
      'PartiallyApproved': 'status-partial',
      'Rejected': 'status-rejected',
      'Paid': 'status-paid'
    };
    return statusMap[status] || 'status-default';
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Draft': 'Rascunho',
      'Generated': 'Gerado',
      'Sent': 'Enviado',
      'Processing': 'Processando',
      'Approved': 'Aprovado',
      'PartiallyApproved': 'Parcialmente Aprovado',
      'Rejected': 'Rejeitado',
      'Paid': 'Pago'
    };
    return labels[status] || status;
  }

  canGenerateXml(): boolean {
    const batch = this.batch();
    return batch !== null && batch.status === 'Draft' && !batch.xmlFilePath;
  }

  canDownloadXml(): boolean {
    const batch = this.batch();
    return batch !== null && !!batch.xmlFilePath;
  }
}
