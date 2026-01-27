import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FilaEsperaService } from '../../../services/fila-espera.service';
import { ConsultarSenhaResponse, StatusSenha } from '../../../models/fila-espera.model';

@Component({
  selector: 'app-consultar-senha',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './consultar-senha.component.html',
  styleUrls: ['./consultar-senha.component.scss']
})
export class ConsultarSenhaComponent implements OnInit {
  form!: FormGroup;
  clinicId = signal<string>('');
  filaId = signal<string>('');
  loading = signal<boolean>(false);
  resultado = signal<ConsultarSenhaResponse | null>(null);
  StatusSenha = StatusSenha;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private filaService: FilaEsperaService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.clinicId.set(params['clinicId']);
      this.filaId.set(params['filaId']);
    });

    this.form = this.fb.group({
      numeroSenha: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  async consultar(): Promise<void> {
    if (!this.form.valid) {
      this.snackBar.open('Digite o número da senha', 'OK', { duration: 3000 });
      return;
    }

    this.loading.set(true);

    try {
      const numeroSenha = this.form.value.numeroSenha.toUpperCase().trim();
      const resultado = await this.filaService.consultarSenhaAsync(
        this.filaId(),
        numeroSenha,
        this.clinicId()
      );
      
      this.resultado.set(resultado);
    } catch (error: any) {
      console.error('Error consulting senha', error);
      this.snackBar.open(
        error?.error?.message || 'Senha não encontrada',
        'OK',
        { duration: 5000 }
      );
      this.resultado.set(null);
    } finally {
      this.loading.set(false);
    }
  }

  getStatusText(status: StatusSenha): string {
    switch (status) {
      case StatusSenha.Aguardando:
        return 'Aguardando Atendimento';
      case StatusSenha.Chamando:
        return 'Sendo Chamado';
      case StatusSenha.EmAtendimento:
        return 'Em Atendimento';
      case StatusSenha.Atendido:
        return 'Atendido';
      case StatusSenha.NaoCompareceu:
        return 'Não Compareceu';
      case StatusSenha.Cancelado:
        return 'Cancelado';
      default:
        return 'Desconhecido';
    }
  }

  getStatusColor(status: StatusSenha): string {
    switch (status) {
      case StatusSenha.Aguardando:
        return '#2196F3';
      case StatusSenha.Chamando:
        return '#FF9800';
      case StatusSenha.EmAtendimento:
        return '#4CAF50';
      case StatusSenha.Atendido:
        return '#757575';
      case StatusSenha.NaoCompareceu:
      case StatusSenha.Cancelado:
        return '#F44336';
      default:
        return '#757575';
    }
  }

  getStatusIcon(status: StatusSenha): string {
    switch (status) {
      case StatusSenha.Aguardando:
        return 'schedule';
      case StatusSenha.Chamando:
        return 'notifications_active';
      case StatusSenha.EmAtendimento:
        return 'medical_services';
      case StatusSenha.Atendido:
        return 'check_circle';
      case StatusSenha.NaoCompareceu:
        return 'person_off';
      case StatusSenha.Cancelado:
        return 'cancel';
      default:
        return 'help';
    }
  }

  voltarAoMenu(): void {
    this.router.navigate(['/fila-espera/totem', this.clinicId(), this.filaId()]);
  }

  novaConsulta(): void {
    this.resultado.set(null);
    this.form.reset();
  }

  onSenhaInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value.toUpperCase();
    this.form.patchValue({ numeroSenha: value });
  }
}
