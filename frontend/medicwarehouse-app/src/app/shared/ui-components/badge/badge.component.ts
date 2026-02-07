import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Badge Component - Migrated from clinic-harmony-ui-main
 * 
 * Displays a badge for labels, counts, or status indicators.
 * 
 * @example
 * <app-badge>Default</app-badge>
 * <app-badge variant="secondary">Secondary</app-badge>
 * <app-badge variant="destructive">Error</app-badge>
 * <app-badge variant="outline">Outline</app-badge>
 */
@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getBadgeClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './badge.component.scss'
})
export class BadgeComponent {
  /**
   * Badge variant style
   */
  @Input() variant: 'default' | 'secondary' | 'destructive' | 'outline' = 'default';
  
  /**
   * Additional CSS classes
   */
  @Input() className?: string;
  
  getBadgeClasses(): string {
    const baseClasses = 'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2';
    
    const variantClasses = {
      default: 'border-transparent bg-primary text-primary-foreground hover:bg-primary-hover',
      secondary: 'border-transparent bg-secondary text-secondary-foreground hover:bg-secondary-hover',
      destructive: 'border-transparent bg-destructive text-destructive-foreground hover:bg-destructive-hover',
      outline: 'text-foreground border-border'
    };
    
    return `${baseClasses} ${variantClasses[this.variant]} ${this.className || ''}`.trim();
  }
}
