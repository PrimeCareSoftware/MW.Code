import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { SubscriptionService } from '../../services/subscription';
import { CartService } from '../../services/cart';
import { SubscriptionPlan } from '../../models/subscription-plan.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, RouterLink],
  templateUrl: './pricing.html',
  styleUrl: './pricing.scss'
})
export class PricingComponent {
  private router = inject(Router);
  private subscriptionService = inject(SubscriptionService);
  protected cartService = inject(CartService);
  
  plans: SubscriptionPlan[] = [];
  whatsappNumber = environment.whatsappNumber;

  ngOnInit(): void {
    this.plans = this.subscriptionService.getPlans();
  }

  selectPlan(plan: SubscriptionPlan): void {
    if (plan.type === 4) { // Enterprise/Custom
      this.contactForCustomPlan();
    } else {
      this.cartService.addToCart(plan);
      this.router.navigate(['/register'], { queryParams: { plan: plan.id } });
    }
  }

  contactForCustomPlan(): void {
    window.open(`https://wa.me/${this.whatsappNumber}?text=Olá, gostaria de saber mais sobre o plano personalizado.`, '_blank');
  }
}
