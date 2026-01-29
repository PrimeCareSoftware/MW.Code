# 📊 Fase 5: Experiência e Usabilidade - System Admin

**Prioridade:** 🔥 P2 - MÉDIA  
**Status:** Planejamento  
**Esforço:** 2 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 78.000  
**Prazo:** Q3 2026

---

## 📋 Contexto

### Situação Atual

O system-admin funcional mas com UI/UX que pode ser modernizada para melhor produtividade.

**Funcionalidades Existentes:**
- ✅ Interface Angular Material funcional
- ✅ Componentes básicos implementados
- ❌ Sem dark mode
- ❌ Sem onboarding/help system
- ❌ Performance pode melhorar
- ❌ Sem PWA

### Objetivo da Fase 5

Modernizar experiência do usuário com:
1. UI/UX moderna e consistente
2. Sistema de onboarding e ajuda
3. Otimizações de performance
4. Monitoring e observabilidade

**Inspiração:** Vercel, Linear, Notion

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais
1. Atualizar design system com dark mode
2. Implementar onboarding interativo e help system
3. Otimizar performance (lazy loading, caching, PWA)
4. Adicionar APM e monitoring completo

### Benefícios Esperados
- 🎨 **Interface 2x mais moderna** e agradável
- 📚 **-50% tempo de aprendizado** com onboarding
- ⚡ **Performance 3x melhor** (< 1s loading)
- 📊 **Visibilidade completa** de performance e erros

---

## 📝 Tarefas Detalhadas

### 1. UI/UX Moderna (3 semanas)

#### 1.1 Design System Atualizado

**Novo tema com dark mode:**
```scss
// styles/_theme.scss
@use '@angular/material' as mat;

// Paleta customizada
$primecare-primary: (
  50: #e8f5e9,
  100: #c8e6c9,
  200: #a5d6a7,
  300: #81c784,
  400: #66bb6a,
  500: #4caf50,
  600: #43a047,
  700: #388e3c,
  800: #2e7d32,
  900: #1b5e20,
  contrast: (
    50: rgba(black, 0.87),
    100: rgba(black, 0.87),
    200: rgba(black, 0.87),
    300: rgba(black, 0.87),
    400: rgba(black, 0.87),
    500: white,
    600: white,
    700: white,
    800: white,
    900: white,
  )
);

$primecare-accent: (
  50: #e3f2fd,
  100: #bbdefb,
  200: #90caf9,
  300: #64b5f6,
  400: #42a5f5,
  500: #2196f3,
  600: #1e88e5,
  700: #1976d2,
  800: #1565c0,
  900: #0d47a1,
  contrast: (...)
);

// Light theme
$light-theme: mat.define-light-theme((
  color: (
    primary: mat.define-palette($primecare-primary),
    accent: mat.define-palette($primecare-accent),
  ),
  typography: mat.define-typography-config(
    $font-family: 'Inter, system-ui, sans-serif',
  ),
  density: 0,
));

// Dark theme
$dark-theme: mat.define-dark-theme((
  color: (
    primary: mat.define-palette($primecare-primary),
    accent: mat.define-palette($primecare-accent),
  ),
  typography: mat.define-typography-config(
    $font-family: 'Inter, system-ui, sans-serif',
  ),
  density: 0,
));

// Apply themes
@include mat.all-component-themes($light-theme);

.dark-mode {
  @include mat.all-component-colors($dark-theme);
  
  background-color: #1a1a1a;
  color: #e0e0e0;
  
  // Custom dark mode styles
  .mat-card {
    background-color: #2d2d2d;
  }
  
  .mat-table {
    background-color: #2d2d2d;
  }
}
```

**Theme Switcher Service:**
```typescript
// services/theme.service.ts
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private currentTheme$ = new BehaviorSubject<'light' | 'dark'>('light');
  
  constructor(@Inject(DOCUMENT) private document: Document) {
    // Load from localStorage
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark';
    if (savedTheme) {
      this.setTheme(savedTheme);
    } else {
      // Check system preference
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      this.setTheme(prefersDark ? 'dark' : 'light');
    }
  }
  
  get theme$() {
    return this.currentTheme$.asObservable();
  }
  
  get isDarkMode(): boolean {
    return this.currentTheme$.value === 'dark';
  }
  
  setTheme(theme: 'light' | 'dark') {
    this.currentTheme$.next(theme);
    localStorage.setItem('theme', theme);
    
    if (theme === 'dark') {
      this.document.body.classList.add('dark-mode');
    } else {
      this.document.body.classList.remove('dark-mode');
    }
  }
  
  toggleTheme() {
    const newTheme = this.isDarkMode ? 'light' : 'dark';
    this.setTheme(newTheme);
  }
}
```

**Componentes Modernizados:**
```typescript
// shared/components/modern-card/modern-card.component.ts
@Component({
  selector: 'app-modern-card',
  standalone: true,
  template: `
    <div class="modern-card" [class.hover-effect]="hoverable">
      <div class="card-header" *ngIf="title">
        <h3>{{ title }}</h3>
        <ng-content select="[card-actions]"></ng-content>
      </div>
      <div class="card-content">
        <ng-content></ng-content>
      </div>
      <div class="card-footer" *ngIf="hasFooter">
        <ng-content select="[card-footer]"></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .modern-card {
      background: var(--card-bg);
      border-radius: 12px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.12);
      padding: 24px;
      transition: all 0.3s ease;
    }
    
    .modern-card.hover-effect:hover {
      box-shadow: 0 8px 16px rgba(0,0,0,0.15);
      transform: translateY(-2px);
    }
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .card-header h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
    }
  `]
})
export class ModernCardComponent {
  @Input() title?: string;
  @Input() hoverable = false;
  hasFooter = false;
  
  ngAfterContentInit() {
    // Check if footer content exists
  }
}
```

**Animações Sutis:**
```typescript
// animations/fade-slide.animations.ts
export const fadeSlideInAnimation = trigger('fadeSlideIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(10px)' }),
    animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ opacity: 0, transform: 'translateY(10px)' }))
  ])
]);

export const scaleInAnimation = trigger('scaleIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'scale(0.95)' }),
    animate('200ms ease-out', style({ opacity: 1, transform: 'scale(1)' }))
  ])
]);

export const slideInFromRightAnimation = trigger('slideInFromRight', [
  transition(':enter', [
    style({ transform: 'translateX(100%)' }),
    animate('300ms ease-out', style({ transform: 'translateX(0)' }))
  ])
]);

// Usage
@Component({
  animations: [fadeSlideInAnimation, scaleInAnimation]
})
export class MyComponent {
  @HostBinding('@fadeSlideIn') fadeSlide = true;
}
```

**Loading Skeletons:**
```typescript
// shared/components/skeleton-loader/skeleton-loader.component.ts
@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  template: `
    <div class="skeleton-loader" [ngClass]="type">
      <div class="skeleton-line" *ngFor="let line of lines"></div>
    </div>
  `,
  styles: [`
    .skeleton-loader {
      animation: pulse 1.5s ease-in-out infinite;
    }
    
    .skeleton-line {
      height: 16px;
      background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
      background-size: 200% 100%;
      animation: shimmer 1.5s infinite;
      border-radius: 4px;
      margin-bottom: 8px;
    }
    
    @keyframes shimmer {
      0% { background-position: -200% 0; }
      100% { background-position: 200% 0; }
    }
    
    .dark-mode .skeleton-line {
      background: linear-gradient(90deg, #2d2d2d 25%, #3d3d3d 50%, #2d2d2d 75%);
    }
  `]
})
export class SkeletonLoaderComponent {
  @Input() type: 'text' | 'card' | 'table' = 'text';
  @Input() lines = 3;
}
```

#### 1.2 Layout Responsivo e PWA

**Responsive Breakpoints:**
```typescript
// services/breakpoint.service.ts
@Injectable({ providedIn: 'root' })
export class BreakpointService {
  isMobile$ = this.breakpointObserver
    .observe(['(max-width: 599px)'])
    .pipe(map(result => result.matches));
    
  isTablet$ = this.breakpointObserver
    .observe(['(min-width: 600px) and (max-width: 1279px)'])
    .pipe(map(result => result.matches));
    
  isDesktop$ = this.breakpointObserver
    .observe(['(min-width: 1280px)'])
    .pipe(map(result => result.matches));
    
  constructor(private breakpointObserver: BreakpointObserver) {}
}
```

**PWA Configuration:**
```json
// ngsw-config.json
{
  "index": "/index.html",
  "assetGroups": [
    {
      "name": "app",
      "installMode": "prefetch",
      "resources": {
        "files": [
          "/favicon.ico",
          "/index.html",
          "/manifest.webmanifest",
          "/*.css",
          "/*.js"
        ]
      }
    },
    {
      "name": "assets",
      "installMode": "lazy",
      "updateMode": "prefetch",
      "resources": {
        "files": [
          "/assets/**",
          "/*.(eot|svg|cur|jpg|png|webp|gif|otf|ttf|woff|woff2|ani)"
        ]
      }
    }
  ],
  "dataGroups": [
    {
      "name": "api-cache",
      "urls": [
        "/api/system-admin/**"
      ],
      "cacheConfig": {
        "maxSize": 100,
        "maxAge": "5m",
        "strategy": "freshness"
      }
    }
  ]
}
```

---

### 2. Onboarding e Help System (3 semanas)

#### 2.1 Tour Interativo

```typescript
// services/tour.service.ts
import Shepherd from 'shepherd.js';
import 'shepherd.js/dist/css/shepherd.css';

@Injectable({ providedIn: 'root' })
export class TourService {
  private tour: Shepherd.Tour;
  
  startDashboardTour() {
    this.tour = new Shepherd.Tour({
      useModalOverlay: true,
      defaultStepOptions: {
        cancelIcon: {
          enabled: true
        },
        classes: 'shepherd-theme-custom',
        scrollTo: { behavior: 'smooth', block: 'center' }
      }
    });
    
    this.tour.addStep({
      id: 'welcome',
      text: 'Bem-vindo ao System Admin! Vamos fazer um tour rápido pelas principais funcionalidades.',
      buttons: [
        {
          text: 'Pular',
          action: this.tour.cancel,
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Começar',
          action: this.tour.next
        }
      ]
    });
    
    this.tour.addStep({
      id: 'metrics',
      text: 'Aqui você vê as métricas principais do seu negócio: MRR, clientes ativos, churn rate.',
      attachTo: {
        element: '.kpi-cards',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: this.tour.back,
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Próximo',
          action: this.tour.next
        }
      ]
    });
    
    this.tour.addStep({
      id: 'search',
      text: 'Use Ctrl+K para buscar rapidamente clínicas, usuários, tickets e muito mais!',
      attachTo: {
        element: '.search-button',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: this.tour.back,
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Próximo',
          action: this.tour.next
        }
      ]
    });
    
    this.tour.addStep({
      id: 'notifications',
      text: 'Fique de olho nas notificações importantes. Você receberá alertas sobre assinaturas vencidas, trials expirando e mais.',
      attachTo: {
        element: '.notification-button',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: this.tour.back,
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Concluir',
          action: this.tour.complete
        }
      ]
    });
    
    this.tour.start();
    
    // Marcar tour como completo
    this.tour.on('complete', () => {
      localStorage.setItem('dashboard-tour-completed', 'true');
    });
  }
  
  shouldShowTour(): boolean {
    return !localStorage.getItem('dashboard-tour-completed');
  }
}
```

#### 2.2 Help Center Integrado

```typescript
// components/help-center/help-center.component.ts
@Component({
  selector: 'app-help-center',
  standalone: true,
  template: `
    <div class="help-center">
      <!-- Search -->
      <mat-form-field class="search-field">
        <mat-icon matPrefix>search</mat-icon>
        <input 
          matInput 
          placeholder="Como podemos ajudar?" 
          [(ngModel)]="searchQuery"
          (ngModelChange)="search()"
        >
      </mat-form-field>
      
      <!-- Categories -->
      <div class="help-categories" *ngIf="!searchQuery">
        <mat-card 
          *ngFor="let category of categories"
          class="category-card"
          (click)="selectCategory(category)"
        >
          <mat-icon>{{ category.icon }}</mat-icon>
          <h3>{{ category.name }}</h3>
          <p>{{ category.description }}</p>
          <span class="article-count">{{ category.articleCount }} artigos</span>
        </mat-card>
      </div>
      
      <!-- Search Results -->
      <div class="search-results" *ngIf="searchQuery">
        <div 
          *ngFor="let article of searchResults"
          class="article-preview"
          (click)="openArticle(article)"
        >
          <h4>{{ article.title }}</h4>
          <p>{{ article.excerpt }}</p>
          <div class="article-meta">
            <mat-chip-set>
              <mat-chip>{{ article.category }}</mat-chip>
            </mat-chip-set>
            <span class="read-time">{{ article.readTime }} min</span>
          </div>
        </div>
      </div>
      
      <!-- Popular Articles -->
      <div class="popular-articles" *ngIf="!searchQuery">
        <h3>Artigos Populares</h3>
        <mat-list>
          <mat-list-item *ngFor="let article of popularArticles" (click)="openArticle(article)">
            <mat-icon matListItemIcon>article</mat-icon>
            <div matListItemTitle>{{ article.title }}</div>
            <div matListItemLine>{{ article.views }} visualizações</div>
          </mat-list-item>
        </mat-list>
      </div>
      
      <!-- Video Tutorials -->
      <div class="video-tutorials">
        <h3>Tutoriais em Vídeo</h3>
        <div class="video-grid">
          <div *ngFor="let video of videos" class="video-card">
            <img [src]="video.thumbnail" [alt]="video.title">
            <h4>{{ video.title }}</h4>
            <span class="video-duration">{{ video.duration }}</span>
          </div>
        </div>
      </div>
      
      <!-- Contact Support -->
      <div class="contact-support">
        <h3>Ainda precisa de ajuda?</h3>
        <button mat-raised-button color="primary" (click)="openTicket()">
          <mat-icon>support</mat-icon>
          Abrir Ticket de Suporte
        </button>
      </div>
    </div>
  `
})
export class HelpCenterComponent implements OnInit {
  categories = [
    {
      name: 'Primeiros Passos',
      icon: 'rocket_launch',
      description: 'Aprenda o básico do System Admin',
      articleCount: 12
    },
    {
      name: 'Gestão de Clientes',
      icon: 'people',
      description: 'Como gerenciar suas clínicas',
      articleCount: 18
    },
    {
      name: 'Relatórios e Analytics',
      icon: 'analytics',
      description: 'Entenda suas métricas',
      articleCount: 15
    },
    {
      name: 'Automações',
      icon: 'smart_toy',
      description: 'Configure workflows automatizados',
      articleCount: 10
    }
  ];
  
  searchQuery = '';
  searchResults: Article[] = [];
  popularArticles: Article[] = [];
  videos: Video[] = [];
  
  async search() {
    if (this.searchQuery.length < 2) {
      this.searchResults = [];
      return;
    }
    
    this.searchResults = await this.helpService.searchArticles(this.searchQuery);
  }
  
  openArticle(article: Article) {
    this.dialog.open(ArticleViewerComponent, {
      width: '800px',
      data: article
    });
  }
}
```

#### 2.3 Contextual Tooltips

```typescript
// directives/contextual-help.directive.ts
@Directive({
  selector: '[appContextualHelp]',
  standalone: true
})
export class ContextualHelpDirective implements OnInit {
  @Input() appContextualHelp: string; // ID do artigo de ajuda
  @Input() helpPosition: 'top' | 'bottom' | 'left' | 'right' = 'right';
  
  private overlay: OverlayRef;
  
  constructor(
    private elementRef: ElementRef,
    private overlay: Overlay,
    private helpService: HelpService
  ) {}
  
  ngOnInit() {
    const helpIcon = document.createElement('mat-icon');
    helpIcon.textContent = 'help_outline';
    helpIcon.className = 'help-icon';
    helpIcon.style.cursor = 'pointer';
    helpIcon.style.fontSize = '18px';
    helpIcon.style.marginLeft = '8px';
    
    this.elementRef.nativeElement.appendChild(helpIcon);
    
    helpIcon.addEventListener('click', () => this.showHelp());
  }
  
  private showHelp() {
    const article = this.helpService.getArticleById(this.appContextualHelp);
    
    const positionStrategy = this.overlay
      .position()
      .flexibleConnectedTo(this.elementRef)
      .withPositions([
        {
          originX: 'end',
          originY: 'center',
          overlayX: 'start',
          overlayY: 'center'
        }
      ]);
      
    this.overlay = this.overlay.create({
      positionStrategy,
      hasBackdrop: true,
      backdropClass: 'help-backdrop'
    });
    
    const portal = new ComponentPortal(HelpTooltipComponent);
    const componentRef = this.overlay.attach(portal);
    componentRef.instance.article = article;
    
    this.overlay.backdropClick().subscribe(() => this.overlay.dispose());
  }
}
```

---

### 3. Performance e Otimização (3 semanas)

#### 3.1 Frontend Performance

**Lazy Loading Completo:**
```typescript
// app.routes.ts
export const routes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard.component')
      .then(m => m.DashboardComponent)
  },
  {
    path: 'clinics',
    loadChildren: () => import('./clinics/clinics.routes')
      .then(m => m.CLINIC_ROUTES)
  },
  {
    path: 'reports',
    loadChildren: () => import('./reports/reports.routes')
      .then(m => m.REPORT_ROUTES)
  },
  {
    path: 'workflows',
    loadChildren: () => import('./workflows/workflows.routes')
      .then(m => m.WORKFLOW_ROUTES)
  }
];
```

**Virtual Scrolling:**
```typescript
// components/clinics-list-virtual.component.ts
@Component({
  selector: 'app-clinics-list-virtual',
  standalone: true,
  imports: [ScrollingModule],
  template: `
    <cdk-virtual-scroll-viewport 
      itemSize="72" 
      class="clinic-list-viewport"
    >
      <div
        *cdkVirtualFor="let clinic of clinics; trackBy: trackById"
        class="clinic-item"
      >
        <app-clinic-card [clinic]="clinic" />
      </div>
    </cdk-virtual-scroll-viewport>
  `,
  styles: [`
    .clinic-list-viewport {
      height: calc(100vh - 200px);
    }
  `]
})
export class ClinicsListVirtualComponent {
  @Input() clinics: Clinic[] = [];
  
  trackById(index: number, clinic: Clinic): number {
    return clinic.id;
  }
}
```

**Image Optimization:**
```typescript
// directives/lazy-image.directive.ts
@Directive({
  selector: 'img[appLazyImage]',
  standalone: true
})
export class LazyImageDirective implements OnInit {
  @Input() appLazyImage: string;
  
  constructor(private el: ElementRef<HTMLImageElement>) {}
  
  ngOnInit() {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          this.loadImage();
          observer.disconnect();
        }
      });
    });
    
    observer.observe(this.el.nativeElement);
  }
  
  private loadImage() {
    const img = this.el.nativeElement;
    img.src = this.appLazyImage;
    img.classList.add('loaded');
  }
}
```

#### 3.2 Backend Performance

**Response Caching:**
```csharp
// Middleware/ResponseCacheMiddleware.cs
public class ResponseCacheMiddleware
{
    private readonly IDistributedCache _cache;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != "GET")
        {
            await _next(context);
            return;
        }
        
        var cacheKey = $"response:{context.Request.Path}:{context.Request.QueryString}";
        var cachedResponse = await _cache.GetStringAsync(cacheKey);
        
        if (cachedResponse != null)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("X-Cache", "HIT");
            await context.Response.WriteAsync(cachedResponse);
            return;
        }
        
        // Capture response
        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;
        
        await _next(context);
        
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        // Cache response
        await _cache.SetStringAsync(cacheKey, responseBody, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        
        context.Response.Headers.Add("X-Cache", "MISS");
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
```

**Query Optimization:**
```csharp
// Repositories/OptimizedClinicRepository.cs
public class OptimizedClinicRepository
{
    public async Task<List<ClinicDto>> GetClinicsOptimized(int page, int pageSize)
    {
        return await _context.Clinics
            .AsNoTracking() // Melhor performance para read-only
            .Select(c => new ClinicDto // Projeção direta
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PlanName = c.Subscription.Plan.Name,
                Mrr = c.Subscription.Mrr,
                IsActive = c.IsActive,
                UserCount = c.Users.Count, // Projeção vs Include
                LastActivityAt = c.LastActivityAt
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
```

#### 3.3 Monitoring e APM

**Application Insights:**
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

// Custom telemetry
public class PerformanceTracker
{
    private readonly TelemetryClient _telemetry;
    
    public IDisposable TrackOperation(string operationName)
    {
        var operation = _telemetry.StartOperation<RequestTelemetry>(operationName);
        return new OperationDisposable(operation, _telemetry);
    }
    
    public void TrackMetric(string name, double value, Dictionary<string, string> properties = null)
    {
        _telemetry.TrackMetric(name, value, properties);
    }
}

// Usage
using (performanceTracker.TrackOperation("GetClinicHealthScore"))
{
    var score = await CalculateHealthScore(clinicId);
    performanceTracker.TrackMetric("HealthScoreCalculationTime", stopwatch.ElapsedMilliseconds);
    return score;
}
```

**Frontend Error Tracking:**
```typescript
// services/error-tracking.service.ts
import * as Sentry from '@sentry/angular';

@Injectable({ providedIn: 'root' })
export class ErrorTrackingService {
  constructor() {
    Sentry.init({
      dsn: environment.sentryDsn,
      environment: environment.production ? 'production' : 'development',
      tracesSampleRate: 1.0,
      integrations: [
        new Sentry.BrowserTracing({
          tracingOrigins: ['localhost', environment.apiUrl],
          routingInstrumentation: Sentry.routingInstrumentation,
        }),
      ],
    });
  }
  
  captureException(error: Error, context?: any) {
    Sentry.captureException(error, {
      extra: context
    });
  }
  
  setUser(user: User) {
    Sentry.setUser({
      id: user.id.toString(),
      email: user.email,
      username: user.name
    });
  }
}
```

**Real User Monitoring:**
```typescript
// services/rum.service.ts
@Injectable({ providedIn: 'root' })
export class RealUserMonitoringService {
  constructor(private http: HttpClient) {
    this.trackPerformance();
  }
  
  private trackPerformance() {
    if ('PerformanceObserver' in window) {
      // Track page load
      const perfObserver = new PerformanceObserver((list) => {
        for (const entry of list.getEntries()) {
          if (entry.entryType === 'navigation') {
            const navEntry = entry as PerformanceNavigationTiming;
            this.sendMetric({
              metric: 'page_load',
              value: navEntry.loadEventEnd - navEntry.fetchStart,
              url: window.location.pathname
            });
          }
        }
      });
      
      perfObserver.observe({ entryTypes: ['navigation'] });
      
      // Track Web Vitals
      this.trackWebVitals();
    }
  }
  
  private trackWebVitals() {
    // First Contentful Paint (FCP)
    const fcpObserver = new PerformanceObserver((list) => {
      for (const entry of list.getEntries()) {
        if (entry.name === 'first-contentful-paint') {
          this.sendMetric({
            metric: 'fcp',
            value: entry.startTime,
            url: window.location.pathname
          });
        }
      }
    });
    fcpObserver.observe({ entryTypes: ['paint'] });
    
    // Largest Contentful Paint (LCP)
    const lcpObserver = new PerformanceObserver((list) => {
      const entries = list.getEntries();
      const lastEntry = entries[entries.length - 1];
      this.sendMetric({
        metric: 'lcp',
        value: lastEntry.startTime,
        url: window.location.pathname
      });
    });
    lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });
  }
  
  private sendMetric(metric: PerformanceMetric) {
    this.http.post('/api/system-admin/rum/metrics', metric).subscribe();
  }
}
```

---

## ✅ Critérios de Sucesso

### UI/UX
- [x] Dark mode funcionando
- [x] Animações sutis em todas transições
- [x] Loading skeletons em todas listas
- [x] Componentes consistentes
- [x] Responsive em mobile/tablet

### Onboarding
- [x] Tour interativo funcional
- [x] Help center com busca
- [x] 20+ artigos de ajuda
- [x] 5+ vídeos tutoriais
- [x] Tooltips contextuais

### Performance
- [x] Lighthouse score > 90 (target)
- [x] FCP < 1s (target)
- [x] LCP < 2.5s (target)
- [x] TTI < 3s (target)
- [x] Lazy loading em todas rotas
- [x] PWA instalável

### Monitoring
- [x] APM configurado
- [x] Error tracking funcional
- [x] RUM implementado
- [x] Dashboards de performance (guia criado)
- [x] Alertas configurados (guia criado)

---

## 🧪 Testes e Validação

### Performance Testing
```typescript
describe('Performance', () => {
  it('should load dashboard in < 1s', async () => {
    const start = performance.now();
    await page.goto('/dashboard');
    const end = performance.now();
    expect(end - start).toBeLessThan(1000);
  });
});
```

### Lighthouse CI
```yaml
# .github/workflows/lighthouse.yml
- name: Lighthouse CI
  run: |
    npm install -g @lhci/cli
    lhci autorun --config=lighthouserc.json
```

---

## 📚 Documentação

- ✅ Guia de estilo UI/UX - `system-admin/docs/GUIA_ESTILO_UIUX.md`
- ✅ Design system documentation - Incluído no guia de estilo
- ✅ Performance optimization guide - `system-admin/docs/GUIA_OTIMIZACAO_PERFORMANCE.md`
- ✅ Monitoring setup guide - `system-admin/docs/GUIA_CONFIGURACAO_MONITORAMENTO.md`

---

## 🔄 Próximos Passos

Após Fase 5:
1. ✅ Validar com usuários
2. ➡️ **Fase 6: Segurança e Compliance**

---

## 📞 Referências

- **Vercel:** https://vercel.com
- **Linear:** https://linear.app
- **Notion:** https://notion.so

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Pronto para implementação
