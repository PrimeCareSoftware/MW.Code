import { Directive, ElementRef, Input, OnInit } from '@angular/core';

@Directive({
  selector: 'img[appLazyImage]',
  standalone: true
})
export class LazyImageDirective implements OnInit {
  @Input() appLazyImage!: string;
  @Input() placeholder = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 300"%3E%3Crect fill="%23f0f0f0" width="400" height="300"/%3E%3C/svg%3E';
  
  constructor(private el: ElementRef<HTMLImageElement>) {}
  
  ngOnInit(): void {
    const img = this.el.nativeElement;
    
    // Set placeholder
    img.src = this.placeholder;
    img.classList.add('lazy-loading');
    
    // Create intersection observer
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            this.loadImage();
            observer.disconnect();
          }
        });
      },
      {
        rootMargin: '50px', // Start loading 50px before image is visible
        threshold: 0.01
      }
    );
    
    observer.observe(img);
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
      img.classList.remove('lazy-loading');
      img.classList.add('lazy-error');
    };
    
    tempImg.src = this.appLazyImage;
  }
}
