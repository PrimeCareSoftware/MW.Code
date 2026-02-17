import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BlogPostService } from '../../services/blog-post.service';
import { BlogPost, CreateBlogPostRequest, UpdateBlogPostRequest } from '../../models/blog-post.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-blog-post-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './blog-post-editor.html',
  styleUrl: './blog-post-editor.scss'
})
export class BlogPostEditor implements OnInit {
  loading = signal(false);
  saving = signal(false);
  error = signal<string | null>(null);
  
  isEditMode = false;
  postId: string | null = null;

  formData = {
    title: '',
    content: '',
    excerpt: '',
    category: '',
    featuredImage: '',
    readTimeMinutes: 5
  };

  constructor(
    private blogPostService: BlogPostService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.postId = this.route.snapshot.paramMap.get('id');
    
    if (this.postId) {
      this.isEditMode = true;
      this.loadPost(this.postId);
    }
  }

  loadPost(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.blogPostService.getPostById(id).subscribe({
      next: (post: BlogPost) => {
        this.formData = {
          title: post.title,
          content: post.content,
          excerpt: post.excerpt,
          category: post.category,
          featuredImage: post.featuredImage || '',
          readTimeMinutes: post.readTimeMinutes
        };
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar post');
        this.loading.set(false);
      }
    });
  }

  save(): void {
    if (!this.validateForm()) {
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    if (this.isEditMode && this.postId) {
      const request: UpdateBlogPostRequest = {
        title: this.formData.title,
        content: this.formData.content,
        excerpt: this.formData.excerpt,
        category: this.formData.category,
        featuredImage: this.formData.featuredImage || undefined,
        readTimeMinutes: this.formData.readTimeMinutes
      };

      this.blogPostService.updatePost(this.postId, request).subscribe({
        next: () => {
          this.saving.set(false);
          alert('Post atualizado com sucesso!');
          this.router.navigate(['/blog-posts']);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Erro ao atualizar post');
          this.saving.set(false);
        }
      });
    } else {
      const request: CreateBlogPostRequest = {
        title: this.formData.title,
        content: this.formData.content,
        excerpt: this.formData.excerpt,
        category: this.formData.category,
        featuredImage: this.formData.featuredImage || undefined,
        readTimeMinutes: this.formData.readTimeMinutes
      };

      this.blogPostService.createPost(request).subscribe({
        next: () => {
          this.saving.set(false);
          alert('Post criado com sucesso!');
          this.router.navigate(['/blog-posts']);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Erro ao criar post');
          this.saving.set(false);
        }
      });
    }
  }

  validateForm(): boolean {
    if (!this.formData.title.trim()) {
      alert('Por favor, preencha o título do post');
      return false;
    }
    if (!this.formData.content.trim()) {
      alert('Por favor, adicione o conteúdo do post');
      return false;
    }
    if (!this.formData.excerpt.trim()) {
      alert('Por favor, adicione um resumo do post');
      return false;
    }
    if (!this.formData.category.trim()) {
      alert('Por favor, selecione uma categoria');
      return false;
    }
    if (this.formData.readTimeMinutes < 1) {
      alert('O tempo de leitura deve ser pelo menos 1 minuto');
      return false;
    }
    return true;
  }

  cancel(): void {
    if (confirm('Deseja cancelar? As alterações não salvas serão perdidas.')) {
      this.router.navigate(['/blog-posts']);
    }
  }

  // Simple text formatting helpers
  formatText(command: string, value: string | null = null): void {
    const textarea = document.getElementById('content-editor') as HTMLTextAreaElement;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = this.formData.content.substring(start, end);
    
    let formattedText = '';
    
    switch(command) {
      case 'bold':
        formattedText = `**${selectedText || 'texto em negrito'}**`;
        break;
      case 'italic':
        formattedText = `*${selectedText || 'texto em itálico'}*`;
        break;
      case 'heading2':
        formattedText = `\n## ${selectedText || 'Título'}\n`;
        break;
      case 'heading3':
        formattedText = `\n### ${selectedText || 'Subtítulo'}\n`;
        break;
      case 'list':
        formattedText = `\n- ${selectedText || 'Item da lista'}\n`;
        break;
      case 'link':
        const url = prompt('Digite a URL:') || '';
        formattedText = `[${selectedText || 'texto do link'}](${url})`;
        break;
      case 'image':
        const imgUrl = prompt('Digite a URL da imagem:') || '';
        formattedText = `\n![${selectedText || 'descrição da imagem'}](${imgUrl})\n`;
        break;
    }
    
    this.formData.content = 
      this.formData.content.substring(0, start) +
      formattedText +
      this.formData.content.substring(end);
    
    // Set cursor position after inserted text
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(start + formattedText.length, start + formattedText.length);
    }, 0);
  }
}
