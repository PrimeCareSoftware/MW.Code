# Módulo Financeiro - Documentação Completa

## ⚠️ NOTA IMPORTANTE SOBRE NOTA FISCAL ELETRÔNICA

**Status:** Janeiro 2026  
**Decisão Pendente:** Emissão de NF-e/NFS-e

O módulo financeiro está **COMPLETO E FUNCIONAL**, com exceção da emissão oficial de notas fiscais eletrônicas (NF-e/NFS-e), que aguarda decisão estratégica sobre:

1. **Usar serviço externo (RECOMENDADO):** Focus NFe, ENotas, PlugNotas, NFSe.io
2. **Desenvolver integração própria:** Integração direta com SEFAZ

📄 **Documentação Completa da Decisão:** [DECISAO_NOTA_FISCAL.md](DECISAO_NOTA_FISCAL.md)

O sistema atual de Invoice está implementado para controle interno, mas NÃO emite notas fiscais oficiais. Para compliance total, é necessário implementar uma das opções acima.

---

## Visão Geral

O Módulo Financeiro é uma solução completa para gestão financeira de clínicas e consultórios médicos, incluindo contas a receber, contas a pagar, fluxo de caixa, fechamento de consultas e integração com TISS/TUSS.

## Índice

1. [Funcionalidades](#funcionalidades)
2. [Arquitetura](#arquitetura)
3. [Entidades do Domínio](#entidades-do-domínio)
4. [API Endpoints](#api-endpoints)
5. [Modelos de Negócio](#modelos-de-negócio)
6. [Integração TISS/TUSS](#integração-tiss-tuss)
7. [Segurança e Permissões](#segurança-e-permissões)
8. [Exemplos de Uso](#exemplos-de-uso)

## Funcionalidades

### 1. Contas a Receber (Accounts Receivable)
- ✅ Gestão completa de recebíveis
- ✅ Suporte a parcelamento
- ✅ Controle de inadimplência
- ✅ Cálculo automático de juros e multas
- ✅ Descontos para pagamento antecipado
- ✅ Histórico de pagamentos
- ✅ Vinculação com consultas e convênios

### 2. Contas a Pagar (Accounts Payable)
- ✅ Gestão de fornecedores
- ✅ Categorização de despesas
- ✅ Controle de vencimentos
- ✅ Suporte a parcelamento
- ✅ Informações bancárias e PIX
- ✅ Histórico de pagamentos

### 3. Fornecedores (Suppliers)
- ✅ Cadastro completo de fornecedores
- ✅ Informações de contato
- ✅ Dados bancários
- ✅ Controle de ativação/desativação
- ✅ Documentos (CNPJ/CPF)

### 4. Fluxo de Caixa (Cash Flow)
- ✅ Registro de entradas e saídas
- ✅ Categorização detalhada
- ✅ Relatórios por período
- ✅ Balanço automático
- ✅ Vinculação com pagamentos e recebimentos

### 5. Fechamento Financeiro (Financial Closure)
- ✅ Fechamento de consultas e procedimentos
- ✅ Divisão entre particular e convênio
- ✅ Itens detalhados
- ✅ Aplicação de descontos
- ✅ Controle de pagamentos
- ✅ Geração automática de contas a receber

## Arquitetura

### Camadas da Aplicação

```
┌─────────────────────────────────────┐
│    API Layer (Controllers)          │
│  - AccountsReceivableController     │
│  - AccountsPayableController        │
│  - SuppliersController              │
│  - CashFlowController               │
│  - FinancialClosureController       │
└─────────────────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│    Application Layer (DTOs)         │
│  - Data Transfer Objects            │
│  - Request/Response Models          │
└─────────────────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│    Domain Layer (Entities)          │
│  - AccountsReceivable               │
│  - AccountsPayable                  │
│  - Supplier                         │
│  - CashFlowEntry                    │
│  - FinancialClosure                 │
└─────────────────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│    Repository Layer (EF Core)       │
│  - Repositories                     │
│  - Entity Configurations            │
│  - Database Migrations              │
└─────────────────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│    Database (PostgreSQL)            │
└─────────────────────────────────────┘
```

## Entidades do Domínio

### AccountsReceivable (Contas a Receber)

```csharp
public class AccountsReceivable : BaseEntity
{
    public string DocumentNumber { get; }
    public ReceivableType Type { get; }
    public ReceivableStatus Status { get; }
    public DateTime DueDate { get; }
    public decimal TotalAmount { get; }
    public decimal PaidAmount { get; }
    public decimal OutstandingAmount { get; }
    
    // Parcelamento
    public int? InstallmentNumber { get; }
    public int? TotalInstallments { get; }
    
    // Penalidades e descontos
    public decimal? InterestRate { get; }
    public decimal? FineRate { get; }
    public decimal? DiscountRate { get; }
    
    // Relacionamentos
    public Guid? AppointmentId { get; }
    public Guid? PatientId { get; }
    public Guid? HealthInsuranceOperatorId { get; }
}
```

**Status disponíveis:**
- `Pending` - Pendente
- `PartiallyPaid` - Parcialmente pago
- `Paid` - Pago
- `Overdue` - Vencido
- `Cancelled` - Cancelado
- `InNegotiation` - Em negociação

**Tipos de recebível:**
- `Consultation` - Consulta
- `Procedure` - Procedimento
- `Exam` - Exame
- `HealthInsurance` - Convênio
- `Other` - Outros

### AccountsPayable (Contas a Pagar)

```csharp
public class AccountsPayable : BaseEntity
{
    public string DocumentNumber { get; }
    public Guid? SupplierId { get; }
    public PayableCategory Category { get; }
    public PayableStatus Status { get; }
    public DateTime DueDate { get; }
    public decimal TotalAmount { get; }
    public decimal PaidAmount { get; }
    public decimal OutstandingAmount { get; }
    public string Description { get; }
    
    // Informações bancárias
    public string? BankName { get; }
    public string? BankAccount { get; }
    public string? PixKey { get; }
}
```

**Categorias de despesa:**
- `Rent` - Aluguel
- `Salaries` - Salários
- `Supplies` - Materiais e suprimentos
- `Equipment` - Equipamentos
- `Maintenance` - Manutenção
- `Utilities` - Utilidades
- `Marketing` - Marketing
- `Insurance` - Seguros
- `Taxes` - Impostos
- `ProfessionalServices` - Serviços profissionais
- `Laboratory` - Laboratório
- `Pharmacy` - Farmácia
- `Other` - Outros

### CashFlowEntry (Fluxo de Caixa)

```csharp
public class CashFlowEntry : BaseEntity
{
    public CashFlowType Type { get; }          // Income/Expense
    public CashFlowCategory Category { get; }
    public DateTime TransactionDate { get; }
    public decimal Amount { get; }
    public string Description { get; }
    public string? Reference { get; }
    
    // Referências opcionais
    public Guid? PaymentId { get; }
    public Guid? ReceivableId { get; }
    public Guid? PayableId { get; }
    public Guid? AppointmentId { get; }
}
```

### FinancialClosure (Fechamento Financeiro)

```csharp
public class FinancialClosure : BaseEntity
{
    public Guid AppointmentId { get; }
    public Guid PatientId { get; }
    public string ClosureNumber { get; }
    public FinancialClosureStatus Status { get; }
    public ClosurePaymentType PaymentType { get; }
    public decimal TotalAmount { get; }
    public decimal PatientAmount { get; }      // Valor particular
    public decimal InsuranceAmount { get; }    // Valor convênio
    public decimal PaidAmount { get; }
    public decimal OutstandingAmount { get; }
    public IReadOnlyCollection<FinancialClosureItem> Items { get; }
}
```

**Status do fechamento:**
- `Open` - Aberto
- `PendingPayment` - Aguardando pagamento
- `PartiallyPaid` - Parcialmente pago
- `Closed` - Fechado
- `Cancelled` - Cancelado

**Tipos de pagamento:**
- `OutOfPocket` - Particular
- `HealthInsurance` - Convênio
- `Mixed` - Misto (particular + convênio)

## API Endpoints

### Contas a Receber

#### GET `/api/accounts-receivable`
Lista todas as contas a receber.

**Permissão:** `AccountsReceivableView`

**Resposta:**
```json
[
  {
    "id": "guid",
    "documentNumber": "REC-2024-001",
    "type": "Consultation",
    "status": "Pending",
    "issueDate": "2024-01-15T10:00:00Z",
    "dueDate": "2024-02-15T00:00:00Z",
    "totalAmount": 350.00,
    "paidAmount": 0,
    "outstandingAmount": 350.00,
    "patientId": "guid",
    "appointmentId": "guid"
  }
]
```

#### POST `/api/accounts-receivable`
Cria uma nova conta a receber.

**Permissão:** `AccountsReceivableManage`

**Request:**
```json
{
  "documentNumber": "REC-2024-001",
  "type": "Consultation",
  "dueDate": "2024-02-15",
  "totalAmount": 350.00,
  "description": "Consulta de cardiologia",
  "patientId": "guid",
  "appointmentId": "guid",
  "installmentNumber": 1,
  "totalInstallments": 3,
  "interestRate": 2.0,
  "fineRate": 10.0,
  "discountRate": 5.0
}
```

#### POST `/api/accounts-receivable/{id}/payments`
Adiciona um pagamento a uma conta a receber.

**Permissão:** `AccountsReceivableManage`

**Request:**
```json
{
  "amount": 350.00,
  "paymentDate": "2024-02-10T14:30:00Z",
  "transactionId": "TXN-123456",
  "notes": "Pagamento via PIX"
}
```

#### GET `/api/accounts-receivable/overdue`
Lista contas vencidas.

**Permissão:** `AccountsReceivableView`

#### GET `/api/accounts-receivable/total-outstanding`
Retorna o total em aberto.

**Permissão:** `AccountsReceivableView`

**Resposta:**
```json
{
  "totalOutstanding": 15750.00
}
```

### Contas a Pagar

#### GET `/api/accounts-payable`
Lista todas as contas a pagar.

**Permissão:** `AccountsPayableView`

#### POST `/api/accounts-payable`
Cria uma nova conta a pagar.

**Permissão:** `AccountsPayableManage`

**Request:**
```json
{
  "documentNumber": "PAG-2024-001",
  "category": "Supplies",
  "dueDate": "2024-02-20",
  "totalAmount": 1200.00,
  "description": "Material médico",
  "supplierId": "guid",
  "installmentNumber": 1,
  "totalInstallments": 2
}
```

#### GET `/api/accounts-payable/by-category/{category}`
Lista contas por categoria.

**Permissão:** `AccountsPayableView`

### Fornecedores

#### GET `/api/suppliers`
Lista todos os fornecedores.

**Permissão:** `SuppliersView`

#### POST `/api/suppliers`
Cria um novo fornecedor.

**Permissão:** `SuppliersManage`

**Request:**
```json
{
  "name": "Fornecedor Médico LTDA",
  "tradeName": "Fornecedor Médico",
  "documentNumber": "12.345.678/0001-90",
  "email": "contato@fornecedor.com",
  "phone": "(11) 98765-4321",
  "address": "Rua das Flores, 123",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567"
}
```

#### GET `/api/suppliers/active`
Lista apenas fornecedores ativos.

**Permissão:** `SuppliersView`

### Fluxo de Caixa

#### GET `/api/cash-flow/summary`
Retorna resumo do fluxo de caixa.

**Permissão:** `CashFlowView`

**Query params:**
- `startDate` (obrigatório)
- `endDate` (obrigatório)

**Resposta:**
```json
{
  "startDate": "2024-01-01",
  "endDate": "2024-01-31",
  "totalIncome": 45000.00,
  "totalExpense": 18500.00,
  "balance": 26500.00,
  "incomeByCategory": {
    "ConsultationPayment": 30000.00,
    "ProcedurePayment": 15000.00
  },
  "expenseByCategory": {
    "Rent": 5000.00,
    "Salaries": 10000.00,
    "Supplies": 3500.00
  }
}
```

#### POST `/api/cash-flow`
Cria uma nova entrada no fluxo de caixa.

**Permissão:** `CashFlowManage`

**Request:**
```json
{
  "type": "Income",
  "category": "ConsultationPayment",
  "transactionDate": "2024-01-15T14:30:00Z",
  "amount": 350.00,
  "description": "Pagamento consulta - Dr. Silva",
  "reference": "REC-2024-001",
  "paymentId": "guid",
  "appointmentId": "guid"
}
```

### Fechamento Financeiro

#### GET `/api/financial-closures`
Lista todos os fechamentos.

**Permissão:** `FinancialClosureView`

#### POST `/api/financial-closures`
Cria um novo fechamento.

**Permissão:** `FinancialClosureManage`

**Request:**
```json
{
  "appointmentId": "guid",
  "patientId": "guid",
  "closureNumber": "CLOSE-2024-001",
  "paymentType": "Mixed",
  "healthInsuranceOperatorId": "guid"
}
```

#### POST `/api/financial-closures/{id}/items`
Adiciona um item ao fechamento.

**Permissão:** `FinancialClosureManage`

**Request:**
```json
{
  "description": "Consulta cardiológica",
  "quantity": 1,
  "unitPrice": 350.00,
  "coverByInsurance": false
}
```

#### POST `/api/financial-closures/{id}/apply-discount`
Aplica desconto ao fechamento.

**Permissão:** `FinancialClosureManage`

**Request:**
```json
{
  "discountAmount": 50.00,
  "reason": "Desconto de cortesia"
}
```

#### POST `/api/financial-closures/{id}/record-payment`
Registra um pagamento no fechamento.

**Permissão:** `FinancialClosureManage`

**Request:**
```json
{
  "amount": 300.00
}
```

## Modelos de Negócio

### Fluxo de Trabalho - Consulta Particular

1. **Agendamento da consulta** → Cria Appointment
2. **Atendimento** → Registra procedimentos e materiais
3. **Fechamento financeiro** → Cria FinancialClosure
4. **Adiciona itens** → Adiciona FinancialClosureItem
5. **Marca como pendente** → Status: PendingPayment
6. **Recebe pagamento** → Cria Payment e atualiza FinancialClosure
7. **Gera conta a receber** (se parcelado) → Cria AccountsReceivable
8. **Registra no fluxo de caixa** → Cria CashFlowEntry

### Fluxo de Trabalho - Consulta com Convênio

1. **Agendamento da consulta** → Cria Appointment com HealthInsuranceOperator
2. **Atendimento** → Registra procedimentos
3. **Fechamento financeiro** → Cria FinancialClosure (tipo: HealthInsurance)
4. **Adiciona itens** → Marca como coberto pelo convênio
5. **Gera guia TISS** → Cria TissGuide
6. **Envia para operadora** → Status: Sent
7. **Recebe retorno** → Processa aprovação/glosa
8. **Cria conta a receber** → AccountsReceivable para o valor aprovado
9. **Recebe pagamento** → Atualiza AccountsReceivable e registra no fluxo de caixa

### Fluxo de Trabalho - Contas a Pagar

1. **Recebe nota fiscal** → Registra no sistema
2. **Cria conta a pagar** → AccountsPayable
3. **Agenda pagamento** → Define DueDate
4. **Efetua pagamento** → Adiciona PayablePayment
5. **Registra no fluxo de caixa** → CashFlowEntry (tipo: Expense)
6. **Atualiza status** → Status: Paid

## Integração TISS/TUSS

O módulo financeiro se integra com o sistema TISS/TUSS existente:

### Entidades TISS Relacionadas

- **TissBatch**: Lote de guias
- **TissGuide**: Guia individual
- **TissGuideProcedure**: Procedimentos da guia
- **TussProcedure**: Tabela TUSS de procedimentos

### Fluxo de Integração

```
FinancialClosure → TissGuide → TissGuideProcedure
                      ↓
                 TissBatch (envio para operadora)
                      ↓
              Retorno da operadora
                      ↓
         AccountsReceivable (valor aprovado)
                      ↓
              CashFlowEntry (recebimento)
```

### Controle de Glosas

O sistema permite:
- Registrar valores glosados
- Motivo da glosa
- Recurso de glosa
- Análise de rentabilidade considerando glosas

## Segurança e Permissões

### Permissões do Módulo Financeiro

| Permissão | Descrição | Recursos Permitidos |
|-----------|-----------|---------------------|
| `AccountsReceivableView` | Visualizar contas a receber | GET endpoints de receivables |
| `AccountsReceivableManage` | Gerenciar contas a receber | POST, PUT, DELETE de receivables |
| `AccountsPayableView` | Visualizar contas a pagar | GET endpoints de payables |
| `AccountsPayableManage` | Gerenciar contas a pagar | POST, PUT, DELETE de payables |
| `SuppliersView` | Visualizar fornecedores | GET endpoints de suppliers |
| `SuppliersManage` | Gerenciar fornecedores | POST, PUT, DELETE de suppliers |
| `CashFlowView` | Visualizar fluxo de caixa | GET endpoints de cash flow |
| `CashFlowManage` | Gerenciar fluxo de caixa | POST, PUT, DELETE de cash flow |
| `FinancialClosureView` | Visualizar fechamentos | GET endpoints de closures |
| `FinancialClosureManage` | Gerenciar fechamentos | POST, PUT, DELETE de closures |

### Multi-tenancy

Todas as entidades são isoladas por tenant:
- Cada requisição filtra automaticamente por `TenantId`
- Impossível acessar dados de outros tenants
- Repositories implementam filtros obrigatórios

## Exemplos de Uso

### Exemplo 1: Criar e Receber Conta

```bash
# 1. Criar conta a receber
POST /api/accounts-receivable
{
  "documentNumber": "REC-2024-001",
  "type": "Consultation",
  "dueDate": "2024-02-15",
  "totalAmount": 350.00,
  "patientId": "patient-guid",
  "appointmentId": "appointment-guid"
}

# 2. Adicionar pagamento
POST /api/accounts-receivable/{id}/payments
{
  "amount": 350.00,
  "paymentDate": "2024-02-10T14:30:00Z",
  "transactionId": "PIX-123456"
}

# 3. Verificar status
GET /api/accounts-receivable/{id}
# Status será "Paid"
```

### Exemplo 2: Fechamento de Consulta

```bash
# 1. Criar fechamento
POST /api/financial-closures
{
  "appointmentId": "appointment-guid",
  "patientId": "patient-guid",
  "closureNumber": "CLOSE-2024-001",
  "paymentType": "OutOfPocket"
}

# 2. Adicionar itens
POST /api/financial-closures/{id}/items
{
  "description": "Consulta cardiológica",
  "quantity": 1,
  "unitPrice": 350.00
}

POST /api/financial-closures/{id}/items
{
  "description": "ECG",
  "quantity": 1,
  "unitPrice": 80.00
}

# 3. Aplicar desconto
POST /api/financial-closures/{id}/apply-discount
{
  "discountAmount": 30.00,
  "reason": "Paciente fidelizado"
}

# 4. Registrar pagamento
POST /api/financial-closures/{id}/record-payment
{
  "amount": 400.00
}
# Total: 350 + 80 - 30 = 400
# Status será "Closed"
```

### Exemplo 3: Relatório de Fluxo de Caixa

```bash
# Obter resumo mensal
GET /api/cash-flow/summary?startDate=2024-01-01&endDate=2024-01-31

# Resposta
{
  "totalIncome": 45000.00,
  "totalExpense": 18500.00,
  "balance": 26500.00,
  "incomeByCategory": {
    "ConsultationPayment": 30000.00,
    "ProcedurePayment": 15000.00
  },
  "expenseByCategory": {
    "Rent": 5000.00,
    "Salaries": 10000.00,
    "Supplies": 3500.00
  }
}
```

### Exemplo 4: Controle de Fornecedores

```bash
# 1. Criar fornecedor
POST /api/suppliers
{
  "name": "Fornecedor Médico LTDA",
  "documentNumber": "12.345.678/0001-90",
  "email": "contato@fornecedor.com",
  "phone": "(11) 98765-4321",
  "pixKey": "12345678000190"
}

# 2. Criar conta a pagar
POST /api/accounts-payable
{
  "documentNumber": "NF-12345",
  "category": "Supplies",
  "dueDate": "2024-02-20",
  "totalAmount": 1200.00,
  "description": "Material cirúrgico",
  "supplierId": "supplier-guid"
}

# 3. Listar contas por fornecedor
GET /api/accounts-payable/by-supplier/{supplierId}
```

### DRE - Demonstrativo de Resultados

#### GET `/api/reports/dre`
Gera o DRE (Demonstrativo de Resultados do Exercício) para análise financeira.

**Permissão:** `ReportsFinancial`

**Query params:**
- `clinicId` (obrigatório) - ID da clínica
- `startDate` (obrigatório) - Data inicial do período
- `endDate` (obrigatório) - Data final do período

**Resposta:**
```json
{
  "periodStart": "2024-01-01",
  "periodEnd": "2024-01-31",
  "grossRevenue": 50000.00,
  "deductions": 1000.00,
  "netRevenue": 49000.00,
  "operationalCosts": 8000.00,
  "administrativeExpenses": 12000.00,
  "salesExpenses": 2000.00,
  "financialExpenses": 3000.00,
  "totalExpenses": 25000.00,
  "operationalProfit": 24000.00,
  "netProfit": 24000.00,
  "profitMargin": 48.98,
  "revenueDetails": [
    {
      "category": "CreditCard",
      "amount": 30000.00,
      "percentage": 60.00
    },
    {
      "category": "Pix",
      "amount": 15000.00,
      "percentage": 30.00
    }
  ],
  "expenseDetails": [
    {
      "category": "Salary",
      "amount": 10000.00,
      "percentage": 40.00
    },
    {
      "category": "Rent",
      "amount": 5000.00,
      "percentage": 20.00
    }
  ]
}
```

### Previsão de Fluxo de Caixa

#### GET `/api/reports/cash-flow-forecast`
Gera projeção de fluxo de caixa baseada em contas a receber e pagar pendentes.

**Permissão:** `ReportsFinancial`

**Query params:**
- `clinicId` (obrigatório) - ID da clínica
- `months` (opcional, padrão: 3) - Número de meses a projetar (1-12)

**Resposta:**
```json
{
  "startDate": "2024-01-22",
  "endDate": "2024-04-22",
  "currentBalance": 15000.00,
  "projectedIncome": 45000.00,
  "projectedExpenses": 20000.00,
  "projectedBalance": 40000.00,
  "monthlyForecast": [
    {
      "year": 2024,
      "month": 2,
      "expectedIncome": 15000.00,
      "expectedExpenses": 8000.00,
      "expectedBalance": 7000.00,
      "cumulativeBalance": 22000.00
    },
    {
      "year": 2024,
      "month": 3,
      "expectedIncome": 18000.00,
      "expectedExpenses": 7000.00,
      "expectedBalance": 11000.00,
      "cumulativeBalance": 33000.00
    }
  ],
  "pendingReceivables": [
    {
      "id": "guid",
      "documentNumber": "REC-2024-001",
      "dueDate": "2024-02-15",
      "amount": 350.00,
      "status": "Pending",
      "patientName": "João Silva"
    }
  ],
  "pendingPayables": [
    {
      "id": "guid",
      "documentNumber": "PAG-2024-001",
      "dueDate": "2024-02-20",
      "amount": 1200.00,
      "category": "Supplies",
      "supplierName": "Fornecedor Médico LTDA"
    }
  ]
}
```

### Análise de Rentabilidade

#### GET `/api/reports/profitability`
Analisa a rentabilidade por procedimento, médico e convênio.

**Permissão:** `ReportsFinancial`

**Query params:**
- `clinicId` (obrigatório) - ID da clínica
- `startDate` (obrigatório) - Data inicial do período
- `endDate` (obrigatório) - Data final do período

**Resposta:**
```json
{
  "periodStart": "2024-01-01",
  "periodEnd": "2024-01-31",
  "totalRevenue": 45000.00,
  "totalCosts": 18500.00,
  "totalProfit": 26500.00,
  "profitMargin": 58.89,
  "byProcedure": [
    {
      "procedureName": "Consultation",
      "count": 50,
      "revenue": 25000.00,
      "averageValue": 500.00,
      "percentage": 55.56
    },
    {
      "procedureName": "Exam",
      "count": 30,
      "revenue": 15000.00,
      "averageValue": 500.00,
      "percentage": 33.33
    }
  ],
  "byDoctor": [
    {
      "doctorId": "guid",
      "doctorName": "Dr. João Silva",
      "appointmentsCount": 35,
      "revenue": 20000.00,
      "averageAppointmentValue": 571.43,
      "percentage": 44.44
    },
    {
      "doctorId": "guid",
      "doctorName": "Dra. Maria Santos",
      "appointmentsCount": 25,
      "revenue": 15000.00,
      "averageAppointmentValue": 600.00,
      "percentage": 33.33
    }
  ],
  "byInsurance": [
    {
      "insuranceId": null,
      "insuranceName": "Particular",
      "appointmentsCount": 40,
      "revenue": 25000.00,
      "averageValue": 625.00,
      "percentage": 55.56
    },
    {
      "insuranceId": "guid",
      "insuranceName": "Unimed",
      "appointmentsCount": 20,
      "revenue": 12000.00,
      "averageValue": 600.00,
      "percentage": 26.67
    }
  ]
}
```

## Próximos Passos

### Funcionalidades Pendentes

1. **Relatórios Financeiros**
   - [x] DRE (Demonstrativo de Resultados) ✅ **Janeiro 2026**
   - [x] Previsão de fluxo de caixa ✅ **Janeiro 2026**
   - [x] Rentabilidade por procedimento ✅ **Janeiro 2026**
   - [ ] Análise de inadimplência com dashboard
   - [ ] Dashboard financeiro frontend

2. **Automações**
   - [ ] Geração automática de contas a receber após fechamento
   - [ ] Alertas de vencimento
   - [ ] Reconciliação bancária
   - [ ] Alertas de fluxo de caixa negativo

3. **Integrações**
   - [ ] Gateway de pagamento (Stripe, MercadoPago)
   - [ ] Bancos (OFX, API bancária)
   - [ ] Nota fiscal eletrônica
   - [ ] Conciliação automática

4. **Melhorias TISS/TUSS**
   - [ ] Campos financeiros adicionais nas guias
   - [ ] Controle de repasses
   - [ ] Análise de glosas
   - [ ] Relatórios de rentabilidade por convênio

## Suporte

Para dúvidas ou suporte, consulte:
- Documentação técnica do projeto
- Issues no GitHub
- Equipe de desenvolvimento

---

**Versão:** 1.1.0  
**Data:** Janeiro 2026  
**Última atualização:** 22 de Janeiro de 2026  
**Autor:** Sistema MedicWare
