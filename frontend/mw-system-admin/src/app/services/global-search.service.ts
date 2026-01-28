import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { GlobalSearchResult } from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class GlobalSearchService {
  private apiUrl = `${environment.apiUrl}/system-admin/search`;

  constructor(private http: HttpClient) {}

  /**
   * Global search across all entities
   */
  search(query: string, maxResults: number = 50): Observable<GlobalSearchResult> {
    return this.http.get<GlobalSearchResult>(this.apiUrl, {
      params: {
        q: query,
        maxResults: maxResults.toString()
      }
    });
  }

  /**
   * Get recent searches from localStorage
   */
  getRecentSearches(): string[] {
    const recent = localStorage.getItem('recentSearches');
    return recent ? JSON.parse(recent) : [];
  }

  /**
   * Save search to history
   */
  saveSearchToHistory(query: string): void {
    const recent = this.getRecentSearches();
    const updated = [query, ...recent.filter(q => q !== query)].slice(0, 10);
    localStorage.setItem('recentSearches', JSON.stringify(updated));
  }

  /**
   * Clear search history
   */
  clearSearchHistory(): void {
    localStorage.removeItem('recentSearches');
  }
}
