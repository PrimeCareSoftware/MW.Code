import { Injectable, signal } from '@angular/core';
import { Cart, CartItem } from '../models/cart-item.model';
import { SubscriptionPlan } from '../models/subscription-plan.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cart = signal<Cart>({ items: [], total: 0 });

  constructor() {
    this.loadCart();
  }

  getCart() {
    return this.cart;
  }

  addToCart(plan: SubscriptionPlan): void {
    const currentCart = this.cart();
    const existingItem = currentCart.items.find(item => item.plan.id === plan.id);

    if (existingItem) {
      existingItem.quantity++;
    } else {
      currentCart.items.push({
        plan,
        quantity: 1,
        addedAt: new Date()
      });
    }

    this.updateTotal();
    this.saveCart();
  }

  removeFromCart(planId: string): void {
    const currentCart = this.cart();
    currentCart.items = currentCart.items.filter(item => item.plan.id !== planId);
    this.updateTotal();
    this.saveCart();
  }

  clearCart(): void {
    this.cart.set({ items: [], total: 0 });
    this.saveCart();
  }

  private updateTotal(): void {
    const currentCart = this.cart();
    currentCart.total = currentCart.items.reduce((sum, item) => {
      return sum + (item.plan.monthlyPrice * item.quantity);
    }, 0);
    this.cart.set({ ...currentCart });
  }

  private saveCart(): void {
    localStorage.setItem('mw-cart', JSON.stringify(this.cart()));
  }

  private loadCart(): void {
    const savedCart = localStorage.getItem('mw-cart');
    if (savedCart) {
      try {
        const cart = JSON.parse(savedCart);
        // Convert date strings back to Date objects
        cart.items.forEach((item: CartItem) => {
          item.addedAt = new Date(item.addedAt);
        });
        this.cart.set(cart);
      } catch (e) {
        console.error('Failed to load cart from localStorage', e);
      }
    }
  }

  getItemCount(): number {
    return this.cart().items.reduce((sum, item) => sum + item.quantity, 0);
  }
}
