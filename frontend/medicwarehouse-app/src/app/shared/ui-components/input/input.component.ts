import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';

/**
 * Input Component - Migrated from clinic-harmony-ui-main
 * 
 * A text input field component with support for various types.
 * 
 * @example
 * <app-input type="text" placeholder="Enter name" [(ngModel)]="name"></app-input>
 * <app-input type="email" placeholder="Enter email" [(ngModel)]="email"></app-input>
 */
@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <input
      [type]="type"
      [placeholder]="placeholder"
      [disabled]="disabled"
      [class]="getInputClasses()"
      [value]="value"
      (input)="onInput($event)"
      (blur)="onTouched()"
      [attr.aria-label]="ariaLabel"
    />
  `,
  styleUrl: './input.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    }
  ]
})
export class InputComponent implements ControlValueAccessor {
  @Input() type: string = 'text';
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() className?: string;
  @Input() ariaLabel?: string;
  
  @Output() inputChange = new EventEmitter<string>();
  
  value: string = '';
  
  // ControlValueAccessor implementation
  onChange: any = () => {};
  onTouched: any = () => {};
  
  writeValue(value: string): void {
    this.value = value || '';
  }
  
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  
  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
  
  onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value = target.value;
    this.onChange(this.value);
    this.inputChange.emit(this.value);
  }
  
  getInputClasses(): string {
    const baseClasses = 'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 md:text-sm';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
