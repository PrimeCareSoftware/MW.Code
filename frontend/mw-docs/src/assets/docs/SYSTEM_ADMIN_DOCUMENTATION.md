# Área do Administrador do Sistema (System Owner)

## Visão Geral

A área do **System Owner** (Administrador do Sistema) permite que você, como dono do sistema MedicWarehouse, gerencie todas as clínicas, usuários, assinaturas e tenha acesso a analytics do sistema completo.

## Características

- ✅ Acesso cross-tenant (todas as clínicas)
- ✅ Gerenciamento de assinaturas
- ✅ Analytics e métricas globais
- ✅ Gestão de planos e preços
- ✅ Criação de outros administradores do sistema
- ✅ Ativação/Desativação de clínicas
- ✅ Suporte e troubleshooting

---

## Autenticação

O System Owner usa credenciais especiais com role `SystemAdmin` e tenant `"system"`.

```bash
POST /api/auth/login
{
  "username": "owner",
  "password": "SecureOwnerPassword123!",
  "tenantId": "system"
}
```

---

## API Endpoints

### 1. Listar Todas as Clínicas

**Endpoint**: `GET /api/system-admin/clinics`

**Query Parameters**:
- `status` (opcional): "active" ou "inactive"
- `page` (opcional): número da página (padrão: 1)
- `pageSize` (opcional): itens por página (padrão: 20)

**Request**:
```bash
GET /api/system-admin/clinics?status=active&page=1&pageSize=20
Authorization: Bearer {system_admin_token}
```

**Response**:
```json
{
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3,
  "clinics": [
    {
      "id": "a1b2c3d4-...",
      "name": "Clínica Sorriso Feliz",
      "document": "12.345.678/0001-90",
      "email": "contato@clinicasorriso.com",
      "phone": "+5511999999999",
      "address": "Rua das Flores, 123 - São Paulo/SP",
      "isActive": true,
      "tenantId": "clinic-abc-123",
      "createdAt": "2025-01-15T10:30:00Z",
      "subscriptionStatus": "Active",
      "planName": "Premium",
      "nextBillingDate": "2025-11-15T00:00:00Z"
    },
    // ... mais clínicas
  ]
}
```

**Casos de Uso**:
- Ver todas as clínicas cadastradas
- Filtrar clínicas ativas/inativas
- Monitorar status de assinaturas
- Identificar clínicas com problemas de pagamento

---

### 2. Detalhes de uma Clínica

**Endpoint**: `GET /api/system-admin/clinics/{id}`

**Request**:
```bash
GET /api/system-admin/clinics/a1b2c3d4-e5f6-7890-abcd-ef1234567890
Authorization: Bearer {system_admin_token}
```

**Response**:
```json
{
  "id": "a1b2c3d4-...",
  "name": "Clínica Sorriso Feliz",
  "document": "12.345.678/0001-90",
  "email": "contato@clinicasorriso.com",
  "phone": "+5511999999999",
  "address": "Rua das Flores, 123, Sala 10 - Centro - São Paulo/SP - 01234-567",
  "isActive": true,
  "tenantId": "clinic-abc-123",
  "createdAt": "2025-01-15T10:30:00Z",
  "subscriptionStatus": "Active",
  "planName": "Premium",
  "planPrice": 320.00,
  "nextBillingDate": "2025-11-15T00:00:00Z",
  "trialEndsAt": null,
  "totalUsers": 5,
  "activeUsers": 4
}
```

**Informações Incluídas**:
- Dados completos da clínica
- Status da assinatura
- Plano atual e preço
- Número de usuários (total e ativos)
- Datas importantes

---

### 3. Atualizar Assinatura de uma Clínica

**Endpoint**: `PUT /api/system-admin/clinics/{id}/subscription`

**Request**:
```bash
PUT /api/system-admin/clinics/a1b2c3d4-e5f6-7890-abcd-ef1234567890/subscription
Authorization: Bearer {system_admin_token}
Content-Type: application/json

{
  "newPlanId": "premium-plan-id",
  "status": "Active"
}
```

**Response**:
```json
{
  "message": "Assinatura atualizada com sucesso"
}
```

**Casos de Uso**:
- Fazer upgrade/downgrade manual
- Reativar assinatura suspensa
- Aplicar desconto especial
- Resolver problemas de cobrança

---

### 4. Ativar/Desativar Clínica

**Endpoint**: `POST /api/system-admin/clinics/{id}/toggle-status`

**Request**:
```bash
POST /api/system-admin/clinics/a1b2c3d4-e5f6-7890-abcd-ef1234567890/toggle-status
Authorization: Bearer {system_admin_token}
```

**Response**:
```json
{
  "message": "Clínica desativada com sucesso",
  "isActive": false
}
```

**Quando Usar**:
- Suspender clínica por falta de pagamento
- Desativar clínica que cancelou serviço
- Reativar após resolução de problemas

---

### 5. Analytics do Sistema

**Endpoint**: `GET /api/system-admin/analytics`

**Request**:
```bash
GET /api/system-admin/analytics
Authorization: Bearer {system_admin_token}
```

**Response**:
```json
{
  "totalClinics": 45,
  "activeClinics": 42,
  "inactiveClinics": 3,
  "totalUsers": 215,
  "activeUsers": 198,
  "totalPatients": 8750,
  "monthlyRecurringRevenue": 12480.00,
  "subscriptionsByStatus": [
    { "status": "Active", "count": 38 },
    { "status": "Trial", "count": 4 },
    { "status": "PaymentOverdue", "count": 2 },
    { "status": "Suspended", "count": 1 }
  ],
  "subscriptionsByPlan": [
    { "plan": "Basic", "count": 15 },
    { "plan": "Standard", "count": 18 },
    { "plan": "Premium", "count": 10 },
    { "plan": "Enterprise", "count": 2 }
  ]
}
```

**Métricas Importantes**:
- **MRR (Monthly Recurring Revenue)**: Receita recorrente mensal
- **Churn Rate**: Taxa de cancelamento
- **Growth Rate**: Taxa de crescimento
- **Trial Conversion**: Conversão de trial para pago
- **ARPU**: Receita média por usuário

**Dashboards Sugeridos**:
1. Overview financeiro (MRR, previsão)
2. Saúde das clínicas (ativas vs inativas)
3. Distribuição por planos
4. Timeline de crescimento

---

### 6. Criar Novo Administrador do Sistema

**Endpoint**: `POST /api/system-admin/users`

**Request**:
```bash
POST /api/system-admin/users
Authorization: Bearer {system_admin_token}
Content-Type: application/json

{
  "username": "carlos.admin",
  "email": "carlos@medicwarehouse.com",
  "password": "SecureAdminPass123!",
  "fullName": "Carlos Silva",
  "phone": "+5511988887777"
}
```

**Response**:
```json
{
  "message": "Administrador do sistema criado com sucesso",
  "userId": "new-admin-id",
  "username": "carlos.admin"
}
```

**Permissões de SystemAdmin**:
- Acesso a todas as clínicas (cross-tenant)
- Gerenciar assinaturas e planos
- Criar outros system admins
- Ver analytics globais
- Fazer troubleshooting
- Acesso a logs de auditoria

---

### 7. Listar Todos os Planos

**Endpoint**: `GET /api/system-admin/plans`

**Request**:
```bash
GET /api/system-admin/plans
Authorization: Bearer {system_admin_token}
```

**Response**:
```json
[
  {
    "id": "basic-plan-id",
    "name": "Básico",
    "description": "Plano para clínicas pequenas",
    "monthlyPrice": 190.00,
    "maxUsers": 2,
    "maxPatients": 100,
    "hasReports": false,
    "hasWhatsApp": false,
    "hasSMS": false,
    "hasTISS": false,
    "isActive": true
  },
  {
    "id": "premium-plan-id",
    "name": "Premium",
    "description": "Todos os recursos incluídos",
    "monthlyPrice": 320.00,
    "maxUsers": 5,
    "maxPatients": -1, // ilimitado
    "hasReports": true,
    "hasWhatsApp": true,
    "hasSMS": true,
    "hasTISS": true,
    "isActive": true
  }
]
```

---

## Fluxo de Trabalho Típico

### 1. Monitoramento Diário

```bash
# 1. Ver analytics gerais
GET /api/system-admin/analytics

# 2. Verificar clínicas com pagamento atrasado
GET /api/system-admin/clinics?status=payment_overdue

# 3. Analisar trials próximos do fim
GET /api/system-admin/clinics?status=trial
```

### 2. Onboarding de Nova Clínica

Quando uma nova clínica se cadastra pelo site:

1. Sistema cria automaticamente:
   - Registro da clínica
   - Assinatura (trial por 15 dias)
   - Primeiro usuário (ClinicOwner)
   - Tenant isolado

2. System Owner valida:
   ```bash
   GET /api/system-admin/clinics/{new_clinic_id}
   ```

3. Se necessário, ajusta plano manualmente

### 3. Suporte a Cliente

```bash
# 1. Buscar clínica por CNPJ ou nome
GET /api/system-admin/clinics?search=12.345.678/0001-90

# 2. Ver detalhes completos
GET /api/system-admin/clinics/{clinic_id}

# 3. Verificar usuários e status
# (informações incluídas no detalhe)

# 4. Ajustar assinatura se necessário
PUT /api/system-admin/clinics/{clinic_id}/subscription
```

### 4. Resolução de Problemas de Pagamento

```bash
# 1. Identificar clínicas com pagamento atrasado
GET /api/system-admin/clinics?status=payment_overdue

# 2. Após resolução, reativar assinatura
PUT /api/system-admin/clinics/{clinic_id}/subscription
{
  "status": "Active"
}
```

---

## Segurança e Controle de Acesso

### Verificação de Role

Todos os endpoints do SystemAdmin verificam automaticamente se o usuário tem role `SystemAdmin`:

```csharp
// No controller
var userRole = User.FindFirst("role")?.Value;
if (userRole != "SystemAdmin") 
    return Forbid();
```

### Cross-Tenant Access

System Admins podem acessar dados de qualquer tenant usando `IgnoreQueryFilters()`:

```csharp
var allClinics = await _context.Clinics
    .IgnoreQueryFilters()  // Bypass tenant isolation
    .ToListAsync();
```

### Auditoria

Todas as ações de System Admin devem ser logadas:

```csharp
_auditLog.Log(new AuditEntry
{
    UserId = currentUserId,
    Action = "ClinicDeactivated",
    TargetId = clinicId,
    Timestamp = DateTime.UtcNow,
    Details = "Clínica desativada por falta de pagamento"
});
```

---

## Best Practices

### 1. Monitoramento Proativo

- Configure alertas para:
  - Clínicas com pagamento atrasado > 3 dias
  - Trials terminando em 3 dias
  - Taxa de churn > 5% ao mês
  - MRR drop > 10%

### 2. Customer Success

- Entre em contato com clínicas antes do trial acabar
- Ofereça ajuda para clínicas com baixo uso
- Colete feedback de clínicas que cancelaram

### 3. Pricing Strategy

- Revise preços trimestralmente
- Analise elasticidade de preço
- Teste preços A/B para novos clientes

### 4. Segurança

- Mínimo de 2 System Admins (redundância)
- MFA obrigatório para System Admins
- Revisar logs de auditoria semanalmente
- Revogar acesso de ex-funcionários imediatamente

---

## Relatórios Úteis

### 1. Relatório de MRR

```sql
SELECT 
    sp.Name as Plan,
    COUNT(*) as Clinics,
    SUM(cs.CurrentPrice) as MRR,
    AVG(cs.CurrentPrice) as ARPU
FROM ClinicSubscriptions cs
JOIN SubscriptionPlans sp ON cs.SubscriptionPlanId = sp.Id
WHERE cs.Status = 'Active'
GROUP BY sp.Name
ORDER BY MRR DESC
```

### 2. Relatório de Churn

```sql
SELECT 
    DATEPART(YEAR, CancellationDate) as Year,
    DATEPART(MONTH, CancellationDate) as Month,
    COUNT(*) as Cancellations,
    AVG(DATEDIFF(day, StartDate, CancellationDate)) as AvgLifetimeDays
FROM ClinicSubscriptions
WHERE Status = 'Cancelled'
  AND CancellationDate >= DATEADD(month, -12, GETDATE())
GROUP BY DATEPART(YEAR, CancellationDate), DATEPART(MONTH, CancellationDate)
ORDER BY Year DESC, Month DESC
```

### 3. Trial Conversion Rate

```sql
SELECT 
    COUNT(CASE WHEN Status = 'Trial' THEN 1 END) as TrialCount,
    COUNT(CASE WHEN Status = 'Active' AND TrialEndDate IS NOT NULL THEN 1 END) as ConvertedCount,
    CAST(COUNT(CASE WHEN Status = 'Active' AND TrialEndDate IS NOT NULL THEN 1 END) * 100.0 
         / NULLIF(COUNT(*), 0) as decimal(5,2)) as ConversionRate
FROM ClinicSubscriptions
WHERE CreatedAt >= DATEADD(month, -3, GETDATE())
```

---

## Troubleshooting Common Issues

### Clínica não consegue fazer login

```bash
# 1. Verificar se clínica está ativa
GET /api/system-admin/clinics/{clinic_id}

# 2. Se inativa, reativar
POST /api/system-admin/clinics/{clinic_id}/toggle-status

# 3. Verificar status da assinatura
# Se suspensa, reativar
PUT /api/system-admin/clinics/{clinic_id}/subscription
```

### Problema de cobrança

```bash
# 1. Ver detalhes da assinatura
GET /api/system-admin/clinics/{clinic_id}

# 2. Verificar histórico de pagamentos
GET /api/payments?clinicId={clinic_id}

# 3. Manualmente marcar como pago
POST /api/payments/{payment_id}/mark-paid
```

### Cliente quer downgrade imediato

```bash
# Normalmente downgrade é no próximo ciclo,
# mas System Admin pode forçar:
PUT /api/system-admin/clinics/{clinic_id}/subscription
{
  "newPlanId": "basic-plan-id",
  "status": "Active"
}
```

---

## Roadmap de Melhorias

### Fase 1 (Implementado) ✅
- Listar todas as clínicas
- Ver detalhes de clínica
- Ativar/Desativar clínicas
- Analytics básicos
- Criar system admins

### Fase 2 (Planejado) 📋
- Dashboard visual com gráficos
- Relatórios exportáveis (Excel/PDF)
- Sistema de alertas automáticos
- Chat interno para suporte
- Timeline de eventos da clínica

### Fase 3 (Futuro) 🚀
- Machine Learning para prever churn
- Recomendações de upgrade automáticas
- A/B testing de preços
- Customer health score
- Integração com CRM

---

## Conclusão

A área de System Owner é o centro de controle do MedicWarehouse, permitindo gestão completa de todas as clínicas, usuários e assinaturas. Use essas ferramentas para:

- 📊 Monitorar saúde do negócio
- 💰 Otimizar receita (MRR)
- 🎯 Melhorar retenção de clientes
- 🛠️ Resolver problemas rapidamente
- 📈 Crescer o negócio de forma sustentável

**Lembre-se**: Com grandes poderes vêm grandes responsabilidades. Use o acesso cross-tenant com cuidado e sempre respeite a privacidade dos dados das clínicas.

---

**Última Atualização**: 2025-10-11  
**Versão**: 1.0  
**Responsável**: System Owner  
**Status**: ✅ Implementado

