import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CertificadoDigitalService } from '../../services/certificado-digital.service';
import { CertificadoDigital, TipoCertificado } from '../../models/certificado-digital.model';
import { ImportarCertificadoComponent } from './importar-certificado.component';

@Component({
  selector: 'app-gerenciar-certificados',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatTooltipModule,
  ],
  templateUrl: './gerenciar-certificados.component.html',
  styleUrls: ['./gerenciar-certificados.component.scss']
})
export class GerenciarCertificadosComponent implements OnInit {
  certificados = signal<CertificadoDigital[]>([]);
  isLoading = signal<boolean>(false);
  displayedColumns: string[] = ['tipo', 'subjectName', 'issuerName', 'dataExpiracao', 'status', 'totalAssinaturas', 'acoes'];

  constructor(
    private certificadoService: CertificadoDigitalService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.carregarCertificados();
  }

  carregarCertificados(): void {
    this.isLoading.set(true);
    this.certificadoService.listarCertificados().subscribe({
      next: (data: CertificadoDigital[]) => {
        this.certificados.set(data);
        this.isLoading.set(false);
      },
      error: (error: any) => {
        this.snackBar.open('Erro ao carregar certificados', 'Fechar', { duration: 5000 });
        this.isLoading.set(false);
        console.error('Erro ao carregar certificados:', error);
      }
    });
  }

  abrirDialogImportar(): void {
    const dialogRef = this.dialog.open(ImportarCertificadoComponent, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregarCertificados();
      }
    });
  }

  invalidarCertificado(certificado: CertificadoDigital): void {
    if (confirm(`Tem certeza que deseja invalidar o certificado ${certificado.subjectName}?`)) {
      this.certificadoService.invalidarCertificado(certificado.id).subscribe({
        next: () => {
          this.snackBar.open('Certificado invalidado com sucesso', 'Fechar', { duration: 3000 });
          this.carregarCertificados();
        },
        error: (error: any) => {
          this.snackBar.open('Erro ao invalidar certificado', 'Fechar', { duration: 5000 });
          console.error('Erro ao invalidar certificado:', error);
        }
      });
    }
  }

  getStatusChipColor(certificado: CertificadoDigital): string {
    if (!certificado.valido) return 'warn';
    if (certificado.diasParaExpiracao !== undefined && certificado.diasParaExpiracao < 30) return 'accent';
    return 'primary';
  }

  getStatusText(certificado: CertificadoDigital): string {
    if (!certificado.valido) return 'Inválido';
    if (certificado.diasParaExpiracao !== undefined && certificado.diasParaExpiracao < 0) return 'Expirado';
    if (certificado.diasParaExpiracao !== undefined && certificado.diasParaExpiracao < 30) return `Expira em ${certificado.diasParaExpiracao} dias`;
    return 'Válido';
  }

  getTipoCertificadoIcon(tipo: TipoCertificado): string {
    return tipo === TipoCertificado.A1 ? 'computer' : 'usb';
  }

  getTipoCertificadoTooltip(tipo: TipoCertificado): string {
    return tipo === TipoCertificado.A1 
      ? 'Certificado A1 - Armazenado em software' 
      : 'Certificado A3 - Token/Smartcard';
  }
}
