# 08 - Cenários de Testes de Acessibilidade

> **Módulo:** Acessibilidade WCAG 2.1 AA  
> **Tempo estimado:** 30 minutos

## 🎯 Objetivo

Validar conformidade com WCAG 2.1 AA:
- ✅ Navegação por teclado
- ✅ Leitores de tela (NVDA, JAWS)
- ✅ Contraste de cores
- ✅ Textos alternativos
- ✅ Formulários acessíveis

## 📝 Casos de Teste

### CT-ACCESS-001: Navegação por Teclado
**Passos:** Use apenas Tab, Enter, Esc para navegar
**Esperado:** 
- ✅ Todos os elementos focáveis
- ✅ Ordem lógica de foco
- ✅ Foco visível (outline)
- ✅ Possível usar sistema completo

**Prioridade:** 🔴 Crítica

---

### CT-ACCESS-002: Testar com Leitor de Tela (NVDA)
**Passos:** 
1. Instale NVDA (gratuito)
2. Ative o leitor
3. Navegue pelo sistema

**Esperado:**
- ✅ Todos os textos são lidos
- ✅ Botões têm rótulos descritivos
- ✅ Imagens têm alt text
- ✅ Formulários têm labels associados

**Prioridade:** 🔴 Crítica

---

### CT-ACCESS-003: Verificar Contraste de Cores
**Passos:** Use extensão "WCAG Color Contrast Checker"
**Esperado:** Razão de contraste mínima 4.5:1 para textos

**Prioridade:** 🟡 Média

---

### CT-ACCESS-004: Zoom 200%
**Passos:** Pressione Ctrl/Cmd + várias vezes até 200%
**Esperado:**
- ✅ Layout não quebra
- ✅ Textos legíveis
- ✅ Sem sobreposição de elementos

**Prioridade:** 🟡 Média

---

### CT-ACCESS-005: Formulários Acessíveis
**Passos:** Use Tab para navegar em formulário de login
**Esperado:**
- ✅ Labels associados a inputs
- ✅ Mensagens de erro anunciadas
- ✅ Placeholders não são único indicador

**Prioridade:** 🔴 Crítica

---

### CT-ACCESS-006: Títulos de Página (Headings)
**Passos:** Use extensão "HeadingsMap"
**Esperado:**
- ✅ Estrutura hierárquica correta (H1 > H2 > H3)
- ✅ H1 único por página
- ✅ Headings descritivos

**Prioridade:** 🟡 Média

---

### CT-ACCESS-007: Landmarks ARIA
**Passos:** Inspecione elementos no DevTools
**Esperado:**
- ✅ role="main" no conteúdo principal
- ✅ role="navigation" no menu
- ✅ role="complementary" em sidebars

**Prioridade:** 🟡 Média

---

### CT-ACCESS-008: Botões e Links Descritivos
**Passos:** Verifique textos de botões/links
**Esperado:**
- ✅ Evitar "Clique aqui"
- ✅ Textos descritivos: "Agendar consulta"
- ✅ aria-label quando necessário

**Prioridade:** 🟡 Média

---

### CT-ACCESS-009: Testar com VoiceOver (macOS/iOS)
**Passos:** Ative VoiceOver e navegue
**Esperado:** Comportamento similar ao NVDA

**Prioridade:** 🟡 Média

---

### CT-ACCESS-010: Lighthouse Accessibility Score
**Passos:** 
1. DevTools > Lighthouse
2. Selecione "Accessibility"
3. Generate report

**Esperado:** Score 90+ (mínimo)

**Prioridade:** 🔴 Crítica

---

### CT-ACCESS-011: Tabelas Acessíveis
**Passos:** Verifique tabelas de dados
**Esperado:**
- ✅ <th> para cabeçalhos
- ✅ scope="col" ou scope="row"
- ✅ <caption> presente

**Prioridade:** 🟡 Média

---

### CT-ACCESS-012: Modais e Dialogs
**Passos:** Abra modal, navegue por Tab
**Esperado:**
- ✅ Foco capturado dentro do modal
- ✅ Esc fecha o modal
- ✅ Foco retorna ao elemento que abriu

**Prioridade:** 🔴 Crítica

---

### CT-ACCESS-013: Vídeos e Mídias
**Passos:** Se houver vídeos, verifique
**Esperado:**
- ✅ Legendas (closed captions)
- ✅ Transcrição disponível
- ✅ Controles acessíveis

**Prioridade:** 🟡 Média (se aplicável)

---

### CT-ACCESS-014: Timeouts e Sessões
**Passos:** Verifique avisos de sessão expirando
**Esperado:**
- ✅ Aviso com antecedência (2 minutos)
- ✅ Possível estender sessão
- ✅ Anunciado para leitores de tela

**Prioridade:** 🟡 Média

---

### CT-ACCESS-015: Modo Alto Contraste
**Passos:** Ative modo alto contraste do Windows
**Esperado:**
- ✅ Interface ainda utilizável
- ✅ Ícones visíveis
- ✅ Bordas de elementos visíveis

**Prioridade:** 🟢 Baixa

---

## 🛠️ Ferramentas Recomendadas

### Leitores de Tela
- **NVDA** (Windows, gratuito): https://www.nvaccess.org/
- **JAWS** (Windows, pago): https://www.freedomscientific.com/
- **VoiceOver** (macOS/iOS, nativo)
- **TalkBack** (Android, nativo)

### Extensões de Navegador
- **axe DevTools** - Teste automatizado
- **WAVE** - Análise visual de acessibilidade
- **Color Contrast Checker** - Verificar contraste
- **HeadingsMap** - Visualizar estrutura de headings
- **Accessibility Insights** (Microsoft)

### Ferramentas Online
- **WebAIM Contrast Checker**: https://webaim.org/resources/contrastchecker/
- **WAVE**: https://wave.webaim.org/

## ✅ Critérios de Aceite

### Navegação
- [ ] 100% navegável por teclado
- [ ] Ordem de foco lógica
- [ ] Foco sempre visível

### Leitores de Tela
- [ ] Todos os elementos anunciados
- [ ] Textos alternativos presentes
- [ ] Formulários totalmente acessíveis

### Visual
- [ ] Contraste mínimo 4.5:1
- [ ] Zoom 200% funcional
- [ ] Alto contraste compatível

### Estrutura
- [ ] Headings hierárquicos
- [ ] Landmarks ARIA corretos
- [ ] Semântica HTML correta

### Score
- [ ] Lighthouse Accessibility: 90+
- [ ] axe DevTools: 0 erros críticos
- [ ] WAVE: Mínimo de alertas

## 📚 Documentação Relacionada

- [Accessibility Guide](../../ACCESSIBILITY_GUIDE.md)
- [Accessibility Testing Guide](../../ACCESSIBILITY_TESTING_GUIDE.md)
- [WCAG Compliance Statement](../../WCAG_COMPLIANCE_STATEMENT.md)
- [Accessibility Implementation Summary](../../ACCESSIBILITY_IMPLEMENTATION_SUMMARY.md)

## 📋 Checklist de Conformidade WCAG 2.1 AA

### Nível A (Obrigatório)
- [ ] 1.1.1 - Conteúdo não textual tem alternativa
- [ ] 1.3.1 - Informação e relações preservadas
- [ ] 2.1.1 - Teclado: Toda funcionalidade acessível
- [ ] 2.1.2 - Sem armadilha de teclado
- [ ] 2.4.1 - Bypass blocks (skip links)
- [ ] 3.1.1 - Idioma da página definido
- [ ] 4.1.1 - Parsing: HTML válido
- [ ] 4.1.2 - Nome, role e valor disponíveis

### Nível AA (Objetivo)
- [ ] 1.4.3 - Contraste mínimo 4.5:1
- [ ] 1.4.5 - Imagens de texto evitadas
- [ ] 2.4.6 - Headings e labels descritivos
- [ ] 2.4.7 - Foco visível
- [ ] 3.2.3 - Navegação consistente
- [ ] 3.2.4 - Identificação consistente
- [ ] 3.3.3 - Sugestões de erro
- [ ] 3.3.4 - Prevenção de erros

## 🏆 Meta de Conformidade

**Objetivo:** 100% de conformidade WCAG 2.1 AA

**Status Atual:** ~95% (verificar com testes)

## ⏭️ Conclusão

Após completar todos os 8 módulos de teste:
1. ✅ Autenticação
2. ✅ Agendamento
3. ✅ Prontuário
4. ✅ LGPD
5. ✅ Portal do Paciente
6. ✅ CRM
7. ✅ Analytics
8. ✅ Acessibilidade

**Sistema completamente testado! 🎉**

---

**Encontrou problemas de acessibilidade?** Priorize correções:
- 🔴 Crítico: Impede uso (ex: não navegável por teclado)
- 🟡 Importante: Dificulta uso (ex: contraste baixo)
- 🟢 Desejável: Melhoria (ex: landmarks faltando)
