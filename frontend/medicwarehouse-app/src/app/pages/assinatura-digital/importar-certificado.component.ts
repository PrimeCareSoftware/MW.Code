import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CertificadoDigitalService } from '../../../services/certificado-digital.service';
import { CertificateInfo } from '../../../models/certificado-digital.model';

@Component({
  selector: 'app-importar-certificado',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './importar-certificado.component.html',
  styleUrls: ['./importar-certificado.component.scss']
})
export class ImportarCertificadoComponent {
  // A1 fields
  arquivoPfx: File | null = null;
  senhaA1: string = '';
  
  // A3 fields
  certificadosA3Disponiveis = signal<CertificateInfo[]>([]);
  certificadoA3Selecionado: string = '';
  
  isImportando = signal<boolean>(false);
  isCarregandoA3 = signal<boolean>(false);

  constructor(
    private certificadoService: CertificadoDigitalService,
    private dialogRef: MatDialogRef<ImportarCertificadoComponent>,
    private snackBar: MatSnackBar
  ) {}

  onTabChange(event: any): void {
    if (event.index === 1) {
      // Tab A3 selecionada - carregar certificados disponíveis
      this.carregarCertificadosA3();
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      if (file.name.toLowerCase().endsWith('.pfx') || file.name.toLowerCase().endsWith('.p12')) {
        this.arquivoPfx = file;
      } else {
        this.snackBar.open('Por favor, selecione um arquivo .pfx ou .p12', 'Fechar', { duration: 5000 });
        event.target.value = '';
      }
    }
  }

  carregarCertificadosA3(): void {
    this.isCarregandoA3.set(true);
    this.certificadoService.listarCertificadosA3Disponiveis().subscribe({
      next: (data) => {
        this.certificadosA3Disponiveis.set(data);
        this.isCarregandoA3.set(false);
        
        if (data.length === 0) {
          this.snackBar.open(
            'Nenhum certificado A3 encontrado. Certifique-se de que o token está conectado.',
            'Fechar',
            { duration: 5000 }
          );
        }
      },
      error: (error) => {
        this.snackBar.open('Erro ao listar certificados A3', 'Fechar', { duration: 5000 });
        this.isCarregandoA3.set(false);
        console.error('Erro ao listar certificados A3:', error);
      }
    });
  }

  importarA1(): void {
    if (!this.arquivoPfx) {
      this.snackBar.open('Selecione um arquivo de certificado', 'Fechar', { duration: 3000 });
      return;
    }

    if (!this.senhaA1) {
      this.snackBar.open('Digite a senha do certificado', 'Fechar', { duration: 3000 });
      return;
    }

    this.isImportando.set(true);
    this.certificadoService.importarCertificadoA1(this.arquivoPfx, this.senhaA1).subscribe({
      next: (certificado) => {
        this.snackBar.open('Certificado A1 importado com sucesso!', 'Fechar', { duration: 3000 });
        this.isImportando.set(false);
        this.dialogRef.close(true);
      },
      error: (error) => {
        const mensagem = error.error?.message || error.error || 'Erro ao importar certificado A1';
        this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
        this.isImportando.set(false);
        console.error('Erro ao importar certificado A1:', error);
      }
    });
  }

  registrarA3(): void {
    if (!this.certificadoA3Selecionado) {
      this.snackBar.open('Selecione um certificado', 'Fechar', { duration: 3000 });
      return;
    }

    this.isImportando.set(true);
    this.certificadoService.registrarCertificadoA3(this.certificadoA3Selecionado).subscribe({
      next: (certificado) => {
        this.snackBar.open('Certificado A3 registrado com sucesso!', 'Fechar', { duration: 3000 });
        this.isImportando.set(false);
        this.dialogRef.close(true);
      },
      error: (error) => {
        const mensagem = error.error?.message || error.error || 'Erro ao registrar certificado A3';
        this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
        this.isImportando.set(false);
        console.error('Erro ao registrar certificado A3:', error);
      }
    });
  }

  cancelar(): void {
    this.dialogRef.close(false);
  }

  formatarCertificadoA3(cert: CertificateInfo): string {
    const validTo = new Date(cert.validTo);
    const dataFormatada = validTo.toLocaleDateString('pt-BR');
    return `${cert.subject} (Válido até: ${dataFormatada})`;
  }
}
