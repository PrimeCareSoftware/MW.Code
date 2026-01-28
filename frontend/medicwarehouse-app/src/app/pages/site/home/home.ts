import { Component, OnInit, OnDestroy, AfterViewInit, HostListener } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

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
  private pageStartTime = Date.now();
  private scrollDepthTracked = { 25: false, 50: false, 75: false, 100: false };
  private scrollSubject = new Subject<void>();
  private destroy$ = new Subject<void>();
  
  // Video configuration
  // TODO: Replace with actual video URL when video is produced
  // IMPORTANT: Only use trusted video hosting services (YouTube, Vimeo) to prevent XSS
  // Accepted URL patterns:
  // - YouTube: https://www.youtube.com/embed/VIDEO_ID
  // - Vimeo: https://player.vimeo.com/video/VIDEO_ID
  demoVideoUrl: string = ''; // Empty = show placeholder
  
  constructor(private analytics: WebsiteAnalyticsService) {}
  
  get hasVideo(): boolean {
    return this.demoVideoUrl.trim().length > 0;
  }

  ngOnInit(): void {
    this.setupIntersectionObserver();
    
    // Track page view
    this.analytics.trackPageView('/home', 'PrimeCare Software - Home');
    
    // Setup throttled scroll tracking
    this.scrollSubject
      .pipe(throttleTime(500)) // Throttle to once every 500ms
      .subscribe(() => this.trackScrollDepth());
  }

  ngAfterViewInit(): void {
    // Setup observers after view is fully initialized to avoid missing elements
    this.observeElements();
  }

  ngOnDestroy(): void {
    if (this.observer) {
      this.observer.disconnect();
    }
    
    // Complete subjects
    this.scrollSubject.complete();
    this.destroy$.next();
    this.destroy$.complete();
    
    // Track engagement time when leaving the page
    const timeOnPage = (Date.now() - this.pageStartTime) / 1000;
    this.analytics.trackEngagementTime('home', timeOnPage);
  }
  
  @HostListener('window:scroll')
  onScroll(): void {
    // Emit scroll event to throttled subject
    this.scrollSubject.next();
  }
  
  /**
   * Track scroll depth at key milestones (throttled)
   */
  private trackScrollDepth(): void {
    const scrollHeight = document.documentElement.scrollHeight;
    const clientHeight = window.innerHeight;
    const scrollTop = window.scrollY;
    
    // Prevent division by zero
    const scrollableHeight = scrollHeight - clientHeight;
    if (scrollableHeight <= 0) {
      return; // Page is not scrollable
    }
    
    const scrollPercent = Math.round((scrollTop / scrollableHeight) * 100);
    
    // Track at 25%, 50%, 75%, and 100%
    const milestones = [25, 50, 75, 100] as const;
    milestones.forEach(milestone => {
      if (scrollPercent >= milestone && !this.scrollDepthTracked[milestone]) {
        this.scrollDepthTracked[milestone] = true;
        this.analytics.trackScrollDepth(milestone, 'home');
      }
    });
  }
  
  /**
   * Track CTA clicks
   */
  trackCTA(ctaName: string, ctaLocation: string = 'hero'): void {
    this.analytics.trackCTAClick(ctaName, ctaLocation);
    this.analytics.trackConversion('trial_signup');
  }
  
  /**
   * Track navigation clicks
   */
  trackNavigation(destination: string): void {
    this.analytics.trackNavigation(destination, 'home');
  }
  
  /**
   * Track video engagement
   */
  onVideoPlay(): void {
    this.analytics.trackVideoEngagement('home-demo-video', 'play');
  }
  
  onVideoPause(): void {
    this.analytics.trackVideoEngagement('home-demo-video', 'pause');
  }
  
  onVideoComplete(): void {
    this.analytics.trackVideoEngagement('home-demo-video', 'complete');
  }

  openWhatsApp(): void {
    this.analytics.trackCTAClick('WhatsApp', 'floating-button');
    this.analytics.trackConversion('contact');
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
