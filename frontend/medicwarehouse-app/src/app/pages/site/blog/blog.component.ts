import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { BlogService, BlogArticle, BlogCategory, BlogFilters, PaginatedBlogResponse } from '../../../services/blog/blog.service';
import { SeoService } from '../../../services/seo/seo.service';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, HeaderComponent, FooterComponent],
  templateUrl: './blog.component.html',
  styleUrl: './blog.component.scss'
})
export class BlogComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  articles: BlogArticle[] = [];
  categories: BlogCategory[] = [];
  popularArticles: BlogArticle[] = [];
  
  isLoading = true;
  currentPage = 1;
  totalPages = 1;
  selectedCategory: string | null = null;
  searchTerm = '';
  
  constructor(
    private blogService: BlogService,
    private seo: SeoService,
    private analytics: WebsiteAnalyticsService
  ) {}
  
  ngOnInit(): void {
    this.setupSEO();
    this.loadArticles();
    this.loadCategories();
    this.loadPopularArticles();
    
    // Track page view
    this.analytics.trackPageView('/blog', 'PrimeCare Blog - Conteúdo para Profissionais de Saúde');
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  private setupSEO(): void {
    this.seo.updateMetadata({
      title: 'Blog PrimeCare - Dicas e Novidades para Clínicas Médicas',
      description: 'Artigos sobre gestão clínica, tecnologia em saúde, prontuário eletrônico e muito mais. Conteúdo exclusivo para profissionais de saúde.',
      keywords: ['blog médico', 'gestão clínica', 'tecnologia em saúde', 'prontuário eletrônico', 'dicas clínica'],
      type: 'website',
      url: window.location.href
    });
  }
  
  loadArticles(): void {
    this.isLoading = true;
    
    const filters: BlogFilters = {
      page: this.currentPage,
      perPage: 9
    };
    
    if (this.selectedCategory) {
      filters.category = this.selectedCategory;
    }
    
    if (this.searchTerm) {
      filters.search = this.searchTerm;
    }
    
    this.blogService.getArticles(filters)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: PaginatedBlogResponse) => {
          this.articles = response.articles;
          this.totalPages = response.totalPages;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading articles:', error);
          this.isLoading = false;
        }
      });
  }
  
  loadCategories(): void {
    this.blogService.getCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories: BlogCategory[]) => {
          this.categories = categories;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });
  }
  
  loadPopularArticles(): void {
    this.blogService.getPopularArticles(3)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (articles: BlogArticle[]) => {
          this.popularArticles = articles;
        },
        error: (error) => {
          console.error('Error loading popular articles:', error);
        }
      });
  }
  
  onCategorySelect(categorySlug: string | null): void {
    this.selectedCategory = categorySlug;
    this.currentPage = 1;
    this.loadArticles();
    
    // Track category filter
    if (categorySlug) {
      this.analytics.trackButtonClick(`Filter: ${categorySlug}`, 'blog');
    }
  }
  
  onSearch(): void {
    this.currentPage = 1;
    this.loadArticles();
    
    // Track search
    this.analytics.trackSearch(this.searchTerm, this.articles.length);
  }
  
  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadArticles();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  
  trackArticleClick(article: BlogArticle): void {
    this.analytics.trackNavigation(`/blog/${article.slug}`, 'blog-list');
  }
}
