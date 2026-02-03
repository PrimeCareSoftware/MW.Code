# 📚 Guia Completo de APIs - Omni Care Software

> **Última Atualização:** Janeiro 2026  
> **Versão da API:** 2.0  
> **Base URL:** `http://localhost:5000/api` (desenvolvimento) | `https://api.mwsistema.com.br/api` (produção)

---

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Autenticação](#autenticação)
- [Headers Obrigatórios](#headers-obrigatórios)
- [Endpoints por Categoria](#endpoints-por-categoria)
- [Códigos de Status HTTP](#códigos-de-status-http)
- [Exemplos de Uso](#exemplos-de-uso)
- [Postman Collection](#postman-collection)

---

## 🎯 Visão Geral

A API do Omni Care Software é uma **API RESTful** completa que fornece acesso a todas as funcionalidades do sistema através de endpoints HTTP padronizados.

### Características

- ✅ **RESTful** - Segue princípios REST
- ✅ **JSON** - Comunicação em JSON
- ✅ **JWT** - Autenticação via token
- ✅ **Multi-tenant** - Isolamento por TenantId
- ✅ **Swagger** - Documentação interativa
- ✅ **Versionamento** - Suporte a múltiplas versões
- ✅ **Rate Limiting** - Proteção contra abuso
- ✅ **CORS** - Configuração cross-origin

### Tecnologias

- **.NET 8** - Framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT** - Autenticação
- **Swagger/OpenAPI** - Documentação

---

## 🔐 Autenticação

### Tipos de Login

A API suporta dois tipos de autenticação:

#### 1. Login de Usuários (Funcionários)
Para médicos, secretárias, enfermeiros, etc.

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "dr.silva",
  "password": "Doctor@123",
  "tenantId": "demo-clinic-001"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "uuid",
    "name": "Dr. João Silva",
    "username": "dr.silva",
    "email": "dr.silva@clinic.com",
    "role": "Doctor",
    "clinicId": "uuid",
    "tenantId": "demo-clinic-001"
  }
}
```

#### 2. Login de Proprietários (Owners)
Para donos de clínicas e administradores do sistema.

```http
POST /api/auth/owner-login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "system"
}
```

### Usar o Token

Após autenticação, inclua o token em todas as requisições:

```http
GET /api/patients
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Validar Token

```http
POST /api/auth/validate
Authorization: Bearer {token}
```

### Recuperação de Senha

#### Passo 1: Solicitar código de verificação
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "usuario@email.com",
  "method": "Email"  // ou "SMS"
}
```

#### Passo 2: Resetar senha com código
```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "usuario@email.com",
  "verificationCode": "123456",
  "newPassword": "NovaSenha@123"
}
```

---

## 📦 Headers Obrigatórios

### Em Todas as Requisições Autenticadas

```http
Authorization: Bearer {token}
Content-Type: application/json
```

### Opcional (Multi-tenant)

```http
X-Tenant-Id: demo-clinic-001
```

> **Nota:** O TenantId geralmente vem do token JWT, mas pode ser sobrescrito com este header.

---

## 📚 Endpoints por Categoria

### 1. Autenticação e Autorização

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Login de usuários | ❌ |
| POST | `/api/auth/owner-login` | Login de proprietários | ❌ |
| POST | `/api/auth/validate` | Validar token JWT | ✅ |
| POST | `/api/auth/forgot-password` | Solicitar recuperação de senha | ❌ |
| POST | `/api/auth/reset-password` | Resetar senha | ❌ |
| POST | `/api/auth/change-password` | Alterar senha | ✅ |
| POST | `/api/auth/logout` | Fazer logout | ✅ |

### 2. Registro e Configuração

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/registration` | Registrar nova clínica | ❌ |
| GET | `/api/registration/check-cnpj/{cnpj}` | Verificar disponibilidade de CNPJ | ❌ |
| GET | `/api/registration/check-username/{username}` | Verificar disponibilidade de username | ❌ |
| GET | `/api/registration/{id}` | Obter dados de registro | ✅ |

### 3. Pacientes

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/patients` | Listar pacientes (paginado) | ✅ |
| GET | `/api/patients/{id}` | Obter paciente por ID | ✅ |
| GET | `/api/patients/search?searchTerm={termo}` | Buscar por CPF, Nome ou Telefone | ✅ |
| GET | `/api/patients/by-document/{cpf}` | Buscar por CPF em todas clínicas | ✅ |
| POST | `/api/patients` | Criar novo paciente | ✅ |
| PUT | `/api/patients/{id}` | Atualizar paciente | ✅ |
| DELETE | `/api/patients/{id}` | Excluir paciente (soft delete) | ✅ |
| POST | `/api/patients/{patientId}/link-clinic/{clinicId}` | Vincular paciente à clínica | ✅ |
| POST | `/api/patients/{childId}/link-guardian/{guardianId}` | Vincular criança a responsável | ✅ |
| GET | `/api/patients/{guardianId}/children` | Listar filhos de um responsável | ✅ |
| GET | `/api/patients/{patientId}/history` | Histórico completo do paciente | ✅ |

### 4. Agendamentos

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/appointments` | Listar agendamentos | ✅ |
| GET | `/api/appointments/{id}` | Obter agendamento por ID | ✅ |
| GET | `/api/appointments/agenda` | Agenda diária | ✅ |
| GET | `/api/appointments/available-slots` | Horários disponíveis | ✅ |
| GET | `/api/appointments/calendar/{year}/{month}` | Visualização calendário mensal | ✅ |
| POST | `/api/appointments` | Criar agendamento | ✅ |
| PUT | `/api/appointments/{id}` | Atualizar agendamento | ✅ |
| PUT | `/api/appointments/{id}/cancel` | Cancelar agendamento | ✅ |
| PUT | `/api/appointments/{id}/confirm` | Confirmar agendamento | ✅ |
| PUT | `/api/appointments/{id}/check-in` | Fazer check-in | ✅ |
| PUT | `/api/appointments/{id}/start` | Iniciar atendimento | ✅ |
| PUT | `/api/appointments/{id}/complete` | Completar atendimento | ✅ |

### 5. Prontuários Médicos

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/medical-records` | Listar prontuários | ✅ |
| GET | `/api/medical-records/{id}` | Obter prontuário por ID | ✅ |
| GET | `/api/medical-records/appointment/{appointmentId}` | Buscar por agendamento | ✅ |
| GET | `/api/medical-records/patient/{patientId}` | Histórico do paciente | ✅ |
| POST | `/api/medical-records` | Criar prontuário | ✅ |
| PUT | `/api/medical-records/{id}` | Atualizar prontuário | ✅ |
| POST | `/api/medical-records/{id}/complete` | Finalizar atendimento | ✅ |
| DELETE | `/api/medical-records/{id}` | Excluir prontuário (soft delete) | ✅ |

### 6. Hipóteses Diagnósticas (CID-10)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/diagnostic-hypotheses` | Listar diagnósticos | ✅ |
| GET | `/api/diagnostic-hypotheses/{id}` | Obter diagnóstico por ID | ✅ |
| GET | `/api/diagnostic-hypotheses/medical-record/{recordId}` | Diagnósticos de um prontuário | ✅ |
| POST | `/api/diagnostic-hypotheses` | Criar diagnóstico | ✅ |
| PUT | `/api/diagnostic-hypotheses/{id}` | Atualizar diagnóstico | ✅ |
| DELETE | `/api/diagnostic-hypotheses/{id}` | Excluir diagnóstico | ✅ |

### 7. Consentimento Informado

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/informed-consents` | Listar consentimentos | ✅ |
| GET | `/api/informed-consents/{id}` | Obter consentimento por ID | ✅ |
| GET | `/api/informed-consents/patient/{patientId}` | Consentimentos do paciente | ✅ |
| POST | `/api/informed-consents` | Criar consentimento | ✅ |
| PUT | `/api/informed-consents/{id}/sign` | Assinar digitalmente | ✅ |

### 8. Procedimentos e Serviços

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/procedures` | Listar procedimentos | ✅ |
| GET | `/api/procedures/{id}` | Obter procedimento por ID | ✅ |
| GET | `/api/procedures/category/{category}` | Listar por categoria | ✅ |
| POST | `/api/procedures` | Criar procedimento | ✅ |
| PUT | `/api/procedures/{id}` | Atualizar procedimento | ✅ |
| DELETE | `/api/procedures/{id}` | Desativar procedimento | ✅ |
| POST | `/api/procedures/appointments/{id}/procedures` | Adicionar ao atendimento | ✅ |
| GET | `/api/procedures/appointments/{id}/procedures` | Listar procedimentos do atendimento | ✅ |
| GET | `/api/procedures/appointments/{id}/billing-summary` | Resumo de cobrança | ✅ |

### 9. Medicações (Autocomplete)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/medications` | Listar medicações | ✅ |
| GET | `/api/medications/search?term={termo}` | Busca para autocomplete | ✅ |
| GET | `/api/medications/{id}` | Obter medicação por ID | ✅ |
| GET | `/api/medications/category/{category}` | Listar por categoria | ✅ |
| POST | `/api/medications` | Criar medicação | ✅ |
| PUT | `/api/medications/{id}` | Atualizar medicação | ✅ |
| DELETE | `/api/medications/{id}` | Desativar medicação | ✅ |

### 10. Catálogo de Exames (Autocomplete)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/exam-catalog` | Listar exames | ✅ |
| GET | `/api/exam-catalog/search?term={termo}` | Busca para autocomplete | ✅ |
| GET | `/api/exam-catalog/{id}` | Obter exame por ID | ✅ |
| GET | `/api/exam-catalog/type/{examType}` | Listar por tipo | ✅ |
| GET | `/api/exam-catalog/category/{category}` | Listar por categoria | ✅ |
| POST | `/api/exam-catalog` | Criar exame | ✅ |
| PUT | `/api/exam-catalog/{id}` | Atualizar exame | ✅ |
| DELETE | `/api/exam-catalog/{id}` | Desativar exame | ✅ |

### 11. Prescrições Digitais

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/digital-prescriptions` | Listar prescrições | ✅ |
| GET | `/api/digital-prescriptions/{id}` | Obter prescrição por ID | ✅ |
| GET | `/api/digital-prescriptions/patient/{patientId}` | Prescrições do paciente | ✅ |
| POST | `/api/digital-prescriptions` | Criar prescrição | ✅ |
| PUT | `/api/digital-prescriptions/{id}` | Atualizar prescrição | ✅ |
| GET | `/api/digital-prescriptions/{id}/pdf` | Gerar PDF | ✅ |

### 12. Pagamentos

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/payments` | Listar pagamentos | ✅ |
| GET | `/api/payments/{id}` | Obter pagamento por ID | ✅ |
| POST | `/api/payments` | Criar pagamento | ✅ |
| PUT | `/api/payments/process` | Processar pagamento | ✅ |
| PUT | `/api/payments/{id}/refund` | Reembolsar | ✅ |
| PUT | `/api/payments/{id}/cancel` | Cancelar | ✅ |

### 13. Notas Fiscais

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/invoices` | Listar notas fiscais | ✅ |
| GET | `/api/invoices/{id}` | Obter nota por ID | ✅ |
| POST | `/api/invoices` | Criar nota fiscal | ✅ |
| PUT | `/api/invoices/{id}` | Atualizar nota | ✅ |
| PUT | `/api/invoices/{id}/issue` | Emitir nota | ✅ |
| PUT | `/api/invoices/{id}/send` | Enviar por email | ✅ |
| PUT | `/api/invoices/{id}/cancel` | Cancelar nota | ✅ |

### 14. Despesas (Contas a Pagar)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/expenses` | Listar despesas | ✅ |
| GET | `/api/expenses/{id}` | Obter despesa por ID | ✅ |
| POST | `/api/expenses` | Criar despesa | ✅ |
| PUT | `/api/expenses/{id}` | Atualizar despesa | ✅ |
| PUT | `/api/expenses/{id}/pay` | Marcar como paga | ✅ |
| PUT | `/api/expenses/{id}/cancel` | Cancelar despesa | ✅ |
| DELETE | `/api/expenses/{id}` | Excluir despesa | ✅ |

### 15. Relatórios e Dashboards

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/reports/financial-summary` | Resumo financeiro completo | ✅ |
| GET | `/api/reports/revenue` | Relatório de receita | ✅ |
| GET | `/api/reports/appointments` | Relatório de agendamentos | ✅ |
| GET | `/api/reports/patients` | Relatório de pacientes | ✅ |
| GET | `/api/reports/accounts-receivable` | Contas a receber | ✅ |
| GET | `/api/reports/accounts-payable` | Contas a pagar | ✅ |

### 16. Notificações

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/notifications` | Listar notificações | ✅ |
| GET | `/api/notifications/{id}` | Obter notificação por ID | ✅ |
| POST | `/api/notifications/send` | Enviar notificação | ✅ |
| PUT | `/api/notifications/{id}/mark-read` | Marcar como lida | ✅ |

### 17. Rotinas de Notificação

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/notificationroutines` | Listar rotinas ativas | ✅ |
| GET | `/api/notificationroutines/{id}` | Obter rotina por ID | ✅ |
| POST | `/api/notificationroutines` | Criar rotina | ✅ |
| PUT | `/api/notificationroutines/{id}` | Atualizar rotina | ✅ |
| DELETE | `/api/notificationroutines/{id}` | Excluir rotina | ✅ |
| PUT | `/api/notificationroutines/{id}/activate` | Ativar rotina | ✅ |
| PUT | `/api/notificationroutines/{id}/deactivate` | Desativar rotina | ✅ |

### 18. Fila de Espera

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/waiting-queue` | Listar fila de espera | ✅ |
| GET | `/api/waiting-queue/{id}` | Obter item por ID | ✅ |
| POST | `/api/waiting-queue` | Adicionar à fila | ✅ |
| PUT | `/api/waiting-queue/{id}/call` | Chamar paciente | ✅ |
| PUT | `/api/waiting-queue/{id}/complete` | Completar atendimento | ✅ |
| PUT | `/api/waiting-queue/{id}/cancel` | Cancelar | ✅ |

### 19. Tickets de Suporte

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/tickets` | Listar tickets | ✅ |
| GET | `/api/tickets/{id}` | Obter ticket por ID | ✅ |
| POST | `/api/tickets` | Criar ticket | ✅ |
| PUT | `/api/tickets/{id}` | Atualizar ticket | ✅ |
| POST | `/api/tickets/{id}/comments` | Adicionar comentário | ✅ |
| PUT | `/api/tickets/{id}/assign` | Atribuir para owner | ✅ |
| PUT | `/api/tickets/{id}/close` | Fechar ticket | ✅ |
| POST | `/api/tickets/{id}/attachments` | Adicionar anexo | ✅ |
| GET | `/api/tickets/statistics` | Estatísticas de tickets | ✅ |

### 20. Clínicas (System Owner)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/clinics` | Listar clínicas | ✅ |
| GET | `/api/clinics/{id}` | Obter clínica por ID | ✅ |
| POST | `/api/clinics` | Criar clínica | ✅ |
| PUT | `/api/clinics/{id}` | Atualizar clínica | ✅ |
| PUT | `/api/clinics/{id}/activate` | Ativar clínica | ✅ |
| PUT | `/api/clinics/{id}/deactivate` | Desativar clínica | ✅ |

### 21. Assinaturas

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/subscriptions` | Listar assinaturas | ✅ |
| GET | `/api/subscriptions/{id}` | Obter assinatura por ID | ✅ |
| POST | `/api/subscriptions/upgrade` | Fazer upgrade | ✅ |
| POST | `/api/subscriptions/downgrade` | Fazer downgrade | ✅ |
| POST | `/api/subscriptions/freeze` | Congelar plano | ✅ |
| POST | `/api/subscriptions/reactivate` | Reativar assinatura | ✅ |

### 22. Módulos de Configuração

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/module-config` | Listar configurações | ✅ |
| GET | `/api/module-config/{id}` | Obter configuração por ID | ✅ |
| POST | `/api/module-config` | Criar configuração | ✅ |
| PUT | `/api/module-config/{id}` | Atualizar configuração | ✅ |
| PUT | `/api/module-config/{id}/toggle` | Habilitar/desabilitar módulo | ✅ |

### 23. Perfis de Acesso

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/access-profiles` | Listar perfis | ✅ |
| GET | `/api/access-profiles/{id}` | Obter perfil por ID | ✅ |
| POST | `/api/access-profiles` | Criar perfil | ✅ |
| PUT | `/api/access-profiles/{id}` | Atualizar perfil | ✅ |
| DELETE | `/api/access-profiles/{id}` | Excluir perfil | ✅ |

### 24. Data Seeding (Desenvolvimento)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/data-seeder/demo-info` | Informações sobre dados demo | ❌ |
| POST | `/api/data-seeder/seed-demo` | Gerar dados de teste completos | ❌ |
| POST | `/api/data-seeder/seed-system-owner` | Criar system owner | ❌ |
| DELETE | `/api/data-seeder/clear-database` | Limpar dados demo | ❌ |

### 25. Contato (Site)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/contact` | Enviar mensagem de contato | ❌ |

---

## 🔢 Códigos de Status HTTP

### Sucesso (2xx)

| Código | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Requisição bem-sucedida (GET, PUT) |
| 201 | Created | Recurso criado com sucesso (POST) |
| 204 | No Content | Sucesso sem retorno de corpo (DELETE) |

### Erro do Cliente (4xx)

| Código | Significado | Uso |
|--------|-------------|-----|
| 400 | Bad Request | Dados inválidos na requisição |
| 401 | Unauthorized | Token ausente ou inválido |
| 403 | Forbidden | Sem permissão para acessar recurso |
| 404 | Not Found | Recurso não encontrado |
| 409 | Conflict | Conflito (ex: CPF duplicado) |
| 422 | Unprocessable Entity | Validação de negócio falhou |
| 429 | Too Many Requests | Rate limit excedido |

### Erro do Servidor (5xx)

| Código | Significado | Uso |
|--------|-------------|-----|
| 500 | Internal Server Error | Erro não tratado no servidor |
| 503 | Service Unavailable | Servidor temporariamente indisponível |

---

## 💡 Exemplos de Uso

### Exemplo 1: Fluxo Completo de Autenticação e Listagem

```bash
# 1. Fazer login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "dr.silva",
    "password": "Doctor@123",
    "tenantId": "demo-clinic-001"
  }'

# Resposta: {"token": "eyJhbGc...", ...}

# 2. Listar pacientes usando o token
curl -X GET http://localhost:5000/api/patients \
  -H "Authorization: Bearer eyJhbGc..."
```

### Exemplo 2: Criar e Agendar Consulta

```bash
# 1. Buscar paciente por CPF
curl -X GET "http://localhost:5000/api/patients/search?searchTerm=12345678901" \
  -H "Authorization: Bearer {token}"

# 2. Criar agendamento
curl -X POST http://localhost:5000/api/appointments \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "uuid-do-paciente",
    "doctorId": "uuid-do-medico",
    "appointmentDate": "2026-01-15T10:00:00",
    "appointmentType": "Consulta",
    "duration": 30,
    "notes": "Consulta de rotina"
  }'
```

### Exemplo 3: Criar Prontuário com Prescrição

```bash
curl -X POST http://localhost:5000/api/medical-records \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "appointmentId": "uuid-do-agendamento",
    "chiefComplaint": "Dor de cabeça há 3 dias",
    "historyOfPresentIllness": "Paciente relata cefaleia frontal...",
    "bloodPressureSystolic": 120,
    "bloodPressureDiastolic": 80,
    "heartRate": 72,
    "diagnosis": "Cefaleia tensional",
    "treatment": "Prescrição de analgésico",
    "prescription": "Paracetamol 500mg - 1 comp a cada 6h",
    "returnDate": "2026-01-30"
  }'
```

### Exemplo 4: Gerar Relatório Financeiro

```bash
curl -X GET "http://localhost:5000/api/reports/financial-summary?startDate=2026-01-01&endDate=2026-01-31" \
  -H "Authorization: Bearer {token}"
```

---

## 📮 Postman Collection

Para facilitar os testes, importa a coleção completa do Postman:

**Arquivo:** `Omni Care Software-Postman-Collection.json` (na raiz do projeto)

### Como usar:

1. Abra o Postman
2. Clique em "Import"
3. Selecione o arquivo `Omni Care Software-Postman-Collection.json`
4. Configure as variáveis:
   - `base_url`: `http://localhost:5000/api`
   - `token`: (será preenchido automaticamente após login)
   - `tenant_id`: `demo-clinic-001`

**Guia completo:** [POSTMAN_IMPORT_GUIDE.md](./POSTMAN_IMPORT_GUIDE.md)

---

## 📖 Documentação Interativa

### Swagger UI

Acesse a documentação interativa Swagger:

- **Desenvolvimento:** http://localhost:5000/swagger
- **Produção:** https://api.mwsistema.com.br/swagger

No Swagger você pode:
- ✅ Ver todos os endpoints
- ✅ Testar requisições
- ✅ Ver schemas de dados
- ✅ Copiar exemplos de código

---

## 🔗 Links Úteis

- [README Principal](../README.md)
- [Resumo Técnico Completo](./RESUMO_TECNICO_COMPLETO.md)
- [Guia de Autenticação](./AUTHENTICATION_GUIDE.md)
- [Guia de Seeders](./SEEDER_GUIDE.md)
- [Índice de Documentação](./DOCUMENTATION_INDEX.md)

---

## 📞 Suporte

- **GitHub Issues:** https://github.com/Omni Care Software/MW.Code/issues
- **Email:** contato@omnicaresoftware.com
- **Documentação:** https://github.com/Omni Care Software/MW.Code/tree/main/docs

---

**Documento atualizado em:** Janeiro 2026  
**Versão:** 2.0
