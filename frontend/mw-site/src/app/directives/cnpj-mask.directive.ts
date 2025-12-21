import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appCnpjMask]',
  standalone: true
})
export class CnpjMaskDirective {
  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Limit to 14 digits
    if (value.length > 14) {
      value = value.substring(0, 14);
    }
    
    // Apply CNPJ mask: 00.000.000/0000-00
    if (value.length <= 2) {
      input.value = value;
    } else if (value.length <= 5) {
      input.value = value.replace(/^(\d{2})(\d{0,3})/, '$1.$2');
    } else if (value.length <= 8) {
      input.value = value.replace(/^(\d{2})(\d{3})(\d{0,3})/, '$1.$2.$3');
    } else if (value.length <= 12) {
      input.value = value.replace(/^(\d{2})(\d{3})(\d{3})(\d{0,4})/, '$1.$2.$3/$4');
    } else {
      input.value = value.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{0,2})/, '$1.$2.$3/$4-$5');
    }
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Only format if we have the complete CNPJ
    if (value.length === 14) {
      input.value = value.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
    }
  }
}
