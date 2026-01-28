# 📚 Guia de Acessibilidade - PrimeCare Software

> **Conformidade:** WCAG 2.1 Level AA | Lei Brasileira de Inclusão (LBI)  
> **Última Atualização:** Janeiro 2026  
> **Status:** ✅ Em Implementação

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Componentes de Acessibilidade](#componentes-de-acessibilidade)
3. [Padrões de Desenvolvimento](#padrões-de-desenvolvimento)
4. [Testes de Acessibilidade](#testes-de-acessibilidade)
5. [Recursos e Ferramentas](#recursos-e-ferramentas)

---

## 🎯 Visão Geral

O PrimeCare Software está comprometido com a acessibilidade digital, garantindo que todas as pessoas, independentemente de suas habilidades, possam usar o sistema de forma eficaz.

### Princípios WCAG 2.1

Nossa implementação segue os **4 princípios fundamentais** da acessibilidade web:

1. **Perceptível** - A informação deve ser apresentada de forma que os usuários possam percebê-la
2. **Operável** - Os componentes devem ser operáveis por todos os usuários
3. **Compreensível** - A informação e operação da interface devem ser compreensíveis
4. **Robusto** - O conteúdo deve ser robusto o suficiente para funcionar com tecnologias assistivas

### Conformidade Legal

- ✅ **WCAG 2.1 Level AA** - Padrão internacional de acessibilidade web
- ✅ **Lei Brasileira de Inclusão (LBI)** - Lei 13.146/2015
- ✅ **Decreto 5.296/2004** - Acessibilidade digital no Brasil

---

## 🧩 Componentes de Acessibilidade

### 1. Navegação por Teclado

#### KeyboardNavigationService

Serviço que fornece suporte completo para navegação via teclado.

```typescript
import { KeyboardNavigationService } from '@shared/accessibility/hooks/keyboard-navigation.hook';

export class MyComponent implements OnInit, OnDestroy {
  constructor(
    private keyboardNav: KeyboardNavigationService,
    private elementRef: ElementRef
  ) {}

  ngOnInit() {
    this.keyboardNav.registerHandlers(this.elementRef.nativeElement, {
      onEnter: () => this.handleSubmit(),
      onEscape: () => this.handleCancel(),
      onArrowUp: () => this.navigatePrevious(),
      onArrowDown: () => this.navigateNext()
    });
  }

  ngOnDestroy() {
    this.keyboardNav.unregisterHandlers(this.elementRef.nativeElement);
  }
}
```

**Teclas Suportadas:**
- `Enter` - Ativar/confirmar
- `Escape` - Cancelar/fechar
- `Arrow Up/Down` - Navegar em listas
- `Arrow Left/Right` - Navegar em slides/tabs
- `Tab` - Navegar entre elementos focáveis
- `Space` - Ativar botões/checkboxes

---

### 2. Suporte a Leitores de Tela

#### ScreenReaderService

Serviço para anúncios dinâmicos compatível com NVDA, JAWS e VoiceOver.

```typescript
import { ScreenReaderService } from '@shared/accessibility/hooks/screen-reader.service';

export class MyComponent {
  constructor(private screenReader: ScreenReaderService) {}

  saveData() {
    // ... lógica de salvamento
    this.screenReader.announceSuccess('Dados salvos com sucesso');
  }

  loadData() {
    this.screenReader.announceLoading('dados do paciente');
    // ... lógica de carregamento
    this.screenReader.announceLoadComplete('Dados do paciente');
  }

  showError(message: string) {
    this.screenReader.announceError(message);
  }
}
```

**Métodos Disponíveis:**
- `announce(message, mode)` - Anúncio geral
- `announceSuccess(message)` - Mensagens de sucesso
- `announceError(message)` - Mensagens de erro (alta prioridade)
- `announceWarning(message)` - Avisos
- `announceInfo(message)` - Informações gerais
- `announceNavigation(pageName)` - Mudanças de página
- `announceLoading(description)` - Estados de carregamento

---

### 3. Focus Trap (Trap de Foco)

#### FocusTrapDirective

Diretiva para manter o foco dentro de modais e diálogos.

```typescript
// No template do modal
<div appFocusTrap class="modal">
  <h2>Título do Modal</h2>
  <p>Conteúdo do modal...</p>
  <button (click)="close()">Fechar</button>
</div>
```

**Comportamento:**
- Mantém foco dentro do modal
- `Tab` navega apenas entre elementos do modal
- `Shift + Tab` navega em ordem reversa
- Ao fechar, restaura foco ao elemento anterior

---

### 4. Skip to Content

#### SkipToContentComponent

Permite usuários de teclado pularem a navegação principal.

```typescript
// No template principal (app.component.html)
<app-skip-to-content></app-skip-to-content>
<nav>...</nav>
<main id="main-content" tabindex="-1">
  <!-- Conteúdo principal -->
</main>
```

**Requisitos:**
- Link visível apenas ao receber foco
- Deve ser o primeiro elemento focável da página
- Elemento principal deve ter `id="main-content"`

---

### 5. Breadcrumbs Acessíveis

#### AccessibleBreadcrumbsComponent

Navegação estrutural semântica.

```typescript
<app-accessible-breadcrumbs [items]="breadcrumbs"></app-accessible-breadcrumbs>
```

```typescript
breadcrumbs: BreadcrumbItem[] = [
  { label: 'Início', url: '/' },
  { label: 'Pacientes', url: '/patients' },
  { label: 'João Silva' } // Página atual (sem url)
];
```

---

## 📝 Padrões de Desenvolvimento

### 1. HTML Semântico

**✅ Use tags semânticas:**
```html
<header>Cabeçalho</header>
<nav>Navegação</nav>
<main>Conteúdo principal</main>
<article>Artigo</article>
<section>Seção</section>
<aside>Conteúdo lateral</aside>
<footer>Rodapé</footer>
```

**❌ Evite divs genéricas:**
```html
<div class="header">...</div> <!-- Não semântico -->
```

---

### 2. ARIA Labels e Roles

**Use ARIA para complementar, não substituir HTML semântico:**

```html
<!-- Botões sem texto visível -->
<button aria-label="Fechar modal">
  <i class="icon-close"></i>
</button>

<!-- Links com contexto adicional -->
<a href="/edit" aria-label="Editar paciente João Silva">
  Editar
</a>

<!-- Regiões -->
<div role="region" aria-label="Resultados de busca">
  <!-- conteúdo -->
</div>

<!-- Estados dinâmicos -->
<button 
  aria-pressed="true"
  aria-expanded="false"
>
  Menu
</button>
```

---

### 3. Textos Alternativos

**Imagens:**
```html
<!-- Informativas -->
<img src="doctor.jpg" alt="Dr. João Silva, cardiologista">

<!-- Decorativas -->
<img src="decoration.svg" alt="" aria-hidden="true">
```

**Ícones:**
```html
<i class="icon-save" aria-hidden="true"></i>
<span class="sr-only">Salvar</span>
```

---

### 4. Formulários Acessíveis

```html
<form>
  <!-- Labels sempre visíveis -->
  <label for="patient-name">Nome do Paciente *</label>
  <input 
    id="patient-name"
    type="text"
    required
    aria-required="true"
    aria-describedby="name-help"
  >
  <small id="name-help">Nome completo do paciente</small>
  
  <!-- Mensagens de erro -->
  <div 
    *ngIf="nameError" 
    role="alert"
    aria-live="assertive"
    class="error-message"
  >
    {{ nameError }}
  </div>
</form>
```

---

### 5. Contrastes de Cores

**Mínimos WCAG 2.1 AA:**
- Texto normal: **4.5:1**
- Texto grande (18pt+): **3:1**
- Componentes UI: **3:1**

**Paleta Acessível:**
```scss
$primary: #1976d2;      // Azul - Contraste 4.51:1
$success: #2e7d32;      // Verde - Contraste 4.54:1
$error: #c62828;        // Vermelho - Contraste 5.13:1
$warning: #e65100;      // Laranja - Contraste 4.54:1
```

---

### 6. Indicadores de Foco

**Todos os elementos interativos devem ter foco visível:**

```scss
// Já implementado globalmente em accessibility.scss
button:focus-visible {
  outline: 3px solid #ffc107;
  outline-offset: 2px;
  box-shadow: 0 0 0 3px rgba(255, 193, 7, 0.3);
}
```

---

## 🧪 Testes de Acessibilidade

### Scripts Disponíveis

```bash
# Auditoria completa com axe-core e puppeteer
npm run audit:axe

# Testes com pa11y
npm run audit:a11y

# Lighthouse accessibility
npm run audit:lighthouse

# Testes unitários de acessibilidade
npm run test:a11y
```

### Ferramentas Manuais

1. **Extensões de Navegador:**
   - axe DevTools
   - WAVE Evaluation Tool
   - Lighthouse (Chrome DevTools)

2. **Leitores de Tela:**
   - **NVDA** (Windows - gratuito)
   - **JAWS** (Windows - pago)
   - **VoiceOver** (macOS/iOS - nativo)
   - **TalkBack** (Android - nativo)

3. **Testes de Teclado:**
   - Navegue pela página usando apenas `Tab`
   - Ative elementos com `Enter` ou `Space`
   - Feche modais com `Escape`

---

## 🎓 Checklist de Desenvolvimento

### Antes de Fazer um PR

- [ ] Todos os elementos interativos são acessíveis por teclado
- [ ] Foco visível em todos os elementos interativos
- [ ] Todos os formulários têm labels associados
- [ ] Imagens têm texto alternativo apropriado
- [ ] Contraste de cores atende WCAG AA (4.5:1)
- [ ] HTML semântico utilizado
- [ ] ARIA labels onde necessário
- [ ] Testado com leitor de tela
- [ ] Auditoria axe sem violações críticas
- [ ] Funciona com zoom 200%

---

## 📚 Recursos e Referências

### Documentação Oficial

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [MDN Web Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
- [W3C WAI-ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)

### Legislação Brasileira

- [Lei Brasileira de Inclusão (LBI)](https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2015/lei/l13146.htm)
- [Decreto 5.296/2004](http://www.planalto.gov.br/ccivil_03/_ato2004-2006/2004/decreto/d5296.htm)
- [eMAG - Modelo de Acessibilidade em Governo Eletrônico](https://www.gov.br/governodigital/pt-br/acessibilidade-digital)

### Ferramentas

- [axe DevTools](https://www.deque.com/axe/devtools/)
- [WAVE](https://wave.webaim.org/)
- [Color Contrast Analyzer](https://www.tpgi.com/color-contrast-checker/)
- [NVDA Screen Reader](https://www.nvaccess.org/)

---

## 💡 Suporte

Para dúvidas sobre acessibilidade:
- Consulte este guia
- Revise os componentes em `src/app/shared/accessibility/`
- Execute os testes de acessibilidade
- Abra uma issue no repositório

---

**Lembre-se:** Acessibilidade não é apenas conformidade legal - é sobre criar experiências inclusivas para todos os usuários! 🌟
