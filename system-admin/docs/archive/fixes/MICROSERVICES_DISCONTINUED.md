# Omni Care Software Microservices - DESCONTINUADO

> ⚠️ **IMPORTANTE**: Os microserviços foram **descontinuados** em Janeiro de 2026.
> 
> **Todas as funcionalidades foram consolidadas na API principal** (`src/MedicSoft.Api`)
> 
> **Motivo da descontinuação**: Complexidade operacional desnecessária para o escopo atual do projeto. Todas as funcionalidades dos microserviços já existiam (ou existem no domínio) da API monolítica principal.

## 🚫 Microserviços Descontinuados

Os seguintes microserviços foram **removidos**:

| Service | Port | Status | Migração |
|---------|------|--------|----------|
| Auth | 5001 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |
| Patients | 5002 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |
| Appointments | 5003 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |
| MedicalRecords | 5004 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |
| Billing | 5005 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |
| SystemAdmin | 5006 | ❌ DELETADO | ✅ Funcionalidade completa na API principal |

## 🎯 API Principal (Monolítica)

Toda a funcionalidade está disponível na **API principal**:

- 📁 **Localização**: `src/MedicSoft.Api`
- 🌐 **URL Desenvolvimento**: `http://localhost:5000/api`
- 📖 **Swagger**: `http://localhost:5000/swagger`
- 🐳 **Docker**: Use `docker-compose.yml` (não mais `docker-compose.microservices.yml`)

## 📖 Funcionalidades Disponíveis na API Principal

Todas as funcionalidades dos microserviços estão disponíveis nos seguintes controllers:

### Autenticação e Autorização
- **AuthController** (`/api/auth`)
  - Login, Owner Login, Token Validation, Session Validation
  
### Gestão de Pacientes
- **PatientsController** (`/api/patients`)
  - CRUD completo, busca por documento, vinculação clínica/responsável

### Agendamentos
- **AppointmentsController** (`/api/appointments`)
  - Criação, cancelamento, agenda diária, slots disponíveis
  - ⚠️ Nota: CheckIn/CheckOut existem na entidade de domínio mas não estão expostos como endpoints

### Prontuários e Medicações
- **MedicalRecordsController** (`/api/medical-records`)
  - CRUD completo, completar prontuário
- **MedicationsController** (`/api/medications`)
  - CRUD completo, busca por categoria/termo

### Faturamento
- **SubscriptionsController** (`/api/subscriptions`)
- **PaymentsController** (`/api/payments`)
- **ExpensesController** (`/api/expenses`)
- **InvoicesController** (`/api/invoices`)

### Administração do Sistema
- **TenantController** (`/api/tenant`)
  - Resolução de subdomínios
- **SystemAdmin-related controllers**
  - Gerenciamento de clínicas, proprietários, tickets
  - Subscription override

## 🚀 Como Usar a API Principal

### Desenvolvimento Local
```bash
cd src/MedicSoft.Api
dotnet run
```

### Com Docker
```bash
docker-compose up -d
```

### Acessar Swagger
```
http://localhost:5000/swagger
```

## 📚 Documentação Adicional

- [Guia de Início Rápido](../docs/GUIA_INICIO_RAPIDO_LOCAL.md)
- [Resumo Técnico Completo](../docs/RESUMO_TECNICO_COMPLETO.md)
- [README Principal](../README.md)

---

**Data de Descontinuação**: Janeiro 2026  
**Motivo**: Consolidação na API monolítica para simplificar arquitetura e operações
