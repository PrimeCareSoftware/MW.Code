import { Component, OnInit, OnDestroy, signal, computed, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged, firstValueFrom } from 'rxjs';
import { HubConnectionState } from '@microsoft/signalr';
import { ChatService } from './services/chat.service';
import { ChatHubService } from './services/chat-hub.service';
import { Auth } from '../../services/auth';
import { Navbar } from '../../shared/navbar/navbar';
import { 
  Conversation, 
  ChatMessage, 
  UserPresence,
  TypingIndicator 
} from './models/chat.models';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('messagesContainer', { read: ElementRef }) messagesContainer?: ElementRef;
  
  private destroy$ = new Subject<void>();
  private typingSubject = new Subject<string>();
  private typingTimeouts = new Map<string, number>();
  
  // Signals for reactive state
  conversations = signal<Conversation[]>([]);
  selectedConversation = signal<Conversation | null>(null);
  messages = signal<ChatMessage[]>([]);
  users = signal<UserPresence[]>([]);
  isLoadingConversations = signal(true);
  isLoadingMessages = signal(false);
  isConnected = signal(false);
  currentUserId = signal<string>('');
  
  // Input state
  messageInput = '';
  searchQuery = '';
  showUserList = false;
  typingUsers = signal<Set<string>>(new Set());
  
  // Computed values
  sortedConversations = computed(() => {
    return [...this.conversations()].sort((a, b) => {
      const aTime = a.lastMessageAt?.getTime() ?? a.createdAt.getTime();
      const bTime = b.lastMessageAt?.getTime() ?? b.createdAt.getTime();
      return bTime - aTime;
    });
  });
  
  filteredUsers = computed(() => {
    const query = this.searchQuery.toLowerCase();
    return this.users().filter(user => 
      user.fullName.toLowerCase().includes(query) ||
      user.userName.toLowerCase().includes(query)
    );
  });
  
  unreadTotal = computed(() => {
    return this.conversations().reduce((sum, conv) => sum + conv.unreadCount, 0);
  });

  constructor(
    private chatService: ChatService,
    private chatHubService: ChatHubService,
    private authService: Auth,
    private router: Router
  ) {
    // Setup typing debounce
    this.typingSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(conversationId => {
      if (conversationId) {
        this.chatHubService.sendTypingIndicator(conversationId);
      }
    });
  }

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (!user) {
      this.router.navigate(['/login']);
      return;
    }
    
    // Extract userId from JWT token
    const userId = this.extractUserIdFromToken();
    if (userId) {
      this.currentUserId.set(userId);
    }
    this.initializeChat();
  }

  private extractUserIdFromToken(): string | null {
    const token = this.authService.getToken();
    if (!token) return null;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // Try different claim names that might contain the userId
      return payload.sub || payload.userId || payload.nameid || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
    } catch (error) {
      console.error('Error parsing JWT token:', error);
      return null;
    }
  }

  ngOnDestroy(): void {
    // Clear all typing timeouts
    this.typingTimeouts.forEach(timeoutId => clearTimeout(timeoutId));
    this.typingTimeouts.clear();
    
    this.destroy$.next();
    this.destroy$.complete();
    this.chatHubService.stopConnection();
  }

  ngAfterViewInit(): void {
    // View is initialized - we can now safely scroll
  }

  private async initializeChat(): Promise<void> {
    try {
      // Start SignalR connection
      const token = this.authService.getToken();
      if (token) {
        await this.chatHubService.startConnection(token);
        this.isConnected.set(true);
      }

      // Setup SignalR event handlers
      this.setupSignalRHandlers();

      // Load initial data
      await this.loadConversations();
      await this.loadUsers();
      
    } catch (error) {
      console.error('Error initializing chat:', error);
      this.isConnected.set(false);
    }
  }

  private setupSignalRHandlers(): void {
    // New message received
    this.chatHubService.messageReceived$
      .pipe(takeUntil(this.destroy$))
      .subscribe(message => {
        this.onMessageReceived(message);
      });

    // Message sent confirmation
    this.chatHubService.messageSent$
      .pipe(takeUntil(this.destroy$))
      .subscribe(message => {
        this.onMessageSent(message);
      });

    // Typing indicator
    this.chatHubService.userTyping$
      .pipe(takeUntil(this.destroy$))
      .subscribe(indicator => {
        this.onUserTyping(indicator);
      });

    // Presence changed
    this.chatHubService.presenceChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe(event => {
        this.onPresenceChanged(event);
      });

    // Message read
    this.chatHubService.messageRead$
      .pipe(takeUntil(this.destroy$))
      .subscribe(event => {
        this.onMessageRead(event);
      });

    // Connection state
    this.chatHubService.connectionState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        this.isConnected.set(state === HubConnectionState.Connected);
      });
  }

  private async loadConversations(): Promise<void> {
    try {
      this.isLoadingConversations.set(true);
      const response = await firstValueFrom(this.chatService.getConversations());
      if (response) {
        this.conversations.set(response.conversations);
      }
    } catch (error) {
      console.error('Error loading conversations:', error);
    } finally {
      this.isLoadingConversations.set(false);
    }
  }

  private async loadUsers(): Promise<void> {
    try {
      const users = await firstValueFrom(this.chatService.getClinicUsers());
      if (users) {
        // Filter out current user
        const filteredUsers = users.filter(u => u.userId !== this.currentUserId());
        this.users.set(filteredUsers);
      }
    } catch (error) {
      console.error('Error loading users:', error);
    }
  }

  async selectConversation(conversation: Conversation): Promise<void> {
    if (this.selectedConversation()?.id === conversation.id) {
      return;
    }

    this.selectedConversation.set(conversation);
    this.messageInput = '';
    await this.loadMessages(conversation.id);
    
    // Join conversation in SignalR
    await this.chatHubService.joinConversation(conversation.id);
    
    // Mark as read if there are unread messages
    if (conversation.unreadCount > 0 && conversation.lastMessage) {
      await this.chatHubService.markAsRead(conversation.id, conversation.lastMessage.id);
    }
  }

  private async loadMessages(conversationId: string): Promise<void> {
    try {
      this.isLoadingMessages.set(true);
      const response = await firstValueFrom(this.chatService.getMessages(conversationId));
      if (response) {
        this.messages.set(response.messages);
        // Scroll to bottom after messages load
        setTimeout(() => this.scrollToBottom(), 100);
      }
    } catch (error) {
      console.error('Error loading messages:', error);
    } finally {
      this.isLoadingMessages.set(false);
    }
  }

  async sendMessage(): Promise<void> {
    const content = this.messageInput.trim();
    if (!content || !this.selectedConversation()) {
      return;
    }

    const conversationId = this.selectedConversation()!.id;
    
    try {
      await this.chatHubService.sendMessage(conversationId, content);
      this.messageInput = '';
    } catch (error) {
      console.error('Error sending message:', error);
    }
  }

  onTyping(): void {
    const conversationId = this.selectedConversation()?.id;
    if (conversationId) {
      this.typingSubject.next(conversationId);
    }
  }

  async startConversation(user: UserPresence): Promise<void> {
    try {
      const conversation = await firstValueFrom(this.chatService.createDirectConversation(user.userId));
      if (conversation) {
        // Add to conversations list if not already there
        const exists = this.conversations().find(c => c.id === conversation.id);
        if (!exists) {
          this.conversations.update(convs => [...convs, conversation]);
        }
        // Select the conversation
        await this.selectConversation(conversation);
        this.showUserList = false;
      }
    } catch (error) {
      console.error('Error starting conversation:', error);
    }
  }

  toggleUserList(): void {
    this.showUserList = !this.showUserList;
  }

  private onMessageReceived(message: ChatMessage): void {
    const selected = this.selectedConversation();
    
    // Add to messages if it's for the selected conversation
    if (selected && message.conversationId === selected.id) {
      this.messages.update(msgs => [...msgs, message]);
      this.scrollToBottom();
      
      // Mark as read
      this.chatHubService.markAsRead(message.conversationId, message.id);
    }
    
    // Update conversation list
    this.updateConversationWithMessage(message);
  }

  private onMessageSent(message: ChatMessage): void {
    const selected = this.selectedConversation();
    
    // Add to messages if it's for the selected conversation
    if (selected && message.conversationId === selected.id) {
      // Check if message already exists (avoid duplicates)
      const exists = this.messages().find(m => m.id === message.id);
      if (!exists) {
        this.messages.update(msgs => [...msgs, message]);
        this.scrollToBottom();
      }
    }
    
    // Update conversation list
    this.updateConversationWithMessage(message);
  }

  private updateConversationWithMessage(message: ChatMessage): void {
    this.conversations.update(convs => 
      convs.map(conv => {
        if (conv.id === message.conversationId) {
          return {
            ...conv,
            lastMessage: message,
            lastMessageAt: message.sentAt,
            unreadCount: message.senderId === this.currentUserId() ? 0 : conv.unreadCount + 1
          };
        }
        return conv;
      })
    );
  }

  private onUserTyping(indicator: TypingIndicator): void {
    const userId = indicator.userId;
    
    // Clear existing timeout for this user if it exists
    const existingTimeout = this.typingTimeouts.get(userId);
    if (existingTimeout) {
      clearTimeout(existingTimeout);
    }
    
    // Show typing indicator
    this.typingUsers.update(users => new Set(users).add(userId));
    
    // Set new timeout to hide after 3 seconds
    const timeoutId = window.setTimeout(() => {
      this.typingUsers.update(users => {
        const newSet = new Set(users);
        newSet.delete(userId);
        return newSet;
      });
      this.typingTimeouts.delete(userId);
    }, 3000);
    
    this.typingTimeouts.set(userId, timeoutId);
  }

  private onPresenceChanged(event: any): void {
    // Update user presence
    this.users.update(users => 
      users.map(user => {
        if (user.userId === event.userId) {
          return {
            ...user,
            status: event.status,
            isOnline: event.isOnline,
            statusMessage: event.statusMessage
          };
        }
        return user;
      })
    );
  }

  private onMessageRead(event: any): void {
    // Update message read status
    this.messages.update(msgs =>
      msgs.map(msg => {
        if (msg.id === event.messageId) {
          return {
            ...msg,
            readBy: [...msg.readBy, event.readBy]
          };
        }
        return msg;
      })
    );
  }

  private scrollToBottom(): void {
    if (this.messagesContainer?.nativeElement) {
      const element = this.messagesContainer.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }

  getConversationTitle(conversation: Conversation): string {
    if (conversation.type === 'Direct') {
      const otherUser = conversation.participants.find(
        p => p.userId !== this.currentUserId()
      );
      return otherUser?.fullName || 'Conversa';
    }
    return conversation.title;
  }

  getLastMessagePreview(conversation: Conversation): string {
    if (!conversation.lastMessage) {
      return 'Nenhuma mensagem ainda';
    }
    return conversation.lastMessage.content.substring(0, 50) + 
      (conversation.lastMessage.content.length > 50 ? '...' : '');
  }

  formatTime(date: Date): string {
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (minutes < 1) return 'Agora';
    if (minutes < 60) return `${minutes}min`;
    if (hours < 24) return `${hours}h`;
    if (days < 7) return `${days}d`;
    
    return date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
  }

  getPresenceClass(user: UserPresence): string {
    switch (user.status) {
      case 'Online': return 'online';
      case 'Away': return 'away';
      case 'Busy': return 'busy';
      default: return 'offline';
    }
  }

  isOwnMessage(message: ChatMessage): boolean {
    return message.senderId === this.currentUserId();
  }
}
