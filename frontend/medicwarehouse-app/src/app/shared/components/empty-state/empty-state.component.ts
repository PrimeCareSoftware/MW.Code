import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

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
  styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent {
  /**
   * Icon to display (uses Material Icons or inline SVG)
   */
  @Input() icon?: string;
  
  /**
   * Custom SVG to display instead of icon
   */
  @Input() customSvg?: string;
  
  /**
   * Main title (h3)
   */
  @Input() title!: string;
  
  /**
   * Description text
   */
  @Input() description!: string;
  
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
