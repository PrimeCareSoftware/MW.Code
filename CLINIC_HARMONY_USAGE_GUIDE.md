# Guia de Uso - Sistema de Design Clinic Harmony

## Introdução

Este guia fornece instruções práticas para utilizar o sistema de design migrado do Clinic Harmony nas aplicações Angular do MedicWarehouse.

## Importação Básica

### Em qualquer arquivo SCSS de componente:

```scss
@use '../../../shared-styles' as *;

.my-component {
  background: var(--background);
  color: var(--foreground);
  border-radius: var(--radius);
}
```

### Em arquivos de estilo principais (styles.scss):

```scss
// Já importado automaticamente
@use '../../shared-styles' as *;
```

## Cores

### Cores Primárias

```scss
// Uso básico
.btn-primary {
  background: var(--primary-500);  // #3D9DED - Soft Medical Blue
  color: white;
}

// Com HSL (para manipulação)
.overlay {
  background: hsl(var(--primary-hsl) / 0.1);  // 10% opacity
}
```

### Cores Semânticas

```scss
// Success
.success-message {
  background: var(--success-50);
  color: var(--success-700);
  border: 1px solid var(--success-200);
}

// Error
.error-message {
  background: var(--error-50);
  color: var(--error-700);
  border: 1px solid var(--error-200);
}

// Warning
.warning-message {
  background: var(--warning-50);
  color: var(--warning-700);
  border: 1px solid var(--warning-200);
}

// Info
.info-message {
  background: var(--info-50);
  color: var(--info-700);
  border: 1px solid var(--info-200);
}
```

### Cores de Acento

```scss
.accent-button {
  background: var(--accent-500);  // #14b8a6 - Subtle Teal
  color: white;
  
  &:hover {
    background: var(--accent-600);
  }
}
```

## Tipografia

### Tamanhos de Fonte

```html
<h1 class="font-size-4xl">Título Principal</h1>
<h2 class="font-size-3xl">Subtítulo</h2>
<h3 class="font-size-2xl">Seção</h3>
<p class="font-size-base">Texto normal</p>
<span class="font-size-sm">Texto pequeno</span>
<span class="font-size-xs">Texto extra pequeno</span>
```

### Pesos de Fonte

```html
<p class="font-weight-light">Leve (300)</p>
<p class="font-weight-normal">Normal (400)</p>
<p class="font-weight-medium">Médio (500)</p>
<p class="font-weight-semibold">Semi-negrito (600)</p>
<p class="font-weight-bold">Negrito (700)</p>
```

### Alturas de Linha

```html
<p class="line-height-tight">Linha apertada (1.25)</p>
<p class="line-height-normal">Linha normal (1.5)</p>
<p class="line-height-relaxed">Linha relaxada (1.75)</p>
```

## Espaçamento

### Classes de Margin

```html
<div class="m-0">Sem margem</div>
<div class="m-4">Margem de 16px em todos os lados</div>
<div class="mt-6">Margem superior de 24px</div>
<div class="mb-8">Margem inferior de 32px</div>
<div class="ml-2">Margem esquerda de 8px</div>
<div class="mr-3">Margem direita de 12px</div>
```

### Classes de Padding

```html
<div class="p-0">Sem padding</div>
<div class="p-4">Padding de 16px em todos os lados</div>
<div class="pt-6">Padding superior de 24px</div>
<div class="pb-8">Padding inferior de 32px</div>
<div class="pl-2">Padding esquerdo de 8px</div>
<div class="pr-3">Padding direito de 12px</div>
```

### Gap (para Flexbox/Grid)

```html
<div class="d-flex gap-2">Elementos com espaço de 8px</div>
<div class="d-flex gap-4">Elementos com espaço de 16px</div>
<div class="d-grid gap-6">Grid com espaço de 24px</div>
```

## Componentes Pré-estilizados

### Botões

```html
<!-- Botão Primário -->
<button class="btn btn-primary">
  Salvar
</button>

<!-- Botão Secundário -->
<button class="btn btn-secondary">
  Cancelar
</button>

<!-- Botão de Erro -->
<button class="btn btn-danger">
  Excluir
</button>

<!-- Botão de Sucesso -->
<button class="btn btn-success">
  Confirmar
</button>

<!-- Botões com Tamanhos -->
<button class="btn btn-primary btn-sm">Pequeno</button>
<button class="btn btn-primary">Normal</button>
<button class="btn btn-primary btn-lg">Grande</button>
```

### Cards

```html
<div class="card">
  <div class="card-header">
    <h3>Título do Card</h3>
  </div>
  <div class="card-body">
    <p>Conteúdo do card aqui.</p>
  </div>
  <div class="card-footer">
    <button class="btn btn-primary">Ação</button>
  </div>
</div>

<!-- Card com Hover Effect -->
<div class="card card-hover">
  Card com elevação ao passar o mouse
</div>
```

### Badges

```html
<span class="badge badge-primary">Primário</span>
<span class="badge badge-success">Sucesso</span>
<span class="badge badge-warning">Aviso</span>
<span class="badge badge-error">Erro</span>
<span class="badge badge-info">Info</span>
```

### Formulários

```html
<div class="form-group">
  <label class="form-label required">Nome</label>
  <input type="text" class="form-control" placeholder="Digite seu nome">
  <div class="form-help">Texto de ajuda</div>
</div>

<div class="form-group">
  <label class="form-label">Email</label>
  <input type="email" class="form-control" placeholder="email@exemplo.com">
  <div class="form-error">Mensagem de erro</div>
</div>
```

### Alertas

```html
<div class="alert alert-success">
  Operação realizada com sucesso!
</div>

<div class="alert alert-error">
  Erro ao processar a solicitação.
</div>

<div class="alert alert-warning">
  Atenção: Isso é um aviso.
</div>

<div class="alert alert-info">
  Informação importante.
</div>
```

## Efeitos Clinic Harmony

### Glassmorphism (Efeito Vidro)

```html
<div class="glass p-6 rounded-lg">
  Conteúdo com efeito de vidro fosco
</div>

<!-- Dark mode -->
<div class="glass-dark p-6 rounded-lg">
  Vidro fosco para modo escuro
</div>
```

### Card Hover (Elevação Suave)

```html
<div class="card card-hover">
  Card que se eleva suavemente ao passar o mouse
</div>
```

### Focus Ring (Apple-style)

```html
<input type="text" class="form-control focus-ring">
<button class="btn btn-primary focus-ring">Botão com foco estilizado</button>
```

### Transições Suaves

```html
<div class="transition-apple">
  Elemento com transição suave (200ms)
</div>
```

## Gradientes para Métricas

```html
<div class="metric-primary p-6 rounded">
  <h3>1,234</h3>
  <p>Novos pacientes</p>
</div>

<div class="metric-accent p-6 rounded">
  <h3>89%</h3>
  <p>Taxa de satisfação</p>
</div>

<div class="metric-success p-6 rounded">
  <h3>567</h3>
  <p>Consultas realizadas</p>
</div>

<div class="metric-warning p-6 rounded">
  <h3>12</h3>
  <p>Pendências</p>
</div>
```

## Animações

### Classes de Animação

```html
<div class="fade-in">
  Aparece com fade (0.3s)
</div>

<div class="scale-in">
  Aparece com escala (0.2s)
</div>

<div class="slide-in-right">
  Desliza da direita (0.3s)
</div>

<div class="pulse-soft">
  Pulsa suavemente (loop infinito)
</div>
```

## Utilidades de Layout

### Flexbox

```html
<div class="d-flex justify-center align-center gap-4">
  <div>Item 1</div>
  <div>Item 2</div>
</div>

<div class="d-flex flex-column gap-3">
  <div>Item 1</div>
  <div>Item 2</div>
</div>

<div class="d-flex justify-between align-start">
  <div>Esquerda</div>
  <div>Direita</div>
</div>
```

### Grid

```html
<div class="d-grid gap-4">
  <div>Item 1</div>
  <div>Item 2</div>
  <div>Item 3</div>
</div>
```

### Visibilidade

```html
<div class="d-none">Oculto</div>
<div class="d-block">Bloco</div>
<div class="d-flex">Flex</div>
<div class="invisible">Invisível (mantém espaço)</div>
```

## Borders e Sombras

### Border Radius

```html
<div class="rounded">Border radius padrão (12px)</div>
<div class="rounded-sm">Border radius pequeno (8px)</div>
<div class="rounded-lg">Border radius grande (20px)</div>
<div class="rounded-full">Completamente arredondado</div>
```

### Sombras

```html
<div class="shadow-sm">Sombra extra suave</div>
<div class="shadow">Sombra normal</div>
<div class="shadow-md">Sombra média</div>
<div class="shadow-lg">Sombra grande</div>
<div class="shadow-xl">Sombra extra grande</div>
```

## Dark Mode

O sistema suporta automaticamente dark mode. Basta adicionar a classe `.dark` ou `.theme-dark` no `<body>`:

```typescript
// No seu componente Angular
toggleDarkMode() {
  document.body.classList.toggle('theme-dark');
}
```

Todas as variáveis CSS são ajustadas automaticamente:

```scss
.my-component {
  background: var(--background);  // Muda automaticamente no dark mode
  color: var(--foreground);       // Muda automaticamente no dark mode
}
```

## High Contrast Mode

Para acessibilidade, há suporte a alto contraste:

```typescript
toggleHighContrast() {
  document.body.classList.add('theme-high-contrast');
}
```

## Exemplos Completos

### Card de Métrica Completo

```html
<div class="card metric-primary card-hover p-6">
  <div class="d-flex justify-between align-center mb-4">
    <h4 class="font-size-2xl font-weight-semibold">1,234</h4>
    <span class="badge badge-success">+12%</span>
  </div>
  <p class="text-secondary">Novos Pacientes</p>
  <p class="font-size-sm text-tertiary mt-2">vs. mês anterior</p>
</div>
```

### Formulário Completo

```html
<form class="card p-6">
  <h3 class="font-size-2xl font-weight-semibold mb-6">
    Cadastro de Paciente
  </h3>
  
  <div class="form-group">
    <label class="form-label required">Nome Completo</label>
    <input type="text" class="form-control focus-ring" placeholder="Digite o nome">
  </div>
  
  <div class="form-group">
    <label class="form-label required">Email</label>
    <input type="email" class="form-control focus-ring" placeholder="email@exemplo.com">
    <div class="form-help">Será usado para notificações</div>
  </div>
  
  <div class="form-group">
    <label class="form-label">Telefone</label>
    <input type="tel" class="form-control focus-ring" placeholder="(00) 00000-0000">
  </div>
  
  <div class="d-flex gap-3 justify-end mt-6">
    <button type="button" class="btn btn-secondary">
      Cancelar
    </button>
    <button type="submit" class="btn btn-primary">
      Salvar
    </button>
  </div>
</form>
```

### Dashboard com Cards

```html
<div class="d-grid gap-6" style="grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));">
  <div class="card metric-primary card-hover fade-in p-6">
    <h3 class="font-size-4xl font-weight-bold">1,234</h3>
    <p class="text-secondary mt-2">Pacientes Ativos</p>
  </div>
  
  <div class="card metric-success card-hover fade-in p-6">
    <h3 class="font-size-4xl font-weight-bold">567</h3>
    <p class="text-secondary mt-2">Consultas Este Mês</p>
  </div>
  
  <div class="card metric-accent card-hover fade-in p-6">
    <h3 class="font-size-4xl font-weight-bold">89%</h3>
    <p class="text-secondary mt-2">Taxa de Satisfação</p>
  </div>
  
  <div class="card metric-warning card-hover fade-in p-6">
    <h3 class="font-size-4xl font-weight-bold">12</h3>
    <p class="text-secondary mt-2">Pendências</p>
  </div>
</div>
```

## Dicas de Performance

1. **Use variáveis CSS** em vez de valores fixos para facilitar temas
2. **Prefira classes utilitárias** para estilos simples
3. **Use `@use` em vez de `@import`** no SCSS
4. **Evite !important** sempre que possível
5. **Use animações com moderação** para melhor performance

## Suporte

Para dúvidas:
- Consulte `CLINIC_HARMONY_MIGRATION.md` para detalhes da migração
- Veja os arquivos em `/frontend/shared-styles` para referência
- Entre em contato com a equipe de desenvolvimento Frontend
