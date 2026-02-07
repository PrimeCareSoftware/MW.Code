import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ErrorLoggingService } from '../../services/error-logging.service';

@Component({
  selector: 'app-error-boundary',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div *ngIf="!hasError" class="error-boundary-content">
      <ng-content></ng-content>
    </div>
    
    <mat-card *ngIf="hasError" class="error-boundary-fallback" role="alert">
      <mat-card-content>
        <div class="error-icon">
          <mat-icon>error_outline</mat-icon>
        </div>
        <h2>Ops! Algo deu errado</h2>
        <p class="error-message">{{ errorMessage }}</p>
        <p class="error-hint" *ngIf="showHint">
          Tente recarregar a página ou entre em contato com o suporte se o problema persistir.
        </p>
        <div class="error-actions">
          <button mat-raised-button color="primary" (click)="reload()">
            <mat-icon>refresh</mat-icon>
            Recarregar
          </button>
          <button mat-button (click)="goHome()">
            <mat-icon>home</mat-icon>
            Ir para Início
          </button>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .error-boundary-content {
      width: 100%;
    }

    .error-boundary-fallback {
      margin: var(--spacing-4);
      text-align: center;
      
      .error-icon {
        mat-icon {
          font-size: 64px;
          width: 64px;
          height: 64px;
          color: hsl(var(--destructive));
          margin-bottom: var(--spacing-4);
        }
      }

      h2 {
        color: hsl(var(--foreground));
        margin-bottom: var(--spacing-2);
      }

      .error-message {
        color: hsl(var(--muted-foreground));
        margin-bottom: var(--spacing-2);
      }

      .error-hint {
        color: hsl(var(--muted-foreground));
        font-size: var(--text-sm);
        margin-bottom: var(--spacing-4);
      }

      .error-actions {
        display: flex;
        gap: var(--spacing-2);
        justify-content: center;
        flex-wrap: wrap;
      }
    }
  `]
})
export class ErrorBoundaryComponent implements OnInit, OnDestroy {
  @Input() fallbackMessage = 'Ocorreu um erro inesperado';
  @Input() showHint = true;
  
  hasError = false;
  errorMessage = '';
  
  private errorListener?: (event: ErrorEvent) => void;
  private rejectionListener?: (event: PromiseRejectionEvent) => void;

  constructor(
    private errorLogger: ErrorLoggingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Store listener references for cleanup
    this.errorListener = (event: ErrorEvent) => this.handleError(event.error);
    this.rejectionListener = (event: PromiseRejectionEvent) => this.handleError(event.reason);
    
    // Set up global error handler interception
    window.addEventListener('error', this.errorListener);
    window.addEventListener('unhandledrejection', this.rejectionListener);
  }
  
  ngOnDestroy(): void {
    // Clean up event listeners to prevent memory leaks
    if (this.errorListener) {
      window.removeEventListener('error', this.errorListener);
    }
    if (this.rejectionListener) {
      window.removeEventListener('unhandledrejection', this.rejectionListener);
    }
  }

  handleError(error: any): void {
    this.hasError = true;
    this.errorMessage = this.extractErrorMessage(error);
    this.errorLogger.error('Error boundary caught error', 'ErrorBoundary', error);
  }

  private extractErrorMessage(error: any): string {
    if (typeof error === 'string') {
      return error;
    }
    if (error?.message) {
      return error.message;
    }
    if (error?.error?.message) {
      return error.error.message;
    }
    return this.fallbackMessage;
  }

  reload(): void {
    window.location.reload();
  }

  goHome(): void {
    this.router.navigate(['/dashboard']);
  }

  reset(): void {
    this.hasError = false;
    this.errorMessage = '';
  }
}
