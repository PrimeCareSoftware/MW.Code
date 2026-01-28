import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicSummary, PaginatedClinics, ClinicFilter, Tag } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';
import { ClinicsCardsComponent } from './clinics-cards';
import { ClinicsKanbanComponent } from './clinics-kanban';
import { ClinicsMapComponent } from './clinics-map';

@Component({
  selector: 'app-clinics-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar, ClinicsCardsComponent, ClinicsKanbanComponent, ClinicsMapComponent],
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

  // View mode (list, cards, map, kanban)
  viewMode = signal<'list' | 'cards' | 'map' | 'kanban'>('list');
  
  // Bulk selection
  selectedClinicIds = signal<string[]>([]);
  selectAllChecked = signal(false);
  showBulkActions = signal(false);
  
  // Export
  exportInProgress = signal(false);

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
    // Load counts for each segment
    const segments = ['new', 'trial', 'at-risk', 'healthy', 'needs-attention'];
    segments.forEach(segment => {
      this.systemAdminService.getClinicsBySegment(segment).subscribe({
        next: (response) => {
          // Create a new object to maintain immutability
          const currentCounts = this.segmentCounts();
          const updatedCounts = { ...currentCounts };
          
          if (segment === 'new') updatedCounts.new = response.totalCount;
          else if (segment === 'trial') updatedCounts.trial = response.totalCount;
          else if (segment === 'at-risk') updatedCounts.atRisk = response.totalCount;
          else if (segment === 'healthy') updatedCounts.healthy = response.totalCount;
          else if (segment === 'needs-attention') updatedCounts.needsAttention = response.totalCount;
          
          this.segmentCounts.set(updatedCounts);
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
    const currentTags = this.selectedTags();
    const updatedTags = currentTags.includes(tagId)
      ? currentTags.filter(id => id !== tagId)
      : [...currentTags, tagId];
    this.selectedTags.set(updatedTags);
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

  // View mode methods
  setViewMode(mode: 'list' | 'cards' | 'map' | 'kanban'): void {
    this.viewMode.set(mode);
  }

  // Bulk selection methods
  toggleSelectAll(): void {
    const newValue = !this.selectAllChecked();
    this.selectAllChecked.set(newValue);
    
    if (newValue) {
      const allIds = this.paginatedClinics()?.clinics.map(c => c.id) || [];
      this.selectedClinicIds.set(allIds);
    } else {
      this.selectedClinicIds.set([]);
    }
    
    this.showBulkActions.set(this.selectedClinicIds().length > 0);
  }

  toggleClinicSelection(clinicId: string): void {
    const currentSelected = this.selectedClinicIds();
    const index = currentSelected.indexOf(clinicId);
    
    if (index > -1) {
      this.selectedClinicIds.set(currentSelected.filter(id => id !== clinicId));
    } else {
      this.selectedClinicIds.set([...currentSelected, clinicId]);
    }
    
    const allSelected = this.paginatedClinics()?.clinics.length === this.selectedClinicIds().length;
    this.selectAllChecked.set(allSelected);
    this.showBulkActions.set(this.selectedClinicIds().length > 0);
  }

  isClinicSelected(clinicId: string): boolean {
    return this.selectedClinicIds().includes(clinicId);
  }

  clearSelection(): void {
    this.selectedClinicIds.set([]);
    this.selectAllChecked.set(false);
    this.showBulkActions.set(false);
  }

  // Bulk actions
  bulkActivate(): void {
    if (!confirm(`Ativar ${this.selectedClinicIds().length} clínica(s) selecionada(s)?`)) {
      return;
    }

    this.systemAdminService.bulkAction({
      action: 'activate',
      clinicIds: this.selectedClinicIds(),
      parameters: {}
    }).subscribe({
      next: (result) => {
        alert(`Ação concluída: ${result.successCount} sucesso(s), ${result.failureCount} falha(s)`);
        this.loadClinics();
        this.clearSelection();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao executar ação em lote');
      }
    });
  }

  bulkDeactivate(): void {
    if (!confirm(`Desativar ${this.selectedClinicIds().length} clínica(s) selecionada(s)?`)) {
      return;
    }

    this.systemAdminService.bulkAction({
      action: 'deactivate',
      clinicIds: this.selectedClinicIds(),
      parameters: {}
    }).subscribe({
      next: (result) => {
        alert(`Ação concluída: ${result.successCount} sucesso(s), ${result.failureCount} falha(s)`);
        this.loadClinics();
        this.clearSelection();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao executar ação em lote');
      }
    });
  }

  bulkAddTag(): void {
    const tagId = prompt('Digite o ID da tag para adicionar:');
    if (!tagId) return;

    this.systemAdminService.bulkAction({
      action: 'addTag',
      clinicIds: this.selectedClinicIds(),
      parameters: { tagId }
    }).subscribe({
      next: (result) => {
        alert(`Ação concluída: ${result.successCount} sucesso(s), ${result.failureCount} falha(s)`);
        this.loadClinics();
        this.clearSelection();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao adicionar tags');
      }
    });
  }

  // Export methods
  exportClinics(format: 'csv' | 'excel' | 'pdf'): void {
    if (this.selectedClinicIds().length === 0) {
      alert('Selecione pelo menos uma clínica para exportar');
      return;
    }

    this.exportInProgress.set(true);

    this.systemAdminService.exportClinics({
      clinicIds: this.selectedClinicIds(),
      format: format === 'csv' ? 'Csv' : format === 'excel' ? 'Excel' : 'Pdf',
      includeHealthScore: true,
      includeTags: true,
      includeUsageMetrics: false
    }).subscribe({
      next: (blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `clinics_export_${new Date().getTime()}.${format === 'excel' ? 'xlsx' : format}`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        this.exportInProgress.set(false);
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao exportar clínicas');
        this.exportInProgress.set(false);
      }
    });
  }
}

