import { Component, signal, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../pages/chat/services/chat.service';
import { ChatHubService } from '../../pages/chat/services/chat-hub.service';
import { Conversation, ChatMessage } from '../../pages/chat/models/chat.models';
import { firstValueFrom, Subscription } from 'rxjs';
import { formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';

interface SimpleUser {
  userId: string;
  fullName: string;
  userName: string;
}

@Component({
  selector: 'app-chat-fab',
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-fab.html',
  styleUrl: './chat-fab.scss'
})
export class ChatFabComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('messagesContainer') messagesContainer?: ElementRef<HTMLDivElement>;

  showWidget = signal<boolean>(false);
  isMinimized = signal<boolean>(false);
  
  // Connection state
  isConnected = signal<boolean>(false);
  
  // Data
  conversations = signal<Conversation[]>([]);
  selectedConversation = signal<Conversation | null>(null);
  messages = signal<ChatMessage[]>([]);
  users = signal<SimpleUser[]>([]);
  
  // UI states
  isLoadingConversations = signal<boolean>(false);
  isLoadingMessages = signal<boolean>(false);
  showUserList = false;
  searchQuery = '';
  messageInput = '';
  
  // Typing state
  typingUsers = signal<Set<string>>(new Set());
  private typingTimeout?: ReturnType<typeof setTimeout>;
  
  // Subscriptions
  private subscriptions: Subscription[] = [];
  
  // Current user ID
  private currentUserId: string = '';

  constructor(
    private chatService: ChatService,
    private chatHubService: ChatHubService
  ) {}

  async ngOnInit(): Promise<void> {
    // Extract userId from JWT token
    this.currentUserId = this.extractUserIdFromToken() || '';

    // Initialize SignalR connection
    await this.initializeHub();
    
    // Load initial data
    await this.loadConversations();
    await this.loadUsers();
  }

  private extractUserIdFromToken(): string | null {
    const token = localStorage.getItem('auth_token');
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

  ngAfterViewInit(): void {
    // Scroll to bottom when messages change
    this.scrollToBottom();
  }

  ngOnDestroy(): void {
    // Clean up typing timeout
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }
    
    // Unsubscribe from all subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private async initializeHub(): Promise<void> {
    try {
      // Get access token
      const token = localStorage.getItem('auth_token') || '';
      
      await this.chatHubService.startConnection(token);
      this.isConnected.set(this.chatHubService.isConnected());

      // Subscribe to connection state
      const connectionSub = this.chatHubService.connectionState$.subscribe(state => {
        this.isConnected.set(state === 'Connected');
      });
      this.subscriptions.push(connectionSub);

      // Subscribe to new messages
      const messagesSub = this.chatHubService.messageReceived$.subscribe(message => {
        this.onMessageReceived(message);
      });
      this.subscriptions.push(messagesSub);

      // Subscribe to typing indicators
      const typingSub = this.chatHubService.userTyping$.subscribe(indicator => {
        if (indicator.conversationId === this.selectedConversation()?.id) {
          const users = new Set(this.typingUsers());
          users.add(indicator.userId);
          this.typingUsers.set(users);

          // Clear after 3 seconds
          setTimeout(() => {
            const users = new Set(this.typingUsers());
            users.delete(indicator.userId);
            this.typingUsers.set(users);
          }, 3000);
        }
      });
      this.subscriptions.push(typingSub);

      // Subscribe to message read events
      const readSub = this.chatHubService.messageRead$.subscribe(event => {
        const msgs = this.messages();
        const message = msgs.find(m => m.id === event.messageId);
        if (message && !message.readBy.includes(event.readBy)) {
          message.readBy.push(event.readBy);
          this.messages.set([...msgs]);
        }
      });
      this.subscriptions.push(readSub);
    } catch (error) {
      console.error('Failed to initialize chat hub:', error);
    }
  }

  private async loadConversations(): Promise<void> {
    this.isLoadingConversations.set(true);
    try {
      const response = await firstValueFrom(this.chatService.getConversations());
      this.conversations.set(response.conversations || []);
    } catch (error) {
      console.error('Failed to load conversations:', error);
    } finally {
      this.isLoadingConversations.set(false);
    }
  }

  private async loadUsers(): Promise<void> {
    try {
      const users = await firstValueFrom(this.chatService.getClinicUsers());
      // Map UserPresence to SimpleUser
      this.users.set(users.map(u => ({
        userId: u.userId,
        fullName: u.fullName,
        userName: u.userName
      })));
    } catch (error) {
      console.error('Failed to load users:', error);
    }
  }

  async selectConversation(conversation: Conversation): Promise<void> {
    this.selectedConversation.set(conversation);
    this.isLoadingMessages.set(true);
    
    try {
      const response = await firstValueFrom(
        this.chatService.getMessages(conversation.id, 1, 50)
      );
      this.messages.set(response.messages || []);
      
      // Join conversation via SignalR
      await this.chatHubService.joinConversation(conversation.id);
      
      // Mark messages as read
      const unreadMessages = this.messages().filter(
        m => m.senderId !== this.currentUserId && !m.readBy.includes(this.currentUserId)
      );
      
      for (const message of unreadMessages) {
        try {
          await this.chatHubService.markAsRead(conversation.id, message.id);
        } catch (error) {
          console.error('Failed to mark message as read:', error);
        }
      }
      
      // Update unread count locally
      conversation.unreadCount = 0;
      this.conversations.set([...this.conversations()]);
      
      setTimeout(() => this.scrollToBottom(), 100);
    } catch (error) {
      console.error('Failed to load messages:', error);
    } finally {
      this.isLoadingMessages.set(false);
    }
  }

  async sendMessage(): Promise<void> {
    const content = this.messageInput.trim();
    if (!content || !this.selectedConversation()) return;

    const conversation = this.selectedConversation()!;
    this.messageInput = '';

    try {
      await this.chatHubService.sendMessage(conversation.id, content, undefined);
      setTimeout(() => this.scrollToBottom(), 100);
    } catch (error) {
      console.error('Failed to send message:', error);
      this.messageInput = content; // Restore message on error
    }
  }

  onTyping(): void {
    if (!this.selectedConversation()) return;

    const conversation = this.selectedConversation()!;
    this.chatHubService.sendTypingIndicator(conversation.id);

    // Clear existing timeout
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }
  }

  private onMessageReceived(message: ChatMessage): void {
    // Add to messages if it's for the current conversation
    if (message.conversationId === this.selectedConversation()?.id) {
      this.messages.set([...this.messages(), message]);
      setTimeout(() => this.scrollToBottom(), 100);

      // Mark as read if widget is open and visible
      if (this.showWidget() && !this.isMinimized()) {
        this.chatHubService.markAsRead(message.conversationId, message.id);
      }
    }

    // Update conversation list
    const convs = this.conversations();
    const conv = convs.find(c => c.id === message.conversationId);
    if (conv) {
      conv.lastMessageAt = message.sentAt;
      conv.lastMessage = message;
      if (message.senderId !== this.currentUserId) {
        conv.unreadCount = (conv.unreadCount || 0) + 1;
      }
      this.conversations.set([...convs]);
    } else {
      // New conversation, reload list
      this.loadConversations();
    }
  }

  toggleWidget(): void {
    this.showWidget.set(!this.showWidget());
    if (this.showWidget()) {
      this.isMinimized.set(false);
    }
  }

  minimizeWidget(): void {
    this.isMinimized.set(true);
  }

  maximizeWidget(): void {
    this.isMinimized.set(false);
  }

  closeWidget(): void {
    this.showWidget.set(false);
    this.selectedConversation.set(null);
  }

  toggleUserList(): void {
    this.showUserList = !this.showUserList;
    this.searchQuery = '';
  }

  async startConversation(user: SimpleUser): Promise<void> {
    try {
      const conversation = await firstValueFrom(
        this.chatService.createDirectConversation(user.userId)
      );

      // Check if conversation already exists in list
      const exists = this.conversations().find(c => c.id === conversation.id);
      if (!exists) {
        this.conversations.set([conversation, ...this.conversations()]);
      }

      this.toggleUserList();
      await this.selectConversation(conversation);
    } catch (error) {
      console.error('Failed to create conversation:', error);
    }
  }

  filteredUsers(): SimpleUser[] {
    if (!this.searchQuery.trim()) {
      return this.users();
    }
    
    const query = this.searchQuery.toLowerCase();
    return this.users().filter(user =>
      user.fullName.toLowerCase().includes(query) ||
      user.userName.toLowerCase().includes(query)
    );
  }

  sortedConversations(): Conversation[] {
    return [...this.conversations()].sort((a, b) => {
      const dateA = new Date(a.lastMessageAt || a.createdAt).getTime();
      const dateB = new Date(b.lastMessageAt || b.createdAt).getTime();
      return dateB - dateA;
    });
  }

  getConversationTitle(conversation: Conversation): string {
    const otherParticipant = conversation.participants.find(
      p => p.userId !== this.currentUserId
    );
    return otherParticipant?.userName || 'Conversa';
  }

  getLastMessagePreview(conversation: Conversation): string {
    if (!conversation.lastMessage) {
      return 'Nenhuma mensagem ainda';
    }
    const content = conversation.lastMessage.content;
    return content.length > 50
      ? content.substring(0, 50) + '...'
      : content;
  }

  isOwnMessage(message: ChatMessage): boolean {
    return message.senderId === this.currentUserId;
  }

  formatTime(date: Date | string): string {
    try {
      const dateObj = typeof date === 'string' ? new Date(date) : date;
      return formatDistanceToNow(dateObj, { addSuffix: true, locale: ptBR });
    } catch {
      return '';
    }
  }

  private scrollToBottom(): void {
    if (this.messagesContainer?.nativeElement) {
      const element = this.messagesContainer.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }

  get totalUnreadCount(): number {
    return this.conversations().reduce((sum, conv) => sum + (conv.unreadCount || 0), 0);
  }

  get hasUnread(): boolean {
    return this.totalUnreadCount > 0;
  }
}
