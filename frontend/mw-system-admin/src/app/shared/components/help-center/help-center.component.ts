import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { HelpService, HelpArticle, HelpCategory, HelpVideo } from '../../../services/help.service';
import { fadeInAnimation } from '../../animations/fade-slide.animations';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-help-center',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatCardModule,
    MatListModule,
    MatChipsModule,
    MatButtonModule,
    MatDialogModule
  ],
  animations: [fadeInAnimation],
  template: `
    <div class="help-center" @fadeIn>
      <!-- Search -->
      <div class="search-header">
        <h1>Como podemos ajudar?</h1>
        <mat-form-field class="search-field" appearance="outline">
          <mat-icon matPrefix>search</mat-icon>
          <input 
            matInput 
            placeholder="Buscar artigos, vídeos, tutoriais..." 
            [(ngModel)]="searchQuery"
            (ngModelChange)="search()"
          >
        </mat-form-field>
      </div>
      
      <!-- Categories -->
      <div class="help-categories" *ngIf="!searchQuery">
        <h2>Categorias</h2>
        <div class="categories-grid">
          <mat-card 
            *ngFor="let category of categories"
            class="category-card"
            (click)="selectCategory(category)"
          >
            <mat-icon class="category-icon">{{ category.icon }}</mat-icon>
            <h3>{{ category.name }}</h3>
            <p>{{ category.description }}</p>
            <span class="article-count">{{ category.articleCount }} artigos</span>
          </mat-card>
        </div>
      </div>
      
      <!-- Search Results -->
      <div class="search-results" *ngIf="searchQuery && searchResults.length > 0">
        <h2>Resultados da busca</h2>
        <div class="results-list">
          <div 
            *ngFor="let article of searchResults"
            class="article-preview"
            (click)="openArticle(article)"
          >
            <h4>{{ article.title }}</h4>
            <p>{{ article.excerpt }}</p>
            <div class="article-meta">
              <mat-chip-set>
                <mat-chip>{{ getCategoryName(article.category) }}</mat-chip>
              </mat-chip-set>
              <span class="read-time">{{ article.readTime }} min de leitura</span>
            </div>
          </div>
        </div>
      </div>
      
      <div class="no-results" *ngIf="searchQuery && searchResults.length === 0">
        <mat-icon>search_off</mat-icon>
        <h3>Nenhum resultado encontrado</h3>
        <p>Tente usar palavras-chave diferentes ou navegue pelas categorias.</p>
      </div>
      
      <!-- Popular Articles -->
      <div class="popular-articles" *ngIf="!searchQuery">
        <h2>Artigos Populares</h2>
        <mat-list>
          <mat-list-item *ngFor="let article of popularArticles" (click)="openArticle(article)">
            <mat-icon matListItemIcon>article</mat-icon>
            <div matListItemTitle>{{ article.title }}</div>
            <div matListItemLine>{{ article.views }} visualizações · {{ article.readTime }} min</div>
          </mat-list-item>
        </mat-list>
      </div>
      
      <!-- Video Tutorials -->
      <div class="video-tutorials" *ngIf="!searchQuery">
        <h2>Tutoriais em Vídeo</h2>
        <div class="video-grid">
          <div *ngFor="let video of videos" class="video-card" (click)="openVideo(video)">
            <div class="video-thumbnail">
              <mat-icon class="play-icon">play_circle</mat-icon>
              <span class="video-duration">{{ video.duration }}</span>
            </div>
            <h4>{{ video.title }}</h4>
            <p>{{ video.description }}</p>
          </div>
        </div>
      </div>
      
      <!-- Contact Support -->
      <div class="contact-support">
        <mat-card>
          <h3>Ainda precisa de ajuda?</h3>
          <p>Nossa equipe de suporte está pronta para ajudar você.</p>
          <button mat-raised-button color="primary" (click)="openTicket()">
            <mat-icon>support</mat-icon>
            Abrir Ticket de Suporte
          </button>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .help-center {
      max-width: 1200px;
      margin: 0 auto;
      padding: 32px;
    }
    
    .search-header {
      text-align: center;
      margin-bottom: 48px;
    }
    
    .search-header h1 {
      font-size: 32px;
      font-weight: 600;
      margin-bottom: 24px;
      color: var(--text-primary);
    }
    
    .search-field {
      width: 100%;
      max-width: 600px;
    }
    
    .help-categories h2,
    .search-results h2,
    .popular-articles h2,
    .video-tutorials h2 {
      font-size: 24px;
      font-weight: 600;
      margin-bottom: 24px;
      color: var(--text-primary);
    }
    
    .categories-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 24px;
      margin-bottom: 48px;
    }
    
    .category-card {
      padding: 24px;
      text-align: center;
      cursor: pointer;
      transition: all 0.3s ease;
    }
    
    .category-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
    }
    
    .category-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 16px;
      color: var(--primary-color);
    }
    
    .category-card h3 {
      font-size: 18px;
      font-weight: 600;
      margin-bottom: 8px;
    }
    
    .category-card p {
      color: var(--text-secondary);
      margin-bottom: 12px;
    }
    
    .article-count {
      color: var(--text-tertiary);
      font-size: 14px;
    }
    
    .results-list {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-bottom: 48px;
    }
    
    .article-preview {
      padding: 20px;
      border: 1px solid var(--border-color);
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s ease;
    }
    
    .article-preview:hover {
      border-color: var(--primary-color);
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }
    
    .article-preview h4 {
      margin: 0 0 8px 0;
      font-size: 18px;
      font-weight: 600;
    }
    
    .article-preview p {
      margin: 0 0 12px 0;
      color: var(--text-secondary);
    }
    
    .article-meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    
    .read-time {
      color: var(--text-tertiary);
      font-size: 14px;
    }
    
    .no-results {
      text-align: center;
      padding: 48px 0;
    }
    
    .no-results mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: var(--text-tertiary);
      margin-bottom: 16px;
    }
    
    .popular-articles {
      margin-bottom: 48px;
    }
    
    mat-list-item {
      cursor: pointer;
      margin-bottom: 8px;
    }
    
    mat-list-item:hover {
      background-color: var(--hover-background);
    }
    
    .video-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 24px;
      margin-bottom: 48px;
    }
    
    .video-card {
      cursor: pointer;
      transition: transform 0.3s ease;
    }
    
    .video-card:hover {
      transform: translateY(-4px);
    }
    
    .video-thumbnail {
      position: relative;
      width: 100%;
      aspect-ratio: 16/9;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 12px;
    }
    
    .play-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: white;
    }
    
    .video-duration {
      position: absolute;
      bottom: 8px;
      right: 8px;
      background: rgba(0, 0, 0, 0.8);
      color: white;
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
    }
    
    .video-card h4 {
      margin: 0 0 8px 0;
      font-size: 16px;
      font-weight: 600;
    }
    
    .video-card p {
      margin: 0;
      color: var(--text-secondary);
      font-size: 14px;
    }
    
    .contact-support {
      margin-top: 48px;
    }
    
    .contact-support mat-card {
      text-align: center;
      padding: 32px;
    }
    
    .contact-support h3 {
      margin: 0 0 8px 0;
      font-size: 20px;
      font-weight: 600;
    }
    
    .contact-support p {
      margin: 0 0 24px 0;
      color: var(--text-secondary);
    }
    
    .contact-support button {
      display: inline-flex;
      align-items: center;
      gap: 8px;
    }
  `]
})
export class HelpCenterComponent implements OnInit, OnDestroy {
  categories: HelpCategory[] = [];
  popularArticles: HelpArticle[] = [];
  videos: HelpVideo[] = [];
  searchQuery = '';
  searchResults: HelpArticle[] = [];
  
  private searchSubject = new Subject<string>();
  
  constructor(
    private helpService: HelpService,
    private dialog: MatDialog
  ) {}
  
  ngOnInit(): void {
    this.helpService.getCategories().subscribe(cats => this.categories = cats);
    this.helpService.getPopularArticles().subscribe(arts => this.popularArticles = arts);
    this.helpService.getVideos().subscribe(vids => this.videos = vids);
    
    // Debounce search input
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      if (query.length < 2) {
        this.searchResults = [];
        return;
      }
      this.helpService.searchArticles(query).subscribe(
        results => this.searchResults = results
      );
    });
  }
  
  ngOnDestroy(): void {
    this.searchSubject.complete();
  }
  
  search(): void {
    this.searchSubject.next(this.searchQuery);
  }
  
  selectCategory(category: HelpCategory): void {
    console.log('Selected category:', category);
    // Navigate to category page or show articles
  }
  
  openArticle(article: HelpArticle): void {
    this.helpService.incrementArticleView(article.id);
    console.log('Opening article:', article);
    // Open article in dialog or navigate
  }
  
  openVideo(video: HelpVideo): void {
    // Validate URL is from trusted domain
    const trustedDomains = ['youtube.com', 'youtu.be', 'vimeo.com'];
    try {
      const url = new URL(video.url);
      const isTrusted = trustedDomains.some(domain => url.hostname.includes(domain));
      if (isTrusted) {
        window.open(video.url, '_blank', 'noopener,noreferrer');
      } else {
        console.warn('Attempted to open video from untrusted domain:', url.hostname);
      }
    } catch {
      console.error('Invalid video URL:', video.url);
    }
  }
  
  openTicket(): void {
    console.log('Opening support ticket');
    // Navigate to ticket creation
  }
  
  getCategoryName(categoryId: string): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category?.name || categoryId;
  }
}
