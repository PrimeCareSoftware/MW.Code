import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WebhookService } from '../../services/webhook.service';
import { WebhookDeliveryDto, WebhookSubscriptionDto } from '../../models/webhook.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-webhook-deliveries',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './webhook-deliveries.html',
  styleUrl: './webhook-deliveries.scss'
})
export class WebhookDeliveries implements OnInit {
  subscriptionId!: string;
  subscription = signal<WebhookSubscriptionDto | null>(null);
  deliveries = signal<WebhookDeliveryDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  selectedDelivery = signal<WebhookDeliveryDto | null>(null);

  constructor(
    private webhookService: WebhookService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.subscriptionId = id;
      this.loadSubscription();
      this.loadDeliveries();
    }
  }

  loadSubscription(): void {
    this.webhookService.getSubscription(this.subscriptionId).subscribe({
      next: (subscription) => {
        this.subscription.set(subscription);
      },
      error: (err) => {
        console.error('Error loading subscription:', err);
      }
    });
  }

  loadDeliveries(): void {
    this.loading.set(true);
    this.error.set(null);

    this.webhookService.getDeliveries(this.subscriptionId, 50).subscribe({
      next: (data) => {
        this.deliveries.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error loading deliveries');
        this.loading.set(false);
      }
    });
  }

  viewDeliveryDetails(delivery: WebhookDeliveryDto): void {
    this.selectedDelivery.set(delivery);
  }

  closeDetailsModal(): void {
    this.selectedDelivery.set(null);
  }

  retryDelivery(deliveryId: string): void {
    if (!confirm('Are you sure you want to retry this delivery?')) {
      return;
    }

    this.webhookService.retryDelivery(deliveryId).subscribe({
      next: (response) => {
        alert(response.message || 'Delivery retry initiated');
        this.loadDeliveries();
      },
      error: (err) => {
        alert(err.error?.message || 'Error retrying delivery');
      }
    });
  }

  getStatusClass(status: string): string {
    return status;
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'delivered': return '✅';
      case 'failed': return '❌';
      case 'pending': return '⏳';
      default: return '❓';
    }
  }

  formatPayload(payload: string): string {
    try {
      const parsed = JSON.parse(payload);
      return JSON.stringify(parsed, null, 2);
    } catch {
      return payload;
    }
  }

  backToWebhooks(): void {
    this.router.navigate(['/webhooks']);
  }
}
