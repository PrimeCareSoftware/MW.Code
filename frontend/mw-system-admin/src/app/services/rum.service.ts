import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface PerformanceMetric {
  metric: string;
  value: number;
  url: string;
  timestamp?: number;
  userAgent?: string;
  connection?: string;
}

export interface WebVitals {
  fcp?: number; // First Contentful Paint
  lcp?: number; // Largest Contentful Paint
  fid?: number; // First Input Delay
  cls?: number; // Cumulative Layout Shift
  ttfb?: number; // Time to First Byte
}

@Injectable({
  providedIn: 'root'
})
export class RealUserMonitoringService {
  private metrics: Map<string, number> = new Map();
  private webVitals: WebVitals = {};
  
  constructor(private http: HttpClient) {
    this.initializeMonitoring();
  }
  
  private initializeMonitoring(): void {
    if (typeof window === 'undefined') {
      return;
    }
    
    // Track page load performance
    if (window.performance) {
      window.addEventListener('load', () => {
        setTimeout(() => this.trackPageLoad(), 0);
      });
    }
    
    // Track Web Vitals
    this.trackWebVitals();
    
    // Track navigation timing
    this.trackNavigationTiming();
  }
  
  private trackPageLoad(): void {
    if (!window.performance || !window.performance.timing) {
      return;
    }
    
    const timing = window.performance.timing;
    const loadTime = timing.loadEventEnd - timing.navigationStart;
    const domReadyTime = timing.domContentLoadedEventEnd - timing.navigationStart;
    const responseTime = timing.responseEnd - timing.requestStart;
    
    this.sendMetric({
      metric: 'page_load',
      value: loadTime,
      url: window.location.pathname
    });
    
    this.sendMetric({
      metric: 'dom_ready',
      value: domReadyTime,
      url: window.location.pathname
    });
    
    this.sendMetric({
      metric: 'response_time',
      value: responseTime,
      url: window.location.pathname
    });
  }
  
  private trackNavigationTiming(): void {
    if (!('PerformanceObserver' in window)) {
      return;
    }
    
    try {
      const navObserver = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          if (entry.entryType === 'navigation') {
            const navEntry = entry as PerformanceNavigationTiming;
            
            // DNS lookup time
            const dnsTime = navEntry.domainLookupEnd - navEntry.domainLookupStart;
            this.metrics.set('dns_lookup', dnsTime);
            
            // TCP connection time
            const tcpTime = navEntry.connectEnd - navEntry.connectStart;
            this.metrics.set('tcp_connection', tcpTime);
            
            // Time to First Byte
            const ttfb = navEntry.responseStart - navEntry.requestStart;
            this.webVitals.ttfb = ttfb;
            this.metrics.set('ttfb', ttfb);
            
            this.sendMetric({
              metric: 'ttfb',
              value: ttfb,
              url: window.location.pathname
            });
          }
        }
      });
      
      navObserver.observe({ entryTypes: ['navigation'] });
    } catch (e) {
      console.error('Error tracking navigation timing:', e);
    }
  }
  
  private trackWebVitals(): void {
    if (!('PerformanceObserver' in window)) {
      return;
    }
    
    // First Contentful Paint (FCP)
    try {
      const fcpObserver = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          if (entry.name === 'first-contentful-paint') {
            this.webVitals.fcp = entry.startTime;
            this.sendMetric({
              metric: 'fcp',
              value: entry.startTime,
              url: window.location.pathname
            });
          }
        }
      });
      fcpObserver.observe({ entryTypes: ['paint'] });
    } catch (e) {
      console.error('Error tracking FCP:', e);
    }
    
    // Largest Contentful Paint (LCP)
    try {
      const lcpObserver = new PerformanceObserver((list) => {
        const entries = list.getEntries();
        const lastEntry = entries[entries.length - 1];
        this.webVitals.lcp = lastEntry.startTime;
        this.sendMetric({
          metric: 'lcp',
          value: lastEntry.startTime,
          url: window.location.pathname
        });
      });
      lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });
    } catch (e) {
      console.error('Error tracking LCP:', e);
    }
    
    // First Input Delay (FID)
    try {
      const fidObserver = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          const fidEntry = entry as any;
          const fid = fidEntry.processingStart - fidEntry.startTime;
          this.webVitals.fid = fid;
          this.sendMetric({
            metric: 'fid',
            value: fid,
            url: window.location.pathname
          });
        }
      });
      fidObserver.observe({ entryTypes: ['first-input'] });
    } catch (e) {
      console.error('Error tracking FID:', e);
    }
    
    // Cumulative Layout Shift (CLS)
    try {
      let clsValue = 0;
      const clsObserver = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          const layoutShiftEntry = entry as any;
          if (!layoutShiftEntry.hadRecentInput) {
            clsValue += layoutShiftEntry.value;
          }
        }
        this.webVitals.cls = clsValue;
      });
      clsObserver.observe({ entryTypes: ['layout-shift'] });
      
      // Send CLS on page unload
      window.addEventListener('beforeunload', () => {
        if (clsValue > 0) {
          this.sendMetric({
            metric: 'cls',
            value: clsValue,
            url: window.location.pathname
          });
        }
      });
    } catch (e) {
      console.error('Error tracking CLS:', e);
    }
  }
  
  trackCustomMetric(name: string, value: number): void {
    this.metrics.set(name, value);
    this.sendMetric({
      metric: name,
      value,
      url: window.location.pathname
    });
  }
  
  trackApiCall(endpoint: string, duration: number, status: number): void {
    this.sendMetric({
      metric: 'api_call',
      value: duration,
      url: endpoint
    });
  }
  
  private sendMetric(metric: PerformanceMetric): void {
    const enrichedMetric = {
      ...metric,
      timestamp: Date.now(),
      userAgent: navigator.userAgent,
      connection: this.getConnectionInfo()
    };
    
    // In production, send to backend
    if (environment.production) {
      this.http.post(`${environment.apiUrl}/api/system-admin/rum/metrics`, enrichedMetric)
        .subscribe({
          error: (err) => console.error('Error sending RUM metric:', err)
        });
    } else {
      console.log('RUM Metric:', enrichedMetric);
    }
  }
  
  private getConnectionInfo(): string {
    const connection = (navigator as any).connection;
    if (connection) {
      return `${connection.effectiveType || 'unknown'} - ${connection.downlink || 0}Mbps`;
    }
    return 'unknown';
  }
  
  getWebVitals(): WebVitals {
    return { ...this.webVitals };
  }
  
  getMetrics(): Map<string, number> {
    return new Map(this.metrics);
  }
}
