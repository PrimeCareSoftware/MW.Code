# 📘 TISS API Reference - Complete Endpoint Documentation

**Version:** 1.0  
**Last Updated:** Janeiro 2026  
**Status:** ✅ PRODUCTION-READY  
**Base URL:** `/api/`

---

## 📑 Table of Contents

1. [Operadoras de Saúde (Health Insurance Operators)](#operadoras)
2. [Planos de Saúde (Health Insurance Plans)](#planos)
3. [Vínculo Paciente-Plano (Patient Insurance)](#vinculo-paciente)
4. [Procedimentos TUSS](#procedimentos-tuss)
5. [Solicitações de Autorização](#autorizacoes)
6. [Guias TISS](#guias-tiss)
7. [Lotes de Faturamento (Batches)](#lotes)
8. [Recursos de Glosa](#recursos-glosa)
9. [Relatórios e Analytics](#relatorios)

---

## 🏥 Operadoras de Saúde {#operadoras}

### Lista de Operadoras

```http
GET /api/health-insurance-operators
```

**Query Parameters:**
- `page` (int, optional): Número da página (default: 1)
- `pageSize` (int, optional): Itens por página (default: 10, max: 100)
- `searchTerm` (string, optional): Busca por nome ou ANS
- `isActive` (bool, optional): Filtrar por ativas/inativas

**Response 200:**
```json
{
  "data": [
    {
      "id": "uuid",
      "ansRegistrationNumber": "123456",
      "tradeName": "Unimed São Paulo",
      "cnpj": "12.345.678/0001-90",
      "isActive": true,
      "supportsTissWebservice": true,
      "webserviceUrl": "https://ws.unimed.com.br/tiss",
      "averageGlossaRate": 15.5,
      "totalBilled": 150000.00,
      "totalGlossed": 23250.00
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5
}
```

### Criar Operadora

```http
POST /api/health-insurance-operators
```

**Request Body:**
```json
{
  "ansRegistrationNumber": "123456",
  "tradeName": "Unimed São Paulo",
  "legalName": "Unimed Cooperativa SP",
  "cnpj": "12.345.678/0001-90",
  "phone": "(11) 3000-0000",
  "email": "contato@unimedsp.com.br",
  "supportsTissWebservice": true,
  "webserviceUrl": "https://ws.unimed.com.br/tiss",
  "webserviceAuthType": "Basic",
  "isActive": true
}
```

**Response 201:**
```json
{
  "id": "uuid",
  "ansRegistrationNumber": "123456",
  "tradeName": "Unimed São Paulo",
  "createdAt": "2026-01-29T15:00:00Z"
}
```

### Atualizar Operadora

```http
PUT /api/health-insurance-operators/{id}
```

### Obter Operadora por ID

```http
GET /api/health-insurance-operators/{id}
```

### Deletar Operadora

```http
DELETE /api/health-insurance-operators/{id}
```

**Note:** Soft delete - marca como inativa

---

## 🏥 Planos de Saúde {#planos}

### Listar Planos

```http
GET /api/health-insurance-plans
```

**Query Parameters:**
- `operatorId` (uuid, optional): Filtrar por operadora
- `planType` (string, optional): Individual, Coletivo, Empresarial
- `isActive` (bool, optional)

**Response 200:**
```json
{
  "data": [
    {
      "id": "uuid",
      "operatorId": "uuid",
      "operatorName": "Unimed São Paulo",
      "planCode": "UNI-100",
      "planName": "Unimed Premium",
      "planType": "Empresarial",
      "coverageType": "Ambulatorial + Hospitalar",
      "requiresAuthorization": true,
      "averageAuthorizationTime": "24h",
      "isActive": true
    }
  ]
}
```

### Criar Plano

```http
POST /api/health-insurance-plans
```

**Request Body:**
```json
{
  "operatorId": "uuid",
  "planCode": "UNI-100",
  "planName": "Unimed Premium",
  "planType": "Empresarial",
  "coverageType": "Ambulatorial + Hospitalar",
  "requiresAuthorization": true,
  "coParticipation": true,
  "coParticipationPercentage": 30.0,
  "isActive": true
}
```

---

## 👤 Vínculo Paciente-Plano {#vinculo-paciente}

### Listar Planos do Paciente

```http
GET /api/patients/{patientId}/health-insurance
```

**Response 200:**
```json
{
  "data": [
    {
      "id": "uuid",
      "patientId": "uuid",
      "planId": "uuid",
      "planName": "Unimed Premium",
      "operatorName": "Unimed São Paulo",
      "cardNumber": "1234567890123456",
      "validFrom": "2025-01-01",
      "validUntil": "2026-12-31",
      "isPrimary": true,
      "isActive": true,
      "holder": {
        "name": "João Silva",
        "cpf": "123.456.789-00",
        "relationship": "Titular"
      }
    }
  ]
}
```

### Adicionar Plano ao Paciente

```http
POST /api/patients/{patientId}/health-insurance
```

**Request Body:**
```json
{
  "planId": "uuid",
  "cardNumber": "1234567890123456",
  "validFrom": "2025-01-01",
  "validUntil": "2026-12-31",
  "isPrimary": true,
  "holderName": "João Silva",
  "holderCpf": "123.456.789-00",
  "relationship": "Titular"
}
```

### Validar Carteirinha

```http
POST /api/patients/health-insurance/validate
```

**Request Body:**
```json
{
  "cardNumber": "1234567890123456",
  "patientCpf": "123.456.789-00",
  "operatorId": "uuid"
}
```

**Response 200:**
```json
{
  "isValid": true,
  "validUntil": "2026-12-31",
  "planName": "Unimed Premium",
  "status": "Ativo",
  "message": "Carteirinha válida"
}
```

---

## 📋 Procedimentos TUSS {#procedimentos-tuss}

### Buscar Procedimentos

```http
GET /api/tuss-procedures
```

**Query Parameters:**
- `searchTerm` (string): Busca por código ou descrição
- `category` (string, optional): Categoria do procedimento
- `page` (int)
- `pageSize` (int)

**Response 200:**
```json
{
  "data": [
    {
      "id": "uuid",
      "code": "10101012",
      "description": "Consulta em consultório",
      "category": "Consultas",
      "price": 150.00,
      "requiresAuthorization": false,
      "coverageType": "Ambulatorial"
    }
  ]
}
```

### Importar Tabela TUSS

```http
POST /api/tuss-procedures/import
Content-Type: multipart/form-data
```

**Form Data:**
- `file`: CSV ou Excel com procedimentos TUSS
- `replaceExisting` (bool): Substituir procedimentos existentes

**Response 200:**
```json
{
  "totalImported": 1523,
  "totalUpdated": 234,
  "totalSkipped": 12,
  "errors": []
}
```

---

## 🔐 Solicitações de Autorização {#autorizacoes}

### Criar Solicitação de Autorização

```http
POST /api/authorization-requests
```

**Request Body:**
```json
{
  "patientId": "uuid",
  "patientHealthInsuranceId": "uuid",
  "appointmentId": "uuid",
  "requestDate": "2026-01-29",
  "requestedProcedures": [
    {
      "tussProcedureId": "uuid",
      "quantity": 1,
      "justification": "Paciente com sintomas..."
    }
  ],
  "clinicalIndication": "CID-10: M54.5 - Lombalgia",
  "urgencyLevel": "Eletivo"
}
```

**Response 201:**
```json
{
  "id": "uuid",
  "authorizationNumber": "AUTH-2026-00123",
  "status": "Pendente",
  "requestedAt": "2026-01-29T15:00:00Z",
  "estimatedResponseTime": "48h"
}
```

### Listar Autorizações

```http
GET /api/authorization-requests
```

**Query Parameters:**
- `status` (string): Pendente, Autorizado, Negado, Expirado
- `operatorId` (uuid)
- `patientId` (uuid)
- `dateFrom` (date)
- `dateTo` (date)

### Atualizar Status de Autorização

```http
PUT /api/authorization-requests/{id}/status
```

**Request Body:**
```json
{
  "status": "Autorizado",
  "authorizationCode": "12345678",
  "validUntil": "2026-03-31",
  "notes": "Autorizado para até 10 sessões"
}
```

---

## 📄 Guias TISS {#guias-tiss}

### Criar Guia TISS

```http
POST /api/tiss-guides
```

**Request Body:**
```json
{
  "patientHealthInsuranceId": "uuid",
  "appointmentId": "uuid",
  "guideType": "SP-SADT",
  "guideDate": "2026-01-29",
  "authorizationRequestId": "uuid",
  "authorizationCode": "12345678",
  "procedures": [
    {
      "tussProcedureId": "uuid",
      "quantity": 1,
      "unitPrice": 150.00,
      "totalPrice": 150.00,
      "executionDate": "2026-01-29"
    }
  ],
  "attendingPhysicianId": "uuid",
  "cid10Code": "M54.5"
}
```

**Response 201:**
```json
{
  "id": "uuid",
  "guideNumber": "GUIDE-2026-00456",
  "guideType": "SP-SADT",
  "status": "Criada",
  "totalAmount": 150.00,
  "createdAt": "2026-01-29T15:00:00Z"
}
```

### Listar Guias

```http
GET /api/tiss-guides
```

**Query Parameters:**
- `status` (string): Criada, EmLote, Enviada, Paga, Glosada
- `guideType` (string): SP-SADT, Consulta, Internacao, Honorarios
- `operatorId` (uuid)
- `patientId` (uuid)
- `dateFrom` (date)
- `dateTo` (date)

### Obter Guia por ID

```http
GET /api/tiss-guides/{id}
```

**Response 200:**
```json
{
  "id": "uuid",
  "guideNumber": "GUIDE-2026-00456",
  "guideType": "SP-SADT",
  "status": "Criada",
  "patient": {
    "id": "uuid",
    "name": "João Silva",
    "cpf": "123.456.789-00"
  },
  "operator": {
    "id": "uuid",
    "name": "Unimed São Paulo"
  },
  "procedures": [
    {
      "code": "10101012",
      "description": "Consulta em consultório",
      "quantity": 1,
      "unitPrice": 150.00,
      "totalPrice": 150.00
    }
  ],
  "totalAmount": 150.00,
  "glosaAmount": 0.00,
  "paidAmount": 0.00
}
```

---

## 📦 Lotes de Faturamento {#lotes}

### Criar Lote de Faturamento

```http
POST /api/tiss-batches
```

**Request Body:**
```json
{
  "operatorId": "uuid",
  "referenceMonth": "2026-01",
  "guideIds": ["uuid1", "uuid2", "uuid3"],
  "batchType": "Faturamento"
}
```

**Response 201:**
```json
{
  "id": "uuid",
  "batchNumber": "BATCH-2026-00012",
  "operatorName": "Unimed São Paulo",
  "referenceMonth": "2026-01",
  "totalGuides": 3,
  "totalAmount": 450.00,
  "status": "Criado",
  "createdAt": "2026-01-29T15:00:00Z"
}
```

### Gerar XML do Lote

```http
POST /api/tiss-batches/{id}/generate-xml
```

**Response 200:**
```json
{
  "xmlContent": "<?xml version=\"1.0\"...",
  "fileName": "BATCH-2026-00012.xml",
  "fileSize": 15360,
  "validationStatus": "Válido",
  "schemaVersion": "4.02.00"
}
```

### Validar XML do Lote

```http
POST /api/tiss-batches/{id}/validate-xml
```

**Response 200:**
```json
{
  "isValid": true,
  "schemaVersion": "4.02.00",
  "errors": [],
  "warnings": [
    "Campo opcional 'observacoes' não preenchido"
  ]
}
```

### Download XML do Lote

```http
GET /api/tiss-batches/{id}/download-xml
```

**Response 200:**
- Content-Type: `application/xml`
- Content-Disposition: `attachment; filename="BATCH-2026-00012.xml"`

### Enviar Lote

```http
POST /api/tiss-batches/{id}/send
```

**Request Body:**
```json
{
  "sendMethod": "Webservice",
  "sendDate": "2026-01-29T15:00:00Z"
}
```

**Response 200:**
```json
{
  "success": true,
  "protocolNumber": "PROTO-2026-XYZ123",
  "sentAt": "2026-01-29T15:00:00Z",
  "message": "Lote enviado com sucesso via webservice"
}
```

### Registrar Retorno do Lote

```http
POST /api/tiss-batches/{id}/register-return
```

**Request Body:**
```json
{
  "returnDate": "2026-02-05",
  "returnFile": "base64_encoded_xml",
  "returnType": "Aprovado"
}
```

### Listar Lotes

```http
GET /api/tiss-batches
```

**Query Parameters:**
- `status` (string): Criado, Enviado, EmProcessamento, Aprovado, RejeitadoParcial, Rejeitado
- `operatorId` (uuid)
- `referenceMonth` (string): YYYY-MM
- `dateFrom` (date)
- `dateTo` (date)

---

## 🔄 Recursos de Glosa {#recursos-glosa}

### Criar Recurso de Glosa

```http
POST /api/tiss-glosa-appeals
```

**Request Body:**
```json
{
  "glosaId": "uuid",
  "appealDate": "2026-02-10",
  "justification": "Procedimento foi autorizado previamente...",
  "supportingDocuments": [
    {
      "documentType": "Autorização Prévia",
      "documentUrl": "/files/auth-12345.pdf"
    }
  ]
}
```

### Listar Recursos

```http
GET /api/tiss-glosa-appeals
```

### Atualizar Status do Recurso

```http
PUT /api/tiss-glosa-appeals/{id}/status
```

---

## 📊 Relatórios e Analytics {#relatorios}

### Dashboard de Faturamento

```http
GET /api/tiss/reports/billing-dashboard
```

**Query Parameters:**
- `operatorId` (uuid, optional)
- `startDate` (date)
- `endDate` (date)

**Response 200:**
```json
{
  "totalBilled": 250000.00,
  "totalPaid": 210000.00,
  "totalGlossed": 40000.00,
  "glossaRate": 16.0,
  "totalGuides": 1520,
  "totalBatches": 12,
  "averageTicket": 164.47,
  "byOperator": [
    {
      "operatorName": "Unimed São Paulo",
      "totalBilled": 150000.00,
      "totalPaid": 130000.00,
      "glossaRate": 13.3
    }
  ]
}
```

### Relatório de Glosas

```http
GET /api/tiss/reports/glosas
```

**Query Parameters:**
- `operatorId` (uuid, optional)
- `startDate` (date)
- `endDate` (date)
- `glossaType` (string, optional): Administrativa, Técnica, Financeira

**Response 200:**
```json
{
  "totalGlosas": 125,
  "totalAmount": 40000.00,
  "byType": [
    {
      "type": "Administrativa",
      "count": 80,
      "amount": 25000.00,
      "percentage": 62.5
    },
    {
      "type": "Técnica",
      "count": 45,
      "amount": 15000.00,
      "percentage": 37.5
    }
  ],
  "topReasons": [
    {
      "reason": "Falta de autorização prévia",
      "count": 40,
      "amount": 12000.00
    }
  ]
}
```

### Relatório de Performance por Operadora

```http
GET /api/tiss/reports/operator-performance
```

**Response 200:**
```json
{
  "operators": [
    {
      "operatorName": "Unimed São Paulo",
      "totalGuides": 850,
      "totalBilled": 150000.00,
      "averageGlossaRate": 13.3,
      "averagePaymentTime": "32 dias",
      "authorizationApprovalRate": 92.5
    }
  ]
}
```

### Relatório de Procedimentos Mais Faturados

```http
GET /api/tiss/reports/top-procedures
```

**Query Parameters:**
- `top` (int, default: 10)
- `startDate` (date)
- `endDate` (date)

**Response 200:**
```json
{
  "procedures": [
    {
      "code": "10101012",
      "description": "Consulta em consultório",
      "totalQuantity": 1200,
      "totalAmount": 180000.00,
      "averagePrice": 150.00
    }
  ]
}
```

---

## 🔒 Autenticação e Autorização

Todos os endpoints requerem autenticação via Bearer Token:

```http
Authorization: Bearer {jwt_token}
```

### Permissões Necessárias

| Endpoint | Permissão Requerida |
|----------|-------------------|
| Operadoras (Write) | `tiss:operators:write` |
| Operadoras (Read) | `tiss:operators:read` |
| Guias (Write) | `tiss:guides:write` |
| Guias (Read) | `tiss:guides:read` |
| Lotes (Write) | `tiss:batches:write` |
| Lotes (Read) | `tiss:batches:read` |
| Relatórios | `tiss:reports:read` |

---

## ⚠️ Tratamento de Erros

### Códigos de Status HTTP

- `200 OK` - Sucesso
- `201 Created` - Recurso criado com sucesso
- `400 Bad Request` - Requisição inválida
- `401 Unauthorized` - Token inválido ou ausente
- `403 Forbidden` - Sem permissão para ação
- `404 Not Found` - Recurso não encontrado
- `409 Conflict` - Conflito de estado (ex: lote já enviado)
- `422 Unprocessable Entity` - Validação falhou
- `500 Internal Server Error` - Erro no servidor

### Formato de Erro Padrão

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Dados inválidos",
    "details": [
      {
        "field": "cardNumber",
        "message": "Número de carteirinha inválido"
      }
    ]
  }
}
```

---

## 📚 Recursos Adicionais

- **Guia do Usuário:** [GUIA_USUARIO_TISS.md](../guias/GUIA_USUARIO_TISS.md)
- **Status da Implementação:** [TISS_IMPLEMENTATION_STATUS.md](../implementacoes/TISS_IMPLEMENTATION_STATUS.md)
- **Testes:** [TISS_TEST_COVERAGE_PLAN.md](../regras-negocio/TISS_TEST_COVERAGE_PLAN.md)
- **Índice de Documentação:** [TISS_DOCUMENTATION_INDEX.md](../regras-negocio/TISS_DOCUMENTATION_INDEX.md)

---

## 🆘 Suporte

Para dúvidas ou problemas:
1. Consulte o [Guia de Troubleshooting](#)
2. Verifique os logs da aplicação
3. Entre em contato com o time de desenvolvimento

**Última Atualização:** Janeiro 2026  
**Versão da API:** 1.0  
**Versão TISS:** 4.02.00 (ANS)
