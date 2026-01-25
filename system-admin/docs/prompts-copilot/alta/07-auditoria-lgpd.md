# 🔐 Prompt: Auditoria Completa (LGPD)

## 📊 Status
- **Prioridade**: 🔥🔥 ALTA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 2 meses | 1 dev
- **Prazo**: Q1/2025

## 🎯 Contexto

Implementar sistema completo de auditoria para rastreabilidade de todas as ações no sistema, garantindo compliance total com a LGPD (Lei Geral de Proteção de Dados) - Lei 13.709/2018. Este é um requisito **OBRIGATÓRIO POR LEI**.

## ⚖️ Legislação Aplicável

- **LGPD** (Lei 13.709/2018): Artigo 37 - Registro das operações
- **ANPD**: Resoluções sobre tratamento de dados
- **CFM**: Código de Ética Médica - Sigilo profissional
- **ISO 27001**: Padrões de segurança da informação

## 📋 Eventos a Auditar

### Autenticação
- Login bem-sucedido
- Tentativa de login falhada
- Logout
- Expiração de sessão
- Token renovado
- Token invalidado
- MFA habilitado/desabilitado
- Senha alterada
- Recuperação de senha

### Autorização
- Acesso negado (403)
- Tentativa de acesso a recurso de outro tenant
- Escalação de privilégios tentada
- Alteração de permissões

### Dados Sensíveis (LGPD)
- Acesso a prontuário médico
- Modificação de dados de paciente
- Download de relatórios
- Exportação de dados
- Exclusão de registros (soft delete)
- Visualização de dados sensíveis
- Impressão de documentos

### Configurações
- Mudança de configuração do sistema
- Criação/alteração de usuário
- Mudança de permissões
- Alteração de planos
- Mudança de tenant

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// Entidade de Auditoria
public class AuditLog : Entity
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Usuário que executou a ação
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    
    // Tenant (multi-tenancy)
    public string TenantId { get; set; }
    
    // Ação executada
    public AuditAction Action { get; set; }  // CREATE, READ, UPDATE, DELETE, LOGIN, LOGOUT
    public string ActionDescription { get; set; }
    
    // Entidade afetada
    public string EntityType { get; set; }  // Patient, MedicalRecord, User, etc
    public string EntityId { get; set; }
    public string EntityDisplayName { get; set; }
    
    // Dados da requisição
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string RequestPath { get; set; }
    public string HttpMethod { get; set; }
    
    // Dados antes e depois (para UPDATE)
    public string OldValues { get; set; }  // JSON
    public string NewValues { get; set; }  // JSON
    public List<string> ChangedFields { get; set; }  // Lista de campos alterados
    
    // Resultado da operação
    public OperationResult Result { get; set; }  // SUCCESS, FAILED, UNAUTHORIZED
    public string FailureReason { get; set; }
    public int? StatusCode { get; set; }
    
    // Categoria LGPD
    public DataCategory DataCategory { get; set; }  // SENSITIVE, PERSONAL, PUBLIC
    public LgpdPurpose Purpose { get; set; }  // Finalidade do tratamento
    
    // Severidade
    public AuditSeverity Severity { get; set; }  // INFO, WARNING, ERROR, CRITICAL
}

// Enums
public enum AuditAction
{
    // CRUD
    CREATE,
    READ,
    UPDATE,
    DELETE,
    
    // Auth
    LOGIN,
    LOGOUT,
    LOGIN_FAILED,
    PASSWORD_CHANGED,
    PASSWORD_RESET_REQUESTED,
    MFA_ENABLED,
    MFA_DISABLED,
    
    // Authorization
    ACCESS_DENIED,
    PERMISSION_CHANGED,
    ROLE_CHANGED,
    
    // Data Export
    EXPORT,
    DOWNLOAD,
    PRINT,
    
    // LGPD Rights
    DATA_ACCESS_REQUEST,
    DATA_DELETION_REQUEST,
    DATA_PORTABILITY_REQUEST,
    DATA_CORRECTION_REQUEST
}

public enum OperationResult
{
    SUCCESS,
    FAILED,
    UNAUTHORIZED,
    PARTIAL_SUCCESS
}

public enum DataCategory
{
    PUBLIC,          // Dados públicos
    PERSONAL,        // Dados pessoais (LGPD)
    SENSITIVE,       // Dados sensíveis (saúde, biométricos)
    CONFIDENTIAL     // Dados confidenciais (segredos comerciais)
}

public enum LgpdPurpose
{
    HEALTHCARE,             // Prestação de serviços de saúde
    BILLING,                // Faturamento
    LEGAL_OBLIGATION,       // Obrigação legal
    LEGITIMATE_INTEREST,    // Interesse legítimo
    CONSENT                 // Consentimento
}

public enum AuditSeverity
{
    INFO,       // Informativo
    WARNING,    // Aviso
    ERROR,      // Erro
    CRITICAL    // Crítico (violação de segurança, tentativa de invasão)
}

// Consentimento LGPD
public class DataProcessingConsent : Entity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }  // Titular dos dados
    public DateTime ConsentDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public bool IsRevoked { get; set; }
    
    public LgpdPurpose Purpose { get; set; }
    public string PurposeDescription { get; set; }
    
    public List<DataCategory> DataCategories { get; set; }
    public string ConsentText { get; set; }
    
    // Evidência
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string ConsentMethod { get; set; }  // WEB, MOBILE, PAPER
}
```

### Camada de Aplicação (Application Layer)

```csharp
// Service Interface
public interface IAuditService
{
    Task LogAsync(AuditLog auditLog);
    Task LogAuthenticationAsync(string userId, string action, bool success, string reason = null);
    Task LogDataAccessAsync(string userId, string entityType, string entityId, string action);
    Task LogDataModificationAsync(string userId, string entityType, string entityId, 
        object oldValues, object newValues);
    Task<List<AuditLog>> GetUserActivityAsync(string userId, DateTime? startDate, DateTime? endDate);
    Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, string entityId);
    Task<List<AuditLog>> GetSecurityEventsAsync(DateTime? startDate, DateTime? endDate);
    Task<AuditReport> GenerateLgpdReportAsync(string userId);
}

// DTOs
public record AuditLogDto(
    Guid Id,
    DateTime Timestamp,
    string UserName,
    string Action,
    string EntityType,
    string EntityId,
    string Result,
    string IpAddress
);

public record AuditReport(
    string UserId,
    string UserName,
    DateTime GeneratedAt,
    int TotalAccesses,
    int DataModifications,
    int DataExports,
    List<AuditLogDto> RecentActivity
);

// Audit Filter
public class AuditFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public AuditAction? Action { get; set; }
    public OperationResult? Result { get; set; }
    public AuditSeverity? Severity { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
```

### Camada de Infraestrutura (Infrastructure Layer)

```csharp
// Audit Repository
public class AuditRepository : IAuditRepository
{
    private readonly AuditDbContext _context;  // Banco separado para auditoria
    
    public async Task AddAsync(AuditLog auditLog)
    {
        // Write-only operation, nunca deletar
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<AuditLog>> QueryAsync(AuditFilter filter)
    {
        var query = _context.AuditLogs.AsQueryable();
        
        if (filter.StartDate.HasValue)
            query = query.Where(a => a.Timestamp >= filter.StartDate.Value);
        
        if (filter.EndDate.HasValue)
            query = query.Where(a => a.Timestamp <= filter.EndDate.Value);
        
        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(a => a.UserId == filter.UserId);
        
        if (!string.IsNullOrEmpty(filter.EntityType))
            query = query.Where(a => a.EntityType == filter.EntityType);
        
        if (filter.Action.HasValue)
            query = query.Where(a => a.Action == filter.Action.Value);
        
        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }
}

// Audit Middleware (ASP.NET Core)
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuditService _auditService;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Capturar request
        var requestBody = await ReadRequestBody(context.Request);
        var startTime = DateTime.UtcNow;
        
        // Executar request
        await _next(context);
        
        // Log audit
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = context.User?.FindFirst(ClaimTypes.Name)?.Value;
        
        var auditLog = new AuditLog
        {
            Timestamp = startTime,
            UserId = userId,
            UserName = userName,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RequestPath = context.Request.Path,
            HttpMethod = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            Result = context.Response.StatusCode < 400 
                ? OperationResult.SUCCESS 
                : OperationResult.FAILED,
            Severity = DetermineSeverity(context)
        };
        
        await _auditService.LogAsync(auditLog);
    }
    
    private AuditSeverity DetermineSeverity(HttpContext context)
    {
        if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            return AuditSeverity.WARNING;
        
        if (context.Response.StatusCode >= 500)
            return AuditSeverity.ERROR;
        
        return AuditSeverity.INFO;
    }
}

// Audit Interceptor (Entity Framework)
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                await _auditService.LogAsync(new AuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    Action = AuditAction.CREATE,
                    EntityType = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry.Entity),
                    NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                    Result = OperationResult.SUCCESS
                });
            }
            else if (entry.State == EntityState.Modified)
            {
                var oldValues = entry.OriginalValues.ToObject();
                var newValues = entry.CurrentValues.ToObject();
                
                await _auditService.LogDataModificationAsync(
                    userId,
                    entry.Entity.GetType().Name,
                    GetEntityId(entry.Entity),
                    oldValues,
                    newValues
                );
            }
            else if (entry.State == EntityState.Deleted)
            {
                await _auditService.LogAsync(new AuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    Action = AuditAction.DELETE,
                    EntityType = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry.Entity),
                    OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
                    Result = OperationResult.SUCCESS
                });
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

### Camada de API (API Layer)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> GetUserActivity(
        string userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var logs = await _auditService.GetUserActivityAsync(userId, startDate, endDate);
        return Ok(logs);
    }
    
    [HttpGet("entity/{entityType}/{entityId}")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> GetEntityHistory(
        string entityType,
        string entityId)
    {
        var logs = await _auditService.GetEntityHistoryAsync(entityType, entityId);
        return Ok(logs);
    }
    
    [HttpGet("security-events")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> GetSecurityEvents(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var logs = await _auditService.GetSecurityEventsAsync(startDate, endDate);
        return Ok(logs);
    }
    
    [HttpGet("lgpd-report/{userId}")]
    public async Task<IActionResult> GetLgpdReport(string userId)
    {
        // Usuário pode ver seu próprio relatório
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (currentUserId != userId && !User.IsInRole("Admin"))
            return Forbid();
        
        var report = await _auditService.GenerateLgpdReportAsync(userId);
        return Ok(report);
    }
    
    [HttpPost("query")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> QueryAuditLogs([FromBody] AuditFilter filter)
    {
        var logs = await _auditRepository.QueryAsync(filter);
        return Ok(logs);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes Necessários

```typescript
// Audit Log List Component
@Component({
  selector: 'app-audit-log-list',
  template: `
    <h2>Logs de Auditoria</h2>
    
    <mat-card class="filter-card">
      <mat-card-content>
        <form [formGroup]="filterForm">
          <mat-form-field>
            <mat-date-range-input [rangePicker]="picker">
              <input matStartDate placeholder="Data Inicial" formControlName="startDate">
              <input matEndDate placeholder="Data Final" formControlName="endDate">
            </mat-date-range-input>
            <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
            <mat-date-range-picker #picker></mat-date-range-picker>
          </mat-form-field>
          
          <mat-form-field>
            <mat-select placeholder="Ação" formControlName="action">
              <mat-option value="">Todas</mat-option>
              <mat-option value="CREATE">Criação</mat-option>
              <mat-option value="READ">Leitura</mat-option>
              <mat-option value="UPDATE">Atualização</mat-option>
              <mat-option value="DELETE">Exclusão</mat-option>
              <mat-option value="LOGIN">Login</mat-option>
              <mat-option value="EXPORT">Exportação</mat-option>
            </mat-select>
          </mat-form-field>
          
          <mat-form-field>
            <mat-select placeholder="Resultado" formControlName="result">
              <mat-option value="">Todos</mat-option>
              <mat-option value="SUCCESS">Sucesso</mat-option>
              <mat-option value="FAILED">Falha</mat-option>
              <mat-option value="UNAUTHORIZED">Não Autorizado</mat-option>
            </mat-select>
          </mat-form-field>
          
          <button mat-raised-button color="primary" (click)="filter()">Filtrar</button>
          <button mat-button (click)="clearFilters()">Limpar</button>
        </form>
      </mat-card-content>
    </mat-card>
    
    <table mat-table [dataSource]="auditLogs" class="audit-table">
      <ng-container matColumnDef="timestamp">
        <th mat-header-cell *matHeaderCellDef>Data/Hora</th>
        <td mat-cell *matCellDef="let log">
          {{ log.timestamp | date:'dd/MM/yyyy HH:mm:ss' }}
        </td>
      </ng-container>
      
      <ng-container matColumnDef="user">
        <th mat-header-cell *matHeaderCellDef>Usuário</th>
        <td mat-cell *matCellDef="let log">{{ log.userName }}</td>
      </ng-container>
      
      <ng-container matColumnDef="action">
        <th mat-header-cell *matHeaderCellDef>Ação</th>
        <td mat-cell *matCellDef="let log">
          <mat-chip [color]="getActionColor(log.action)">
            {{ getActionText(log.action) }}
          </mat-chip>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="entity">
        <th mat-header-cell *matHeaderCellDef>Entidade</th>
        <td mat-cell *matCellDef="let log">
          {{ log.entityType }}
          <span class="entity-id">{{ log.entityDisplayName }}</span>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="result">
        <th mat-header-cell *matHeaderCellDef>Resultado</th>
        <td mat-cell *matCellDef="let log">
          <mat-icon [color]="getResultColor(log.result)">
            {{ getResultIcon(log.result) }}
          </mat-icon>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="ipAddress">
        <th mat-header-cell *matHeaderCellDef>IP</th>
        <td mat-cell *matCellDef="let log">{{ log.ipAddress }}</td>
      </ng-container>
      
      <ng-container matColumnDef="details">
        <th mat-header-cell *matHeaderCellDef>Detalhes</th>
        <td mat-cell *matCellDef="let log">
          <button mat-icon-button (click)="viewDetails(log)">
            <mat-icon>info</mat-icon>
          </button>
        </td>
      </ng-container>
      
      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
    
    <mat-paginator [length]="totalLogs" [pageSize]="pageSize"></mat-paginator>
  `
})
export class AuditLogListComponent { }

// Audit Log Details Dialog
@Component({
  selector: 'app-audit-log-details',
  template: `
    <h2 mat-dialog-title>Detalhes da Auditoria</h2>
    <mat-dialog-content>
      <div class="detail-section">
        <h3>Informações Gerais</h3>
        <dl>
          <dt>Data/Hora:</dt>
          <dd>{{ log.timestamp | date:'dd/MM/yyyy HH:mm:ss' }}</dd>
          
          <dt>Usuário:</dt>
          <dd>{{ log.userName }} ({{ log.userEmail }})</dd>
          
          <dt>Ação:</dt>
          <dd>{{ log.actionDescription }}</dd>
          
          <dt>IP Address:</dt>
          <dd>{{ log.ipAddress }}</dd>
          
          <dt>User Agent:</dt>
          <dd class="small-text">{{ log.userAgent }}</dd>
        </dl>
      </div>
      
      <div class="detail-section" *ngIf="log.oldValues || log.newValues">
        <h3>Alterações</h3>
        <div class="changes-container">
          <div class="old-values" *ngIf="log.oldValues">
            <h4>Valores Anteriores</h4>
            <pre>{{ log.oldValues | json }}</pre>
          </div>
          <div class="new-values" *ngIf="log.newValues">
            <h4>Valores Novos</h4>
            <pre>{{ log.newValues | json }}</pre>
          </div>
        </div>
      </div>
      
      <div class="detail-section" *ngIf="log.failureReason">
        <h3>Razão da Falha</h3>
        <p class="error-message">{{ log.failureReason }}</p>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions>
      <button mat-button mat-dialog-close>Fechar</button>
    </mat-dialog-actions>
  `
})
export class AuditLogDetailsDialogComponent { }
```

## 📋 Checklist de Implementação

### Backend

- [ ] Criar entidade AuditLog
- [ ] Criar banco de dados separado para auditoria
- [ ] Implementar repositório de auditoria
- [ ] Criar middleware de auditoria
- [ ] Implementar interceptor EF Core
- [ ] Criar serviço de auditoria
- [ ] Implementar log de autenticação
- [ ] Implementar log de acesso a dados sensíveis
- [ ] Implementar log de modificações
- [ ] Criar controller de auditoria
- [ ] Implementar filtros e queries
- [ ] Implementar relatório LGPD
- [ ] Adicionar migrations
- [ ] Implementar retenção de logs (7-10 anos)
- [ ] Implementar exportação de logs

### Frontend

- [ ] Criar componente de listagem de logs
- [ ] Implementar filtros de busca
- [ ] Criar dialog de detalhes
- [ ] Implementar visualizador de diff
- [ ] Criar dashboard de auditoria
- [ ] Implementar gráficos de atividade
- [ ] Criar relatório LGPD para usuário
- [ ] Implementar exportação de dados (CSV, PDF)

### LGPD Compliance

- [ ] Sistema de consentimento
- [ ] Direito ao esquecimento
- [ ] Portabilidade de dados
- [ ] Relatório de atividades
- [ ] Política de retenção (7-10 anos)
- [ ] Auditoria de acessos
- [ ] Alertas de segurança

## 🧪 Testes

### Testes Unitários
```csharp
public class AuditServiceTests
{
    [Fact]
    public async Task ShouldLogAuthentication()
    {
        // Test auth logging
    }
    
    [Fact]
    public async Task ShouldLogDataModification()
    {
        // Test data modification logging
    }
    
    [Fact]
    public async Task ShouldGenerateLgpdReport()
    {
        // Test LGPD report generation
    }
}
```

## 📚 Referências

- [PENDING_TASKS.md - Seção Auditoria LGPD](../../PENDING_TASKS.md#5-auditoria-completa-lgpd)
- [LGPD - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/L13709.htm)
- [LGPD_COMPLIANCE_DOCUMENTATION.md](../../LGPD_COMPLIANCE_DOCUMENTATION.md)
- [SUGESTOES_MELHORIAS_SEGURANCA.md](../../SUGESTOES_MELHORIAS_SEGURANCA.md)

## 💰 Investimento

- **Desenvolvimento**: 2 meses, 1 dev
- **Custo**: R$ 45k
- **ROI Esperado**: Compliance legal obrigatório, segurança

## ✅ Critérios de Aceitação

1. ✅ Todas as ações são auditadas automaticamente
2. ✅ Logs incluem usuário, timestamp, IP e ação
3. ✅ Modificações registram valores antes/depois
4. ✅ Banco de dados separado para auditoria
5. ✅ Logs nunca são deletados (write-only)
6. ✅ Retenção de 7-10 anos
7. ✅ Relatório LGPD disponível para usuários
8. ✅ Alertas de eventos de segurança críticos
9. ✅ Exportação de logs
10. ✅ Dashboard de auditoria para administradores

---

**Última Atualização**: Janeiro 2026
**Status**: Pronto para desenvolvimento
**Próximo Passo**: Criar banco de dados de auditoria e implementar middleware
