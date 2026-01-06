# 🧪 Guia Completo de Testes Passo a Passo - MedicWarehouse

Este guia fornece instruções detalhadas para testar cada tela e API do sistema MedicWarehouse no seu computador local.

## 📋 Índice

1. [Preparação do Ambiente](#preparação-do-ambiente)
2. [Teste 1: Cadastrar Clínica e Usuário Owner](#teste-1-cadastrar-clínica-e-usuário-owner)
3. [Teste 2: Login do Owner](#teste-2-login-do-owner)
4. [Teste 3: Cadastrar Usuários Adicionais](#teste-3-cadastrar-usuários-adicionais)
5. [Teste 4: Cadastrar Pacientes](#teste-4-cadastrar-pacientes)
6. [Teste 5: Cadastrar Procedimentos](#teste-5-cadastrar-procedimentos)
7. [Teste 6: Criar Agendamentos](#teste-6-criar-agendamentos)
8. [Teste 7: Realizar Atendimento](#teste-7-realizar-atendimento)
9. [Teste 8: Gerenciar Prontuários](#teste-8-gerenciar-prontuários)
10. [Teste 9: Processar Pagamentos](#teste-9-processar-pagamentos)
11. [Teste 10: Visualizar Relatórios](#teste-10-visualizar-relatórios)
12. [Teste 11: Gerenciar Assinatura](#teste-11-gerenciar-assinatura)
13. [Teste 12: Configurar Notificações](#teste-12-configurar-notificações)
14. [Teste 13: Telas do Frontend](#teste-13-telas-do-frontend)
15. [Troubleshooting](#troubleshooting)

---

## Preparação do Ambiente

### 1. Configurar Banco de Dados

```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Api
dotnet ef database update
```

### 2. Iniciar a API

```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Api
dotnet run
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### 3. Verificar Planos de Assinatura

Os planos devem estar pré-configurados no banco. Para verificar:

```bash
# Abra o Swagger: https://localhost:5001/swagger
# Ou use o endpoint diretamente
curl http://localhost:5000/api/system-admin/plans
```

### 4. Ferramentas Necessárias

- **Swagger UI**: https://localhost:5001/swagger (já incluído na API)
- **Postman** (opcional): Para testes mais complexos
- **curl** ou **httpie**: Para testes via linha de comando

---

## Teste 1: Cadastrar Clínica e Usuário Owner

Este é o **primeiro e mais importante teste**. Ele cria a clínica e o usuário Owner simultaneamente.

### 📍 Endpoint

```
POST /api/registration
```

### 🔓 Autenticação

**NÃO REQUER** autenticação (endpoint público)

### 📝 Dados de Teste

```json
{
  "clinicName": "Clínica Teste",
  "clinicCNPJ": "12345678000195",
  "clinicPhone": "(11) 98765-4321",
  "clinicEmail": "contato@clinicateste.com",
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Sala 10",
  "neighborhood": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567",
  "ownerName": "Dr. João Silva",
  "ownerCPF": "12345678901",
  "ownerPhone": "(11) 99999-8888",
  "ownerEmail": "joao@clinicateste.com",
  "username": "joao.silva",
  "password": "MedicWare2024!@#",
  "planId": "usar-guid-do-plano-trial",
  "acceptTerms": true,
  "useTrial": true
}
```

### 🎯 Como Testar no Swagger

1. Abra https://localhost:5001/swagger
2. Localize o endpoint `POST /api/registration`
3. Clique em "Try it out"
4. Cole os dados de teste no corpo da requisição
5. **IMPORTANTE**: Substitua `"usar-guid-do-plano-trial"` pelo GUID real do plano Trial
   - Para obter: Execute `GET /api/system-admin/plans` primeiro
   - Copie o `id` do plano "Trial"
6. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "success": true,
  "message": "Registration successful! Welcome to MedicWarehouse. You can now login with your credentials.",
  "clinicId": "guid-da-clinica-criada",
  "userId": "guid-do-owner-criado"
}
```

### 💾 Importante: Guardar Informações

**ANOTE ESTES VALORES** para os próximos testes:
- ✏️ `clinicId`: Este será seu `tenantId`
- ✏️ `username`: joao.silva
- ✏️ `password`: MedicWare2024!@#

### 🐛 Possíveis Erros

| Erro | Causa | Solução |
|------|-------|---------|
| `CNPJ already registered` | CNPJ já existe no banco | Use outro CNPJ (ex: 98765432000195) |
| `Username already taken` | Username já existe | Use outro username (ex: joao.silva2) |
| `Password validation failed` | Senha não atende requisitos | Use senha forte: MedicWare2024!@# |
| `Plan not found` | PlanId inválido | Execute GET /api/system-admin/plans e copie o GUID correto |

---

## Teste 2: Login do Owner

Após criar a clínica e o owner, precisamos fazer login para obter o token JWT.

### 📍 Endpoint

```
POST /api/auth/login
```

### 🔓 Autenticação

**NÃO REQUER** autenticação (endpoint público de login)

### 📝 Dados de Teste

```json
{
  "username": "joao.silva",
  "password": "MedicWare2024!@#",
  "tenantId": "clinicId-do-teste-1"
}
```

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/auth/login`
2. Clique em "Try it out"
3. Cole os dados de teste
4. **IMPORTANTE**: Substitua `"clinicId-do-teste-1"` pelo `clinicId` que você anotou no Teste 1
5. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "joao.silva",
  "tenantId": "guid-da-clinica",
  "role": "ClinicOwner",
  "userId": "guid-do-owner",
  "clinicId": "guid-da-clinica",
  "expiresAt": "2024-01-15T11:30:00Z"
}
```

### 💾 Importante: Guardar o Token

**COPIE E GUARDE O TOKEN JWT** - você vai precisar dele em TODOS os próximos testes!

### 🔑 Configurar Autenticação no Swagger

1. No topo da página do Swagger, clique no botão **"Authorize" 🔒**
2. No campo de valor, digite: `Bearer {seu-token-jwt}`
   - Exemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Clique em "Authorize"
4. Clique em "Close"

Agora TODOS os endpoints protegidos funcionarão automaticamente!

### 🐛 Possíveis Erros

| Erro | Causa | Solução |
|------|-------|---------|
| `Invalid credentials` | Username, senha ou tenantId incorretos | Verifique os dados do Teste 1 |
| `User is not active` | Usuário foi desativado | Reative o usuário ou crie outro |
| `Clinic not found` | TenantId inválido | Use o clinicId correto do Teste 1 |

---

## Teste 3: Cadastrar Usuários Adicionais

Agora que você está autenticado como Owner, pode criar outros usuários (médicos, enfermeiros, recepcionistas).

### 📍 Endpoint

```
POST /api/users
```

### 🔒 Autenticação

**REQUER** autenticação (use o token do Teste 2)

### 📝 Dados de Teste - Médico

```json
{
  "username": "dra.maria",
  "email": "maria@clinicateste.com",
  "password": "MedicWare2024!@#",
  "fullName": "Dra. Maria Santos",
  "phone": "(11) 97777-6666",
  "role": "Doctor",
  "professionalId": "CRM 12345-SP",
  "specialty": "Cardiologia"
}
```

### 📝 Dados de Teste - Recepcionista

```json
{
  "username": "ana.recep",
  "email": "ana@clinicateste.com",
  "password": "MedicWare2024!@#",
  "fullName": "Ana Oliveira",
  "phone": "(11) 96666-5555",
  "role": "Receptionist",
  "professionalId": null,
  "specialty": null
}
```

### 🎯 Como Testar no Swagger

1. Certifique-se de que configurou a autenticação (botão Authorize)
2. Localize o endpoint `POST /api/users`
3. Clique em "Try it out"
4. Cole os dados de teste
5. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-usuario",
  "username": "dra.maria",
  "email": "maria@clinicateste.com",
  "fullName": "Dra. Maria Santos",
  "phone": "(11) 97777-6666",
  "role": "Doctor",
  "professionalId": "CRM 12345-SP",
  "specialty": "Cardiologia",
  "isActive": true,
  "tenantId": "guid-da-clinica"
}
```

### 📊 Roles Disponíveis

- `ClinicOwner` - Proprietário da clínica (você)
- `Doctor` - Médico
- `Dentist` - Dentista
- `Nurse` - Enfermeiro
- `Receptionist` - Recepcionista
- `Secretary` - Secretário

### 🔍 Listar Todos os Usuários

```
GET /api/users
```

Teste este endpoint para ver todos os usuários da sua clínica.

### 🐛 Possíveis Erros

| Erro | Causa | Solução |
|------|-------|---------|
| `401 Unauthorized` | Token não configurado | Configure no botão Authorize |
| `Username already taken` | Username já existe na clínica | Use outro username |
| `Email already registered` | Email já existe na clínica | Use outro email |

---

## Teste 4: Cadastrar Pacientes

Vamos cadastrar pacientes para poder criar agendamentos depois.

### 📍 Endpoint

```
POST /api/patients
```

### 🔒 Autenticação

**REQUER** autenticação

### 📝 Dados de Teste - Paciente Adulto

```json
{
  "fullName": "Carlos Alberto Santos",
  "cpf": "11122233344",
  "rg": "123456789",
  "birthDate": "1985-05-15",
  "gender": "Male",
  "email": "carlos@email.com",
  "phone": "(11) 95555-4444",
  "street": "Av. Paulista",
  "number": "1000",
  "complement": "Apto 101",
  "neighborhood": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01310-100",
  "emergencyContact": "(11) 94444-3333",
  "observations": "Paciente hipertenso, usa medicamento contínuo"
}
```

### 📝 Dados de Teste - Paciente Criança

```json
{
  "fullName": "Joana Silva Santos",
  "cpf": "55566677788",
  "rg": "987654321",
  "birthDate": "2018-03-20",
  "gender": "Female",
  "email": null,
  "phone": "(11) 93333-2222",
  "street": "Rua das Acácias",
  "number": "456",
  "complement": null,
  "neighborhood": "Jardim Paulista",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01405-000",
  "emergencyContact": "(11) 92222-1111",
  "observations": "Menor de idade"
}
```

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/patients`
2. Clique em "Try it out"
3. Cole os dados de teste
4. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-paciente",
  "fullName": "Carlos Alberto Santos",
  "cpf": "11122233344",
  "rg": "123456789",
  "birthDate": "1985-05-15",
  "age": 39,
  "gender": "Male",
  "email": "carlos@email.com",
  "phone": "(11) 95555-4444",
  "address": {
    "street": "Av. Paulista",
    "number": "1000",
    "complement": "Apto 101",
    "neighborhood": "Bela Vista",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01310-100"
  },
  "emergencyContact": "(11) 94444-3333",
  "observations": "Paciente hipertenso, usa medicamento contínuo",
  "isActive": true,
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

### 🔍 Endpoints Adicionais de Pacientes

```
GET /api/patients                          - Lista todos os pacientes
GET /api/patients/{id}                     - Busca paciente por ID
GET /api/patients/search?name=carlos       - Busca por nome
GET /api/patients/by-document/{cpf}        - Busca por CPF
PUT /api/patients/{id}                     - Atualiza paciente
DELETE /api/patients/{id}                  - Deleta paciente (soft delete)
```

### 👨‍👩‍👧 Vincular Responsável e Criança

Para vincular um responsável a uma criança:

```
POST /api/patients/{childId}/link-guardian/{guardianId}
```

Exemplo: Se Carlos (guid-123) é responsável por Joana (guid-456):
```
POST /api/patients/guid-456/link-guardian/guid-123
```

### 🐛 Possíveis Erros

| Erro | Causa | Solução |
|------|-------|---------|
| `CPF already registered` | CPF já existe | Use outro CPF de teste |
| `Invalid CPF format` | CPF inválido | Use 11 dígitos numéricos |
| `BirthDate cannot be in the future` | Data futura | Use data passada |

---

## Teste 5: Cadastrar Procedimentos

Cadastre os procedimentos/tratamentos oferecidos pela clínica.

### 📍 Endpoint

```
POST /api/procedures
```

### 🔒 Autenticação

**REQUER** autenticação

### 📝 Dados de Teste - Consulta

```json
{
  "name": "Consulta Cardiológica",
  "description": "Consulta completa com ECG",
  "price": 250.00,
  "duration": 60,
  "category": "Consultation"
}
```

### 📝 Dados de Teste - Exame

```json
{
  "name": "Ecocardiograma",
  "description": "Exame de ultrassom do coração",
  "price": 450.00,
  "duration": 30,
  "category": "Exam"
}
```

### 📝 Dados de Teste - Tratamento

```json
{
  "name": "Holter 24 horas",
  "description": "Monitoramento cardíaco por 24h",
  "price": 380.00,
  "duration": 15,
  "category": "Treatment"
}
```

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/procedures`
2. Clique em "Try it out"
3. Cole os dados de teste
4. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-procedimento",
  "name": "Consulta Cardiológica",
  "description": "Consulta completa com ECG",
  "price": 250.00,
  "duration": 60,
  "category": "Consultation",
  "isActive": true,
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

### 📊 Categorias Disponíveis

- `Consultation` - Consulta
- `Exam` - Exame
- `Treatment` - Tratamento
- `Surgery` - Cirurgia
- `Therapy` - Terapia

### 🔍 Endpoints Adicionais de Procedimentos

```
GET /api/procedures                        - Lista todos os procedimentos
GET /api/procedures/{id}                   - Busca procedimento por ID
PUT /api/procedures/{id}                   - Atualiza procedimento
DELETE /api/procedures/{id}                - Deleta procedimento
```

---

## Teste 6: Criar Agendamentos

Agora vamos criar agendamentos combinando pacientes, usuários (médicos) e procedimentos.

### 📍 Endpoint

```
POST /api/appointments
```

### 🔒 Autenticação

**REQUER** autenticação

### 📝 Dados de Teste

```json
{
  "patientId": "guid-do-paciente-carlos",
  "userId": "guid-da-dra-maria",
  "procedureId": "guid-consulta-cardiologica",
  "appointmentDate": "2024-01-20T14:00:00Z",
  "observations": "Primeira consulta - Paciente com histórico de hipertensão"
}
```

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/appointments`
2. Clique em "Try it out"
3. Cole os dados de teste
4. **IMPORTANTE**: Substitua os GUIDs pelos IDs reais que você criou nos testes anteriores:
   - `patientId`: Do Teste 4 (Carlos)
   - `userId`: Do Teste 3 (Dra. Maria)
   - `procedureId`: Do Teste 5 (Consulta Cardiológica)
5. Ajuste a data para uma data futura
6. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-agendamento",
  "patientId": "guid-do-paciente",
  "patientName": "Carlos Alberto Santos",
  "userId": "guid-do-usuario",
  "userName": "Dra. Maria Santos",
  "procedureId": "guid-do-procedimento",
  "procedureName": "Consulta Cardiológica",
  "appointmentDate": "2024-01-20T14:00:00Z",
  "status": "Scheduled",
  "observations": "Primeira consulta - Paciente com histórico de hipertensão",
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

### 📊 Status de Agendamento

- `Scheduled` - Agendado
- `Confirmed` - Confirmado
- `InProgress` - Em andamento
- `Completed` - Concluído
- `Cancelled` - Cancelado
- `NoShow` - Paciente não compareceu

### 🔍 Endpoints Adicionais de Agendamentos

```
GET /api/appointments/agenda                       - Agenda do dia
GET /api/appointments/{id}                         - Busca agendamento por ID
GET /api/appointments/available-slots              - Horários disponíveis
PUT /api/appointments/{id}/cancel                  - Cancela agendamento
```

### 📅 Verificar Horários Disponíveis

Antes de criar um agendamento, você pode verificar os horários disponíveis:

```
GET /api/appointments/available-slots?date=2024-01-20&userId={guid-do-medico}
```

---

## Teste 7: Realizar Atendimento

Durante ou após o atendimento, o médico registra o prontuário.

### 📍 Endpoint

```
POST /api/medical-records
```

### 🔒 Autenticação

**REQUER** autenticação (médico ou profissional de saúde)

### 📝 Dados de Teste

```json
{
  "appointmentId": "guid-do-agendamento",
  "patientId": "guid-do-paciente-carlos",
  "chiefComplaint": "Dor no peito e falta de ar",
  "historyOfPresentIllness": "Paciente relata dor torácica há 3 dias, especialmente ao fazer esforço físico.",
  "physicalExamination": "PA: 140/90 mmHg, FC: 88 bpm, FR: 16 rpm. Ausculta cardíaca: ritmo regular, bulhas normofonéticas.",
  "diagnosis": "Hipertensão arterial sistêmica não controlada. Investigar doença coronariana.",
  "treatment": "Iniciado Losartana 50mg 1x/dia. Solicitado ECG e teste ergométrico.",
  "prescriptions": "Losartana 50mg - 1 comprimido pela manhã - 30 dias",
  "followUpInstructions": "Retornar em 15 dias com resultados dos exames. Orientado sobre dieta hipossódica e atividade física leve."
}
```

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/medical-records`
2. Clique em "Try it out"
3. Cole os dados de teste
4. **IMPORTANTE**: Substitua os GUIDs pelos IDs reais
5. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-prontuario",
  "appointmentId": "guid-do-agendamento",
  "patientId": "guid-do-paciente",
  "patientName": "Carlos Alberto Santos",
  "userId": "guid-do-medico",
  "userName": "Dra. Maria Santos",
  "chiefComplaint": "Dor no peito e falta de ar",
  "historyOfPresentIllness": "Paciente relata dor torácica há 3 dias...",
  "physicalExamination": "PA: 140/90 mmHg, FC: 88 bpm...",
  "diagnosis": "Hipertensão arterial sistêmica não controlada...",
  "treatment": "Iniciado Losartana 50mg 1x/dia...",
  "prescriptions": "Losartana 50mg - 1 comprimido pela manhã - 30 dias",
  "followUpInstructions": "Retornar em 15 dias com resultados dos exames...",
  "status": "InProgress",
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-15T14:30:00Z"
}
```

### 🔍 Endpoints Adicionais de Prontuários

```
GET /api/medical-records/appointment/{appointmentId}   - Prontuário por agendamento
GET /api/medical-records/patient/{patientId}           - Todos os prontuários do paciente
PUT /api/medical-records/{id}                          - Atualiza prontuário
POST /api/medical-records/{id}/complete                - Finaliza prontuário
```

### ✅ Finalizar Prontuário

Quando o atendimento estiver completo:

```
POST /api/medical-records/{id}/complete
```

Isso muda o status para `Completed` e não permite mais edições.

---

## Teste 8: Gerenciar Prontuários

Consulte o histórico médico completo dos pacientes.

### 🔍 Buscar Prontuários do Paciente

```
GET /api/medical-records/patient/{patientId}
```

**Como testar**:
1. Use o `patientId` do Carlos que criamos no Teste 4
2. Deve retornar todos os prontuários desse paciente

### ✅ Resposta Esperada

```json
[
  {
    "id": "guid-prontuario-1",
    "appointmentDate": "2024-01-20T14:00:00Z",
    "diagnosis": "Hipertensão arterial sistêmica não controlada",
    "treatment": "Iniciado Losartana 50mg 1x/dia",
    "userName": "Dra. Maria Santos",
    "status": "Completed"
  },
  {
    "id": "guid-prontuario-2",
    "appointmentDate": "2024-02-05T14:00:00Z",
    "diagnosis": "Hipertensão controlada. Dislipidemia.",
    "treatment": "Mantido Losartana. Iniciado Sinvastatina 20mg",
    "userName": "Dra. Maria Santos",
    "status": "Completed"
  }
]
```

### 🔍 Buscar Prontuário por Agendamento

```
GET /api/medical-records/appointment/{appointmentId}
```

Retorna o prontuário específico de um agendamento.

---

## Teste 9: Processar Pagamentos

Registre e processe pagamentos de consultas.

### 📍 Endpoint - Criar Pagamento

```
POST /api/payments
```

### 🔒 Autenticação

**REQUER** autenticação

### 📝 Dados de Teste

```json
{
  "appointmentId": "guid-do-agendamento",
  "amount": 250.00,
  "paymentMethod": "CreditCard",
  "paymentDate": "2024-01-20T15:00:00Z",
  "observations": "Pagamento à vista no cartão"
}
```

### 📊 Métodos de Pagamento

- `Cash` - Dinheiro
- `CreditCard` - Cartão de Crédito
- `DebitCard` - Cartão de Débito
- `Pix` - PIX
- `BankTransfer` - Transferência Bancária
- `Check` - Cheque

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/payments`
2. Clique em "Try it out"
3. Cole os dados de teste
4. Substitua o `appointmentId` pelo GUID real
5. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-do-pagamento",
  "appointmentId": "guid-do-agendamento",
  "amount": 250.00,
  "paymentMethod": "CreditCard",
  "paymentDate": "2024-01-20T15:00:00Z",
  "status": "Pending",
  "observations": "Pagamento à vista no cartão",
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-20T15:00:00Z"
}
```

### 💳 Processar Pagamento

Após criar, processe o pagamento:

```
PUT /api/payments/process
```

Body:
```json
{
  "paymentId": "guid-do-pagamento",
  "transactionId": "TRX-123456"
}
```

### 🔍 Endpoints Adicionais de Pagamentos

```
GET /api/payments/{id}                          - Busca pagamento por ID
GET /api/payments/appointment/{appointmentId}   - Pagamentos de um agendamento
PUT /api/payments/{id}/refund                   - Estorna pagamento
PUT /api/payments/{id}/cancel                   - Cancela pagamento
```

---

## Teste 10: Visualizar Relatórios

Acesse relatórios financeiros e estatísticos da clínica.

### 🔒 Autenticação

**REQUER** autenticação (ClinicOwner ou SystemAdmin)

### 📊 Relatório de Resumo Financeiro

```
GET /api/reports/financial-summary?startDate=2024-01-01&endDate=2024-01-31
```

**Resposta**:
```json
{
  "period": {
    "startDate": "2024-01-01",
    "endDate": "2024-01-31"
  },
  "revenue": {
    "total": 15250.00,
    "received": 12500.00,
    "pending": 2750.00
  },
  "expenses": {
    "total": 5800.00,
    "paid": 4500.00,
    "pending": 1300.00
  },
  "netProfit": 6700.00,
  "appointmentsCount": 45,
  "patientsCount": 32
}
```

### 📊 Relatório de Receitas

```
GET /api/reports/revenue?startDate=2024-01-01&endDate=2024-01-31&groupBy=day
```

Parâmetros `groupBy`:
- `day` - Por dia
- `week` - Por semana
- `month` - Por mês

### 📊 Relatório de Agendamentos

```
GET /api/reports/appointments?startDate=2024-01-01&endDate=2024-01-31
```

**Resposta**:
```json
{
  "period": {
    "startDate": "2024-01-01",
    "endDate": "2024-01-31"
  },
  "totalAppointments": 45,
  "byStatus": {
    "Scheduled": 12,
    "Completed": 28,
    "Cancelled": 3,
    "NoShow": 2
  },
  "byProcedure": [
    {
      "procedureName": "Consulta Cardiológica",
      "count": 20,
      "revenue": 5000.00
    }
  ]
}
```

### 📊 Relatório de Pacientes

```
GET /api/reports/patients?startDate=2024-01-01&endDate=2024-01-31
```

### 📊 Contas a Receber

```
GET /api/reports/accounts-receivable
```

Lista todos os pagamentos pendentes.

### 📊 Contas a Pagar

```
GET /api/reports/accounts-payable
```

Lista todas as despesas pendentes.

### 🎯 Como Testar no Swagger

1. Localize qualquer endpoint em `/api/reports`
2. Clique em "Try it out"
3. Ajuste as datas conforme necessário
4. Clique em "Execute"

---

## Teste 11: Gerenciar Assinatura

Como Owner, você pode gerenciar o plano de assinatura da clínica.

### 🔒 Autenticação

**REQUER** autenticação (ClinicOwner)

### 📋 Ver Assinatura Atual

```
GET /api/subscriptions/current
```

**Resposta**:
```json
{
  "id": "guid-da-assinatura",
  "clinicId": "guid-da-clinica",
  "planId": "guid-do-plano",
  "planName": "Trial",
  "status": "Active",
  "startDate": "2024-01-15",
  "endDate": "2024-01-30",
  "isTrial": true,
  "trialEndDate": "2024-01-30",
  "maxUsers": 2,
  "maxPatients": 100,
  "price": 0.00
}
```

### ⬆️ Fazer Upgrade de Plano

```
POST /api/subscriptions/upgrade
```

Body:
```json
{
  "newPlanId": "guid-do-plano-basic"
}
```

### ⬇️ Fazer Downgrade de Plano

```
POST /api/subscriptions/downgrade
```

Body:
```json
{
  "newPlanId": "guid-do-plano-trial",
  "reason": "Reduzindo custos temporariamente"
}
```

### ❄️ Congelar Assinatura

```
POST /api/subscriptions/freeze
```

Body:
```json
{
  "reason": "Clínica fechada para reformas - 30 dias",
  "freezeDays": 30
}
```

### 🔥 Descongelar Assinatura

```
POST /api/subscriptions/unfreeze
```

### 🚫 Cancelar Mudança Pendente

Se você solicitou um upgrade/downgrade mas quer cancelar:

```
POST /api/subscriptions/cancel-pending-change
```

---

## Teste 12: Configurar Notificações

Configure rotinas automáticas de notificação para pacientes.

### 🔒 Autenticação

**REQUER** autenticação (ClinicOwner ou SystemAdmin)

### 📍 Endpoint - Criar Rotina de Notificação

```
POST /api/notification-routines
```

### 📝 Dados de Teste - Lembrete de Consulta

```json
{
  "name": "Lembrete 24h antes da consulta",
  "description": "Envia SMS para paciente 24h antes do agendamento",
  "trigger": "BeforeAppointment",
  "hoursBeforeAppointment": 24,
  "channel": "SMS",
  "messageTemplate": "Olá {{PatientName}}! Lembre-se: você tem consulta amanhã às {{AppointmentTime}} com {{DoctorName}}. Clínica: {{ClinicName}}. Dúvidas: {{ClinicPhone}}",
  "isActive": true
}
```

### 📝 Dados de Teste - Confirmação de Agendamento

```json
{
  "name": "Confirmação de Agendamento",
  "description": "Envia confirmação imediata após criar agendamento",
  "trigger": "AppointmentCreated",
  "channel": "Email",
  "messageTemplate": "Olá {{PatientName}}! Sua consulta foi agendada para {{AppointmentDate}} às {{AppointmentTime}} com {{DoctorName}}. Local: {{ClinicAddress}}.",
  "isActive": true
}
```

### 📊 Triggers Disponíveis

- `AppointmentCreated` - Ao criar agendamento
- `BeforeAppointment` - X horas antes do agendamento
- `AfterAppointment` - X horas depois do agendamento
- `PaymentReceived` - Ao receber pagamento
- `BirthdayReminder` - Aniversário do paciente

### 📧 Canais de Comunicação

- `SMS` - Mensagem de texto
- `Email` - Email
- `WhatsApp` - WhatsApp (se configurado)
- `Push` - Notificação push no app

### 🎯 Como Testar no Swagger

1. Localize o endpoint `POST /api/notification-routines`
2. Clique em "Try it out"
3. Cole os dados de teste
4. Clique em "Execute"

### ✅ Resposta Esperada

```json
{
  "id": "guid-da-rotina",
  "name": "Lembrete 24h antes da consulta",
  "description": "Envia SMS para paciente 24h antes do agendamento",
  "trigger": "BeforeAppointment",
  "hoursBeforeAppointment": 24,
  "channel": "SMS",
  "messageTemplate": "Olá {{PatientName}}! Lembre-se...",
  "isActive": true,
  "tenantId": "guid-da-clinica",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

### 🔍 Endpoints Adicionais de Notificações

```
GET /api/notification-routines              - Lista todas as rotinas
GET /api/notification-routines/active       - Lista apenas as ativas
GET /api/notification-routines/{id}         - Busca rotina por ID
PUT /api/notification-routines/{id}         - Atualiza rotina
POST /api/notification-routines/{id}/activate     - Ativa rotina
POST /api/notification-routines/{id}/deactivate   - Desativa rotina
DELETE /api/notification-routines/{id}      - Deleta rotina
```

### 📱 Variáveis de Template

Você pode usar estas variáveis nos templates de mensagem:

- `{{PatientName}}` - Nome do paciente
- `{{DoctorName}}` - Nome do médico
- `{{AppointmentDate}}` - Data do agendamento
- `{{AppointmentTime}}` - Hora do agendamento
- `{{ProcedureName}}` - Nome do procedimento
- `{{ClinicName}}` - Nome da clínica
- `{{ClinicPhone}}` - Telefone da clínica
- `{{ClinicAddress}}` - Endereço da clínica

---

## Teste 13: Telas do Frontend

Agora vamos testar as telas do frontend (interface visual).

### 🖥️ Iniciar o Frontend

```bash
cd /home/runner/work/MW.Code/MW.Code/frontend/medicwarehouse-app
npm install
npm start
```

O app estará disponível em: http://localhost:4200

---

### 🔐 Tela 1: Login

**URL**: http://localhost:4200/login

#### 📋 O que testar:

1. **Campos visíveis**:
   - ✅ Campo "Usuário"
   - ✅ Campo "Senha"
   - ✅ Campo "Tenant ID" (ID da clínica)
   - ✅ Botão "Entrar"
   - ✅ Link "Não tem conta? Cadastre-se"

2. **Teste de Login**:
   - Digite: `joao.silva`
   - Senha: `MedicWare2024!@#`
   - Tenant ID: O `clinicId` do Teste 1
   - Clique em "Entrar"

3. **Resultado Esperado**:
   - ✅ Redireciona para o Dashboard
   - ✅ Mostra mensagem "Bem-vindo, João Silva"

4. **Teste de Erro**:
   - Digite credenciais erradas
   - ✅ Deve mostrar "Credenciais inválidas"

---

### 📝 Tela 2: Cadastro (Registro)

**URL**: http://localhost:4200/register

#### 📋 O que testar:

1. **Seções visíveis**:
   - ✅ Dados da Clínica (nome, CNPJ, email, telefone, endereço)
   - ✅ Dados do Proprietário (nome, CPF, email, telefone)
   - ✅ Dados de Login (username, senha)
   - ✅ Escolha de Plano
   - ✅ Termos e Condições
   - ✅ Botão "Cadastrar"

2. **Teste de Cadastro**:
   - Preencha todos os campos com dados válidos
   - Selecione um plano (Trial para testar)
   - Marque "Aceito os termos"
   - Clique em "Cadastrar"

3. **Resultado Esperado**:
   - ✅ Mostra mensagem "Cadastro realizado com sucesso!"
   - ✅ Redireciona para a tela de Login

4. **Validações a testar**:
   - ✅ CNPJ inválido → mostra erro
   - ✅ CPF inválido → mostra erro
   - ✅ Senha fraca → mostra erro
   - ✅ Email inválido → mostra erro
   - ✅ Campos obrigatórios vazios → desabilita botão

---

### 📊 Tela 3: Dashboard

**URL**: http://localhost:4200/dashboard

#### 📋 O que testar:

1. **Cards de Resumo**:
   - ✅ Total de Pacientes
   - ✅ Agendamentos de Hoje
   - ✅ Receita do Mês
   - ✅ Pendências

2. **Gráficos**:
   - ✅ Gráfico de receitas (últimos 30 dias)
   - ✅ Gráfico de agendamentos por status

3. **Agenda do Dia**:
   - ✅ Lista dos agendamentos de hoje
   - ✅ Horário, paciente, médico, procedimento
   - ✅ Botão "Iniciar Atendimento" para agendamentos confirmados

4. **Menu Lateral**:
   - ✅ Dashboard
   - ✅ Pacientes
   - ✅ Agendamentos
   - ✅ Atendimentos
   - ✅ Relatórios (se for Owner)
   - ✅ Configurações (se for Owner)
   - ✅ Sair

---

### 👥 Tela 4: Lista de Pacientes

**URL**: http://localhost:4200/patients

#### 📋 O que testar:

1. **Barra de Ferramentas**:
   - ✅ Campo de busca (por nome ou CPF)
   - ✅ Botão "Novo Paciente"
   - ✅ Filtros (todos, ativos, inativos)

2. **Tabela de Pacientes**:
   - ✅ Colunas: Nome, CPF, Telefone, Email, Idade, Ações
   - ✅ Botão "Editar" em cada linha
   - ✅ Botão "Visualizar Prontuários" em cada linha
   - ✅ Paginação (se houver muitos pacientes)

3. **Teste de Busca**:
   - Digite "Carlos" na busca
   - ✅ Deve filtrar e mostrar apenas Carlos Alberto Santos

4. **Teste de Novo Paciente**:
   - Clique em "Novo Paciente"
   - ✅ Deve abrir o formulário de cadastro

---

### 📝 Tela 5: Formulário de Paciente

**URL**: http://localhost:4200/patients/new ou `/patients/edit/{id}`

#### 📋 O que testar:

1. **Seções do Formulário**:
   - ✅ Dados Pessoais (nome, CPF, RG, data nascimento, sexo)
   - ✅ Contatos (email, telefone, telefone emergência)
   - ✅ Endereço (rua, número, complemento, bairro, cidade, estado, CEP)
   - ✅ Observações

2. **Teste de Cadastro**:
   - Preencha todos os campos
   - Clique em "Salvar"
   - ✅ Deve salvar e retornar para a lista
   - ✅ Deve mostrar mensagem "Paciente cadastrado com sucesso!"

3. **Teste de Edição**:
   - Na lista, clique em "Editar" de um paciente
   - Altere o telefone
   - Clique em "Salvar"
   - ✅ Deve atualizar e voltar para a lista

4. **Validações a testar**:
   - ✅ CPF inválido → mostra erro
   - ✅ Email inválido → mostra erro
   - ✅ Data futura → mostra erro
   - ✅ Campos obrigatórios vazios → desabilita botão

---

### 📅 Tela 6: Lista de Agendamentos

**URL**: http://localhost:4200/appointments

#### 📋 O que testar:

1. **Visualizações Disponíveis**:
   - ✅ Calendário mensal
   - ✅ Agenda semanal
   - ✅ Lista completa

2. **Filtros**:
   - ✅ Por data
   - ✅ Por médico
   - ✅ Por status (agendado, confirmado, concluído, cancelado)

3. **Tabela de Agendamentos**:
   - ✅ Colunas: Data/Hora, Paciente, Médico, Procedimento, Status, Ações
   - ✅ Botão "Iniciar Atendimento" (se status = Confirmado)
   - ✅ Botão "Cancelar"
   - ✅ Badge colorido para cada status

4. **Teste de Calendário**:
   - Clique em um dia no calendário
   - ✅ Deve mostrar os agendamentos daquele dia

---

### 📝 Tela 7: Formulário de Agendamento

**URL**: http://localhost:4200/appointments/new

#### 📋 O que testar:

1. **Campos do Formulário**:
   - ✅ Seleção de Paciente (combobox com busca)
   - ✅ Seleção de Médico (combobox)
   - ✅ Seleção de Procedimento (combobox)
   - ✅ Seleção de Data (calendário)
   - ✅ Seleção de Horário (combobox com horários disponíveis)
   - ✅ Campo de Observações
   - ✅ Botão "Agendar"

2. **Teste de Criação**:
   - Selecione um paciente
   - Selecione um médico
   - Selecione um procedimento
   - Escolha uma data futura
   - ✅ Horários disponíveis devem aparecer automaticamente
   - Selecione um horário
   - Clique em "Agendar"

3. **Resultado Esperado**:
   - ✅ Salva o agendamento
   - ✅ Mostra mensagem "Agendamento criado com sucesso!"
   - ✅ Retorna para a lista de agendamentos

4. **Validações a testar**:
   - ✅ Data no passado → mostra erro
   - ✅ Horário já ocupado → mostra erro
   - ✅ Campos obrigatórios vazios → desabilita botão

---

### 🏥 Tela 8: Atendimento (Prontuário)

**URL**: http://localhost:4200/attendance/{appointmentId}

#### 📋 O que testar:

1. **Informações do Cabeçalho**:
   - ✅ Nome do paciente
   - ✅ Idade
   - ✅ Procedimento
   - ✅ Data/hora da consulta

2. **Formulário de Prontuário**:
   - ✅ Queixa Principal
   - ✅ História da Doença Atual
   - ✅ Exame Físico
   - ✅ Diagnóstico
   - ✅ Tratamento/Conduta
   - ✅ Prescrições
   - ✅ Orientações de Retorno

3. **Histórico do Paciente** (painel lateral):
   - ✅ Prontuários anteriores
   - ✅ Data de cada atendimento
   - ✅ Diagnóstico de cada atendimento
   - ✅ Botão para visualizar detalhes

4. **Teste de Atendimento Completo**:
   - Preencha todos os campos do prontuário
   - Clique em "Salvar Rascunho" (salva sem finalizar)
   - ✅ Deve salvar e mostrar mensagem
   - Continue editando
   - Clique em "Finalizar Atendimento"
   - ✅ Deve finalizar e retornar para agendamentos
   - ✅ Status do agendamento deve mudar para "Concluído"

5. **Teste de Visualização de Histórico**:
   - Clique em um prontuário anterior no painel lateral
   - ✅ Deve mostrar os detalhes daquele atendimento em modo leitura

---

## Troubleshooting

### ❌ Problema: "401 Unauthorized" em todas as requisições

**Causa**: Token JWT não configurado ou expirado

**Solução**:
1. Faça login novamente (Teste 2)
2. Copie o novo token
3. Configure no Swagger (botão Authorize)
4. Formato: `Bearer {token}`

---

### ❌ Problema: "No connection could be made" ou erro de conexão

**Causa**: API não está rodando

**Solução**:
```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Api
dotnet run
```

Verifique se aparece: `Now listening on: https://localhost:5001`

---

### ❌ Problema: "Invalid tenant" ou "Tenant not found"

**Causa**: Header X-Tenant-Id não configurado ou inválido

**Solução no Swagger**:
1. O header X-Tenant-Id é enviado automaticamente pelo sistema após o login
2. Se ainda assim houver erro, verifique se o `clinicId` está correto
3. Use o `clinicId` retornado no Teste 1

---

### ❌ Problema: "CNPJ already registered" ou "Username already taken"

**Causa**: Dados já existem no banco de outros testes

**Solução**:
1. Use outros valores (ex: CNPJ 98765432000195, username joao.silva2)
2. Ou limpe o banco e recomece:
```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Api
dotnet ef database drop
dotnet ef database update
```

---

### ❌ Problema: "Password validation failed"

**Causa**: Senha não atende aos requisitos de segurança

**Requisitos da Senha**:
- ✅ Mínimo 8 caracteres
- ✅ Pelo menos 1 letra minúscula (a-z)
- ✅ Pelo menos 1 letra maiúscula (A-Z)
- ✅ Pelo menos 1 número (0-9)
- ✅ Pelo menos 1 caractere especial (!@#$%^&*)

**Exemplo de senha válida**: `MedicWare2024!@#`

---

### ❌ Problema: Frontend não carrega ou "Cannot GET /"

**Causa**: Frontend não está rodando

**Solução**:
```bash
cd /home/runner/work/MW.Code/MW.Code/frontend/medicwarehouse-app
npm install
npm start
```

Aguarde compilar e abra: http://localhost:4200

---

### ❌ Problema: "Invalid CPF" ou "Invalid CNPJ"

**Causa**: Formato inválido

**Solução**:
- CPF deve ter 11 dígitos: `12345678901`
- CNPJ deve ter 14 dígitos: `12345678000195`
- Não use pontos, traços ou barras nas APIs

---

### ❌ Problema: Swagger não mostra os endpoints

**Causa**: Erro ao carregar a documentação OpenAPI

**Solução**:
1. Limpe o cache do navegador (Ctrl+Shift+Delete)
2. Tente em modo anônimo
3. Acesse diretamente: https://localhost:5001/swagger/v1/swagger.json
4. Se houver erro JSON, recompile o projeto:
```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Api
dotnet clean
dotnet build
dotnet run
```

---

### ❌ Problema: Erro de CORS no frontend

**Causa**: API não permite requisições do frontend

**Solução**:
- Verifique se o `appsettings.json` tem configuração de CORS para http://localhost:4200
- A configuração já deve estar correta no projeto

---

### ❌ Problema: Token expira muito rápido

**Causa**: Configuração de expiração em 60 minutos

**Solução temporária para testes**:
1. Edite `appsettings.json`
2. Altere `"ExpiryMinutes": 60` para `"ExpiryMinutes": 480` (8 horas)
3. Reinicie a API
4. **Importante**: Não faça isso em produção!

---

## 📝 Resumo dos Dados de Teste

Use estes dados consistentemente em todos os testes:

### Clínica
- Nome: Clínica Teste
- CNPJ: 12345678000195
- Email: contato@clinicateste.com
- Telefone: (11) 98765-4321

### Usuário Owner
- Username: joao.silva
- Password: MedicWare2024!@#
- Email: joao@clinicateste.com
- Nome: Dr. João Silva

### Médica
- Username: dra.maria
- Email: maria@clinicateste.com
- Nome: Dra. Maria Santos
- Especialidade: Cardiologia

### Paciente 1 (Adulto)
- Nome: Carlos Alberto Santos
- CPF: 11122233344
- Email: carlos@email.com
- Telefone: (11) 95555-4444

### Paciente 2 (Criança)
- Nome: Joana Silva Santos
- CPF: 55566677788
- Telefone: (11) 93333-2222

### Procedimentos
1. Consulta Cardiológica - R$ 250,00
2. Ecocardiograma - R$ 450,00
3. Holter 24 horas - R$ 380,00

---

## ✅ Checklist de Testes Completos

Marque cada teste conforme completar:

- [ ] ✅ Teste 1: Cadastrar Clínica e Owner
- [ ] ✅ Teste 2: Login do Owner
- [ ] ✅ Teste 3: Cadastrar Médica (Dra. Maria)
- [ ] ✅ Teste 4: Cadastrar Paciente Adulto (Carlos)
- [ ] ✅ Teste 4b: Cadastrar Paciente Criança (Joana)
- [ ] ✅ Teste 5: Cadastrar 3 Procedimentos
- [ ] ✅ Teste 6: Criar Agendamento
- [ ] ✅ Teste 7: Registrar Atendimento/Prontuário
- [ ] ✅ Teste 8: Consultar Histórico do Paciente
- [ ] ✅ Teste 9: Processar Pagamento
- [ ] ✅ Teste 10: Ver Relatórios Financeiros
- [ ] ✅ Teste 11: Gerenciar Assinatura
- [ ] ✅ Teste 12: Configurar Notificações
- [ ] ✅ Teste 13: Testar Frontend - Login
- [ ] ✅ Teste 13b: Testar Frontend - Cadastro
- [ ] ✅ Teste 13c: Testar Frontend - Dashboard
- [ ] ✅ Teste 13d: Testar Frontend - Pacientes
- [ ] ✅ Teste 13e: Testar Frontend - Agendamentos
- [ ] ✅ Teste 13f: Testar Frontend - Atendimento

---

## 🎓 Próximos Passos

Depois de completar todos os testes acima, você pode explorar:

1. **Recursos Avançados**:
   - Gerenciamento de despesas (`/api/expenses`)
   - Faturas e notas fiscais (`/api/invoices`)
   - Configuração de módulos (`/api/module-config`)
   - Recuperação de senha (`/api/password-recovery`)

2. **Funcionalidades de Admin**:
   - Painel de administração do sistema (`/api/system-admin`)
   - Gerenciamento de múltiplas clínicas
   - Analytics e métricas globais

3. **Contato e Suporte**:
   - Formulário de contato (`/api/contact`)
   - Cadastro de dados demo (`/api/data-seeder/seed-demo`)

---

## 📚 Documentação Relacionada

- [SYSTEM_SETUP_GUIDE.md](../frontend/mw-docs/src/assets/docs/SYSTEM_SETUP_GUIDE.md) - Guia completo de configuração
- [ORDEM_CORRETA_CADASTRO.md](./ORDEM_CORRETA_CADASTRO.md) - Referência rápida da ordem de cadastro
- [SCREENS_DOCUMENTATION.md](../frontend/mw-docs/src/assets/docs/SCREENS_DOCUMENTATION.md) - Documentação detalhada das telas
- [OWNER_FLOW_DOCUMENTATION.md](../OWNER_FLOW_DOCUMENTATION.md) - Fluxo de proprietários
- [POSTMAN_QUICK_GUIDE.md](../POSTMAN_QUICK_GUIDE.md) - Guia de uso do Postman

---

**Última Atualização**: Janeiro 2025
**Versão**: 1.0
