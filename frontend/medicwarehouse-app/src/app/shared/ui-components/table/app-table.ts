import { Component, Input, ContentChildren, QueryList, TemplateRef, AfterContentInit, OnChanges, SimpleChanges, Directive } from '@angular/core';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
}

@Directive({ selector: '[appTableCell]', standalone: true })
export class TableCellDirective {
  @Input('appTableCell') columnKey = '';
  constructor(public templateRef: TemplateRef<any>) {}
}

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, FormsModule, NgTemplateOutlet],
  templateUrl: './app-table.html',
  styleUrl: './app-table.scss'
})
export class AppTableComponent implements OnChanges, AfterContentInit {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() pageSize = 10;
  @Input() showPagination = true;
  @Input() emptyMessage = 'Nenhum registro encontrado.';
  @Input() loading = false;

  @ContentChildren(TableCellDirective) cellDirectives!: QueryList<TableCellDirective>;

  private templateMap = new Map<string, TemplateRef<any>>();

  sortColumn = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  currentPage = 1;

  get sortedData(): any[] {
    if (!this.sortColumn) return [...this.data];
    return [...this.data].sort((a, b) => {
      const av = a[this.sortColumn] ?? '';
      const bv = b[this.sortColumn] ?? '';
      const cmp = av < bv ? -1 : av > bv ? 1 : 0;
      return this.sortDirection === 'asc' ? cmp : -cmp;
    });
  }

  get pagedData(): any[] {
    if (!this.showPagination) return this.sortedData;
    const start = (this.currentPage - 1) * this.pageSize;
    return this.sortedData.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.data.length / this.pageSize));
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  get startIndex(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.data.length);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.currentPage = 1;
    }
  }

  ngAfterContentInit(): void {
    this.buildTemplateMap();
    this.cellDirectives.changes.subscribe(() => this.buildTemplateMap());
  }

  private buildTemplateMap(): void {
    this.templateMap.clear();
    this.cellDirectives?.forEach(d => this.templateMap.set(d.columnKey, d.templateRef));
  }

  getCellTemplate(key: string): TemplateRef<any> | null {
    return this.templateMap.get(key) ?? null;
  }

  sort(column: TableColumn): void {
    if (!column.sortable) return;
    if (this.sortColumn === column.key) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column.key;
      this.sortDirection = 'asc';
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  getCellValue(row: any, key: string): any {
    return key.split('.').reduce((obj, k) => obj?.[k], row);
  }
}
