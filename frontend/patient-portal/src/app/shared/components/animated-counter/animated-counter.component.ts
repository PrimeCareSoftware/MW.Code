import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-animated-counter',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="animated-counter">{{ displayValue }}</span>
  `,
  styles: [`
    .animated-counter {
      font-weight: var(--font-extrabold);
      font-size: inherit;
      color: inherit;
    }
  `]
})
export class AnimatedCounterComponent implements OnInit, OnDestroy {
  @Input() value = 0;
  @Input() duration = 1000; // ms
  @Input() prefix = '';
  @Input() suffix = '';

  displayValue = '0';
  private animationFrame: number | null = null;
  private startTime?: number;
  private startValue = 0;

  ngOnInit() {
    this.animateCount();
  }

  ngOnDestroy() {
    if (this.animationFrame !== null) {
      cancelAnimationFrame(this.animationFrame);
    }
  }

  private animateCount() {
    const animate = (timestamp: number) => {
      if (!this.startTime) {
        this.startTime = timestamp;
      }

      const progress = Math.min((timestamp - this.startTime) / this.duration, 1);
      const currentValue = Math.floor(this.startValue + (this.value - this.startValue) * this.easeOutCubic(progress));
      
      this.displayValue = `${this.prefix}${currentValue}${this.suffix}`;

      if (progress < 1) {
        this.animationFrame = requestAnimationFrame(animate);
      }
    };

    this.animationFrame = requestAnimationFrame(animate);
  }

  private easeOutCubic(t: number): number {
    return 1 - Math.pow(1 - t, 3);
  }
}
