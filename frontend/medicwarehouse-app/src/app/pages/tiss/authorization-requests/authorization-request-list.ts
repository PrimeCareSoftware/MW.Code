import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthorizationRequestService } from '../../../services/authorization-request.service';
import { AuthorizationRequest, AuthorizationStatus } from '../../../models/tiss.model';

@Component({
  selector: 'app-authorization-request-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './authorization-request-list.html',
  styleUrl: './authorization-request-list.scss'
})
export class AuthorizationRequestList implements OnInit {
  requests = signal<AuthorizationRequest[]>([]);
  filteredRequests = signal<AuthorizationRequest[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedStatus: AuthorizationStatus | 'all' = 'all';

  AuthorizationStatus = AuthorizationStatus;

  statusOptions = [
    { value: 'all', label: 'Todos' },
    { value: AuthorizationStatus.Pending, label: 'Pendente' },
    { value: AuthorizationStatus.Approved, label: 'Aprovado' },
    { value: AuthorizationStatus.PartiallyApproved, label: 'Parcialmente Aprovado' },
    { value: AuthorizationStatus.Denied, label: 'Negado' },
    { value: AuthorizationStatus.Cancelled, label: 'Cancelado' }
  ];

  constructor(
    private authService: AuthorizationRequestService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.isLoading.set(true);
    this.authService.getAll().subscribe({
      next: (data) => {
        this.requests.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar autorizações');
        this.isLoading.set(false);
        console.error('Error loading requests:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = this.requests();

    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(r => r.status === this.selectedStatus);
    }

    this.filteredRequests.set(filtered);
  }

  onStatusChange(): void {
    this.applyFilters();
  }

  viewDetails(id: string): void {
    this.router.navigate(['/tiss/authorizations', id]);
  }

  getStatusClass(status: AuthorizationStatus): string {
    const statusMap: Record<AuthorizationStatus, string> = {
      [AuthorizationStatus.Pending]: 'badge-warning',
      [AuthorizationStatus.Approved]: 'badge-success',
      [AuthorizationStatus.PartiallyApproved]: 'badge-info',
      [AuthorizationStatus.Denied]: 'badge-danger',
      [AuthorizationStatus.Cancelled]: 'badge-secondary'
    };
    return statusMap[status] || 'badge-secondary';
  }

  getStatusLabel(status: AuthorizationStatus): string {
    const labelMap: Record<AuthorizationStatus, string> = {
      [AuthorizationStatus.Pending]: 'Pendente',
      [AuthorizationStatus.Approved]: 'Aprovado',
      [AuthorizationStatus.PartiallyApproved]: 'Parcialmente Aprovado',
      [AuthorizationStatus.Denied]: 'Negado',
      [AuthorizationStatus.Cancelled]: 'Cancelado'
    };
    return labelMap[status] || status;
  }
}
