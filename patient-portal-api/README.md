# Portal do Paciente (Patient Portal) - Omni Care Software

> **🚀 NOVO DESENVOLVEDOR?** Comece aqui: [DEVELOPER_QUICKSTART.md](./DEVELOPER_QUICKSTART.md) - Setup em < 30 minutos!

> **📊 STATUS ATUAL:** 70% Completo - MVP Pronto | [Ver Status Detalhado →](../PORTAL_PACIENTE_STATUS_JAN2026.md)

---

## 📚 Documentação Completa

### 🎯 Início Rápido
- **[DEVELOPER_QUICKSTART.md](./DEVELOPER_QUICKSTART.md)** ⚡ - Setup e primeiros passos (< 30 min)
- **[TROUBLESHOOTING_FAQ.md](./TROUBLESHOOTING_FAQ.md)** 🔧 - Resolução de problemas comuns

### 📖 Documentação Principal
- **[PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md)** - Arquitetura DDD/Clean Architecture
- **[PATIENT_PORTAL_SECURITY_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_SECURITY_GUIDE.md)** - Segurança e LGPD
- **[PATIENT_PORTAL_USER_MANUAL.md](../system-admin/regras-negocio/PATIENT_PORTAL_USER_MANUAL.md)** - Manual do usuário final
- **[PATIENT_PORTAL_CI_CD_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_CI_CD_GUIDE.md)** - Pipeline CI/CD
- **[PATIENT_PORTAL_DEPLOYMENT_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_DEPLOYMENT_GUIDE.md)** - Deploy e infraestrutura
- **[PATIENT_PORTAL_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_GUIDE.md)** - Guia de implementação completo

### 🔮 Funcionalidades Pendentes (30%)
- **[BOOKING_IMPLEMENTATION_GUIDE.md](./BOOKING_IMPLEMENTATION_GUIDE.md)** 📅 - Agendamento online (3 semanas)
- **[NOTIFICATION_SERVICE_GUIDE.md](./NOTIFICATION_SERVICE_GUIDE.md)** 🔔 - Sistema de lembretes (1 semana)

### 📈 Status e Planejamento
- **[PORTAL_PACIENTE_STATUS_JAN2026.md](../PORTAL_PACIENTE_STATUS_JAN2026.md)** - Status técnico detalhado
- **[10-portal-paciente.md](../Plano_Desenvolvimento/fase-2-seguranca-lgpd/10-portal-paciente.md)** - Requisitos originais
- **[PORTAL_PACIENTE_IMPLEMENTACAO_STATUS.md](../Plano_Desenvolvimento/fase-2-seguranca-lgpd/PORTAL_PACIENTE_IMPLEMENTACAO_STATUS.md)** - Status de implementação

---

## 📋 Visão Geral

O Portal do Paciente é uma aplicação web dedicada que permite aos pacientes acessar suas informações médicas, gerenciar agendamentos e interagir com documentos de forma segura e independente.

### ✅ Funcionalidades Implementadas (70%)

- 🔐 **Autenticação segura** (JWT + Refresh Tokens)
- 📊 **Dashboard do paciente** (estatísticas e ações rápidas)
- 📅 **Visualização de agendamentos** (listar, filtrar, detalhes)
- 📄 **Gerenciamento de documentos** (listar, visualizar, download)
- 👤 **Perfil do paciente** (editar dados, alterar senha)
- 🔒 **Compliance LGPD/CFM** (auditoria, segurança)
- 🧪 **Testes automatizados** (35+ backend, 58+ frontend)

### ❌ Pendentes (30%)

- 📅 **Agendamento online** (0%) - [Ver guia →](./BOOKING_IMPLEMENTATION_GUIDE.md)
- 🔔 **Notificações automáticas** (0%) - [Ver guia →](./NOTIFICATION_SERVICE_GUIDE.md)
- 📱 **PWA completo** (30%) - Service workers e offline

## 🏗️ Arquitetura

### Backend (.NET 8)

O backend segue os princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**, organizado em camadas:

```
patient-portal-api/
├── PatientPortal.Domain/          # Camada de Domínio
│   ├── Entities/                  # Entidades do domínio
│   │   ├── PatientUser.cs         # Usuário paciente
│   │   ├── RefreshToken.cs        # Token de atualização JWT
│   │   ├── AppointmentView.cs     # Visualização de agendamentos
│   │   └── DocumentView.cs        # Visualização de documentos
│   ├── Enums/                     # Enumerações
│   │   ├── AppointmentStatus.cs
│   │   └── DocumentType.cs
│   └── Interfaces/                # Interfaces de repositórios
│
├── PatientPortal.Application/     # Camada de Aplicação
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Auth/                  # DTOs de autenticação
│   │   ├── Appointments/          # DTOs de agendamentos
│   │   └── Documents/             # DTOs de documentos
│   ├── Interfaces/                # Interfaces de serviços
│   │   ├── IAuthService.cs
│   │   ├── ITokenService.cs
│   │   ├── IAppointmentService.cs
│   │   └── IDocumentService.cs
│   └── Services/                  # Implementações
│       └── AuthService.cs         # Serviço de autenticação
│
├── PatientPortal.Infrastructure/  # Camada de Infraestrutura
│   ├── Data/                      # Contexto EF Core
│   ├── Repositories/              # Implementações dos repositórios
│   └── Migrations/                # Migrações do banco
│
├── PatientPortal.Api/             # Camada de API
│   └── Controllers/               # Controllers REST
│
└── PatientPortal.Tests/           # Testes unitários e de integração
```

### Frontend (Angular 20)

```
frontend/patient-portal/
├── src/
│   ├── app/
│   │   ├── pages/                 # Páginas da aplicação
│   │   │   ├── auth/              # Login, registro, recuperação de senha
│   │   │   ├── dashboard/         # Dashboard do paciente
│   │   │   ├── appointments/      # Gestão de agendamentos
│   │   │   ├── documents/         # Visualização de documentos
│   │   │   └── profile/           # Perfil do paciente
│   │   ├── services/              # Serviços Angular
│   │   ├── guards/                # Guards de autenticação
│   │   ├── interceptors/          # HTTP interceptors
│   │   └── models/                # Modelos TypeScript
│   └── assets/                    # Recursos estáticos
```

## 🔐 Segurança

### Autenticação

- **JWT (JSON Web Tokens)** para autenticação stateless
- **Refresh Tokens** com rotação automática (validade de 7 dias)
- **Access Tokens** de curta duração (15 minutos)
- **Password Hashing** com PBKDF2 (100.000 iterações)
- **Account Lockout** após 5 tentativas falhadas (bloqueio de 15 minutos)

### Proteção de Dados

- Conformidade com **LGPD** (Lei Geral de Proteção de Dados)
- Conformidade com **CFM 2.314/2022** (Telemedicina)
- Auditoria completa de acessos
- Criptografia de dados sensíveis em repouso

## 📊 Funcionalidades

### ✅ Implementadas (Fase 1-2)

#### Backend
- [x] Estrutura completa DDD
- [x] Entidades de domínio (PatientUser, RefreshToken, AppointmentView, DocumentView)
- [x] DTOs para autenticação, agendamentos e documentos
- [x] Serviço de autenticação completo
- [x] Interface para serviços de appointment e document

#### Frontend
- [x] Projeto Angular 20 configurado
- [x] Estrutura de pastas otimizada

### 🚧 Em Desenvolvimento (Fase 2-3)

- [ ] Infrastructure layer (repositórios, EF Core, PostgreSQL)
- [ ] API Controllers (Auth, Appointments, Documents, Profile)
- [ ] JWT middleware e configuração
- [ ] Pages Angular (Login, Register, Dashboard, etc.)
- [ ] Services Angular e HTTP interceptors

### 📋 Planejadas (Fase 4-6)

- [ ] Agendamento online de consultas
- [ ] Reagendamento e cancelamento
- [ ] Download de documentos (PDF)
- [ ] Notificações por email/SMS
- [ ] Integração com telemedicina
- [ ] Suporte a 2FA (autenticação de dois fatores)

## 🚀 Instalação e Configuração

### Pré-requisitos

- .NET 8 SDK
- Node.js 18+ e npm
- PostgreSQL 14+
- Angular CLI 20

### Backend

```bash
cd patient-portal-api

# Restaurar pacotes
dotnet restore

# Configurar connection string no appsettings.json
# "ConnectionStrings": {
#   "DefaultConnection": "Host=localhost;Database=patient_portal;Username=postgres;Password=..."
# }

# Criar e aplicar migrations
dotnet ef migrations add InitialCreate --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
dotnet ef database update --project PatientPortal.Infrastructure --startup-project PatientPortal.Api

# Executar API
dotnet run --project PatientPortal.Api
```

A API estará disponível em: `https://localhost:7000`

### Frontend

```bash
cd frontend/patient-portal

# Instalar dependências
npm install

# Executar em modo de desenvolvimento
npm start
```

O frontend estará disponível em: `http://localhost:4200`

## 🔧 Configuração

### appsettings.json (Backend)

O backend suporta múltiplos ambientes com arquivos de configuração específicos:
- `appsettings.json` - Configuração base (Production)
- `appsettings.Development.json` - Desenvolvimento local
- `appsettings.Testing.json` - Testes automatizados/CI

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=patient_portal;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "PatientPortal.Api",
    "Audience": "PatientPortal.Frontend",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "AppointmentReminder": {
    "Enabled": true,
    "CheckIntervalMinutes": 60,
    "AdvanceNoticeHours": 24
  },
  "RateLimiting": {
    "EnableRateLimiting": true,
    "PermitLimit": 100,
    "WindowMinutes": 1
  }
}
```

**Nota:** Para testes (ASPNETCORE_ENVIRONMENT=Testing), o serviço de lembretes é desabilitado automaticamente via `appsettings.Testing.json` para evitar dependências do banco de dados principal.

### environment.ts (Frontend)

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api',
  tokenKey: 'patient_access_token',
  refreshTokenKey: 'patient_refresh_token'
};
```

## 🧪 Testes

### Backend

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover

# Executar apenas testes de segurança
dotnet test --filter "Category=Security"

# Executar apenas testes de performance
dotnet test --filter "Category=Performance"

# Executar com verbosidade
dotnet test --verbosity normal
```

**Testes Implementados:**
- ✅ 15 testes unitários (Domain entities)
- ✅ 7 testes de integração (API endpoints)
- ✅ 8 testes de segurança (JWT, passwords, SQL injection)
- ✅ 5 testes de performance (response time, concurrency)

### Frontend

```bash
# Testes unitários (Karma/Jasmine)
npm test

# Testes E2E (Playwright)
npm run e2e

# E2E com UI interativa
npm run e2e:ui

# E2E em browser específico
npm run e2e -- --project chromium
```

**Testes E2E Implementados:**
- ✅ auth.spec.ts (7 testes de autenticação)
- ✅ dashboard.spec.ts (6 testes de navegação)
- ✅ appointments.spec.ts (5 testes de agendamentos)
- ✅ documents.spec.ts (6 testes de documentos)
- ✅ profile.spec.ts (6 testes de perfil)

## 🔄 CI/CD

O projeto possui um pipeline completo de CI/CD usando GitHub Actions.

**Workflow:** `.github/workflows/patient-portal-ci.yml`

**Jobs do Pipeline:**
1. ✅ Backend Tests - Testes automatizados do backend
2. ✅ Frontend Tests - Testes unitários do frontend
3. ✅ Security Tests - OWASP Dependency Check
4. ✅ Build Backend - Docker image da API
5. ✅ Build Frontend - Docker image do frontend
6. ✅ Performance Tests - Load testing com k6
7. 🚀 Deploy Staging - Deploy automático no develop
8. 🚀 Deploy Production - Deploy automático no main

**Executar Localmente:**

```bash
# Build Docker images
cd patient-portal-api
docker build -f PatientPortal.Api/Dockerfile -t patient-portal-api:local .

cd ../frontend/patient-portal
docker build -t patient-portal-frontend:local .

# Executar com docker-compose
cd ../patient-portal-api
docker-compose up --build
```

Veja [CI_CD_GUIDE.md](CI_CD_GUIDE.md) para documentação completa.

## 📚 API Endpoints

### Autenticação

```
POST   /api/auth/login           # Login de paciente (email ou CPF + senha)
POST   /api/auth/register        # Registro de novo paciente
POST   /api/auth/refresh         # Atualizar access token
POST   /api/auth/logout          # Logout e revogação de token
POST   /api/auth/change-password # Alterar senha
```

### Agendamentos

```
GET    /api/appointments                 # Listar todos agendamentos (com paginação)
GET    /api/appointments/{id}            # Obter agendamento específico
GET    /api/appointments/upcoming        # Listar próximos agendamentos
GET    /api/appointments/status/{status} # Filtrar por status (Scheduled, Completed, Cancelled)
GET    /api/appointments/count           # Contagem total de agendamentos
```

### Documentos

```
GET    /api/documents               # Listar todos documentos (com paginação)
GET    /api/documents/{id}          # Obter documento específico
GET    /api/documents/{id}/download # Download do documento
GET    /api/documents/recent        # Documentos recentes
GET    /api/documents/type/{type}   # Filtrar por tipo (Prescription, Exam, MedicalCertificate, Referral)
GET    /api/documents/count         # Contagem total de documentos
```

### Perfil

```
GET    /api/profile/me              # Obter perfil do paciente autenticado
PUT    /api/profile/me              # Atualizar perfil (nome, telefone)
```

## 📖 Documentação da API

Após executar a API, acesse a documentação Swagger interativa em:
- **Local:** `http://localhost:5000` (porta padrão configurável)
- **Swagger UI:** Disponível na raiz da aplicação
- **Autenticação:** Use o botão "Authorize" no Swagger para testar endpoints protegidos

## 🔒 Compliance e Regulamentações

### CFM (Conselho Federal de Medicina)

- ✅ **Resolução CFM 2.314/2022** - Telemedicina (preparado)
- ✅ **Resolução CFM 1.821/2007** - Prontuário Eletrônico (visualização)
- ✅ **Resolução CFM 1.638/2002** - Segurança de Dados

### LGPD

- ✅ Consentimento do paciente registrado
- ✅ Direito ao esquecimento (soft delete)
- ✅ Portabilidade de dados (export)
- ✅ Auditoria de acessos completa
- ✅ Criptografia de dados sensíveis

## 👥 Autores

- **Omni Care Software Team**
- **GitHub Copilot** - Desenvolvimento assistido por IA

## 📄 Licença

Este projeto faz parte do sistema Omni Care Software.

## 🔗 Links Relacionados

- [Documentação Principal do Omni Care Software](../../docs/README.md)
- [PLANO_DESENVOLVIMENTO.md](../../docs/PLANO_DESENVOLVIMENTO.md)
- [PENDING_TASKS.md](../../docs/PENDING_TASKS.md)
- [RESUMO_TECNICO_COMPLETO.md](../../docs/RESUMO_TECNICO_COMPLETO.md)

---

**Versão:** 1.0.0 (Janeiro 2026)  
**Status:** Em Desenvolvimento (Fases 1-2 Completas)
