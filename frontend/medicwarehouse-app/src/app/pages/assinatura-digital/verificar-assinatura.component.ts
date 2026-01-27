import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AssinaturaDigitalService } from '../../../services/assinatura-digital.service';
import { AssinaturaDigital } from '../../../models/assinatura-digital.model';

@Component({
  selector: 'app-verificar-assinatura',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './verificar-assinatura.component.html',
  styleUrls: ['./verificar-assinatura.component.scss']
})
export class VerificarAssinaturaComponent {
  @Input() assinatura!: AssinaturaDigital;
  isRevalidando = signal<boolean>(false);

  constructor(
    private assinaturaService: AssinaturaDigitalService,
    private snackBar: MatSnackBar
  ) {}

  revalidar(): void {
    if (!this.assinatura?.id) {
      return;
    }

    this.isRevalidando.set(true);
    this.assinaturaService.validarAssinatura(this.assinatura.id).subscribe({
      next: (resultado) => {
        this.assinatura.valida = resultado.valida;
        this.assinatura.dataUltimaValidacao = new Date();
        
        const mensagem = resultado.valida 
          ? 'Assinatura válida!' 
          : `Assinatura inválida: ${resultado.motivo}`;
        
        this.snackBar.open(mensagem, 'Fechar', { 
          duration: 5000,
          panelClass: resultado.valida ? 'success-snackbar' : 'error-snackbar'
        });
        
        this.isRevalidando.set(false);
      },
      error: (error) => {
        this.snackBar.open('Erro ao validar assinatura', 'Fechar', { duration: 5000 });
        this.isRevalidando.set(false);
        console.error('Erro ao validar assinatura:', error);
      }
    });
  }

  getStatusIcon(): string {
    return this.assinatura?.valida ? 'verified' : 'warning';
  }

  getStatusText(): string {
    return this.assinatura?.valida ? 'Assinatura Válida' : 'Assinatura Inválida';
  }

  getStatusColor(): 'primary' | 'warn' {
    return this.assinatura?.valida ? 'primary' : 'warn';
  }

  truncateHash(hash: string): string {
    if (!hash || hash.length <= 16) return hash;
    return `${hash.substring(0, 8)}...${hash.substring(hash.length - 8)}`;
  }
}
