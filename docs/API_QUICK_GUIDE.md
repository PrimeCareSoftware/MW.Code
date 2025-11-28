# Guia Rápido de API - Novas Funcionalidades

## 🔍 Endpoints de Busca de Pacientes

### 1. Busca Combinada (CPF, Nome ou Telefone)

**Endpoint**: `GET /api/patients/search`

**Query Parameters**:
- `searchTerm` (string, obrigatório): Termo de busca

**Exemplos**:
```bash
# Buscar por CPF
GET /api/patients/search?searchTerm=123.456.789-00

# Buscar por Nome
GET /api/patients/search?searchTerm=João Silva

# Buscar por Telefone
GET /api/patients/search?searchTerm=11987654321
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "name": "João Silva",
    "document": "123.456.789-00",
    "phone": "+55 11 98765-4321",
    "email": "joao@email.com",
    "dateOfBirth": "1980-01-15",
    "age": 44
  }
]
```

**Características**:
- ✅ Busca em CPF, Nome e Telefone simultaneamente
- ✅ Filtrado automaticamente pela clínica atual (TenantId)
- ✅ Ordenado por nome
- ✅ Case-insensitive

---

### 2. Busca Global por CPF

**Endpoint**: `GET /api/patients/by-document/{document}`

**Path Parameters**:
- `document` (string): CPF do paciente

**Exemplo**:
```bash
GET /api/patients/by-document/123.456.789-00
```

**Resposta**:
```json
{
  "id": "guid",
  "name": "João Silva",
  "document": "123.456.789-00",
  "phone": "+55 11 98765-4321",
  "email": "joao@email.com",
  "dateOfBirth": "1980-01-15",
  "age": 44,
  "allergies": "Penicilina",
  "address": {
    "street": "Rua das Flores",
    "number": "123",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567"
  }
}
```

**Características**:
- ✅ Busca em **todas as clínicas** (não filtrado por TenantId)
- ✅ Usado para detectar cadastro prévio
- ✅ Retorna dados completos do paciente
- ✅ Permite reutilização de cadastro

**Caso de Uso**:
```
1. Recepcionista digita CPF do novo paciente
2. Sistema faz GET /api/patients/by-document/{cpf}
3. Se paciente existe:
   - Exibe dados existentes
   - Permite edição se necessário
   - Cria vínculo com clínica atual
4. Se não existe:
   - Cria novo cadastro
```

---

## 🔗 Endpoint de Vínculo de Paciente à Clínica

**Endpoint**: `POST /api/patients/{patientId}/link-clinic/{clinicId}`

**Path Parameters**:
- `patientId` (guid): ID do paciente
- `clinicId` (guid): ID da clínica

**Exemplo**:
```bash
POST /api/patients/550e8400-e29b-41d4-a716-446655440000/link-clinic/660e8400-e29b-41d4-a716-446655440001
```

**Headers**:
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Resposta**:
```json
{
  "success": true
}
```

**Características**:
- ✅ Cria vínculo N:N entre paciente e clínica
- ✅ Verifica se vínculo já existe
- ✅ Reativa vínculo se estava inativo
- ✅ Isolado por TenantId

**Fluxo Completo**:
```
┌─────────────────────────────────────────────────────┐
│ 1. Busca paciente por CPF                          │
│    GET /api/patients/by-document/{cpf}             │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
         ┌────────────────┐
         │ Paciente existe? │
         └────────┬─────────┘
                  │
         ┌────────┴────────┐
         │                 │
        Sim               Não
         │                 │
         ▼                 ▼
┌────────────────┐  ┌──────────────────┐
│ 2a. Vincular   │  │ 2b. Criar Novo   │
│ POST /patients/│  │ POST /patients   │
│ {id}/link-     │  │                  │
│ clinic/{id}    │  │ (vínculo auto)   │
└────────────────┘  └──────────────────┘
```

---

## 📊 Endpoints de Histórico do Paciente

### Timeline de Consultas

**Endpoint**: `GET /api/medical-records/patient/{patientId}`

**Path Parameters**:
- `patientId` (guid): ID do paciente

**Exemplo**:
```bash
GET /api/medical-records/patient/550e8400-e29b-41d4-a716-446655440000
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "appointmentId": "guid",
    "patientId": "guid",
    "patientName": "João Silva",
    "diagnosis": "Hipertensão controlada",
    "prescription": "Losartana 50mg - 1x ao dia",
    "notes": "Paciente apresentou melhora",
    "consultationDurationMinutes": 30,
    "consultationStartTime": "2024-01-15T14:30:00Z",
    "consultationEndTime": "2024-01-15T15:00:00Z",
    "createdAt": "2024-01-15T14:30:00Z"
  },
  {
    "id": "guid",
    "appointmentId": "guid",
    "patientId": "guid",
    "patientName": "João Silva",
    "diagnosis": "Gripe comum",
    "prescription": "Paracetamol 750mg - 3x ao dia",
    "notes": "Repouso recomendado",
    "consultationDurationMinutes": 20,
    "consultationStartTime": "2023-12-10T10:00:00Z",
    "consultationEndTime": "2023-12-10T10:20:00Z",
    "createdAt": "2023-12-10T10:00:00Z"
  }
]
```

**Características**:
- ✅ Retorna histórico **apenas da clínica atual** (TenantId)
- ✅ Ordenado por data (mais recente primeiro)
- ✅ Inclui diagnóstico, prescrição e duração
- ✅ Usado para renderizar timeline no frontend

**Privacidade**:
```
Clínica A                    Clínica B
    ↓                            ↓
Prontuários A                Prontuários B
(isolados)                   (isolados)
    ↓                            ↓
GET /medical-records/        GET /medical-records/
patient/{id}                 patient/{id}
    ↓                            ↓
Retorna APENAS               Retorna APENAS
prontuários da               prontuários da
Clínica A                    Clínica B
```

---

## 📝 Templates de Prontuário e Prescrição

### Listar Templates de Prontuário

**Endpoint**: `GET /api/medical-record-templates`

**Query Parameters** (opcionais):
- `category` (string): Filtrar por categoria

**Exemplo**:
```bash
# Todos os templates
GET /api/medical-record-templates

# Por categoria
GET /api/medical-record-templates?category=Cardiologia
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "name": "Consulta de Rotina - Cardiologia",
    "description": "Template padrão para consultas cardiológicas",
    "templateContent": "Pressão Arterial: \nFrequência Cardíaca: \nAuscuta Cardíaca: \n",
    "category": "Cardiologia",
    "isActive": true,
    "createdAt": "2024-01-01T10:00:00Z"
  }
]
```

### Listar Templates de Prescrição

**Endpoint**: `GET /api/prescription-templates`

**Exemplo**:
```bash
GET /api/prescription-templates?category=Hipertensão
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "name": "Prescrição Hipertensão",
    "description": "Template para prescrição de anti-hipertensivos",
    "templateContent": "Losartana 50mg\nTomar 1 comprimido ao dia\nPela manhã, em jejum",
    "category": "Hipertensão",
    "isActive": true
  }
]
```

**Características**:
- ✅ Templates isolados por clínica (TenantId)
- ✅ Categorização por especialidade
- ✅ Reutilizáveis em múltiplos atendimentos
- ✅ Editáveis pela própria clínica

---

## 🔐 Autenticação

Todos os endpoints requerem autenticação JWT.

**Obter Token**:
```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123",
  "tenantId": "clinic-1"
}
```

**Resposta**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-15T20:00:00Z"
}
```

**Usar Token**:
```bash
GET /api/patients/search?searchTerm=João
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📋 Casos de Uso Práticos

### Caso 1: Cadastro de Novo Paciente com Cadastro Prévio

```bash
# 1. Verificar se paciente já existe
GET /api/patients/by-document/123.456.789-00
Authorization: Bearer {token}

# Resposta: 200 OK (paciente encontrado)

# 2. Vincular à clínica atual
POST /api/patients/550e8400-e29b-41d4-a716-446655440000/link-clinic/660e8400-e29b-41d4-a716-446655440001
Authorization: Bearer {token}

# Resposta: 200 OK { "success": true }

# 3. Paciente agora disponível para agendamentos
GET /api/patients
Authorization: Bearer {token}
# Paciente aparece na lista
```

### Caso 2: Busca Rápida de Paciente

```bash
# Buscar por qualquer termo
GET /api/patients/search?searchTerm=João
Authorization: Bearer {token}

# Retorna pacientes que contenham "João" em:
# - Nome
# - CPF
# - Telefone
```

### Caso 3: Visualizar Histórico do Paciente

```bash
# Obter timeline de consultas
GET /api/medical-records/patient/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {token}

# Retorna apenas consultas da clínica atual
# Ordenado por data (mais recente primeiro)
```

---

## 🧪 Testando os Endpoints

### Com cURL

```bash
# 1. Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123",
    "tenantId": "default-tenant"
  }'

# 2. Buscar paciente (usando token do passo 1)
curl -X GET "http://localhost:5000/api/patients/search?searchTerm=Silva" \
  -H "Authorization: Bearer {seu-token-aqui}"

# 3. Buscar por CPF global
curl -X GET http://localhost:5000/api/patients/by-document/123.456.789-00 \
  -H "Authorization: Bearer {seu-token-aqui}"

# 4. Vincular paciente à clínica
curl -X POST http://localhost:5000/api/patients/{patientId}/link-clinic/{clinicId} \
  -H "Authorization: Bearer {seu-token-aqui}"
```

### Com Swagger UI

1. Acesse: `http://localhost:5000`
2. Clique em "Authorize"
3. Execute POST `/api/auth/login`
4. Copie o token retornado
5. Cole no campo "Bearer {token}" do Authorize
6. Teste os endpoints na interface

---

## 📊 Status Codes

| Código | Significado | Quando Ocorre |
|--------|-------------|---------------|
| 200 | OK | Operação bem-sucedida |
| 201 | Created | Recurso criado com sucesso |
| 204 | No Content | Operação bem-sucedida sem corpo de resposta |
| 400 | Bad Request | Dados inválidos ou faltando |
| 401 | Unauthorized | Token ausente ou inválido |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro no servidor |

---

## 🔗 Referências

- **Documentação Completa**: [BUSINESS_RULES.md](BUSINESS_RULES.md)
- **Detalhes Técnicos**: [TECHNICAL_IMPLEMENTATION.md](TECHNICAL_IMPLEMENTATION.md)
- **Editor de Texto Rico e Autocomplete**: [RICH_TEXT_EDITOR_AUTOCOMPLETE.md](RICH_TEXT_EDITOR_AUTOCOMPLETE.md)
- **README**: [README.md](README.md)

---

## 💊 Endpoints de Medicações (NOVO!)

### Busca de Medicações para Autocomplete

**Endpoint**: `GET /api/medications/search`

**Query Parameters**:
- `term` (string, obrigatório): Termo de busca (mínimo 2 caracteres)

**Exemplo**:
```bash
GET /api/medications/search?term=dipi
Authorization: Bearer {token}
X-Tenant-Id: demo-clinic-001
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "name": "Dipirona Sódica",
    "genericName": "Dipyrone",
    "dosage": "500mg",
    "pharmaceuticalForm": "Comprimido",
    "administrationRoute": "Oral",
    "displayText": "Dipirona Sódica 500mg - Comprimido"
  }
]
```

**Características**:
- ✅ Busca por nome comercial e genérico
- ✅ Limite de 20 resultados para performance
- ✅ Filtrado por TenantId
- ✅ Apenas medicações ativas

### Outros Endpoints de Medicações

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/medications` | Lista todas as medicações |
| GET | `/api/medications/{id}` | Obtém medicação por ID |
| GET | `/api/medications/category/{category}` | Lista por categoria |
| POST | `/api/medications` | Cria nova medicação |
| PUT | `/api/medications/{id}` | Atualiza medicação |
| DELETE | `/api/medications/{id}` | Desativa medicação |

---

## 🔬 Endpoints de Catálogo de Exames (NOVO!)

### Busca de Exames para Autocomplete

**Endpoint**: `GET /api/exam-catalog/search`

**Query Parameters**:
- `term` (string, obrigatório): Termo de busca (mínimo 2 caracteres)

**Exemplo**:
```bash
GET /api/exam-catalog/search?term=hemo
Authorization: Bearer {token}
X-Tenant-Id: demo-clinic-001
```

**Resposta**:
```json
[
  {
    "id": "guid",
    "name": "Hemograma Completo",
    "examType": "Laboratory",
    "category": "Hematologia",
    "preparation": "Jejum de 4 horas",
    "displayText": "Hemograma Completo"
  }
]
```

**Características**:
- ✅ Busca por nome e sinônimos
- ✅ Limite de 20 resultados para performance
- ✅ Filtrado por TenantId
- ✅ Apenas exames ativos

### Outros Endpoints de Catálogo de Exames

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/exam-catalog` | Lista todos os exames |
| GET | `/api/exam-catalog/{id}` | Obtém exame por ID |
| GET | `/api/exam-catalog/type/{examType}` | Lista por tipo |
| GET | `/api/exam-catalog/category/{category}` | Lista por categoria |
| POST | `/api/exam-catalog` | Cria novo exame |
| PUT | `/api/exam-catalog/{id}` | Atualiza exame |
| DELETE | `/api/exam-catalog/{id}` | Desativa exame |

### Tipos de Exame Disponíveis

| Tipo | Descrição |
|------|-----------|
| Laboratory | Exames laboratoriais |
| Imaging | Exames de imagem |
| Ultrasound | Ultrassonografia |
| Cardiac | Exames cardíacos |
| Endoscopy | Endoscopia |
| Biopsy | Biópsia |
| Other | Outros |

---

**Versão**: 1.1  
**Data**: Novembro 2025  
**Autor**: Equipe MedicWarehouse
