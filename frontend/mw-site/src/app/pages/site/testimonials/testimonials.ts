import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SAMPLE_TESTIMONIALS, Testimonial } from '../../../models/testimonial.model';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-testimonials',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './testimonials.html',
  styleUrl: './testimonials.scss'
})
export class TestimonialsComponent {
  testimonials: Testimonial[] = SAMPLE_TESTIMONIALS;

  getStars(rating: number): string[] {
    return Array(rating).fill('⭐');
  }
}
