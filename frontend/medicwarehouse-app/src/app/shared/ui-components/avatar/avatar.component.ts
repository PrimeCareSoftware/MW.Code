import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Avatar Component - Migrated from clinic-harmony-ui-main
 * 
 * An image element with a fallback for representing the user.
 * 
 * @example
 * <app-avatar>
 *   <app-avatar-image src="https://..." alt="User" />
 *   <app-avatar-fallback>JD</app-avatar-fallback>
 * </app-avatar>
 */
@Component({
  selector: 'app-avatar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getAvatarClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './avatar.component.scss'
})
export class AvatarComponent {
  @Input() className?: string;
  
  getAvatarClasses(): string {
    const baseClasses = 'relative flex h-10 w-10 shrink-0 overflow-hidden rounded-full';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}

@Component({
  selector: 'app-avatar-image',
  standalone: true,
  imports: [CommonModule],
  template: `
    <img 
      [src]="src" 
      [alt]="alt"
      [class]="getImageClasses()"
      (error)="onImageError()"
    />
  `,
  styleUrl: './avatar.component.scss'
})
export class AvatarImageComponent {
  @Input() src?: string;
  @Input() alt?: string;
  @Input() className?: string;
  
  imageError = false;
  
  getImageClasses(): string {
    const baseClasses = 'aspect-square h-full w-full';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
  
  onImageError(): void {
    this.imageError = true;
  }
}

@Component({
  selector: 'app-avatar-fallback',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getFallbackClasses()">
      <ng-content></ng-content>
    </div>
  `,
  styleUrl: './avatar.component.scss'
})
export class AvatarFallbackComponent {
  @Input() className?: string;
  
  getFallbackClasses(): string {
    const baseClasses = 'flex h-full w-full items-center justify-center rounded-full bg-muted';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
