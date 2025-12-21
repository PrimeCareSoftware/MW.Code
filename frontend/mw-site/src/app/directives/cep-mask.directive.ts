import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appCepMask]',
  standalone: true
})
export class CepMaskDirective {
  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Limit to 8 digits
    if (value.length > 8) {
      value = value.substring(0, 8);
    }
    
    // Apply CEP mask: 00000-000
    if (value.length <= 5) {
      input.value = value;
    } else {
      input.value = value.replace(/^(\d{5})(\d{0,3})/, '$1-$2');
    }
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // Only format if we have the complete CEP
    if (value.length === 8) {
      input.value = value.replace(/^(\d{5})(\d{3})$/, '$1-$2');
    }
  }
}
