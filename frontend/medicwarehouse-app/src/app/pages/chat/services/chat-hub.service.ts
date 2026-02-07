import { Injectable, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject, fromEvent } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ChatMessage, TypingIndicator, PresenceChangeEvent, MessageReadEvent } from '../models/chat.models';

@Injectable({
  providedIn: 'root'
})
export class ChatHubService {
  private hubConnection: HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 10;
  private reconnectDelay = 1000; // Start with 1 second

  // Observables for real-time events
  private messageReceivedSubject = new Subject<ChatMessage>();
  private messageSentSubject = new Subject<ChatMessage>();
  private userTypingSubject = new Subject<TypingIndicator>();
  private presenceChangedSubject = new Subject<PresenceChangeEvent>();
  private messageReadSubject = new Subject<MessageReadEvent>();
  private connectionStateSubject = new BehaviorSubject<HubConnectionState>(HubConnectionState.Disconnected);

  public messageReceived$ = this.messageReceivedSubject.asObservable();
  public messageSent$ = this.messageSentSubject.asObservable();
  public userTyping$ = this.userTypingSubject.asObservable();
  public presenceChanged$ = this.presenceChangedSubject.asObservable();
  public messageRead$ = this.messageReadSubject.asObservable();
  public connectionState$ = this.connectionStateSubject.asObservable();

  async startConnection(accessToken: string): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      console.log('ChatHub already connected');
      return;
    }

    try {
      const hubUrl = environment.apiUrl.replace('/api', '/hubs/chat');
      
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => accessToken,
          withCredentials: false
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // Exponential backoff with max delay of 30 seconds
            const delay = Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
            console.log(`ChatHub reconnecting in ${delay}ms (attempt ${retryContext.previousRetryCount + 1})`);
            return delay;
          }
        })
        .configureLogging(LogLevel.Information)
        .build();

      // Register event handlers
      this.registerEventHandlers();

      // Connection lifecycle handlers
      this.hubConnection.onreconnecting((error) => {
        console.warn('ChatHub reconnecting...', error);
        this.connectionStateSubject.next(HubConnectionState.Reconnecting);
      });

      this.hubConnection.onreconnected((connectionId) => {
        console.log('ChatHub reconnected:', connectionId);
        this.reconnectAttempts = 0;
        this.connectionStateSubject.next(HubConnectionState.Connected);
      });

      this.hubConnection.onclose((error) => {
        console.error('ChatHub connection closed', error);
        this.connectionStateSubject.next(HubConnectionState.Disconnected);
        
        // Try to reconnect if not manually disconnected
        if (error && this.reconnectAttempts < this.maxReconnectAttempts) {
          this.scheduleReconnect(accessToken);
        }
      });

      // Start connection
      await this.hubConnection.start();
      this.reconnectAttempts = 0;
      this.connectionStateSubject.next(HubConnectionState.Connected);
      console.log('ChatHub connected successfully');
    } catch (error) {
      console.error('Error starting ChatHub connection:', error);
      this.connectionStateSubject.next(HubConnectionState.Disconnected);
      
      // Schedule reconnect
      if (this.reconnectAttempts < this.maxReconnectAttempts) {
        this.scheduleReconnect(accessToken);
      }
      
      throw error;
    }
  }

  private scheduleReconnect(accessToken: string): void {
    this.reconnectAttempts++;
    const delay = Math.min(this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1), 30000);
    
    console.log(`Scheduling ChatHub reconnect attempt ${this.reconnectAttempts}/${this.maxReconnectAttempts} in ${delay}ms`);
    
    setTimeout(() => {
      if (this.hubConnection?.state !== HubConnectionState.Connected) {
        this.startConnection(accessToken).catch(err => 
          console.error('Reconnect attempt failed:', err)
        );
      }
    }, delay);
  }

  private registerEventHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      console.log('Message received:', message);
      this.messageReceivedSubject.next(message);
    });

    this.hubConnection.on('MessageSent', (message: ChatMessage) => {
      console.log('Message sent confirmation:', message);
      this.messageSentSubject.next(message);
    });

    this.hubConnection.on('UserTyping', (indicator: TypingIndicator) => {
      this.userTypingSubject.next(indicator);
    });

    this.hubConnection.on('UserPresenceChanged', (event: PresenceChangeEvent) => {
      console.log('Presence changed:', event);
      this.presenceChangedSubject.next(event);
    });

    this.hubConnection.on('MessageRead', (event: MessageReadEvent) => {
      this.messageReadSubject.next(event);
    });

    this.hubConnection.on('MessageError', (error: any) => {
      console.error('Message error from server:', error);
    });
  }

  async sendMessage(conversationId: string, content: string, replyToMessageId?: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      throw new Error('ChatHub not connected');
    }

    try {
      await this.hubConnection.invoke('SendMessage', {
        conversationId,
        content,
        replyToMessageId
      });
    } catch (error) {
      console.error('Error sending message:', error);
      throw error;
    }
  }

  async sendTypingIndicator(conversationId: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('SendTypingIndicator', conversationId);
    } catch (error) {
      console.error('Error sending typing indicator:', error);
    }
  }

  async markAsRead(conversationId: string, messageId: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('MarkAsRead', {
        conversationId,
        messageId
      });
    } catch (error) {
      console.error('Error marking message as read:', error);
    }
  }

  async updateStatus(status: string, statusMessage?: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('UpdateStatus', status, statusMessage);
    } catch (error) {
      console.error('Error updating status:', error);
    }
  }

  async joinConversation(conversationId: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('JoinConversation', conversationId);
    } catch (error) {
      console.error('Error joining conversation:', error);
    }
  }

  async leaveConversation(conversationId: string): Promise<void> {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    try {
      await this.hubConnection.invoke('LeaveConversation', conversationId);
    } catch (error) {
      console.error('Error leaving conversation:', error);
    }
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      try {
        await this.hubConnection.stop();
        this.connectionStateSubject.next(HubConnectionState.Disconnected);
        console.log('ChatHub connection stopped');
      } catch (error) {
        console.error('Error stopping ChatHub connection:', error);
      }
    }
  }

  isConnected(): boolean {
    return this.hubConnection?.state === HubConnectionState.Connected;
  }

  getConnectionState(): HubConnectionState {
    return this.hubConnection?.state ?? HubConnectionState.Disconnected;
  }
}
