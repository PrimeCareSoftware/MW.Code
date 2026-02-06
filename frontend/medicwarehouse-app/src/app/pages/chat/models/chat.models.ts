export interface ChatMessage {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  content: string;
  type: 'Text' | 'Image' | 'File' | 'System';
  sentAt: Date;
  isEdited: boolean;
  editedAt?: Date;
  isDeleted: boolean;
  readBy: string[];
  replyToMessageId?: string;
}

export interface Conversation {
  id: string;
  title: string;
  type: 'Direct' | 'Group';
  createdAt: Date;
  lastMessageAt?: Date;
  lastMessage?: ChatMessage;
  participants: ChatParticipant[];
  unreadCount: number;
}

export interface ChatParticipant {
  id: string;
  userId: string;
  userName: string;
  fullName: string;
  joinedAt: Date;
  isActive: boolean;
  unreadCount: number;
  isMuted: boolean;
  role: 'Member' | 'Admin';
}

export interface UserPresence {
  userId: string;
  userName: string;
  fullName: string;
  status: 'Online' | 'Away' | 'Busy' | 'Offline';
  lastSeenAt: Date;
  isOnline: boolean;
  statusMessage?: string;
}

export interface SendMessageDto {
  conversationId: string;
  content: string;
  replyToMessageId?: string;
}

export interface CreateDirectConversationDto {
  otherUserId: string;
}

export interface MarkAsReadDto {
  conversationId: string;
  messageId: string;
}

export interface TypingIndicator {
  userId: string;
  conversationId: string;
  timestamp: Date;
}

export interface PresenceChangeEvent {
  userId: string;
  status: string;
  isOnline: boolean;
  statusMessage?: string;
  timestamp: Date;
}

export interface MessageReadEvent {
  messageId: string;
  conversationId: string;
  readBy: string;
  readAt: Date;
}
