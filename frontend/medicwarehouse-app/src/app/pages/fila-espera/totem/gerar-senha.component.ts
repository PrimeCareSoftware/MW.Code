import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FilaEsperaService } from '../../../services/fila-espera.service';
import { SenhaFila, GerarSenhaRequest } from '../../../models/fila-espera.model';

@Component({
  selector: 'app-gerar-senha',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './gerar-senha.component.html',
  styleUrls: ['./gerar-senha.component.scss']
})
export class GerarSenhaComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  clinicId = signal<string>('');
  filaId = signal<string>('');
  loading = signal<boolean>(false);
  senhaGerada = signal<SenhaFila | null>(null);
  posicaoNaFila = signal<number>(0);
  tempoEstimado = signal<number>(0);
  autoReturnTimer?: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private filaService: FilaEsperaService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.clinicId.set(params['clinicId']);
      this.filaId.set(params['filaId']);
    });

    this.form = this.fb.group({
      nomePaciente: ['', [Validators.required, Validators.minLength(3)]],
      cpf: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      telefone: ['', [Validators.required, Validators.minLength(10)]],
      dataNascimento: ['', Validators.required],
      especialidadeId: [null],
      isGestante: [false],
      isDeficiente: [false]
    });
  }

  ngOnDestroy(): void {
    if (this.autoReturnTimer) {
      clearTimeout(this.autoReturnTimer);
    }
  }

  async gerarSenha(): Promise<void> {
    if (!this.form.valid) {
      this.snackBar.open('Por favor, preencha todos os campos obrigatórios', 'OK', {
        duration: 3000
      });
      return;
    }

    this.loading.set(true);

    try {
      const request: GerarSenhaRequest = {
        filaId: this.filaId(),
        nomePaciente: this.form.value.nomePaciente,
        cpf: this.form.value.cpf,
        telefone: this.form.value.telefone,
        dataNascimento: new Date(this.form.value.dataNascimento),
        especialidadeId: this.form.value.especialidadeId,
        isGestante: this.form.value.isGestante,
        isDeficiente: this.form.value.isDeficiente
      };

      const senha = await this.filaService.gerarSenhaAsync(
        this.filaId(),
        request,
        this.clinicId()
      );

      this.senhaGerada.set(senha);
      
      // Get position and estimated time
      const consulta = await this.filaService.consultarSenhaAsync(
        this.filaId(),
        senha.numeroSenha,
        this.clinicId()
      );
      
      this.posicaoNaFila.set(consulta.posicaoNaFila);
      this.tempoEstimado.set(consulta.tempoEstimadoMinutos);

      // Show success message
      this.snackBar.open('Senha gerada com sucesso!', 'OK', {
        duration: 3000
      });

      // Auto-return to main menu after 15 seconds
      this.autoReturnTimer = setTimeout(() => {
        this.voltarAoMenu();
      }, 15000);

    } catch (error: any) {
      console.error('Error generating senha', error);
      this.snackBar.open(
        error?.error?.message || 'Erro ao gerar senha. Tente novamente.',
        'OK',
        { duration: 5000 }
      );
    } finally {
      this.loading.set(false);
    }
  }

  formatCpf(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length > 11) {
      value = value.substring(0, 11);
    }
    this.form.patchValue({ cpf: value });
  }

  formatTelefone(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length > 11) {
      value = value.substring(0, 11);
    }
    this.form.patchValue({ telefone: value });
  }

  voltarAoMenu(): void {
    if (this.autoReturnTimer) {
      clearTimeout(this.autoReturnTimer);
    }
    this.router.navigate(['/fila-espera/totem', this.clinicId(), this.filaId()]);
  }

  gerarNovaSenha(): void {
    this.senhaGerada.set(null);
    this.form.reset({
      isGestante: false,
      isDeficiente: false
    });
    if (this.autoReturnTimer) {
      clearTimeout(this.autoReturnTimer);
    }
  }
}
