# 🧪 Guia de Testes de Acessibilidade - Omni Care Software

> **Conformidade:** WCAG 2.1 Level AA  
> **Última Atualização:** Janeiro 2026

## 📋 Índice

1. [Testes Automatizados](#testes-automatizados)
2. [Testes Manuais](#testes-manuais)
3. [Testes com Leitores de Tela](#testes-com-leitores-de-tela)
4. [Testes de Navegação por Teclado](#testes-de-navegação-por-teclado)
5. [Relatórios de Auditoria](#relatórios-de-auditoria)

---

## 🤖 Testes Automatizados

### 1. Auditoria com axe-core

Execute a auditoria completa do site:

```bash
# Certifique-se de que o frontend está rodando
npm start

# Em outro terminal, execute a auditoria
cd frontend/medicwarehouse-app
npm run audit:axe
```

**Saída:**
- Relatório HTML: `a11y-reports/summary.html`
- Detalhes JSON: `a11y-reports/*.json`

**Interpretação:**
- ✅ **0 violações críticas/sérias** = Aprovado
- ⚠️ **1+ violações críticas** = Correções necessárias
- 📝 **Violações moderadas/menores** = Melhorias recomendadas

---

### 2. Testes com pa11y

```bash
npm run audit:a11y
```

Testa páginas individuais contra padrões WCAG 2.1 AA.

---

### 3. Lighthouse Accessibility

```bash
npm run audit:lighthouse
```

**Score mínimo esperado:** 95+

**Relatório:** `a11y-reports/lighthouse-a11y.html`

---

### 4. Testes Unitários de Acessibilidade

Exemplo de teste com jasmine-axe:

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { axe, toHaveNoViolations } from 'jasmine-axe';
import { PatientListComponent } from './patient-list.component';

describe('PatientListComponent - Accessibility', () => {
  let component: PatientListComponent;
  let fixture: ComponentFixture<PatientListComponent>;

  beforeEach(() => {
    jasmine.addMatchers(toHaveNoViolations);
    TestBed.configureTestingModule({
      imports: [PatientListComponent]
    });
    fixture = TestBed.createComponent(PatientListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should not have any accessibility violations', async () => {
    const results = await axe(fixture.nativeElement);
    expect(results).toHaveNoViolations();
  });

  it('should have proper ARIA labels', () => {
    const table = fixture.nativeElement.querySelector('table');
    expect(table?.getAttribute('aria-label')).toBeTruthy();
  });

  it('should be keyboard navigable', () => {
    const buttons = fixture.nativeElement.querySelectorAll('button');
    buttons.forEach(button => {
      expect(button.tabIndex).toBeGreaterThanOrEqual(0);
    });
  });
});
```

---

## ✋ Testes Manuais

### 1. Teste de Contraste de Cores

**Ferramenta:** [Color Contrast Analyzer](https://www.tpgi.com/color-contrast-checker/)

**Checklist:**
- [ ] Texto normal: contraste mínimo 4.5:1
- [ ] Texto grande (18pt+): contraste mínimo 3:1
- [ ] Componentes de UI: contraste mínimo 3:1
- [ ] Teste em modo claro e escuro

**Exemplo de teste:**

| Elemento | Cor Texto | Cor Fundo | Contraste | Status |
|----------|-----------|-----------|-----------|--------|
| Texto principal | #212121 | #ffffff | 16.1:1 | ✅ |
| Link azul | #1976d2 | #ffffff | 4.51:1 | ✅ |
| Botão de erro | #ffffff | #c62828 | 5.13:1 | ✅ |

---

### 2. Teste de Zoom

**Requisito WCAG:** Conteúdo deve ser utilizável com zoom 200%

**Passos:**
1. Abra a página no navegador
2. Pressione `Ctrl + +` (ou `Cmd + +` no Mac) até 200%
3. Verifique:
   - [ ] Todo conteúdo permanece visível
   - [ ] Não há sobreposição de elementos
   - [ ] Não é necessário scroll horizontal
   - [ ] Funcionalidades continuam operáveis

---

### 3. Teste de Responsividade

**Viewports a testar:**
- 📱 Mobile: 375x667 (iPhone SE)
- 📱 Tablet: 768x1024 (iPad)
- 💻 Desktop: 1920x1080
- 🖥️ Large Desktop: 2560x1440

**Checklist:**
- [ ] Layout adaptável
- [ ] Botões facilmente clicáveis (min 44x44px)
- [ ] Texto legível sem zoom
- [ ] Navegação funcional

---

## 🔊 Testes com Leitores de Tela

### NVDA (Windows - Gratuito)

**Instalação:**
1. Baixe em [nvaccess.org](https://www.nvaccess.org/)
2. Instale e inicie NVDA
3. Abra o navegador (Chrome/Firefox)

**Comandos Básicos:**
- `Ctrl` - Pausar/Retomar fala
- `Insert + Down Arrow` - Ler tudo
- `Insert + T` - Ler título
- `Insert + F7` - Lista de links
- `H` - Próximo heading
- `Tab` - Próximo elemento focável

**Checklist de Teste:**
- [ ] Título da página é anunciado
- [ ] Headings estão em ordem lógica (h1, h2, h3)
- [ ] Links têm texto descritivo
- [ ] Imagens têm alt text apropriado
- [ ] Formulários têm labels associados
- [ ] Estados de botões são anunciados (pressed, expanded)
- [ ] Mensagens de erro são anunciadas
- [ ] Carregamento dinâmico é anunciado

---

### JAWS (Windows - Pago)

Similar ao NVDA, mas com comandos ligeiramente diferentes.

**Comandos Básicos:**
- `Insert + Down Arrow` - Ler tudo
- `Insert + F5` - Lista de formulários
- `Insert + F6` - Lista de headings
- `Insert + F7` - Lista de links

---

### VoiceOver (macOS/iOS - Nativo)

**Ativar no Mac:**
- `Cmd + F5` ou Configurações > Acessibilidade > VoiceOver

**Comandos Básicos:**
- `VO + A` - Ler tudo (VO = Ctrl + Option)
- `VO + Right/Left Arrow` - Navegar elementos
- `VO + Space` - Ativar elemento
- `VO + U` - Rotor (lista de elementos)

**No iOS:**
- Configurações > Acessibilidade > VoiceOver
- Deslizar com 1 dedo - Navegar
- Duplo toque - Ativar
- Rotor com 2 dedos - Ajustar configurações

---

## ⌨️ Testes de Navegação por Teclado

### Teclas Padrão

| Tecla | Função | Deve funcionar em |
|-------|--------|-------------------|
| `Tab` | Avançar foco | Todos os elementos interativos |
| `Shift + Tab` | Voltar foco | Todos os elementos interativos |
| `Enter` | Ativar | Links, botões, formulários |
| `Space` | Ativar | Botões, checkboxes |
| `Escape` | Fechar | Modais, dropdowns |
| `Arrow Up/Down` | Navegar | Listas, selects |
| `Arrow Left/Right` | Navegar | Sliders, tabs |
| `Home` | Início | Listas |
| `End` | Fim | Listas |

---

### Checklist de Teste

**Navegação Geral:**
- [ ] Todos os elementos interativos são alcançáveis com Tab
- [ ] Ordem de tabulação é lógica e previsível
- [ ] Foco é visível em todos os elementos
- [ ] Não há armadilhas de teclado (keyboard traps)

**Modais:**
- [ ] Foco vai para o modal ao abrir
- [ ] Tab navega apenas dentro do modal
- [ ] Escape fecha o modal
- [ ] Foco retorna ao elemento que abriu o modal

**Formulários:**
- [ ] Tab navega entre campos
- [ ] Enter submete formulário
- [ ] Validação é acessível por teclado
- [ ] Mensagens de erro são anunciadas

**Listas e Tabelas:**
- [ ] Arrow keys navegam entre itens
- [ ] Enter/Space ativa item selecionado
- [ ] Home/End vão para primeiro/último item

---

## 📊 Relatórios de Auditoria

### Estrutura do Relatório

```
a11y-reports/
├── summary.html          # Relatório visual consolidado
├── summary.json          # Dados estruturados
├── Home.json            # Detalhes página Home
├── Dashboard.json       # Detalhes página Dashboard
├── Patients-List.json   # Detalhes página Patients
└── lighthouse-a11y.html # Relatório Lighthouse
```

### Interpretação de Violações

**Severidade:**
- 🔴 **Critical** - Bloqueia totalmente usuários
- 🟠 **Serious** - Dificulta significativamente
- 🟡 **Moderate** - Causa inconveniência
- 🟢 **Minor** - Pequeno impacto

**Ações por Severidade:**
- **Critical/Serious:** Correção obrigatória antes do merge
- **Moderate:** Correção recomendada
- **Minor:** Correção opcional, considere no backlog

---

## 📋 Protocolo de Teste Completo

### Antes de cada Release

1. **Testes Automatizados** (30 min)
   ```bash
   npm run audit:axe
   npm run audit:lighthouse
   npm run test:a11y
   ```

2. **Teste de Teclado** (15 min)
   - Navegar todas as páginas principais usando apenas teclado
   - Verificar foco visível
   - Testar modais e formulários

3. **Teste com Leitor de Tela** (30 min)
   - NVDA ou VoiceOver
   - Testar fluxos principais (login, cadastro, busca)
   - Verificar anúncios de mudanças dinâmicas

4. **Teste de Contraste** (15 min)
   - Color Contrast Analyzer em componentes principais
   - Verificar modo claro e escuro

5. **Teste de Zoom** (10 min)
   - Testar 200% zoom em páginas principais
   - Verificar layout e funcionalidade

**Tempo Total:** ~1h40min

---

## ✅ Critérios de Aceitação

### Para Aprovar um PR

- [ ] `npm run audit:axe` sem violações críticas/sérias
- [ ] Score Lighthouse >= 95
- [ ] Navegável 100% por teclado
- [ ] Foco visível em todos elementos interativos
- [ ] Testado com leitor de tela (pelo menos uma ferramenta)
- [ ] Contraste de cores >= 4.5:1
- [ ] Funciona com zoom 200%

### Conformidade WCAG 2.1 AA

O sistema deve atender todos os **50 critérios** do WCAG 2.1 Level AA.

**Status Atual:** 🟡 Em Progresso

**Principais Critérios:**
- ✅ 1.4.3 Contraste (Mínimo)
- ✅ 2.1.1 Teclado
- ✅ 2.1.2 Sem Armadilha de Teclado
- ✅ 2.4.3 Ordem de Foco
- ✅ 2.4.7 Foco Visível
- ✅ 3.2.4 Identificação Consistente
- ✅ 4.1.2 Nome, Função, Valor

---

## 🆘 Solução de Problemas

### Auditoria axe falha ao iniciar

**Erro:** `Cannot connect to http://localhost:4200`

**Solução:**
```bash
# Certifique-se de que o frontend está rodando
cd frontend/medicwarehouse-app
npm start

# Em outro terminal
npm run audit:axe
```

---

### Puppeteer não instalado

**Erro:** `Puppeteer not found`

**Solução:**
```bash
npm install
# ou forçar instalação do Puppeteer
npx puppeteer browsers install chrome
```

---

### NVDA não anuncia mudanças dinâmicas

**Verificar:**
1. Elemento tem `aria-live="polite"` ou `aria-live="assertive"`
2. Conteúdo está sendo atualizado via JavaScript
3. ScreenReaderService está sendo usado corretamente

---

## 📚 Recursos Adicionais

- [WCAG 2.1 Checklist](https://www.wuhcag.com/wcag-checklist/)
- [WebAIM Keyboard Testing](https://webaim.org/articles/keyboard/)
- [NVDA User Guide](https://www.nvaccess.org/files/nvda/documentation/userGuide.html)
- [VoiceOver Getting Started](https://support.apple.com/guide/voiceover/welcome/mac)

---

**Dúvidas?** Consulte o [Guia de Acessibilidade](./ACCESSIBILITY_GUIDE.md) ou abra uma issue.
