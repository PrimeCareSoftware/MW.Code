import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  BlogPost, 
  BlogPostSummary, 
  BlogPostsListResponse, 
  CreateBlogPostRequest, 
  UpdateBlogPostRequest,
  PublishBlogPostRequest 
} from '../models/blog-post.model';

@Injectable({
  providedIn: 'root'
})
export class BlogPostService {
  private readonly baseUrl = '/api/blog-posts';

  constructor(private http: HttpClient) {}

  // Public methods
  getPublishedPosts(page = 1, pageSize = 10): Observable<BlogPostsListResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<BlogPostsListResponse>(`${this.baseUrl}/published`, { params });
  }

  getPostById(id: string): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${this.baseUrl}/${id}`);
  }

  getPostBySlug(slug: string): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${this.baseUrl}/slug/${slug}`);
  }

  getPostsByCategory(category: string): Observable<BlogPostSummary[]> {
    return this.http.get<BlogPostSummary[]>(`${this.baseUrl}/category/${category}`);
  }

  // Admin methods
  getAllPosts(page = 1, pageSize = 10, publishedOnly = false): Observable<BlogPostsListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (publishedOnly) {
      params = params.set('publishedOnly', 'true');
    }

    return this.http.get<BlogPostsListResponse>(`${this.baseUrl}/admin/all`, { params });
  }

  createPost(request: CreateBlogPostRequest): Observable<BlogPost> {
    return this.http.post<BlogPost>(this.baseUrl, request);
  }

  updatePost(id: string, request: UpdateBlogPostRequest): Observable<BlogPost> {
    return this.http.put<BlogPost>(`${this.baseUrl}/${id}`, request);
  }

  publishPost(id: string, isPublished: boolean): Observable<any> {
    const request: PublishBlogPostRequest = { isPublished };
    return this.http.patch<any>(`${this.baseUrl}/${id}/publish`, request);
  }

  deletePost(id: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }
}
