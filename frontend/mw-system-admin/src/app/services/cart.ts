import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private items: any[] = [];

  getItems(): any[] {
    return this.items;
  }

  addItem(item: any): void {
    this.items.push(item);
  }

  removeItem(id: string): void {
    this.items = this.items.filter(item => item.id !== id);
  }

  getItemCount(): number {
    return this.items.length;
  }

  clear(): void {
    this.items = [];
  }
}
