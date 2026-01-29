import { Component, Input, Output, EventEmitter } from '@angular/core';

export interface TableColumn {
  key: string;
  header: string;
  sortable?: boolean;
}

export interface TableRow {
  [key: string]: any;
}

export interface SortEvent {
  column: string;
  direction: 'asc' | 'desc';
}

/**
 * Accessible table component following WCAG 2.1 AA guidelines
 * - Proper table headers with scope attributes
 * - Sortable columns with ARIA labels
 * - Keyboard navigation support
 * - Screen reader friendly
 * 
 * **Note:** The onSort method mutates the data array by sorting it in place.
 * If the parent component needs to maintain the original data array reference,
 * consider handling the sort event and managing sorting externally.
 */
@Component({
  selector: 'app-accessible-table',
  template: `
    <div class="accessible-table-wrapper" role="region" [attr.aria-label]="caption" tabindex="0">
      <table
        class="accessible-table"
        [attr.aria-label]="caption"
        [attr.aria-describedby]="summaryId"
      >
        <caption *ngIf="showCaption">{{ caption }}</caption>
        
        <thead>
          <tr>
            <th
              *ngFor="let column of columns"
              scope="col"
              [id]="'header-' + column.key"
              [attr.aria-sort]="getSortState(column.key)"
              [class.sortable]="column.sortable"
            >
              <button
                *ngIf="column.sortable"
                type="button"
                (click)="onSort(column.key)"
                (keydown.enter)="onSort(column.key)"
                (keydown.space)="$event.preventDefault(); onSort(column.key)"
                [attr.aria-label]="getSortAriaLabel(column)"
                class="sort-button"
              >
                {{ column.header }}
                <span class="sort-icon" aria-hidden="true">
                  {{ getSortIcon(column.key) }}
                </span>
              </button>
              <span *ngIf="!column.sortable">{{ column.header }}</span>
            </th>
          </tr>
        </thead>
        
        <tbody>
          <tr *ngFor="let row of data; let i = index" [attr.aria-rowindex]="i + 2">
            <td *ngFor="let column of columns" [attr.headers]="'header-' + column.key">
              {{ row[column.key] }}
            </td>
          </tr>
          
          <tr *ngIf="data.length === 0">
            <td [attr.colspan]="columns.length" class="empty-state">
              {{ emptyMessage }}
            </td>
          </tr>
        </tbody>
      </table>
      
      <div *ngIf="summary" [id]="summaryId" class="sr-only">
        {{ summary }}
      </div>
    </div>
  `,
  styles: [`
    .accessible-table-wrapper {
      overflow-x: auto;
      -webkit-overflow-scrolling: touch;
    }

    .accessible-table-wrapper:focus {
      outline: 3px solid #2196f3;
      outline-offset: 2px;
    }

    .accessible-table {
      width: 100%;
      border-collapse: collapse;
      border: 1px solid #ddd;
    }

    .accessible-table th,
    .accessible-table td {
      padding: 12px;
      text-align: left;
      border: 1px solid #ddd;
    }

    .accessible-table th {
      background-color: #f5f5f5;
      font-weight: bold;
      color: #333;
    }

    .sort-button {
      background: none;
      border: none;
      padding: 0;
      font: inherit;
      cursor: pointer;
      color: inherit;
      text-align: left;
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .sort-button:focus-visible {
      outline: 2px solid #2196f3;
      outline-offset: 2px;
    }

    .sort-icon {
      margin-left: 8px;
      font-size: 0.875rem;
    }

    .empty-state {
      text-align: center;
      color: #666;
      font-style: italic;
      padding: 24px;
    }

    .sr-only {
      position: absolute;
      left: -10000px;
      width: 1px;
      height: 1px;
      overflow: hidden;
    }

    caption {
      text-align: left;
      font-weight: bold;
      padding: 12px;
      caption-side: top;
    }
  `]
})
export class AccessibleTableComponent {
  @Input() caption = 'Data table';
  @Input() columns: TableColumn[] = [];
  @Input() data: TableRow[] = [];
  @Input() summary?: string;
  @Input() showCaption = false;
  @Input() emptyMessage = 'Nenhum dado disponível';
  @Output() sorted = new EventEmitter<SortEvent>();
  
  sortColumn?: string;
  sortDirection: 'asc' | 'desc' | 'none' = 'none';
  summaryId = `table-summary-${Date.now()}-${Math.floor(Math.random() * 1000)}`;

  getSortState(columnKey: string): 'ascending' | 'descending' | 'none' {
    if (this.sortColumn !== columnKey) {
      return 'none';
    }
    return this.sortDirection === 'asc' ? 'ascending' : 
           this.sortDirection === 'desc' ? 'descending' : 'none';
  }

  getSortIcon(columnKey: string): string {
    if (this.sortColumn !== columnKey) {
      return '⇅';
    }
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  getSortAriaLabel(column: TableColumn): string {
    const state = this.getSortState(column.key);
    if (state === 'none') {
      return `Ordenar por ${column.header}`;
    }
    const direction = state === 'ascending' ? 'ascendente' : 'descendente';
    return `${column.header}, ordenado ${direction}. Clique para inverter ordem.`;
  }

  onSort(columnKey: string): void {
    if (this.sortColumn === columnKey) {
      // Toggle direction
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columnKey;
      this.sortDirection = 'asc';
    }
    
    // Emit sort event for parent to handle
    this.sorted.emit({
      column: columnKey,
      direction: this.sortDirection
    });
    
    // Sort the data locally
    this.data = [...this.data].sort((a, b) => {
      const aVal = a[columnKey];
      const bVal = b[columnKey];
      const modifier = this.sortDirection === 'asc' ? 1 : -1;
      
      if (aVal < bVal) return -1 * modifier;
      if (aVal > bVal) return 1 * modifier;
      return 0;
    });
  }
}
