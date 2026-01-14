# Portal do Paciente - Frontend

Este é o frontend do Portal do Paciente, construído com Angular 20.

## 📋 Funcionalidades Implementadas

### Páginas
- **Login** - Autenticação de pacientes via email ou CPF
- **Registro** - Cadastro de novos pacientes
- **Dashboard** - Visão geral com estatísticas e acessos rápidos
- **Consultas** - Listagem e visualização de agendamentos
- **Documentos** - Visualização e download de documentos médicos
- **Perfil** - Gerenciamento de dados pessoais e alteração de senha

### Serviços
- **AuthService** - Gerenciamento de autenticação (login, registro, logout, refresh token)
- **AppointmentService** - Integração com API de agendamentos
- **DocumentService** - Integração com API de documentos

### Funcionalidades Técnicas
- **Auth Guard** - Proteção de rotas autenticadas
- **HTTP Interceptor** - Injeção automática de tokens JWT
- **Lazy Loading** - Carregamento otimizado de rotas
- **Material Design** - Interface moderna e responsiva

## 🚀 Como Executar

### Pré-requisitos
- Node.js 18+
- npm 9+

### Instalação

```bash
# Instalar dependências
npm install
```

### Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm start

# O app estará disponível em http://localhost:4200/
```

### Build de Produção

```bash
# Build para produção
npm run build

# Os arquivos estarão em dist/patient-portal/
```

### Testes

```bash
# Executar testes unitários (Karma/Jasmine)
npm test

# Executar testes E2E (Playwright)
npm run e2e

# E2E com UI interativa
npm run e2e:ui

# E2E em modo headed (visível)
npm run e2e:headed

# E2E em browser específico
npm run e2e -- --project chromium
npm run e2e -- --project firefox
npm run e2e -- --project webkit
```

**Testes Implementados:**

#### Unit Tests (Karma/Jasmine)
- Testes de componentes
- Testes de serviços
- Testes de guards e interceptors

#### E2E Tests (Playwright) - 30+ testes
- ✅ **auth.spec.ts** (7 testes) - Autenticação completa
  - Login, registro, validação de formulários
- ✅ **dashboard.spec.ts** (6 testes) - Navegação e dashboard
  - Rotas, welcome message, logout
- ✅ **appointments.spec.ts** (5 testes) - Gestão de consultas
  - Listagem, filtros, visualização
- ✅ **documents.spec.ts** (6 testes) - Documentos
  - Listagem, busca, download
- ✅ **profile.spec.ts** (6 testes) - Perfil do usuário
  - Edição de dados, alteração de senha

**Browsers Testados:**
- ✅ Chromium (Desktop)
- ✅ Firefox (Desktop)
- ✅ WebKit/Safari (Desktop)
- ✅ Mobile Chrome (Pixel 5)
- ✅ Mobile Safari (iPhone 12)

### Docker

```bash
# Build da imagem Docker
docker build -t patient-portal-frontend .

# Executar container
docker run -p 8080:8080 patient-portal-frontend

# Acessar em http://localhost:8080
```

## 🔧 Configuração

### Ambiente de Desenvolvimento
Edite `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // URL da API backend
};
```

### Ambiente de Produção
Edite `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: '/api'  // URL relativa para produção
};
```

## 📁 Estrutura do Projeto

```
src/
├── app/
│   ├── pages/                 # Páginas da aplicação
│   │   ├── auth/              # Login, registro
│   │   ├── dashboard/         # Dashboard
│   │   ├── appointments/      # Gestão de consultas
│   │   ├── documents/         # Visualização de documentos
│   │   └── profile/           # Perfil do usuário
│   ├── services/              # Serviços Angular
│   │   ├── auth.service.ts
│   │   ├── appointment.service.ts
│   │   └── document.service.ts
│   ├── guards/                # Guards de autenticação
│   │   └── auth.guard.ts
│   ├── interceptors/          # HTTP interceptors
│   │   └── auth.interceptor.ts
│   └── models/                # Modelos TypeScript
│       ├── auth.model.ts
│       ├── appointment.model.ts
│       └── document.model.ts
└── environments/              # Configurações de ambiente
```

## 🎨 Design System

O projeto utiliza Angular Material com customizações:
- Paleta de cores: Gradiente roxo/azul (#667eea → #764ba2)
- Typography: Inter font family
- Mobile-first design
- Componentes reutilizáveis

## 🔐 Segurança

- Tokens JWT armazenados em localStorage
- Refresh tokens automáticos via interceptor
- Auth guard protegendo rotas
- HTTPS obrigatório em produção
- Security headers configurados no nginx (Docker)
- CSP (Content Security Policy) habilitado

## 🔄 CI/CD

O frontend está integrado ao pipeline de CI/CD do Patient Portal.

**Workflow:** `.github/workflows/patient-portal-ci.yml`

**Processos Automatizados:**
- ✅ Testes unitários em cada commit
- ✅ Testes E2E em múltiplos browsers
- ✅ Build Docker otimizado
- ✅ Deploy automático para staging (develop branch)
- ✅ Deploy automático para production (main branch)

**Docker Configuration:**
- Multi-stage build (Node.js → nginx)
- Tamanho otimizado (~50MB)
- nginx com security headers
- Health check configurado
- Non-root user para segurança

Veja [CI_CD_GUIDE.md](../../patient-portal-api/CI_CD_GUIDE.md) para mais detalhes.

## 📚 Documentação Adicional

Para mais informações sobre o projeto completo:
- **[TESTING_GUIDE.md](./TESTING_GUIDE.md)** - ✨ **NOVO: Guia completo de testes**
- [PATIENT_PORTAL_GUIDE.md](../../docs/PATIENT_PORTAL_GUIDE.md) - Guia geral
- [README.md](../../patient-portal-api/README.md) - Documentação do backend
- [ARCHITECTURE.md](../../patient-portal-api/ARCHITECTURE.md) - Arquitetura técnica

## 🧪 Testes e Qualidade

### Métricas de Qualidade (Janeiro 2026)
```
✅ Testes Unitários: 52/52 passando (100%)
✅ Code Coverage: 98.66%
  - Statements: 74/75 (98.66%)
  - Branches: 13/14 (92.85%)
  - Functions: 33/33 (100%)
  - Lines: 73/74 (98.64%)
```

### Testes Implementados
- **AuthService:** 18 testes - Login, registro, refresh token, logout
- **AppointmentService:** 12 testes - Listagem, filtros, paginação
- **DocumentService:** 12 testes - Listagem, download, tipos
- **ProfileService:** 9 testes - Visualização e atualização de perfil
- **App Component:** 1 teste - Criação do app

### Executar Testes
```bash
# Todos os testes
npm test

# Com coverage report
npm test -- --code-coverage

# Headless (CI)
npm test -- --browsers=ChromeHeadless --watch=false
```

Veja o [TESTING_GUIDE.md](./TESTING_GUIDE.md) para guia completo de testes.

---

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 20.3.13.
