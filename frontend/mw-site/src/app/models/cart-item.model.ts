import { SubscriptionPlan } from './subscription-plan.model';

export interface CartItem {
  plan: SubscriptionPlan;
  quantity: number;
  addedAt: Date;
}

export interface Cart {
  items: CartItem[];
  total: number;
}
