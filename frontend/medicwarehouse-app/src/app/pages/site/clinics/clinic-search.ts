import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { PublicClinicService, PublicClinicDto, SearchClinicsRequest } from '../../../services/public-clinic.service';

@Component({
  selector: 'app-clinic-search',
  imports: [CommonModule, FormsModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './clinic-search.html',
  styleUrl: './clinic-search.scss'
})
export class ClinicSearchComponent implements OnInit {
  clinics: PublicClinicDto[] = [];
  loading = false;
  error: string | null = null;
  
  // Filtros de busca
  searchName = '';
  searchCity = '';
  searchState = '';
  
  // Paginação
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  constructor(
    private publicClinicService: PublicClinicService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.searchClinics();
  }

  searchClinics(): void {
    this.loading = true;
    this.error = null;

    const request: SearchClinicsRequest = {
      name: this.searchName || undefined,
      city: this.searchCity || undefined,
      state: this.searchState || undefined,
      pageNumber: this.currentPage,
      pageSize: this.pageSize
    };

    this.publicClinicService.searchClinics(request).subscribe({
      next: (result) => {
        this.clinics = result.clinics;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.currentPage = result.pageNumber;
        this.loading = false;
      },
      error: (err) => {
        console.error('Erro ao buscar clínicas:', err);
        this.error = 'Não foi possível carregar as clínicas. Por favor, tente novamente.';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1; // Reset to first page on new search
    this.searchClinics();
  }

  clearFilters(): void {
    this.searchName = '';
    this.searchCity = '';
    this.searchState = '';
    this.currentPage = 1;
    this.searchClinics();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.searchClinics();
    }
  }

  scheduleAppointment(clinic: PublicClinicDto): void {
    this.router.navigate(['/site/clinics', clinic.id, 'schedule']);
  }

  formatTime(time: string): string {
    // Format time string (e.g., "08:00:00" to "08:00")
    return time.substring(0, 5);
  }

  getPaginationArray(): number[] {
    const pages: number[] = [];
    const maxPages = 5; // Show max 5 page numbers
    
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);
    
    // Adjust start if we're near the end
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}
