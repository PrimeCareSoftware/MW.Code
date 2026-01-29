import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-data-portability',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatRadioModule
  ],
  templateUrl: './DataPortability.component.html'
})
export class DataPortabilityComponent {
  selectedFormat = 'json';
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  formats = [
    { value: 'json', label: 'JSON', icon: 'data_object', description: 'Formato estruturado legível por máquinas' },
    { value: 'xml', label: 'XML', icon: 'code', description: 'Formato XML compatível com sistemas legados' },
    { value: 'pdf', label: 'PDF', icon: 'picture_as_pdf', description: 'Documento PDF para leitura humana' },
    { value: 'package', label: 'Pacote Completo', icon: 'folder_zip', description: 'Todos os dados em arquivo ZIP' }
  ];

  constructor(private http: HttpClient) {}

  exportAsJson(): void {
    this.exportData('json', 'application/json', 'meus-dados.json');
  }

  exportAsXml(): void {
    this.exportData('xml', 'application/xml', 'meus-dados.xml');
  }

  exportAsPdf(): void {
    this.exportData('pdf', 'application/pdf', 'meus-dados.pdf');
  }

  exportAsPackage(): void {
    this.exportData('package', 'application/zip', 'meus-dados-completo.zip');
  }

  exportData(format: string, mimeType: string, filename: string): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;

    this.http
      .get(`${environment.apiUrl}/patient-portal/export-data?format=${format}`, {
        responseType: 'blob',
        observe: 'response'
      })
      .subscribe({
        next: (response) => {
          this.loading = false;
          
          if (response.body) {
            const blob = new Blob([response.body], { type: mimeType });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            link.click();
            window.URL.revokeObjectURL(url);

            this.successMessage = `Dados exportados com sucesso em formato ${format.toUpperCase()}!`;
            
            setTimeout(() => {
              this.successMessage = null;
            }, 5000);
          }
        },
        error: (error) => {
          this.loading = false;
          this.error = 'Erro ao exportar dados. Tente novamente mais tarde.';
          console.error('Export error:', error);
        }
      });
  }

  executeExport(): void {
    switch (this.selectedFormat) {
      case 'json':
        this.exportAsJson();
        break;
      case 'xml':
        this.exportAsXml();
        break;
      case 'pdf':
        this.exportAsPdf();
        break;
      case 'package':
        this.exportAsPackage();
        break;
    }
  }
}
