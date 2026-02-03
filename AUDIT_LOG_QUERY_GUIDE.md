# 📊 Guia de Consulta de Audit Logs - Omni Care

**Sistema de Auditoria e Rastreabilidade**  
**Versão:** 1.0  
**Atualizado:** Janeiro 2026

---

## 📋 Sumário

1. [Visão Geral](#visão-geral)
2. [Estrutura do Audit Log](#estrutura-do-audit-log)
3. [Consultas Comuns](#consultas-comuns)
4. [Filtros e Buscas](#filtros-e-buscas)
5. [Relatórios](#relatórios)
6. [Análise de Segurança](#análise-de-segurança)
7. [LGPD e Compliance](#lgpd-e-compliance)
8. [Exemplos Práticos](#exemplos-práticos)

---

## 🎯 Visão Geral

### O que são Audit Logs?

Audit Logs são registros completos de todas as ações realizadas no sistema, incluindo:
- Quem fez
- O que fez
- Quando fez
- Onde fez (IP, dispositivo)
- Resultado (sucesso/falha)
- Dados alterados (before/after)

### Por que são Importantes?

✅ **Compliance LGPD** - Demonstrar conformidade (Art. 6, X)  
✅ **Segurança** - Detectar acessos não autorizados  
✅ **Auditoria** - Rastrear mudanças em dados críticos  
✅ **Troubleshooting** - Investigar problemas  
✅ **Accountability** - Responsabilização de ações

### Cobertura

**100% das operações são registradas:**
- ✅ Autenticação (login, logout, MFA)
- ✅ Acesso a dados sensíveis
- ✅ Criação, edição e exclusão
- ✅ Exportação e downloads
- ✅ Mudanças de permissões
- ✅ Configurações de segurança

---

## 🏗️ Estrutura do Audit Log

### Modelo de Dados

```csharp
public class AuditLog
{
    // Identificação
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string TenantId { get; set; }
    
    // Quem
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    
    // O quê
    public AuditAction Action { get; set; }
    public string ActionDescription { get; set; }
    public OperationResult Result { get; set; }
    
    // Onde
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string EntityDisplayName { get; set; }
    
    // Como
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string RequestPath { get; set; }
    public string HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    
    // Mudanças
    public string OldValues { get; set; }     // JSON
    public string NewValues { get; set; }     // JSON
    public List<string> ChangedFields { get; set; }
    
    // Metadados LGPD
    public DataCategory DataCategory { get; set; }
    public LgpdPurpose Purpose { get; set; }
    public AuditSeverity Severity { get; set; }
    
    // Extras
    public string FailureReason { get; set; }
}
```

### Ações Disponíveis (AuditAction)

#### Autenticação
- `LOGIN` - Login realizado
- `LOGOUT` - Logout realizado
- `LOGIN_FAILED` - Falha no login
- `PASSWORD_CHANGED` - Senha alterada
- `PASSWORD_RESET_REQUESTED` - Recuperação solicitada
- `MFA_ENABLED` - MFA habilitado
- `MFA_DISABLED` - MFA desabilitado
- `MFA_VERIFIED` - Código MFA verificado

#### Autorização
- `ACCESS_DENIED` - Acesso negado (403)
- `PERMISSION_CHANGED` - Permissões alteradas
- `ROLE_CHANGED` - Papel do usuário alterado

#### CRUD
- `CREATE` - Registro criado
- `READ` - Registro acessado
- `UPDATE` - Registro atualizado
- `DELETE` - Registro excluído

#### LGPD
- `EXPORT` - Dados exportados
- `DOWNLOAD` - Arquivo baixado
- `PRINT` - Documento impresso
- `DATA_ACCESS_REQUEST` - Solicitação de acesso
- `DATA_DELETION_REQUEST` - Solicitação de exclusão
- `DATA_PORTABILITY_REQUEST` - Portabilidade
- `DATA_CORRECTION_REQUEST` - Correção de dados
- `DATA_ANONYMIZED` - Dados anonimizados

### Níveis de Severidade

| Severidade | Descrição | Cor | Uso |
|------------|-----------|-----|-----|
| `INFO` | Operações normais | 🟢 Azul | Leitura, criação |
| `WARNING` | Operações sensíveis | 🟡 Amarelo | Falhas, tentativas bloqueadas |
| `CRITICAL` | Operações críticas | 🔴 Vermelho | Exclusões, mudanças de segurança |

### Categorias de Dados

- `PERSONAL` - Dados pessoais comuns (nome, email, CPF)
- `SENSITIVE` - Dados sensíveis (saúde, biometria)
- `FINANCIAL` - Dados financeiros (pagamentos, contas)
- `CLINICAL` - Dados clínicos (prontuários, diagnósticos)
- `BEHAVIORAL` - Dados comportamentais (navegação, uso)

### Finalidades LGPD

- `HEALTHCARE` - Tutela da saúde
- `LEGAL_OBLIGATION` - Obrigação legal
- `LEGITIMATE_INTEREST` - Interesse legítimo
- `CONSENT` - Consentimento do titular
- `CONTRACT_EXECUTION` - Execução de contrato

---

## 🔍 Consultas Comuns

### 1. Atividade de um Usuário

**Objetivo:** Ver tudo que um usuário específico fez

```csharp
// Backend
var logs = await _auditService.GetUserActivityAsync(
    userId: "123e4567-e89b-12d3-a456-426614174000",
    startDate: DateTime.UtcNow.AddDays(-30),
    endDate: DateTime.UtcNow,
    tenantId: tenantId
);

// Filtrar por ação
var loginAttempts = logs.Where(l => 
    l.Action == "LOGIN" || 
    l.Action == "LOGIN_FAILED"
);

// Agrupar por dia
var activityByDay = logs
    .GroupBy(l => l.Timestamp.Date)
    .Select(g => new {
        Date = g.Key,
        Count = g.Count(),
        Actions = g.GroupBy(l => l.Action)
    });
```

**Frontend - Requisição:**
```http
GET /api/audit/users/123e4567-e89b-12d3-a456-426614174000/activity
    ?startDate=2026-01-01
    &endDate=2026-01-31
```

**Casos de Uso:**
- Investigar comportamento suspeito
- Relatório de produtividade
- Compliance LGPD (direito de acesso)

---

### 2. Histórico de uma Entidade

**Objetivo:** Ver todas as mudanças em um paciente, usuário, etc.

```csharp
// Backend
var history = await _auditService.GetEntityHistoryAsync(
    entityType: "Patient",
    entityId: "patient-guid-here",
    tenantId: tenantId
);

// Ver apenas modificações
var modifications = history.Where(l => 
    l.Action == "UPDATE" && 
    l.ChangedFields.Any()
);

// Ver quem acessou
var accessors = history
    .Where(l => l.Action == "READ")
    .Select(l => new {
        l.UserName,
        l.Timestamp,
        l.IpAddress
    })
    .Distinct();
```

**Frontend - Requisição:**
```http
GET /api/audit/patients/abc-123/history
```

**Resposta JSON:**
```json
{
  "entityType": "Patient",
  "entityId": "abc-123",
  "entityName": "João Silva",
  "totalEvents": 45,
  "events": [
    {
      "timestamp": "2026-01-15T14:30:00Z",
      "action": "UPDATE",
      "userName": "Dr. Maria Santos",
      "changedFields": ["phone", "address"],
      "oldValues": {
        "phone": "(11) 1111-1111",
        "address": "Rua A, 123"
      },
      "newValues": {
        "phone": "(11) 2222-2222",
        "address": "Rua B, 456"
      }
    }
  ]
}
```

**Casos de Uso:**
- Auditoria de dados de pacientes
- Compliance médico (CFM 1821/2007)
- Investigação de alterações

---

### 3. Eventos de Segurança

**Objetivo:** Detectar acessos não autorizados, tentativas de invasão

```csharp
// Backend
var securityEvents = await _auditService.GetSecurityEventsAsync(
    startDate: DateTime.UtcNow.AddDays(-7),
    endDate: DateTime.UtcNow,
    tenantId: tenantId
);

// Filtrar por severidade
var criticalEvents = securityEvents.Where(l => 
    l.Severity == AuditSeverity.CRITICAL
);

// Tentativas de login falhadas
var failedLogins = securityEvents
    .Where(l => l.Action == AuditAction.LOGIN_FAILED)
    .GroupBy(l => l.UserId)
    .Where(g => g.Count() > 5) // 5+ falhas
    .Select(g => new {
        UserId = g.Key,
        Attempts = g.Count(),
        LastAttempt = g.Max(l => l.Timestamp),
        IPs = g.Select(l => l.IpAddress).Distinct()
    });
```

**Frontend - Dashboard de Segurança:**
```http
GET /api/audit/security-events?days=7
```

**Alertas Automáticos:**
- 5+ login failures em 1 hora → Possível brute force
- Acesso de novo país → Login suspeito
- Acesso a dados sensíveis fora do horário → Investigar
- Múltiplos acessos negados → Tentativa de escalação de privilégios

---

### 4. Relatório LGPD

**Objetivo:** Demonstrar compliance com a LGPD

```csharp
// Backend
var lgpdReport = await _auditService.GenerateLgpdReportAsync(
    userId: "user-id",
    tenantId: tenantId
);

// Estrutura do relatório
public class AuditReport
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTime GeneratedAt { get; set; }
    
    // Métricas
    public int TotalAccesses { get; set; }
    public int DataModifications { get; set; }
    public int DataExports { get; set; }
    
    // Atividade recente
    public List<AuditLogDto> RecentActivity { get; set; }
    
    // Por categoria
    public Dictionary<DataCategory, int> AccessByCategory { get; set; }
    
    // Por finalidade
    public Dictionary<LgpdPurpose, int> AccessByPurpose { get; set; }
}
```

**Casos de Uso:**
- Resposta a solicitação de titular (Art. 18 LGPD)
- Auditoria interna
- Demonstração para ANPD

---

## 🎛️ Filtros e Buscas

### Filtros Disponíveis

```csharp
public class AuditFilter
{
    // Período
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Quem
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    
    // O quê
    public List<AuditAction>? Actions { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    
    // Resultado
    public OperationResult? Result { get; set; }
    public AuditSeverity? Severity { get; set; }
    
    // Onde
    public string? IpAddress { get; set; }
    
    // LGPD
    public DataCategory? DataCategory { get; set; }
    public LgpdPurpose? Purpose { get; set; }
    
    // Paginação
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    
    // Ordenação
    public string? SortBy { get; set; } = "Timestamp";
    public bool Descending { get; set; } = true;
}
```

### Exemplo de Consulta Complexa

```csharp
var filter = new AuditFilter
{
    StartDate = DateTime.UtcNow.AddMonths(-1),
    EndDate = DateTime.UtcNow,
    Actions = new List<AuditAction> 
    { 
        AuditAction.UPDATE, 
        AuditAction.DELETE 
    },
    EntityType = "Patient",
    DataCategory = DataCategory.SENSITIVE,
    Severity = AuditSeverity.CRITICAL,
    Page = 1,
    PageSize = 100
};

var (logs, totalCount) = await _auditService.QueryAsync(filter);

Console.WriteLine($"Encontrados {totalCount} registros");
Console.WriteLine($"Página {filter.Page} de {Math.Ceiling((double)totalCount / filter.PageSize)}");
```

**Traduzindo para SQL:**
```sql
SELECT *
FROM AuditLogs
WHERE Timestamp BETWEEN '2025-12-15' AND '2026-01-15'
  AND Action IN ('UPDATE', 'DELETE')
  AND EntityType = 'Patient'
  AND DataCategory = 'SENSITIVE'
  AND Severity = 'CRITICAL'
  AND TenantId = 'tenant-abc'
ORDER BY Timestamp DESC
LIMIT 100 OFFSET 0;
```

---

## 📈 Relatórios

### 1. Relatório de Acessos a Dados Sensíveis

**Objetivo:** Demonstrar controle sobre dados de saúde (LGPD)

```csharp
var sensitiveDataAccess = await _auditService.QueryAsync(new AuditFilter
{
    DataCategory = DataCategory.SENSITIVE,
    StartDate = DateTime.UtcNow.AddMonths(-1),
    EndDate = DateTime.UtcNow
});

var report = new
{
    Period = "Últimos 30 dias",
    TotalAccesses = sensitiveDataAccess.TotalCount,
    
    ByUser = sensitiveDataAccess.Logs
        .GroupBy(l => l.UserName)
        .Select(g => new {
            User = g.Key,
            Accesses = g.Count(),
            LastAccess = g.Max(l => l.Timestamp)
        })
        .OrderByDescending(x => x.Accesses),
    
    ByEntityType = sensitiveDataAccess.Logs
        .GroupBy(l => l.EntityType)
        .Select(g => new {
            Type = g.Key,
            Count = g.Count()
        }),
    
    OutsideBusinessHours = sensitiveDataAccess.Logs
        .Where(l => l.Timestamp.Hour < 7 || l.Timestamp.Hour > 19)
        .Count()
};
```

**Visualização:**
```
╔═══════════════════════════════════════════════╗
║  Acessos a Dados Sensíveis - Jan/2026        ║
╠═══════════════════════════════════════════════╣
║  Total: 1,245 acessos                         ║
║                                               ║
║  Top Usuários:                                ║
║  1. Dr. João Silva         342 acessos        ║
║  2. Dra. Maria Santos      298 acessos        ║
║  3. Enf. Ana Costa         187 acessos        ║
║                                               ║
║  Por Tipo:                                    ║
║  • Prontuários: 890 (71.5%)                   ║
║  • Exames: 245 (19.7%)                        ║
║  • Prescrições: 110 (8.8%)                    ║
║                                               ║
║  Fora do horário: 23 (1.8%)                   ║
╚═══════════════════════════════════════════════╝
```

---

### 2. Relatório de Compliance LGPD

```csharp
var lgpdMetrics = new
{
    // Art. 18 - Direitos dos Titulares
    AccessRequests = await CountLogs(AuditAction.DATA_ACCESS_REQUEST),
    DeletionRequests = await CountLogs(AuditAction.DATA_DELETION_REQUEST),
    CorrectionRequests = await CountLogs(AuditAction.DATA_CORRECTION_REQUEST),
    PortabilityRequests = await CountLogs(AuditAction.DATA_PORTABILITY_REQUEST),
    
    // Respostas (< 15 dias)
    AverageResponseTime = await CalculateAvgResponseTime(),
    CompliantResponses = await CountCompletions(withinDeadline: true),
    
    // Segurança
    FailedLoginAttempts = await CountLogs(AuditAction.LOGIN_FAILED),
    AccessDenied = await CountLogs(AuditAction.ACCESS_DENIED),
    CriticalEvents = await CountBySeverity(AuditSeverity.CRITICAL),
    
    // Anonymization
    AnonymizedRecords = await CountLogs(AuditAction.DATA_ANONYMIZED),
    
    // Data Exports
    TotalExports = await CountLogs(AuditAction.EXPORT)
};
```

---

### 3. Relatório de Atividade Mensal

```typescript
// Frontend - Dashboard
interface MonthlyActivity {
  month: string;
  totalActions: number;
  byAction: {
    create: number;
    read: number;
    update: number;
    delete: number;
  };
  topUsers: Array<{
    name: string;
    actions: number;
  }>;
  securityEvents: number;
}
```

---

## 🔐 Análise de Segurança

### Detecção de Anomalias

#### 1. Acessos Massivos

```csharp
// Detectar usuário que acessou muitos registros em curto período
var massAccess = logs
    .Where(l => l.Action == AuditAction.READ)
    .GroupBy(l => new { l.UserId, Hour = l.Timestamp.Hour })
    .Where(g => g.Count() > 100) // 100+ acessos em 1 hora
    .Select(g => new {
        UserId = g.Key.UserId,
        Hour = g.Key.Hour,
        Count = g.Count(),
        EntityTypes = g.Select(l => l.EntityType).Distinct()
    });

// Alerta: Possível exfiltração de dados
```

#### 2. Múltiplas Falhas de Login

```csharp
var bruteForceAttempts = logs
    .Where(l => l.Action == AuditAction.LOGIN_FAILED)
    .Where(l => l.Timestamp > DateTime.UtcNow.AddHours(-1))
    .GroupBy(l => new { l.UserId, l.IpAddress })
    .Where(g => g.Count() >= 5)
    .Select(g => new {
        UserId = g.Key.UserId,
        IpAddress = g.Key.IpAddress,
        Attempts = g.Count(),
        FirstAttempt = g.Min(l => l.Timestamp),
        LastAttempt = g.Max(l => l.Timestamp)
    });

// Alerta: Possível ataque de força bruta
// Ação: Bloquear IP temporariamente
```

#### 3. Acesso de Localização Incomum

```csharp
var unusualLocation = logs
    .Where(l => l.Action == AuditAction.LOGIN)
    .Where(l => !IsUsualCountry(l.UserId, GetCountry(l.IpAddress)))
    .Select(l => new {
        l.UserId,
        l.UserName,
        Country = GetCountry(l.IpAddress),
        l.IpAddress,
        l.Timestamp
    });

// Alerta: Login de país incomum
// Ação: Exigir MFA adicional
```

#### 4. Escalação de Privilégios

```csharp
var privilegeEscalation = logs
    .Where(l => l.Action == AuditAction.ACCESS_DENIED)
    .GroupBy(l => l.UserId)
    .Where(g => g.Count() > 10) // 10+ acessos negados
    .Select(g => new {
        UserId = g.Key,
        DeniedAttempts = g.Count(),
        TargetedResources = g.Select(l => l.EntityType).Distinct(),
        Timeline = g.OrderBy(l => l.Timestamp).ToList()
    });

// Alerta: Possível tentativa de escalação
// Ação: Investigar e notificar administrador
```

---

## 🏥 LGPD e Compliance

### Demonstrar Conformidade (Art. 6, X)

**Requisito:** Demonstrar que adota medidas eficazes e capazes de comprovar a conformidade

**Como o Audit Log Ajuda:**

1. **Transparência**
   - Todos os acessos a dados pessoais registrados
   - Quem, quando e por que acessou

2. **Accountability**
   - Rastreabilidade completa
   - Impossível negar ações (não-repúdio)

3. **Controle de Acesso**
   - Evidência de permissões respeitadas
   - Acessos negados registrados

4. **Resposta a Incidentes**
   - Histórico completo para investigação
   - Timeline precisa de eventos

### Relatório para ANPD

```csharp
public async Task<ANPDReport> GenerateANPDReportAsync(
    DateTime startDate, 
    DateTime endDate)
{
    return new ANPDReport
    {
        Period = new { startDate, endDate },
        
        // Tratamento de dados
        DataProcessing = new {
            TotalOperations = await CountAllOperations(),
            ByLegalBasis = await GroupByLegalBasis(),
            DataCategories = await GroupByDataCategory()
        },
        
        // Direitos dos titulares
        TitularRights = new {
            AccessRequests = await CountRequests("access"),
            DeletionRequests = await CountRequests("deletion"),
            CorrectionRequests = await CountRequests("correction"),
            AverageResponseTime = await CalculateResponseTime(),
            CompliantResponses = await CountCompliantResponses()
        },
        
        // Segurança
        Security = new {
            FailedLogins = await CountFailedLogins(),
            UnauthorizedAccess = await CountAccessDenied(),
            SecurityIncidents = await CountIncidents(),
            IncidentsResolved = await CountResolvedIncidents()
        },
        
        // Compartilhamento
        DataSharing = new {
            ThirdPartyAccesses = await CountThirdPartyAccess(),
            DataExports = await CountExports(),
            SharedWith = await ListThirdParties()
        }
    };
}
```

---

## 💡 Exemplos Práticos

### Exemplo 1: Investigar Alteração Suspeita

**Cenário:** Um paciente reportou que seus dados foram alterados sem consentimento.

```csharp
// 1. Buscar todas as modificações no paciente
var patientHistory = await _auditService.GetEntityHistoryAsync(
    "Patient",
    patientId,
    tenantId
);

// 2. Filtrar apenas updates
var updates = patientHistory
    .Where(l => l.Action == "UPDATE")
    .OrderByDescending(l => l.Timestamp);

// 3. Analisar cada mudança
foreach (var update in updates)
{
    Console.WriteLine($"Data: {update.Timestamp}");
    Console.WriteLine($"Usuário: {update.UserName}");
    Console.WriteLine($"IP: {update.IpAddress}");
    Console.WriteLine($"Campos alterados: {string.Join(", ", update.ChangedFields)}");
    Console.WriteLine($"Valores antigos: {update.OldValues}");
    Console.WriteLine($"Valores novos: {update.NewValues}");
    Console.WriteLine("---");
}
```

**Resultado:**
```
Data: 2026-01-15 14:32:18
Usuário: recepcao@clinica.com
IP: 192.168.1.50
Campos alterados: phone, email
Valores antigos: {"phone":"1111-1111","email":"antigo@email.com"}
Valores novos: {"phone":"2222-2222","email":"novo@email.com"}
---
```

**Conclusão:** Alteração legítima feita pela recepção.

---

### Exemplo 2: Auditoria de Prontuários

**Cenário:** Auditoria interna requer lista de quem acessou prontuários no último mês.

```csharp
var medicalRecordAccess = await _auditService.QueryAsync(new AuditFilter
{
    EntityType = "MedicalRecord",
    StartDate = DateTime.UtcNow.AddMonths(-1),
    EndDate = DateTime.UtcNow,
    Actions = new List<AuditAction> 
    { 
        AuditAction.READ, 
        AuditAction.UPDATE 
    }
});

// Agrupar por usuário
var accessByUser = medicalRecordAccess.Logs
    .GroupBy(l => l.UserName)
    .Select(g => new {
        UserName = g.Key,
        TotalAccesses = g.Count(),
        UniquePatients = g.Select(l => l.EntityId).Distinct().Count(),
        LastAccess = g.Max(l => l.Timestamp)
    })
    .OrderByDescending(x => x.TotalAccesses);

// Exportar para Excel
await ExportToExcel(accessByUser, "audit_medical_records.xlsx");
```

---

### Exemplo 3: Compliance Report para Cliente

**Cenário:** Cliente (clínica) solicitou relatório de compliance.

```csharp
var report = await _auditService.GenerateLgpdReportAsync(clinicId, tenantId);

var pdf = GeneratePDF(new 
{
    Title = "Relatório de Compliance LGPD",
    Clinic = clinicName,
    Period = $"{startDate:dd/MM/yyyy} a {endDate:dd/MM/yyyy}",
    
    Metrics = new {
        TotalOperations = report.TotalAccesses + report.DataModifications,
        DataAccess = report.TotalAccesses,
        DataModifications = report.DataModifications,
        DataExports = report.DataExports
    },
    
    RecentActivity = report.RecentActivity.Take(20),
    
    Compliance = new {
        AuditLogCoverage = "100%",
        RetentionPolicy = "2 anos",
        EncryptionAtRest = "✅ Ativo",
        EncryptionInTransit = "✅ HTTPS/TLS 1.3",
        MFAAvailable = "✅ TOTP e SMS",
        BackupFrequency = "Diário"
    }
});

return File(pdf, "application/pdf", $"compliance_report_{clinicId}.pdf");
```

---

## 🛠️ Ferramentas e APIs

### REST API Endpoints

```
GET    /api/audit/users/{id}/activity
GET    /api/audit/{entityType}/{id}/history
GET    /api/audit/security-events
GET    /api/audit/query
GET    /api/audit/reports/lgpd
POST   /api/audit/search
```

### Frontend Components

```typescript
// Componente de visualização de audit log
<audit-log-viewer
  [entityType]="'Patient'"
  [entityId]="patientId"
  [showFilters]="true"
  [pageSize]="50"
/>

// Timeline de mudanças
<audit-timeline
  [logs]="auditLogs"
  [groupByDate]="true"
/>

// Gráfico de atividade
<audit-activity-chart
  [period]="'last30days'"
  [chartType]="'line'"
/>
```

---

## 📚 Referências

- [LGPD Compliance Guide](./LGPD_COMPLIANCE_GUIDE.md)
- [Security Best Practices](./SECURITY_BEST_PRACTICES_GUIDE.md)
- [Permissions Reference](./PERMISSIONS_REFERENCE.md)

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Próxima Revisão:** Julho 2026
