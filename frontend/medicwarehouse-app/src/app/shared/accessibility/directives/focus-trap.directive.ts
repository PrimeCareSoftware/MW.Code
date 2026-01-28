/**
 * Diretiva de Trap de Foco para Modais
 * Mantém o foco dentro de um modal conforme WCAG 2.1
 */

import { Directive, ElementRef, AfterViewInit, OnDestroy, HostListener } from '@angular/core';

@Directive({
  selector: '[appFocusTrap]',
  standalone: true
})
export class FocusTrapDirective implements AfterViewInit, OnDestroy {
  private firstFocusableElement?: HTMLElement;
  private lastFocusableElement?: HTMLElement;
  private previousActiveElement?: HTMLElement;

  constructor(private elementRef: ElementRef<HTMLElement>) {}

  ngAfterViewInit(): void {
    // Salvar elemento ativo atual
    this.previousActiveElement = document.activeElement as HTMLElement;

    // Atualizar elementos focáveis
    this.updateFocusableElements();

    // Focar no primeiro elemento
    setTimeout(() => {
      this.firstFocusableElement?.focus();
    }, 100);
  }

  ngOnDestroy(): void {
    // Restaurar foco ao elemento anterior
    if (this.previousActiveElement) {
      this.previousActiveElement.focus();
    }
  }

  /**
   * Captura eventos de teclado para manter o foco no modal
   */
  @HostListener('keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent): void {
    if (event.key !== 'Tab') {
      return;
    }

    // Atualizar lista de elementos focáveis
    this.updateFocusableElements();

    if (!this.firstFocusableElement || !this.lastFocusableElement) {
      return;
    }

    // Tab + Shift - volta para o último elemento
    if (event.shiftKey) {
      if (document.activeElement === this.firstFocusableElement) {
        event.preventDefault();
        this.lastFocusableElement.focus();
      }
    } 
    // Tab - avança para o primeiro elemento
    else {
      if (document.activeElement === this.lastFocusableElement) {
        event.preventDefault();
        this.firstFocusableElement.focus();
      }
    }
  }

  /**
   * Atualiza a lista de elementos focáveis
   */
  private updateFocusableElements(): void {
    const focusableSelectors = [
      'a[href]:not([disabled])',
      'button:not([disabled])',
      'textarea:not([disabled])',
      'input:not([disabled])',
      'select:not([disabled])',
      '[tabindex]:not([tabindex="-1"]):not([disabled])'
    ].join(', ');

    const focusableElements = Array.from(
      this.elementRef.nativeElement.querySelectorAll<HTMLElement>(focusableSelectors)
    ).filter(element => {
      // Check if element is visible using getComputedStyle
      const style = window.getComputedStyle(element);
      return style.display !== 'none' && 
             style.visibility !== 'hidden' && 
             element.offsetWidth > 0 && 
             element.offsetHeight > 0;
    });

    if (focusableElements.length > 0) {
      this.firstFocusableElement = focusableElements[0];
      this.lastFocusableElement = focusableElements[focusableElements.length - 1];
    }
  }

  /**
   * Foca no primeiro elemento focável
   */
  focusFirst(): void {
    this.updateFocusableElements();
    this.firstFocusableElement?.focus();
  }

  /**
   * Foca no último elemento focável
   */
  focusLast(): void {
    this.updateFocusableElements();
    this.lastFocusableElement?.focus();
  }
}
