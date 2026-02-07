import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Alert Component - Migrated from clinic-harmony-ui-main
 * 
 * Displays a callout for user attention.
 * 
 * @example
 * <app-alert>
 *   <app-alert-title>Heads up!</app-alert-title>
 *   <app-alert-description>
 *     You can add components to your app using the cli.
 *   </app-alert-description>
 * </app-alert>
 */
@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div role="alert" [class]="getAlertClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './alert.component.scss'
})
export class AlertComponent {
  @Input() variant: 'default' | 'destructive' = 'default';
  @Input() className?: string;
  
  getAlertClasses(): string {
    const baseClasses = 'relative w-full rounded-lg border p-4';
    const variantClasses = {
      default: 'bg-background text-foreground',
      destructive: 'border-destructive-light text-destructive'
    };
    return `${baseClasses} ${variantClasses[this.variant]} ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-alert-title',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h5 [class]="getTitleClasses()">
      <ng-content></ng-content>
    </h5>
  `,
  styleUrl: './alert.component.scss'
})
export class AlertTitleComponent {
  @Input() className?: string;
  
  getTitleClasses(): string {
    const baseClasses = 'mb-1 font-medium leading-none tracking-tight';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-alert-description',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getDescriptionClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './alert.component.scss'
})
export class AlertDescriptionComponent {
  @Input() className?: string;
  
  getDescriptionClasses(): string {
    const baseClasses = 'text-sm';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
