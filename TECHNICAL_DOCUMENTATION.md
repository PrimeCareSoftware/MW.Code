# 🔧 Documentação Técnica - MVP Fase 1

## 📋 Visão Geral

Este documento fornece informações técnicas sobre a arquitetura, APIs, deployment e operação do Omni Care Software MVP Fase 1.

**Versão**: 1.0.0-MVP
**Data**: Janeiro 2026
**Stack Principal**: .NET Core, Angular, PostgreSQL

> ⚠️ **Nota**: Todos os exemplos de código, URLs, emails e telefones neste documento são fictícios para fins de documentação. Use valores reais em ambiente de produção.

---

## 🏗️ Arquitetura do Sistema

### Componentes Principais

```
┌─────────────────────────────────────────────────┐
│                 Load Balancer                    │
└─────────────────┬───────────────────────────────┘
                  │
        ┌─────────┴─────────┐
        │                   │
┌───────▼────────┐  ┌──────▼─────────┐
│  Web Frontend  │  │   Mobile Web   │
│   (Angular)    │  │  (Responsive)  │
└───────┬────────┘  └────────┬───────┘
        │                    │
        └──────────┬─────────┘
                   │
        ┌──────────▼──────────────┐
        │      API Gateway         │
        │   (ASP.NET Core)         │
        └──────────┬──────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────▼────────┐  ┌────────▼───────┐
│   Core API     │  │  Patient Portal│
│  (Main System) │  │      API       │
└───────┬────────┘  └────────┬───────┘
        │                    │
        └──────────┬─────────┘
                   │
        ┌──────────▼──────────┐
        │   PostgreSQL DB     │
        │   (Multi-tenant)    │
        └─────────────────────┘
```

### Stack Tecnológico

#### Backend
- **Framework**: ASP.NET Core 8.0
- **Linguagem**: C# 12
- **ORM**: Entity Framework Core
- **API Style**: RESTful
- **Documentação**: Swagger/OpenAPI 3.0
- **Autenticação**: JWT (JSON Web Tokens)

#### Frontend
- **Framework**: Angular 17+
- **Linguagem**: TypeScript 5.3
- **UI**: Angular Material
- **State Management**: NgRx
- **Build**: Angular CLI

#### Banco de Dados
- **SGBD**: PostgreSQL 15
- **Estratégia**: Multi-tenant (schema por tenant)
- **Migrations**: Entity Framework Migrations
- **Backup**: Diário automático

#### Infraestrutura
- **Container**: Docker
- **Orquestração**: Docker Compose / Kubernetes (produção)
- **Proxy**: Nginx
- **Cache**: Redis (opcional)

---

## 🔌 APIs Disponíveis

### Documentação Swagger

**URL**: `https://api.omnicaresoftware.com/swagger`

**Acesso**: 
- Desenvolvimento: Público
- Produção: Requer autenticação

### Principais Endpoints

#### 1. Autenticação

**Base URL**: `/api/auth`

##### POST /api/auth/login
Autenticar usuário e obter token JWT.

**Request**:
```json
{
  "email": "usuario@clinica.com",
  "password": "senha123",
  "tenantId": "clinic-123"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "refresh_token_here",
  "expiresIn": 3600,
  "user": {
    "id": "user-123",
    "name": "Dr. João Silva",
    "email": "joao@clinica.com",
    "role": "DOCTOR"
  }
}
```

##### POST /api/auth/refresh
Renovar token expirado.

##### POST /api/auth/logout
Invalidar token atual.

##### POST /api/auth/forgot-password
Solicitar recuperação de senha.

---

#### 2. Pacientes

**Base URL**: `/api/patients`

##### GET /api/patients
Listar pacientes do tenant.

**Query Parameters**:
- `page`: Página (padrão: 1)
- `pageSize`: Tamanho da página (padrão: 30, max: 100)
- `search`: Busca por nome ou CPF
- `orderBy`: Campo para ordenação (name, createdAt, etc.)
- `orderDirection`: asc ou desc

**Response (200 OK)**:
```json
{
  "data": [
    {
      "id": "patient-123",
      "name": "Maria da Silva",
      "cpf": "123.456.789-00",
      "birthDate": "1990-01-15",
      "email": "maria@email.com",
      "phone": "(11) 99999-0001",
      "createdAt": "2026-01-01T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 30,
    "totalItems": 150,
    "totalPages": 5
  }
}
```

##### POST /api/patients
Cadastrar novo paciente.

**Request**:
```json
{
  "name": "João da Silva",
  "cpf": "987.654.321-00",
  "birthDate": "1985-05-20",
  "gender": "M",
  "email": "joao@email.com",
  "phone": "(11) 98888-0001",
  "address": {
    "zipCode": "01234-567",
    "street": "Rua Exemplo",
    "number": "100",
    "complement": "Apto 10",
    "neighborhood": "Centro",
    "city": "São Paulo",
    "state": "SP"
  },
  "healthInsurance": {
    "name": "Unimed",
    "cardNumber": "123456789012345"
  },
  "lgpdConsent": true
}
```

##### GET /api/patients/{id}
Buscar paciente por ID.

##### PUT /api/patients/{id}
Atualizar dados do paciente.

##### DELETE /api/patients/{id}
Inativar paciente (soft delete).

---

#### 3. Agendamentos

**Base URL**: `/api/appointments`

##### GET /api/appointments
Listar agendamentos.

**Query Parameters**:
- `startDate`: Data inicial (ISO 8601)
- `endDate`: Data final (ISO 8601)
- `professionalId`: Filtrar por profissional
- `patientId`: Filtrar por paciente
- `status`: Filtrar por status (SCHEDULED, CONFIRMED, COMPLETED, CANCELLED)

##### POST /api/appointments
Criar novo agendamento.

**Request**:
```json
{
  "patientId": "patient-123",
  "professionalId": "prof-456",
  "startTime": "2026-02-15T14:00:00Z",
  "duration": 30,
  "type": "CONSULTATION",
  "notes": "Consulta de rotina"
}
```

##### GET /api/appointments/{id}
Buscar agendamento específico.

##### PUT /api/appointments/{id}
Atualizar agendamento.

##### DELETE /api/appointments/{id}
Cancelar agendamento.

##### GET /api/appointments/availability
Verificar horários disponíveis.

**Query Parameters**:
- `professionalId`: ID do profissional (obrigatório)
- `date`: Data desejada (ISO 8601)
- `duration`: Duração da consulta em minutos

**Response**:
```json
{
  "date": "2026-02-15",
  "availableSlots": [
    {
      "startTime": "09:00",
      "endTime": "09:30",
      "available": true
    },
    {
      "startTime": "09:30",
      "endTime": "10:00",
      "available": true
    },
    {
      "startTime": "10:00",
      "endTime": "10:30",
      "available": false
    }
  ]
}
```

---

#### 4. Prontuário Médico

**Base URL**: `/api/medical-records`

##### GET /api/medical-records/{patientId}
Listar prontuários do paciente.

##### POST /api/medical-records
Criar novo prontuário.

**Request**:
```json
{
  "patientId": "patient-123",
  "appointmentId": "appt-456",
  "chiefComplaint": "Dor de cabeça há 3 dias",
  "historyOfPresentIllness": "Paciente relata...",
  "physicalExamination": {
    "vitalSigns": {
      "bloodPressure": "120/80",
      "heartRate": 72,
      "temperature": 36.5,
      "weight": 70,
      "height": 170
    },
    "generalExam": "Paciente em bom estado geral..."
  },
  "diagnosis": [
    "Cefaleia tensional (CID-10: G44.2)"
  ],
  "treatment": "Prescrição de analgésico...",
  "prescription": [
    {
      "medication": "Paracetamol 500mg",
      "dosage": "1 comprimido a cada 8 horas",
      "duration": "5 dias"
    }
  ]
}
```

##### GET /api/medical-records/{id}
Buscar prontuário específico.

##### POST /api/medical-records/{id}/documents
Anexar documento ao prontuário.

---

#### 5. Relatórios

**Base URL**: `/api/reports`

##### GET /api/reports/appointments
Relatório de consultas por período.

**Query Parameters**:
- `startDate`: Data inicial
- `endDate`: Data final
- `professionalId`: (Opcional) Filtrar por profissional
- `format`: pdf ou excel

##### GET /api/reports/patients
Relatório de pacientes atendidos.

##### GET /api/reports/financial
Relatório financeiro básico.

---

### Autenticação e Autorização

#### JWT Token

Todas as requisições (exceto login) devem incluir o token JWT no header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

#### Roles (Perfis)

- `ADMIN`: Acesso total
- `DOCTOR`: Acesso a agenda, pacientes e prontuários
- `RECEPTIONIST`: Acesso a agenda e cadastros
- `FINANCIAL`: Acesso a módulo financeiro

#### Rate Limiting

- **Limite**: 1000 requisições por hora por tenant
- **Headers de resposta**:
  - `X-RateLimit-Limit`: Limite total
  - `X-RateLimit-Remaining`: Requisições restantes
  - `X-RateLimit-Reset`: Timestamp do reset

**Response (429 Too Many Requests)**:
```json
{
  "error": "Rate limit exceeded",
  "message": "Too many requests. Please try again later.",
  "retryAfter": 3600
}
```

---

## 🚀 Deployment

### Variáveis de Ambiente

#### Aplicação Principal

```bash
# Database
DATABASE_HOST=postgres.internal
DATABASE_PORT=5432
DATABASE_NAME=omnicare_db
DATABASE_USER=omnicare_user
DATABASE_PASSWORD=***SECURE_PASSWORD***

# JWT
JWT_SECRET=***SECURE_SECRET_KEY***
JWT_EXPIRATION=3600
JWT_ISSUER=omnicare.com
JWT_AUDIENCE=omnicare-api

# SMTP (Email)
SMTP_HOST=smtp.example.com
SMTP_PORT=587
SMTP_USER=noreply@omnicaresoftware.com
SMTP_PASSWORD=***SECURE_PASSWORD***
SMTP_FROM=noreply@omnicaresoftware.com

# Storage (Arquivos)
STORAGE_TYPE=S3  # ou LOCAL
S3_BUCKET=omnicare-files
S3_REGION=us-east-1
S3_ACCESS_KEY=***ACCESS_KEY***
S3_SECRET_KEY=***SECRET_KEY***

# Payment Gateway
PAYMENT_GATEWAY_URL=https://gateway.example.com
PAYMENT_API_KEY=***API_KEY***
PAYMENT_WEBHOOK_SECRET=***WEBHOOK_SECRET***

# Application
APP_NAME=Omni Care
APP_ENV=production  # development, staging, production
APP_DEBUG=false
APP_URL=https://app.omnicaresoftware.com
API_URL=https://api.omnicaresoftware.com

# Redis (Cache)
REDIS_HOST=redis.internal
REDIS_PORT=6379
REDIS_PASSWORD=***REDIS_PASSWORD***

# Monitoring
SENTRY_DSN=https://...@sentry.io/...
LOG_LEVEL=info  # debug, info, warn, error
```

#### Portal do Paciente

```bash
# API
API_URL=https://api.omnicaresoftware.com
PATIENT_PORTAL_URL=https://paciente.omnicaresoftware.com

# Authentication
JWT_SECRET=***SECURE_SECRET_KEY***  # Mesmo da API principal
```

### Docker Compose

**Arquivo**: `docker-compose.yml`

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: omnicare_db
      POSTGRES_USER: omnicare_user
      POSTGRES_PASSWORD: ${DATABASE_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    networks:
      - omnicare_network

  api:
    image: primecare/api:latest
    environment:
      - DATABASE_HOST=postgres
      - DATABASE_PORT=5432
      - DATABASE_NAME=omnicare_db
      - DATABASE_USER=omnicare_user
      - DATABASE_PASSWORD=${DATABASE_PASSWORD}
      - JWT_SECRET=${JWT_SECRET}
    depends_on:
      - postgres
    ports:
      - "5000:80"
    networks:
      - omnicare_network

  frontend:
    image: primecare/frontend:latest
    environment:
      - API_URL=http://api:80
    ports:
      - "4200:80"
    networks:
      - omnicare_network

  nginx:
    image: nginx:alpine
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - api
      - frontend
    networks:
      - omnicare_network

volumes:
  postgres_data:

networks:
  omnicare_network:
    driver: bridge
```

### Processo de Deploy

#### 1. Build da Aplicação

```bash
# Backend
cd src/Api
dotnet publish -c Release -o ./publish

# Frontend
cd frontend/medicwarehouse-app
npm run build -- --configuration=production
```

#### 2. Build das Imagens Docker

```bash
# API
docker build -t primecare/api:latest -f Dockerfile.api .

# Frontend
docker build -t primecare/frontend:latest -f Dockerfile.frontend .
```

#### 3. Push para Registry

```bash
docker push primecare/api:latest
docker push primecare/frontend:latest
```

#### 4. Deploy

```bash
# Em produção (com Docker Compose)
docker-compose -f docker-compose.production.yml up -d

# Ou com Kubernetes
kubectl apply -f k8s/
```

#### 5. Migrations

```bash
# Rodar migrations do banco
docker exec -it omnicare_api dotnet ef database update
```

### CI/CD Pipeline

```yaml
# .github/workflows/deploy.yml

name: Deploy to Production

on:
  push:
    branches:
      - main

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Build API
        run: dotnet build

      - name: Run Tests
        run: dotnet test

      - name: Build Docker Image
        run: docker build -t primecare/api:${{ github.sha }} .

      - name: Push to Registry
        run: docker push primecare/api:${{ github.sha }}

      - name: Deploy
        run: |
          kubectl set image deployment/api api=primecare/api:${{ github.sha }}
```

---

## 🔧 Operação e Manutenção

### Backup

#### Backup do Banco de Dados

**Automático**: Diariamente às 02:00 (horário de Brasília)

**Manual**:
```bash
# Backup
docker exec omnicare_postgres pg_dump -U omnicare_user omnicare_db > backup_$(date +%Y%m%d).sql

# Restore
docker exec -i omnicare_postgres psql -U omnicare_user omnicare_db < backup_20260130.sql
```

#### Retenção de Backups
- **Diários**: 7 dias
- **Semanais**: 4 semanas
- **Mensais**: 12 meses

### Monitoramento

#### Health Check

**Endpoint**: `GET /health`

```json
{
  "status": "Healthy",
  "database": "Healthy",
  "redis": "Healthy",
  "version": "1.0.0",
  "uptime": 86400
}
```

#### Logs

**Localização**: 
- Desenvolvimento: Console
- Produção: Sentry + File logs

**Níveis**:
- **ERROR**: Erros críticos
- **WARN**: Avisos importantes
- **INFO**: Informações gerais
- **DEBUG**: Detalhes (apenas desenvolvimento)

**Exemplo de Log**:
```
2026-01-30 10:15:30 [INFO] [Auth] User logged in: user@example.com
2026-01-30 10:16:45 [ERROR] [Database] Connection timeout after 30s
```

### Troubleshooting

#### Problema: API não responde

**Verificar**:
1. Container está rodando: `docker ps`
2. Logs do container: `docker logs omnicare_api`
3. Conectividade com banco: `docker exec omnicare_api ping postgres`

#### Problema: Banco de dados lento

**Verificar**:
1. Conexões abertas: `SELECT count(*) FROM pg_stat_activity;`
2. Queries lentas: Ativar `log_min_duration_statement`
3. Índices faltando: Analisar com `EXPLAIN ANALYZE`

#### Problema: Espaço em disco

**Verificar**:
```bash
# Espaço total
df -h

# Tamanho do banco
docker exec omnicare_postgres psql -U omnicare_user -c "SELECT pg_size_pretty(pg_database_size('omnicare_db'));"

# Limpar logs antigos
find /var/log/primecare -type f -mtime +30 -delete
```

---

## 📞 Suporte Técnico

### Contatos

**Equipe DevOps**:
- Email: devops@omnicaresoftware.com
- Urgências: +55 11 99999-9999 (plantão)

**Documentação Adicional**:
- Wiki: https://wiki.omnicaresoftware.com
- Runbooks: https://docs.omnicaresoftware.com/runbooks
- Status Page: https://status.omnicaresoftware.com

---

**Última atualização**: Janeiro 2026
**Versão do documento**: 1.0.0
