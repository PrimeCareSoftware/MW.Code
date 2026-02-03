# 03 - Configuração do Frontend (Angular 20)

> **Objetivo:** Configurar e executar a aplicação frontend do Omni Care Software  
> **Tempo estimado:** 10-15 minutos  
> **Pré-requisitos:** [01-Configuracao-Ambiente.md](01-Configuracao-Ambiente.md) e [02-Configuracao-Backend.md](02-Configuracao-Backend.md) completos

## 📋 Índice

1. [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
2. [Instalação de Dependências](#instalação-de-dependências)
3. [Configuração](#configuração)
4. [Executar o Frontend](#executar-o-frontend)
5. [Verificação](#verificação)
6. [Próximos Passos](#próximos-passos)

## 🏗️ Visão Geral da Arquitetura

O frontend do Omni Care é construído com **Angular 20** e possui três aplicações principais:

```
frontend/                      # Aplicação principal (porta 4200)
├── src/
│   ├── app/
│   │   ├── auth/             # Módulo de autenticação
│   │   ├── patients/         # Gestão de pacientes
│   │   ├── appointments/     # Agendamento
│   │   ├── medical-records/  # Prontuários SOAP
│   │   ├── analytics/        # Dashboards BI
│   │   ├── crm/              # CRM avançado
│   │   └── shared/           # Componentes compartilhados
│   └── environments/         # Variáveis de ambiente

system-admin/                  # Admin SPA (porta 3000)
├── src/
│   ├── app/
│   │   ├── dashboard/        # Dashboard admin
│   │   ├── tenants/          # Gestão de tenants
│   │   ├── users/            # Gestão de usuários
│   │   └── settings/         # Configurações sistema

patient-portal-frontend/       # Portal do Paciente (porta 4100)
└── (integrado com frontend principal via PWA)
```

### Características Técnicas

- ✅ **Angular 20** com Standalone Components
- ✅ **TypeScript 5.4+**
- ✅ **Angular Material 20** para UI
- ✅ **RxJS 7** para programação reativa
- ✅ **NgRx** para gerenciamento de estado
- ✅ **PWA** (Progressive Web App)
- ✅ **Acessibilidade WCAG 2.1 AA**
- ✅ **171+ componentes**

## 📦 Instalação de Dependências

### 1. Frontend Principal

```bash
cd frontend
npm install
```

Isso instalará todas as dependências:
- Angular 20
- Angular Material
- NgRx Store/Effects
- RxJS
- Chart.js
- E outras bibliotecas

**Tempo estimado:** 3-5 minutos

### 2. System Admin

```bash
cd system-admin
npm install
```

### 3. Verificar Instalação

```bash
# Verificar se as dependências foram instaladas corretamente
npm list --depth=0
```

## ⚙️ Configuração

### 1. Configurar Ambiente de Desenvolvimento

#### Frontend Principal

Edite `frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  apiBaseUrl: 'http://localhost:5000',
  patientPortalApiUrl: 'http://localhost:5100/api',
  telemedicineApiUrl: 'http://localhost:5200/api',
  version: '1.0.0',
  
  // JWT
  jwtTokenKey: 'omnicare_token',
  jwtRefreshTokenKey: 'omnicare_refresh_token',
  
  // Upload
  maxFileSize: 10 * 1024 * 1024, // 10 MB
  allowedFileTypes: ['image/jpeg', 'image/png', 'application/pdf'],
  
  // Features flags
  features: {
    enablePWA: true,
    enableOfflineMode: true,
    enable2FA: true,
    enableTelemedicine: true,
    enableCRM: true,
    enableAnalytics: true,
  },
  
  // External services
  googleAnalyticsId: 'GA-XXXXXXXX',
  sentryDsn: '',
};
```

#### System Admin

Edite `system-admin/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  version: '1.0.0',
  features: {
    enableDarkMode: true,
    enableTour: true,
    enableHelpCenter: true,
  }
};
```

### 2. Configurar Proxy (Opcional, mas recomendado)

Para evitar problemas de CORS durante o desenvolvimento, configure um proxy.

Crie `frontend/proxy.conf.json`:

```json
{
  "/api": {
    "target": "http://localhost:5000",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  },
  "/patient-api": {
    "target": "http://localhost:5100",
    "secure": false,
    "changeOrigin": true,
    "pathRewrite": {
      "^/patient-api": "/api"
    }
  }
}
```

### 3. Configurar Angular CLI

O arquivo `angular.json` já está configurado, mas você pode ajustar:

```json
{
  "projects": {
    "primecare": {
      "architect": {
        "serve": {
          "options": {
            "proxyConfig": "proxy.conf.json",
            "port": 4200,
            "host": "localhost"
          }
        }
      }
    }
  }
}
```

## 🚀 Executar o Frontend

### Opção 1: Frontend Principal (Modo Desenvolvimento)

```bash
cd frontend
ng serve
```

ou com proxy:

```bash
cd frontend
ng serve --proxy-config proxy.conf.json
```

A aplicação estará disponível em: `http://localhost:4200`

### Opção 2: System Admin

```bash
cd system-admin
npm start
```

A aplicação estará disponível em: `http://localhost:3000`

### Opção 3: Modo de Produção Local

```bash
cd frontend
ng build --configuration production
ng serve --configuration production
```

### Opção 4: Com Hot Module Replacement (HMR)

Para recarregamento mais rápido:

```bash
cd frontend
ng serve --hmr
```

### Opção 5: Executar com Dados Mockados (Sem Backend)

Para testar o frontend sem o backend rodando:

```bash
cd frontend
npm run start:mock
```

Isso usa interceptors HTTP para retornar dados mockados.

## ✅ Verificação

### 1. Acessar a Aplicação

Abra o navegador em:

```
http://localhost:4200
```

Você deve ver a página de login do Omni Care Software.

### 2. Verificar Console do Navegador

Abra o DevTools (F12) e verifique o console:
- ✅ Não deve haver erros críticos
- ⚠️ Warnings de desenvolvimento são normais

### 3. Verificar Network

No DevTools, aba Network:
- ✅ Todos os assets (JS, CSS) devem carregar
- ✅ Conexão com API deve funcionar (após configurar banco de dados)

### 4. Testar Responsividade

Use o DevTools para testar em diferentes resoluções:
- 📱 Mobile (375x667)
- 📱 Tablet (768x1024)
- 💻 Desktop (1920x1080)

### 5. Testar Acessibilidade

Use o Lighthouse no Chrome DevTools:

```
DevTools > Lighthouse > Accessibility > Generate Report
```

Score esperado: 90+ (WCAG 2.1 AA)

### Checklist de Verificação

- [ ] Frontend compila sem erros
- [ ] Aplicação carrega em http://localhost:4200
- [ ] Página de login é exibida corretamente
- [ ] Não há erros críticos no console
- [ ] Assets carregam corretamente
- [ ] Responsivo funciona em mobile/tablet/desktop
- [ ] Score de acessibilidade > 90

## 🎨 Estrutura de Módulos

### Frontend Principal

```
src/app/
├── core/                   # Serviços core (singleton)
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── api.service.ts
│   │   └── notification.service.ts
│   ├── guards/
│   │   ├── auth.guard.ts
│   │   └── role.guard.ts
│   └── interceptors/
│       ├── jwt.interceptor.ts
│       └── error.interceptor.ts
│
├── shared/                 # Componentes compartilhados
│   ├── components/
│   │   ├── header/
│   │   ├── sidebar/
│   │   ├── footer/
│   │   └── dialogs/
│   ├── directives/
│   └── pipes/
│
├── features/               # Módulos de funcionalidades
│   ├── auth/              # Login, 2FA, Recuperação
│   ├── dashboard/         # Dashboard principal
│   ├── patients/          # Gestão de pacientes
│   ├── doctors/           # Gestão de médicos
│   ├── appointments/      # Agendamento
│   ├── medical-records/   # Prontuários SOAP
│   ├── prescriptions/     # Prescrições
│   ├── analytics/         # BI e Analytics
│   ├── crm/               # CRM Avançado
│   └── lgpd/              # Conformidade LGPD
│
└── store/                 # NgRx State Management
    ├── actions/
    ├── reducers/
    ├── effects/
    └── selectors/
```

### Rotas Principais

| Rota | Componente | Descrição |
|------|------------|-----------|
| `/login` | LoginComponent | Página de login |
| `/dashboard` | DashboardComponent | Dashboard principal |
| `/patients` | PatientsListComponent | Lista de pacientes |
| `/patients/:id` | PatientDetailComponent | Detalhes do paciente |
| `/appointments` | AppointmentsComponent | Agendamentos |
| `/medical-records` | MedicalRecordsComponent | Prontuários |
| `/analytics` | AnalyticsComponent | Dashboards BI |
| `/crm` | CRMComponent | CRM |

## 🔧 Comandos Úteis

### Desenvolvimento

```bash
# Executar com watch mode
ng serve --watch

# Executar testes unitários
ng test

# Executar testes com cobertura
ng test --code-coverage

# Executar linting
ng lint

# Executar formatação
npm run format

# Executar todos os checks
npm run check
```

### Build

```bash
# Build de desenvolvimento
ng build

# Build de produção
ng build --configuration production

# Build com análise de bundle
ng build --stats-json
npm run analyze
```

### Testes

```bash
# Testes unitários
ng test

# Testes E2E (Cypress)
npm run e2e

# Testes E2E em modo interativo
npm run e2e:open
```

## 🚨 Problemas Comuns

### Problema: Erro "Module not found"

**Solução:**
```bash
# Limpar node_modules e reinstalar
rm -rf node_modules package-lock.json
npm install
```

### Problema: Porta 4200 já está em uso

**Solução:**
```bash
# Usar outra porta
ng serve --port 4201

# Ou matar o processo na porta 4200
# Windows
netstat -ano | findstr :4200
taskkill /PID <PID> /F

# macOS/Linux
lsof -ti:4200 | xargs kill -9
```

### Problema: Erro de CORS

**Solução:** Use o proxy configurado:
```bash
ng serve --proxy-config proxy.conf.json
```

### Problema: Build muito lento

**Solução:**
```bash
# Use incremental builds
ng build --watch

# Ou ajuste tsconfig.json para usar cache
{
  "compilerOptions": {
    "incremental": true
  }
}
```

### Problema: Erro ao fazer login (API não conecta)

**Solução:** Verifique:
1. Backend está rodando em http://localhost:5000
2. Banco de dados está configurado
3. Proxy está configurado corretamente
4. URL da API em `environment.ts` está correta

## 🎨 Temas e Estilos

### Tema Padrão

O sistema usa Angular Material com tema customizado:

```scss
// frontend/src/styles.scss
@use '@angular/material' as mat;

$omnicare-primary: mat.define-palette(mat.$indigo-palette);
$omnicare-accent: mat.define-palette(mat.$pink-palette, A200, A100, A400);
$omnicare-warn: mat.define-palette(mat.$red-palette);

$omnicare-theme: mat.define-light-theme((
  color: (
    primary: $omnicare-primary,
    accent: $omnicare-accent,
    warn: $omnicare-warn,
  )
));

@include mat.all-component-themes($omnicare-theme);
```

### Dark Mode

O sistema suporta dark mode automático:

```typescript
// No componente
import { ThemeService } from './core/services/theme.service';

constructor(private themeService: ThemeService) {
  this.themeService.initTheme();
}

toggleTheme() {
  this.themeService.toggleTheme();
}
```

## 📚 Documentação Adicional

- [Design System Usage Guide](../../DESIGN_SYSTEM_USAGE_GUIDE.md)
- [Accessibility Guide](../../ACCESSIBILITY_GUIDE.md)
- [Frontend Integration Summary](../../FRONTEND_INTEGRATION_SUMMARY.md)
- [PWA Installation Guide](../../system-admin/guias/PWA_INSTALLATION_GUIDE.md)

## ⏭️ Próximos Passos

Agora que o frontend está configurado:

1. ✅ Frontend configurado e rodando
2. ➡️ Vá para [04-Configuracao-Banco-Dados.md](04-Configuracao-Banco-Dados.md) para configurar o banco de dados e executar migrations
3. Após isso, o sistema estará totalmente funcional para testes

---

**Dúvidas?** Acesse http://localhost:4200 e explore a interface!
