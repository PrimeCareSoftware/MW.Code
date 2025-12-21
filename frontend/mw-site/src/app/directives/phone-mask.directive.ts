import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appPhoneMask]',
  standalone: true
})
export class PhoneMaskDirective {
  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Limit to 11 digits (with area code)
    if (value.length > 11) {
      value = value.substring(0, 11);
    }
    
    // Apply phone mask: (00) 00000-0000 or (00) 0000-0000
    if (value.length <= 2) {
      input.value = value;
    } else if (value.length <= 6) {
      input.value = value.replace(/^(\d{2})(\d{1,4})/, '($1) $2');
    } else if (value.length <= 10) {
      input.value = value.replace(/^(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3');
    } else {
      // For 11 digits (9 in the front), use format (00) 00000-0000
      input.value = value.replace(/^(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
    }
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Format based on length
    if (value.length === 10) {
      // Landline: (00) 0000-0000
      input.value = value.replace(/^(\d{2})(\d{4})(\d{4})$/, '($1) $2-$3');
    } else if (value.length === 11) {
      // Mobile: (00) 00000-0000
      input.value = value.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3');
    }
  }
}
