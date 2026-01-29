# API de Receitas Médicas Digitais - Guia de Referência Rápida

**Versão:** 1.0  
**Data:** 29 de Janeiro de 2026  
**Base URL:** `/api`

## 🚀 Quick Start

### 1. Criar Prescrição
```http
POST /api/DigitalPrescriptions
Content-Type: application/json

{
  "type": "SpecialControlB",
  "doctorName": "Dr. João Silva",
  "patientName": "Maria Santos",
  "items": [...]
}
```

### 2. Download PDF
```http
GET /api/DigitalPrescriptions/{id}/pdf
```

### 3. Export XML ANVISA
```http
GET /api/DigitalPrescriptions/{id}/xml
```

## 📊 Endpoints Principais

### Digital Prescriptions Controller

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/DigitalPrescriptions` | Criar prescrição |
| GET | `/api/DigitalPrescriptions/{id}` | Buscar por ID |
| GET | `/api/DigitalPrescriptions/{id}/pdf` | Download PDF |
| GET | `/api/DigitalPrescriptions/{id}/pdf/preview` | Preview PDF |
| GET | `/api/DigitalPrescriptions/{id}/xml` | Export XML ANVISA |
| GET | `/api/DigitalPrescriptions/verify/{code}` | Verificar por QR Code |
| POST | `/api/DigitalPrescriptions/{id}/sign` | Assinar digitalmente |

### SNGPC Reports Controller

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/SNGPCReports` | Criar relatório mensal |
| POST | `/api/SNGPCReports/{id}/generate-xml` | Gerar XML ANVISA |
| GET | `/api/SNGPCReports/{id}/download-xml` | Download XML |
| GET | `/api/SNGPCReports/overdue` | Relatórios vencidos |
| GET | `/api/SNGPCReports/active-alerts` | Alertas ativos |

## 🔐 Autenticação

Todas as rotas requerem Bearer Token (exceto `/verify`):

```http
Authorization: Bearer {token}
```

## 📄 Tipos de Prescrição

- `Simple` - Receita simples (30 dias)
- `SpecialControlA` - Lista A (Entorpecentes)
- `SpecialControlB` - Lista B (Psicotrópicos)
- `SpecialControlC1` - Lista C1 (Outros controlados)
- `Antimicrobial` - Antimicrobianos (10 dias)

---

**Documentação Completa:** Veja os controllers no código-fonte para detalhes de todos os endpoints e modelos.
