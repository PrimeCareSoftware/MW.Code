# 🎯 Guia Visual Rápido - Postman Collection

## 📥 Resultado Final

Após importar a coleção no Postman, você verá:

```
📁 Omni Care Software API
   │
   ├── 🔐 Auth (2 requests)
   │   ├── 📨 Login
   │   └── 👤 Get Current User Info
   │
   ├── 🏥 Patients (11 requests)
   │   ├── 📋 List Patients
   │   ├── 🔍 Get Patient by ID
   │   ├── 🔎 Search Patients
   │   ├── 📄 Get Patient by Document (CPF)
   │   ├── ➕ Create Patient
   │   ├── ✏️ Update Patient
   │   ├── ❌ Delete Patient
   │   ├── 🔗 Link Patient to Clinic
   │   ├── 👶 Link Child to Guardian
   │   └── 👨‍👩‍👧‍👦 List Children of Guardian
   │
   ├── 📅 Appointments (5 requests)
   │   ├── ➕ Create Appointment
   │   ├── 🔍 Get Appointment by ID
   │   ├── ❌ Cancel Appointment
   │   ├── 📆 Daily Agenda
   │   └── ⏰ Available Time Slots
   │
   ├── 📋 Medical Records (5 requests)
   │   ├── ➕ Create Medical Record
   │   ├── ✏️ Update Medical Record
   │   ├── ✅ Complete Medical Record
   │   ├── 🔍 Get by Appointment
   │   └── 📜 Patient History
   │
   ├── 💉 Procedures (8 requests)
   │   ├── 📋 List Procedures
   │   ├── 🔍 Get Procedure by ID
   │   ├── ➕ Create Procedure
   │   ├── ✏️ Update Procedure
   │   ├── ❌ Delete Procedure
   │   ├── ➕ Add Procedure to Appointment
   │   ├── 📋 List Appointment Procedures
   │   └── 💰 Billing Summary
   │
   ├── 💸 Expenses (7 requests)
   │   ├── 📋 List Expenses
   │   ├── 🔍 Get Expense by ID
   │   ├── ➕ Create Expense
   │   ├── ✏️ Update Expense
   │   ├── ✅ Pay Expense
   │   ├── ❌ Cancel Expense
   │   └── 🗑️ Delete Expense
   │
   ├── 📊 Reports (6 requests)
   │   ├── 💰 Financial Summary
   │   ├── 💵 Revenue Report
   │   ├── 📅 Appointments Report
   │   ├── 👥 Patients Report
   │   ├── 📈 Accounts Receivable
   │   └── 📉 Accounts Payable
   │
   └── 🌱 Data Seeder (2 requests)
       ├── ℹ️ Get Demo Info
       └── 🔧 Seed Demo Data
```

## 🔧 Variáveis Configuradas

```
base_url         = http://localhost:5000
bearer_token     = (vazio - preencher após login)
tenant_id        = demo-clinic-001
```

## 🚀 Fluxo de Uso Rápido

### 1️⃣ Primeiro Uso

```
1. Importar coleção no Postman
2. Executar: Data Seeder > Seed Demo Data
3. Executar: Auth > Login
4. Copiar token da resposta
5. Colar token na variável bearer_token
6. Pronto! Todos os endpoints estão prontos
```

### 2️⃣ Teste Completo

```
🌱 Seed Demo Data
    ↓
🔐 Login (copiar token)
    ↓
👤 Get Current User Info (testar autenticação)
    ↓
🏥 List Patients (ver pacientes criados)
    ↓
📅 Daily Agenda (ver agendamentos)
    ↓
📊 Financial Summary (ver relatórios)
```

## 📝 Exemplos de Body

### Login Request
```json
{
  "username": "admin",
  "password": "admin123",
  "tenantId": "demo-clinic-001"
}
```

### Create Patient Request
```json
{
  "name": "João Silva",
  "document": "123.456.789-00",
  "dateOfBirth": "1990-01-15",
  "phone": "+55 11 98765-4321",
  "email": "joao@email.com",
  "address": "Rua Exemplo, 123",
  "guardianId": null
}
```

### Create Appointment Request
```json
{
  "patientId": "",
  "doctorId": "",
  "clinicId": "demo-clinic-001",
  "scheduledDate": "2024-12-01T10:00:00",
  "appointmentType": "Consulta",
  "notes": "Consulta de rotina"
}
```

## ✨ Recursos Automáticos

Todos os requests já incluem automaticamente:

✅ **Authorization Header**: `Bearer {{bearer_token}}`  
✅ **X-Tenant-Id Header**: `{{tenant_id}}`  
✅ **Content-Type**: `application/json` (onde necessário)  
✅ **Base URL**: `{{base_url}}` em todos os endpoints

## 🎨 Benefícios

- ⚡ **Teste Rápido**: Não precisa digitar URLs ou headers
- 🔄 **Reutilizável**: Salve IDs em variáveis para reusar
- 📚 **Organizado**: Estrutura clara por funcionalidade
- 🌐 **Multi-ambiente**: Fácil alternar entre Dev/Staging/Prod
- 💾 **Exportável**: Compartilhe com o time facilmente
- 📖 **Documentado**: Descrições em cada request

## 🎯 Casos de Uso

### Desenvolvimento
- Testar endpoints durante implementação
- Validar payloads e respostas
- Debug de problemas de integração

### QA/Testing
- Criar cenários de teste
- Validar fluxos completos
- Testes de regressão

### Integração
- Referência para desenvolvedores frontend
- Exemplos práticos de uso da API
- Prototipagem rápida

---

**Arquivo**: `Omni Care Software-Postman-Collection.json`  
**Guia Completo**: `POSTMAN_IMPORT_GUIDE.md`  
**Repositório**: https://github.com/Omni Care Software/MW.Code
