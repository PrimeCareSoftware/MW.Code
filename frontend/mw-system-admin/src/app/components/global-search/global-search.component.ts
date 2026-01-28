import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-global-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="search-modal" *ngIf="isOpen" (click)="closeModal()">
      <div class="search-container" (click)="$event.stopPropagation()">
        <input
          type="text"
          [(ngModel)]="query"
          placeholder="Buscar... (Ctrl+K)"
          class="search-input"
        />
        <p class="hint">Busca global - Em desenvolvimento</p>
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
      padding: 24px;
      width: 90%;
      max-width: 600px;
    }

    .search-input {
      width: 100%;
      border: 1px solid #e5e7eb;
      border-radius: 8px;
      padding: 12px;
      font-size: 16px;
    }

    .hint {
      margin-top: 12px;
      color: #6b7280;
      font-size: 14px;
    }
  `]
})
export class GlobalSearchComponent {
  isOpen = false;
  query = '';

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
      event.preventDefault();
      this.isOpen = !this.isOpen;
    }
    if (event.key === 'Escape') {
      this.closeModal();
    }
  }

  closeModal(): void {
    this.isOpen = false;
  }
}
