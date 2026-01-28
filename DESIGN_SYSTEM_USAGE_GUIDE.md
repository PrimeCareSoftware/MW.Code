# Guia de Uso - Design System Atualizado (PROMPT 3)

> **Criado:** 28 de Janeiro de 2026  
> **Versão:** 1.0  
> **Para:** Desenvolvedores Frontend  
> **Documentação Completa:** [PROMPT3_IMPLEMENTATION_STATUS.md](./PROMPT3_IMPLEMENTATION_STATUS.md)

---

## 🚀 Quick Start

Este guia mostra como usar os novos componentes de Design System implementados no PROMPT 3.

---

## 📦 1. Micro-interações (Automáticas)

### Cards com Hover
As micro-interações em cards são **aplicadas automaticamente**. Basta usar as classes padrão:

```html
<!-- Qualquer card terá hover automático -->
<mat-card>
  <mat-card-content>
    Conteúdo do card
  </mat-card-content>
</mat-card>

<!-- Ou com classe personalizada -->
<div class="card">
  Conteúdo do card
</div>
```

### Inputs com Validação Visual
Os inputs já têm animação de shake e cores automáticas para validação:

```html
<!-- Angular Forms - Validação automática -->
<mat-form-field>
  <input matInput formControlName="nome" required>
  <mat-error *ngIf="form.get('nome')?.hasError('required')">
    Campo obrigatório
  </mat-error>
</mat-form-field>

<!-- Com classe de erro manual -->
<input type="text" class="error" placeholder="Campo com erro">
```

### Toast Notifications
Use o Angular Material Snackbar ou classes personalizadas:

```typescript
// No seu componente TypeScript
constructor(private snackBar: MatSnackBar) {}

showSuccess() {
  this.snackBar.open('Salvo com sucesso!', 'Fechar', {
    duration: 3000,
    panelClass: ['toast-success']
  });
}

showError() {
  this.snackBar.open('Erro ao salvar', 'Tentar Novamente', {
    duration: 5000,
    panelClass: ['toast-error']
  });
}
```

```html
<!-- Ou HTML personalizado -->
<div class="toast toast-success">
  ✅ Operação concluída com sucesso!
</div>

<div class="toast toast-error">
  ❌ Erro ao processar sua solicitação
</div>
```

---

## ⏳ 2. Loading States

### Skeleton - Lista de Pacientes

```html
<!-- Enquanto carrega -->
<div class="skeleton-patient-list" *ngIf="loading">
  <div class="skeleton-patient-item" *ngFor="let i of [1,2,3,4,5]">
    <div class="skeleton skeleton-avatar"></div>
    <div class="skeleton-info">
      <div class="skeleton skeleton-name"></div>
      <div class="skeleton skeleton-details"></div>
    </div>
    <div class="skeleton skeleton-status"></div>
  </div>
</div>

<!-- Dados carregados -->
<div class="patient-list" *ngIf="!loading">
  <div class="patient-item" *ngFor="let patient of patients">
    <!-- Conteúdo real -->
  </div>
</div>
```

### Skeleton - Dashboard

```html
<div class="skeleton-dashboard" *ngIf="loading">
  <div class="skeleton-stat-card" *ngFor="let i of [1,2,3,4]">
    <div class="skeleton skeleton-icon"></div>
    <div class="skeleton skeleton-value"></div>
    <div class="skeleton skeleton-label"></div>
  </div>
</div>

<div class="dashboard" *ngIf="!loading">
  <!-- Cards reais do dashboard -->
</div>
```

### Skeleton - Calendário

```html
<div class="skeleton-calendar" *ngIf="loading">
  <div class="skeleton-header">
    <div class="skeleton skeleton-title"></div>
    <div class="skeleton-controls">
      <div class="skeleton skeleton-button"></div>
      <div class="skeleton skeleton-button"></div>
    </div>
  </div>
  <div class="skeleton-grid">
    <div class="skeleton skeleton-day" *ngFor="let i of [1,2,3,4,5,6,7,8,9,10,11,12,13,14]"></div>
  </div>
</div>
```

### Skeleton - Formulário Complexo

```html
<div class="skeleton-form" *ngIf="loading">
  <div class="skeleton-form-group" *ngFor="let i of [1,2,3,4]">
    <div class="skeleton skeleton-label"></div>
    <div class="skeleton skeleton-input"></div>
  </div>
  <div class="skeleton-form-actions">
    <div class="skeleton skeleton-button"></div>
    <div class="skeleton skeleton-button"></div>
  </div>
</div>
```

### Spinners em 3 Tamanhos

```html
<!-- Pequeno - para botões -->
<button mat-raised-button [disabled]="loading">
  <div class="spinner spinner-small" *ngIf="loading"></div>
  <span *ngIf="!loading">Salvar</span>
</button>

<!-- Médio - para seções -->
<div *ngIf="loading" style="text-align: center; padding: 2rem;">
  <div class="spinner spinner-medium"></div>
</div>

<!-- Grande - para página inteira -->
<div *ngIf="loading" class="loading-overlay">
  <div class="spinner spinner-large"></div>
</div>
```

---

## 🗂️ 3. Empty States

### Template Básico

```html
<div class="empty-state" *ngIf="items.length === 0 && !loading">
  <div class="empty-icon">📦</div>
  <h3>Título do Empty State</h3>
  <p>Descrição curta e amigável explicando a situação.</p>
  <div class="empty-action">
    <button mat-raised-button color="primary" (click)="acao()">
      <mat-icon>add</mat-icon>
      Ação Principal
    </button>
    <a routerLink="/ajuda" class="link-secondary">
      Precisa de ajuda?
    </a>
  </div>
</div>
```

### Exemplo 1: Nenhum Paciente

```html
<div class="empty-state" *ngIf="patients.length === 0 && !loading">
  <div class="empty-icon">👥</div>
  <h3>Nenhum paciente cadastrado</h3>
  <p>Adicione seu primeiro paciente para começar a usar o sistema. É rápido e fácil!</p>
  <div class="empty-action">
    <button mat-raised-button color="primary" (click)="openAddPatientDialog()">
      <mat-icon>add</mat-icon>
      Adicionar Primeiro Paciente
    </button>
    <a routerLink="/help/patients" class="link-secondary">
      Como adicionar pacientes?
    </a>
  </div>
</div>
```

### Exemplo 2: Agenda Vazia

```html
<div class="empty-state" *ngIf="appointments.length === 0 && !loading">
  <div class="empty-icon">📅</div>
  <h3>Nenhuma consulta agendada</h3>
  <p>Sua agenda está livre. Que tal agendar a primeira consulta?</p>
  <div class="empty-action">
    <button mat-raised-button color="primary" (click)="scheduleAppointment()">
      <mat-icon>event</mat-icon>
      Agendar Consulta
    </button>
    <a routerLink="/help/scheduling" class="link-secondary">
      Como funciona o agendamento?
    </a>
  </div>
</div>
```

### Exemplo 3: Sem Notificações

```html
<div class="empty-state" *ngIf="notifications.length === 0">
  <div class="empty-icon">🔔</div>
  <h3>Você está em dia!</h3>
  <p>Não há notificações pendentes no momento.</p>
</div>
```

### Exemplo 4: Busca Sem Resultados

```html
<div class="empty-state" *ngIf="searchResults.length === 0 && searchQuery">
  <div class="empty-icon">🔍</div>
  <h3>Nenhum resultado encontrado</h3>
  <p>Tente ajustar os filtros ou usar termos de busca diferentes.</p>
  <div class="empty-action">
    <button mat-raised-button (click)="clearFilters()">
      <mat-icon>clear</mat-icon>
      Limpar Filtros
    </button>
  </div>
</div>
```

---

## ❌ 4. Error Messages

### Mensagem de Erro Completa

```html
<div class="error-message" *ngIf="error">
  <div class="error-icon">⚠️</div>
  <div class="error-content">
    <div class="error-title">Ops! Algo deu errado</div>
    <div class="error-description">
      {{ error.message || 'Não foi possível completar a operação. Tente novamente.' }}
    </div>
    <div class="error-actions">
      <button class="btn-retry" (click)="retry()">
        Tentar Novamente
      </button>
      <button class="btn-dismiss" (click)="dismissError()">
        Fechar
      </button>
    </div>
  </div>
  <button class="error-close" (click)="dismissError()">✕</button>
</div>
```

### Erro de Campo (Inline)

```html
<mat-form-field>
  <input matInput formControlName="email" required email>
  <mat-error *ngIf="form.get('email')?.hasError('required')">
    <div class="field-error">
      O e-mail é obrigatório
    </div>
  </mat-error>
  <mat-error *ngIf="form.get('email')?.hasError('email')">
    <div class="field-error">
      Digite um e-mail válido
    </div>
  </mat-error>
</mat-form-field>
```

### Erro de Conexão

```html
<div class="network-error" *ngIf="!isOnline">
  <div class="network-icon">📡</div>
  <h3>Sem conexão com a internet</h3>
  <p>Verifique sua conexão e tente novamente.</p>
  <button class="btn-retry-network" (click)="checkConnection()">
    <mat-icon>refresh</mat-icon>
    Tentar Novamente
  </button>
</div>
```

### Banner de Erro Global

```html
<!-- No app.component.html ou layout principal -->
<div class="error-banner" *ngIf="globalError">
  <div class="banner-content">
    <div class="banner-icon">⚠️</div>
    <div class="banner-text">
      <div class="banner-title">{{ globalError.title }}</div>
      <div class="banner-description">{{ globalError.description }}</div>
    </div>
  </div>
  <div class="banner-actions">
    <button class="btn-banner" (click)="retryGlobalAction()">
      Tentar Novamente
    </button>
    <button class="btn-banner" (click)="dismissGlobalError()">
      Fechar
    </button>
  </div>
</div>
```

### TypeScript - Gerenciamento de Erros

```typescript
// error-handler.service.ts
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  
  constructor(private snackBar: MatSnackBar) {}
  
  // Mapeia erros técnicos para mensagens humanizadas
  getHumanizedError(error: any): string {
    const errorMap: { [key: string]: string } = {
      'Bad Request': 'Ops! Alguns campos precisam de atenção. Verifique e tente novamente.',
      'Network error': 'Sem conexão com a internet. Verifique sua rede e tente novamente.',
      'Internal Server Error': 'Algo deu errado no servidor. Nossa equipe já foi notificada.',
      'Forbidden': 'Você não tem permissão para realizar esta ação.',
      'Unauthorized': 'Sua sessão expirou. Faça login novamente.',
      'Not Found': 'O recurso solicitado não foi encontrado.',
      'Timeout': 'A operação está demorando mais que o esperado.'
    };
    
    const errorType = error?.error?.message || error?.message || 'Unknown error';
    
    return errorMap[errorType] || 'Ocorreu um erro inesperado. Tente novamente.';
  }
  
  // Mostra erro como toast
  showError(error: any) {
    const message = this.getHumanizedError(error);
    this.snackBar.open(message, 'Fechar', {
      duration: 5000,
      panelClass: ['toast-error']
    });
  }
  
  // Mostra sucesso como toast
  showSuccess(message: string) {
    this.snackBar.open(message, 'OK', {
      duration: 3000,
      panelClass: ['toast-success']
    });
  }
}
```

**Uso no componente:**

```typescript
import { ErrorHandlerService } from './services/error-handler.service';

export class PatientListComponent {
  patients: Patient[] = [];
  loading = false;
  error: any = null;
  
  constructor(
    private patientService: PatientService,
    private errorHandler: ErrorHandlerService
  ) {}
  
  loadPatients() {
    this.loading = true;
    this.error = null;
    
    this.patientService.getPatients().subscribe({
      next: (data) => {
        this.patients = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err;
        this.loading = false;
        this.errorHandler.showError(err);
      }
    });
  }
  
  retry() {
    this.loadPatients();
  }
}
```

---

## 🎨 5. Customização

### Cores Personalizadas

Você pode personalizar as cores usando variáveis CSS:

```scss
// No seu arquivo .scss
.meu-componente {
  .empty-state {
    .empty-icon {
      color: var(--primary-600); // Usar cor primária
    }
  }
  
  .error-message {
    background-color: var(--error-50);
    border-color: var(--error-200);
    color: var(--error-700);
  }
}
```

### Animações

Você pode desabilitar animações para usuários que preferem:

```scss
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

---

## 📱 6. Responsividade

Todos os componentes são responsivos por padrão. Em mobile:

- Cards reduzem padding
- Skeletons ajustam tamanho
- Empty states centralizam conteúdo
- Mensagens de erro empilham ações verticalmente

Exemplo de ajuste mobile:

```scss
@media (max-width: 768px) {
  .empty-state {
    padding: var(--spacing-8);
    
    p {
      font-size: var(--font-size-sm);
    }
    
    .empty-action {
      flex-direction: column;
      width: 100%;
      
      button {
        width: 100%;
      }
    }
  }
}
```

---

## ✅ Checklist de Implementação

Ao adicionar uma nova tela/componente, verifique:

- [ ] Loading state implementado (skeleton ou spinner)
- [ ] Empty state implementado para listas vazias
- [ ] Mensagens de erro humanizadas
- [ ] Toast notifications para feedback de ações
- [ ] Validação visual em formulários
- [ ] Micro-interações em cards e botões
- [ ] Animações suaves em transições

---

## 🔗 Links Úteis

- [Documentação Completa (PROMPT3_IMPLEMENTATION_STATUS.md)](./PROMPT3_IMPLEMENTATION_STATUS.md)
- [Plano de Melhorias (PLANO_MELHORIAS_WEBSITE_UXUI.md)](./PLANO_MELHORIAS_WEBSITE_UXUI.md)
- [Todos os Prompts (PROMPTS_IMPLEMENTACAO_DETALHADOS.md)](./PROMPTS_IMPLEMENTACAO_DETALHADOS.md)

---

## 🆘 Suporte

Se tiver dúvidas ou problemas:

1. Verifique a documentação completa em [PROMPT3_IMPLEMENTATION_STATUS.md](./PROMPT3_IMPLEMENTATION_STATUS.md)
2. Veja exemplos reais no código existente
3. Consulte o guia de estilos em `/frontend/medicwarehouse-app/src/styles.scss`

---

**Guia criado por:** GitHub Copilot Agent  
**Data:** 28 de Janeiro de 2026  
**Versão:** 1.0
