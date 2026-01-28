import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent implements OnInit, OnDestroy, AfterViewInit {
  whatsappNumber = environment.whatsappNumber;
  stars = [1, 2, 3, 4, 5]; // Array for star rating (avoid creating new array on each change detection)
  private observer?: IntersectionObserver;
  
  // Video configuration
  // TODO: Replace with actual video URL when video is produced
  // Options: YouTube, Vimeo, or self-hosted MP4
  demoVideoUrl: string = ''; // Empty = show placeholder
  // Example YouTube: 'https://www.youtube.com/embed/VIDEO_ID?rel=0&modestbranding=1&cc_load_policy=1&cc_lang_pref=pt'
  // Example Vimeo: 'https://player.vimeo.com/video/VIDEO_ID'
  // Example Self-hosted: '/assets/videos/primecare-demo.mp4'
  
  get hasVideo(): boolean {
    return !!this.demoVideoUrl && this.demoVideoUrl.length > 0;
  }

  ngOnInit(): void {
    this.setupIntersectionObserver();
  }

  ngAfterViewInit(): void {
    // Setup observers after view is fully initialized to avoid missing elements
    this.observeElements();
  }

  ngOnDestroy(): void {
    if (this.observer) {
      this.observer.disconnect();
    }
  }

  openWhatsApp(): void {
    window.open(`https://wa.me/${this.whatsappNumber}`, '_blank');
  }

  private setupIntersectionObserver(): void {
    // Setup Intersection Observer for scroll animations
    const options = {
      root: null,
      rootMargin: '0px',
      threshold: 0.1
    };

    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          // Add visible class when element enters viewport
          // Note: We don't remove the class when leaving viewport (reveal-once behavior)
          entry.target.classList.add('visible');
        }
      });
    }, options);
  }

  private observeElements(): void {
    // Observe all elements with animate-on-scroll class
    const elements = document.querySelectorAll('.animate-on-scroll');
    elements.forEach(el => this.observer?.observe(el));
  }
}
