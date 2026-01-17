import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../../services/cart';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class CartComponent {
  private router = inject(Router);
  protected cartService = inject(CartService);

  removeItem(planId: string): void {
    this.cartService.removeFromCart(planId);
  }

  proceedToCheckout(): void {
    const cart = this.cartService.getCart()();
    if (cart.items.length > 0) {
      this.router.navigate(['/site/register'], { 
        queryParams: { plan: cart.items[0].plan.id } 
      });
    }
  }
}
