import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

/**
 * Button Component - Migrated from clinic-harmony-ui-main
 * 
 * Displays a button or a component that looks like a button.
 * 
 * @example
 * <app-button>Click me</app-button>
 * <app-button variant="destructive">Delete</app-button>
 * <app-button variant="outline" size="sm">Small Button</app-button>
 */
@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './button.component.html',
  styleUrl: './button.component.scss'
})
export class ButtonComponent {
  /**
   * Button variant style
   */
  @Input() variant: 'default' | 'destructive' | 'outline' | 'secondary' | 'ghost' | 'link' = 'default';
  
  /**
   * Button size
   */
  @Input() size: 'default' | 'sm' | 'lg' | 'icon' = 'default';
  
  /**
   * Button type attribute
   */
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  
  /**
   * Disabled state
   */
  @Input() disabled: boolean = false;
  
  /**
   * Use as router link
   */
  @Input() routerLink?: string | any[];
  
  /**
   * Additional CSS classes
   */
  @Input() className?: string;
  
  /**
   * Click event emitter
   */
  @Output() buttonClick = new EventEmitter<MouseEvent>();
  
  /**
   * Get button classes based on variant and size
   */
  getButtonClasses(): string {
    const baseClasses = 'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50';
    
    const variantClasses = {
      default: 'bg-primary text-primary-foreground hover:bg-primary-hover',
      destructive: 'bg-destructive text-destructive-foreground hover:bg-destructive-hover',
      outline: 'border border-input bg-background hover:bg-accent hover:text-accent-foreground',
      secondary: 'bg-secondary text-secondary-foreground hover:bg-secondary-hover',
      ghost: 'hover:bg-accent hover:text-accent-foreground',
      link: 'text-primary underline-offset-4 hover:underline'
    };
    
    const sizeClasses = {
      default: 'h-10 px-4 py-2',
      sm: 'h-9 rounded-md px-3',
      lg: 'h-11 rounded-md px-8',
      icon: 'h-10 w-10'
    };
    
    return `${baseClasses} ${variantClasses[this.variant]} ${sizeClasses[this.size]} ${this.className || ''}`.trim();
  }
  
  onClick(event: MouseEvent): void {
    if (!this.disabled) {
      this.buttonClick.emit(event);
    }
  }
}
