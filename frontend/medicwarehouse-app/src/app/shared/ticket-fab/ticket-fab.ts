import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { NotificationService } from '../../services/notification.service';
import { CreateTicketRequest, TicketType, TicketPriority, getTicketTypeLabel, getTicketPriorityLabel } from '../../models/ticket.model';

@Component({
  selector: 'app-ticket-fab',
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-fab.html',
  styleUrl: './ticket-fab.scss'
})
export class TicketFab {
  showModal = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  
  // Form data
  title = '';
  description = '';
  selectedType: TicketType = TicketType.Other;
  selectedPriority: TicketPriority = TicketPriority.Medium;
  attachments: File[] = [];
  
  // Enums for template
  TicketType = TicketType;
  TicketPriority = TicketPriority;
  
  // Helper functions
  getTicketTypeLabel = getTicketTypeLabel;
  getTicketPriorityLabel = getTicketPriorityLabel;
  
  // Ticket types array
  ticketTypes = [
    { value: TicketType.BugReport, label: 'Reporte de Bug' },
    { value: TicketType.FeatureRequest, label: 'Solicitação de Funcionalidade' },
    { value: TicketType.SystemAdjustment, label: 'Ajuste no Sistema' },
    { value: TicketType.FinancialIssue, label: 'Questão Financeira' },
    { value: TicketType.TechnicalSupport, label: 'Suporte Técnico' },
    { value: TicketType.UserSupport, label: 'Suporte ao Usuário' },
    { value: TicketType.Other, label: 'Outro' }
  ];
  
  // Ticket priorities array
  ticketPriorities = [
    { value: TicketPriority.Low, label: 'Baixa' },
    { value: TicketPriority.Medium, label: 'Média' },
    { value: TicketPriority.High, label: 'Alta' },
    { value: TicketPriority.Critical, label: 'Crítica' }
  ];

  constructor(
    private ticketService: TicketService,
    private notificationService: NotificationService
  ) {}

  openModal(): void {
    this.showModal.set(true);
    this.resetForm();
  }

  closeModal(): void {
    this.showModal.set(false);
    this.resetForm();
  }

  resetForm(): void {
    this.title = '';
    this.description = '';
    this.selectedType = TicketType.Other;
    this.selectedPriority = TicketPriority.Medium;
    this.attachments = [];
  }

  async onSubmit(): Promise<void> {
    if (!this.title.trim() || !this.description.trim()) {
      this.notificationService.warning('Por favor, preencha o título e a descrição do chamado.');
      return;
    }

    this.isSubmitting.set(true);

    try {
      const request: CreateTicketRequest = {
        title: this.title.trim(),
        description: this.description.trim(),
        type: this.selectedType,
        priority: this.selectedPriority
      };

      const response = await this.ticketService.createTicket(request).toPromise();
      
      // Upload attachments if any
      if (this.attachments.length > 0 && response?.ticketId) {
        for (const file of this.attachments) {
          try {
            const base64 = await this.ticketService.fileToBase64(file);
            await this.ticketService.uploadAttachment(response.ticketId, {
              fileName: file.name,
              base64Data: base64,
              contentType: file.type
            }).toPromise();
          } catch (error) {
            console.error('Error uploading attachment:', error);
            this.notificationService.warning('Alguns anexos não puderam ser enviados.');
          }
        }
      }

      this.notificationService.success('Chamado criado com sucesso!');
      this.closeModal();
    } catch (error: any) {
      console.error('Error creating ticket:', error);
      this.notificationService.error(error?.error?.message || 'Erro ao criar chamado. Por favor, tente novamente.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      // Add new files to existing attachments
      const newFiles = Array.from(input.files).filter(file => file.type.startsWith('image/'));
      this.attachments = [...this.attachments, ...newFiles];
      
      // Reset input to allow selecting the same file again
      input.value = '';
    }
  }

  onPaste(event: ClipboardEvent): void {
    const file = this.ticketService.getImageFromClipboard(event);
    if (file) {
      this.attachments = [...this.attachments, file];
      event.preventDefault();
    }
  }

  removeAttachment(index: number): void {
    this.attachments = this.attachments.filter((_, i) => i !== index);
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  get hasUnreadTickets(): boolean {
    return this.ticketService.unreadTicketCount() > 0;
  }

  get unreadCount(): number {
    return this.ticketService.unreadTicketCount();
  }
}
