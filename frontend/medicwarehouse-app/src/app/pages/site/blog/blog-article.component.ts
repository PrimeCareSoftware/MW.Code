import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject, takeUntil } from 'rxjs';
import { BlogService, BlogArticle } from '../../../services/blog/blog.service';
import { SeoService } from '../../../services/seo/seo.service';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

@Component({
  selector: 'app-blog-article',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './blog-article.component.html',
  styleUrls: ['./blog-article.component.scss']
})
export class BlogArticleComponent implements OnInit, OnDestroy {
  article: BlogArticle | null = null;
  relatedArticles: BlogArticle[] = [];
  loading = true;
  notFound = false;
  private destroy$ = new Subject<void>();
  private readStartTime = Date.now();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private blogService: BlogService,
    private seo: SeoService,
    private analytics: WebsiteAnalyticsService
  ) {}

  ngOnInit(): void {
    // Get article slug from route
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const slug = params.get('slug');
        if (slug) {
          this.loadArticle(slug);
        }
      });
  }

  ngOnDestroy(): void {
    // Track reading time
    if (this.article) {
      const readTime = Math.floor((Date.now() - this.readStartTime) / 1000);
      this.analytics.trackBlogArticleRead(this.article.slug, readTime, this.article.category);
    }
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadArticle(slug: string): void {
    this.loading = true;
    this.notFound = false;

    this.blogService.getArticleBySlug(slug)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (article) => {
          if (article) {
            this.article = article;
            this.updateSEO(article);
            this.incrementViews(article.id);
            this.loadRelatedArticles(article.id, article.category);
            this.analytics.trackPageView(`/blog/${slug}`, article.title);
          } else {
            this.notFound = true;
          }
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading article:', error);
          this.notFound = true;
          this.loading = false;
        }
      });
  }

  private updateSEO(article: BlogArticle): void {
    this.seo.updateMetadata({
      title: article.metaTitle || article.title,
      description: article.metaDescription || article.excerpt,
      keywords: article.metaKeywords?.join(', '),
      type: 'article',
      image: article.featuredImage,
      author: article.author.name,
      publishedAt: new Date(article.publishedAt),
      modifiedAt: article.updatedAt ? new Date(article.updatedAt) : undefined
    });

    // Add structured data for article
    this.seo.addStructuredData(
      this.seo.generateArticleStructuredData({
        title: article.title,
        description: article.excerpt,
        author: article.author.name,
        publishedAt: new Date(article.publishedAt),
        modifiedAt: article.updatedAt ? new Date(article.updatedAt) : undefined,
        image: article.featuredImage,
        url: window.location.href
      })
    );
  }

  private incrementViews(articleId: string): void {
    this.blogService.incrementViews(articleId).subscribe();
  }

  private loadRelatedArticles(currentArticleId: string, category: string): void {
    this.blogService.getRelatedArticles(currentArticleId, category)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (articles) => {
          this.relatedArticles = articles;
        },
        error: (error) => {
          console.error('Error loading related articles:', error);
        }
      });
  }

  onLikeArticle(): void {
    if (!this.article) return;

    this.blogService.likeArticle(this.article.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedArticle) => {
          if (updatedArticle) {
            this.article = updatedArticle;
          }
          this.analytics.trackButtonClick('like_article', `/blog/${this.article?.slug}`);
        },
        error: (error) => {
          console.error('Error liking article:', error);
        }
      });
  }

  onShareArticle(platform: 'twitter' | 'linkedin' | 'facebook' | 'whatsapp'): void {
    if (!this.article) return;

    const url = window.location.href;
    const title = this.article.title;
    const text = this.article.excerpt;

    let shareUrl = '';

    switch (platform) {
      case 'twitter':
        shareUrl = `https://twitter.com/intent/tweet?text=${encodeURIComponent(title)}&url=${encodeURIComponent(url)}`;
        break;
      case 'linkedin':
        shareUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`;
        break;
      case 'facebook':
        shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`;
        break;
      case 'whatsapp':
        shareUrl = `https://wa.me/?text=${encodeURIComponent(title + ' - ' + url)}`;
        break;
    }

    if (shareUrl) {
      window.open(shareUrl, '_blank', 'width=600,height=400');
      this.analytics.trackSocialShare(platform, url, 'article');
    }
  }

  onBackToBlog(): void {
    this.router.navigate(['/site/blog']);
  }

  onRelatedArticleClick(article: BlogArticle): void {
    this.router.navigate(['/site/blog', article.slug]);
    this.analytics.trackButtonClick('related_article', `/blog/${article.slug}`);
  }
}
