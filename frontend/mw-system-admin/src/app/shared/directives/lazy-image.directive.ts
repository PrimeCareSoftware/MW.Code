import { Directive, ElementRef, Input, OnInit, OnDestroy } from '@angular/core';

const DEFAULT_PLACEHOLDER = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 300"%3E%3Crect fill="%23f0f0f0" width="400" height="300"/%3E%3C/svg%3E';
const ERROR_PLACEHOLDER = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 300"%3E%3Crect fill="%23fee" width="400" height="300"/%3E%3Ctext x="50%25" y="50%25" text-anchor="middle"%3EImage failed%3C/text%3E%3C/svg%3E';

@Directive({
  selector: 'img[appLazyImage]',
  standalone: true
})
export class LazyImageDirective implements OnInit, OnDestroy {
  @Input() appLazyImage!: string;
  @Input() placeholder = DEFAULT_PLACEHOLDER;
  
  private observer?: IntersectionObserver;
  
  constructor(private el: ElementRef<HTMLImageElement>) {}
  
  ngOnInit(): void {
    const img = this.el.nativeElement;
    
    // Set placeholder
    img.src = this.placeholder;
    img.classList.add('lazy-loading');
    
    // Create intersection observer
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            this.loadImage();
            this.observer?.disconnect();
          }
        });
      },
      {
        rootMargin: '50px', // Start loading 50px before image is visible
        threshold: 0.01
      }
    );
    
    this.observer.observe(img);
  }
  
  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
  
  private loadImage(): void {
    const img = this.el.nativeElement;
    
    // Create a new image to load
    const tempImg = new Image();
    
    tempImg.onload = () => {
      img.src = this.appLazyImage;
      img.classList.remove('lazy-loading');
      img.classList.add('lazy-loaded');
    };
    
    tempImg.onerror = () => {
      console.error(`Failed to load image: ${this.appLazyImage}`);
      img.src = ERROR_PLACEHOLDER;
      img.alt = `Failed to load image: ${img.alt}`;
      img.classList.remove('lazy-loading');
      img.classList.add('lazy-error');
    };
    
    tempImg.src = this.appLazyImage;
  }
}
