# 🎥 MedicSoft Telemedicine Microservice

Microserviço independente para teleconsultas médicas com videochamadas integradas ao MedicWarehouse.

## 📋 Visão Geral

Este microserviço fornece funcionalidade completa de telemedicina, incluindo:
- ✅ Gestão de sessões de videochamada
- ✅ Integração com serviço de vídeo Daily.co
- ✅ Suporte multi-tenant (isolamento por TenantId)
- ✅ Gravação de consultas (opcional)
- ✅ Rastreamento de duração e auditoria
- ✅ APIs RESTful com Swagger

## 🏗️ Arquitetura

O microserviço segue **Clean Architecture** e **Domain-Driven Design (DDD)**:

```
telemedicine/
├── src/
│   ├── MedicSoft.Telemedicine.Domain/        # Entidades, VOs, Interfaces
│   ├── MedicSoft.Telemedicine.Application/   # Serviços, DTOs
│   ├── MedicSoft.Telemedicine.Infrastructure/# Repositórios, Daily.co
│   └── MedicSoft.Telemedicine.Api/           # Controllers, Swagger
└── tests/
    └── MedicSoft.Telemedicine.Tests/         # Testes Unitários (22 tests)
```

### Camadas

#### 1. Domain Layer
- **Entities**: `TelemedicineSession` (Aggregate Root)
- **Value Objects**: `SessionDuration`
- **Enums**: `SessionStatus`, `ParticipantRole`
- **Interfaces**: `ITelemedicineSessionRepository`, `IVideoCallService`

#### 2. Application Layer
- **Services**: `TelemedicineService` (orquestra lógica de negócio)
- **DTOs**: Request/Response objects
- **Interfaces**: `ITelemedicineService`

#### 3. Infrastructure Layer
- **Repositories**: `TelemedicineSessionRepository` (EF Core)
- **External Services**: `DailyCoVideoService` (integração Daily.co)
- **Persistence**: `TelemedicineDbContext` (PostgreSQL/InMemory)

#### 4. API Layer
- **Controllers**: `SessionsController` (endpoints REST)
- **Swagger**: Documentação automática da API

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL (ou usar InMemory para desenvolvimento)
- Conta Daily.co (Free tier disponível)

### Configuração

1. **Clone o repositório**
```bash
cd telemedicine
```

2. **Configure appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=telemedicine;Username=postgres;Password=yourpass"
  },
  "DailyCo": {
    "ApiKey": "sua-chave-api-daily-co"
  }
}
```

3. **Execute as migrations (se usar PostgreSQL)**
```bash
cd src/MedicSoft.Telemedicine.Api
dotnet ef database update
```

4. **Execute a API**
```bash
dotnet run --project src/MedicSoft.Telemedicine.Api
```

A API estará disponível em `https://localhost:7000` (ou `http://localhost:5000`)

### Executar Testes

```bash
dotnet test
```

**Resultado esperado**: 22/22 testes passando ✅

## 📡 API Endpoints

Todos os endpoints requerem header `X-Tenant-Id` para multi-tenancy.

### Criar Sessão
```http
POST /api/sessions
Content-Type: application/json
X-Tenant-Id: clinic-123

{
  "appointmentId": "guid",
  "clinicId": "guid",
  "providerId": "guid",
  "patientId": "guid"
}
```

### Entrar na Sessão
```http
POST /api/sessions/{sessionId}/join
Content-Type: application/json
X-Tenant-Id: clinic-123

{
  "userId": "guid",
  "userName": "Dr. Silva",
  "role": "provider"
}
```

**Resposta:**
```json
{
  "roomUrl": "https://example.daily.co/session-xxx",
  "accessToken": "eyJhbGc...",
  "expiresAt": "2024-10-29T16:00:00Z"
}
```

### Iniciar Sessão
```http
POST /api/sessions/{sessionId}/start
X-Tenant-Id: clinic-123
```

### Completar Sessão
```http
POST /api/sessions/{sessionId}/complete
Content-Type: application/json
X-Tenant-Id: clinic-123

{
  "notes": "Consulta realizada com sucesso"
}
```

### Cancelar Sessão
```http
POST /api/sessions/{sessionId}/cancel
Content-Type: application/json
X-Tenant-Id: clinic-123

"Paciente não compareceu"
```

### Listar Sessões
```http
GET /api/sessions/clinic/{clinicId}?skip=0&take=50
X-Tenant-Id: clinic-123

GET /api/sessions/provider/{providerId}
X-Tenant-Id: clinic-123

GET /api/sessions/patient/{patientId}
X-Tenant-Id: clinic-123
```

## 🎥 Integração Daily.co

### Por que Daily.co?

- ✅ **Free Tier Generoso**: 10.000 minutos/mês grátis
- ✅ **HIPAA Compliant**: Adequado para uso médico
- ✅ **Baixo Custo**: $0.0015/minuto após free tier
- ✅ **API Simples**: Fácil de integrar
- ✅ **Recording Incluído**: Gravação cloud automática

### Exemplo de Custo

```
Para 1.000 consultas/mês de 30 minutos:
- Total: 30.000 minutos/mês
- Free: 10.000 minutos = $0
- Pago: 20.000 minutos × $0.0015 = $30/mês

Custo final: $30/mês para 1.000 consultas
```

### Configurar Daily.co

1. Crie conta em https://daily.co
2. Obtenha API key no dashboard
3. Configure em `appsettings.json`:
```json
{
  "DailyCo": {
    "ApiKey": "sua-chave-aqui"
  }
}
```

## 🧪 Testes Unitários

O projeto inclui 22 testes unitários cobrindo:

### Domain Tests (12 tests)
- Criação de sessões
- Validações de entidade
- Transições de estado (Scheduled → InProgress → Completed)
- Cancelamento e falhas
- Recording URLs
- Notas de sessão

### Application Tests (10 tests)
- Criação de sessões via service
- Join de sessões (geração de tokens)
- Start, complete e cancel de sessões
- Integração com video service
- Tratamento de erros

**Executar testes:**
```bash
dotnet test --verbosity normal
```

## 🔒 Segurança

### Multi-Tenancy
Todas as operações são isoladas por `TenantId`:
- Passado via header `X-Tenant-Id`
- Validado em todas queries
- Impossível acessar dados de outro tenant

### Tokens JWT (Daily.co)
- Tokens temporários (120 minutos)
- Um token por usuário/sessão
- Não expõe room URLs públicas

### HIPAA Compliance
- Daily.co é HIPAA compliant
- BAA (Business Associate Agreement) disponível
- Encryption at rest e in transit
- Logs completos de auditoria

## 📊 Modelo de Dados

### TelemedicineSession
```csharp
public class TelemedicineSession
{
    Guid Id
    string TenantId
    Guid AppointmentId      // Link com Appointment da API principal
    Guid ClinicId
    Guid ProviderId         // Médico/Dentista
    Guid PatientId
    string RoomId           // Daily.co room ID
    string RoomUrl
    SessionStatus Status    // Scheduled, InProgress, Completed, Cancelled, Failed
    SessionDuration Duration
    string? RecordingUrl
    string? SessionNotes
    DateTime CreatedAt
    DateTime? UpdatedAt
}
```

### SessionStatus
- `Scheduled`: Agendada, aguardando início
- `InProgress`: Ativa, em andamento
- `Completed`: Finalizada com sucesso
- `Cancelled`: Cancelada
- `Failed`: Falhou por problemas técnicos

## 🔄 Integração com API Principal

O microserviço se integra com a API principal via IDs:

```
MedicSoft.Api (Principal)          MedicSoft.Telemedicine.Api
─────────────────────────          ───────────────────────────
Appointment (Id)          ────────> TelemedicineSession (AppointmentId)
Clinic (Id)               ────────> TelemedicineSession (ClinicId)
User (Doctor/Patient Id)  ────────> TelemedicineSession (ProviderId/PatientId)
```

### Fluxo de Consulta

1. **Criar Appointment** na API principal
2. **Criar Session** no microserviço Telemedicine
3. **Provider/Patient Join** - gerar tokens de acesso
4. **Start Session** quando iniciada
5. **Complete Session** ao finalizar
6. **Salvar Recording URL** se disponível

## 📈 Escalabilidade

### Desenvolvimento (0-20 clínicas)
- InMemory Database
- Daily.co Free Tier
- Custo: $0/mês

### Produção Inicial (20-100 clínicas)
- PostgreSQL (Railway/Hetzner)
- Daily.co Pay-as-you-go
- Custo: $5-30/mês

### Escala (100-500 clínicas)
- PostgreSQL com Read Replicas
- Daily.co com volume pricing
- Load Balancer
- Custo: $100-300/mês

## 🛠️ Desenvolvimento

### Adicionar Nova Funcionalidade

1. **Domain**: Adicione entidades/value objects em `Domain/`
2. **Application**: Adicione services/DTOs em `Application/`
3. **Infrastructure**: Implemente repositórios/external services
4. **API**: Adicione controllers/endpoints
5. **Tests**: Escreva testes unitários

### Código Limpo

O projeto segue:
- ✅ SOLID principles
- ✅ DDD patterns (Aggregate Roots, Value Objects)
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ Clean Architecture
- ✅ Unit Testing (AAA pattern)

## 📚 Documentação Adicional

- [Análise de Serviços de Vídeo](../docs/TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md) - Análise completa de 5 provedores
- [Daily.co API Docs](https://docs.daily.co) - Documentação oficial da API
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Artigo original
- [DDD Reference](https://www.domainlanguage.com/ddd/) - Guia de referência DDD

## 🤝 Contribuindo

1. Mantenha Clean Code
2. Escreva testes unitários
3. Siga DDD patterns
4. Documente suas mudanças
5. Use commits semânticos

## 📄 Licença

Este projeto é parte do MedicWarehouse.

---

**Criado por**: GitHub Copilot  
**Data**: Outubro/Novembro 2024  
**Versão**: 1.0.0  
**Tecnologias**: .NET 8, PostgreSQL, Daily.co, Clean Architecture, DDD
