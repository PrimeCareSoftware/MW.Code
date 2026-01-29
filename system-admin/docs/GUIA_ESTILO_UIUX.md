# Guia de Estilo UI/UX - System Admin

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Status:** Ativo

---

## 📋 Visão Geral

Este guia define os padrões de design e experiência do usuário para o System Admin do MedicWarehouse. Seguimos princípios modernos de UI/UX inspirados em Vercel, Linear e Notion.

---

## 🎨 Princípios de Design

### 1. Clareza
- Interfaces limpas e focadas
- Hierarquia visual clara
- Informações organizadas logicamente

### 2. Consistência
- Componentes reutilizáveis
- Padrões uniformes em toda aplicação
- Comportamentos previsíveis

### 3. Feedback Visual
- Animações sutis e significativas
- Loading states apropriados
- Confirmações visuais de ações

### 4. Acessibilidade
- Suporte a dark mode
- Alto contraste opcional
- Navegação por teclado
- Leitores de tela compatíveis

---

## 🎨 Paleta de Cores

### Light Mode
```scss
// Primary - Verde
$primary-50: #e8f5e9;
$primary-500: #4caf50;
$primary-700: #388e3c;
$primary-900: #1b5e20;

// Accent - Azul
$accent-500: #2196f3;
$accent-700: #1976d2;

// Texto
$text-primary: #212121;
$text-secondary: #757575;
$text-disabled: #bdbdbd;

// Background
$surface: #ffffff;
$background: #fafafa;
$divider: #e0e0e0;
```

### Dark Mode
```scss
// Background
$surface-dark: #2d2d2d;
$background-dark: #1a1a1a;
$divider-dark: #424242;

// Texto
$text-primary-dark: #e0e0e0;
$text-secondary-dark: #b0b0b0;
$text-disabled-dark: #757575;
```

---

## 📐 Espaçamento

### Grid System
- Base: 8px
- Múltiplos: 4px, 8px, 16px, 24px, 32px, 48px, 64px

### Padding Padrão
```scss
$padding-xs: 4px;
$padding-sm: 8px;
$padding-md: 16px;
$padding-lg: 24px;
$padding-xl: 32px;
```

### Margins
```scss
$margin-xs: 4px;
$margin-sm: 8px;
$margin-md: 16px;
$margin-lg: 24px;
$margin-xl: 32px;
```

---

## 🔤 Tipografia

### Font Family
```scss
$font-family: 'Inter', system-ui, -apple-system, sans-serif;
$font-family-mono: 'JetBrains Mono', 'Fira Code', monospace;
```

### Font Sizes
```scss
$font-xs: 12px;   // Small text, labels
$font-sm: 14px;   // Body text
$font-md: 16px;   // Default
$font-lg: 18px;   // Subtitles
$font-xl: 24px;   // Titles
$font-2xl: 32px;  // Main headings
$font-3xl: 48px;  // Hero text
```

### Font Weights
```scss
$font-light: 300;
$font-normal: 400;
$font-medium: 500;
$font-semibold: 600;
$font-bold: 700;
```

### Line Heights
```scss
$line-height-tight: 1.25;
$line-height-normal: 1.5;
$line-height-relaxed: 1.75;
```

---

## 🎬 Animações

### Duração
```scss
$duration-fast: 150ms;
$duration-normal: 300ms;
$duration-slow: 500ms;
```

### Easing
```scss
$ease-in: cubic-bezier(0.4, 0, 1, 1);
$ease-out: cubic-bezier(0, 0, 0.2, 1);
$ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
```

### Uso
- **Hover**: 150ms ease-out
- **Transições**: 300ms ease-in-out
- **Modais**: 300ms ease-out
- **Loading**: 500ms ease-in-out

---

## 📦 Componentes

### Cards
```html
<app-modern-card title="Título" [hoverable]="true">
  Conteúdo do card
</app-modern-card>
```

**Propriedades:**
- `title`: Título opcional
- `hoverable`: Efeito hover (padrão: false)
- `elevated`: Elevação maior (padrão: false)

**Estilo:**
- Border radius: 12px
- Shadow: 0 1px 3px rgba(0,0,0,0.12)
- Hover shadow: 0 8px 16px rgba(0,0,0,0.15)

### Botões

#### Primary
```html
<button mat-raised-button color="primary">
  Ação Principal
</button>
```

#### Secondary
```html
<button mat-button>
  Ação Secundária
</button>
```

#### Icon
```html
<button mat-icon-button>
  <mat-icon>more_vert</mat-icon>
</button>
```

### Inputs

```html
<mat-form-field appearance="outline">
  <mat-label>Label</mat-label>
  <input matInput type="text">
  <mat-hint>Texto de ajuda</mat-hint>
</mat-form-field>
```

### Skeleton Loaders

```html
<app-skeleton-loader type="card" [lines]="5"></app-skeleton-loader>
```

**Tipos:**
- `text`: Linhas de texto
- `card`: Card completo
- `table`: Tabela
- `circle`: Avatar circular
- `avatar`: Avatar com texto

---

## 🎯 Padrões de Interação

### Loading States

**Skeleton Loaders:**
- Use para listas e cards
- Melhora percepção de performance
- Mantém layout estável

**Spinners:**
- Use para ações rápidas (< 2s)
- Centralize em overlays
- Mostre progresso quando possível

### Empty States

```html
<div class="empty-state">
  <mat-icon>inbox</mat-icon>
  <h3>Nenhum item encontrado</h3>
  <p>Comece criando seu primeiro item</p>
  <button mat-raised-button color="primary">
    Criar Item
  </button>
</div>
```

### Error States

```html
<div class="error-state">
  <mat-icon>error_outline</mat-icon>
  <h3>Ops! Algo deu errado</h3>
  <p>{{ errorMessage }}</p>
  <button mat-raised-button (click)="retry()">
    Tentar Novamente
  </button>
</div>
```

---

## 📱 Responsividade

### Breakpoints

```scss
$breakpoint-mobile: 599px;    // 0-599px
$breakpoint-tablet: 1279px;   // 600-1279px
$breakpoint-desktop: 1280px;  // 1280px+
```

### Mobile First

```scss
// Mobile (padrão)
.component {
  padding: 16px;
}

// Tablet
@media (min-width: 600px) {
  .component {
    padding: 24px;
  }
}

// Desktop
@media (min-width: 1280px) {
  .component {
    padding: 32px;
  }
}
```

### Uso do BreakpointService

```typescript
import { BreakpointService } from '@app/services/breakpoint.service';

export class MyComponent {
  isMobile$ = this.breakpointService.isMobile$;
  
  constructor(private breakpointService: BreakpointService) {}
}
```

---

## ♿ Acessibilidade

### Contraste

- **Normal text**: Mínimo 4.5:1
- **Large text**: Mínimo 3:1
- **UI components**: Mínimo 3:1

### Keyboard Navigation

```html
<!-- Tab order -->
<button tabindex="0">Primeiro</button>
<button tabindex="0">Segundo</button>

<!-- Skip links -->
<a href="#main-content" class="skip-link">
  Pular para conteúdo
</a>
```

### ARIA Labels

```html
<button mat-icon-button aria-label="Excluir item">
  <mat-icon>delete</mat-icon>
</button>

<div role="alert" aria-live="polite">
  Item excluído com sucesso
</div>
```

### Focus Visible

```scss
button:focus-visible {
  outline: 2px solid $primary-500;
  outline-offset: 2px;
}
```

---

## 🎨 Theme Switcher

### Uso

```typescript
import { ThemeService } from '@app/services/theme.service';

export class SettingsComponent {
  constructor(private themeService: ThemeService) {}
  
  toggleTheme() {
    this.themeService.toggleTheme();
  }
  
  setTheme(theme: 'light' | 'dark' | 'high-contrast') {
    this.themeService.setTheme(theme);
  }
}
```

### CSS Custom Properties

```scss
:root {
  --primary: #4caf50;
  --surface: #ffffff;
  --text-primary: #212121;
}

.theme-dark {
  --primary: #66bb6a;
  --surface: #2d2d2d;
  --text-primary: #e0e0e0;
}
```

---

## 📐 Layout Patterns

### Dashboard Grid

```html
<div class="dashboard-grid">
  <app-modern-card class="grid-item">
    <!-- KPI Card -->
  </app-modern-card>
  <app-modern-card class="grid-item">
    <!-- Chart -->
  </app-modern-card>
  <app-modern-card class="grid-item grid-item-wide">
    <!-- Wide table -->
  </app-modern-card>
</div>
```

```scss
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 24px;
  
  .grid-item-wide {
    grid-column: 1 / -1;
  }
}
```

### Side Navigation

```html
<mat-sidenav-container>
  <mat-sidenav mode="side" opened>
    <!-- Navigation -->
  </mat-sidenav>
  <mat-sidenav-content>
    <!-- Main content -->
  </mat-sidenav-content>
</mat-sidenav-container>
```

---

## ✅ Checklist de Qualidade

### Antes de Publicar

- [ ] Testado em mobile, tablet e desktop
- [ ] Dark mode funcionando
- [ ] Animações suaves
- [ ] Loading states implementados
- [ ] Empty states implementados
- [ ] Error handling apropriado
- [ ] Acessibilidade validada
- [ ] Performance otimizada

### Code Review

- [ ] Componentes reutilizáveis usados
- [ ] Estilos consistentes com guia
- [ ] Código TypeScript type-safe
- [ ] Sem console.log
- [ ] Sem código comentado
- [ ] Documentação atualizada

---

## 📚 Referências

- **Material Design**: https://material.angular.io
- **Vercel Design**: https://vercel.com/design
- **Linear Design**: https://linear.app/method
- **Inter Font**: https://rsms.me/inter/
- **WCAG Guidelines**: https://www.w3.org/WAI/WCAG21/quickref/

---

**Última Atualização:** Janeiro 2026  
**Próxima Revisão:** Março 2026  
**Responsável:** Equipe de Desenvolvimento
