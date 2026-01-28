import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicSummary, PaginatedClinics, ClinicFilter, Tag } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-clinics-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './clinics-list.html',
  styleUrl: './clinics-list.scss'})
export class ClinicsList implements OnInit {
  paginatedClinics = signal<PaginatedClinics | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  selectedFilter = signal('');
  currentPage = signal(1);
  
  // Advanced filters
  showAdvancedFilters = signal(false);
  searchTerm = signal('');
  selectedHealthStatus = signal<string | undefined>(undefined);
  selectedSubscriptionStatus = signal<string | undefined>(undefined);
  selectedTags = signal<string[]>([]);
  availableTags = signal<Tag[]>([]);
  
  // Segment counts
  segmentCounts = signal({
    new: 0,
    trial: 0,
    atRisk: 0,
    healthy: 0,
    needsAttention: 0
  });

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.selectedFilter.set(params['status'] || '');
      this.loadClinics();
      this.loadSegmentCounts();
    });
    this.loadAvailableTags();
  }

  loadClinics(): void {
    this.loading.set(true);
    this.error.set(null);

    // Build filter object for advanced filtering
    const filters: ClinicFilter = {
      page: this.currentPage(),
      pageSize: 20
    };

    // Apply simple status filter or advanced filters
    if (this.selectedFilter()) {
      if (this.selectedFilter() === 'active') {
        filters.isActive = true;
      } else if (this.selectedFilter() === 'inactive') {
        filters.isActive = false;
      }
    }

    // Apply advanced filters if any are set
    if (this.searchTerm()) {
      filters.searchTerm = this.searchTerm();
    }
    if (this.selectedHealthStatus()) {
      filters.healthStatus = this.selectedHealthStatus();
    }
    if (this.selectedSubscriptionStatus()) {
      filters.subscriptionStatus = this.selectedSubscriptionStatus();
    }
    if (this.selectedTags().length > 0) {
      filters.tagIds = this.selectedTags();
    }

    // Use advanced filter endpoint
    this.systemAdminService.filterClinics(filters).subscribe({
      next: (response) => {
        this.paginatedClinics.set({
          clinics: response.data,
          totalCount: response.totalCount,
          page: response.page,
          pageSize: response.pageSize,
          totalPages: response.totalPages
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar clínicas');
        this.loading.set(false);
      }
    });
  }

  loadSegmentCounts(): void {
    // Load counts for each segment (simplified - in production, use dedicated endpoint)
    const segments = ['new', 'trial', 'at-risk', 'healthy', 'needs-attention'];
    segments.forEach(segment => {
      this.systemAdminService.getClinicsBySegment(segment).subscribe({
        next: (response) => {
          const counts = this.segmentCounts();
          if (segment === 'new') counts.new = response.totalCount;
          else if (segment === 'trial') counts.trial = response.totalCount;
          else if (segment === 'at-risk') counts.atRisk = response.totalCount;
          else if (segment === 'healthy') counts.healthy = response.totalCount;
          else if (segment === 'needs-attention') counts.needsAttention = response.totalCount;
          this.segmentCounts.set(counts);
        },
        error: (err) => {
          console.error(`Error loading segment ${segment}:`, err);
        }
      });
    });
  }

  loadAvailableTags(): void {
    this.systemAdminService.getTags().subscribe({
      next: (tags) => {
        this.availableTags.set(tags);
      },
      error: (err) => {
        console.error('Error loading tags:', err);
      }
    });
  }

  applyFilter(status: string): void {
    this.selectedFilter.set(status);
    this.currentPage.set(1);
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: status ? { status } : {},
      queryParamsHandling: 'merge'
    });
  }

  applySegmentFilter(segment: string): void {
    this.selectedFilter.set('');
    this.currentPage.set(1);
    
    // Reset all filters first
    this.searchTerm.set('');
    this.selectedHealthStatus.set(undefined);
    this.selectedSubscriptionStatus.set(undefined);
    this.selectedTags.set([]);

    // Apply segment-specific filter
    if (segment === 'new') {
      // New clinics created in last 30 days - handled by backend
      this.systemAdminService.getClinicsBySegment('new').subscribe({
        next: (response) => {
          this.paginatedClinics.set({
            clinics: response.data,
            totalCount: response.totalCount,
            page: 1,
            pageSize: 20,
            totalPages: Math.ceil(response.totalCount / 20)
          });
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Erro ao carregar clínicas');
          this.loading.set(false);
        }
      });
    } else if (segment === 'trial') {
      this.selectedSubscriptionStatus.set('Trial');
      this.loadClinics();
    } else if (segment === 'at-risk') {
      this.selectedHealthStatus.set('AtRisk');
      this.loadClinics();
    } else if (segment === 'healthy') {
      this.selectedHealthStatus.set('Healthy');
      this.loadClinics();
    } else if (segment === 'needs-attention') {
      this.selectedHealthStatus.set('NeedsAttention');
      this.loadClinics();
    }
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters.set(!this.showAdvancedFilters());
  }

  applyAdvancedFilters(): void {
    this.currentPage.set(1);
    this.loadClinics();
  }

  clearAdvancedFilters(): void {
    this.searchTerm.set('');
    this.selectedHealthStatus.set(undefined);
    this.selectedSubscriptionStatus.set(undefined);
    this.selectedTags.set([]);
    this.currentPage.set(1);
    this.loadClinics();
  }

  toggleTagSelection(tagId: string): void {
    const tags = this.selectedTags();
    const index = tags.indexOf(tagId);
    if (index > -1) {
      tags.splice(index, 1);
    } else {
      tags.push(tagId);
    }
    this.selectedTags.set([...tags]);
  }

  isTagSelected(tagId: string): boolean {
    return this.selectedTags().includes(tagId);
  }

  getActiveFiltersCount(): number {
    let count = 0;
    if (this.searchTerm()) count++;
    if (this.selectedHealthStatus()) count++;
    if (this.selectedSubscriptionStatus()) count++;
    if (this.selectedTags().length > 0) count += this.selectedTags().length;
    return count;
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadClinics();
  }

  navigateToCreate(): void {
    this.router.navigate(['/clinics/create']);
  }

  viewDetails(id: string): void {
    this.router.navigate(['/clinics', id]);
  }

  toggleStatus(id: string, currentStatus: boolean): void {
    if (!confirm(`Tem certeza que deseja ${currentStatus ? 'desativar' : 'ativar'} esta clínica?`)) {
      return;
    }

    this.systemAdminService.toggleClinicStatus(id).subscribe({
      next: () => {
        this.loadClinics();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao alterar status da clínica');
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Active': 'Ativo',
      'Trial': 'Trial',
      'Expired': 'Expirado',
      'Suspended': 'Suspenso',
      'PaymentOverdue': 'Pagamento Atrasado',
      'Cancelled': 'Cancelado'
    };
    return labels[status] || status;
  }

  getStatusClass(status: string): string {
    const classes: { [key: string]: string } = {
      'Active': 'badge-active',
      'Trial': 'badge-trial',
      'Expired': 'badge-expired',
      'Suspended': 'badge-suspended',
      'PaymentOverdue': 'badge-expired',
      'Cancelled': 'badge-cancelled'
    };
    return classes[status] || '';
  }
}
