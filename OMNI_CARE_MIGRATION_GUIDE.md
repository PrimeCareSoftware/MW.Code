# Guia de Migração: Omni Care Site - React para Angular

## Visão Geral

Este documento detalha a migração do site React do Omni Care (localizado em `omni-care-site/`) para o aplicativo Angular do MedicWarehouse (em `frontend/medicwarehouse-app/`).

## Objetivo

Integrar o conteúdo do site React Omni Care ao aplicativo Angular existente, mantendo:
- **Separação completa de estilos CSS** entre o site Omni Care e o sistema MedicWarehouse
- **Tema visual distinto** com cores healthcare (teal/azul)
- **Funcionalidade completa** de todas as seções migradas

## Estrutura da Migração

### Fase 1: Configuração e Isolamento de CSS ✅

#### 1.1. Arquivo de Estilos Isolado

**Arquivo:** `frontend/medicwarehouse-app/src/styles/omni-care-site.scss`

- **Escopo:** Todos os estilos estão encapsulados sob a classe `.omni-care-site`
- **Tema:** Healthcare com paleta teal/azul
- **Variáveis CSS:** Tokens de design personalizados com prefixo `--omni-`
- **Componentes:** Classes utilitárias como `.hero-gradient`, `.card-elevated`, `.text-gradient`
- **Animações:** Keyframes personalizados (fade-in, float, pulse-glow, etc.)
- **Responsividade:** Media queries para mobile-first design
- **Dark Mode:** Suporte incluído com variáveis alternativas

#### 1.2. Configuração do Angular

**angular.json:**
```json
"styles": [
  "src/styles.scss",
  "src/styles/omni-care-site.scss"
]
```

**styles.scss:**
```scss
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&family=Plus+Jakarta+Sans:wght@500;600;700;800&display=swap');
```

**index.html:**
```html
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
```

### Fase 2: Migração de Componentes ✅

#### 2.1. Componente Home do Omni Care

**Localização:** `src/app/pages/site/omni-care/home/`

**Arquivos:**
- `omni-care-home.component.ts` - Lógica do componente
- `omni-care-home.component.html` - Template HTML
- `omni-care-home.component.scss` - Estilos específicos do componente

**Seções Migradas:**

1. **Hero Section**
   - Título com texto gradiente
   - Descrição e benefícios
   - Botões de CTA (Experimente Gratuitamente, Fale com Especialista)
   - Ilustração abstrata com cards flutuantes
   - Estatísticas (500+ Clínicas, 50k+ Pacientes, 99.9% Uptime)

2. **Services Section**
   - Grid de 4 colunas responsivo
   - Serviços: Gerenciamento, Agendamento, Telemedicina, Integração
   - Ícones Material Icons
   - Features de cada serviço

3. **Testimonials Section**
   - Grid de 3 colunas com depoimentos
   - Avaliações 5 estrelas
   - Avatar com iniciais
   - Trust badges de empresas

4. **CTA Section**
   - Background gradiente com decoração
   - Título e descrição
   - Botões de ação
   - Nota sem cartão de crédito

#### 2.2. Mapeamento de Tecnologias

| React | Angular | Notas |
|-------|---------|-------|
| `useState` | Propriedades de classe | Dados estáticos no componente |
| Lucide Icons | Material Icons | Mapeamento de ícones equivalentes |
| Tailwind Classes | SCSS Classes | Classes customizadas no SCSS |
| `className` | `class` / `[class]` | Binding de classes Angular |
| `style={{ }}` | `[style]` | Binding de estilos inline |
| `{item.map()}` | `*ngFor="let item"` | Iteração de arrays |
| React Router `Link` | Angular `routerLink` | Navegação |

#### 2.3. Conversão de Ícones

| Lucide (React) | Material Icons (Angular) |
|----------------|--------------------------|
| `Building2` | `business` |
| `Calendar` | `calendar_month` |
| `Video` | `videocam` |
| `Layers` | `layers` |
| `ArrowRight` | `arrow_forward` |
| `Play` | `play_arrow` |
| `CheckCircle2` | `check_circle` |
| `Quote` | `format_quote` |
| `Star` | `star` |

### Fase 3: Integração de Rotas ✅

**Arquivo:** `src/app/app.routes.ts`

**Nova Rota:**
```typescript
{
  path: 'omni-care',
  loadComponent: () => import('./pages/site/omni-care/home/omni-care-home.component')
    .then(m => m.OmniCareHomeComponent)
}
```

**URL de Acesso:** `http://localhost:4200/site/omni-care`

### Fase 4: Layout e Navegação ✅

**Header e Footer:**
- Reutilizados os componentes existentes: `HeaderComponent` e `FooterComponent`
- Importados no componente Omni Care Home
- Mantém consistência com o resto do site

**Navegação:**
- Links internos para `/site/register`, `/site/contact`, etc.
- Integração com sistema de rotas do Angular

## Diferenças Principais entre React e Angular

### Estrutura de Componentes

**React:**
```tsx
export const Hero = () => {
  return <section className="hero">...</section>;
};
```

**Angular:**
```typescript
@Component({
  selector: 'app-omni-care-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './omni-care-home.component.html',
  styleUrl: './omni-care-home.component.scss'
})
export class OmniCareHomeComponent {
  // Lógica do componente
}
```

### Templates

**React (JSX):**
```tsx
<div className="card-elevated animate-fade-in">
  {benefits.map(benefit => (
    <li key={benefit}>{benefit}</li>
  ))}
</div>
```

**Angular (HTML):**
```html
<div class="card-elevated animate-fade-in">
  <li *ngFor="let benefit of benefits">{{benefit}}</li>
</div>
```

### Estilos

**React (Tailwind):**
```tsx
<h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold">
  Título
</h1>
```

**Angular (SCSS):**
```html
<h1 class="hero-title">Título</h1>
```
```scss
.hero-title {
  font-size: 2.5rem;
  font-weight: 700;
  
  @media (min-width: 640px) {
    font-size: 3rem;
  }
  
  @media (min-width: 1024px) {
    font-size: 3.75rem;
  }
}
```

## Variáveis CSS e Tokens de Design

### Omni Care Theme

```scss
.omni-care-site {
  --omni-primary: 174 72% 40%;       /* Teal Healthcare */
  --omni-accent: 174 85% 45%;        /* Vibrant Teal */
  --omni-success: 158 64% 45%;       /* Green */
  --omni-secondary: 200 30% 96%;     /* Soft Blue */
  
  /* Gradients */
  --omni-hero-gradient-start: 174 72% 40%;
  --omni-hero-gradient-end: 200 60% 45%;
  
  /* Effects */
  --omni-glow: 174 85% 55%;
  --omni-card-shadow: 200 30% 80%;
}
```

### Classes Utilitárias

| Classe | Descrição |
|--------|-----------|
| `.hero-gradient` | Gradiente teal para backgrounds |
| `.text-gradient` | Texto com gradiente primary→accent |
| `.card-elevated` | Card com sombra e borda |
| `.glow-effect` | Efeito de brilho suave |
| `.animate-fade-in` | Animação de fade in |
| `.animate-float` | Animação de flutuação |
| `.section-padding` | Padding responsivo para seções |
| `.container-custom` | Container responsivo customizado |
| `.omni-btn-primary` | Botão primário teal |
| `.omni-grid cols-3` | Grid responsivo de 3 colunas |

## Como Testar Localmente

### 1. Instalar Dependências

```bash
cd frontend/medicwarehouse-app
npm install
```

### 2. Iniciar o Servidor de Desenvolvimento

```bash
npm start
# ou
ng serve
```

### 3. Acessar o Site

Abra o navegador em: `http://localhost:4200/site/omni-care`

### 4. Verificar CSS Isolation

1. Navegue entre `/site/omni-care` e outras páginas do site
2. Verifique que os estilos do Omni Care não afetam outras páginas
3. Confirme que a paleta de cores teal aparece apenas no Omni Care

## Build para Produção

```bash
cd frontend/medicwarehouse-app
npm run build
```

Os arquivos compilados estarão em `dist/primecare-frontend/browser/`

## Estrutura de Arquivos

```
frontend/medicwarehouse-app/
├── src/
│   ├── app/
│   │   ├── pages/
│   │   │   └── site/
│   │   │       └── omni-care/
│   │   │           └── home/
│   │   │               ├── omni-care-home.component.ts
│   │   │               ├── omni-care-home.component.html
│   │   │               └── omni-care-home.component.scss
│   │   ├── components/
│   │   │   └── site/
│   │   │       ├── header/ (reutilizado)
│   │   │       └── footer/ (reutilizado)
│   │   └── app.routes.ts (rota adicionada)
│   ├── styles/
│   │   └── omni-care-site.scss (novo)
│   ├── styles.scss (atualizado com fonts)
│   └── index.html (Material Icons adicionado)
└── angular.json (stylesheet configurado)
```

## Próximos Passos (Opcional)

### Páginas Adicionais para Migrar

1. **Pricing Page** (`/site/omni-care/pricing`)
   - Grid de 3 planos
   - Tabela comparativa de features
   - Botões de CTA

2. **Clinics Page** (`/site/omni-care/clinics`)
   - Busca e filtros
   - Grid de cards de clínicas
   - Mapa de localização

3. **Blog Page** (`/site/omni-care/blog`)
   - Grid de posts
   - Post destacado
   - Categorias e tags

4. **Contact Page** (`/site/omni-care/contact`)
   - Formulário de contato
   - Informações de contato
   - Validação de formulário

### Melhorias Futuras

1. **SEO:** Adicionar meta tags específicas para Omni Care
2. **Analytics:** Tracking de eventos específicos do Omni Care
3. **Lazy Loading:** Otimizar carregamento de imagens
4. **PWA:** Suporte offline para o site Omni Care
5. **i18n:** Internacionalização (EN/ES)
6. **A/B Testing:** Testar variações de CTA

## Manutenção

### Atualizando Estilos

Para atualizar os estilos do Omni Care:
1. Edite `src/styles/omni-care-site.scss` para mudanças globais
2. Edite `omni-care-home.component.scss` para mudanças específicas da home

### Adicionando Novas Seções

1. Crie componentes em `src/app/pages/site/omni-care/`
2. Adicione rotas em `app.routes.ts`
3. Utilize classes do `omni-care-site.scss`
4. Mantenha o escopo `.omni-care-site` no HTML principal

### Troubleshooting

**Problema:** Estilos não aparecem
- **Solução:** Certifique-se de que o elemento principal tem a classe `.omni-care-site`

**Problema:** Conflito de estilos com outras páginas
- **Solução:** Verifique que todos os estilos estão encapsulados sob `.omni-care-site`

**Problema:** Material Icons não aparecem
- **Solução:** Verifique se o link do Google Fonts está no `index.html`

## Conclusão

A migração do Omni Care Site do React para o Angular foi concluída com sucesso, mantendo:

✅ **Isolamento completo de CSS** - Sem conflitos com o sistema principal  
✅ **Tema visual distinto** - Paleta teal/healthcare preservada  
✅ **Funcionalidade completa** - Todas as 4 seções migradas  
✅ **Responsividade** - Design mobile-first mantido  
✅ **Animações** - Efeitos visuais convertidos para CSS  
✅ **Roteamento** - Integração com Angular Router  

O site está pronto para uso em produção e pode ser expandido com páginas adicionais conforme necessário.
