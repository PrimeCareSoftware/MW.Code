import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService, Theme } from '../../services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="theme-toggle" role="region" aria-label="Seletor de tema">
      <button 
        class="theme-toggle-btn"
        [class.active]="currentTheme === 'light'"
        (click)="setTheme('light')"
        [attr.aria-pressed]="currentTheme === 'light'"
        aria-label="Modo claro"
        title="Modo claro">
        <svg width="18" height="18" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M10 3V2M10 18v-1M17 10h1M2 10h1M15.657 4.343l.707-.707M3.636 16.364l.707-.707M15.657 15.657l.707.707M3.636 3.636l.707.707M13 10a3 3 0 11-6 0 3 3 0 016 0z" 
            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      
      <button 
        class="theme-toggle-btn"
        [class.active]="currentTheme === 'dark'"
        (click)="setTheme('dark')"
        [attr.aria-pressed]="currentTheme === 'dark'"
        aria-label="Modo noturno"
        title="Modo noturno">
        <svg width="18" height="18" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z" 
            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      
      <button 
        class="theme-toggle-btn"
        [class.active]="currentTheme === 'high-contrast'"
        (click)="setTheme('high-contrast')"
        [attr.aria-pressed]="currentTheme === 'high-contrast'"
        aria-label="Alto contraste"
        title="Alto contraste">
        <svg width="18" height="18" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M10 18a8 8 0 100-16 8 8 0 000 16zM10 2v16" 
            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
    </div>
  `,
  styles: [`
    .theme-toggle {
      display: flex;
      gap: 0.25rem;
      padding: 0.125rem;
      background: transparent;
      border-radius: var(--radius-md);
      border: 1px solid var(--gray-200);
    }

    .theme-toggle-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0.5rem;
      background: transparent;
      border: none;
      border-radius: var(--radius-sm);
      color: var(--gray-500);
      cursor: pointer;
      transition: all var(--transition-fast);
      min-width: 32px;
      min-height: 32px;
      
      svg {
        flex-shrink: 0;
      }
      
      &:hover {
        background: var(--gray-100);
        color: var(--gray-700);
      }
      
      &:focus-visible {
        outline: 2px solid var(--primary-500);
        outline-offset: 1px;
      }
      
      &.active {
        background: var(--primary-50);
        color: var(--primary-600);
      }
    }

    /* Dark mode styles */
    :host-context(.theme-dark) {
      .theme-toggle {
        background: transparent;
        border-color: var(--gray-700);
      }

      .theme-toggle-btn {
        color: var(--gray-400);

        &:hover {
          background: var(--gray-800);
          color: var(--gray-200);
        }

        &.active {
          background: var(--gray-800);
          color: var(--primary-400);
        }
      }
    }

    /* High contrast mode styles */
    :host-context(.theme-high-contrast) {
      .theme-toggle {
        background: #000;
        border: 3px solid #fff;
      }

      .theme-toggle-btn {
        color: #fff;
        border: 2px solid transparent;

        &:hover {
          background: #333;
        }

        &:focus-visible {
          outline: 3px solid #ff0;
          outline-offset: 2px;
        }

        &.active {
          background: #fff;
          color: #000;
          border-color: #fff;
        }
      }
    }
  `]
})
export class ThemeToggleComponent {
  constructor(private themeService: ThemeService) {}

  get currentTheme(): Theme {
    return this.themeService.getTheme();
  }

  setTheme(theme: Theme): void {
    this.themeService.setTheme(theme);
  }
}
