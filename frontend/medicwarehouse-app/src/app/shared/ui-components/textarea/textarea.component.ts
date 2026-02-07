import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';

/**
 * Textarea Component - Migrated from clinic-harmony-ui-main
 * 
 * A multiline text input component.
 * 
 * @example
 * <app-textarea placeholder="Enter description" [(ngModel)]="description"></app-textarea>
 * <app-textarea rows="5" placeholder="Enter notes" [(ngModel)]="notes"></app-textarea>
 */
@Component({
  selector: 'app-textarea',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <textarea
      [placeholder]="placeholder"
      [disabled]="disabled"
      [rows]="rows"
      [class]="getTextareaClasses()"
      [value]="value"
      (input)="onInput($event)"
      (blur)="onTouched()"
      [attr.aria-label]="ariaLabel"
    ></textarea>
  `,
  styleUrl: './textarea.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextareaComponent),
      multi: true
    }
  ]
})
export class TextareaComponent implements ControlValueAccessor {
  @Input() placeholder?: string;
  @Input() disabled: boolean = false;
  @Input() rows: number = 3;
  @Input() className?: string;
  @Input() ariaLabel?: string;
  
  @Output() textareaChange = new EventEmitter<string>();
  
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
    const target = event.target as HTMLTextAreaElement;
    this.value = target.value;
    this.onChange(this.value);
    this.textareaChange.emit(this.value);
  }
  
  getTextareaClasses(): string {
    const baseClasses = 'flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
