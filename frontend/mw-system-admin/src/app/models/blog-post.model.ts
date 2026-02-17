export interface BlogPost {
  id: string;
  title: string;
  slug: string;
  content: string;
  excerpt: string;
  category: string;
  featuredImage?: string;
  readTimeMinutes: number;
  isPublished: boolean;
  publishedAt?: string;
  authorName: string;
  authorId: string;
  createdAt: string;
  updatedAt?: string;
}

export interface BlogPostSummary {
  id: string;
  title: string;
  slug: string;
  excerpt: string;
  category: string;
  featuredImage?: string;
  readTimeMinutes: number;
  isPublished: boolean;
  publishedAt?: string;
  authorName: string;
  createdAt: string;
}

export interface CreateBlogPostRequest {
  title: string;
  content: string;
  excerpt: string;
  category: string;
  featuredImage?: string;
  readTimeMinutes: number;
}

export interface UpdateBlogPostRequest {
  title: string;
  content: string;
  excerpt: string;
  category: string;
  featuredImage?: string;
  readTimeMinutes: number;
}

export interface BlogPostsListResponse {
  posts: BlogPostSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PublishBlogPostRequest {
  isPublished: boolean;
}
