import { Component, computed, inject } from '@angular/core';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [RouterLink],
  template: `
    <nav class="breadcrumb" aria-label="Breadcrumb">
      @for (item of crumbs(); track item.url) {
        <a [routerLink]="item.url">{{ item.label }}</a>
      }
    </nav>
  `,
  styles: [`.breadcrumb{display:flex;gap:8px;font-size:12px} a{color:var(--color-text-secondary);text-decoration:none}`]
})
export class BreadcrumbComponent {
  private readonly router = inject(Router);
  private readonly currentUrl = computed(() => this.router.url);

  constructor() {
    this.router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe();
  }

  crumbs = computed(() => {
    const segments = this.currentUrl().split('?')[0].split('/').filter(Boolean);
    let current = '';
    return segments.map((segment) => {
      current += `/${segment}`;
      return {
        label: segment.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase()),
        url: current
      };
    });
  });
}
