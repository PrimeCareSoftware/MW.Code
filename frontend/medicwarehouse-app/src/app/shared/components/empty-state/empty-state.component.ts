import { Component, Input, Output, EventEmitter, OnInit, SecurityContext } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

/**
 * EmptyState Component
 * 
 * Reusable component for displaying empty states across the application.
 * Follows PROMPT 6 guidelines from PROMPTS_IMPLEMENTACAO_DETALHADOS.md
 * 
 * @example
 * <app-empty-state
 *   icon="users"
 *   title="Nenhum paciente cadastrado"
 *   description="Adicione seu primeiro paciente para começar a usar o sistema."
 *   primaryButtonText="Adicionar Primeiro Paciente"
 *   (primaryButtonClick)="onAddPatient()"
 *   secondaryLinkText="Como adicionar pacientes?"
 *   secondaryLinkHref="/help/adding-patients">
 * </app-empty-state>
 */
@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss'
})
export class EmptyStateComponent implements OnInit {
  /**
   * Icon to display (uses Material Icons or inline SVG)
   */
  @Input() icon?: string;
  
  /**
   * Custom SVG to display instead of icon (will be sanitized)
   */
  @Input() customSvg?: string;
  
  /**
   * Main title (h3) - defaults to generic message
   */
  @Input() title: string = 'Nenhum item encontrado';
  
  /**
   * Description text - defaults to generic message
   */
  @Input() description: string = 'Não há itens para exibir no momento.';
  
  /**
   * Primary button text
   */
  @Input() primaryButtonText?: string;
  
  /**
   * Primary button route (if using router navigation)
   */
  @Input() primaryButtonRoute?: string;
  
  /**
   * Secondary link text
   */
  @Input() secondaryLinkText?: string;
  
  /**
   * Secondary link href
   */
  @Input() secondaryLinkHref?: string;
  
  /**
   * List of suggestions (for search empty states)
   */
  @Input() suggestions?: string[];
  
  /**
   * Additional CSS class
   */
  @Input() cssClass?: string;
  
  /**
   * Emitted when primary button is clicked
   */
  @Output() primaryButtonClick = new EventEmitter<void>();
  
  /**
   * Emitted when secondary link is clicked
   */
  @Output() secondaryLinkClick = new EventEmitter<void>();
  
  /**
   * Sanitized SVG content
   */
  sanitizedSvg?: SafeHtml;
  
  constructor(private sanitizer: DomSanitizer) {}
  
  ngOnInit(): void {
    // Sanitize custom SVG to prevent XSS
    if (this.customSvg) {
      this.sanitizedSvg = this.sanitizer.sanitize(SecurityContext.HTML, this.customSvg);
    }
  }
  
  onPrimaryButtonClick(): void {
    this.primaryButtonClick.emit();
  }
  
  onSecondaryLinkClick(event: Event): void {
    if (!this.secondaryLinkHref) {
      event.preventDefault();
      this.secondaryLinkClick.emit();
    }
  }
}
