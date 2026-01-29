# Guia de Otimização de Performance - System Admin

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Status:** Ativo

---

## 📋 Visão Geral

Este guia apresenta estratégias e técnicas de otimização de performance implementadas no System Admin, com foco em Web Vitals, lazy loading, caching e monitoramento.

---

## 🎯 Objetivos de Performance

### Core Web Vitals
- **FCP (First Contentful Paint):** < 1s
- **LCP (Largest Contentful Paint):** < 2.5s
- **FID (First Input Delay):** < 100ms
- **CLS (Cumulative Layout Shift):** < 0.1
- **TTFB (Time to First Byte):** < 600ms

### Lighthouse Scores
- **Performance:** > 90
- **Accessibility:** > 95
- **Best Practices:** > 95
- **SEO:** > 90

---

## 🚀 Frontend Optimization

### 1. Lazy Loading

#### Rotas
Todas as rotas usam lazy loading com `loadComponent`:

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
  }
];
```

**Benefícios:**
- Reduz bundle inicial
- Carrega código sob demanda
- Melhora tempo de carregamento

#### Imagens

Use o directive `appLazyImage` para lazy loading de imagens:

```html
<img 
  [appLazyImage]="imageUrl" 
  placeholder="/assets/placeholder.png"
  alt="Descrição da imagem"
>
```

**Implementação:**
```typescript
@Directive({
  selector: 'img[appLazyImage]',
  standalone: true
})
export class LazyImageDirective implements OnInit {
  @Input() appLazyImage: string = '';
  
  ngOnInit() {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          this.loadImage();
          observer.disconnect();
        }
      });
    }, { rootMargin: '50px' });
    
    observer.observe(this.el.nativeElement);
  }
}
```

### 2. Virtual Scrolling

Para listas grandes (> 100 itens), use CDK Virtual Scrolling:

```html
<cdk-virtual-scroll-viewport itemSize="72" class="list-viewport">
  <div
    *cdkVirtualFor="let item of items; trackBy: trackById"
    class="list-item"
  >
    {{ item.name }}
  </div>
</cdk-virtual-scroll-viewport>
```

```scss
.list-viewport {
  height: 600px; // Altura fixa necessária
}
```

**Benefícios:**
- Renderiza apenas itens visíveis
- Suporta milhares de itens
- Scroll suave e performático

### 3. TrackBy Functions

Sempre use trackBy em *ngFor:

```typescript
export class ListComponent {
  items: Item[] = [];
  
  trackById(index: number, item: Item): number {
    return item.id;
  }
}
```

```html
<div *ngFor="let item of items; trackBy: trackById">
  {{ item.name }}
</div>
```

**Benefícios:**
- Evita re-rendering desnecessário
- Mantém estado de componentes filhos
- Melhora performance de listas dinâmicas

### 4. Change Detection Strategy

Use OnPush quando possível:

```typescript
@Component({
  selector: 'app-my-component',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `...`
})
export class MyComponent {
  // Use signals ou observables para dados reativos
  data = signal<Data>({});
}
```

**Quando usar:**
- Componentes que dependem de inputs
- Componentes com dados imutáveis
- Listas de itens

### 5. Skeleton Loaders

Melhore a percepção de performance com skeleton loaders:

```html
<app-skeleton-loader 
  *ngIf="loading" 
  type="card" 
  [lines]="5"
></app-skeleton-loader>

<div *ngIf="!loading">
  <!-- Conteúdo real -->
</div>
```

**Tipos disponíveis:**
- `text`: Linhas de texto
- `card`: Card completo
- `table`: Tabela
- `circle`: Avatar circular
- `avatar`: Avatar com texto

### 6. Debounce e Throttle

Use para inputs de busca e eventos frequentes:

```typescript
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

searchControl = new FormControl('');

ngOnInit() {
  this.searchControl.valueChanges
    .pipe(
      debounceTime(300),
      distinctUntilChanged()
    )
    .subscribe(value => {
      this.search(value);
    });
}
```

**Uso:**
- **Debounce (300ms)**: Busca, auto-save
- **Throttle**: Scroll events, resize

---

## 🔧 Backend Optimization

### 1. Response Caching

Cache responses GET para reduzir carga:

```csharp
[HttpGet]
[ResponseCache(Duration = 300)] // 5 minutos
public async Task<ActionResult<List<Clinic>>> GetClinics()
{
    var clinics = await _service.GetClinicsAsync();
    return Ok(clinics);
}
```

### 2. Query Optimization

**AsNoTracking** para queries read-only:

```csharp
public async Task<List<ClinicDto>> GetClinicsOptimized()
{
    return await _context.Clinics
        .AsNoTracking() // Não rastreia mudanças
        .Select(c => new ClinicDto
        {
            Id = c.Id,
            Name = c.Name,
            // Projeção direta
        })
        .ToListAsync();
}
```

**Projeções** ao invés de Include:

```csharp
// ❌ Ruim - Carrega tudo
var clinics = await _context.Clinics
    .Include(c => c.Users)
    .Include(c => c.Subscription)
    .ToListAsync();

// ✅ Bom - Projeta apenas o necessário
var clinics = await _context.Clinics
    .Select(c => new ClinicDto
    {
        Id = c.Id,
        Name = c.Name,
        UserCount = c.Users.Count
    })
    .ToListAsync();
```

### 3. Paginação

Sempre pagine resultados grandes:

```csharp
public async Task<PagedResult<Clinic>> GetClinics(int page, int pageSize)
{
    var query = _context.Clinics.AsQueryable();
    
    var total = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Clinic>
    {
        Items = items,
        Total = total,
        Page = page,
        PageSize = pageSize
    };
}
```

### 4. Índices de Banco de Dados

Adicione índices para queries frequentes:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Clinic>()
        .HasIndex(c => c.Subdomain);
    
    modelBuilder.Entity<Clinic>()
        .HasIndex(c => c.IsActive);
    
    modelBuilder.Entity<Subscription>()
        .HasIndex(s => new { s.ClinicId, s.Status });
}
```

---

## 📊 Real User Monitoring (RUM)

### Frontend

O RUM Service coleta automaticamente Web Vitals:

```typescript
import { RealUserMonitoringService } from '@app/services/rum.service';

@Injectable({ providedIn: 'root' })
export class RealUserMonitoringService {
  constructor(private http: HttpClient) {
    this.trackPerformance();
  }
  
  private trackPerformance() {
    // Tracked automatically:
    // - FCP, LCP, FID, CLS, TTFB
    // - Page load time
    // - Navigation timing
  }
  
  // Manual tracking
  trackApiCall(url: string, duration: number, statusCode: number) {
    this.sendMetric({
      metric: 'api_call',
      value: duration,
      url,
      additionalData: { statusCode }
    });
  }
}
```

### Backend Endpoints

```
POST /api/system-admin/monitoring/rum/metrics
POST /api/system-admin/monitoring/errors
GET  /api/system-admin/monitoring/web-vitals?days=7
GET  /api/system-admin/monitoring/slow-pages?limit=10
```

### Visualização

Acesse Web Vitals summary:

```typescript
this.http.get<WebVitalsSummaryDto>(
  '/api/system-admin/monitoring/web-vitals?days=7'
).subscribe(summary => {
  console.log('Avg FCP:', summary.avgFcp, 'ms');
  console.log('Avg LCP:', summary.avgLcp, 'ms');
  console.log('Avg FID:', summary.avgFid, 'ms');
  console.log('Avg CLS:', summary.avgCls);
});
```

---

## 🎨 Bundle Optimization

### 1. Tree Shaking

Use imports específicos:

```typescript
// ❌ Ruim
import * as _ from 'lodash';

// ✅ Bom
import { debounce } from 'lodash-es';
```

### 2. Standalone Components

Sempre use standalone components no Angular 20:

```typescript
@Component({
  selector: 'app-my-component',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  template: `...`
})
export class MyComponent {}
```

**Benefícios:**
- Tree shaking automático
- Bundles menores
- Imports explícitos

### 3. Lazy Load Third-Party

Carregue bibliotecas pesadas sob demanda:

```typescript
async loadChart() {
  const ApexCharts = await import('apexcharts');
  const chart = new ApexCharts.default(/* config */);
  chart.render();
}
```

---

## 🔍 Análise e Monitoramento

### Lighthouse CI

Configure CI para monitorar performance:

```yaml
# .github/workflows/lighthouse.yml
name: Lighthouse CI
on: [pull_request]

jobs:
  lighthouse:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
      - run: npm ci
      - run: npm run build
      - run: npm install -g @lhci/cli
      - run: lhci autorun
```

```json
// lighthouserc.json
{
  "ci": {
    "collect": {
      "startServerCommand": "npm run start",
      "url": ["http://localhost:4200"]
    },
    "assert": {
      "assertions": {
        "categories:performance": ["error", {"minScore": 0.9}],
        "categories:accessibility": ["error", {"minScore": 0.95}]
      }
    }
  }
}
```

### Bundle Analyzer

Analise tamanho dos bundles:

```bash
npm install -g webpack-bundle-analyzer
ng build --stats-json
webpack-bundle-analyzer dist/stats.json
```

### Chrome DevTools

Use Performance tab:
1. Abra DevTools (F12)
2. Performance tab
3. Record → Execute ações → Stop
4. Analise flamegraph

**Métricas importantes:**
- Scripting time
- Rendering time
- Painting time
- Idle time

---

## ✅ Checklist de Performance

### Antes de Deploy

- [ ] Lighthouse score > 90
- [ ] Todos os assets comprimidos (gzip/brotli)
- [ ] Imagens otimizadas (WebP quando possível)
- [ ] Lazy loading implementado
- [ ] Virtual scrolling em listas grandes
- [ ] Bundle size < 500KB (initial)
- [ ] Response time < 200ms (p95)
- [ ] Queries otimizadas com índices
- [ ] Cache configurado

### Monitoramento Contínuo

- [ ] RUM coletando dados
- [ ] Alertas configurados (LCP > 3s)
- [ ] Dashboard de Web Vitals
- [ ] Slow queries identificadas
- [ ] Error tracking ativo

---

## 📈 Metas de Performance

### Atual (Baseline)
- FCP: 1.2s
- LCP: 2.8s
- TTI: 3.5s
- Bundle: 450KB

### Meta Q2 2026
- FCP: < 0.8s
- LCP: < 2.0s
- TTI: < 2.5s
- Bundle: < 350KB

---

## 🛠️ Ferramentas

### Análise
- **Chrome DevTools**: Performance profiling
- **Lighthouse**: Audits automatizados
- **WebPageTest**: Performance detalhado
- **Bundle Analyzer**: Análise de bundles

### Monitoramento
- **RUM Service**: Métricas reais de usuários
- **Error Tracking**: Rastreamento de erros
- **Application Insights**: APM (se disponível)

---

## 📚 Referências

- **Web Vitals**: https://web.dev/vitals/
- **Angular Performance**: https://angular.dev/best-practices/runtime-performance
- **Lighthouse**: https://developers.google.com/web/tools/lighthouse
- **CDK Virtual Scrolling**: https://material.angular.io/cdk/scrolling
- **EF Core Performance**: https://learn.microsoft.com/ef/core/performance/

---

**Última Atualização:** Janeiro 2026  
**Próxima Revisão:** Março 2026  
**Responsável:** Equipe de Performance
