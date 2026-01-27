import { Injectable, signal, effect } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { ChamarSenhaEvent, NovaSenhaEvent } from '../models/fila-espera.model';

@Injectable({
  providedIn: 'root'
})
export class FilaSignalRService {
  private hubConnection?: signalR.HubConnection;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 5000;
  
  // Constants for retry configuration
  private readonly RETRY_DELAY_THRESHOLD_MS = 60000; // 60 seconds
  private readonly MAX_RETRY_DELAY_MS = 10000; // 10 seconds

  // Signals for state management
  public isConnected = signal<boolean>(false);
  public connectionError = signal<string | null>(null);
  public currentFilaId = signal<string | null>(null);

  // Event signals
  public novaSenhaEvent = signal<NovaSenhaEvent | null>(null);
  public chamarSenhaEvent = signal<ChamarSenhaEvent | null>(null);
  public senhaEmAtendimentoEvent = signal<string | null>(null);
  public senhaFinalizadaEvent = signal<string | null>(null);

  constructor() {
    // Setup auto-reconnection logic
    effect(() => {
      if (!this.isConnected() && this.reconnectAttempts < this.maxReconnectAttempts) {
        setTimeout(() => this.reconnect(), this.reconnectDelay);
      }
    }, { allowSignalWrites: true });
  }

  /**
   * Initialize SignalR connection
   */
  async connect(token?: string): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR: Already connected');
      return;
    }

    try {
      const hubUrl = environment.apiUrl.replace('/api', '/hubs/fila');
      
      const builder = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => token || '',
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext: signalR.RetryContext) => {
            if (retryContext.elapsedMilliseconds < this.RETRY_DELAY_THRESHOLD_MS) {
              return Math.random() * this.MAX_RETRY_DELAY_MS;
            } else {
              return null;
            }
          }
        })
        .configureLogging(signalR.LogLevel.Information);

      this.hubConnection = builder.build();

      this.setupEventHandlers();
      this.setupConnectionHandlers();

      await this.hubConnection.start();
      this.isConnected.set(true);
      this.connectionError.set(null);
      this.reconnectAttempts = 0;
      
      console.log('SignalR: Connected successfully');
    } catch (error) {
      console.error('SignalR: Connection failed', error);
      this.isConnected.set(false);
      this.connectionError.set(error instanceof Error ? error.message : 'Connection failed');
      this.reconnectAttempts++;
      throw error;
    }
  }

  /**
   * Setup event handlers for SignalR messages
   */
  private setupEventHandlers(): void {
    if (!this.hubConnection) return;

    // Nova senha gerada
    this.hubConnection.on('NovaSenha', (event: NovaSenhaEvent) => {
      console.log('SignalR: NovaSenha', event);
      this.novaSenhaEvent.set(event);
    });

    // Senha sendo chamada
    this.hubConnection.on('ChamarSenha', (event: ChamarSenhaEvent) => {
      console.log('SignalR: ChamarSenha', event);
      this.chamarSenhaEvent.set(event);
    });

    // Senha em atendimento
    this.hubConnection.on('SenhaEmAtendimento', (senhaId: string) => {
      console.log('SignalR: SenhaEmAtendimento', senhaId);
      this.senhaEmAtendimentoEvent.set(senhaId);
    });

    // Senha finalizada
    this.hubConnection.on('SenhaFinalizada', (senhaId: string) => {
      console.log('SignalR: SenhaFinalizada', senhaId);
      this.senhaFinalizadaEvent.set(senhaId);
    });
  }

  /**
   * Setup connection state handlers
   */
  private setupConnectionHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.onreconnecting((error?: Error) => {
      console.warn('SignalR: Reconnecting...', error);
      this.isConnected.set(false);
      this.connectionError.set('Reconectando...');
    });

    this.hubConnection.onreconnected((connectionId?: string) => {
      console.log('SignalR: Reconnected', connectionId);
      this.isConnected.set(true);
      this.connectionError.set(null);
      this.reconnectAttempts = 0;
      
      // Rejoin fila if needed
      const filaId = this.currentFilaId();
      if (filaId) {
        this.joinFila(filaId);
      }
    });

    this.hubConnection.onclose((error?: Error) => {
      console.error('SignalR: Connection closed', error);
      this.isConnected.set(false);
      this.connectionError.set(error?.message || 'Connection closed');
      this.reconnectAttempts++;
    });
  }

  /**
   * Join a specific queue group
   */
  async joinFila(filaId: string): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('SignalR: Not connected');
    }

    try {
      await this.hubConnection.invoke('JoinFila', filaId);
      this.currentFilaId.set(filaId);
      console.log(`SignalR: Joined fila ${filaId}`);
    } catch (error) {
      console.error('SignalR: Failed to join fila', error);
      throw error;
    }
  }

  /**
   * Leave the current queue group
   */
  async leaveFila(filaId: string): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('LeaveFila', filaId);
      this.currentFilaId.set(null);
      console.log(`SignalR: Left fila ${filaId}`);
    } catch (error) {
      console.error('SignalR: Failed to leave fila', error);
    }
  }

  /**
   * Manual reconnection attempt
   */
  private async reconnect(): Promise<void> {
    if (this.isConnected()) return;
    
    try {
      await this.connect();
    } catch (error) {
      console.error('SignalR: Reconnection failed', error);
    }
  }

  /**
   * Disconnect from SignalR
   */
  async disconnect(): Promise<void> {
    if (this.hubConnection) {
      try {
        await this.hubConnection.stop();
        this.isConnected.set(false);
        this.currentFilaId.set(null);
        console.log('SignalR: Disconnected');
      } catch (error) {
        console.error('SignalR: Failed to disconnect', error);
      }
    }
  }

  /**
   * Get connection state
   */
  getConnectionState(): signalR.HubConnectionState {
    return this.hubConnection?.state || signalR.HubConnectionState.Disconnected;
  }
}
