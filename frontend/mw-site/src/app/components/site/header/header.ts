import { Component, inject, HostListener, ElementRef } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../../services/cart';
import { CommonModule } from '@angular/common';
import { ThemeToggleComponent } from '../../../shared/theme-toggle/theme-toggle.component';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, RouterLinkActive, ThemeToggleComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  protected cartService = inject(CartService);
  private elementRef = inject(ElementRef);
  protected isMenuOpen = false;
  protected isLoginDropdownOpen = false;

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  toggleLoginDropdown(): void {
    this.isLoginDropdownOpen = !this.isLoginDropdownOpen;
  }

  closeLoginDropdown(): void {
    this.isLoginDropdownOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside && this.isLoginDropdownOpen) {
      this.isLoginDropdownOpen = false;
    }
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.isLoginDropdownOpen) {
      this.isLoginDropdownOpen = false;
    }
  }
}
