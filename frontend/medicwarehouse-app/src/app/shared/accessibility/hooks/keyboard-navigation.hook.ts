/**
 * Hook de Navegação por Teclado
 * Fornece suporte completo para navegação via teclado conforme WCAG 2.1
 */

import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

export interface KeyboardNavigationOptions {
  onEnter?: () => void;
  onEscape?: () => void;
  onArrowUp?: () => void;
  onArrowDown?: () => void;
  onArrowLeft?: () => void;
  onArrowRight?: () => void;
  onTab?: (event: KeyboardEvent) => void;
  onSpace?: () => void;
}

@Injectable()
export class KeyboardNavigationService implements OnDestroy {
  private destroy$ = new Subject<void>();
  private handlers: Map<string, (event: KeyboardEvent) => void> = new Map();

  /**
   * Registra handlers de teclado
   * @param element Elemento HTML para anexar os event listeners
   * @param options Opções de navegação
   */
  registerHandlers(element: HTMLElement, options: KeyboardNavigationOptions): void {
    const handleKeyDown = (event: KeyboardEvent) => {
      switch (event.key) {
        case 'Enter':
          if (options.onEnter) {
            event.preventDefault();
            options.onEnter();
          }
          break;
        case 'Escape':
          if (options.onEscape) {
            event.preventDefault();
            options.onEscape();
          }
          break;
        case 'ArrowUp':
          if (options.onArrowUp) {
            event.preventDefault();
            options.onArrowUp();
          }
          break;
        case 'ArrowDown':
          if (options.onArrowDown) {
            event.preventDefault();
            options.onArrowDown();
          }
          break;
        case 'ArrowLeft':
          if (options.onArrowLeft) {
            event.preventDefault();
            options.onArrowLeft();
          }
          break;
        case 'ArrowRight':
          if (options.onArrowRight) {
            event.preventDefault();
            options.onArrowRight();
          }
          break;
        case 'Tab':
          if (options.onTab) {
            options.onTab(event);
          }
          break;
        case ' ':
        case 'Spacebar':
          if (options.onSpace) {
            event.preventDefault();
            options.onSpace();
          }
          break;
      }
    };

    element.addEventListener('keydown', handleKeyDown);
    this.handlers.set(element.id || element.tagName, handleKeyDown);
  }

  /**
   * Remove handlers de teclado
   * @param element Elemento HTML
   */
  unregisterHandlers(element: HTMLElement): void {
    const key = element.id || element.tagName;
    const handler = this.handlers.get(key);
    
    if (handler) {
      element.removeEventListener('keydown', handler);
      this.handlers.delete(key);
    }
  }

  /**
   * Foca no primeiro elemento focável dentro de um container
   * @param container Container HTML
   */
  focusFirstElement(container: HTMLElement): void {
    const focusableElements = this.getFocusableElements(container);
    if (focusableElements.length > 0) {
      (focusableElements[0] as HTMLElement).focus();
    }
  }

  /**
   * Foca no último elemento focável dentro de um container
   * @param container Container HTML
   */
  focusLastElement(container: HTMLElement): void {
    const focusableElements = this.getFocusableElements(container);
    if (focusableElements.length > 0) {
      (focusableElements[focusableElements.length - 1] as HTMLElement).focus();
    }
  }

  /**
   * Obtém todos os elementos focáveis dentro de um container
   * @param container Container HTML
   */
  getFocusableElements(container: HTMLElement): Element[] {
    const focusableSelectors = [
      'a[href]',
      'button:not([disabled])',
      'textarea:not([disabled])',
      'input:not([disabled])',
      'select:not([disabled])',
      '[tabindex]:not([tabindex="-1"])'
    ].join(', ');

    return Array.from(container.querySelectorAll(focusableSelectors));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    
    // Limpar todos os handlers
    this.handlers.clear();
  }
}
