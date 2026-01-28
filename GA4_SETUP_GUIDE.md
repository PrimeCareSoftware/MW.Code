# Google Analytics 4 Setup Guide - PrimeCare

> **Última atualização:** 28 de Janeiro de 2026  
> **Status:** Implementado (95%) - Aguardando configuração de Measurement ID

## 📋 Visão Geral

O PrimeCare implementou tracking completo com Google Analytics 4 (GA4) para monitorar o comportamento dos usuários no site público e medir conversões.

## ✅ O Que Já Está Implementado

### 1. WebsiteAnalyticsService
**Localização:** `/frontend/medicwarehouse-app/src/app/services/analytics/website-analytics.service.ts`

Serviço Angular completo com 18+ métodos de tracking:
- ✅ Page views
- ✅ CTA clicks
- ✅ Button clicks
- ✅ Navigation tracking
- ✅ Video engagement (play, pause, complete, seek)
- ✅ Form submissions
- ✅ Search queries
- ✅ Scroll depth (25%, 50%, 75%, 100%)
- ✅ Engagement time
- ✅ Social sharing
- ✅ Conversions (trial signup, demo request, contact)
- ✅ Blog article reads
- ✅ Case study views
- ✅ Pricing plan views
- ✅ Feature usage
- ✅ Error tracking
- ✅ Custom events

### 2. Integração nos Componentes

| Componente | Status | Eventos Rastreados |
|------------|--------|-------------------|
| **HomeComponent** | ✅ Completo | Page view, CTA clicks, scroll depth, engagement time, video engagement |
| **BlogComponent** | ✅ Completo | Page view, search, category filters, article clicks |
| **BlogArticleComponent** | ✅ Completo | Page view, reading time, article likes, social sharing, related articles |
| **PricingComponent** | ✅ Completo | Page view, plan views, plan selection, contact clicks |
| **CasesComponent** | ✅ Completo | Page view, case views, contact clicks |

### 3. Script GA4
**Localização:** `/frontend/medicwarehouse-app/src/index.html`

Script adicionado no `<head>`:
```html
<!-- Google Analytics 4 -->
<script async src="https://www.googletagmanager.com/gtag/js?id=GA_MEASUREMENT_ID"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'GA_MEASUREMENT_ID', {
    send_page_view: false  // Manual page view tracking
  });
</script>
```

## 🔧 Setup - Passo a Passo

### Passo 1: Criar Propriedade GA4

1. Acesse https://analytics.google.com/
2. Clique em **Admin** (ícone de engrenagem no canto inferior esquerdo)
3. Na coluna **Account**, selecione ou crie uma conta para "PrimeCare"
4. Na coluna **Property**, clique em **Create Property**
5. Preencha os dados:
   - **Property name:** PrimeCare Software
   - **Reporting time zone:** (GMT-03:00) Brasília
   - **Currency:** Brazilian Real (R$)
6. Clique em **Next**
7. Preencha informações da empresa:
   - **Industry category:** Health & Fitness
   - **Business size:** Selecione o tamanho apropriado
8. Clique em **Create** e aceite os termos de serviço

### Passo 2: Obter Measurement ID

1. Na nova propriedade criada, vá em **Admin > Data Streams**
2. Clique em **Add stream** > **Web**
3. Preencha:
   - **Website URL:** https://primecare.com.br (ou domínio atual)
   - **Stream name:** PrimeCare Website
4. Clique em **Create stream**
5. **Copie o Measurement ID** (formato: `G-XXXXXXXXXX`)

### Passo 3: Atualizar o Código

1. Abra o arquivo `/frontend/medicwarehouse-app/src/index.html`
2. Localize as duas ocorrências de `GA_MEASUREMENT_ID`
3. Substitua por seu Measurement ID real:

**Antes:**
```html
<script async src="https://www.googletagmanager.com/gtag/js?id=GA_MEASUREMENT_ID"></script>
<script>
  ...
  gtag('config', 'GA_MEASUREMENT_ID', {
```

**Depois:**
```html
<script async src="https://www.googletagmanager.com/gtag/js?id=G-ABC123XYZ"></script>
<script>
  ...
  gtag('config', 'G-ABC123XYZ', {
```

4. Salve o arquivo
5. Faça commit e deploy da alteração

### Passo 4: Verificar Tracking

1. Acesse o site em produção
2. No Google Analytics, vá em **Reports > Realtime**
3. Você deverá ver:
   - Usuários ativos
   - Page views
   - Eventos sendo disparados
4. Navegue pelo site e verifique se os eventos aparecem em tempo real

## 📊 Eventos Customizados Configurados

### Conversões Principais

| Evento | Quando Dispara | Valor |
|--------|---------------|-------|
| `trial_signup` | Usuário inicia cadastro de trial | Valor do plano |
| `demo_request` | Usuário solicita demonstração | - |
| `contact` | Usuário envia formulário de contato | - |
| `pricing_view` | Usuário visualiza página de preços | - |

### Engagement

| Evento | Quando Dispara | Parâmetros |
|--------|---------------|-----------|
| `page_view` | Cada navegação de página | page_title, page_location |
| `click` | Click em CTA, botão ou link | button_name, location |
| `scroll_depth` | Scroll em 25%, 50%, 75%, 100% | percentage |
| `engagement_time` | Ao sair da página | duration_seconds |

### Conteúdo

| Evento | Quando Dispara | Parâmetros |
|--------|---------------|-----------|
| `search` | Busca no blog | search_term, results_count |
| `video` | Interação com vídeo | action (play/pause/complete/seek) |
| `article_read` | Leitura de artigo completa | article_slug, category, read_time |
| `case_study_view` | Visualização de case | case_id, clinic_name, specialty |
| `plan_view` | Visualização de plano | plan_name, plan_price |

## 🎯 Configurar Conversões no GA4

### 1. Marcar Eventos como Conversões

1. No GA4, vá em **Admin > Events**
2. Encontre os eventos:
   - `trial_signup`
   - `demo_request`
   - `contact`
3. Toggle **Mark as conversion** para cada um

### 2. Criar Funil de Conversão

1. Vá em **Explore** > **Funnel exploration**
2. Configure o funil:
   - **Step 1:** `page_view` (página: /pricing)
   - **Step 2:** `plan_view`
   - **Step 3:** `trial_signup`
3. Salve como "Trial Signup Funnel"

### 3. Criar Audiências

**Audiência: Visitantes Engajados**
- Condição: `scroll_depth` >= 75% OU `engagement_time` >= 60s
- Uso: Remarketing

**Audiência: Interessados em Trial**
- Condição: Visitou `/pricing` E não converteu
- Uso: Campanha de retargeting

## 📈 Dashboards Recomendados

### Dashboard 1: Website Overview
- Usuários ativos
- Page views por página
- Taxa de conversão geral
- Top CTAs clicados

### Dashboard 2: Conversões
- Trial signups por dia/semana/mês
- Funil de conversão (pricing → plan view → signup)
- Conversão por source/medium
- Conversão por plano escolhido

### Dashboard 3: Conteúdo
- Artigos mais lidos
- Tempo médio de leitura
- Buscas mais realizadas
- Artigos com maior taxa de compartilhamento

### Dashboard 4: Engagement
- Scroll depth médio por página
- Tempo de engajamento por página
- Vídeos mais assistidos
- Taxa de conclusão de vídeos

## 🔍 Troubleshooting

### Eventos não aparecem no GA4

1. **Verifique o Measurement ID:**
   ```bash
   grep -r "GA_MEASUREMENT_ID" frontend/medicwarehouse-app/src/index.html
   ```
   - Se ainda aparecer o placeholder, substitua pelo ID real

2. **Verifique se o gtag está carregado:**
   - Abra DevTools (F12)
   - Vá em Console
   - Digite: `typeof gtag`
   - Deve retornar `"function"`, não `"undefined"`

3. **Verifique requests no Network:**
   - Abra DevTools > Network
   - Filtre por "collect"
   - Você deve ver requests para `https://www.google-analytics.com/g/collect`

4. **Verifique console logs:**
   - O serviço logra eventos no console em desenvolvimento
   - Procure por `[Analytics]` no console

### Duplicate events

- Se você ver eventos duplicados, verifique se:
  - O script GA4 não está duplicado no `index.html`
  - Não há outro script de analytics conflitante
  - `send_page_view` está configurado como `false`

### Conversões não são atribuídas

- Certifique-se de que:
  - Os eventos estão marcados como "conversão" no painel GA4
  - O cookie consent está permitindo analytics
  - Third-party cookies estão habilitados

## 🚀 Próximos Passos (Opcional)

### 1. Google Tag Manager (GTM)
- Migrar tracking para GTM para facilitar gestão
- Criar tags customizadas sem deploy de código

### 2. Enhanced E-commerce
- Tracking de produtos/planos como e-commerce
- Métricas de revenue mais detalhadas

### 3. Server-side Tracking
- Implementar tracking server-side para maior precisão
- Bypass ad-blockers

### 4. Heatmaps
- Integrar Hotjar ou Microsoft Clarity
- Visualizar onde usuários clicam mais

### 5. A/B Testing
- Integrar Google Optimize ou Optimizely
- Testar variações de CTAs e layouts

## 📚 Recursos

- [GA4 Documentation](https://developers.google.com/analytics/devguides/collection/ga4)
- [GA4 Best Practices](https://support.google.com/analytics/topic/9303475)
- [gtag.js Reference](https://developers.google.com/gtagjs/reference/api)
- [GA4 vs Universal Analytics](https://support.google.com/analytics/answer/11583528)

## ✅ Checklist Final

Antes de considerar o tracking 100% completo:

- [ ] Measurement ID configurado no `index.html`
- [ ] Site em produção com novo código
- [ ] Eventos aparecem no GA4 Realtime
- [ ] Eventos de conversão marcados no painel
- [ ] Funil de conversão criado
- [ ] Audiências configuradas
- [ ] Dashboards criados
- [ ] Time treinado para ler relatórios
- [ ] Alertas configurados para quedas de tráfego

---

**Autor:** GitHub Copilot Agent  
**Data:** 28 de Janeiro de 2026  
**Versão:** 1.0
