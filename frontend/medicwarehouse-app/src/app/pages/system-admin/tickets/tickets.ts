import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { 
  TicketSummary, 
  Ticket,
  TicketStatus,
  TicketType,
  TicketStatistics,
  getTicketStatusLabel, 
  getTicketTypeLabel, 
  getTicketPriorityLabel,
  getTicketStatusColor,
  getTicketPriorityColor,
  UpdateTicketStatusRequest,
  AddTicketCommentRequest,
  AssignTicketRequest
} from '../../models/ticket.model';

interface KanbanColumn {
  status: TicketStatus;
  label: string;
  color: string;
  tickets: TicketSummary[];
}

@Component({
  selector: 'app-tickets',
  imports: [CommonModule, FormsModule],
  templateUrl: './tickets.html',
  styleUrl: './tickets.scss'
})
export class TicketsPage implements OnInit {
  tickets = signal<TicketSummary[]>([]);
  statistics = signal<TicketStatistics | null>(null);
  selectedTicket = signal<Ticket | null>(null);
  isLoading = signal<boolean>(false);
  showDetailModal = signal<boolean>(false);
  
  // Filters
  typeFilter: TicketType | 'all' = 'all';
  searchQuery = '';
  
  // Comment form
  newComment = '';
  isAddingComment = signal<boolean>(false);
  
  // Status change
  newStatus: TicketStatus | null = null;
  statusComment = '';
  
  // Kanban columns
  columns = signal<KanbanColumn[]>([
    { status: TicketStatus.Open, label: 'Aberto', color: '#3b82f6', tickets: [] },
    { status: TicketStatus.InAnalysis, label: 'Em Análise', color: '#f59e0b', tickets: [] },
    { status: TicketStatus.InProgress, label: 'Em Atendimento', color: '#8b5cf6', tickets: [] },
    { status: TicketStatus.Blocked, label: 'Com Impedimento', color: '#ef4444', tickets: [] },
    { status: TicketStatus.Completed, label: 'Concluído', color: '#10b981', tickets: [] },
    { status: TicketStatus.Cancelled, label: 'Cancelado', color: '#6b7280', tickets: [] }
  ]);
  
  // View mode
  viewMode: 'kanban' | 'list' = 'kanban';
  
  // Enums for template
  TicketStatus = TicketStatus;
  TicketType = TicketType;
  
  // Helper functions for template
  getTicketStatusLabel = getTicketStatusLabel;
  getTicketTypeLabel = getTicketTypeLabel;
  getTicketPriorityLabel = getTicketPriorityLabel;
  getTicketStatusColor = getTicketStatusColor;
  getTicketPriorityColor = getTicketPriorityColor;

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.loadTickets();
    this.loadStatistics();
  }

  async loadTickets(): Promise<void> {
    this.isLoading.set(true);
    try {
      const tickets = await this.ticketService.getAllTickets().toPromise();
      this.tickets.set(tickets || []);
      this.updateKanbanColumns();
    } catch (error) {
      console.error('Error loading tickets:', error);
      alert('Erro ao carregar chamados');
    } finally {
      this.isLoading.set(false);
    }
  }

  async loadStatistics(): Promise<void> {
    try {
      const stats = await this.ticketService.getStatistics().toPromise();
      this.statistics.set(stats || null);
    } catch (error) {
      console.error('Error loading statistics:', error);
    }
  }

  updateKanbanColumns(): void {
    const updatedColumns = this.columns().map(column => ({
      ...column,
      tickets: this.filteredTickets.filter(t => t.status === column.status)
    }));
    this.columns.set(updatedColumns);
  }

  get filteredTickets(): TicketSummary[] {
    let filtered = this.tickets();
    
    if (this.typeFilter !== 'all') {
      filtered = filtered.filter(t => t.type === this.typeFilter);
    }
    
    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(t => 
        t.title.toLowerCase().includes(query) ||
        t.userName.toLowerCase().includes(query) ||
        (t.clinicName && t.clinicName.toLowerCase().includes(query))
      );
    }
    
    return filtered;
  }

  async viewTicket(ticketId: string): Promise<void> {
    try {
      const ticket = await this.ticketService.getTicket(ticketId).toPromise();
      if (ticket) {
        this.selectedTicket.set(ticket);
        this.showDetailModal.set(true);
        this.newStatus = ticket.status;
        this.statusComment = '';
      }
    } catch (error) {
      console.error('Error loading ticket details:', error);
      alert('Erro ao carregar detalhes do chamado');
    }
  }

  closeDetailModal(): void {
    this.showDetailModal.set(false);
    this.selectedTicket.set(null);
    this.newComment = '';
    this.newStatus = null;
    this.statusComment = '';
  }

  async updateStatus(): Promise<void> {
    const ticket = this.selectedTicket();
    if (!ticket || this.newStatus === null || this.newStatus === ticket.status) {
      return;
    }

    try {
      const request: UpdateTicketStatusRequest = {
        status: this.newStatus,
        comment: this.statusComment.trim() || undefined
      };
      
      await this.ticketService.updateTicketStatus(ticket.id, request).toPromise();
      
      alert('Status atualizado com sucesso');
      await this.loadTickets();
      await this.loadStatistics();
      
      // Reload ticket details
      const updatedTicket = await this.ticketService.getTicket(ticket.id).toPromise();
      if (updatedTicket) {
        this.selectedTicket.set(updatedTicket);
        this.newStatus = updatedTicket.status;
      }
      
      this.statusComment = '';
    } catch (error) {
      console.error('Error updating status:', error);
      alert('Erro ao atualizar status');
    }
  }

  async addComment(): Promise<void> {
    const ticket = this.selectedTicket();
    if (!ticket || !this.newComment.trim()) {
      return;
    }

    this.isAddingComment.set(true);
    try {
      const request: AddTicketCommentRequest = {
        comment: this.newComment.trim(),
        isInternal: false
      };
      
      await this.ticketService.addComment(ticket.id, request).toPromise();
      
      // Reload ticket details
      const updatedTicket = await this.ticketService.getTicket(ticket.id).toPromise();
      if (updatedTicket) {
        this.selectedTicket.set(updatedTicket);
      }
      
      this.newComment = '';
      alert('Comentário adicionado com sucesso');
    } catch (error) {
      console.error('Error adding comment:', error);
      alert('Erro ao adicionar comentário');
    } finally {
      this.isAddingComment.set(false);
    }
  }

  onDragStart(event: DragEvent, ticket: TicketSummary): void {
    event.dataTransfer!.effectAllowed = 'move';
    event.dataTransfer!.setData('ticketId', ticket.id);
    event.dataTransfer!.setData('fromStatus', ticket.status.toString());
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.dataTransfer!.dropEffect = 'move';
  }

  async onDrop(event: DragEvent, toStatus: TicketStatus): Promise<void> {
    event.preventDefault();
    
    const ticketId = event.dataTransfer!.getData('ticketId');
    const fromStatus = parseInt(event.dataTransfer!.getData('fromStatus'));
    
    if (fromStatus === toStatus) {
      return;
    }

    try {
      const request: UpdateTicketStatusRequest = {
        status: toStatus
      };
      
      await this.ticketService.updateTicketStatus(ticketId, request).toPromise();
      await this.loadTickets();
      await this.loadStatistics();
    } catch (error) {
      console.error('Error updating ticket status:', error);
      alert('Erro ao atualizar status do chamado');
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

  switchViewMode(mode: 'kanban' | 'list'): void {
    this.viewMode = mode;
    if (mode === 'kanban') {
      this.updateKanbanColumns();
    }
  }

  applyFilters(): void {
    if (this.viewMode === 'kanban') {
      this.updateKanbanColumns();
    }
  }
}
