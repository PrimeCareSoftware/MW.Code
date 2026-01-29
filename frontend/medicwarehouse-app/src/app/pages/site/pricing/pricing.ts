import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SubscriptionService } from '../../../services/subscription';
import { CartService } from '../../../services/cart';
import { SubscriptionPlan } from '../../../models/subscription-plan.model';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './pricing.html',
  styleUrl: './pricing.scss'
})
export class PricingComponent {
  private router = inject(Router);
  private subscriptionService = inject(SubscriptionService);
  private analytics = inject(WebsiteAnalyticsService);
  protected cartService = inject(CartService);
  
  plans: SubscriptionPlan[] = [];
  whatsappNumber = environment.whatsappNumber;
  loading = true;

  ngOnInit(): void {
    // Track page view
    this.analytics.trackPageView('/site/pricing', 'Planos e Preços - PrimeCare');

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
    // Track pricing plan view
    this.analytics.trackPricingPlanView(plan.name);

    if (plan.type === 4) { // Enterprise/Custom
      this.contactForCustomPlan();
    } else {
      this.cartService.addToCart(plan);
      this.analytics.trackConversion('trial_signup');
      this.router.navigate(['/site/register'], { queryParams: { plan: plan.id } });
    }
  }

  contactForCustomPlan(): void {
    this.analytics.trackCTAClick('Contato Plano Personalizado', 'Pricing Page');
    window.open(`https://wa.me/${this.whatsappNumber}?text=Olá, gostaria de saber mais sobre o plano personalizado.`, '_blank');
  }
}
