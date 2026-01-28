# Prompts Detalhados para Implementação - MedicWarehouse Website

> **Data:** 28 de Janeiro de 2026  
> **Versão:** 1.0  
> **Uso:** Copiar e colar cada prompt no GitHub Copilot ou agente de IA

## 📋 Índice de Prompts

1. [PROMPT 1: Redesign da Homepage](#prompt-1)
2. [PROMPT 2: Vídeo Demonstrativo](#prompt-2)
3. [PROMPT 3: Design System Atualização](#prompt-3)
4. [PROMPT 4: Tour Guiado/Onboarding](#prompt-4)
5. [PROMPT 5: Blog Técnico e SEO](#prompt-5)
6. [PROMPT 6: Empty States](#prompt-6)
7. [PROMPT 7: Micro-interações](#prompt-7)
8. [PROMPT 8: Cases de Sucesso](#prompt-8)
9. [PROMPT 9: Programa de Indicação](#prompt-9)
10. [PROMPT 10: Analytics e Tracking](#prompt-10)

---

<a name="prompt-1"></a>
## PROMPT 1: Redesign da Homepage

```markdown
# CONTEXTO
Você é um designer UX/UI especializado em SaaS de saúde. O PrimeCare Software 
(MedicWarehouse) precisa de uma homepage moderna que converta visitantes em trials.

O sistema é um software de gestão clínica completo construído com Angular 20, .NET 8 
e PostgreSQL. Já possui funcionalidades robustas mas precisa de uma "vitrine" melhor.

# OBJETIVO
Redesenhar completamente a homepage do PrimeCare para ser:
- **Moderna e profissional** (benchmark: Stripe, Linear, Notion, iClinic)
- **Focada em conversão** (CTAs claros, social proof, urgência sutil)
- **Confiável** (badges, depoimentos, certificações)
- **Rápida** (Core Web Vitals excelentes, Lighthouse 90+)
- **Acessível** (WCAG 2.1 AA compliant)

# ANÁLISE ATUAL
- **Localização:** `/frontend/medicwarehouse-app/src/app/pages/site/home/`
- **Arquivos:** `home.html`, `home.scss`, `home.component.ts`
- **Stack:** Angular 20 + Angular Material + SCSS
- **Estado:** Funcional mas precisa modernização visual e de conversão

# REQUISITOS FUNCIONAIS

## 1. Hero Section (Acima da dobra)
**Objetivo:** Capturar atenção e comunicar proposta de valor em 5 segundos

### Elementos obrigatórios:
- [ ] **Headline impactante** (max 8 palavras)
  - Sugestão: "Gestão clínica que funciona"
  - Alternativa: "Software médico simples e completo"
  
- [ ] **Subheadline explicativo** (max 20 palavras)
  - Sugestão: "Organize consultas, prontuários e pagamentos em um só lugar. Ganhe tempo e foque no que importa: seus pacientes."
  
- [ ] **2 CTAs principais:**
  - CTA Primário: "Começar Gratuitamente" (botão grande, cor accent #6366F1)
  - CTA Secundário: "Ver Demonstração" (botão outline/ghost)
  
- [ ] **Trust badges (mini-features):**
  - "✓ 15 dias grátis"
  - "✓ Sem cartão de crédito"
  - "✓ Suporte 24/7"
  - "✓ Cancele quando quiser"
  
- [ ] **Background visual:**
  - Gradiente sutil (primary → primary-light) OU
  - Pattern geométrico moderno OU
  - Mockup/screenshot do produto em uso (device mockup)
  
- [ ] **Ilustração/Imagem:**
  - Mockup do dashboard em laptop/tablet
  - Screenshot real do sistema em uso
  - Ilustração de médico + paciente (opcional)

### Design guidelines Hero:
```scss
.hero {
  min-height: 600px; // mobile: 500px
  display: flex;
  align-items: center;
  padding: 80px 0; // mobile: 48px 0
  position: relative;
  overflow: hidden;
  
  .hero-background {
    position: absolute;
    top: 0; left: 0; right: 0; bottom: 0;
    background: linear-gradient(135deg, #EEF2FF 0%, #FFFFFF 100%);
    z-index: -1;
    
    // Adicionar circles gradientes para efeito moderno
    .gradient-circle {
      position: absolute;
      border-radius: 50%;
      filter: blur(100px);
      opacity: 0.3;
      
      &-1 {
        width: 500px; height: 500px;
        background: #6366F1;
        top: -200px; right: -100px;
      }
      
      &-2 {
        width: 400px; height: 400px;
        background: #10B981;
        bottom: -150px; left: -100px;
      }
    }
  }
}
```

## 2. Social Proof Section (Logo wall + Stats)
**Objetivo:** Construir confiança através de números e clientes reais

### Elementos:
- [ ] **Estatísticas impressionantes:**
  ```
  [Icon] 500+      [Icon] 50.000+     [Icon] 98%        [Icon] 70%
  Clínicas        Pacientes          Satisfação    Menos Faltas
  ```
  
- [ ] **Logos de clientes** (se disponíveis):
  - 6-8 logos em grayscale
  - Hover: transição para colorido
  - Placeholder: "Usado por clínicas em todo o Brasil"
  
- [ ] **Depoimento destacado:**
  ```
  "O PrimeCare reduziu nossas faltas em 65% e economizou 10 horas/semana"
  - Dr. João Silva, Clínica XYZ
  [Avatar] [5 estrelas]
  ```

## 3. Features Grid (Principais funcionalidades)
**Objetivo:** Mostrar 6-8 features principais de forma visual e escaneável

### Features a destacar:
1. **Agenda Inteligente**
   - Ícone: Calendar
   - Descrição: "Organize horários e visualize compromissos com clareza"
   
2. **Prontuário Completo**
   - Ícone: Document
   - Descrição: "Histórico, prescrições e documentos em um só lugar"
   
3. **Lembretes Automáticos**
   - Ícone: Mobile/WhatsApp
   - Descrição: "Reduza faltas com notificações via WhatsApp e SMS"
   
4. **Relatórios Precisos**
   - Ícone: Chart
   - Descrição: "Acompanhe métricas e tome decisões com dados reais"
   
5. **Segurança Garantida**
   - Ícone: Lock/Shield
   - Descrição: "Dados protegidos com criptografia e backup diário"
   
6. **Performance Ágil**
   - Ícone: Zap/Lightning
   - Descrição: "Interface rápida que economiza seu tempo"

### Design das feature cards:
```scss
.feature-card {
  background: white;
  border-radius: 12px;
  padding: 32px;
  border: 1px solid #E5E7EB;
  transition: all 250ms ease;
  
  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
    border-color: #6366F1;
  }
  
  .feature-icon {
    width: 48px; height: 48px;
    background: linear-gradient(135deg, #6366F1, #8B5CF6);
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 16px;
    
    svg { color: white; width: 24px; height: 24px; }
  }
  
  h3 {
    font-size: 20px;
    font-weight: 600;
    margin-bottom: 8px;
    color: #111827;
  }
  
  p {
    font-size: 16px;
    color: #6B7280;
    line-height: 1.6;
  }
}
```

## 4. Video Demo Section
**Objetivo:** Permitir visitante ver produto em ação

### Elementos:
- [ ] **Player de vídeo estilizado:**
  - Thumbnail customizado (screenshot do dashboard)
  - Play button grande e atrativo
  - Duração visível
  - Placeholder se vídeo não existe ainda
  
- [ ] **Texto de apoio:**
  - Badge: "🎥 Veja o sistema em ação"
  - Headline: "Conheça o PrimeCare em detalhes"
  - Descrição: "Assista ao vídeo e descubra como nosso sistema..."
  
- [ ] **Features listadas ao lado:**
  - ✓ Interface intuitiva - Fácil de usar desde o primeiro dia
  - ⚡ Rápido e eficiente - Economize horas de trabalho
  - 🎯 Suporte dedicado - Nossa equipe está sempre disponível

### Se vídeo não existe:
```html
<div class="video-placeholder">
  <div class="video-placeholder-content">
    <div class="play-button-large">
      <svg><!-- play icon --></svg>
    </div>
    <p class="video-placeholder-text">Vídeo de demonstração em breve</p>
    <p class="video-placeholder-subtext">Estamos preparando conteúdo exclusivo</p>
  </div>
  <!-- Decorative background pattern -->
</div>
```

## 5. How It Works (3 passos simples)
**Objetivo:** Reduzir fricção mostrando que é fácil começar

### Estrutura:
```
[1]                    [2]                    [3]
Crie sua conta     →   Configure           →   Comece a atender
Cadastro simples       Personalize             Gerencie consultas
em 5 minutos           horários e equipe       com eficiência
```

### Design:
- Números grandes e coloridos (gradiente)
- Ícones ilustrativos para cada passo
- Conectores visuais entre passos (linhas ou arrows)
- CTA no final: "Começar agora"

## 6. Pricing Teaser (Optional na homepage)
**Objetivo:** Transparência de preços aumenta conversão

### Elementos:
- [ ] **3 planos lado a lado:**
  - Starter (R$ 89/mês)
  - Professional (R$ 189/mês) ⭐ Mais Popular
  - Premium (R$ 329/mês)
  
- [ ] **Features principais de cada plano** (3-5 por plano)
  
- [ ] **Badge "Mais Popular"** no plano Professional
  
- [ ] **Link "Ver todos os planos"** → /site/pricing

### Design:
```scss
.pricing-card {
  background: white;
  border: 2px solid #E5E7EB;
  border-radius: 16px;
  padding: 32px;
  
  &.popular {
    border-color: #6366F1;
    position: relative;
    
    &::before {
      content: "Mais Popular";
      position: absolute;
      top: -12px; left: 50%;
      transform: translateX(-50%);
      background: #6366F1;
      color: white;
      padding: 4px 16px;
      border-radius: 12px;
      font-size: 14px;
      font-weight: 600;
    }
  }
}
```

## 7. Final CTA Section
**Objetivo:** Última chance de conversão antes do footer

### Elementos:
- [ ] **Background impactante:**
  - Gradiente colorido OU
  - Padrão geométrico OU
  - Imagem com overlay
  
- [ ] **Ícone de sucesso/checkmark grande**
  
- [ ] **Headline final:**
  - "Pronto para começar?"
  - "Transforme sua clínica hoje"
  
- [ ] **Subheadline:**
  - "Experimente gratuitamente por 15 dias. Sem compromisso."
  
- [ ] **Botões:**
  - Primário: "Começar agora" (grande, branco se bg colorido)
  - Secundário: "Falar com consultor" (WhatsApp)
  
- [ ] **Mini trust badges:**
  - ✓ Sem risco
  - ✓ Suporte premium
  - ✓ Cancele quando quiser

### Design:
```scss
.cta-section {
  background: linear-gradient(135deg, #6366F1 0%, #8B5CF6 100%);
  padding: 80px 0;
  text-align: center;
  position: relative;
  overflow: hidden;
  
  h2, p { color: white; }
  
  .btn-white {
    background: white;
    color: #6366F1;
    
    &:hover {
      background: #F9FAFB;
      transform: scale(1.05);
    }
  }
}
```

# ESPECIFICAÇÕES TÉCNICAS

## Estrutura de Arquivos
```
/frontend/medicwarehouse-app/src/app/pages/site/home/
├── home.component.ts (lógica + animações)
├── home.component.html (template)
├── home.component.scss (estilos)
└── home.component.spec.ts (testes)
```

## Tecnologias e Bibliotecas

### Angular Animations
```typescript
import { trigger, transition, style, animate } from '@angular/animations';

export const fadeIn = trigger('fadeIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(20px)' }),
    animate('600ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
  ])
]);
```

### Intersection Observer (scroll animations)
```typescript
import { ViewportScroller } from '@angular/common';

@HostListener('window:scroll')
onScroll() {
  // Animate elements on scroll
  const elements = document.querySelectorAll('.animate-on-scroll');
  elements.forEach(el => {
    const rect = el.getBoundingClientRect();
    if (rect.top < window.innerHeight * 0.8) {
      el.classList.add('visible');
    }
  });
}
```

### Ícones
- **Fonte:** Heroicons (heroicons.com) ou Lucide (lucide.dev)
- **Instalação:** 
  ```bash
  npm install lucide-angular
  ```
- **Uso:** Componente `<lucide-icon name="calendar">`

## Paleta de Cores (Design System)
```scss
// Primary
$primary-50: #EEF2FF;
$primary-500: #6366F1; // Base
$primary-600: #4F46E5;

// Secondary
$secondary-500: #10B981;

// Accent
$accent-500: #F59E0B;

// Neutral
$neutral-50: #F9FAFB;
$neutral-100: #F3F4F6;
$neutral-200: #E5E7EB;
$neutral-500: #6B7280;
$neutral-900: #111827;
```

## Tipografia
```scss
$font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

h1 { font-size: 56px; font-weight: 700; line-height: 1.1; } // mobile: 36px
h2 { font-size: 42px; font-weight: 700; line-height: 1.2; } // mobile: 30px
h3 { font-size: 28px; font-weight: 600; line-height: 1.3; } // mobile: 24px
body { font-size: 18px; line-height: 1.6; } // mobile: 16px
```

## Espaçamento e Layout
```scss
$container-max-width: 1280px;
$section-padding-y: 80px; // mobile: 48px
$grid-gap: 32px; // mobile: 24px
```

## Responsividade
```scss
// Breakpoints
$mobile: 640px;
$tablet: 768px;
$desktop: 1024px;
$wide: 1280px;

// Usage
@media (max-width: $tablet) {
  .hero { padding: 48px 0; }
  h1 { font-size: 36px; }
}
```

# ANIMAÇÕES E MICRO-INTERAÇÕES

## Scroll Animations (Fade in on scroll)
```scss
.animate-on-scroll {
  opacity: 0;
  transform: translateY(30px);
  transition: opacity 600ms ease-out, transform 600ms ease-out;
  
  &.visible {
    opacity: 1;
    transform: translateY(0);
  }
}
```

## Button Hover Effects
```scss
.btn {
  transition: all 250ms ease;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  }
  
  &:active {
    transform: translateY(0);
  }
}
```

## Card Hover Effects
```scss
.feature-card {
  transition: all 250ms ease;
  
  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
  }
}
```

# ACESSIBILIDADE (WCAG 2.1 AA)

## Checklist:
- [ ] Contraste de texto ≥ 4.5:1
- [ ] Contraste de elementos UI ≥ 3:1
- [ ] Todos os botões são focusáveis por teclado
- [ ] Focus indicators visíveis
- [ ] Alt text em todas as imagens
- [ ] Heading hierarchy correta (H1 → H2 → H3)
- [ ] ARIA labels onde necessário
- [ ] Links descritivos (não "clique aqui")
- [ ] Navegação por teclado funcional

## Exemplo de implementação:
```html
<button 
  aria-label="Começar trial gratuito de 15 dias"
  class="btn-primary">
  Começar Gratuitamente
</button>

<img 
  src="dashboard-mockup.webp" 
  alt="Screenshot do dashboard do PrimeCare mostrando agenda e estatísticas">
```

# PERFORMANCE

## Otimizações obrigatórias:

### Imagens
- [ ] Formato WebP (fallback para PNG/JPG)
- [ ] Lazy loading (`loading="lazy"`)
- [ ] Tamanhos responsivos (`srcset`)
- [ ] Compressão (TinyPNG, Squoosh)

```html
<img 
  src="hero-image.webp"
  srcset="hero-image-mobile.webp 640w,
          hero-image-tablet.webp 1024w,
          hero-image-desktop.webp 1920w"
  sizes="(max-width: 640px) 100vw,
         (max-width: 1024px) 80vw,
         1280px"
  loading="lazy"
  alt="...">
```

### Fonts
- [ ] Preload de fonts críticas
- [ ] `font-display: swap`
- [ ] Subset de caracteres (só Latin)

```html
<link rel="preload" 
      href="/assets/fonts/inter-var.woff2" 
      as="font" 
      type="font/woff2" 
      crossorigin>
```

### CSS
- [ ] Critical CSS inline
- [ ] Resto lazy-loaded
- [ ] Minificação
- [ ] Purge de classes não usadas

### JavaScript
- [ ] Code splitting (lazy load de rotas)
- [ ] Tree shaking
- [ ] Minificação
- [ ] Defer de scripts não críticos

## Metas de Performance:
- **Lighthouse Performance:** > 90
- **First Contentful Paint:** < 1.8s
- **Time to Interactive:** < 3.9s
- **Cumulative Layout Shift:** < 0.1
- **Largest Contentful Paint:** < 2.5s

# SEO (BÁSICO)

## Meta Tags obrigatórias:
```html
<title>PrimeCare Software - Sistema de Gestão para Clínicas Médicas</title>
<meta name="description" 
      content="Software completo para gestão de consultórios e clínicas. Agenda, prontuário eletrônico, telemedicina e mais. Experimente grátis por 15 dias.">

<!-- Open Graph -->
<meta property="og:title" content="PrimeCare Software - Sistema de Gestão Clínica">
<meta property="og:description" content="Software completo para gestão de consultórios...">
<meta property="og:image" content="https://primecare.com.br/og-image.jpg">
<meta property="og:url" content="https://primecare.com.br">

<!-- Twitter Card -->
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:title" content="PrimeCare Software">
<meta name="twitter:description" content="Software completo...">
<meta name="twitter:image" content="https://primecare.com.br/twitter-image.jpg">

<!-- Canonical -->
<link rel="canonical" href="https://primecare.com.br">
```

## Structured Data (Schema.org):
```html
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "SoftwareApplication",
  "name": "PrimeCare Software",
  "applicationCategory": "HealthApplication",
  "offers": {
    "@type": "Offer",
    "price": "89.00",
    "priceCurrency": "BRL"
  },
  "aggregateRating": {
    "@type": "AggregateRating",
    "ratingValue": "4.9",
    "ratingCount": "500"
  }
}
</script>
```

# TESTES E VALIDAÇÃO

## Checklist de QA:
- [ ] Testar em Chrome, Safari, Firefox, Edge
- [ ] Testar em mobile (iOS Safari, Chrome Android)
- [ ] Testar com keyboard navigation (Tab, Enter, Esc)
- [ ] Testar com screen reader (NVDA, VoiceOver)
- [ ] Lighthouse audit (Performance, Accessibility, SEO)
- [ ] Validar HTML (validator.w3.org)
- [ ] Validar contraste (WebAIM Contrast Checker)
- [ ] Validar responsividade (todos breakpoints)

## Métricas de Sucesso:
- [ ] Conversão website→trial: +50% (de 1.5% para 2.25%+)
- [ ] Tempo médio na página: > 2 minutos
- [ ] Taxa de rejeição: < 50%
- [ ] Lighthouse scores: 90+ em todas as métricas

# ENTREGÁVEIS

1. ✅ Código-fonte atualizado (home.component.*)
2. ✅ Assets (imagens, ícones otimizados)
3. ✅ Screenshots before/after
4. ✅ Lighthouse report (PDF)
5. ✅ Documentação de mudanças (CHANGELOG)
6. ✅ Guia de manutenção (README)

# PRAZO
**4-5 dias** de trabalho focado para um desenvolvedor senior

# INSPIRAÇÕES (Benchmarks)

## SaaS de Saúde:
- iClinic.com.br (estrutura e features)
- amplimed.com.br (design moderno)
- doctoralia.com.br (simplicidade)

## SaaS de Referência:
- stripe.com (profissionalismo e clareza)
- linear.app (design moderno, animações sutis)
- notion.so (simplicidade e legibilidade)
- vercel.com (performance e gradientes)

## Design Systems:
- Tailwind UI (componentes prontos)
- Material Design 3 (guidelines)
- Radix UI (acessibilidade)

# RECURSOS ADICIONAIS

## Imagens Stock (gratuitas):
- Unsplash.com (fotos de consultórios/médicos)
- Pexels.com (alternativa ao Unsplash)
- Undraw.co (ilustrações customizáveis)

## Ícones:
- Heroicons.com (oficial do Tailwind)
- Lucide.dev (fork melhorado do Feather)
- Phosphoricons.com (mais opções)

## Gradientes:
- uigradients.com (galeria de gradientes)
- cssgradient.io (gerador)

## Ferramentas de Design:
- Figma (design e prototipagem)
- Canva (mockups rápidos)
- Excalidraw (wireframes)

---

**IMPORTANTE:** Este é um redesign focado em conversão. Toda decisão de design deve
ser validada com métricas (A/B testing se possível). O objetivo é aumentar conversões,
não apenas "ficar bonito".

**BOA SORTE COM A IMPLEMENTAÇÃO! 🚀**
```

---

<a name="prompt-2"></a>
## PROMPT 2: Vídeo Demonstrativo

(Conteúdo completo do prompt já foi incluído no documento PLANO_MELHORIAS_WEBSITE_UXUI.md)

**Ver:** PLANO_MELHORIAS_WEBSITE_UXUI.md, seção "PROMPT 2"

---

<a name="prompt-3"></a>
## PROMPT 3: Design System Atualização

(Conteúdo completo do prompt já foi incluído no documento PLANO_MELHORIAS_WEBSITE_UXUI.md)

**Ver:** PLANO_MELHORIAS_WEBSITE_UXUI.md, seção "PROMPT 3"

---

<a name="prompt-4"></a>
## PROMPT 4: Tour Guiado/Onboarding

(Conteúdo completo do prompt já foi incluído no documento PLANO_MELHORIAS_WEBSITE_UXUI.md)

**Ver:** PLANO_MELHORIAS_WEBSITE_UXUI.md, seção "PROMPT 4"

---

<a name="prompt-5"></a>
## PROMPT 5: Blog Técnico e SEO

(Conteúdo completo do prompt já foi incluído no documento PLANO_MELHORIAS_WEBSITE_UXUI.md)

**Ver:** PLANO_MELHORIAS_WEBSITE_UXUI.md, seção "PROMPT 5"

---

<a name="prompt-6"></a>
## PROMPT 6: Empty States

```markdown
# CONTEXTO
Empty states (estados vazios) são momentos críticos na UX onde não há dados para 
exibir. São oportunidades para guiar o usuário sobre próximas ações.

# OBJETIVO
Criar empty states amigáveis e acionáveis para todas as principais telas do 
PrimeCare Software.

# PRINCÍPIOS DE EMPTY STATES

## 1. Seja Humano
❌ "Nenhum registro encontrado"
✅ "Sua lista está vazia (por enquanto!)"

## 2. Explique o Por quê
❌ "Sem pacientes"
✅ "Você ainda não adicionou nenhum paciente"

## 3. Mostre o Próximo Passo
❌ Apenas mensagem
✅ Mensagem + Botão "Adicionar primeiro paciente"

## 4. Use Ilustrações
- Adiciona personalidade
- Facilita compreensão
- Reduz frustração

# EMPTY STATES A CRIAR

## 1. Lista de Pacientes Vazia
**Localização:** `/app/patients/patient-list`

**Conteúdo:**
```html
<div class="empty-state">
  <!-- Ilustração: Pessoas com ícone de + -->
  <img src="/assets/illustrations/empty-patients.svg" 
       alt="Nenhum paciente cadastrado">
  
  <h3>Nenhum paciente cadastrado</h3>
  <p>
    Adicione seu primeiro paciente para começar a usar o sistema.
    É rápido e fácil!
  </p>
  
  <button mat-raised-button color="primary" 
          (click)="openAddPatientDialog()">
    <mat-icon>add</mat-icon>
    Adicionar Primeiro Paciente
  </button>
  
  <a href="/help/adding-patients" class="link-secondary">
    Como adicionar pacientes?
  </a>
</div>
```

## 2. Agenda Vazia
**Localização:** `/app/appointments`

**Conteúdo:**
```html
<div class="empty-state">
  <img src="/assets/illustrations/empty-calendar.svg" 
       alt="Agenda vazia">
  
  <h3>Nenhuma consulta agendada</h3>
  <p>
    Sua agenda está livre. Que tal agendar a primeira consulta?
  </p>
  
  <button mat-raised-button color="primary"
          (click)="openNewAppointmentDialog()">
    <mat-icon>event</mat-icon>
    Agendar Primeira Consulta
  </button>
  
  <div class="quick-links">
    <a href="/help/calendar">Ver tutorial da agenda</a>
    <span>•</span>
    <a href="/app/patients">Ver pacientes</a>
  </div>
</div>
```

## 3. Busca Sem Resultados
**Localização:** Qualquer tela com busca

**Conteúdo:**
```html
<div class="empty-state-search">
  <img src="/assets/illustrations/search-empty.svg" 
       alt="Nenhum resultado encontrado">
  
  <h3>Nenhum resultado para "{{ searchTerm }}"</h3>
  <p>Tente buscar por:</p>
  
  <ul class="suggestions">
    <li>Nome completo do paciente</li>
    <li>CPF ou RG</li>
    <li>Telefone de contato</li>
  </ul>
  
  <button mat-stroked-button (click)="clearSearch()">
    Limpar busca
  </button>
</div>
```

## 4. Notificações Vazias
**Localização:** `/app/notifications`

**Conteúdo:**
```html
<div class="empty-state-notifications">
  <img src="/assets/illustrations/inbox-zero.svg" 
       alt="Nenhuma notificação">
  
  <h3>Caixa limpa! 🎉</h3>
  <p>
    Você não tem novas notificações.
    Volte mais tarde para ver atualizações.
  </p>
  
  <button mat-button (click)="closePanel()">
    Fechar
  </button>
</div>
```

## 5. Relatórios Sem Dados
**Localização:** `/app/reports` (qualquer relatório)

**Conteúdo:**
```html
<div class="empty-state-report">
  <img src="/assets/illustrations/empty-chart.svg" 
       alt="Sem dados para relatório">
  
  <h3>Dados insuficientes</h3>
  <p>
    Não há dados suficientes para gerar este relatório no período selecionado.
  </p>
  
  <div class="actions">
    <button mat-raised-button color="primary"
            (click)="adjustDateRange()">
      Ajustar Período
    </button>
    <button mat-button (click)="goToDashboard()">
      Voltar ao Dashboard
    </button>
  </div>
</div>
```

## 6. Primeiro Acesso (Onboarding)
**Localização:** `/app/dashboard` (primeira vez)

**Conteúdo:**
```html
<div class="empty-state-onboarding">
  <img src="/assets/illustrations/welcome.svg" 
       alt="Bem-vindo ao PrimeCare">
  
  <h2>Bem-vindo ao PrimeCare! 👋</h2>
  <p>
    Vamos configurar sua clínica em 5 minutos.
    Você pode pular etapas e fazer depois.
  </p>
  
  <div class="checklist">
    <div class="checklist-item" [class.completed]="hasCompletedStep1">
      <mat-icon>{{ hasCompletedStep1 ? 'check_circle' : 'radio_button_unchecked' }}</mat-icon>
      <span>Configure horários de atendimento</span>
    </div>
    <div class="checklist-item" [class.completed]="hasCompletedStep2">
      <mat-icon>{{ hasCompletedStep2 ? 'check_circle' : 'radio_button_unchecked' }}</mat-icon>
      <span>Adicione um paciente</span>
    </div>
    <div class="checklist-item" [class.completed]="hasCompletedStep3">
      <mat-icon>{{ hasCompletedStep3 ? 'check_circle' : 'radio_button_unchecked' }}</mat-icon>
      <span>Agende sua primeira consulta</span>
    </div>
  </div>
  
  <button mat-raised-button color="primary"
          (click)="startOnboarding()">
    Começar Configuração
  </button>
  
  <button mat-button (click)="skipOnboarding()">
    Pular por agora
  </button>
</div>
```

# ESTILOS GLOBAIS PARA EMPTY STATES

```scss
// /src/styles/components/_empty-states.scss

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: 80px 24px;
  min-height: 400px;
  
  img {
    width: 240px;
    height: 240px;
    margin-bottom: 32px;
    opacity: 0.9;
  }
  
  h3 {
    font-size: 24px;
    font-weight: 600;
    color: $neutral-900;
    margin-bottom: 12px;
  }
  
  p {
    font-size: 16px;
    color: $neutral-600;
    max-width: 480px;
    margin-bottom: 24px;
    line-height: 1.6;
  }
  
  button {
    margin-bottom: 16px;
  }
  
  .link-secondary {
    font-size: 14px;
    color: $primary-600;
    text-decoration: none;
    
    &:hover {
      text-decoration: underline;
    }
  }
  
  .quick-links {
    display: flex;
    gap: 12px;
    align-items: center;
    font-size: 14px;
    
    a {
      color: $primary-600;
      text-decoration: none;
      
      &:hover { text-decoration: underline; }
    }
    
    span { color: $neutral-400; }
  }
  
  ul.suggestions {
    list-style: none;
    padding: 0;
    margin: 16px 0;
    
    li {
      padding: 8px 0;
      color: $neutral-600;
      
      &::before {
        content: "→";
        margin-right: 8px;
        color: $primary-500;
      }
    }
  }
}

// Animação de entrada
.empty-state {
  animation: fadeInUp 500ms ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// Responsividade
@media (max-width: $mobile) {
  .empty-state {
    padding: 48px 16px;
    
    img {
      width: 180px;
      height: 180px;
    }
    
    h3 {
      font-size: 20px;
    }
    
    p {
      font-size: 14px;
    }
  }
}
```

# ILUSTRAÇÕES

## Opção 1: Usar Biblioteca Gratuita
- **Undraw:** undraw.co (customizável por cor)
- **Storyset:** storyset.com (animadas)
- **Manypixels:** manypixels.co/gallery

## Opção 2: Criar Custom (Figma)
- Usar ícones do sistema + formas simples
- Manter paleta de cores do design system
- Exportar em SVG (otimizado)

## Opção 3: Usar Ícones Grandes
Se não tem ilustração pronta:
```html
<div class="empty-state-icon">
  <mat-icon>inbox</mat-icon>
</div>

<style>
.empty-state-icon {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  background: linear-gradient(135deg, $primary-100, $primary-50);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 24px;
  
  mat-icon {
    font-size: 64px;
    width: 64px;
    height: 64px;
    color: $primary-500;
  }
}
</style>
```

# BOAS PRÁTICAS

## ✅ Fazer:
- Usar tom positivo e encorajador
- Mostrar caminho claro para ação
- Incluir ilustração ou ícone grande
- Oferecer ajuda/tutorial
- Manter consistência visual

## ❌ Evitar:
- Mensagens técnicas/de erro
- Deixar usuário sem opções
- Usar muito texto
- Culpar o usuário
- Empty state genérico para tudo

# TESTES

## Checklist:
- [ ] Testar em diferentes resoluções
- [ ] Verificar se CTAs funcionam
- [ ] Validar acessibilidade (alt text, focus)
- [ ] Garantir que ilustrações carregam
- [ ] Testar com tela escura (se aplicável)

# ENTREGÁVEIS

1. ✅ Componentes de empty state (HTML + SCSS + TS)
2. ✅ Ilustrações ou ícones (SVG otimizados)
3. ✅ Documentação de uso
4. ✅ Screenshots de cada empty state

# PRAZO
2-3 dias para criar todos os empty states principais

# MÉTRICAS DE SUCESSO
- Redução de 40% em tickets "não sei como usar X"
- Aumento de 30% em ações tomadas após empty state
- Feedback positivo de usuários

**BOA IMPLEMENTAÇÃO! 🎨**
```

---

## 📚 Documentos Relacionados

1. **ANALISE_COMPETITIVA_MEDICWAREHOUSE.md** - Análise de mercado
2. **PLANO_MELHORIAS_WEBSITE_UXUI.md** - Plano estratégico completo
3. Este documento - Prompts técnicos para implementação

---

> **Última Atualização:** 28 de Janeiro de 2026  
> **Versão:** 1.0  
> **Uso:** Copiar e colar prompts conforme necessário

> **Nota:** Os demais prompts (7-10) seguem estrutura similar e estão disponíveis 
> no documento PLANO_MELHORIAS_WEBSITE_UXUI.md para referência.
