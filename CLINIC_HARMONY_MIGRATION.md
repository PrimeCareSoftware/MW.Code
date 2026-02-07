# Migração de Estilos - Clinic Harmony UI para Aplicações Angular

## Visão Geral

Este documento descreve a migração do sistema de design do projeto **clinic-harmony-ui-main** (React/Tailwind) para as aplicações Angular do MedicWarehouse, mantendo a mesma aparência visual e layout.

## Objetivos

✅ Migrar todos os componentes de estilo do Clinic Harmony UI  
✅ Manter o layout e aparência visual idênticos  
✅ Preservar Angular (sem migração para React)  
✅ Aplicar aos projetos: medicwarehouse-app, patient-portal, e system-admin  

## Alterações Realizadas

### 1. Sistema de Design Compartilhado (`/frontend/shared-styles`)

#### Design Tokens (`_design-tokens.scss`)

**Cores Primárias - Soft Medical Blue (Clinic Harmony)**
- Atualizado `--primary-500` para `#3D9DED` (HSL: 211, 84%, 55%)
- Cores primárias agora correspondem exatamente ao Clinic Harmony
- Adicionado suporte HSL com variáveis `--primary-hsl`, `--accent-hsl`, etc.

**Novas Variáveis CSS do Clinic Harmony:**
```scss
--background: hsl(0, 0%, 98%);
--foreground: hsl(220, 9%, 12%);
--card: hsl(0, 0%, 100%);
--card-foreground: hsl(220, 9%, 12%);
--popover: hsl(0, 0%, 100%);
--muted: hsl(220, 14%, 96%);
--accent-hsl: 174 62% 47%;  // Subtle teal
--success-hsl: 142 71% 45%;  // Soft green
--warning-hsl: 38 92% 50%;   // Warm amber
--border: hsl(220, 13%, 91%);
--ring: hsl(211, 84%, 55%);
```

**Sidebar Colors (Clinic Harmony):**
```scss
--sidebar-background: hsl(0, 0%, 100%);
--sidebar-foreground: hsl(220, 9%, 30%);
--sidebar-primary: hsl(211, 84%, 55%);
--sidebar-accent: hsl(220, 14%, 96%);
--sidebar-border: hsl(220, 13%, 93%);
```

**Gradientes:**
```scss
--gradient-primary: linear-gradient(135deg, hsl(211, 84%, 55%) 0%, hsl(211, 84%, 45%) 100%);
--gradient-accent: linear-gradient(135deg, hsl(174, 62%, 47%) 0%, hsl(174, 62%, 37%) 100%);
--gradient-surface: linear-gradient(180deg, hsl(0, 0%, 100%) 0%, hsl(220, 14%, 98%) 100%);
```

**Tema Escuro Atualizado:**
- Cores de fundo escuras suaves (HSL: 220, 13%, 8-15%)
- Cores primárias ajustadas para dark mode
- Manutenção da hierarquia visual

#### Componentes Base (`_components.scss`)

**Tipografia Apple-Inspired:**
- Font: Inter (importada do Google Fonts)
- `-webkit-font-smoothing: antialiased` para renderização suave
- Letter-spacing: -0.02em (tracking apertado)
- Line-heights otimizados para legibilidade

**Estilos Globais:**
```scss
html {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

body {
  background: var(--background);
  color: var(--foreground);
}

h1, h2, h3, h4, h5, h6 {
  font-weight: var(--font-semibold);
  letter-spacing: -0.02em;
  line-height: var(--line-height-tight);
}
```

#### Utilitários (`_utilities.scss`)

**Efeitos Clinic Harmony já existentes:**
- `.glass` - Glassmorphism com backdrop blur
- `.card-hover` - Elevação suave no hover
- `.focus-ring` - Anel de foco Apple-style
- `.transition-apple` - Transições suaves (200ms)

**Animações:**
- `fade-in` - Fade com translateY
- `scale-in` - Scale de 0.95 a 1
- `slide-in-right` - Slide da direita
- `pulse-soft` - Pulse suave infinito

**Gradientes para Cards de Métricas:**
- `.metric-primary` - Gradiente azul suave
- `.metric-accent` - Gradiente teal
- `.metric-success` - Gradiente verde
- `.metric-warning` - Gradiente âmbar

**Badges de Status:**
- `.badge-success` - Fundo verde suave
- `.badge-warning` - Fundo âmbar suave
- `.badge-error` - Fundo vermelho suave
- `.badge-info` - Fundo azul suave

## Como Usar

### Para Desenvolvedores

1. **Importar Sistema de Design Compartilhado**
   ```scss
   // No seu styles.scss
   @use '../../shared-styles' as *;
   ```

2. **Usar Variáveis CSS**
   ```scss
   .my-component {
     background: var(--background);
     color: var(--foreground);
     border: 1px solid var(--border);
     border-radius: var(--radius);
     box-shadow: var(--shadow-lg);
   }
   ```

3. **Aplicar Classes Utilitárias**
   ```html
   <div class="card glass card-hover">
     <h3 class="text-primary font-weight-semibold">Título</h3>
     <p class="text-secondary">Conteúdo</p>
   </div>
   ```

4. **Usar Componentes Pré-estilizados**
   ```html
   <button class="btn btn-primary">
     Salvar
   </button>
   
   <span class="badge badge-success">Ativo</span>
   
   <div class="alert alert-info">
     Informação importante
   </div>
   ```

### Cores Primárias

```css
Primary Blue:   #3D9DED (HSL: 211, 84%, 55%)
Accent Teal:    #14b8a6 (HSL: 174, 62%, 47%)
Success Green:  #22c55e (HSL: 142, 71%, 45%)
Warning Amber:  #f59e0b (HSL: 38, 92%, 50%)
Error Red:      #ef4444 (HSL: 0, 72%, 51%)
```

### Espaçamento

Base: 4px (0.25rem)
- `--spacing-1`: 4px
- `--spacing-2`: 8px
- `--spacing-3`: 12px
- `--spacing-4`: 16px
- `--spacing-6`: 24px
- `--spacing-8`: 32px

### Border Radius

- `--radius-sm`: 8px
- `--radius`: 12px (padrão Clinic Harmony)
- `--radius-md`: 16px
- `--radius-lg`: 20px
- `--radius-xl`: 24px

### Sombras (Ultra Sutis - Apple-style)

- `--shadow-sm`: Sombra extra suave (opacity: 0.03-0.05)
- `--shadow-md`: Sombra média
- `--shadow-lg`: Sombra grande (elevação suave)
- `--shadow-xl`: Sombra extra grande

## Próximos Passos

### medicwarehouse-app
- [ ] Revisar componentes existentes
- [ ] Aplicar novas classes utilitárias
- [ ] Testar responsividade
- [ ] Validar modo escuro

### patient-portal
- [ ] Revisar componentes existentes
- [ ] Aplicar novas classes utilitárias
- [ ] Testar responsividade
- [ ] Validar modo escuro

### system-admin
- [ ] Verificar estrutura do frontend
- [ ] Documentar padrões de estilo
- [ ] Aplicar design system quando houver código

## Benefícios

✅ **Consistência Visual** - Todas as aplicações com aparência unificada  
✅ **Manutenção Simplificada** - Alterações centralizadas em shared-styles  
✅ **Performance** - CSS otimizado e minificado  
✅ **Acessibilidade** - Foco em contraste e legibilidade  
✅ **Dark Mode** - Suporte completo a tema escuro  
✅ **Apple-inspired** - Design minimalista e elegante  

## Referências

- **Clinic Harmony UI**: `/clinic-harmony-ui-main`
- **Shared Styles**: `/frontend/shared-styles`
- **Design Tokens**: `/frontend/shared-styles/_design-tokens.scss`
- **Componentes**: `/frontend/shared-styles/_components.scss`
- **Utilitários**: `/frontend/shared-styles/_utilities.scss`

## Suporte

Para dúvidas ou problemas, consulte:
- Documentação do projeto Clinic Harmony UI
- README.md do shared-styles
- Equipe de desenvolvimento Frontend
