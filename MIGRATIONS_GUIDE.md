# MedicWarehouse - Database Migrations Guide

## Visão Geral

Este documento descreve como executar todas as migrations do sistema MedicWarehouse na ordem correta para manter o banco de dados sincronizado durante o desenvolvimento.

## Arquitetura de Banco de Dados

O MedicWarehouse utiliza **PostgreSQL** como banco de dados principal e possui múltiplos contextos EF Core:

### 1. **Aplicação Principal** (`MedicSoftDbContext`)
- Localização: `src/MedicSoft.Api`
- Migrations: `src/MedicSoft.Repository/Migrations/PostgreSQL/`
- Descrição: Núcleo do sistema com tabelas principais (clínicas, usuários, pacientes, consultas, etc.)
- **Total de Migrations**: 7

### 2. **Patient Portal** (`PatientPortalDbContext`)
- Localização: `patient-portal-api/PatientPortal.Api`
- Migrations: `patient-portal-api/PatientPortal.Infrastructure/Migrations/`
- Descrição: Portal para acesso de pacientes
- **Total de Migrations**: 1

### 3. **Microserviços**

#### Auth (`AuthDbContext`)
- Localização: `microservices/auth/MedicSoft.Auth.Api`
- Migrations: `microservices/auth/MedicSoft.Auth.Api/Data/Migrations/`
- Descrição: Autenticação e gerenciamento de sessões
- **Total de Migrations**: 1

#### Appointments (`AppointmentsDbContext`)
- Localização: `microservices/appointments/MedicSoft.Appointments.Api`
- Migrations: `microservices/appointments/MedicSoft.Appointments.Api/Data/Migrations/`
- Descrição: Agendamentos e fila de espera
- **Total de Migrations**: 1

#### Patients (`PatientsDbContext`)
- Localização: `microservices/patients/MedicSoft.Patients.Api`
- Migrations: `microservices/patients/MedicSoft.Patients.Api/Data/Migrations/`
- Descrição: Gerenciamento de pacientes
- **Total de Migrations**: 1

#### Medical Records (`MedicalRecordsDbContext`)
- Localização: `microservices/medicalrecords/MedicSoft.MedicalRecords.Api`
- Migrations: `microservices/medicalrecords/MedicSoft.MedicalRecords.Api/Data/Migrations/`
- Descrição: Prontuários e dados clínicos
- **Total de Migrations**: 1

#### Billing (`BillingDbContext`)
- Localização: `microservices/billing/MedicSoft.Billing.Api`
- Migrations: `microservices/billing/MedicSoft.Billing.Api/Data/Migrations/`
- Descrição: Transações financeiras e faturamento
- **Total de Migrations**: 1

#### System Admin (`SystemAdminDbContext`)
- Localização: `microservices/systemadmin/MedicSoft.SystemAdmin.Api`
- Migrations: `microservices/systemadmin/MedicSoft.SystemAdmin.Api/Data/Migrations/`
- Descrição: Administração do sistema
- **Total de Migrations**: 1

### 4. **Telemedicine** (`TelemedicineDbContext`)
- Localização: `telemedicine/src/MedicSoft.Telemedicine.Api`
- Migrations: `telemedicine/src/MedicSoft.Telemedicine.Infrastructure/Persistence/Migrations/`
- Descrição: Plataforma de videoconsultas
- **Total de Migrations**: 1

## Scripts de Execução

### Executar Todas as Migrations

Para facilitar o processo de desenvolvimento, foram criados scripts que executam **todas as migrations na ordem correta**.

#### Linux/macOS (Bash)
```bash
# Usando connection string padrão
./run-all-migrations.sh

# Especificando connection string customizada
./run-all-migrations.sh "Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"

# Usando variável de ambiente
export DATABASE_CONNECTION_STRING="Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"
./run-all-migrations.sh
```

#### Windows (PowerShell)
```powershell
# Usando connection string padrão
.\run-all-migrations.ps1

# Especificando connection string customizada
.\run-all-migrations.ps1 -ConnectionString "Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"

# Usando variável de ambiente
$env:DATABASE_CONNECTION_STRING = "Host=localhost;Database=medicsoft;Username=postgres;Password=suasenha"
.\run-all-migrations.ps1
```

### Ordem de Execução

Os scripts aplicam as migrations na seguinte ordem:

1. **Main Application** - Base do sistema
2. **Patient Portal** - Portal do paciente
3. **Auth Microservice** - Autenticação
4. **Appointments Microservice** - Agendamentos
5. **Patients Microservice** - Pacientes
6. **Medical Records Microservice** - Prontuários
7. **Billing Microservice** - Faturamento
8. **System Admin Microservice** - Administração
9. **Telemedicine** - Videoconsultas

Esta ordem garante que todas as dependências sejam satisfeitas e evita conflitos.

## Executar Migrations Individualmente

Se necessário, você pode executar migrations de um contexto específico:

### Exemplo: Main Application
```bash
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext --connection "Host=localhost;Database=medicsoft;Username=postgres;Password=postgres"
```

### Exemplo: Patient Portal
```bash
cd patient-portal-api/PatientPortal.Api
dotnet ef database update --context PatientPortalDbContext --connection "Host=localhost;Database=medicsoft;Username=postgres;Password=postgres"
```

### Exemplo: Auth Microservice
```bash
cd microservices/auth/MedicSoft.Auth.Api
dotnet ef database update --context AuthDbContext --connection "Host=localhost;Database=medicsoft;Username=postgres;Password=postgres"
```

## Criar Novas Migrations

### Main Application
```bash
cd src/MedicSoft.Api
dotnet ef migrations add NomeDaMigration --context MedicSoftDbContext --output-dir ../MedicSoft.Repository/Migrations/PostgreSQL
```

### Patient Portal
```bash
cd patient-portal-api/PatientPortal.Api
dotnet ef migrations add NomeDaMigration --context PatientPortalDbContext --project ../PatientPortal.Infrastructure --output-dir Migrations
```

### Microserviços (Exemplo: Auth)
```bash
cd microservices/auth/MedicSoft.Auth.Api
dotnet ef migrations add NomeDaMigration --context AuthDbContext --output-dir Data/Migrations
```

### Telemedicine
```bash
cd telemedicine/src/MedicSoft.Telemedicine.Infrastructure
dotnet ef migrations add NomeDaMigration --startup-project ../MedicSoft.Telemedicine.Api --output-dir Persistence/Migrations
```

## Aplicação Automática de Migrations

### Aplicação Principal
A aplicação principal (`MedicSoft.Api`) aplica migrations **automaticamente** na inicialização através de:
```csharp
context.Database.Migrate();
```

### Microserviços
O microserviço de Auth também aplica migrations automaticamente na inicialização.

### Patient Portal & Telemedicine
Necessitam de execução manual das migrations via script ou comando EF Core.

## Connection String Padrão

Para desenvolvimento local, a connection string padrão é:
```
Host=localhost;Database=medicsoft;Username=postgres;Password=postgres
```

Esta connection string é utilizada quando nenhuma é fornecida aos scripts.

## Requisitos

- **.NET 8.0 SDK** ou superior
- **PostgreSQL** instalado e rodando
- **EF Core Tools** instalados:
  ```bash
  dotnet tool restore
  ```

## Troubleshooting

### Erro: "No migrations configuration type was found"
- Verifique se está no diretório correto do projeto
- Verifique se o projeto tem o pacote `Microsoft.EntityFrameworkCore.Design`

### Erro: "Unable to connect to database"
- Verifique se o PostgreSQL está rodando
- Verifique a connection string (host, porta, usuário, senha)
- Verifique se o banco de dados existe

### Migration já aplicada
- O EF Core é idempotente - executar migrations já aplicadas é seguro
- Nenhuma alteração será feita se a migration já foi aplicada

### Reverter uma migration
```bash
# Reverter última migration
dotnet ef database update NomeDaMigrationAnterior --context NomeDoContexto

# Remover migration (antes de aplicar)
dotnet ef migrations remove --context NomeDoContexto
```

## Referências

- [Documentação EF Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [MedicWarehouse README Principal](./README.md)
