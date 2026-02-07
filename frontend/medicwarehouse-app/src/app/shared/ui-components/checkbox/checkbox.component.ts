import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';

/**
 * Checkbox Component - Migrated from clinic-harmony-ui-main
 * 
 * A checkbox input component with support for Angular forms.
 * 
 * @example
 * <app-checkbox [(ngModel)]="isChecked" label="Accept terms"></app-checkbox>
 * <app-checkbox [checked]="isChecked" (checkedChange)="onCheckedChange($event)"></app-checkbox>
 */
@Component({
  selector: 'app-checkbox',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="checkbox-wrapper">
      <input
        type="checkbox"
        [id]="id"
        [checked]="checked"
        [disabled]="disabled"
        [class]="getCheckboxClasses()"
        (change)="onCheckboxChange($event)"
        [attr.aria-label]="ariaLabel"
      />
      <label *ngIf="label" [for]="id" class="checkbox-label">
        {{ label }}
      </label>
      <svg
        *ngIf="checked"
        class="check-icon"
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round">
        <polyline points="20 6 9 17 4 12"></polyline>
      </svg>
    </div>
  `,
  styleUrl: './checkbox.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxComponent),
      multi: true
    }
  ]
})
export class CheckboxComponent implements ControlValueAccessor {
  @Input() id: string = `checkbox-${Math.random().toString(36).substr(2, 9)}`;
  @Input() disabled: boolean = false;
  @Input() className?: string;
  @Input() ariaLabel?: string;
  @Input() label?: string;
  @Input() checked: boolean = false;
  
  @Output() checkedChange = new EventEmitter<boolean>();
  
  // ControlValueAccessor implementation
  onChange: any = () => {};
  onTouched: any = () => {};
  
  writeValue(value: boolean): void {
    this.checked = value;
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
  
  onCheckboxChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.checked = target.checked;
    this.onChange(this.checked);
    this.checkedChange.emit(this.checked);
    this.onTouched();
  }
  
  getCheckboxClasses(): string {
    const baseClasses = 'peer h-4 w-4 shrink-0 rounded-sm border border-primary ring-offset-background checked:bg-primary checked:text-primary-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50';
    return `${baseClasses} ${this.className || ''}`.trim();
  }
}
