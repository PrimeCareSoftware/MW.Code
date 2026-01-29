import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface BlogArticle {
  id: string;
  slug: string;
  title: string;
  excerpt: string;
  content: string;
  featuredImage?: string;
  category: string;
  tags: string[];
  author: {
    name: string;
    avatar?: string;
    bio?: string;
  };
  publishedAt: Date;
  updatedAt?: Date;
  readTime: number; // in minutes
  views: number;
  likes: number;
  metaTitle?: string;
  metaDescription?: string;
  metaKeywords?: string[];
}

export interface BlogCategory {
  id: string;
  name: string;
  slug: string;
  description?: string;
  articleCount: number;
}

export interface BlogFilters {
  category?: string;
  tag?: string;
  search?: string;
  page?: number;
  perPage?: number;
}

export interface PaginatedBlogResponse {
  articles: BlogArticle[];
  total: number;
  page: number;
  perPage: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  private readonly apiUrl = `${environment.apiUrl}/api/blog`;
  private readonly mockEnabled = true; // Set to false when backend is ready

  constructor(private http: HttpClient) {}

  /**
   * Get all articles with optional filters and pagination
   */
  getArticles(filters?: BlogFilters): Observable<PaginatedBlogResponse> {
    if (this.mockEnabled) {
      return of(this.getMockArticles(filters));
    }

    let params = new HttpParams();
    if (filters) {
      if (filters.category) params = params.set('category', filters.category);
      if (filters.tag) params = params.set('tag', filters.tag);
      if (filters.search) params = params.set('search', filters.search);
      if (filters.page) params = params.set('page', filters.page.toString());
      if (filters.perPage) params = params.set('perPage', filters.perPage.toString());
    }

    return this.http.get<PaginatedBlogResponse>(`${this.apiUrl}/articles`, { params })
      .pipe(
        catchError(error => {
          console.error('Error fetching articles:', error);
          return of(this.getMockArticles(filters));
        })
      );
  }

  /**
   * Get a single article by slug
   */
  getArticleBySlug(slug: string): Observable<BlogArticle | null> {
    if (this.mockEnabled) {
      const mockData = this.getMockArticles();
      const article = mockData.articles.find(a => a.slug === slug);
      return of(article || null);
    }

    return this.http.get<BlogArticle>(`${this.apiUrl}/articles/${slug}`)
      .pipe(
        catchError(error => {
          console.error('Error fetching article:', error);
          return of(null);
        })
      );
  }

  /**
   * Get all categories
   */
  getCategories(): Observable<BlogCategory[]> {
    if (this.mockEnabled) {
      return of(this.getMockCategories());
    }

    return this.http.get<BlogCategory[]>(`${this.apiUrl}/categories`)
      .pipe(
        catchError(error => {
          console.error('Error fetching categories:', error);
          return of(this.getMockCategories());
        })
      );
  }

  /**
   * Get popular articles
   */
  getPopularArticles(limit: number = 5): Observable<BlogArticle[]> {
    if (this.mockEnabled) {
      const mockData = this.getMockArticles();
      return of(mockData.articles.slice(0, limit));
    }

    return this.http.get<BlogArticle[]>(`${this.apiUrl}/articles/popular?limit=${limit}`)
      .pipe(
        catchError(error => {
          console.error('Error fetching popular articles:', error);
          return of([]);
        })
      );
  }

  /**
   * Get related articles
   */
  getRelatedArticles(articleId: string, limit: number = 3): Observable<BlogArticle[]> {
    if (this.mockEnabled) {
      const mockData = this.getMockArticles();
      return of(mockData.articles.filter(a => a.id !== articleId).slice(0, limit));
    }

    return this.http.get<BlogArticle[]>(`${this.apiUrl}/articles/${articleId}/related?limit=${limit}`)
      .pipe(
        catchError(error => {
          console.error('Error fetching related articles:', error);
          return of([]);
        })
      );
  }

  /**
   * Increment article view count
   */
  incrementViews(articleId: string): Observable<void> {
    if (this.mockEnabled) {
      return of(void 0);
    }

    return this.http.post<void>(`${this.apiUrl}/articles/${articleId}/view`, {})
      .pipe(
        catchError(error => {
          console.error('Error incrementing views:', error);
          return of(void 0);
        })
      );
  }

  /**
   * Like an article
   */
  likeArticle(articleId: string): Observable<void> {
    if (this.mockEnabled) {
      return of(void 0);
    }

    return this.http.post<void>(`${this.apiUrl}/articles/${articleId}/like`, {})
      .pipe(
        catchError(error => {
          console.error('Error liking article:', error);
          return of(void 0);
        })
      );
  }

  /**
   * Mock data for development
   */
  private getMockArticles(filters?: BlogFilters): PaginatedBlogResponse {
    const allArticles: BlogArticle[] = [
      {
        id: '1',
        slug: 'como-escolher-software-medico',
        title: 'Como escolher o melhor software médico para sua clínica',
        excerpt: 'Descubra os principais critérios para avaliar e escolher o sistema de gestão ideal para sua prática médica.',
        content: '<p>O conteúdo completo do artigo...</p>',
        category: 'Gestão Clínica',
        tags: ['software médico', 'gestão', 'tecnologia'],
        author: {
          name: 'Dr. João Silva',
          avatar: 'https://ui-avatars.com/api/?name=Joao+Silva&background=1e40af&color=fff'
        },
        publishedAt: new Date('2026-01-15'),
        readTime: 5,
        views: 1250,
        likes: 87,
        metaTitle: 'Como escolher o melhor software médico | PrimeCare',
        metaDescription: 'Guia completo com critérios essenciais para escolher o software de gestão clínica ideal para sua prática médica.',
        metaKeywords: ['software médico', 'gestão clínica', 'tecnologia médica']
      },
      {
        id: '2',
        slug: 'beneficios-prontuario-eletronico',
        title: '7 benefícios do prontuário eletrônico para sua clínica',
        excerpt: 'Entenda como a digitalização dos prontuários pode melhorar a eficiência e a qualidade do atendimento.',
        content: '<p>O conteúdo completo do artigo...</p>',
        category: 'Tecnologia',
        tags: ['prontuário eletrônico', 'eficiência', 'qualidade'],
        author: {
          name: 'Dra. Maria Santos',
          avatar: 'https://ui-avatars.com/api/?name=Maria+Santos&background=1e40af&color=fff'
        },
        publishedAt: new Date('2026-01-20'),
        readTime: 7,
        views: 980,
        likes: 64,
        metaTitle: '7 benefícios do prontuário eletrônico | PrimeCare Blog',
        metaDescription: 'Descubra como o prontuário eletrônico pode transformar sua clínica com mais eficiência e qualidade no atendimento.',
        metaKeywords: ['prontuário eletrônico', 'pep', 'digitalização']
      },
      {
        id: '3',
        slug: 'telemedicina-futuro-saude',
        title: 'Telemedicina: o futuro da saúde está aqui',
        excerpt: 'Como a telemedicina está revolucionando o atendimento médico e beneficiando pacientes e profissionais.',
        content: '<p>O conteúdo completo do artigo...</p>',
        category: 'Telemedicina',
        tags: ['telemedicina', 'inovação', 'atendimento online'],
        author: {
          name: 'Dr. Pedro Oliveira',
          avatar: 'https://ui-avatars.com/api/?name=Pedro+Oliveira&background=1e40af&color=fff'
        },
        publishedAt: new Date('2026-01-22'),
        readTime: 6,
        views: 1540,
        likes: 112,
        metaTitle: 'Telemedicina: o futuro da saúde | PrimeCare',
        metaDescription: 'Entenda como a telemedicina está transformando o atendimento médico e criando novas oportunidades.',
        metaKeywords: ['telemedicina', 'saúde digital', 'consulta online']
      },
      {
        id: '4',
        slug: 'gestao-financeira-clinicas',
        title: 'Gestão financeira para clínicas: dicas essenciais',
        excerpt: 'Aprenda a organizar as finanças da sua clínica e aumentar a lucratividade com práticas eficientes.',
        content: '<p>O conteúdo completo do artigo...</p>',
        category: 'Gestão Financeira',
        tags: ['finanças', 'gestão', 'lucratividade'],
        author: {
          name: 'Ana Costa',
          avatar: 'https://ui-avatars.com/api/?name=Ana+Costa&background=1e40af&color=fff'
        },
        publishedAt: new Date('2026-01-25'),
        readTime: 8,
        views: 820,
        likes: 56,
        metaTitle: 'Gestão financeira para clínicas | PrimeCare Blog',
        metaDescription: 'Descubra dicas práticas para melhorar a gestão financeira da sua clínica e aumentar a lucratividade.',
        metaKeywords: ['gestão financeira', 'clínicas', 'lucratividade']
      },
      {
        id: '5',
        slug: 'lgpd-clinicas-medicas',
        title: 'LGPD nas clínicas médicas: guia completo',
        excerpt: 'Tudo o que você precisa saber sobre a Lei Geral de Proteção de Dados no contexto da saúde.',
        content: '<p>O conteúdo completo do artigo...</p>',
        category: 'Compliance',
        tags: ['LGPD', 'privacidade', 'compliance'],
        author: {
          name: 'Dr. Carlos Mendes',
          avatar: 'https://ui-avatars.com/api/?name=Carlos+Mendes&background=1e40af&color=fff'
        },
        publishedAt: new Date('2026-01-27'),
        readTime: 10,
        views: 1120,
        likes: 93,
        metaTitle: 'LGPD nas clínicas médicas: guia completo | PrimeCare',
        metaDescription: 'Guia completo sobre como aplicar a LGPD em clínicas médicas e proteger os dados dos pacientes.',
        metaKeywords: ['LGPD', 'proteção de dados', 'privacidade médica']
      }
    ];

    // Apply filters
    let filtered = allArticles;
    if (filters?.category) {
      filtered = filtered.filter(a => a.category === filters.category);
    }
    if (filters?.tag) {
      filtered = filtered.filter(a => a.tags.includes(filters.tag || ''));
    }
    if (filters?.search) {
      const searchLower = filters.search.toLowerCase();
      filtered = filtered.filter(a =>
        a.title.toLowerCase().includes(searchLower) ||
        a.excerpt.toLowerCase().includes(searchLower)
      );
    }

    // Pagination
    const page = filters?.page || 1;
    const perPage = filters?.perPage || 10;
    const start = (page - 1) * perPage;
    const end = start + perPage;
    const paginatedArticles = filtered.slice(start, end);

    return {
      articles: paginatedArticles,
      total: filtered.length,
      page,
      perPage,
      totalPages: Math.ceil(filtered.length / perPage)
    };
  }

  /**
   * Mock categories
   */
  private getMockCategories(): BlogCategory[] {
    return [
      {
        id: '1',
        name: 'Gestão Clínica',
        slug: 'gestao-clinica',
        description: 'Dicas e estratégias para melhorar a gestão da sua clínica',
        articleCount: 15
      },
      {
        id: '2',
        name: 'Tecnologia',
        slug: 'tecnologia',
        description: 'Novidades e tendências em tecnologia médica',
        articleCount: 12
      },
      {
        id: '3',
        name: 'Telemedicina',
        slug: 'telemedicina',
        description: 'Tudo sobre atendimento médico online',
        articleCount: 8
      },
      {
        id: '4',
        name: 'Gestão Financeira',
        slug: 'gestao-financeira',
        description: 'Finanças e lucratividade para clínicas',
        articleCount: 10
      },
      {
        id: '5',
        name: 'Compliance',
        slug: 'compliance',
        description: 'Regulamentações e boas práticas',
        articleCount: 6
      }
    ];
  }
}
