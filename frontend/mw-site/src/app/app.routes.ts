import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { PricingComponent } from './pages/pricing/pricing';
import { ContactComponent } from './pages/contact/contact';
import { TestimonialsComponent } from './pages/testimonials/testimonials';
import { RegisterComponent } from './pages/register/register';
import { CartComponent } from './pages/cart/cart';
import { CheckoutComponent } from './pages/checkout/checkout';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'pricing', component: PricingComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'testimonials', component: TestimonialsComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'cart', component: CartComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: '**', redirectTo: '' }
];
