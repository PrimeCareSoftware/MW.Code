import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Separator Component - Migrated from clinic-harmony-ui-main
 * 
 * Visually or semantically separates content.
 * 
 * @example
 * <app-separator></app-separator>
 * <app-separator orientation="vertical"></app-separator>
 */
@Component({
  selector: 'app-separator',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      role="separator"
      [attr.aria-orientation]="orientation"
      [class]="getSeparatorClasses()"
    ></div>
  `,
  styleUrl: './separator.component.scss'
})
export class SeparatorComponent {
  @Input() orientation: 'horizontal' | 'vertical' = 'horizontal';
  @Input() decorative: boolean = true;
  @Input() className?: string;
  
  getSeparatorClasses(): string {
    const baseClasses = 'shrink-0 bg-border';
    const orientationClasses = this.orientation === 'horizontal' 
      ? 'h-[1px] w-full' 
      : 'h-full w-[1px]';
    return `${baseClasses} ${orientationClasses} ${this.className || ''}`.trim();
  }
}
