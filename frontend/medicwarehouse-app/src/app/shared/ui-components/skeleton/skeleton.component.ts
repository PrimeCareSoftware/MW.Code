import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Skeleton Component - Migrated from clinic-harmony-ui-main
 * 
 * A placeholder component for loading states.
 * 
 * @example
 * <app-skeleton class="w-full h-12"></app-skeleton>
 * <app-skeleton class="w-[100px] h-[20px] rounded-full"></app-skeleton>
 */
@Component({
  selector: 'app-skeleton',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="getSkeletonClasses()"></div>
  `,
  styleUrl: './skeleton.component.scss'
})
export class SkeletonComponent {
  @Input() className?: string;
  
  getSkeletonClasses(): string {
    const baseClasses = 'animate-pulse rounded-md bg-muted';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
