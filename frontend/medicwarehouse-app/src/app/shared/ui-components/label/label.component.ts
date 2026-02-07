import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Label Component - Migrated from clinic-harmony-ui-main
 * 
 * A label element for form controls.
 * 
 * @example
 * <app-label for="email">Email Address</app-label>
 * <app-input id="email" type="email" />
 */
@Component({
  selector: 'app-label',
  standalone: true,
  imports: [CommonModule],
  template: `
    <label [for]="for" [class]="getLabelClasses()">
      <ng-content></ng-content>
    </label>
  `,
  styleUrl: './label.component.scss'
})
export class LabelComponent {
  /**
   * The id of the form control this label is for
   */
  @Input() for?: string;
  
  /**
   * Additional CSS classes
   */
  @Input() className?: string;
  
  getLabelClasses(): string {
    const baseClasses = 'text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
