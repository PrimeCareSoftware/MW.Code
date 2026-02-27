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
        <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M9 2.7V1.8M9 16.2v-.9M15.3 9h.9M1.8 9h.9M14.091 3.909l.636-.636M3.273 14.727l.636-.636M14.091 14.091l.636.636M3.273 3.273l.636.636M11.7 9a2.7 2.7 0 11-5.4 0 2.7 2.7 0 015.4 0z" 
            stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      
      <button 
        class="theme-toggle-btn"
        [class.active]="currentTheme === 'dark'"
        (click)="setTheme('dark')"
        [attr.aria-pressed]="currentTheme === 'dark'"
        aria-label="Modo noturno"
        title="Modo noturno">
        <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M15.564 11.964A7.2 7.2 0 016.036 2.436a7.201 7.201 0 109.528 9.528z" 
            stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      
      <button 
        class="theme-toggle-btn"
        [class.active]="currentTheme === 'high-contrast'"
        (click)="setTheme('high-contrast')"
        [attr.aria-pressed]="currentTheme === 'high-contrast'"
        aria-label="Alto contraste"
        title="Alto contraste">
        <svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M9 16.2a7.2 7.2 0 100-14.4 7.2 7.2 0 000 14.4zM9 1.8v14.4" 
            stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
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
