import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WebhookService } from '../../services/webhook.service';
import { 
  WebhookSubscriptionDto, 
  CreateWebhookSubscriptionDto,
  WebhookEvent,
  WebhookEventLabels
} from '../../models/webhook.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-webhooks-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './webhooks-list.html',
  styleUrl: './webhooks-list.scss'
})
export class WebhooksList implements OnInit {
  subscriptions = signal<WebhookSubscriptionDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingSubscription: WebhookSubscriptionDto | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  formData: CreateWebhookSubscriptionDto = {
    name: '',
    description: '',
    targetUrl: '',
    subscribedEvents: [],
    maxRetries: 3,
    retryDelaySeconds: 60
  };

  availableEvents = Object.entries(WebhookEventLabels).map(([key, label]) => ({
    value: parseInt(key),
    label
  }));

  constructor(
    private webhookService: WebhookService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSubscriptions();
  }

  loadSubscriptions(): void {
    this.loading.set(true);
    this.error.set(null);

    this.webhookService.getAllSubscriptions().subscribe({
      next: (data) => {
        this.subscriptions.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error loading webhooks');
        this.loading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingSubscription = null;
    this.formData = {
      name: '',
      description: '',
      targetUrl: '',
      subscribedEvents: [],
      maxRetries: 3,
      retryDelaySeconds: 60
    };
    this.showModal = true;
  }

  openEditModal(subscription: WebhookSubscriptionDto): void {
    this.editingSubscription = subscription;
    this.formData = {
      name: subscription.name,
      description: subscription.description || '',
      targetUrl: subscription.targetUrl,
      subscribedEvents: [...subscription.subscribedEvents],
      maxRetries: subscription.maxRetries,
      retryDelaySeconds: subscription.retryDelaySeconds
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingSubscription = null;
    this.modalError.set(null);
  }

  onSubmit(): void {
    this.submitting.set(true);
    this.modalError.set(null);

    if (this.editingSubscription) {
      this.webhookService.updateSubscription(this.editingSubscription.id, this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadSubscriptions();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Error updating webhook');
          this.submitting.set(false);
        }
      });
    } else {
      this.webhookService.createSubscription(this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadSubscriptions();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Error creating webhook');
          this.submitting.set(false);
        }
      });
    }
  }

  toggleStatus(id: string, isActive: boolean): void {
    const action = isActive ? this.webhookService.deactivateSubscription(id) : this.webhookService.activateSubscription(id);
    
    action.subscribe({
      next: () => {
        this.loadSubscriptions();
      },
      error: (err) => {
        alert(err.error?.message || 'Error toggling webhook status');
      }
    });
  }

  regenerateSecret(id: string): void {
    if (!confirm('Are you sure you want to regenerate the secret? The old secret will stop working.')) {
      return;
    }

    this.webhookService.regenerateSecret(id).subscribe({
      next: () => {
        this.loadSubscriptions();
        alert('Secret regenerated successfully');
      },
      error: (err) => {
        alert(err.error?.message || 'Error regenerating secret');
      }
    });
  }

  deleteSubscription(id: string, name: string): void {
    if (!confirm(`Are you sure you want to delete webhook "${name}"?`)) {
      return;
    }

    this.webhookService.deleteSubscription(id).subscribe({
      next: () => {
        this.loadSubscriptions();
      },
      error: (err) => {
        alert(err.error?.message || 'Error deleting webhook');
      }
    });
  }

  viewDeliveries(id: string): void {
    this.router.navigate(['/webhooks', id, 'deliveries']);
  }

  getEventLabels(events: number[]): string {
    return events.map(e => WebhookEventLabels[e as WebhookEvent]).join(', ');
  }

  toggleEventSelection(eventValue: number): void {
    const index = this.formData.subscribedEvents.indexOf(eventValue);
    if (index > -1) {
      this.formData.subscribedEvents.splice(index, 1);
    } else {
      this.formData.subscribedEvents.push(eventValue);
    }
  }

  isEventSelected(eventValue: number): boolean {
    return this.formData.subscribedEvents.includes(eventValue);
  }
}
