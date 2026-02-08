import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { 
  Conversation, 
  ChatMessage, 
  UserPresence, 
  CreateDirectConversationDto,
  ChatParticipant 
} from '../models/chat.models';

export interface ConversationListResponse {
  conversations: Conversation[];
  totalCount: number;
}

export interface MessageListResponse {
  messages: ChatMessage[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/chat`;

  /**
   * Obter todas as conversas do usuário
   */
  getConversations(): Observable<ConversationListResponse> {
    return this.http.get<ConversationListResponse>(`${this.apiUrl}/conversations`).pipe(
      map(response => ({
        ...response,
        conversations: response.conversations.map(this.parseConversationDates)
      }))
    );
  }

  /**
   * Criar uma conversa direta com outro usuário
   */
  createDirectConversation(otherUserId: string): Observable<Conversation> {
    const dto: CreateDirectConversationDto = { otherUserId };
    return this.http.post<Conversation>(`${this.apiUrl}/conversations/direct`, dto).pipe(
      map(this.parseConversationDates)
    );
  }

  /**
   * Obter mensagens de uma conversa (paginado)
   */
  getMessages(conversationId: string, page: number = 1, pageSize: number = 50): Observable<MessageListResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<MessageListResponse>(
      `${this.apiUrl}/conversations/${conversationId}/messages`,
      { params }
    ).pipe(
      map(response => ({
        ...response,
        messages: response.messages.map(this.parseMessageDates)
      }))
    );
  }

  /**
   * Obter contador de mensagens não lidas em uma conversa
   */
  getUnreadCount(conversationId: string): Observable<{ conversationId: string; unreadCount: number }> {
    return this.http.get<{ conversationId: string; unreadCount: number }>(
      `${this.apiUrl}/conversations/${conversationId}/unread-count`
    );
  }

  /**
   * Buscar mensagens em uma conversa
   */
  searchMessages(conversationId: string, query: string): Observable<MessageListResponse> {
    const params = new HttpParams().set('query', query);
    
    return this.http.get<MessageListResponse>(
      `${this.apiUrl}/conversations/${conversationId}/search`,
      { params }
    ).pipe(
      map(response => ({
        ...response,
        messages: response.messages.map(this.parseMessageDates)
      }))
    );
  }

  /**
   * Obter status de presença de todos os usuários da clínica
   */
  getAllPresence(): Observable<UserPresence[]> {
    return this.http.get<UserPresence[]>(`${this.apiUrl}/presence`).pipe(
      map(presences => presences.map(this.parsePresenceDates))
    );
  }

  /**
   * Obter lista de usuários da clínica (para iniciar conversas)
   */
  getClinicUsers(): Observable<UserPresence[]> {
    return this.http.get<UserPresence[]>(`${this.apiUrl}/users`).pipe(
      map(users => users.map(this.parsePresenceDates))
    );
  }

  /**
   * Editar uma mensagem
   */
  editMessage(messageId: string, content: string): Observable<ChatMessage> {
    return this.http.put<ChatMessage>(
      `${this.apiUrl}/messages/${messageId}`,
      { content }
    ).pipe(
      map(this.parseMessageDates)
    );
  }

  /**
   * Deletar uma mensagem
   */
  deleteMessage(messageId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/messages/${messageId}`);
  }

  /**
   * Obter participantes de uma conversa
   */
  getParticipants(conversationId: string): Observable<ChatParticipant[]> {
    return this.http.get<ChatParticipant[]>(
      `${this.apiUrl}/conversations/${conversationId}/participants`
    ).pipe(
      map(participants => participants.map(p => ({
        ...p,
        joinedAt: new Date(p.joinedAt)
      })))
    );
  }

  // Helper methods to parse date strings
  private parseConversationDates(conversation: any): Conversation {
    return {
      ...conversation,
      createdAt: new Date(conversation.createdAt),
      lastMessageAt: conversation.lastMessageAt ? new Date(conversation.lastMessageAt) : undefined,
      lastMessage: conversation.lastMessage ? this.parseMessageDates(conversation.lastMessage) : undefined,
      participants: conversation.participants?.map((p: any) => ({
        ...p,
        joinedAt: new Date(p.joinedAt)
      })) || []
    };
  }

  private parseMessageDates(message: any): ChatMessage {
    return {
      ...message,
      sentAt: new Date(message.sentAt),
      editedAt: message.editedAt ? new Date(message.editedAt) : undefined
    };
  }

  private parsePresenceDates(presence: any): UserPresence {
    return {
      ...presence,
      lastSeenAt: new Date(presence.lastSeenAt)
    };
  }
}
