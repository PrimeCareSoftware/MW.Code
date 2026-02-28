import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../services/ticket.service';
import { NotificationService } from '../../services/notification.service';
import { 
  TicketSummary, 
  Ticket,
  TicketStatus,
  getTicketStatusLabel, 
  getTicketTypeLabel, 
  getTicketPriorityLabel,
  getTicketStatusColor,
  getTicketPriorityColor,
  UpdateTicketStatusRequest,
  AddTicketCommentRequest
} from '../../models/ticket.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-tickets',
  imports: [CommonModule, FormsModule],
  templateUrl: './tickets.html',
  styleUrl: './tickets.scss'
})
export class Tickets implements OnInit {
  tickets = signal<TicketSummary[]>([]);
  selectedTicket = signal<Ticket | null>(null);
  isLoading = signal<boolean>(false);
  showDetailModal = signal<boolean>(false);
  
  // Comment form
  newComment = '';
  isAddingComment = signal<boolean>(false);
  
  // Filter
  statusFilter: TicketStatus | 'all' = 'all';
  TicketStatus = TicketStatus;
  
  // Helper functions for template
  getTicketStatusLabel = getTicketStatusLabel;
  getTicketTypeLabel = getTicketTypeLabel;
  getTicketPriorityLabel = getTicketPriorityLabel;
  getTicketStatusColor = getTicketStatusColor;
  getTicketPriorityColor = getTicketPriorityColor;

  constructor(
    private ticketService: TicketService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  async loadTickets(): Promise<void> {
    this.isLoading.set(true);
    try {
      const tickets = await this.ticketService.getMyTickets().toPromise();
      this.tickets.set(tickets || []);
    } catch (error) {
      console.error('Error loading tickets:', error);
      this.notificationService.error('Erro ao carregar chamados');
    } finally {
      this.isLoading.set(false);
    }
  }

  get filteredTickets(): TicketSummary[] {
    if (this.statusFilter === 'all') {
      return this.tickets();
    }
    return this.tickets().filter(t => t.status === this.statusFilter);
  }

  async viewTicket(ticketId: string): Promise<void> {
    try {
      const ticket = await this.ticketService.getTicket(ticketId).toPromise();
      if (ticket) {
        this.selectedTicket.set(ticket);
        this.showDetailModal.set(true);
        await this.ticketService.markAsRead(ticketId).toPromise();
      }
    } catch (error) {
      console.error('Error loading ticket details:', error);
      this.notificationService.error('Erro ao carregar detalhes do chamado');
    }
  }

  closeDetailModal(): void {
    this.showDetailModal.set(false);
    this.selectedTicket.set(null);
    this.newComment = '';
  }

  async addComment(): Promise<void> {
    const ticket = this.selectedTicket();
    if (!ticket || !this.newComment.trim()) {
      return;
    }

    this.isAddingComment.set(true);
    try {
      const request: AddTicketCommentRequest = {
        comment: this.newComment.trim()
      };
      
      await this.ticketService.addComment(ticket.id, request).toPromise();
      
      // Reload ticket details
      const updatedTicket = await this.ticketService.getTicket(ticket.id).toPromise();
      if (updatedTicket) {
        this.selectedTicket.set(updatedTicket);
      }
      
      this.newComment = '';
      this.notificationService.success('Comentário adicionado com sucesso');
    } catch (error) {
      console.error('Error adding comment:', error);
      this.notificationService.error('Erro ao adicionar comentário');
    } finally {
      this.isAddingComment.set(false);
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusBadgeClass(status: TicketStatus): string {
    const classes: { [key in TicketStatus]: string } = {
      [TicketStatus.Open]: 'badge-open',
      [TicketStatus.InAnalysis]: 'badge-analysis',
      [TicketStatus.InProgress]: 'badge-progress',
      [TicketStatus.Blocked]: 'badge-blocked',
      [TicketStatus.Completed]: 'badge-completed',
      [TicketStatus.Cancelled]: 'badge-cancelled'
    };
    return classes[status];
  }

  getPriorityBadgeClass(priority: number): string {
    const classes = ['badge-low', 'badge-medium', 'badge-high', 'badge-critical'];
    return classes[priority] || 'badge-medium';
  }
}
