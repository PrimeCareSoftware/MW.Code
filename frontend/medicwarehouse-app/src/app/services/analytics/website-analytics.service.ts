import { Injectable } from '@angular/core';

export interface AnalyticsEvent {
  category: string;
  action: string;
  label?: string;
  value?: number;
}

export interface PageViewEvent {
  page: string;
  title?: string;
  referrer?: string;
}

@Injectable({
  providedIn: 'root'
})
export class WebsiteAnalyticsService {
  private readonly GA_ENABLED = true; // Set to false to disable Google Analytics

  constructor() {
    // Initialize GA4 if available
    if (this.GA_ENABLED && typeof window !== 'undefined' && (window as any).gtag) {
      console.log('WebsiteAnalyticsService initialized with Google Analytics');
    }
  }

  /**
   * Track page view
   */
  trackPageView(page: string, title?: string): void {
    if (!this.GA_ENABLED || typeof window === 'undefined') return;

    const event: PageViewEvent = {
      page,
      title: title || document.title,
      referrer: document.referrer
    };

    // Google Analytics 4
    if ((window as any).gtag) {
      (window as any).gtag('event', 'page_view', {
        page_path: event.page,
        page_title: event.title,
        page_location: window.location.href
      });
    }

    console.log('Page view tracked:', event);
  }

  /**
   * Track CTA click
   */
  trackCTAClick(ctaName: string, ctaLocation?: string): void {
    this.trackEvent({
      category: 'CTA',
      action: 'click',
      label: `${ctaName}${ctaLocation ? ` - ${ctaLocation}` : ''}`
    });
  }

  /**
   * Track video engagement
   */
  trackVideoEngagement(videoId: string, event: 'play' | 'pause' | 'complete' | 'seek', progress?: number): void {
    this.trackEvent({
      category: 'Video',
      action: event,
      label: videoId,
      value: progress
    });
  }

  /**
   * Track form submission
   */
  trackFormSubmission(formName: string, success: boolean = true): void {
    this.trackEvent({
      category: 'Form',
      action: success ? 'submit_success' : 'submit_error',
      label: formName
    });
  }

  /**
   * Track conversion
   */
  trackConversion(conversionType: 'trial_signup' | 'demo_request' | 'contact' | 'pricing_view', value?: number): void {
    this.trackEvent({
      category: 'Conversion',
      action: conversionType,
      value: value
    });

    // Send conversion to GA4
    if (this.GA_ENABLED && typeof window !== 'undefined' && (window as any).gtag) {
      (window as any).gtag('event', 'conversion', {
        conversion_type: conversionType,
        value: value,
        currency: 'BRL'
      });
    }
  }

  /**
   * Track button click
   */
  trackButtonClick(buttonName: string, section?: string): void {
    this.trackEvent({
      category: 'Button',
      action: 'click',
      label: `${buttonName}${section ? ` - ${section}` : ''}`
    });
  }

  /**
   * Track navigation
   */
  trackNavigation(destination: string, source?: string): void {
    this.trackEvent({
      category: 'Navigation',
      action: 'navigate',
      label: `${destination}${source ? ` from ${source}` : ''}`
    });
  }

  /**
   * Track search
   */
  trackSearch(searchTerm: string, resultsCount?: number): void {
    this.trackEvent({
      category: 'Search',
      action: 'search',
      label: searchTerm,
      value: resultsCount
    });
  }

  /**
   * Track download
   */
  trackDownload(fileName: string, fileType?: string): void {
    this.trackEvent({
      category: 'Download',
      action: 'download',
      label: `${fileName}${fileType ? ` (${fileType})` : ''}`
    });
  }

  /**
   * Track social share
   */
  trackSocialShare(platform: 'facebook' | 'twitter' | 'linkedin' | 'whatsapp', contentType?: string): void {
    this.trackEvent({
      category: 'Social',
      action: 'share',
      label: `${platform}${contentType ? ` - ${contentType}` : ''}`
    });
  }

  /**
   * Track engagement time
   */
  trackEngagementTime(pageName: string, timeInSeconds: number): void {
    this.trackEvent({
      category: 'Engagement',
      action: 'time_on_page',
      label: pageName,
      value: Math.round(timeInSeconds)
    });
  }

  /**
   * Track scroll depth
   */
  trackScrollDepth(percentage: number, pageName?: string): void {
    this.trackEvent({
      category: 'Engagement',
      action: 'scroll_depth',
      label: pageName || window.location.pathname,
      value: percentage
    });
  }

  /**
   * Track feature usage
   */
  trackFeatureUsage(featureName: string, action: string = 'use'): void {
    this.trackEvent({
      category: 'Feature',
      action: action,
      label: featureName
    });
  }

  /**
   * Track error
   */
  trackError(errorType: string, errorMessage?: string): void {
    this.trackEvent({
      category: 'Error',
      action: errorType,
      label: errorMessage
    });
  }

  /**
   * Track blog article read
   */
  trackBlogArticleRead(articleTitle: string, articleCategory?: string, readTime?: number): void {
    this.trackEvent({
      category: 'Blog',
      action: 'article_read',
      label: `${articleTitle}${articleCategory ? ` - ${articleCategory}` : ''}`,
      value: readTime
    });
  }

  /**
   * Track case study view
   */
  trackCaseStudyView(caseTitle: string): void {
    this.trackEvent({
      category: 'Case Study',
      action: 'view',
      label: caseTitle
    });
  }

  /**
   * Track pricing plan view
   */
  trackPricingPlanView(planName: string): void {
    this.trackEvent({
      category: 'Pricing',
      action: 'plan_view',
      label: planName
    });
  }

  /**
   * Generic event tracking
   */
  private trackEvent(event: AnalyticsEvent): void {
    if (!this.GA_ENABLED || typeof window === 'undefined') return;

    // Google Analytics 4
    if ((window as any).gtag) {
      (window as any).gtag('event', event.action, {
        event_category: event.category,
        event_label: event.label,
        value: event.value
      });
    }

    // Console log for development
    console.log('Analytics event tracked:', event);
  }

  /**
   * Set user properties
   */
  setUserProperty(propertyName: string, propertyValue: string): void {
    if (!this.GA_ENABLED || typeof window === 'undefined') return;

    if ((window as any).gtag) {
      (window as any).gtag('set', 'user_properties', {
        [propertyName]: propertyValue
      });
    }
  }

  /**
   * Set user ID (for logged in users)
   */
  setUserId(userId: string): void {
    if (!this.GA_ENABLED || typeof window === 'undefined') return;

    if ((window as any).gtag) {
      (window as any).gtag('config', 'GA_MEASUREMENT_ID', {
        user_id: userId
      });
    }
  }

  /**
   * Track custom dimension
   */
  trackCustomDimension(dimensionName: string, dimensionValue: string): void {
    if (!this.GA_ENABLED || typeof window === 'undefined') return;

    if ((window as any).gtag) {
      (window as any).gtag('event', 'custom_dimension', {
        [dimensionName]: dimensionValue
      });
    }
  }
}
