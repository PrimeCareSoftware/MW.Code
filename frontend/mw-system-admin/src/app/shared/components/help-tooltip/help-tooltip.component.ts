import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HelpService, HelpArticle } from '../../../services/help.service';

/**
 * Tooltip component for contextual help
 */
@Component({
  selector: 'app-help-tooltip',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  template: `
    <div class="help-tooltip" *ngIf="article">
      <div class="help-tooltip-header">
        <h4>{{ article.title }}</h4>
        <button mat-icon-button (click)="onClose()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>
      <div class="help-tooltip-content">
        <p>{{ article.content }}</p>
        <div class="help-tooltip-actions" *ngIf="article.id">
          <button mat-button color="primary" (click)="openFullArticle()">
            Ler mais
          </button>
        </div>
      </div>
    </div>
    <div class="help-tooltip" *ngIf="!article">
      <div class="help-tooltip-header">
        <h4>Ajuda não disponível</h4>
        <button mat-icon-button (click)="onClose()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>
      <div class="help-tooltip-content">
        <p>Desculpe, a ajuda para este item não está disponível no momento.</p>
      </div>
    </div>
  `,
  styles: [`
    .help-tooltip {
      background: var(--surface, white);
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      max-width: 400px;
      overflow: hidden;
    }

    .help-tooltip-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px;
      border-bottom: 1px solid var(--divider, #e0e0e0);
      background: var(--primary-50, #f5f5f5);
    }

    .help-tooltip-header h4 {
      margin: 0;
      font-size: 16px;
      font-weight: 600;
      color: var(--text-primary, #212121);
    }

    .close-button {
      margin-left: auto;
      width: 32px;
      height: 32px;
    }

    .help-tooltip-content {
      padding: 16px;
    }

    .help-tooltip-content p {
      margin: 0 0 16px 0;
      font-size: 14px;
      line-height: 1.6;
      color: var(--text-secondary, #757575);
    }

    .help-tooltip-actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 16px;
    }

    /* Dark mode support */
    .theme-dark .help-tooltip {
      background: var(--surface-dark, #2d2d2d);
    }

    .theme-dark .help-tooltip-header {
      background: var(--primary-dark, #424242);
      border-bottom-color: var(--divider-dark, #424242);
    }

    .theme-dark .help-tooltip-header h4 {
      color: var(--text-primary-dark, #e0e0e0);
    }

    .theme-dark .help-tooltip-content p {
      color: var(--text-secondary-dark, #b0b0b0);
    }

    /* High contrast mode support */
    .theme-high-contrast .help-tooltip {
      border: 2px solid var(--primary, #4caf50);
    }

    .theme-high-contrast .help-tooltip-header {
      border-bottom-width: 2px;
    }
  `]
})
export class HelpTooltipComponent implements OnInit {
  @Input() articleId: string = '';
  @Output() close = new EventEmitter<void>();

  private helpService = inject(HelpService);
  
  article: HelpArticle | null = null;

  ngOnInit() {
    if (this.articleId) {
      this.article = this.helpService.getArticleById(this.articleId) || null;
    }
  }

  onClose() {
    this.close.emit();
  }

  openFullArticle() {
    // Navigate to full article or open in dialog
    // For now, just close the tooltip
    this.onClose();
  }
}
