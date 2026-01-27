import { Component, Inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CertificadoDigitalService } from '../../../services/certificado-digital.service';
import { AssinaturaDigitalService } from '../../../services/assinatura-digital.service';
import { CertificadoDigital, TipoCertificado } from '../../../models/certificado-digital.model';
import { TipoDocumento } from '../../../models/assinatura-digital.model';

export interface AssinarDocumentoDialogData {
  documentoId: string;
  tipoDocumento: TipoDocumento;
  tipoDocumentoNome: string;
  documentoBytes: string; // Base64
  pacienteNome?: string;
  dataDocumento?: Date | string;
}

@Component({
  selector: 'app-assinar-documento',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './assinar-documento.component.html',
  styleUrls: ['./assinar-documento.component.scss']
})
export class AssinarDocumentoComponent implements OnInit {
  certificados = signal<CertificadoDigital[]>([]);
  certificadoSelecionado: string = '';
  senha: string = '';
  incluirTimestamp: boolean = true;
  isCarregando = signal<boolean>(false);
  isAssinando = signal<boolean>(false);
  TipoCertificado = TipoCertificado;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AssinarDocumentoDialogData,
    private dialogRef: MatDialogRef<AssinarDocumentoComponent>,
    private certificadoService: CertificadoDigitalService,
    private assinaturaService: AssinaturaDigitalService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.carregarCertificados();
  }

  carregarCertificados(): void {
    this.isCarregando.set(true);
    this.certificadoService.listarCertificados().subscribe({
      next: (data) => {
        const certificadosValidos = data.filter(c => c.valido);
        this.certificados.set(certificadosValidos);
        this.isCarregando.set(false);

        if (certificadosValidos.length === 0) {
          this.snackBar.open(
            'Você não possui certificados válidos. Por favor, importe um certificado primeiro.',
            'Fechar',
            { duration: 5000 }
          );
        } else if (certificadosValidos.length === 1) {
          this.certificadoSelecionado = certificadosValidos[0].id;
        }
      },
      error: (error) => {
        this.snackBar.open('Erro ao carregar certificados', 'Fechar', { duration: 5000 });
        this.isCarregando.set(false);
        console.error('Erro ao carregar certificados:', error);
      }
    });
  }

  getCertificadoSelecionadoDetalhes(): CertificadoDigital | undefined {
    return this.certificados().find(c => c.id === this.certificadoSelecionado);
  }

  precisaSenha(): boolean {
    const cert = this.getCertificadoSelecionadoDetalhes();
    return cert?.tipo === TipoCertificado.A1;
  }

  assinar(): void {
    if (!this.certificadoSelecionado) {
      this.snackBar.open('Selecione um certificado', 'Fechar', { duration: 3000 });
      return;
    }

    const cert = this.getCertificadoSelecionadoDetalhes();
    if (cert?.tipo === TipoCertificado.A1 && !this.senha) {
      this.snackBar.open('Digite a senha do certificado', 'Fechar', { duration: 3000 });
      return;
    }

    this.isAssinando.set(true);

    const request = {
      documentoId: this.data.documentoId,
      tipoDocumento: this.data.tipoDocumento,
      documentoBytes: this.data.documentoBytes,
      senhaCertificado: this.senha && this.senha.trim() ? this.senha : undefined
    };

    this.assinaturaService.assinarDocumento(request).subscribe({
      next: (resultado) => {
        if (resultado.sucesso) {
          this.snackBar.open('Documento assinado com sucesso!', 'Fechar', { duration: 3000 });
          this.dialogRef.close(resultado);
        } else {
          this.snackBar.open(resultado.mensagem || 'Erro ao assinar documento', 'Fechar', { duration: 5000 });
          this.isAssinando.set(false);
        }
      },
      error: (error) => {
        const mensagem = error.error?.message || error.error || 'Erro ao assinar documento';
        this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
        this.isAssinando.set(false);
        console.error('Erro ao assinar documento:', error);
      }
    });
  }

  cancelar(): void {
    this.dialogRef.close(null);
  }

  formatarDataExpiracao(data: Date | string): string {
    const date = new Date(data);
    return date.toLocaleDateString('pt-BR');
  }
}
