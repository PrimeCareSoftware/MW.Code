import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { FilaEsperaService } from '../../../services/fila-espera.service';
import { FilaEspera } from '../../../models/fila-espera.model';

@Component({
  selector: 'app-totem',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './totem.component.html',
  styleUrls: ['./totem.component.scss']
})
export class TotemComponent implements OnInit {
  clinicId = signal<string>('');
  filaId = signal<string>('');
  fila = signal<FilaEspera | null>(null);
  loading = signal<boolean>(false);
  currentTime = signal<Date>(new Date());

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private filaService: FilaEsperaService
  ) {
    // Update clock every second
    setInterval(() => {
      this.currentTime.set(new Date());
    }, 1000);
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.clinicId.set(params['clinicId']);
      this.filaId.set(params['filaId']);
      this.loadFila();
    });
  }

  async loadFila(): Promise<void> {
    try {
      this.loading.set(true);
      const fila = await this.filaService.getFilaAsync(this.filaId());
      this.fila.set(fila);
    } catch (error) {
      console.error('Error loading fila', error);
    } finally {
      this.loading.set(false);
    }
  }

  checkIn(): void {
    this.router.navigate([
      '/fila-espera/check-in',
      this.clinicId(),
      this.filaId()
    ]);
  }

  gerarSenha(): void {
    this.router.navigate([
      '/fila-espera/gerar-senha',
      this.clinicId(),
      this.filaId()
    ]);
  }

  consultarSenha(): void {
    this.router.navigate([
      '/fila-espera/consultar',
      this.clinicId(),
      this.filaId()
    ]);
  }
}
