import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SubscriptionService } from '../../../services/subscription';
import { CartService } from '../../../services/cart';
import { SubscriptionPlan } from '../../../models/subscription-plan.model';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './pricing.html',
  styleUrl: './pricing.scss'
})
export class PricingComponent {
  private router = inject(Router);
  private subscriptionService = inject(SubscriptionService);
  protected cartService = inject(CartService);
  
  plans: SubscriptionPlan[] = [];
  whatsappNumber = environment.whatsappNumber;
  loading = true;

  ngOnInit(): void {
    this.subscriptionService.getPlans().subscribe({
      next: (plans) => {
        this.plans = plans;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading plans:', error);
        this.loading = false;
      }
    });
  }

  selectPlan(plan: SubscriptionPlan): void {
    if (plan.type === 4) { // Enterprise/Custom
      this.contactForCustomPlan();
    } else {
      this.cartService.addToCart(plan);
      this.router.navigate(['/site/register'], { queryParams: { plan: plan.id } });
    }
  }

  contactForCustomPlan(): void {
    window.open(`https://wa.me/${this.whatsappNumber}?text=Olá, gostaria de saber mais sobre o plano personalizado.`, '_blank');
  }
}
