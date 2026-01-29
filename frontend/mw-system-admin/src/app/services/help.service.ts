import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface HelpArticle {
  id: string;
  title: string;
  excerpt: string;
  content: string;
  category: string;
  tags: string[];
  readTime: number;
  views: number;
  helpful: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface HelpCategory {
  id: string;
  name: string;
  icon: string;
  description: string;
  articleCount: number;
}

export interface HelpVideo {
  id: string;
  title: string;
  description: string;
  thumbnail: string;
  duration: string;
  url: string;
  category: string;
}

@Injectable({
  providedIn: 'root'
})
export class HelpService {
  
  private categories: HelpCategory[] = [
    {
      id: 'getting-started',
      name: 'Primeiros Passos',
      icon: 'rocket_launch',
      description: 'Aprenda o básico do System Admin',
      articleCount: 12
    },
    {
      id: 'customer-management',
      name: 'Gestão de Clientes',
      icon: 'people',
      description: 'Como gerenciar suas clínicas',
      articleCount: 18
    },
    {
      id: 'analytics',
      name: 'Relatórios e Analytics',
      icon: 'analytics',
      description: 'Entenda suas métricas',
      articleCount: 15
    },
    {
      id: 'automation',
      name: 'Automações',
      icon: 'smart_toy',
      description: 'Configure workflows automatizados',
      articleCount: 10
    },
    {
      id: 'billing',
      name: 'Faturamento',
      icon: 'receipt_long',
      description: 'Gerencie planos e pagamentos',
      articleCount: 8
    },
    {
      id: 'support',
      name: 'Suporte',
      icon: 'support_agent',
      description: 'Como obter ajuda',
      articleCount: 6
    }
  ];
  
  private articles: HelpArticle[] = [
    {
      id: '1',
      title: 'Como criar uma nova clínica',
      excerpt: 'Aprenda a adicionar e configurar uma nova clínica no sistema.',
      content: `# Como criar uma nova clínica\n\nPara criar uma nova clínica...\n\n1. Navegue até Clínicas\n2. Clique em "Nova Clínica"\n3. Preencha os dados...`,
      category: 'getting-started',
      tags: ['clínicas', 'cadastro', 'início'],
      readTime: 5,
      views: 1234,
      helpful: 98,
      createdAt: new Date('2024-01-01'),
      updatedAt: new Date('2024-01-15')
    },
    {
      id: '2',
      title: 'Entendendo as métricas de MRR',
      excerpt: 'Saiba como o MRR é calculado e como interpretá-lo.',
      content: `# Entendendo as métricas de MRR\n\nO MRR (Monthly Recurring Revenue) representa...`,
      category: 'analytics',
      tags: ['mrr', 'métricas', 'financeiro'],
      readTime: 8,
      views: 892,
      helpful: 87,
      createdAt: new Date('2024-01-05'),
      updatedAt: new Date('2024-01-20')
    },
    {
      id: '3',
      title: 'Configurando workflows de automação',
      excerpt: 'Configure fluxos automatizados para economia de tempo.',
      content: `# Configurando workflows de automação\n\nOs workflows permitem automatizar...`,
      category: 'automation',
      tags: ['workflows', 'automação', 'produtividade'],
      readTime: 12,
      views: 567,
      helpful: 76,
      createdAt: new Date('2024-01-10'),
      updatedAt: new Date('2024-01-25')
    }
  ];
  
  private videos: HelpVideo[] = [
    {
      id: 'v1',
      title: 'Tour completo do System Admin',
      description: 'Uma visão geral de todas as funcionalidades',
      thumbnail: '/assets/video-thumbnails/tour.jpg',
      duration: '12:34',
      url: 'https://youtube.com/watch?v=example1',
      category: 'getting-started'
    },
    {
      id: 'v2',
      title: 'Gerenciando clínicas e usuários',
      description: 'Como adicionar e gerenciar clínicas',
      thumbnail: '/assets/video-thumbnails/clinics.jpg',
      duration: '8:45',
      url: 'https://youtube.com/watch?v=example2',
      category: 'customer-management'
    },
    {
      id: 'v3',
      title: 'Criando relatórios personalizados',
      description: 'Aprenda a criar relatórios customizados',
      thumbnail: '/assets/video-thumbnails/reports.jpg',
      duration: '15:20',
      url: 'https://youtube.com/watch?v=example3',
      category: 'analytics'
    }
  ];
  
  getCategories(): Observable<HelpCategory[]> {
    return of(this.categories);
  }
  
  getArticlesByCategory(categoryId: string): Observable<HelpArticle[]> {
    const filtered = this.articles.filter(a => a.category === categoryId);
    return of(filtered);
  }
  
  searchArticles(query: string): Observable<HelpArticle[]> {
    const lowerQuery = query.toLowerCase();
    const filtered = this.articles.filter(article => 
      article.title.toLowerCase().includes(lowerQuery) ||
      article.excerpt.toLowerCase().includes(lowerQuery) ||
      article.tags.some(tag => tag.toLowerCase().includes(lowerQuery))
    );
    return of(filtered);
  }
  
  getArticleById(id: string): HelpArticle | undefined {
    return this.articles.find(a => a.id === id);
  }
  
  getPopularArticles(limit: number = 5): Observable<HelpArticle[]> {
    const sorted = [...this.articles].sort((a, b) => b.views - a.views);
    return of(sorted.slice(0, limit));
  }
  
  getVideos(): Observable<HelpVideo[]> {
    return of(this.videos);
  }
  
  getVideosByCategory(categoryId: string): Observable<HelpVideo[]> {
    const filtered = this.videos.filter(v => v.category === categoryId);
    return of(filtered);
  }
  
  markArticleAsHelpful(articleId: string): void {
    const article = this.articles.find(a => a.id === articleId);
    if (article) {
      article.helpful++;
      localStorage.setItem(`article-${articleId}-helpful`, 'true');
    }
  }
  
  incrementArticleView(articleId: string): void {
    const article = this.articles.find(a => a.id === articleId);
    if (article) {
      article.views++;
    }
  }
}
