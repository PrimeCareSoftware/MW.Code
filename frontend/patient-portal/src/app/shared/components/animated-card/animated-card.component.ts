import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-animated-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule],
  template: `
    <mat-card 
      class="animated-card"
      [ngClass]="{
        'clickable': clickable,
        'hoverable': hoverable,
        'elevated': elevated
      }"
      (click)="handleClick($event)">
      <mat-card-content>
        <ng-content></ng-content>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .animated-card {
      position: relative;
      background: hsl(var(--card));
      border: 1px solid hsl(var(--border));
      border-radius: var(--radius-lg);
      transition: all var(--transition-base);
      overflow: hidden;
    }

    .animated-card.elevated {
      box-shadow: var(--shadow-md);
    }

    .animated-card.hoverable:hover {
      transform: translateY(-4px);
      box-shadow: var(--shadow-xl);
      border-color: hsl(var(--primary) / 0.3);
    }

    .animated-card.clickable {
      cursor: pointer;
      user-select: none;
    }

    .animated-card.clickable:active {
      transform: scale(0.98);
    }

    .animated-card::before {
      content: '';
      position: absolute;
      top: 0;
      left: -100%;
      width: 100%;
      height: 100%;
      background: linear-gradient(
        90deg,
        transparent,
        hsl(var(--primary) / 0.05),
        transparent
      );
      transition: left 0.5s;
    }

    .animated-card:hover::before {
      left: 100%;
    }

    mat-card-content {
      padding: var(--spacing-6);
    }
  `]
})
export class AnimatedCardComponent {
  @Input() clickable = false;
  @Input() hoverable = true;
  @Input() elevated = true;
  @Output() cardClick = new EventEmitter<Event>();

  handleClick(event: Event) {
    if (this.clickable) {
      this.cardClick.emit(event);
    }
  }
}
