import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { BlogPostService } from '../../services/blog-post.service';
import { BlogPostSummary, BlogPostsListResponse } from '../../models/blog-post.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-blog-posts-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './blog-posts-list.html',
  styleUrl: './blog-posts-list.scss'
})
export class BlogPostsList implements OnInit {
  posts = signal<BlogPostSummary[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  
  showPublishedOnly = false;

  constructor(
    private blogPostService: BlogPostService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.loading.set(true);
    this.error.set(null);

    this.blogPostService.getAllPosts(this.currentPage, this.pageSize, this.showPublishedOnly).subscribe({
      next: (response: BlogPostsListResponse) => {
        this.posts.set(response.posts);
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar posts');
        this.loading.set(false);
      }
    });
  }

  toggleFilter(): void {
    this.showPublishedOnly = !this.showPublishedOnly;
    this.currentPage = 1;
    this.loadPosts();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadPosts();
    }
  }

  createPost(): void {
    this.router.navigate(['/blog-posts/create']);
  }

  editPost(id: string): void {
    this.router.navigate(['/blog-posts/edit', id]);
  }

  publishPost(post: BlogPostSummary): void {
    if (confirm(`Deseja ${post.isPublished ? 'despublicar' : 'publicar'} este post?`)) {
      this.blogPostService.publishPost(post.id, !post.isPublished).subscribe({
        next: () => {
          this.loadPosts();
        },
        error: (err) => {
          alert(err.error?.message || 'Erro ao atualizar post');
        }
      });
    }
  }

  deletePost(post: BlogPostSummary): void {
    if (confirm(`Tem certeza que deseja excluir o post "${post.title}"?`)) {
      this.blogPostService.deletePost(post.id).subscribe({
        next: () => {
          this.loadPosts();
        },
        error: (err) => {
          alert(err.error?.message || 'Erro ao excluir post');
        }
      });
    }
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    const halfRange = Math.floor(maxPagesToShow / 2);
    
    let startPage = Math.max(1, this.currentPage - halfRange);
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);
    
    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}
