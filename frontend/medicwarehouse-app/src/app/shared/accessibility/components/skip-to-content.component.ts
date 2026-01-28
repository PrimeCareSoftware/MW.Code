/**
 * Componente Skip to Content
 * Permite usuários de teclado/leitores de tela pular navegação
 */

import { Component } from '@angular/core';

@Component({
  selector: 'app-skip-to-content',
  standalone: true,
  template: `
    <a 
      href="#main-content" 
      class="skip-to-content"
      (click)="skipToContent($event)"
      [attr.aria-label]="'Pular para o conteúdo principal'"
    >
      Pular para o conteúdo principal
    </a>
  `,
  styles: [`
    .skip-to-content {
      position: absolute;
      top: -40px;
      left: 0;
      z-index: 10000;
      padding: 8px 16px;
      background: #1976d2;
      color: white;
      text-decoration: none;
      border-radius: 0 0 4px 0;
      font-weight: 600;
      font-size: 14px;
      transition: top 0.2s ease;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    }

    .skip-to-content:focus {
      top: 0;
      outline: 3px solid #ffc107;
      outline-offset: 2px;
    }

    .skip-to-content:hover {
      background: #1565c0;
    }
  `]
})
export class SkipToContentComponent {
  skipToContent(event: Event): void {
    event.preventDefault();
    
    const mainContent = document.getElementById('main-content');
    if (mainContent) {
      // Adicionar tabindex temporário se necessário
      if (!mainContent.hasAttribute('tabindex')) {
        mainContent.setAttribute('tabindex', '-1');
      }
      
      mainContent.focus();
      
      // Respeitar prefers-reduced-motion
      const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
      mainContent.scrollIntoView({ 
        behavior: prefersReducedMotion ? 'auto' : 'smooth', 
        block: 'start' 
      });
    }
  }
}
