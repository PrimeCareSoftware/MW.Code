import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appDateMask]',
  standalone: true
})
export class DateMaskDirective {
  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    // Skip if input type is already 'date' (browser handles it)
    if (input.type === 'date') {
      return;
    }
    
    let value = input.value.replace(/\D/g, '');
    
    // Limit to 8 digits (DDMMYYYY)
    if (value.length > 8) {
      value = value.substring(0, 8);
    }
    
    // Apply date mask: DD/MM/YYYY
    if (value.length <= 2) {
      input.value = value;
    } else if (value.length <= 4) {
      input.value = value.replace(/^(\d{2})(\d{0,2})/, '$1/$2');
    } else {
      input.value = value.replace(/^(\d{2})(\d{2})(\d{0,4})/, '$1/$2/$3');
    }
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    // Skip if input type is already 'date'
    if (input.type === 'date') {
      return;
    }
    
    let value = input.value.replace(/\D/g, '');
    
    // Only format if we have the complete date
    if (value.length === 8) {
      input.value = value.replace(/^(\d{2})(\d{2})(\d{4})$/, '$1/$2/$3');
    }
  }
}
