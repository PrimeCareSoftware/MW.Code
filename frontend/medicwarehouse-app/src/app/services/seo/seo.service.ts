import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

export interface SeoMetadata {
  title?: string;
  description?: string;
  keywords?: string[];
  image?: string;
  url?: string;
  type?: 'website' | 'article' | 'product';
  author?: string;
  publishedAt?: Date;
  modifiedAt?: Date;
  locale?: string;
}

export interface StructuredData {
  '@context': string;
  '@type': string;
  [key: string]: any;
}

/**
 * SEO Service
 * 
 * Manages meta tags, structured data, and SEO optimization for the website.
 * Supports:
 * - Dynamic meta tags (title, description, keywords)
 * - Open Graph tags (Facebook, LinkedIn)
 * - Twitter Cards
 * - JSON-LD structured data (Schema.org)
 * - Canonical URLs
 * - Robots meta tags
 */
@Injectable({
  providedIn: 'root'
})
export class SeoService {
  private meta = inject(Meta);
  private titleService = inject(Title);
  private router = inject(Router);
  
  private defaultMetadata: SeoMetadata = {
    title: 'PrimeCare Software - Sistema de Gestão para Clínicas Médicas',
    description: 'Software completo para gestão de consultórios e clínicas. Agenda, prontuário eletrônico, telemedicina e mais. Experimente grátis por 15 dias.',
    keywords: ['software médico', 'gestão clínica', 'prontuário eletrônico', 'agenda médica', 'telemedicina'],
    image: 'https://primecare.com.br/assets/og-image.jpg',
    type: 'website',
    locale: 'pt_BR'
  };

  constructor() {
    this.setupRouteChangeTracking();
  }

  /**
   * Update page SEO metadata
   */
  updateMetadata(metadata: SeoMetadata): void {
    // Merge with defaults
    const meta = { ...this.defaultMetadata, ...metadata };
    
    // Update title
    if (meta.title) {
      this.titleService.setTitle(meta.title);
      this.meta.updateTag({ property: 'og:title', content: meta.title });
      this.meta.updateTag({ name: 'twitter:title', content: meta.title });
    }
    
    // Update description
    if (meta.description) {
      this.meta.updateTag({ name: 'description', content: meta.description });
      this.meta.updateTag({ property: 'og:description', content: meta.description });
      this.meta.updateTag({ name: 'twitter:description', content: meta.description });
    }
    
    // Update keywords
    if (meta.keywords && meta.keywords.length > 0) {
      this.meta.updateTag({ name: 'keywords', content: meta.keywords.join(', ') });
    }
    
    // Update image
    if (meta.image) {
      this.meta.updateTag({ property: 'og:image', content: meta.image });
      this.meta.updateTag({ name: 'twitter:image', content: meta.image });
    }
    
    // Update URL
    const currentUrl = meta.url || window.location.href;
    this.meta.updateTag({ property: 'og:url', content: currentUrl });
    this.meta.updateTag({ name: 'twitter:url', content: currentUrl });
    this.updateCanonicalUrl(currentUrl);
    
    // Update type
    if (meta.type) {
      this.meta.updateTag({ property: 'og:type', content: meta.type });
    }
    
    // Update locale
    if (meta.locale) {
      this.meta.updateTag({ property: 'og:locale', content: meta.locale });
    }
    
    // Article-specific metadata
    if (meta.type === 'article') {
      if (meta.author) {
        this.meta.updateTag({ name: 'author', content: meta.author });
        this.meta.updateTag({ property: 'article:author', content: meta.author });
      }
      if (meta.publishedAt) {
        this.meta.updateTag({ 
          property: 'article:published_time', 
          content: meta.publishedAt.toISOString() 
        });
      }
      if (meta.modifiedAt) {
        this.meta.updateTag({ 
          property: 'article:modified_time', 
          content: meta.modifiedAt.toISOString() 
        });
      }
    }
  }

  /**
   * Update canonical URL
   * Validates URL format and ensures HTTPS for production
   */
  updateCanonicalUrl(url: string): void {
    // Validate URL format
    try {
      const urlObj = new URL(url);
      
      // Warn if not HTTPS in production
      if (urlObj.protocol !== 'https:' && window.location.protocol === 'https:') {
        console.warn('Canonical URL should use HTTPS:', url);
      }
    } catch (error) {
      console.error('Invalid canonical URL format:', url);
      return;
    }
    
    let link: HTMLLinkElement | null = document.querySelector('link[rel="canonical"]');
    
    if (!link) {
      link = document.createElement('link');
      link.setAttribute('rel', 'canonical');
      document.head.appendChild(link);
    }
    
    link.setAttribute('href', url);
  }

  /**
   * Add JSON-LD structured data to page
   */
  addStructuredData(data: StructuredData): void {
    // Check if script already exists
    let script = document.querySelector('script[type="application/ld+json"][data-schema="dynamic"]');
    
    if (script) {
      // Update existing script
      script.textContent = JSON.stringify(data, null, 2);
    } else {
      // Create new script
      script = document.createElement('script');
      script.setAttribute('type', 'application/ld+json');
      script.setAttribute('data-schema', 'dynamic');
      script.textContent = JSON.stringify(data, null, 2);
      document.head.appendChild(script);
    }
  }

  /**
   * Remove structured data from page
   */
  removeStructuredData(): void {
    const script = document.querySelector('script[type="application/ld+json"][data-schema="dynamic"]');
    if (script) {
      script.remove();
    }
  }

  /**
   * Generate article structured data (Schema.org)
   */
  generateArticleStructuredData(article: {
    title: string;
    description: string;
    image?: string;
    author: string;
    publishedAt: Date;
    modifiedAt?: Date;
    url: string;
  }): StructuredData {
    return {
      '@context': 'https://schema.org',
      '@type': 'Article',
      'headline': article.title,
      'description': article.description,
      'image': article.image || this.defaultMetadata.image,
      'author': {
        '@type': 'Person',
        'name': article.author
      },
      'publisher': {
        '@type': 'Organization',
        'name': 'PrimeCare Software',
        'logo': {
          '@type': 'ImageObject',
          'url': 'https://primecare.com.br/assets/logo.png'
        }
      },
      'datePublished': article.publishedAt.toISOString(),
      'dateModified': (article.modifiedAt || article.publishedAt).toISOString(),
      'mainEntityOfPage': {
        '@type': 'WebPage',
        '@id': article.url
      }
    };
  }

  /**
   * Generate breadcrumb structured data (Schema.org)
   */
  generateBreadcrumbStructuredData(items: Array<{ name: string; url: string }>): StructuredData {
    return {
      '@context': 'https://schema.org',
      '@type': 'BreadcrumbList',
      'itemListElement': items.map((item, index) => ({
        '@type': 'ListItem',
        'position': index + 1,
        'name': item.name,
        'item': item.url
      }))
    };
  }

  /**
   * Set robots meta tag
   */
  setRobots(value: 'index,follow' | 'noindex,nofollow' | 'index,nofollow' | 'noindex,follow'): void {
    this.meta.updateTag({ name: 'robots', content: value });
  }

  /**
   * Setup automatic route change tracking for default metadata
   */
  private setupRouteChangeTracking(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        // Reset to default metadata on route change
        // Individual pages should call updateMetadata() to override
        this.updateMetadata({});
      });
  }

  /**
   * Reset to default metadata
   */
  resetMetadata(): void {
    this.updateMetadata(this.defaultMetadata);
    this.removeStructuredData();
  }

  /**
   * Generate FAQ structured data (Schema.org)
   */
  generateFAQStructuredData(faqs: Array<{ question: string; answer: string }>): StructuredData {
    return {
      '@context': 'https://schema.org',
      '@type': 'FAQPage',
      'mainEntity': faqs.map(faq => ({
        '@type': 'Question',
        'name': faq.question,
        'acceptedAnswer': {
          '@type': 'Answer',
          'text': faq.answer
        }
      }))
    };
  }

  /**
   * Generate organization structured data (Schema.org)
   */
  generateOrganizationStructuredData(): StructuredData {
    return {
      '@context': 'https://schema.org',
      '@type': 'Organization',
      'name': 'PrimeCare Software',
      'url': 'https://primecare.com.br',
      'logo': 'https://primecare.com.br/assets/logo.png',
      'description': 'Software completo para gestão de consultórios e clínicas médicas',
      'contactPoint': {
        '@type': 'ContactPoint',
        'telephone': '+55-11-99999-9999',
        'contactType': 'customer service',
        'areaServed': 'BR',
        'availableLanguage': 'Portuguese'
      },
      'sameAs': [
        'https://www.facebook.com/primecare',
        'https://www.linkedin.com/company/primecare',
        'https://twitter.com/primecare'
      ]
    };
  }
}
