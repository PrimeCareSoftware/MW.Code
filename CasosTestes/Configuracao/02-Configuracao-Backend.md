# 02 - Configuração do Backend (.NET 8)

> **Objetivo:** Configurar e executar a API do PrimeCare Software  
> **Tempo estimado:** 10-15 minutos  
> **Pré-requisitos:** [01-Configuracao-Ambiente.md](01-Configuracao-Ambiente.md) completo

## 📋 Índice

1. [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
2. [Configuração Inicial](#configuração-inicial)
3. [Configurar Variáveis de Ambiente](#configurar-variáveis-de-ambiente)
4. [Compilar o Backend](#compilar-o-backend)
5. [Executar o Backend](#executar-o-backend)
6. [Verificação](#verificação)
7. [Próximos Passos](#próximos-passos)

## 🏗️ Visão Geral da Arquitetura

O backend do PrimeCare é construído com **Domain-Driven Design (DDD)** e possui a seguinte estrutura:

```
src/
├── MedicSoft.Api/              # API REST principal (porta 5000/5001)
├── MedicSoft.Application/      # Casos de uso e lógica de aplicação
├── MedicSoft.Domain/           # Entidades e regras de negócio
├── MedicSoft.Repository/       # Acesso a dados (Entity Framework)
├── MedicSoft.CrossCutting/     # Serviços compartilhados
├── MedicSoft.Analytics/        # Módulo de Analytics e BI
├── MedicSoft.ML/               # Machine Learning
└── MedicSoft.WhatsAppAgent/    # Integração WhatsApp

patient-portal-api/             # API do Portal do Paciente (porta 5100)
telemedicine/                   # Microserviço de Telemedicina (porta 5200)
system-admin/                   # Admin SPA (Angular)
```

### Principais Controllers (50+)

- **AuthController** - Autenticação, 2FA, JWT
- **AppointmentsController** - Agendamento de consultas
- **PatientsController** - Gestão de pacientes
- **DoctorsController** - Gestão de médicos
- **MedicalRecordsController** - Prontuários SOAP
- **PrescriptionsController** - Prescrições digitais
- **AnalyticsController** - Dashboards e relatórios
- **CRMController** - Gestão de relacionamento
- E muitos outros...

## ⚙️ Configuração Inicial

### 1. Clone o Repositório (se ainda não fez)

```bash
git clone https://github.com/PrimeCareSoftware/MW.Code.git
cd MW.Code
```

### 2. Restaurar Dependências NuGet

```bash
# Restaurar todos os pacotes do projeto
dotnet restore MedicWarehouse.sln
```

Isso irá baixar todas as dependências necessárias:
- Entity Framework Core 8.0
- AutoMapper
- FluentValidation
- JWT Authentication
- SignalR
- E outras bibliotecas

## 🔐 Configurar Variáveis de Ambiente

### 1. Copiar Arquivo de Exemplo

```bash
cp .env.example .env
```

### 2. Editar o Arquivo `.env`

Abra o arquivo `.env` e configure as seguintes variáveis:

```bash
# Configuração do Banco de Dados PostgreSQL
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_NAME=medicsoft_dev
DATABASE_USER=postgres
DATABASE_PASSWORD=sua_senha_aqui

# Configuração JWT
JWT_SECRET=sua_chave_secreta_minimo_32_caracteres_aqui
JWT_ISSUER=PrimeCare
JWT_AUDIENCE=PrimeCare-API
JWT_EXPIRATION_MINUTES=60

# Configuração de Email (para testes, use Mailtrap ou similar)
SMTP_HOST=smtp.mailtrap.io
SMTP_PORT=2525
SMTP_USER=seu_usuario
SMTP_PASSWORD=sua_senha
SMTP_FROM=noreply@primecare.com.br

# Configuração de Upload de Arquivos
UPLOAD_PATH=./uploads
MAX_FILE_SIZE_MB=10

# Ambiente
ASPNETCORE_ENVIRONMENT=Development

# URLs
API_BASE_URL=http://localhost:5000
FRONTEND_URL=http://localhost:4200
PATIENT_PORTAL_URL=http://localhost:5100

# Configuração de Logs
LOG_LEVEL=Information
```

### 3. Configurar appsettings.json

Edite `src/MedicSoft.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicsoft_dev;Username=postgres;Password=sua_senha"
  },
  "JwtSettings": {
    "Secret": "sua_chave_secreta_minimo_32_caracteres_aqui",
    "Issuer": "PrimeCare",
    "Audience": "PrimeCare-API",
    "ExpirationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CorsOrigins": [
    "http://localhost:4200",
    "http://localhost:5100",
    "http://localhost:3000"
  ]
}
```

## 🔨 Compilar o Backend

### Compilar Todos os Projetos

```bash
# Build completo da solução
dotnet build MedicWarehouse.sln --configuration Release
```

Ou para build apenas da API principal:

```bash
cd src/MedicSoft.Api
dotnet build
```

### Verificar Erros de Compilação

Se houver erros, verifique:
- ✅ Todas as dependências foram restauradas
- ✅ .NET 8 SDK está instalado
- ✅ Não há conflitos de versão de pacotes

## 🚀 Executar o Backend

### Opção 1: Executar API Principal

```bash
cd src/MedicSoft.Api
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Opção 2: Executar com Watch (Recarrega automaticamente)

```bash
cd src/MedicSoft.Api
dotnet watch run
```

### Opção 3: Executar Todos os Serviços

Para executar API principal + Portal do Paciente + Telemedicina:

```bash
# Terminal 1 - API Principal
cd src/MedicSoft.Api
dotnet run

# Terminal 2 - Portal do Paciente (em outro terminal)
cd patient-portal-api
dotnet run

# Terminal 3 - Telemedicina (em outro terminal)
cd telemedicine
dotnet run
```

### Opção 4: Usar Docker/Podman (Recomendado para QA)

```bash
# Se tiver Docker instalado
docker-compose up -d

# Se tiver Podman instalado
podman-compose up -d
```

## ✅ Verificação

### 1. Verificar se a API está Rodando

Abra o navegador e acesse:

```
http://localhost:5000/health
```

Resposta esperada:
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-02T18:48:11Z"
}
```

### 2. Acessar o Swagger UI

O Swagger UI fornece documentação interativa da API:

```
http://localhost:5000/swagger
```

Você deve ver a documentação completa com todos os endpoints disponíveis.

### 3. Testar Endpoint de Status

```bash
curl http://localhost:5000/api/status
```

Resposta esperada:
```json
{
  "version": "1.0",
  "environment": "Development",
  "database": "Connected",
  "timestamp": "2026-02-02T18:48:11Z"
}
```

### 4. Verificar Logs

Os logs aparecem no terminal onde você executou `dotnet run`. Verifique se não há erros críticos.

Exemplos de logs esperados:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Checklist de Verificação

- [ ] API compilou sem erros
- [ ] API está rodando (porta 5000/5001)
- [ ] Endpoint /health retorna "Healthy"
- [ ] Swagger UI está acessível
- [ ] Não há erros críticos nos logs
- [ ] Conexão com banco de dados estabelecida (será configurada no próximo passo)

## 🔧 Configurações Avançadas

### Habilitar Hot Reload

Para desenvolvimento mais rápido, habilite o hot reload:

```bash
dotnet watch run --non-interactive
```

### Executar Testes Unitários

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
cd tests/MedicSoft.Test
dotnet test

# Executar com cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html
```

### Depuração com VS Code

Crie `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/MedicSoft.Api/bin/Debug/net8.0/MedicSoft.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/MedicSoft.Api",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    }
  ]
}
```

## 🚨 Problemas Comuns

### Problema: Erro ao restaurar pacotes NuGet

**Solução:**
```bash
# Limpar cache do NuGet
dotnet nuget locals all --clear

# Restaurar novamente
dotnet restore
```

### Problema: Porta 5000 já está em uso

**Solução:**
```bash
# Windows - Encontrar e matar processo
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# macOS/Linux - Encontrar e matar processo
lsof -ti:5000 | xargs kill -9
```

Ou configure outra porta em `appsettings.json`:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5050"
      }
    }
  }
}
```

### Problema: Erro de conexão com banco de dados

**Solução:** Verifique se o PostgreSQL está rodando e a connection string está correta. Veja [04-Configuracao-Banco-Dados.md](04-Configuracao-Banco-Dados.md).

### Problema: Erro 401 ao testar endpoints protegidos

**Solução:** Isso é esperado. Endpoints protegidos requerem autenticação JWT. Você precisará fazer login primeiro para obter um token. Veja os cenários de teste em [../CenariosTestesQA/01-Testes-Autenticacao.md](../CenariosTestesQA/01-Testes-Autenticacao.md).

## 📚 Estrutura de Controllers

### Controllers Principais

| Controller | Endpoint Base | Descrição |
|-----------|---------------|-----------|
| AuthController | /api/auth | Login, registro, 2FA |
| PatientsController | /api/patients | CRUD de pacientes |
| DoctorsController | /api/doctors | CRUD de médicos |
| AppointmentsController | /api/appointments | Agendamento |
| MedicalRecordsController | /api/medical-records | Prontuários |
| PrescriptionsController | /api/prescriptions | Prescrições |
| AnalyticsController | /api/analytics | Dashboards |
| CRMController | /api/crm | CRM |
| LGPDController | /api/lgpd | Conformidade LGPD |

### Swagger Tags

Os endpoints estão organizados por tags no Swagger:
- 🔐 Authentication
- 👥 Patients
- ⚕️ Doctors
- 📅 Appointments
- 📋 Medical Records
- 💊 Prescriptions
- 📊 Analytics
- 🤝 CRM
- 🛡️ LGPD

## 📚 Documentação Adicional

- [Technical Documentation](../../TECHNICAL_DOCUMENTATION.md)
- [API Endpoint Guide](../../API_ENDPOINT_GUIDE.md)
- [Authentication Architecture](../../AUTHENTICATION_ARCHITECTURE.txt)
- [Business Rules](../../system-admin/docs/BUSINESS_RULES.md)

## ⏭️ Próximos Passos

Agora que o backend está configurado e rodando:

1. ✅ Backend configurado e rodando
2. ➡️ Vá para [03-Configuracao-Frontend.md](03-Configuracao-Frontend.md) para configurar o frontend Angular
3. Depois configure o banco de dados em [04-Configuracao-Banco-Dados.md](04-Configuracao-Banco-Dados.md)

---

**Dúvidas?** Consulte o [Swagger UI](http://localhost:5000/swagger) ou a documentação principal.
