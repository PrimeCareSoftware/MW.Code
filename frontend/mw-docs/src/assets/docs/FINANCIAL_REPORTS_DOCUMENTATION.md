# Sistema de Gestão Financeira e Relatórios - PrimeCare Software

## 📊 Visão Geral

O sistema de gestão financeira e relatórios do PrimeCare Software oferece controle completo sobre receitas, despesas e análises operacionais. Com dashboards intuitivos e relatórios detalhados, você pode tomar decisões baseadas em dados reais.

---

## 💼 Gestão de Despesas (Contas a Pagar)

### Funcionalidades

- ✅ CRUD completo de despesas
- ✅ Categorização automática
- ✅ Controle de vencimento
- ✅ Alertas de despesas vencidas
- ✅ Cadastro de fornecedores
- ✅ Múltiplos métodos de pagamento
- ✅ Histórico completo de transações

### Categorias de Despesas

```typescript
enum ExpenseCategory {
  Rent = 1,              // Aluguel
  Utilities = 2,         // Utilidades (água, luz, internet)
  Supplies = 3,          // Materiais e suprimentos
  Equipment = 4,         // Equipamentos
  Maintenance = 5,       // Manutenção
  Marketing = 6,         // Marketing e publicidade
  Software = 7,          // Software e assinaturas
  Salary = 8,            // Salários e folha de pagamento
  Taxes = 9,             // Impostos e taxas
  Insurance = 10,        // Seguros
  ProfessionalServices = 11, // Serviços profissionais
  Transportation = 12,   // Transporte
  Training = 13,         // Treinamento e educação
  Other = 14             // Outros
}
```

### Status de Despesas

```typescript
enum ExpenseStatus {
  Pending = 1,   // Pendente
  Paid = 2,      // Pago
  Overdue = 3,   // Vencido
  Cancelled = 4  // Cancelado
}
```

### API Endpoints

#### Criar Despesa

```bash
POST /api/expenses
Content-Type: application/json
X-Tenant-Id: clinica-exemplo

{
  "clinicId": "guid-da-clinica",
  "description": "Aluguel do consultório - Setembro 2025",
  "category": "Rent",
  "amount": 3500.00,
  "dueDate": "2025-09-10",
  "supplierName": "Imobiliária XYZ",
  "supplierDocument": "12.345.678/0001-99",
  "notes": "Pagamento via transferência bancária"
}
```

**Resposta (201 Created):**
```json
{
  "id": "expense-guid",
  "clinicId": "guid-da-clinica",
  "description": "Aluguel do consultório - Setembro 2025",
  "category": "Rent",
  "amount": 3500.00,
  "dueDate": "2025-09-10",
  "status": "Pending",
  "supplierName": "Imobiliária XYZ",
  "supplierDocument": "12.345.678/0001-99",
  "notes": "Pagamento via transferência bancária",
  "createdAt": "2025-10-10T19:00:00Z"
}
```

#### Listar Despesas

```bash
GET /api/expenses?clinicId={guid}&status=Pending&category=Rent
```

**Resposta (200 OK):**
```json
[
  {
    "id": "expense-guid",
    "clinicId": "guid-da-clinica",
    "description": "Aluguel do consultório - Setembro 2025",
    "category": "Rent",
    "amount": 3500.00,
    "dueDate": "2025-09-10",
    "status": "Pending",
    "supplierName": "Imobiliária XYZ",
    "daysOverdue": null,
    "createdAt": "2025-10-10T19:00:00Z"
  }
]
```

#### Marcar Despesa como Paga

```bash
PUT /api/expenses/{id}/pay
Content-Type: application/json

{
  "paymentMethod": "BankTransfer",
  "paymentReference": "TRF-123456"
}
```

**Resposta (204 No Content)**

#### Cancelar Despesa

```bash
PUT /api/expenses/{id}/cancel
Content-Type: application/json

{
  "reason": "Serviço não foi realizado"
}
```

**Resposta (204 No Content)**

---

## 📊 Relatórios Financeiros

### 1. Resumo Financeiro

Fornece uma visão completa da saúde financeira da clínica em um período específico.

```bash
GET /api/reports/financial-summary?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30
```

**Resposta:**
```json
{
  "periodStart": "2025-09-01",
  "periodEnd": "2025-09-30",
  "totalRevenue": 45000.00,
  "totalExpenses": 12500.00,
  "netProfit": 32500.00,
  "totalAppointments": 150,
  "totalPatients": 98,
  "averageAppointmentValue": 300.00,
  "revenueByPaymentMethod": [
    {
      "paymentMethod": "Pix",
      "amount": 20000.00,
      "count": 67,
      "percentage": 44.4
    },
    {
      "paymentMethod": "CreditCard",
      "amount": 15000.00,
      "count": 50,
      "percentage": 33.3
    },
    {
      "paymentMethod": "Cash",
      "amount": 10000.00,
      "count": 33,
      "percentage": 22.3
    }
  ],
  "expensesByCategory": [
    {
      "category": "Rent",
      "amount": 3500.00,
      "count": 1,
      "percentage": 28.0
    },
    {
      "category": "Salary",
      "amount": 6000.00,
      "count": 3,
      "percentage": 48.0
    },
    {
      "category": "Supplies",
      "amount": 3000.00,
      "count": 12,
      "percentage": 24.0
    }
  ]
}
```

### 2. Relatório de Receita

Detalha a receita diária do período.

```bash
GET /api/reports/revenue?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30
```

**Resposta:**
```json
{
  "periodStart": "2025-09-01",
  "periodEnd": "2025-09-30",
  "totalRevenue": 45000.00,
  "totalTransactions": 150,
  "dailyBreakdown": [
    {
      "date": "2025-09-01",
      "revenue": 1200.00,
      "transactions": 4
    },
    {
      "date": "2025-09-02",
      "revenue": 1800.00,
      "transactions": 6
    }
  ]
}
```

### 3. Relatório de Agendamentos

Estatísticas sobre consultas realizadas.

```bash
GET /api/reports/appointments?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30
```

**Resposta:**
```json
{
  "periodStart": "2025-09-01",
  "periodEnd": "2025-09-30",
  "totalAppointments": 150,
  "completedAppointments": 135,
  "cancelledAppointments": 10,
  "noShowAppointments": 5,
  "completionRate": 90.0,
  "cancellationRate": 6.67,
  "appointmentsByStatus": [
    {
      "status": "Completed",
      "count": 135,
      "percentage": 90.0
    },
    {
      "status": "Cancelled",
      "count": 10,
      "percentage": 6.67
    },
    {
      "status": "NoShow",
      "count": 5,
      "percentage": 3.33
    }
  ],
  "appointmentsByType": [
    {
      "type": "Regular",
      "count": 100,
      "percentage": 66.67
    },
    {
      "type": "Return",
      "count": 30,
      "percentage": 20.0
    },
    {
      "type": "Emergency",
      "count": 20,
      "percentage": 13.33
    }
  ]
}
```

### 4. Relatório de Pacientes

Crescimento da base de pacientes.

```bash
GET /api/reports/patients?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30
```

**Resposta:**
```json
{
  "periodStart": "2025-09-01",
  "periodEnd": "2025-09-30",
  "totalPatients": 450,
  "newPatients": 25,
  "activePatients": 98,
  "monthlyBreakdown": [
    {
      "year": 2025,
      "month": 9,
      "newPatients": 25,
      "totalPatients": 450
    }
  ]
}
```

### 5. Contas a Receber

Controle de pagamentos pendentes.

```bash
GET /api/reports/accounts-receivable?clinicId={guid}
```

**Resposta:**
```json
{
  "totalPending": 15000.00,
  "totalOverdue": 3500.00,
  "pendingCount": 25,
  "overdueCount": 5,
  "overdueInvoices": [
    {
      "invoiceId": "invoice-guid",
      "invoiceNumber": "NF-2025-001",
      "amount": 1500.00,
      "dueDate": "2025-08-15",
      "daysOverdue": 26,
      "patientName": "João Silva"
    }
  ]
}
```

### 6. Contas a Pagar

Controle de despesas pendentes.

```bash
GET /api/reports/accounts-payable?clinicId={guid}
```

**Resposta:**
```json
{
  "totalPending": 8500.00,
  "totalOverdue": 2000.00,
  "pendingCount": 12,
  "overdueCount": 2,
  "overdueExpenses": [
    {
      "expenseId": "expense-guid",
      "description": "Material de limpeza",
      "category": "Supplies",
      "amount": 1000.00,
      "dueDate": "2025-09-05",
      "daysOverdue": 5,
      "supplierName": "Distribuidora ABC"
    }
  ]
}
```

---

## 🎯 Casos de Uso

### Caso 1: Controle Mensal de Despesas

**Objetivo:** Acompanhar todas as despesas do mês e garantir pagamentos em dia.

```bash
# 1. Listar despesas pendentes
GET /api/expenses?clinicId={guid}&status=Pending

# 2. Verificar despesas vencidas
GET /api/reports/accounts-payable?clinicId={guid}

# 3. Pagar despesa
PUT /api/expenses/{id}/pay
{
  "paymentMethod": "BankTransfer",
  "paymentReference": "TRF-123456"
}
```

### Caso 2: Análise Financeira Mensal

**Objetivo:** Avaliar o desempenho financeiro do mês.

```bash
# 1. Obter resumo financeiro
GET /api/reports/financial-summary?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30

# 2. Analisar receita diária
GET /api/reports/revenue?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30

# 3. Verificar contas a receber e a pagar
GET /api/reports/accounts-receivable?clinicId={guid}
GET /api/reports/accounts-payable?clinicId={guid}
```

### Caso 3: Acompanhamento de Performance

**Objetivo:** Avaliar produtividade e crescimento da clínica.

```bash
# 1. Relatório de agendamentos
GET /api/reports/appointments?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30

# 2. Relatório de crescimento de pacientes
GET /api/reports/patients?clinicId={guid}&startDate=2025-09-01&endDate=2025-09-30
```

---

## 📈 KPIs Disponíveis

### Financeiros
- **Receita Total**: Soma de todos os pagamentos recebidos
- **Despesas Totais**: Soma de todas as despesas pagas
- **Lucro Líquido**: Receita - Despesas
- **Ticket Médio**: Receita Total / Número de Consultas
- **Contas a Receber**: Total de pagamentos pendentes
- **Contas a Pagar**: Total de despesas pendentes

### Operacionais
- **Taxa de Conclusão**: Consultas completadas / Total de consultas
- **Taxa de Cancelamento**: Consultas canceladas / Total de consultas
- **Taxa de No-Show**: Faltas / Total de consultas
- **Pacientes Ativos**: Pacientes com consultas no período
- **Novos Pacientes**: Pacientes cadastrados no período

### Distribuição
- **Receita por Método de Pagamento**: Percentual de cada método
- **Despesas por Categoria**: Percentual de cada categoria
- **Consultas por Tipo**: Distribuição de tipos de consulta
- **Consultas por Status**: Distribuição de status

---

## 🔐 Segurança e Permissões

Todos os endpoints de relatórios e despesas:
- ✅ Requerem autenticação JWT
- ✅ Validam TenantId (multitenancy)
- ✅ Aplicam filtros de acesso por clínica
- ✅ Registram auditoria de operações

---

## 📱 Próximos Passos - Frontend

Para completar a implementação, os seguintes componentes frontend devem ser criados:

1. **Dashboard Financeiro**
   - Cards com KPIs principais
   - Gráficos de receita e despesas
   - Alertas de contas vencidas

2. **Tela de Despesas**
   - Lista de despesas com filtros
   - Formulário de cadastro/edição
   - Ações de pagar e cancelar

3. **Tela de Relatórios**
   - Seletor de período
   - Visualização de relatórios
   - Exportação de dados (PDF/Excel)

4. **Componentes de Gráficos**
   - Gráfico de linha (receita diária)
   - Gráfico de pizza (distribuição)
   - Gráfico de barras (comparativos)

---

## 📚 Referências

- [Payment System Documentation](PAYMENT_FLOW.md)
- [Invoice System Documentation](IMPLEMENTATION_PAYMENT_SYSTEM.md)
- [API Quick Guide](API_QUICK_GUIDE.md)
- [Business Rules](BUSINESS_RULES.md)
