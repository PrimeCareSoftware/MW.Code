import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Card Component - Migrated from clinic-harmony-ui-main
 * 
 * A container component for displaying content in a card layout.
 * 
 * @example
 * <app-card>
 *   <app-card-header>
 *     <app-card-title>Card Title</app-card-title>
 *     <app-card-description>Card description</app-card-description>
 *   </app-card-header>
 *   <app-card-content>
 *     Content goes here
 *   </app-card-content>
 *   <app-card-footer>
 *     Footer content
 *   </app-card-footer>
 * </app-card>
 */
@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getCardClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './card.component.scss'
})
export class CardComponent {
  @Input() className?: string;
  
  getCardClasses(): string {
    return `card rounded-lg border bg-card text-card-foreground shadow-sm ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-card-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './card.component.scss'
})
export class CardHeaderComponent {
  @Input() className?: string;
  
  getClasses(): string {
    return `card-header flex flex-col space-y-1.5 p-6 ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-card-title',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h3 [class]="getClasses()">
      <ng-content></ng-content>
    </h3>
  `,
  styleUrl: './card.component.scss'
})
export class CardTitleComponent {
  @Input() className?: string;
  
  getClasses(): string {
    return `card-title text-2xl font-semibold leading-none tracking-tight ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-card-description',
  standalone: true,
  imports: [CommonModule],
  template: `
    <p [class]="getClasses()">
      <ng-content></ng-content>
    </p>
  `,
  styleUrl: './card.component.scss'
})
export class CardDescriptionComponent {
  @Input() className?: string;
  
  getClasses(): string {
    return `card-description text-sm text-muted-foreground ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-card-content',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './card.component.scss'
})
export class CardContentComponent {
  @Input() className?: string;
  
  getClasses(): string {
    return `card-content p-6 pt-0 ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-card-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './card.component.scss'
})
export class CardFooterComponent {
  @Input() className?: string;
  
  getClasses(): string {
    return `card-footer flex items-center p-6 pt-0 ${this.className || ''}`.trim();
  }
}
