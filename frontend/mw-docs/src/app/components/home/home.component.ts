import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DocumentationService } from '../../services/documentation.service';
import { DocCategory, DocItem } from '../../models/doc-item.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  categories: DocCategory[] = [];
  filteredCategories: DocCategory[] = [];
  searchQuery = '';

  constructor(private docService: DocumentationService) {}

  ngOnInit() {
    this.categories = this.docService.getCategories();
    this.filteredCategories = this.categories;
  }

  onSearch() {
    if (!this.searchQuery.trim()) {
      this.filteredCategories = this.categories;
      return;
    }

    const searchResults = this.docService.searchDocs(this.searchQuery);
    
    // Group results by category
    const categoryMap = new Map<string, DocItem[]>();
    searchResults.forEach(doc => {
      const categoryName = this.categories.find(cat => 
        cat.docs.some(d => d.id === doc.id)
      )?.name || 'Outros';
      
      if (!categoryMap.has(categoryName)) {
        categoryMap.set(categoryName, []);
      }
      categoryMap.get(categoryName)!.push(doc);
    });

    this.filteredCategories = Array.from(categoryMap.entries()).map(([name, docs]) => ({
      name,
      icon: this.categories.find(c => c.name === name)?.icon || '📄',
      docs
    }));
  }

  clearSearch() {
    this.searchQuery = '';
    this.filteredCategories = this.categories;
  }

  getTotalDocs(): number {
    return this.categories.reduce((sum, cat) => sum + cat.docs.length, 0);
  }
}
