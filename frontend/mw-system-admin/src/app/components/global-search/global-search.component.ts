import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DomSanitizer, SafeHtml, SecurityContext } from '@angular/platform-browser';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { GlobalSearchService } from '../../services/global-search.service';
import { GlobalSearchResult } from '../../models/system-admin.model';

// Constants
const MIN_SEARCH_LENGTH = 2;
const SEARCH_DEBOUNCE_MS = 300;
const FOCUS_DELAY_MS = 100;

@Component({
  selector: 'app-global-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="search-modal" *ngIf="isOpen" (click)="closeModal()">
      <div class="search-container" (click)="$event.stopPropagation()">
        <div class="search-header">
          <input
            #searchInput
            type="text"
            [(ngModel)]="query"
            (ngModelChange)="onQueryChange($event)"
            placeholder="Buscar clínicas, usuários, tickets... (Ctrl+K)"
            class="search-input"
            autofocus
          />
          <span class="search-shortcut">ESC para fechar</span>
        </div>

        <!-- Loading -->
        <div *ngIf="isLoading" class="search-loading">
          <div class="spinner"></div>
          Buscando...
        </div>

        <!-- Results -->
        <div class="search-results" *ngIf="results && !isLoading">
          <!-- Clinics -->
          <div class="result-group" *ngIf="results.clinics && results.clinics.length > 0">
            <h3 class="result-group-title">🏥 Clínicas ({{ results.clinics.length }})</h3>
            <div 
              *ngFor="let clinic of results.clinics"
              class="result-item"
              (click)="navigateToClinic(clinic.id)"
            >
              <div class="result-icon">🏥</div>
              <div class="result-content">
                <div class="result-title" [innerHTML]="highlightQuery(clinic.name)"></div>
                <div class="result-subtitle">{{ clinic.document }} • {{ clinic.planName }} • {{ clinic.status }}</div>
              </div>
            </div>
          </div>

          <!-- Users -->
          <div class="result-group" *ngIf="results.users && results.users.length > 0">
            <h3 class="result-group-title">👥 Usuários ({{ results.users.length }})</h3>
            <div 
              *ngFor="let user of results.users"
              class="result-item"
              (click)="navigateToUser(user.id)"
            >
              <div class="result-icon">👤</div>
              <div class="result-content">
                <div class="result-title" [innerHTML]="highlightQuery(user.fullName)"></div>
                <div class="result-subtitle">@{{ user.username }} • {{ user.email }} • {{ user.clinicName }}</div>
              </div>
            </div>
          </div>

          <!-- Tickets -->
          <div class="result-group" *ngIf="results.tickets && results.tickets.length > 0">
            <h3 class="result-group-title">🎫 Tickets ({{ results.tickets.length }})</h3>
            <div 
              *ngFor="let ticket of results.tickets"
              class="result-item"
              (click)="navigateToTicket(ticket.id)"
            >
              <div class="result-icon">🎫</div>
              <div class="result-content">
                <div class="result-title" [innerHTML]="highlightQuery(ticket.title)"></div>
                <div class="result-subtitle">{{ ticket.status }} • {{ ticket.priority }} • {{ ticket.clinicName }}</div>
              </div>
            </div>
          </div>

          <!-- Plans -->
          <div class="result-group" *ngIf="results.plans && results.plans.length > 0">
            <h3 class="result-group-title">📋 Planos ({{ results.plans.length }})</h3>
            <div 
              *ngFor="let plan of results.plans"
              class="result-item"
              (click)="navigateToPlan(plan.id)"
            >
              <div class="result-icon">📋</div>
              <div class="result-content">
                <div class="result-title" [innerHTML]="highlightQuery(plan.name)"></div>
                <div class="result-subtitle">R$ {{ plan.monthlyPrice }} • {{ plan.activeSubscriptions }} assinaturas ativas</div>
              </div>
            </div>
          </div>

          <!-- No results -->
          <div *ngIf="results.totalResults === 0" class="no-results">
            <div class="no-results-icon">🔍</div>
            <p>Nenhum resultado encontrado para "{{ query }}"</p>
          </div>

          <!-- Search stats -->
          <div *ngIf="results.totalResults > 0" class="search-stats">
            {{ results.totalResults }} resultado(s) em {{ results.searchDurationMs?.toFixed(0) }}ms
          </div>
        </div>

        <!-- Recent Searches -->
        <div class="recent-searches" *ngIf="!query && !results && recentSearches.length > 0">
          <h3 class="result-group-title">🕒 Buscas Recentes</h3>
          <div 
            *ngFor="let recent of recentSearches"
            class="recent-item"
            (click)="query = recent; onQueryChange(recent)"
          >
            <div class="result-icon">🕒</div>
            <span>{{ recent }}</span>
          </div>
        </div>

        <!-- Empty state -->
        <div class="empty-state" *ngIf="!query && !results && recentSearches.length === 0">
          <p>Digite para buscar clínicas, usuários, tickets e mais...</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .search-modal {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: flex-start;
      justify-content: center;
      padding-top: 10vh;
      z-index: 9999;
    }

    .search-container {
      background: white;
      border-radius: 12px;
      width: 90%;
      max-width: 700px;
      max-height: 80vh;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
    }

    .search-header {
      padding: 20px;
      border-bottom: 1px solid #e5e7eb;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .search-input {
      flex: 1;
      border: none;
      outline: none;
      font-size: 16px;
      padding: 0;
    }

    .search-shortcut {
      font-size: 12px;
      color: #9ca3af;
      background: #f3f4f6;
      padding: 4px 8px;
      border-radius: 4px;
    }

    .search-loading {
      padding: 40px;
      text-align: center;
      color: #6b7280;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
    }

    .spinner {
      width: 24px;
      height: 24px;
      border: 3px solid #e5e7eb;
      border-top-color: #667eea;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .search-results {
      overflow-y: auto;
      max-height: calc(80vh - 80px);
      padding: 8px;
    }

    .result-group {
      margin-bottom: 16px;
    }

    .result-group-title {
      font-size: 12px;
      font-weight: 600;
      color: #6b7280;
      text-transform: uppercase;
      padding: 8px 12px;
      margin: 0;
    }

    .result-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      border-radius: 8px;
      cursor: pointer;
      transition: background 0.15s;
    }

    .result-item:hover {
      background: #f3f4f6;
    }

    .result-icon {
      font-size: 24px;
      flex-shrink: 0;
    }

    .result-content {
      flex: 1;
      min-width: 0;
    }

    .result-title {
      font-weight: 500;
      color: #1f2937;
      margin-bottom: 4px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .result-title ::ng-deep mark {
      background: #fef3c7;
      color: #92400e;
      padding: 2px 0;
      border-radius: 2px;
    }

    .result-subtitle {
      font-size: 13px;
      color: #6b7280;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .recent-searches {
      padding: 8px;
    }

    .recent-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      border-radius: 8px;
      cursor: pointer;
      transition: background 0.15s;
    }

    .recent-item:hover {
      background: #f3f4f6;
    }

    .no-results {
      padding: 60px 20px;
      text-align: center;
      color: #6b7280;
    }

    .no-results-icon {
      font-size: 48px;
      margin-bottom: 16px;
    }

    .empty-state {
      padding: 60px 20px;
      text-align: center;
      color: #9ca3af;
    }

    .search-stats {
      padding: 8px 12px;
      text-align: center;
      font-size: 12px;
      color: #9ca3af;
      border-top: 1px solid #e5e7eb;
    }
  `]
})
export class GlobalSearchComponent implements OnInit, OnDestroy {
  isOpen = false;
  query = '';
  results: GlobalSearchResult | null = null;
  recentSearches: string[] = [];
  isLoading = false;

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

  constructor(
    private searchService: GlobalSearchService,
    private router: Router,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    // Load recent searches
    this.recentSearches = this.searchService.getRecentSearches();

    // Setup debounced search
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      distinctUntilChanged(),
      switchMap(query => {
        this.isLoading = true;
        return this.searchService.search(query);
      })
    ).subscribe({
      next: (results) => {
        this.results = results;
        this.isLoading = false;
        if (this.query) {
          this.searchService.saveSearchToHistory(this.query);
          this.recentSearches = this.searchService.getRecentSearches();
        }
      },
      error: (error) => {
        console.error('Search error:', error);
        this.isLoading = false;
        // Show error state to user
        this.results = { 
          clinics: [], users: [], tickets: [], plans: [], auditLogs: [], 
          totalResults: 0, searchDurationMs: 0 
        };
      }
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
      event.preventDefault();
      this.isOpen = !this.isOpen;
      if (this.isOpen) {
        setTimeout(() => {
          const input = document.querySelector('.search-input') as HTMLInputElement;
          if (input) input.focus();
        }, FOCUS_DELAY_MS);
      }
    }
    if (event.key === 'Escape') {
      this.closeModal();
    }
  }

  onQueryChange(query: string): void {
    if (query.length >= MIN_SEARCH_LENGTH) {
      this.searchSubject.next(query);
    } else {
      this.results = null;
      this.isLoading = false;
    }
  }

  closeModal(): void {
    this.isOpen = false;
    this.query = '';
    this.results = null;
  }

  highlightQuery(text: string): SafeHtml {
    if (!this.query) return text;
    const regex = new RegExp(`(${this.escapeRegExp(this.query)})`, 'gi');
    const highlighted = text.replace(regex, '<mark>$1</mark>');
    return this.sanitizer.sanitize(SecurityContext.HTML, highlighted) || text;
  }

  private escapeRegExp(text: string): string {
    return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  }

  navigateToClinic(id: number): void {
    this.closeModal();
    this.router.navigate(['/clinics', id]);
  }

  navigateToUser(id: number): void {
    this.closeModal();
    this.router.navigate(['/users', id]);
  }

  navigateToTicket(id: number): void {
    this.closeModal();
    this.router.navigate(['/tickets', id]);
  }

  navigateToPlan(id: number): void {
    this.closeModal();
    this.router.navigate(['/plans', id]);
  }
}
