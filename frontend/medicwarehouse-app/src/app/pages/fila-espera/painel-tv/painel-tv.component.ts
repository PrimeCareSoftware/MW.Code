import { Component, OnInit, OnDestroy, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { FilaEsperaService } from '../../../services/fila-espera.service';
import { FilaSignalRService } from '../../../services/fila-signalr.service';
import { SenhaFila, ChamarSenhaEvent, PrioridadeAtendimento } from '../../../models/fila-espera.model';

@Component({
  selector: 'app-painel-tv',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule
  ],
  templateUrl: './painel-tv.component.html',
  styleUrls: ['./painel-tv.component.scss']
})
export class PainelTvComponent implements OnInit, OnDestroy {
  clinicId = signal<string>('');
  filaId = signal<string>('');
  chamadaAtual = signal<ChamarSenhaEvent | null>(null);
  ultimasChamadas = signal<ChamarSenhaEvent[]>([]);
  senhasAguardando = signal<SenhaFila[]>([]);
  tempoMedioEspera = signal<number>(0);
  currentTime = signal<Date>(new Date());
  isConnected = signal<boolean>(false);
  
  private refreshInterval?: any;
  private clockInterval?: any;
  private chamadaTimeout?: any;

  constructor(
    private route: ActivatedRoute,
    private filaService: FilaEsperaService,
    private signalRService: FilaSignalRService
  ) {
    // Setup effect for SignalR events
    effect(() => {
      const event = this.signalRService.chamarSenhaEvent();
      if (event) {
        this.onChamarSenha(event);
      }
    }, { allowSignalWrites: true });

    effect(() => {
      const senhaId = this.signalRService.senhaEmAtendimentoEvent();
      if (senhaId) {
        this.onSenhaEmAtendimento(senhaId);
      }
    }, { allowSignalWrites: true });

    effect(() => {
      const event = this.signalRService.novaSenhaEvent();
      if (event) {
        this.loadFilaStatus();
      }
    }, { allowSignalWrites: true });
  }

  async ngOnInit(): Promise<void> {
    this.route.params.subscribe(params => {
      this.clinicId.set(params['clinicId']);
      this.filaId.set(params['filaId']);
      this.initializePainel();
    });

    // Update clock every second
    this.clockInterval = setInterval(() => {
      this.currentTime.set(new Date());
    }, 1000);

    // Refresh queue status every 30 seconds as fallback
    this.refreshInterval = setInterval(() => {
      this.loadFilaStatus();
    }, 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
    if (this.clockInterval) {
      clearInterval(this.clockInterval);
    }
    if (this.chamadaTimeout) {
      clearTimeout(this.chamadaTimeout);
    }
    
    // Disconnect from SignalR
    this.signalRService.leaveFila(this.filaId());
    this.signalRService.disconnect();
  }

  async initializePainel(): Promise<void> {
    try {
      // Connect to SignalR
      await this.signalRService.connect();
      await this.signalRService.joinFila(this.filaId());
      this.isConnected.set(true);

      // Load initial data
      await this.loadFilaStatus();
      await this.loadUltimasChamadas();
      await this.loadTempoMedio();
    } catch (error) {
      console.error('Error initializing painel', error);
      this.isConnected.set(false);
    }
  }

  async loadFilaStatus(): Promise<void> {
    try {
      const senhas = await this.filaService.getSenhasAguardando(this.filaId()).toPromise();
      this.senhasAguardando.set(senhas || []);
    } catch (error) {
      console.error('Error loading fila status', error);
    }
  }

  async loadUltimasChamadas(): Promise<void> {
    try {
      const chamadas = await this.filaService.getUltimasChamadas(this.filaId(), 5).toPromise();
      const events: ChamarSenhaEvent[] = (chamadas || []).map(senha => ({
        senha: senha.numeroSenha,
        paciente: senha.nomePaciente,
        consultorio: senha.numeroConsultorio || '',
        senhaId: senha.id
      }));
      this.ultimasChamadas.set(events);
    } catch (error) {
      console.error('Error loading ultimas chamadas', error);
    }
  }

  async loadTempoMedio(): Promise<void> {
    try {
      const tempo = await this.filaService.getTempoMedioEspera(this.filaId()).toPromise();
      this.tempoMedioEspera.set(tempo || 0);
    } catch (error) {
      console.error('Error loading tempo medio', error);
    }
  }

  onChamarSenha(event: ChamarSenhaEvent): void {
    console.log('Chamando senha:', event);
    
    // Clear previous timeout
    if (this.chamadaTimeout) {
      clearTimeout(this.chamadaTimeout);
    }

    // Set current call
    this.chamadaAtual.set(event);

    // Play notification sound
    this.reproduzirSom();

    // Play text-to-speech
    this.reproduzirVoz(event);

    // Move to history after 30 seconds
    this.chamadaTimeout = setTimeout(() => {
      const ultimas = [event, ...this.ultimasChamadas()];
      if (ultimas.length > 5) {
        ultimas.pop();
      }
      this.ultimasChamadas.set(ultimas);
      this.chamadaAtual.set(null);
    }, 30000);

    // Reload queue status
    this.loadFilaStatus();
  }

  onSenhaEmAtendimento(senhaId: string): void {
    // Remove from waiting list
    const aguardando = this.senhasAguardando().filter(s => s.id !== senhaId);
    this.senhasAguardando.set(aguardando);
  }

  reproduzirVoz(event: ChamarSenhaEvent): void {
    if (!('speechSynthesis' in window)) {
      console.warn('Text-to-Speech not supported');
      return;
    }

    try {
      // Cancel any ongoing speech
      speechSynthesis.cancel();

      const text = `Senha ${event.senha}, ${event.paciente}, comparecer ao consultório ${event.consultorio}`;
      const utterance = new SpeechSynthesisUtterance(text);
      utterance.lang = 'pt-BR';
      utterance.rate = 0.9;
      utterance.pitch = 1.0;
      utterance.volume = 1.0;

      speechSynthesis.speak(utterance);
    } catch (error) {
      console.error('Error playing text-to-speech', error);
    }
  }

  reproduzirSom(): void {
    try {
      // Use Web Audio API to generate a beep sound
      const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();

      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);

      oscillator.frequency.value = 800;
      oscillator.type = 'sine';

      gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

      oscillator.start(audioContext.currentTime);
      oscillator.stop(audioContext.currentTime + 0.5);
    } catch (error) {
      console.error('Error playing sound', error);
    }
  }

  isPrioritaria(senha: SenhaFila): boolean {
    return senha.prioridade > PrioridadeAtendimento.Normal;
  }

  getSenhaClass(senha: SenhaFila): string {
    return this.isPrioritaria(senha) ? 'prioritaria' : '';
  }
}
