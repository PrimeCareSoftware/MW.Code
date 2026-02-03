# 🚀 Portal do Paciente - Guia de Início Rápido para Desenvolvedores

> **Objetivo:** Colocar você desenvolvendo no Portal do Paciente em **< 30 minutos**  
> **Público:** Desenvolvedores novos no projeto  
> **Última Atualização:** 26 de Janeiro de 2026

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (ou superior)
- ✅ [Node.js 18+](https://nodejs.org/) e npm
- ✅ [PostgreSQL 14+](https://www.postgresql.org/download/)
- ✅ [Angular CLI 20](https://angular.io/cli): `npm install -g @angular/cli`
- ✅ [Git](https://git-scm.com/)
- ✅ IDE: [VS Code](https://code.visualstudio.com/) ou [Visual Studio 2022](https://visualstudio.microsoft.com/)

---

## ⚡ Setup Rápido (5 minutos)

### 1. Clone o Repositório

```bash
git clone https://github.com/Omni CareSoftware/MW.Code.git
cd MW.Code
```

### 2. Configure o Banco de Dados

**Opção A: PostgreSQL Local**

```bash
# Criar banco de dados
psql -U postgres
CREATE DATABASE patient_portal;
\q
```

**Opção B: Docker (mais rápido)**

```bash
docker run --name patient-portal-db \
  -e POSTGRES_PASSWORD=dev123 \
  -e POSTGRES_DB=patient_portal \
  -p 5432:5432 \
  -d postgres:14-alpine
```

### 3. Configure a Connection String

**Backend: `patient-portal-api/PatientPortal.Api/appsettings.Development.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=patient_portal;Username=postgres;Password=dev123"
  },
  "JwtSettings": {
    "SecretKey": "dev-secret-key-min-32-characters-long-12345",
    "Issuer": "PatientPortal.Api",
    "Audience": "PatientPortal.Frontend",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 4. Aplique as Migrations

```bash
cd patient-portal-api

# Restaurar pacotes
dotnet restore

# Aplicar migrations
dotnet ef database update --project PatientPortal.Infrastructure --startup-project PatientPortal.Api

# Se migrations não existem, criar:
dotnet ef migrations add InitialCreate --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
```

### 5. Execute o Backend

```bash
cd PatientPortal.Api
dotnet run

# OU usar watch (hot reload)
dotnet watch run

# API estará em: https://localhost:7000
# Swagger UI: https://localhost:7000
```

### 6. Configure o Frontend

**`frontend/patient-portal/src/environments/environment.ts`**

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api',
  tokenKey: 'patient_access_token',
  refreshTokenKey: 'patient_refresh_token'
};
```

### 7. Execute o Frontend

```bash
cd frontend/patient-portal

# Instalar dependências
npm install

# Executar em modo dev
npm start

# Frontend estará em: http://localhost:4200
```

---

## ✅ Verificação Rápida

### Teste o Backend

```bash
# Health check
curl https://localhost:7000/health

# Swagger UI
open https://localhost:7000
```

### Teste o Frontend

```bash
# Abrir no browser
open http://localhost:4200

# Você deve ver a tela de login
```

---

## 🧪 Dados de Teste

### Criar Usuário de Teste (Opcional)

```sql
-- Conectar ao banco
psql -U postgres -d patient_portal

-- Criar paciente de teste
INSERT INTO "PatientUsers" ("Id", "Name", "Email", "CPF", "Phone", "PasswordHash", "EmailConfirmed", "CreatedAt")
VALUES (
  'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
  'João da Silva',
  'joao.silva@example.com',
  '12345678901',
  '5511999999999',
  'AQAAAAIAAYagAAAAEJ...', -- Senha: 'Test@123'
  true,
  NOW()
);

-- Ver usuários
SELECT "Name", "Email", "CPF" FROM "PatientUsers";
```

**Ou use o endpoint de registro:**

```bash
curl -X POST https://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João da Silva",
    "email": "joao@example.com",
    "cpf": "12345678901",
    "phone": "11999999999",
    "password": "Test@123",
    "confirmPassword": "Test@123",
    "birthDate": "1990-01-15"
  }'
```

---

## 🎯 Próximos Passos

### 1. Entenda a Arquitetura (15 min)

Leia os seguintes documentos na ordem:

1. **[README.md](./README.md)** - Visão geral do projeto
2. **[PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md)** - Arquitetura DDD/Clean Architecture
3. **[PORTAL_PACIENTE_STATUS_JAN2026.md](../PORTAL_PACIENTE_STATUS_JAN2026.md)** - Status atual (70% completo)

### 2. Explore o Código (30 min)

#### Backend (.NET 8)

```
patient-portal-api/
├── PatientPortal.Domain/        # ← Comece aqui (Entidades)
│   ├── Entities/
│   │   ├── PatientUser.cs       # Modelo de usuário
│   │   ├── Appointment.cs       # Agendamento
│   │   └── Document.cs          # Documento médico
│   └── Interfaces/
│
├── PatientPortal.Application/   # ← Lógica de negócio
│   ├── DTOs/
│   └── Services/
│       └── AuthService.cs       # Autenticação
│
├── PatientPortal.Infrastructure/ # ← Banco de dados
│   ├── Data/
│   │   └── PatientPortalContext.cs
│   └── Repositories/
│
└── PatientPortal.Api/           # ← Controllers
    └── Controllers/
        ├── AuthController.cs    # Login, registro
        ├── AppointmentsController.cs
        └── DocumentsController.cs
```

#### Frontend (Angular 20)

```
frontend/patient-portal/src/app/
├── pages/                       # ← Comece aqui (Páginas)
│   ├── auth/
│   │   ├── login/               # Tela de login
│   │   └── register/            # Registro
│   ├── dashboard/               # Dashboard principal
│   ├── appointments/            # Gestão de consultas
│   ├── documents/               # Documentos
│   └── profile/                 # Perfil do paciente
│
├── services/                    # ← Serviços HTTP
│   ├── auth.service.ts
│   ├── appointment.service.ts
│   └── document.service.ts
│
├── guards/                      # ← Proteção de rotas
│   └── auth.guard.ts
│
└── interceptors/                # ← Interceptors HTTP
    └── auth.interceptor.ts      # Adiciona JWT em requests
```

### 3. Execute os Testes (10 min)

#### Backend

```bash
cd patient-portal-api

# Todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true

# Testes de uma categoria específica
dotnet test --filter "Category=Security"
```

**Resultado esperado:** 35+ testes passando (100%)

#### Frontend

```bash
cd frontend/patient-portal

# Testes unitários
npm test

# Testes E2E
npm run e2e

# E2E com UI
npm run e2e:ui
```

**Resultado esperado:** 58 unit tests (98.66% coverage), 30+ E2E tests

---

## 🔧 Tarefas Comuns de Desenvolvimento

### Adicionar um Novo Endpoint

**1. Criar DTO:**

```csharp
// PatientPortal.Application/DTOs/MyFeature/MyRequestDto.cs
public class MyRequestDto
{
    [Required]
    public string Name { get; set; }
}
```

**2. Criar Controller:**

```csharp
// PatientPortal.Api/Controllers/MyFeatureController.cs
[ApiController]
[Route("api/my-feature")]
[Authorize]
public class MyFeatureController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MyRequestDto dto)
    {
        // Lógica aqui
        return Ok();
    }
}
```

**3. Testar no Swagger:**

```
https://localhost:7000
```

### Adicionar uma Nova Página (Frontend)

```bash
cd frontend/patient-portal

# Gerar novo componente
ng generate component pages/my-new-page

# Adicionar rota
# Editar: src/app/app-routing.module.ts
```

```typescript
const routes: Routes = [
  // ... rotas existentes
  {
    path: 'my-new-page',
    component: MyNewPageComponent,
    canActivate: [AuthGuard]  // Se requer autenticação
  }
];
```

### Adicionar Migration

```bash
cd patient-portal-api

# Criar migration
dotnet ef migrations add AddMyNewFeature \
  --project PatientPortal.Infrastructure \
  --startup-project PatientPortal.Api

# Aplicar
dotnet ef database update \
  --project PatientPortal.Infrastructure \
  --startup-project PatientPortal.Api
```

---

## 🐛 Debugging

### Backend (.NET)

**VS Code:**

1. Abra `patient-portal-api` no VS Code
2. Pressione `F5` (ou Run → Start Debugging)
3. Coloque breakpoints clicando na linha

**Visual Studio:**

1. Abra `PatientPortal.sln`
2. Defina `PatientPortal.Api` como projeto de inicialização
3. Pressione `F5`

### Frontend (Angular)

**Chrome DevTools:**

1. Abra `http://localhost:4200`
2. Pressione `F12`
3. Vá para "Sources" → `webpack://` → `.` → `src`
4. Coloque breakpoints

**VS Code:**

1. Instale extensão "Debugger for Chrome"
2. Pressione `F5`
3. Selecione "Chrome"

---

## 📚 Recursos de Aprendizado

### Documentação do Projeto

| Documento | Propósito | Tempo |
|-----------|-----------|-------|
| [README.md](./README.md) | Visão geral | 5 min |
| [PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md) | Arquitetura detalhada | 15 min |
| [PORTAL_PACIENTE_STATUS_JAN2026.md](../PORTAL_PACIENTE_STATUS_JAN2026.md) | Status e roadmap | 10 min |
| [BOOKING_IMPLEMENTATION_GUIDE.md](./BOOKING_IMPLEMENTATION_GUIDE.md) | Agendamento online (TODO) | 20 min |
| [NOTIFICATION_SERVICE_GUIDE.md](./NOTIFICATION_SERVICE_GUIDE.md) | Notificações (TODO) | 15 min |
| [TROUBLESHOOTING_FAQ.md](./TROUBLESHOOTING_FAQ.md) | Resolução de problemas | Conforme necessário |

### Conceitos Importantes

- **DDD (Domain-Driven Design):** Arquitetura do backend
- **Clean Architecture:** Separação de camadas
- **JWT:** Autenticação stateless
- **LGPD:** Compliance de privacidade
- **CFM 2.314/2022:** Regulamentação de telemedicina

### Tecnologias Usadas

**Backend:**
- .NET 8 / C#
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI

**Frontend:**
- Angular 20 / TypeScript
- Angular Material
- RxJS
- Playwright (E2E)
- Jasmine/Karma (Unit)

---

## ❓ Dúvidas Frequentes

**Q: Preciso configurar Twilio/SendGrid para desenvolvimento?**  
A: Não. Notificações são opcionais. Você pode desenvolver sem elas.

**Q: Como resetar o banco de dados?**  
```bash
dotnet ef database drop --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
dotnet ef database update --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
```

**Q: Erro "Port 7000 already in use"?**  
```bash
# Mudar porta em appsettings.json ou:
export ASPNETCORE_URLS="http://+:5001;https://+:7001"
```

**Q: Frontend não conecta ao backend?**  
- Verifique CORS no backend (`Program.cs`)
- Verifique `apiUrl` no `environment.ts`
- Backend precisa estar rodando

**Q: Onde está a documentação da API?**  
- Swagger: `https://localhost:7000`
- Ou veja controllers no código

---

## 🚨 Problemas Comuns

### Backend não inicia

```bash
# Verificar .NET SDK
dotnet --version  # Deve ser 8.0+

# Restaurar pacotes
dotnet restore

# Limpar e rebuild
dotnet clean
dotnet build
```

### Migrations falham

```bash
# Verificar se banco existe
psql -U postgres -l | grep patient_portal

# Recriar banco
dropdb patient_portal
createdb patient_portal

# Aplicar migrations
dotnet ef database update
```

### Frontend não carrega

```bash
# Verificar Node.js
node --version  # Deve ser 18+

# Limpar cache
rm -rf node_modules package-lock.json
npm cache clean --force
npm install

# Verificar porta
lsof -i :4200  # Deve estar livre
```

---

## 🎯 Checklist de Setup Completo

- [ ] ✅ .NET 8 SDK instalado
- [ ] ✅ Node.js 18+ instalado
- [ ] ✅ PostgreSQL rodando
- [ ] ✅ Banco `patient_portal` criado
- [ ] ✅ Connection string configurada
- [ ] ✅ Migrations aplicadas
- [ ] ✅ Backend rodando em https://localhost:7000
- [ ] ✅ Swagger acessível
- [ ] ✅ Frontend rodando em http://localhost:4200
- [ ] ✅ Testes backend passando (35+ testes)
- [ ] ✅ Testes frontend passando (58+ testes)
- [ ] ✅ Consegue fazer login/logout
- [ ] ✅ Consegue ver documentos/agendamentos

---

## 📞 Precisa de Ajuda?

1. **Verifique [TROUBLESHOOTING_FAQ.md](./TROUBLESHOOTING_FAQ.md)**
2. **Pergunte no canal #dev-portal-paciente** (Slack/Teams)
3. **Abra uma issue** no GitHub com label `question`

---

**Bem-vindo ao time! 🎉**

Agora você está pronto para começar a desenvolver. Qualquer dúvida, consulte a documentação ou peça ajuda aos membros da equipe.

---

**Última Atualização:** 26 de Janeiro de 2026  
**Próximo Passo:** [PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md)
