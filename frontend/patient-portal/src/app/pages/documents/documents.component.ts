import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { DocumentService } from '../../services/document.service';
import { NotificationService } from '../../services/notification.service';
import { Document } from '../../models/document.model';

@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    MatDividerModule
  ],
  templateUrl: './documents.component.html',
  styleUrls: ['./documents.component.scss']
})
export class DocumentsComponent implements OnInit {
  documents: Document[] = [];
  loading = true;
  loadingError = false;
  downloadingIds: Set<string> = new Set();

  constructor(
    private documentService: DocumentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading = true;
    this.loadingError = false;

    this.documentService.getMyDocuments().subscribe({
      next: (documents) => {
        this.documents = documents;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading documents:', error);
        this.loadingError = true;
        this.loading = false;
        this.notificationService.error('Erro ao carregar documentos');
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  getDocumentTypeColor(type: string): string {
    const typeColorMap: { [key: string]: string } = {
      'Prescription': 'primary',
      'MedicalCertificate': 'accent',
      'LabReport': 'warn',
      'ImagingReport': 'primary',
      'MedicalReport': 'accent',
      'Other': 'default'
    };
    return typeColorMap[type] || 'default';
  }

  isDownloading(docId: string): boolean {
    return this.downloadingIds.has(docId);
  }

  downloadDocument(doc: Document): void {
    if (!doc.isAvailable || !doc.id || this.isDownloading(doc.id)) {
      return;
    }

    const docId = doc.id; // Store in local variable to satisfy TypeScript
    this.downloadingIds.add(docId);

    this.documentService.downloadDocument(docId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = window.document.createElement('a');
        link.href = url;
        link.download = doc.fileName || `${doc.title}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.downloadingIds.delete(docId);
        this.notificationService.success('Download iniciado com sucesso!');
      },
      error: (error) => {
        console.error('Error downloading document:', error);
        this.downloadingIds.delete(docId);
        this.notificationService.error('Erro ao baixar documento. Tente novamente.');
      }
    });
  }

  retry(): void {
    this.loadDocuments();
  }
}
