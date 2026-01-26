import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { HelpButtonComponent } from '../../../shared/help-button/help-button';
import { TissGuideService } from '../../../services/tiss-guide.service';
import { TissGuide, GuideStatus, TissGuideType } from '../../../models/tiss.model';

@Component({
  selector: 'app-tiss-guide-list',
  imports: [HelpButtonComponent, CommonModule, RouterLink, FormsModule, Navbar],
  templateUrl: './tiss-guide-list.html',
  styleUrl: './tiss-guide-list.scss'
})
export class TissGuideList implements OnInit {
  guides = signal<TissGuide[]>([]);
  filteredGuides = signal<TissGuide[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = '';
  selectedStatus: GuideStatus | 'all' = 'all';

  GuideStatus = GuideStatus;
  TissGuideType = TissGuideType;

  statusOptions = [
    { value: 'all', label: 'Todos' },
    { value: GuideStatus.Draft, label: 'Rascunho' },
    { value: GuideStatus.Pending, label: 'Pendente' },
    { value: GuideStatus.Approved, label: 'Aprovado' },
    { value: GuideStatus.Rejected, label: 'Rejeitado' },
    { value: GuideStatus.Billed, label: 'Faturado' },
    { value: GuideStatus.Paid, label: 'Pago' },
    { value: GuideStatus.Cancelled, label: 'Cancelado' }
  ];

  constructor(
    private guideService: TissGuideService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadGuides();
  }

  loadGuides(): void {
    this.isLoading.set(true);
    this.guideService.getAll().subscribe({
      next: (data) => {
        this.guides.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar guias');
        this.isLoading.set(false);
        console.error('Error loading guides:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = this.guides();

    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(g => g.status === this.selectedStatus);
    }

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(g =>
        g.guideNumber.toLowerCase().includes(term) ||
        g.patientName?.toLowerCase().includes(term) ||
        g.authorizationNumber?.toLowerCase().includes(term)
      );
    }

    this.filteredGuides.set(filtered);
  }

  onSearch(): void {
    this.applyFilters();
  }

  onStatusChange(): void {
    this.applyFilters();
  }

  deleteGuide(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta guia?')) {
      this.guideService.delete(id).subscribe({
        next: () => {
          this.loadGuides();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir guia');
          console.error('Error deleting guide:', error);
        }
      });
    }
  }

  viewDetails(id: string): void {
    this.router.navigate(['/tiss/guides', id]);
  }

  getStatusClass(status: GuideStatus): string {
    const statusMap: Record<GuideStatus, string> = {
      [GuideStatus.Draft]: 'badge-secondary',
      [GuideStatus.Pending]: 'badge-warning',
      [GuideStatus.Approved]: 'badge-success',
      [GuideStatus.Rejected]: 'badge-danger',
      [GuideStatus.Billed]: 'badge-info',
      [GuideStatus.Paid]: 'badge-success',
      [GuideStatus.Cancelled]: 'badge-secondary'
    };
    return statusMap[status] || 'badge-secondary';
  }

  getStatusLabel(status: GuideStatus): string {
    const labelMap: Record<GuideStatus, string> = {
      [GuideStatus.Draft]: 'Rascunho',
      [GuideStatus.Pending]: 'Pendente',
      [GuideStatus.Approved]: 'Aprovado',
      [GuideStatus.Rejected]: 'Rejeitado',
      [GuideStatus.Billed]: 'Faturado',
      [GuideStatus.Paid]: 'Pago',
      [GuideStatus.Cancelled]: 'Cancelado'
    };
    return labelMap[status] || status;
  }

  getGuideTypeLabel(type: TissGuideType): string {
    const labelMap: Record<TissGuideType, string> = {
      [TissGuideType.ConsultationGuide]: 'Consulta',
      [TissGuideType.ServiceProvisionGuide]: 'SP/SADT',
      [TissGuideType.HospitalizationGuide]: 'Internação',
      [TissGuideType.SadtGuide]: 'SADT'
    };
    return labelMap[type] || type;
  }
}
