# 🔍 Auditoria Completa e Compliance LGPD

**Prioridade:** 🔥🔥 P1 - ALTA  
**Obrigatoriedade:** Legal (LGPD - Lei 13.709/2018)  
**Status Atual:** 0% completo  
**Esforço:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000  
**Prazo:** Q1 2026 (Janeiro-Março)

## 📋 Contexto

A **Lei Geral de Proteção de Dados (LGPD)** está em vigor desde setembro de 2020, e empresas de saúde são **alvo prioritário** da ANPD (Autoridade Nacional de Proteção de Dados) devido à sensibilidade dos dados tratados.

### Por que é Prioridade Alta?

1. **Obrigatoriedade Legal:** LGPD é lei federal com multas de até R$ 50 milhões ou 2% do faturamento
2. **Dados Sensíveis:** Dados de saúde são categoria especial (Art. 11)
3. **Rastreabilidade Obrigatória:** Necessário comprovar compliance
4. **Risco de Multas:** Empresas de saúde são fiscalizadas primeiro
5. **Confiança do Cliente:** Sistema sem auditoria não inspira confiança

### Situação Atual

- ❌ **Sem sistema de auditoria centralizado**
- ❌ Sem registro de acessos a dados sensíveis
- ❌ Sem logs de consentimento
- ❌ Sem mecanismo de portabilidade de dados
- ❌ Sem processo de direito ao esquecimento
- ❌ Sem relatórios LGPD para ANPD

### Riscos de Não Implementar

- Multas de até **R$ 50 milhões** (Art. 52)
- Impossibilidade de comprovar compliance em fiscalização
- Perda de clientes por falta de transparência
- Danos reputacionais irreversíveis
- Processos judiciais de pacientes

## 🎯 Objetivos da Tarefa

Implementar sistema completo de auditoria que registre todas as operações sensíveis do sistema, com foco em compliance LGPD, permitindo rastreabilidade completa, gestão de consentimentos, e suporte aos direitos dos titulares de dados.

## 📝 Tarefas Detalhadas

### 1. Modelagem de Dados de Auditoria (1 semana)

#### 1.1 Entidade AuditLog
```csharp
// src/MedicSoft.Core/Entities/Audit/AuditLog.cs
namespace MedicSoft.Core.Entities.Audit
{
    public class AuditLog : BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Identificação
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        
        // Ação
        public AuditActionType Action { get; set; }  // CREATE, READ, UPDATE, DELETE, LOGIN, EXPORT
        public string EntityType { get; set; }  // Patient, MedicalRecord, Prescription, etc.
        public string EntityId { get; set; }
        public string EntityDescription { get; set; }
        
        // Contexto
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string RequestUrl { get; set; }
        public string HttpMethod { get; set; }
        
        // Dados
        public string OldValues { get; set; }  // JSON before (para UPDATE/DELETE)
        public string NewValues { get; set; }  // JSON after (para CREATE/UPDATE)
        
        // Resultado
        public AuditResultType Result { get; set; }  // SUCCESS, FAILED, UNAUTHORIZED
        public string FailureReason { get; set; }
        public string ExceptionMessage { get; set; }
        
        // LGPD
        public bool IsSensitiveData { get; set; }
        public string LegalBasis { get; set; }  // Consent, Legal Obligation, etc.
    }
    
    public enum AuditActionType
    {
        // Autenticação
        Login,
        Logout,
        LoginFailed,
        PasswordChanged,
        MfaEnabled,
        MfaDisabled,
        
        // CRUD
        Create,
        Read,
        Update,
        Delete,
        
        // Operações Especiais
        Export,
        Print,
        Share,
        
        // LGPD
        ConsentGiven,
        ConsentRevoked,
        DataPortabilityRequest,
        DataDeletionRequest
    }
    
    public enum AuditResultType
    {
        Success,
        Failed,
        Unauthorized,
        PartialSuccess
    }
}
```

#### 1.2 Entidade DataConsentLog
```csharp
// src/MedicSoft.Core/Entities/Audit/DataConsentLog.cs
namespace MedicSoft.Core.Entities.Audit
{
    public class DataConsentLog : BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Titular
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        
        // Consentimento
        public ConsentType Type { get; set; }
        public ConsentPurpose Purpose { get; set; }
        public string Description { get; set; }
        
        // Status
        public ConsentStatus Status { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string RevocationReason { get; set; }
        
        // Contexto
        public string IpAddress { get; set; }
        public string ConsentText { get; set; }  // Texto exato apresentado
        public string ConsentVersion { get; set; }
    }
    
    public enum ConsentType
    {
        MedicalTreatment,
        DataSharing,
        Marketing,
        Research,
        Telemedicine
    }
    
    public enum ConsentPurpose
    {
        Treatment,
        DiagnosticProcedures,
        DataSharing,
        MarketingCommunication,
        ClinicalResearch,
        QualityImprovement
    }
    
    public enum ConsentStatus
    {
        Active,
        Revoked,
        Expired
    }
}
```

#### 1.3 Entidade DataAccessLog
```csharp
// src/MedicSoft.Core/Entities/Audit/DataAccessLog.cs
namespace MedicSoft.Core.Entities.Audit
{
    /// <summary>
    /// Log específico para acesso a dados sensíveis (LGPD Art. 37)
    /// </summary>
    public class DataAccessLog : BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Quem acessou
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        
        // O que foi acessado
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public List<string> FieldsAccessed { get; set; }
        
        // Paciente titular dos dados
        public Guid? PatientId { get; set; }
        
        // Contexto
        public string AccessReason { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        
        // Resultado
        public bool WasAuthorized { get; set; }
        public string DenialReason { get; set; }
    }
}
```

### 2. Implementação Backend (3 semanas)

#### 2.1 AuditService
```csharp
// src/MedicSoft.Core/Services/Audit/AuditService.cs
namespace MedicSoft.Core.Services.Audit
{
    public interface IAuditService
    {
        Task LogAsync(AuditLog auditLog);
        Task LogActionAsync(string userId, AuditActionType action, string entityType, 
            string entityId, object oldValues = null, object newValues = null);
        Task LogDataAccessAsync(string userId, string entityType, string entityId, 
            List<string> fieldsAccessed, string reason);
        Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogFilterDto filter);
        Task<List<AuditLog>> GetUserActivityAsync(string userId, DateTime? from = null, DateTime? to = null);
        Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, string entityId);
        Task<List<DataAccessLog>> GetPatientDataAccessHistoryAsync(Guid patientId);
        Task<AuditStatistics> GetStatisticsAsync(DateTime from, DateTime to);
    }
    
    public class AuditService : IAuditService
    {
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly IRepository<DataAccessLog> _dataAccessLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;
        
        public AuditService(
            IRepository<AuditLog> auditLogRepository,
            IRepository<DataAccessLog> dataAccessLogRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _dataAccessLogRepository = dataAccessLogRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        
        public async Task LogAsync(AuditLog auditLog)
        {
            try
            {
                // Enriquecer com contexto HTTP
                var context = _httpContextAccessor.HttpContext;
                if (context != null)
                {
                    auditLog.IpAddress ??= context.Connection.RemoteIpAddress?.ToString();
                    auditLog.UserAgent ??= context.Request.Headers["User-Agent"].ToString();
                    auditLog.RequestUrl ??= $"{context.Request.Path}{context.Request.QueryString}";
                    auditLog.HttpMethod ??= context.Request.Method;
                }
                
                auditLog.Timestamp = DateTime.UtcNow;
                
                await _auditLogRepository.AddAsync(auditLog);
                
                // Não fazer throw se falhar - auditoria não deve quebrar funcionalidade
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save audit log");
            }
        }
        
        public async Task LogActionAsync(
            string userId, 
            AuditActionType action, 
            string entityType, 
            string entityId,
            object oldValues = null, 
            object newValues = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                Result = AuditResultType.Success,
                IsSensitiveData = IsSensitiveEntityType(entityType)
            };
            
            await LogAsync(auditLog);
        }
        
        public async Task LogDataAccessAsync(
            string userId, 
            string entityType, 
            string entityId,
            List<string> fieldsAccessed, 
            string reason)
        {
            var log = new DataAccessLog
            {
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId,
                FieldsAccessed = fieldsAccessed,
                AccessReason = reason,
                Timestamp = DateTime.UtcNow,
                WasAuthorized = true
            };
            
            await _dataAccessLogRepository.AddAsync(log);
        }
        
        private bool IsSensitiveEntityType(string entityType)
        {
            var sensitiveTypes = new[] 
            { 
                "Patient", "MedicalRecord", "Prescription", 
                "LabResult", "DiagnosticHypothesis", "ClinicalExamination" 
            };
            
            return sensitiveTypes.Contains(entityType);
        }
        
        public async Task<List<AuditLog>> GetUserActivityAsync(
            string userId, 
            DateTime? from = null, 
            DateTime? to = null)
        {
            var query = _auditLogRepository.GetAll()
                .Where(a => a.UserId == userId);
            
            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);
                
            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);
            
            return await query
                .OrderByDescending(a => a.Timestamp)
                .Take(1000)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, string entityId)
        {
            return await _auditLogRepository.GetAll()
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<DataAccessLog>> GetPatientDataAccessHistoryAsync(Guid patientId)
        {
            return await _dataAccessLogRepository.GetAll()
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<AuditStatistics> GetStatisticsAsync(DateTime from, DateTime to)
        {
            var logs = await _auditLogRepository.GetAll()
                .Where(a => a.Timestamp >= from && a.Timestamp <= to)
                .ToListAsync();
            
            return new AuditStatistics
            {
                TotalEvents = logs.Count,
                SuccessfulEvents = logs.Count(l => l.Result == AuditResultType.Success),
                FailedEvents = logs.Count(l => l.Result == AuditResultType.Failed),
                UnauthorizedAttempts = logs.Count(l => l.Result == AuditResultType.Unauthorized),
                SensitiveDataAccesses = logs.Count(l => l.IsSensitiveData),
                UniqueUsers = logs.Select(l => l.UserId).Distinct().Count(),
                MostAccessedEntities = logs
                    .GroupBy(l => l.EntityType)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
        
        public async Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogFilterDto filter)
        {
            var query = _auditLogRepository.GetAll();
            
            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(a => a.UserId == filter.UserId);
            
            if (filter.Action.HasValue)
                query = query.Where(a => a.Action == filter.Action.Value);
            
            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(a => a.EntityType == filter.EntityType);
            
            if (filter.Result.HasValue)
                query = query.Where(a => a.Result == filter.Result.Value);
            
            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.Timestamp >= filter.DateFrom.Value);
            
            if (filter.DateTo.HasValue)
                query = query.Where(a => a.Timestamp <= filter.DateTo.Value);
            
            if (filter.IsSensitiveOnly)
                query = query.Where(a => a.IsSensitiveData);
            
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(a => 
                    a.UserName.Contains(filter.SearchTerm) ||
                    a.EntityType.Contains(filter.SearchTerm) ||
                    a.EntityDescription.Contains(filter.SearchTerm)
                );
            }
            
            // Contar total antes da paginação
            var totalCount = await query.CountAsync();
            
            // Ordenar e paginar
            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            
            return new PagedResult<AuditLog>
            {
                Items = logs,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }
    }
    
    // DTOs
    public class AuditLogFilterDto
    {
        public string UserId { get; set; }
        public AuditActionType? Action { get; set; }
        public string EntityType { get; set; }
        public AuditResultType? Result { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsSensitiveOnly { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
    }
    
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    
    public class AuditStatistics
    {
        public int TotalEvents { get; set; }
        public int SuccessfulEvents { get; set; }
        public int FailedEvents { get; set; }
        public int UnauthorizedAttempts { get; set; }
        public int SensitiveDataAccesses { get; set; }
        public int UniqueUsers { get; set; }
        public Dictionary<string, int> MostAccessedEntities { get; set; }
    }
}
```

#### 2.2 Interceptor Global (Middleware)
```csharp
// src/MedicSoft.Api/Middleware/AuditMiddleware.cs
namespace MedicSoft.Api.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;
        
        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            var startTime = DateTime.UtcNow;
            var originalBodyStream = context.Response.Body;
            
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            try
            {
                await _next(context);
                
                // Auditar apenas endpoints críticos
                if (ShouldAudit(context))
                {
                    await AuditRequestAsync(context, auditService, startTime, null);
                }
            }
            catch (Exception ex)
            {
                if (ShouldAudit(context))
                {
                    await AuditRequestAsync(context, auditService, startTime, ex);
                }
                throw;
            }
            finally
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        
        private bool ShouldAudit(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            
            // Auditar apenas rotas críticas
            var criticalPaths = new[]
            {
                "/api/patients",
                "/api/medicalrecords",
                "/api/prescriptions",
                "/api/auth",
                "/api/users"
            };
            
            return criticalPaths.Any(cp => path?.StartsWith(cp) == true);
        }
        
        private async Task AuditRequestAsync(
            HttpContext context, 
            IAuditService auditService,
            DateTime startTime,
            Exception exception)
        {
            var userId = context.User?.FindFirst("sub")?.Value 
                ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var action = MapHttpMethodToAction(context.Request.Method);
            
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                RequestUrl = $"{context.Request.Path}{context.Request.QueryString}",
                HttpMethod = context.Request.Method,
                Result = exception == null && context.Response.StatusCode < 400 
                    ? AuditResultType.Success 
                    : AuditResultType.Failed,
                ExceptionMessage = exception?.Message
            };
            
            await auditService.LogAsync(auditLog);
        }
        
        private AuditActionType MapHttpMethodToAction(string method)
        {
            return method.ToUpper() switch
            {
                "POST" => AuditActionType.Create,
                "GET" => AuditActionType.Read,
                "PUT" => AuditActionType.Update,
                "PATCH" => AuditActionType.Update,
                "DELETE" => AuditActionType.Delete,
                _ => AuditActionType.Read
            };
        }
    }
}
```

### 3. API Controllers (2 semanas)

#### 3.1 AuditLogsController
```csharp
// src/MedicSoft.Api/Controllers/AuditLogsController.cs
namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;
        
        public AuditLogsController(IAuditService auditService)
        {
            _auditService = auditService;
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedResult<AuditLog>>> GetAuditLogs(
            [FromQuery] AuditLogFilterDto filter)
        {
            var logs = await _auditService.GetAuditLogsAsync(filter);
            return Ok(logs);
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<AuditLog>>> GetUserActivity(
            string userId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var logs = await _auditService.GetUserActivityAsync(userId, from, to);
            return Ok(logs);
        }
        
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<List<AuditLog>>> GetEntityHistory(
            string entityType,
            string entityId)
        {
            var logs = await _auditService.GetEntityHistoryAsync(entityType, entityId);
            return Ok(logs);
        }
        
        [HttpGet("statistics")]
        public async Task<ActionResult<AuditStatistics>> GetStatistics(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var stats = await _auditService.GetStatisticsAsync(from, to);
            return Ok(stats);
        }
        
        [HttpGet("patient/{patientId}/access-history")]
        public async Task<ActionResult<List<DataAccessLog>>> GetPatientAccessHistory(Guid patientId)
        {
            var logs = await _auditService.GetPatientDataAccessHistoryAsync(patientId);
            return Ok(logs);
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportAuditLogs([FromBody] AuditLogFilterDto filter)
        {
            var result = await _auditService.GetAuditLogsAsync(filter);
            var csv = GenerateCsv(result.Items);
            
            return File(
                Encoding.UTF8.GetBytes(csv),
                "text/csv",
                $"audit-logs-{DateTime.UtcNow:yyyyMMdd}.csv"
            );
        }
        
        private string GenerateCsv(List<AuditLog> logs)
        {
            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine("Timestamp,User,Action,Entity Type,Entity ID,Result,IP Address,User Agent");
            
            // Rows
            foreach (var log in logs)
            {
                sb.AppendLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\"," +
                    $"\"{log.UserName}\"," +
                    $"\"{log.Action}\"," +
                    $"\"{log.EntityType}\"," +
                    $"\"{log.EntityId}\"," +
                    $"\"{log.Result}\"," +
                    $"\"{log.IpAddress}\"," +
                    $"\"{log.UserAgent}\"");
            }
            
            return sb.ToString();
        }
    }
}
```

### 4. Frontend - Visualização de Auditoria (2 semanas)

#### 4.1 Componente de Listagem
```typescript
// frontend/src/app/admin/audit-logs/audit-log-list.component.ts
import { Component, OnInit } from '@angular/core';
import { AuditService } from '../../services/audit.service';

@Component({
  selector: 'app-audit-log-list',
  templateUrl: './audit-log-list.component.html'
})
export class AuditLogListComponent implements OnInit {
  auditLogs: AuditLog[] = [];
  displayedColumns = ['timestamp', 'user', 'action', 'entity', 'result', 'ipAddress', 'actions'];
  
  // Filtros
  filters = {
    userId: '',
    action: '',
    entityType: '',
    result: '',
    dateFrom: null,
    dateTo: null,
    isSensitiveOnly: false
  };
  
  constructor(private auditService: AuditService) {}
  
  ngOnInit() {
    this.loadAuditLogs();
  }
  
  loadAuditLogs() {
    this.auditService.getAuditLogs(this.filters).subscribe(
      (response) => {
        this.auditLogs = response.items;
      }
    );
  }
  
  applyFilters() {
    this.loadAuditLogs();
  }
  
  clearFilters() {
    this.filters = {
      userId: '',
      action: '',
      entityType: '',
      result: '',
      dateFrom: null,
      dateTo: null,
      isSensitiveOnly: false
    };
    this.loadAuditLogs();
  }
  
  viewDetails(log: AuditLog) {
    // Abrir modal com detalhes completos
    const dialogRef = this.dialog.open(AuditLogDetailsDialog, {
      width: '800px',
      data: log
    });
  }
  
  exportToCSV() {
    this.auditService.exportAuditLogs(this.filters).subscribe(
      (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `audit-logs-${new Date().toISOString()}.csv`;
        a.click();
      }
    );
  }
}
```

#### 4.2 Template HTML
```html
<!-- frontend/src/app/admin/audit-logs/audit-log-list.component.html -->
<div class="audit-logs-container">
  <mat-card>
    <mat-card-header>
      <mat-card-title>
        <mat-icon>security</mat-icon>
        Logs de Auditoria LGPD
      </mat-card-title>
    </mat-card-header>
    
    <mat-card-content>
      <!-- Filtros -->
      <div class="filters-section">
        <h3>Filtros</h3>
        <div class="filters-grid">
          <mat-form-field>
            <mat-label>Usuário</mat-label>
            <input matInput [(ngModel)]="filters.userId" placeholder="ID do usuário">
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Ação</mat-label>
            <mat-select [(ngModel)]="filters.action">
              <mat-option value="">Todas</mat-option>
              <mat-option value="Create">Criação</mat-option>
              <mat-option value="Read">Leitura</mat-option>
              <mat-option value="Update">Atualização</mat-option>
              <mat-option value="Delete">Exclusão</mat-option>
              <mat-option value="Login">Login</mat-option>
              <mat-option value="Export">Exportação</mat-option>
            </mat-select>
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Tipo de Entidade</mat-label>
            <input matInput [(ngModel)]="filters.entityType" placeholder="Patient, MedicalRecord, etc.">
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Resultado</mat-label>
            <mat-select [(ngModel)]="filters.result">
              <mat-option value="">Todos</mat-option>
              <mat-option value="Success">Sucesso</mat-option>
              <mat-option value="Failed">Falha</mat-option>
              <mat-option value="Unauthorized">Não Autorizado</mat-option>
            </mat-select>
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Data Início</mat-label>
            <input matInput [matDatepicker]="pickerFrom" [(ngModel)]="filters.dateFrom">
            <mat-datepicker-toggle matSuffix [for]="pickerFrom"></mat-datepicker-toggle>
            <mat-datepicker #pickerFrom></mat-datepicker>
          </mat-form-field>
          
          <mat-form-field>
            <mat-label>Data Fim</mat-label>
            <input matInput [matDatepicker]="pickerTo" [(ngModel)]="filters.dateTo">
            <mat-datepicker-toggle matSuffix [for]="pickerTo"></mat-datepicker-toggle>
            <mat-datepicker #pickerTo></mat-datepicker>
          </mat-form-field>
          
          <mat-checkbox [(ngModel)]="filters.isSensitiveOnly">
            Apenas Dados Sensíveis
          </mat-checkbox>
        </div>
        
        <div class="filter-actions">
          <button mat-raised-button color="primary" (click)="applyFilters()">
            <mat-icon>search</mat-icon>
            Aplicar Filtros
          </button>
          <button mat-button (click)="clearFilters()">
            <mat-icon>clear</mat-icon>
            Limpar
          </button>
          <button mat-raised-button color="accent" (click)="exportToCSV()">
            <mat-icon>download</mat-icon>
            Exportar CSV
          </button>
        </div>
      </div>
      
      <!-- Tabela de Logs -->
      <div class="table-container">
        <table mat-table [dataSource]="auditLogs" class="audit-logs-table">
          
          <!-- Timestamp Column -->
          <ng-container matColumnDef="timestamp">
            <th mat-header-cell *matHeaderCellDef>Data/Hora</th>
            <td mat-cell *matCellDef="let log">
              {{ log.timestamp | date:'dd/MM/yyyy HH:mm:ss' }}
            </td>
          </ng-container>
          
          <!-- User Column -->
          <ng-container matColumnDef="user">
            <th mat-header-cell *matHeaderCellDef>Usuário</th>
            <td mat-cell *matCellDef="let log">
              <div class="user-info">
                <strong>{{ log.userName }}</strong>
                <small>{{ log.userEmail }}</small>
              </div>
            </td>
          </ng-container>
          
          <!-- Action Column -->
          <ng-container matColumnDef="action">
            <th mat-header-cell *matHeaderCellDef>Ação</th>
            <td mat-cell *matCellDef="let log">
              <mat-chip [color]="getActionColor(log.action)" selected>
                {{ log.action }}
              </mat-chip>
            </td>
          </ng-container>
          
          <!-- Entity Column -->
          <ng-container matColumnDef="entity">
            <th mat-header-cell *matHeaderCellDef>Entidade</th>
            <td mat-cell *matCellDef="let log">
              <div class="entity-info">
                <strong>{{ log.entityType }}</strong>
                <small>{{ log.entityId }}</small>
              </div>
            </td>
          </ng-container>
          
          <!-- Result Column -->
          <ng-container matColumnDef="result">
            <th mat-header-cell *matHeaderCellDef>Resultado</th>
            <td mat-cell *matCellDef="let log">
              <mat-icon [class]="'result-icon ' + log.result.toLowerCase()">
                {{ getResultIcon(log.result) }}
              </mat-icon>
              {{ log.result }}
            </td>
          </ng-container>
          
          <!-- IP Address Column -->
          <ng-container matColumnDef="ipAddress">
            <th mat-header-cell *matHeaderCellDef>IP</th>
            <td mat-cell *matCellDef="let log">{{ log.ipAddress }}</td>
          </ng-container>
          
          <!-- Actions Column -->
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Ações</th>
            <td mat-cell *matCellDef="let log">
              <button mat-icon-button (click)="viewDetails(log)" matTooltip="Ver Detalhes">
                <mat-icon>visibility</mat-icon>
              </button>
            </td>
          </ng-container>
          
          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>
      </div>
      
      <!-- Paginação -->
      <mat-paginator 
        [length]="totalCount"
        [pageSize]="filters.pageSize"
        [pageSizeOptions]="[10, 30, 50, 100]"
        (page)="onPageChange($event)">
      </mat-paginator>
    </mat-card-content>
  </mat-card>
</div>
```

#### 4.3 Estilos CSS
```scss
// frontend/src/app/admin/audit-logs/audit-log-list.component.scss
.audit-logs-container {
  padding: 20px;
}

.filters-section {
  margin-bottom: 30px;
  
  h3 {
    margin-bottom: 15px;
    color: #333;
  }
  
  .filters-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 15px;
    margin-bottom: 15px;
  }
  
  .filter-actions {
    display: flex;
    gap: 10px;
    
    button {
      mat-icon {
        margin-right: 5px;
      }
    }
  }
}

.table-container {
  overflow-x: auto;
  margin-bottom: 20px;
}

.audit-logs-table {
  width: 100%;
  
  .user-info,
  .entity-info {
    display: flex;
    flex-direction: column;
    
    small {
      color: #666;
      font-size: 0.85em;
    }
  }
  
  mat-chip {
    font-size: 0.85em;
  }
  
  .result-icon {
    vertical-align: middle;
    margin-right: 5px;
    
    &.success {
      color: #4caf50;
    }
    
    &.failed {
      color: #f44336;
    }
    
    &.unauthorized {
      color: #ff9800;
    }
  }
}

mat-card {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

mat-card-header {
  mat-card-title {
    display: flex;
    align-items: center;
    gap: 10px;
    font-size: 1.5em;
    
    mat-icon {
      color: #1976d2;
    }
  }
}

// Responsividade
@media (max-width: 768px) {
  .filters-grid {
    grid-template-columns: 1fr;
  }
  
  .filter-actions {
    flex-direction: column;
    
    button {
      width: 100%;
    }
  }
}
```

#### 4.4 TypeScript Helper Methods
```typescript
// Adicionar ao audit-log-list.component.ts

  getActionColor(action: string): string {
    const colorMap = {
      'Create': 'primary',
      'Read': 'accent',
      'Update': 'warn',
      'Delete': 'warn',
      'Login': 'primary',
      'Export': 'accent'
    };
    return colorMap[action] || '';
  }
  
  getResultIcon(result: string): string {
    const iconMap = {
      'Success': 'check_circle',
      'Failed': 'error',
      'Unauthorized': 'warning'
    };
    return iconMap[result] || 'info';
  }
  
  onPageChange(event: any) {
    this.filters.page = event.pageIndex + 1;
    this.filters.pageSize = event.pageSize;
    this.loadAuditLogs();
  }
}

// Componente de Diálogo para Detalhes
@Component({
  selector: 'audit-log-details-dialog',
  template: `
    <h2 mat-dialog-title>Detalhes do Log de Auditoria</h2>
    <mat-dialog-content>
      <div class="detail-section">
        <h3>Informações Básicas</h3>
        <div class="detail-row">
          <strong>Timestamp:</strong>
          <span>{{ data.timestamp | date:'dd/MM/yyyy HH:mm:ss' }}</span>
        </div>
        <div class="detail-row">
          <strong>Usuário:</strong>
          <span>{{ data.userName }} ({{ data.userEmail }})</span>
        </div>
        <div class="detail-row">
          <strong>Ação:</strong>
          <span>{{ data.action }}</span>
        </div>
        <div class="detail-row">
          <strong>Resultado:</strong>
          <span>{{ data.result }}</span>
        </div>
      </div>
      
      <div class="detail-section">
        <h3>Contexto</h3>
        <div class="detail-row">
          <strong>IP:</strong>
          <span>{{ data.ipAddress }}</span>
        </div>
        <div class="detail-row">
          <strong>User Agent:</strong>
          <span>{{ data.userAgent }}</span>
        </div>
        <div class="detail-row">
          <strong>URL:</strong>
          <span>{{ data.requestUrl }}</span>
        </div>
      </div>
      
      <div class="detail-section" *ngIf="data.oldValues || data.newValues">
        <h3>Dados Alterados</h3>
        <div class="detail-row" *ngIf="data.oldValues">
          <strong>Valores Antigos:</strong>
          <pre>{{ data.oldValues }}</pre>
        </div>
        <div class="detail-row" *ngIf="data.newValues">
          <strong>Valores Novos:</strong>
          <pre>{{ data.newValues }}</pre>
        </div>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Fechar</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .detail-section {
      margin-bottom: 20px;
      
      h3 {
        color: #1976d2;
        margin-bottom: 10px;
      }
      
      .detail-row {
        display: grid;
        grid-template-columns: 150px 1fr;
        gap: 10px;
        padding: 8px 0;
        border-bottom: 1px solid #eee;
        
        strong {
          color: #666;
        }
        
        pre {
          background: #f5f5f5;
          padding: 10px;
          border-radius: 4px;
          overflow-x: auto;
        }
      }
    }
  `]
})
export class AuditLogDetailsDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: AuditLog) {}
}
```

### 5. LGPD Específico (1 semana)

#### 5.1 Serviço de Gestão de Consentimento
```csharp
// src/MedicSoft.Core/Services/LGPD/ConsentManagementService.cs
namespace MedicSoft.Core.Services.LGPD
{
    public interface IConsentManagementService
    {
        Task<Guid> RecordConsentAsync(Guid patientId, ConsentType type, ConsentPurpose purpose);
        Task RevokeConsentAsync(Guid consentId, string reason);
        Task<bool> HasActiveConsentAsync(Guid patientId, ConsentType type);
        Task<List<DataConsentLog>> GetPatientConsentsAsync(Guid patientId);
    }
    
    public class ConsentManagementService : IConsentManagementService
    {
        private readonly IRepository<DataConsentLog> _consentRepository;
        private readonly IAuditService _auditService;
        
        public async Task<Guid> RecordConsentAsync(
            Guid patientId, 
            ConsentType type, 
            ConsentPurpose purpose)
        {
            var consent = new DataConsentLog
            {
                PatientId = patientId,
                Type = type,
                Purpose = purpose,
                Status = ConsentStatus.Active,
                Timestamp = DateTime.UtcNow,
                ConsentVersion = "1.0"
            };
            
            await _consentRepository.AddAsync(consent);
            
            // Auditar
            await _auditService.LogActionAsync(
                patientId.ToString(),
                AuditActionType.ConsentGiven,
                nameof(DataConsentLog),
                consent.Id.ToString()
            );
            
            return consent.Id;
        }
        
        public async Task RevokeConsentAsync(Guid consentId, string reason)
        {
            var consent = await _consentRepository.GetByIdAsync(consentId);
            if (consent == null) return;
            
            consent.Status = ConsentStatus.Revoked;
            consent.RevokedDate = DateTime.UtcNow;
            consent.RevocationReason = reason;
            
            await _consentRepository.UpdateAsync(consent);
            
            // Auditar
            await _auditService.LogActionAsync(
                consent.PatientId.ToString(),
                AuditActionType.ConsentRevoked,
                nameof(DataConsentLog),
                consentId.ToString()
            );
        }
    }
}
```

#### 5.2 Direito ao Esquecimento
```csharp
// src/MedicSoft.Core/Services/LGPD/DataDeletionService.cs
namespace MedicSoft.Core.Services.LGPD
{
    public interface IDataDeletionService
    {
        Task<DataDeletionRequest> RequestDataDeletionAsync(Guid patientId, string reason);
        Task ProcessDataDeletionAsync(Guid requestId);
        Task<bool> CanDeletePatientDataAsync(Guid patientId);
    }
    
    public class DataDeletionService : IDataDeletionService
    {
        private readonly IRepository<DataDeletionRequest> _requestRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<DataDeletionService> _logger;
        
        public DataDeletionService(
            IRepository<DataDeletionRequest> requestRepository,
            IRepository<Patient> patientRepository,
            IRepository<Appointment> appointmentRepository,
            IAuditService auditService,
            ILogger<DataDeletionService> logger)
        {
            _requestRepository = requestRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _auditService = auditService;
            _logger = logger;
        }
        
        // LGPD Art. 18 - Direito ao esquecimento
        
        public async Task<DataDeletionRequest> RequestDataDeletionAsync(
            Guid patientId, 
            string reason)
        {
            try
            {
                // Verificar se pode deletar (não pode ter pendências)
                var canDelete = await CanDeletePatientDataAsync(patientId);
                
                var request = new DataDeletionRequest
                {
                    PatientId = patientId,
                    RequestDate = DateTime.UtcNow,
                    Reason = reason,
                    Status = canDelete ? DeletionStatus.Pending : DeletionStatus.Blocked,
                    BlockingReasons = canDelete ? null : await GetBlockingReasonsAsync(patientId)
                };
                
                await _requestRepository.AddAsync(request);
                
                // Auditar solicitação
                await _auditService.LogActionAsync(
                    patientId.ToString(),
                    AuditActionType.DataDeletionRequest,
                    nameof(DataDeletionRequest),
                    request.Id.ToString()
                );
                
                _logger.LogInformation("Data deletion requested for patient {PatientId}, Status: {Status}", 
                    patientId, request.Status);
                
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to request data deletion for patient {PatientId}", patientId);
                throw;
            }
        }
        
        public async Task<bool> CanDeletePatientDataAsync(Guid patientId)
        {
            // Verificar se há consultas futuras agendadas
            var futureAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartTime > DateTime.UtcNow)
                .AnyAsync();
            
            if (futureAppointments)
                return false;
            
            // Verificar se há consultas recentes (últimos 30 dias)
            var recentAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartTime > DateTime.UtcNow.AddDays(-30))
                .AnyAsync();
            
            if (recentAppointments)
                return false;
            
            // Verificar se há obrigações legais de retenção
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return false;
            
            // Dados de saúde devem ser mantidos por 20 anos (CFM Resolução 1.821/2007)
            var creationDate = patient.CreatedAt;
            var retentionPeriod = TimeSpan.FromDays(365 * 20);
            var canDeleteAfter = creationDate.Add(retentionPeriod);
            
            if (DateTime.UtcNow < canDeleteAfter)
                return false;
            
            return true;
        }
        
        private async Task<string> GetBlockingReasonsAsync(Guid patientId)
        {
            var reasons = new List<string>();
            
            var futureAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartTime > DateTime.UtcNow)
                .CountAsync();
            
            if (futureAppointments > 0)
                reasons.Add($"{futureAppointments} consultas futuras agendadas");
            
            var recentAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.PatientId == patientId && a.StartTime > DateTime.UtcNow.AddDays(-30))
                .CountAsync();
            
            if (recentAppointments > 0)
                reasons.Add($"{recentAppointments} consultas nos últimos 30 dias");
            
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient != null)
            {
                var creationDate = patient.CreatedAt;
                var retentionPeriod = TimeSpan.FromDays(365 * 20);
                var canDeleteAfter = creationDate.Add(retentionPeriod);
                
                if (DateTime.UtcNow < canDeleteAfter)
                {
                    var yearsRemaining = (canDeleteAfter - DateTime.UtcNow).TotalDays / 365;
                    reasons.Add($"Retenção legal obrigatória por mais {yearsRemaining:F1} anos (CFM 1.821/2007)");
                }
            }
            
            return string.Join("; ", reasons);
        }
        
        public async Task ProcessDataDeletionAsync(Guid requestId)
        {
            try
            {
                // Anonimizar ao invés de deletar completamente
                // LGPD permite manter dados anonimizados para fins estatísticos
                
                var request = await _requestRepository.GetByIdAsync(requestId);
                if (request == null)
                    throw new InvalidOperationException($"Deletion request {requestId} not found");
                
                if (request.Status != DeletionStatus.Pending)
                    throw new InvalidOperationException($"Cannot process request with status {request.Status}");
                
                var patient = await _patientRepository.GetByIdAsync(request.PatientId);
                if (patient == null)
                    throw new InvalidOperationException($"Patient {request.PatientId} not found");
                
                // Anonimizar dados pessoais
                var anonymousId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                patient.Name = $"ANONIMIZADO_{anonymousId}";
                patient.CPF = null;
                patient.RG = null;
                patient.Email = null;
                patient.Phone = null;
                patient.MobilePhone = null;
                patient.Address = null;
                patient.City = null;
                patient.State = null;
                patient.PostalCode = null;
                patient.IsAnonymized = true;
                patient.AnonymizedAt = DateTime.UtcNow;
                
                await _patientRepository.UpdateAsync(patient);
                
                // Atualizar status da solicitação
                request.Status = DeletionStatus.Completed;
                request.CompletedDate = DateTime.UtcNow;
                request.ProcessedBy = "System"; // Ou ID do usuário que aprovou
                await _requestRepository.UpdateAsync(request);
                
                // Auditar conclusão
                await _auditService.LogActionAsync(
                    request.PatientId.ToString(),
                    AuditActionType.DataDeletionRequest,
                    nameof(Patient),
                    patient.Id.ToString(),
                    null,
                    new { Status = "Anonymized", AnonymousId = anonymousId }
                );
                
                _logger.LogInformation("Data deletion completed for patient {PatientId}, anonymized as {AnonymousId}", 
                    request.PatientId, anonymousId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process data deletion for request {RequestId}", requestId);
                
                // Atualizar status para erro
                var request = await _requestRepository.GetByIdAsync(requestId);
                if (request != null)
                {
                    request.Status = DeletionStatus.Failed;
                    request.FailureReason = ex.Message;
                    await _requestRepository.UpdateAsync(request);
                }
                
                throw;
            }
        }
    }
    
    // Supporting entities
    public class DataDeletionRequest : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public DeletionStatus Status { get; set; }
        public string BlockingReasons { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ProcessedBy { get; set; }
        public string FailureReason { get; set; }
    }
    
    public enum DeletionStatus
    {
        Pending,
        Blocked,
        Completed,
        Failed
    }
}
```

#### 5.3 Portabilidade de Dados
```csharp
// src/MedicSoft.Core/Services/LGPD/DataPortabilityService.cs
namespace MedicSoft.Core.Services.LGPD
{
    public interface IDataPortabilityService
    {
        Task<string> ExportPatientDataAsync(Guid patientId, ExportFormat format);
    }
    
    public class DataPortabilityService : IDataPortabilityService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<MedicalRecord> _medicalRecordRepository;
        private readonly IRepository<Prescription> _prescriptionRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<DataPortabilityService> _logger;
        
        public DataPortabilityService(
            IRepository<Patient> patientRepository,
            IRepository<MedicalRecord> medicalRecordRepository,
            IRepository<Prescription> prescriptionRepository,
            IRepository<Appointment> appointmentRepository,
            IAuditService auditService,
            ILogger<DataPortabilityService> logger)
        {
            _patientRepository = patientRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
            _auditService = auditService;
            _logger = logger;
        }
        
        // LGPD Art. 18 - Portabilidade de dados
        
        public async Task<string> ExportPatientDataAsync(
            Guid patientId, 
            ExportFormat format)
        {
            try
            {
                _logger.LogInformation("Starting data export for patient {PatientId} in format {Format}", 
                    patientId, format);
                
                // Coletar todos os dados do paciente
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                    throw new InvalidOperationException($"Patient {patientId} not found");
                
                var medicalRecords = await _medicalRecordRepository.GetAll()
                    .Where(m => m.PatientId == patientId)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();
                    
                var prescriptions = await _prescriptionRepository.GetAll()
                    .Where(p => p.PatientId == patientId)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();
                    
                var appointments = await _appointmentRepository.GetAll()
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.StartTime)
                    .ToListAsync();
                
                var exportData = new PatientDataExport
                {
                    PersonalData = patient,
                    MedicalRecords = medicalRecords,
                    Prescriptions = prescriptions,
                    Appointments = appointments,
                    ExportDate = DateTime.UtcNow,
                    ExportFormat = format.ToString(),
                    TotalRecords = medicalRecords.Count + prescriptions.Count + appointments.Count
                };
                
                // Auditar exportação
                await _auditService.LogActionAsync(
                    patientId.ToString(),
                    AuditActionType.DataPortabilityRequest,
                    "PatientDataExport",
                    patientId.ToString(),
                    null,
                    new { Format = format, RecordCount = exportData.TotalRecords }
                );
                
                _logger.LogInformation("Data export completed for patient {PatientId}, {RecordCount} records exported", 
                    patientId, exportData.TotalRecords);
                
                return format switch
                {
                    ExportFormat.JSON => SerializeToJson(exportData),
                    ExportFormat.XML => SerializeToXml(exportData),
                    ExportFormat.PDF => GeneratePdf(exportData),
                    _ => throw new NotSupportedException($"Export format {format} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export data for patient {PatientId}", patientId);
                throw;
            }
        }
        
        private string SerializeToJson(PatientDataExport exportData)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            return JsonSerializer.Serialize(exportData, options);
        }
        
        private string SerializeToXml(PatientDataExport exportData)
        {
            var xmlSerializer = new XmlSerializer(typeof(PatientDataExport));
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                Encoding = Encoding.UTF8
            });
            
            xmlSerializer.Serialize(xmlWriter, exportData);
            return stringWriter.ToString();
        }
        
        private string GeneratePdf(PatientDataExport exportData)
        {
            // Utilizando biblioteca iTextSharp ou similar para gerar PDF
            // Esta é uma implementação simplificada
            
            var htmlContent = GenerateHtmlReport(exportData);
            
            // Converter HTML para PDF usando biblioteca como SelectPdf, IronPdf, etc.
            // Por simplicidade, retornamos o HTML aqui - em produção, converter para PDF
            
            // Exemplo com IronPdf (requer pacote IronPdf):
            // var renderer = new ChromePdfRenderer();
            // var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            // return Convert.ToBase64String(pdf.BinaryData);
            
            return htmlContent; // Retorna HTML por simplicidade
        }
        
        private string GenerateHtmlReport(PatientDataExport exportData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset='UTF-8'>");
            sb.AppendLine("  <title>Exportação de Dados do Paciente</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body { font-family: Arial, sans-serif; margin: 20px; }");
            sb.AppendLine("    h1 { color: #1976d2; }");
            sb.AppendLine("    h2 { color: #424242; border-bottom: 2px solid #1976d2; padding-bottom: 5px; }");
            sb.AppendLine("    table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            sb.AppendLine("    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            sb.AppendLine("    th { background-color: #1976d2; color: white; }");
            sb.AppendLine("    .info-row { margin: 10px 0; }");
            sb.AppendLine("    .info-label { font-weight: bold; display: inline-block; width: 150px; }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            // Cabeçalho
            sb.AppendLine($"  <h1>Exportação de Dados do Paciente</h1>");
            sb.AppendLine($"  <p><strong>Data da Exportação:</strong> {exportData.ExportDate:dd/MM/yyyy HH:mm:ss}</p>");
            sb.AppendLine($"  <p><strong>Total de Registros:</strong> {exportData.TotalRecords}</p>");
            sb.AppendLine("  <hr>");
            
            // Dados Pessoais
            sb.AppendLine("  <h2>Dados Pessoais</h2>");
            sb.AppendLine($"  <div class='info-row'><span class='info-label'>Nome:</span> {exportData.PersonalData.Name}</div>");
            sb.AppendLine($"  <div class='info-row'><span class='info-label'>CPF:</span> {exportData.PersonalData.CPF}</div>");
            sb.AppendLine($"  <div class='info-row'><span class='info-label'>Data de Nascimento:</span> {exportData.PersonalData.BirthDate:dd/MM/yyyy}</div>");
            sb.AppendLine($"  <div class='info-row'><span class='info-label'>Email:</span> {exportData.PersonalData.Email}</div>");
            sb.AppendLine($"  <div class='info-row'><span class='info-label'>Telefone:</span> {exportData.PersonalData.Phone}</div>");
            
            // Prontuários Médicos
            if (exportData.MedicalRecords.Any())
            {
                sb.AppendLine("  <h2>Prontuários Médicos</h2>");
                sb.AppendLine("  <table>");
                sb.AppendLine("    <tr><th>Data</th><th>Profissional</th><th>Diagnóstico</th><th>Observações</th></tr>");
                foreach (var record in exportData.MedicalRecords.Take(50)) // Limitar para PDF
                {
                    sb.AppendLine($"    <tr>");
                    sb.AppendLine($"      <td>{record.Date:dd/MM/yyyy}</td>");
                    sb.AppendLine($"      <td>{record.DoctorName}</td>");
                    sb.AppendLine($"      <td>{record.Diagnosis}</td>");
                    sb.AppendLine($"      <td>{record.Notes?.Substring(0, Math.Min(100, record.Notes.Length ?? 0))}</td>");
                    sb.AppendLine($"    </tr>");
                }
                sb.AppendLine("  </table>");
            }
            
            // Prescrições
            if (exportData.Prescriptions.Any())
            {
                sb.AppendLine("  <h2>Prescrições</h2>");
                sb.AppendLine("  <table>");
                sb.AppendLine("    <tr><th>Data</th><th>Medicamento</th><th>Dosagem</th><th>Duração</th></tr>");
                foreach (var prescription in exportData.Prescriptions.Take(50))
                {
                    sb.AppendLine($"    <tr>");
                    sb.AppendLine($"      <td>{prescription.Date:dd/MM/yyyy}</td>");
                    sb.AppendLine($"      <td>{prescription.Medication}</td>");
                    sb.AppendLine($"      <td>{prescription.Dosage}</td>");
                    sb.AppendLine($"      <td>{prescription.Duration}</td>");
                    sb.AppendLine($"    </tr>");
                }
                sb.AppendLine("  </table>");
            }
            
            // Consultas
            if (exportData.Appointments.Any())
            {
                sb.AppendLine("  <h2>Histórico de Consultas</h2>");
                sb.AppendLine("  <table>");
                sb.AppendLine("    <tr><th>Data</th><th>Profissional</th><th>Especialidade</th><th>Status</th></tr>");
                foreach (var appointment in exportData.Appointments.Take(50))
                {
                    sb.AppendLine($"    <tr>");
                    sb.AppendLine($"      <td>{appointment.StartTime:dd/MM/yyyy HH:mm}</td>");
                    sb.AppendLine($"      <td>{appointment.DoctorName}</td>");
                    sb.AppendLine($"      <td>{appointment.Specialty}</td>");
                    sb.AppendLine($"      <td>{appointment.Status}</td>");
                    sb.AppendLine($"    </tr>");
                }
                sb.AppendLine("  </table>");
            }
            
            // Rodapé
            sb.AppendLine("  <hr>");
            sb.AppendLine("  <p><small>Este documento foi gerado automaticamente em conformidade com a LGPD (Lei 13.709/2018), Art. 18, inciso IV - Direito à portabilidade de dados.</small></p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
    }
    
    // Supporting classes
    public class PatientDataExport
    {
        public Patient PersonalData { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public List<Prescription> Prescriptions { get; set; }
        public List<Appointment> Appointments { get; set; }
        public DateTime ExportDate { get; set; }
        public string ExportFormat { get; set; }
        public int TotalRecords { get; set; }
    }
    
    public enum ExportFormat
    {
        JSON,
        XML,
        PDF
    }
}
```

### 6. Testes (1 semana)

#### 6.1 Testes Unitários
```csharp
// tests/MedicSoft.Tests/Services/AuditServiceTests.cs
public class AuditServiceTests
{
    [Fact]
    public async Task LogActionAsync_Should_CreateAuditLog()
    {
        // Arrange
        var service = CreateAuditService();
        
        // Act
        await service.LogActionAsync(
            "user123",
            AuditActionType.Create,
            "Patient",
            "patient123"
        );
        
        // Assert
        var logs = await service.GetUserActivityAsync("user123");
        Assert.Single(logs);
        Assert.Equal(AuditActionType.Create, logs[0].Action);
    }
    
    [Fact]
    public async Task LogDataAccessAsync_Should_RecordSensitiveAccess()
    {
        // Arrange
        var service = CreateAuditService();
        var patientId = Guid.NewGuid();
        
        // Act
        await service.LogDataAccessAsync(
            "doctor123",
            "MedicalRecord",
            "record123",
            new List<string> { "Diagnosis", "Prescription" },
            "Medical consultation"
        );
        
        // Assert
        var logs = await service.GetPatientDataAccessHistoryAsync(patientId);
        Assert.Single(logs);
    }
    
    [Fact]
    public async Task GetAuditLogsAsync_Should_ApplyFilters()
    {
        // Arrange
        var service = CreateAuditService();
        await SeedTestData(service);
        
        var filter = new AuditLogFilterDto
        {
            Action = AuditActionType.Create,
            IsSensitiveOnly = true,
            DateFrom = DateTime.UtcNow.AddDays(-7),
            Page = 1,
            PageSize = 10
        };
        
        // Act
        var result = await service.GetAuditLogsAsync(filter);
        
        // Assert
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, log => Assert.Equal(AuditActionType.Create, log.Action));
        Assert.All(result.Items, log => Assert.True(log.IsSensitiveData));
    }
    
    [Fact]
    public async Task GetAuditLogsAsync_Should_Paginate()
    {
        // Arrange
        var service = CreateAuditService();
        await SeedTestData(service, recordCount: 100);
        
        var filter = new AuditLogFilterDto { Page = 2, PageSize = 30 };
        
        // Act
        var result = await service.GetAuditLogsAsync(filter);
        
        // Assert
        Assert.Equal(30, result.Items.Count);
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(4, result.TotalPages); // 100 / 30 = 3.33 -> 4 pages
    }
}
```

#### 6.2 Testes de LGPD Services
```csharp
// tests/MedicSoft.Tests/Services/DataDeletionServiceTests.cs
public class DataDeletionServiceTests
{
    [Fact]
    public async Task RequestDataDeletion_Should_BlockIfFutureAppointments()
    {
        // Arrange
        var service = CreateDataDeletionService();
        var patientId = Guid.NewGuid();
        await SeedFutureAppointment(patientId);
        
        // Act
        var request = await service.RequestDataDeletionAsync(patientId, "Patient request");
        
        // Assert
        Assert.Equal(DeletionStatus.Blocked, request.Status);
        Assert.Contains("consultas futuras", request.BlockingReasons);
    }
    
    [Fact]
    public async Task RequestDataDeletion_Should_AllowIfNoPendencies()
    {
        // Arrange
        var service = CreateDataDeletionService();
        var patientId = Guid.NewGuid();
        await SeedOldPatientData(patientId, yearsOld: 21); // Além do período de retenção
        
        // Act
        var request = await service.RequestDataDeletionAsync(patientId, "Patient request");
        
        // Assert
        Assert.Equal(DeletionStatus.Pending, request.Status);
        Assert.Null(request.BlockingReasons);
    }
    
    [Fact]
    public async Task ProcessDataDeletion_Should_AnonymizePatientData()
    {
        // Arrange
        var service = CreateDataDeletionService();
        var patientId = Guid.NewGuid();
        var patient = await SeedPatient(patientId);
        var requestId = await CreateDeletionRequest(patientId, DeletionStatus.Pending);
        
        var originalCpf = patient.CPF;
        var originalEmail = patient.Email;
        
        // Act
        await service.ProcessDataDeletionAsync(requestId);
        
        // Assert
        var anonymized = await GetPatient(patientId);
        Assert.True(anonymized.IsAnonymized);
        Assert.StartsWith("ANONIMIZADO_", anonymized.Name);
        Assert.Null(anonymized.CPF);
        Assert.Null(anonymized.Email);
        Assert.NotEqual(originalCpf, anonymized.CPF);
        Assert.NotEqual(originalEmail, anonymized.Email);
    }
    
    [Fact]
    public async Task CanDeletePatientData_Should_RespectRetentionPeriod()
    {
        // Arrange
        var service = CreateDataDeletionService();
        var recentPatientId = Guid.NewGuid();
        var oldPatientId = Guid.NewGuid();
        
        await SeedOldPatientData(recentPatientId, yearsOld: 5);  // < 20 anos
        await SeedOldPatientData(oldPatientId, yearsOld: 21);    // > 20 anos
        
        // Act
        var canDeleteRecent = await service.CanDeletePatientDataAsync(recentPatientId);
        var canDeleteOld = await service.CanDeletePatientDataAsync(oldPatientId);
        
        // Assert
        Assert.False(canDeleteRecent); // Deve respeitar CFM 1.821/2007 (20 anos)
        Assert.True(canDeleteOld);
    }
}
```

```csharp
// tests/MedicSoft.Tests/Services/DataPortabilityServiceTests.cs
public class DataPortabilityServiceTests
{
    [Fact]
    public async Task ExportPatientData_Should_GenerateJSON()
    {
        // Arrange
        var service = CreateDataPortabilityService();
        var patientId = Guid.NewGuid();
        await SeedCompletePatientData(patientId);
        
        // Act
        var result = await service.ExportPatientDataAsync(patientId, ExportFormat.JSON);
        
        // Assert
        Assert.NotNull(result);
        var json = JsonDocument.Parse(result);
        Assert.True(json.RootElement.TryGetProperty("personalData", out _));
        Assert.True(json.RootElement.TryGetProperty("medicalRecords", out _));
        Assert.True(json.RootElement.TryGetProperty("prescriptions", out _));
    }
    
    [Fact]
    public async Task ExportPatientData_Should_GenerateXML()
    {
        // Arrange
        var service = CreateDataPortabilityService();
        var patientId = Guid.NewGuid();
        await SeedCompletePatientData(patientId);
        
        // Act
        var result = await service.ExportPatientDataAsync(patientId, ExportFormat.XML);
        
        // Assert
        Assert.NotNull(result);
        Assert.Contains("<?xml", result);
        Assert.Contains("<PatientDataExport>", result);
        Assert.Contains("<PersonalData>", result);
    }
    
    [Fact]
    public async Task ExportPatientData_Should_GeneratePDF()
    {
        // Arrange
        var service = CreateDataPortabilityService();
        var patientId = Guid.NewGuid();
        await SeedCompletePatientData(patientId);
        
        // Act
        var result = await service.ExportPatientDataAsync(patientId, ExportFormat.PDF);
        
        // Assert
        Assert.NotNull(result);
        Assert.Contains("<!DOCTYPE html>", result);
        Assert.Contains("Exportação de Dados do Paciente", result);
    }
    
    [Fact]
    public async Task ExportPatientData_Should_AuditExport()
    {
        // Arrange
        var service = CreateDataPortabilityService();
        var auditService = GetMockAuditService();
        var patientId = Guid.NewGuid();
        await SeedCompletePatientData(patientId);
        
        // Act
        await service.ExportPatientDataAsync(patientId, ExportFormat.JSON);
        
        // Assert
        var auditLogs = await auditService.GetUserActivityAsync(patientId.ToString());
        Assert.Contains(auditLogs, log => 
            log.Action == AuditActionType.DataPortabilityRequest &&
            log.EntityType == "PatientDataExport"
        );
    }
    
    [Fact]
    public async Task ExportPatientData_Should_IncludeAllData()
    {
        // Arrange
        var service = CreateDataPortabilityService();
        var patientId = Guid.NewGuid();
        
        await SeedPatient(patientId);
        await SeedMedicalRecords(patientId, count: 5);
        await SeedPrescriptions(patientId, count: 3);
        await SeedAppointments(patientId, count: 10);
        
        // Act
        var result = await service.ExportPatientDataAsync(patientId, ExportFormat.JSON);
        var data = JsonSerializer.Deserialize<PatientDataExport>(result);
        
        // Assert
        Assert.Equal(5, data.MedicalRecords.Count);
        Assert.Equal(3, data.Prescriptions.Count);
        Assert.Equal(10, data.Appointments.Count);
        Assert.Equal(18, data.TotalRecords);
    }
}
```

#### 6.3 Testes de Integração
```csharp
// tests/MedicSoft.Integration.Tests/AuditMiddlewareTests.cs
public class AuditMiddlewareIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    
    public AuditMiddlewareIntegrationTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task SensitiveEndpoint_Should_CreateAuditLog()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetAuthToken(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await client.GetAsync("/api/patients/123");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var auditLogs = await GetAuditLogs(client);
        Assert.Contains(auditLogs, log => 
            log.RequestUrl.Contains("/api/patients/123") &&
            log.Action == AuditActionType.Read
        );
    }
    
    [Fact]
    public async Task UnauthorizedAccess_Should_LogFailure()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Sem token de autenticação
        
        // Act
        var response = await client.GetAsync("/api/patients/123");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        
        var auditLogs = await GetAuditLogs(client);
        Assert.Contains(auditLogs, log => 
            log.Result == AuditResultType.Unauthorized
        );
    }
}
```

#### 6.4 Testes de Performance
```csharp
// tests/MedicSoft.Performance.Tests/AuditPerformanceTests.cs
public class AuditPerformanceTests
{
    [Fact]
    public async Task AuditLogging_Should_HaveLowOverhead()
    {
        // Arrange
        var service = CreateAuditService();
        var stopwatch = Stopwatch.StartNew();
        
        // Act - Executar 1000 operações
        var tasks = Enumerable.Range(0, 1000)
            .Select(i => service.LogActionAsync(
                $"user{i}",
                AuditActionType.Read,
                "Patient",
                $"patient{i}"
            ));
        
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert - Deve completar em menos de 2 segundos (< 2ms por log)
        Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
            $"Audit logging took {stopwatch.ElapsedMilliseconds}ms for 1000 operations");
    }
    
    [Fact]
    public async Task GetAuditLogs_WithLargeDataset_Should_BeFast()
    {
        // Arrange
        var service = CreateAuditService();
        await SeedTestData(service, recordCount: 10000);
        
        var filter = new AuditLogFilterDto 
        { 
            Page = 1, 
            PageSize = 30,
            DateFrom = DateTime.UtcNow.AddDays(-7)
        };
        
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var result = await service.GetAuditLogsAsync(filter);
        
        stopwatch.Stop();
        
        // Assert - Deve completar em menos de 500ms
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Query took {stopwatch.ElapsedMilliseconds}ms");
        Assert.Equal(30, result.Items.Count);
    }
}
```

### 7. Migração de Banco de Dados

#### 7.1 Script de Migration
```bash
# Criar migration
dotnet ef migrations add AddAuditingSystem -p src/MedicSoft.Infrastructure -s src/MedicSoft.Api

# Aplicar migration
dotnet ef database update -p src/MedicSoft.Infrastructure -s src/MedicSoft.Api
```

#### 7.2 Índices de Performance
```sql
-- Criar índices para otimizar consultas de auditoria
CREATE INDEX IX_AuditLogs_UserId_Timestamp ON AuditLogs(UserId, Timestamp DESC);
CREATE INDEX IX_AuditLogs_EntityType_EntityId ON AuditLogs(EntityType, EntityId);
CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp DESC);
CREATE INDEX IX_AuditLogs_IsSensitiveData ON AuditLogs(IsSensitiveData) WHERE IsSensitiveData = 1;
CREATE INDEX IX_AuditLogs_Action_Result ON AuditLogs(Action, Result);

CREATE INDEX IX_DataAccessLogs_PatientId_Timestamp ON DataAccessLogs(PatientId, Timestamp DESC);
CREATE INDEX IX_DataAccessLogs_UserId ON DataAccessLogs(UserId);

CREATE INDEX IX_DataConsentLogs_PatientId_Status ON DataConsentLogs(PatientId, Status);
```

#### 7.3 Estratégia de Migração de Dados Existentes
```csharp
// Migração de dados históricos (executar uma única vez após deploy)
// src/MedicSoft.Api/Migrations/Scripts/BackfillAuditLogs.cs
public class BackfillAuditLogsScript
{
    public async Task ExecuteAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<BackfillAuditLogsScript>>();
        var auditService = services.GetRequiredService<IAuditService>();
        
        logger.LogInformation("Starting audit logs backfill...");
        
        // Criar logs de auditoria para dados históricos críticos
        // (se necessário para compliance)
        
        // Exemplo: Registrar criação de pacientes existentes
        var patients = await GetAllPatients(services);
        foreach (var patient in patients)
        {
            await auditService.LogActionAsync(
                "SYSTEM",
                AuditActionType.Create,
                "Patient",
                patient.Id.ToString(),
                null,
                new { BackfillDate = DateTime.UtcNow, OriginalDate = patient.CreatedAt }
            );
        }
        
        logger.LogInformation("Audit logs backfill completed. {Count} records processed.", patients.Count);
    }
}
```

### 8. Otimização de Performance

#### 8.1 Async Logging
```csharp
// src/MedicSoft.Core/Services/Audit/AsyncAuditService.cs
public class AsyncAuditService : IAuditService
{
    private readonly BlockingCollection<AuditLog> _auditQueue;
    private readonly Task _processingTask;
    private readonly IAuditService _auditService;
    
    public AsyncAuditService(IAuditService auditService)
    {
        _auditService = auditService;
        _auditQueue = new BlockingCollection<AuditLog>(boundedCapacity: 10000);
        _processingTask = Task.Run(ProcessQueueAsync);
    }
    
    public Task LogAsync(AuditLog auditLog)
    {
        // Adicionar na fila sem bloquear
        if (!_auditQueue.TryAdd(auditLog, TimeSpan.FromMilliseconds(100)))
        {
            // Fila cheia - log de warning mas não falha
            _logger.LogWarning("Audit queue is full, dropping audit log");
        }
        
        return Task.CompletedTask; // Retorna imediatamente
    }
    
    private async Task ProcessQueueAsync()
    {
        foreach (var auditLog in _auditQueue.GetConsumingEnumerable())
        {
            try
            {
                await _auditService.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit log from queue");
            }
        }
    }
}
```

#### 8.2 Particionamento de Tabelas
```sql
-- Particionar tabela de AuditLogs por mês para melhor performance
-- SQL Server example
CREATE PARTITION FUNCTION PF_AuditLogs_ByMonth (DATETIME2)
AS RANGE RIGHT FOR VALUES (
    '2026-01-01', '2026-02-01', '2026-03-01', '2026-04-01',
    '2026-05-01', '2026-06-01', '2026-07-01', '2026-08-01',
    '2026-09-01', '2026-10-01', '2026-11-01', '2026-12-01'
);

CREATE PARTITION SCHEME PS_AuditLogs_ByMonth
AS PARTITION PF_AuditLogs_ByMonth
ALL TO ([PRIMARY]);

-- Recriar tabela com particionamento
CREATE TABLE AuditLogs_Partitioned (
    -- Colunas...
) ON PS_AuditLogs_ByMonth(Timestamp);
```

#### 8.3 Arquivamento Automático
```csharp
// src/MedicSoft.Core/Services/Audit/AuditArchiveService.cs
public class AuditArchiveService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditArchiveService> _logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ArchiveOldLogsAsync();
                
                // Executar diariamente
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in audit archive service");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
    
    private async Task ArchiveOldLogsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Arquivar logs com mais de 1 ano para blob storage
        var cutoffDate = DateTime.UtcNow.AddYears(-1);
        
        var oldLogs = await context.AuditLogs
            .Where(l => l.Timestamp < cutoffDate && !l.IsArchived)
            .Take(10000)
            .ToListAsync();
        
        if (oldLogs.Any())
        {
            // Exportar para blob storage (Azure Storage ou AWS S3)
            await ExportToBlobStorageAsync(oldLogs);
            
            // Marcar como arquivados (não deletar)
            foreach (var log in oldLogs)
            {
                log.IsArchived = true;
                log.ArchiveDate = DateTime.UtcNow;
            }
            
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Archived {Count} audit logs", oldLogs.Count);
        }
    }
}
```

### 9. Configuração Completa (appsettings.json)

```json
{
  "AuditSettings": {
    "Enabled": true,
    "AsyncLogging": true,
    "QueueCapacity": 10000,
    "RetentionPeriodDays": 2555,  // 7 anos (LGPD requirement)
    "ArchiveAfterDays": 365,      // Arquivar após 1 ano
    "SensitiveDataOnly": false,   // Log all operations, not just sensitive
    "ExcludedPaths": [
      "/api/health",
      "/api/metrics",
      "/swagger"
    ],
    "PerformanceThresholdMs": 100  // Log slow operations
  },
  
  "LGPDSettings": {
    "DataRetentionYears": 20,  // CFM Resolution 1.821/2007
    "ConsentExpirationDays": 365,
    "DeletionRequestReviewDays": 30,
    "AllowImmediateDeletion": false,
    "ExportFormats": ["JSON", "XML", "PDF"],
    "MaxExportSizeMB": 100
  },
  
  "AuditStorage": {
    "Provider": "AzureBlobStorage",  // ou "AWSS3", "FileSystem"
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "ContainerName": "audit-logs-archive",
    "ArchiveCompression": true,
    "CompressionLevel": "Optimal"
  },
  
  "Logging": {
    "LogLevel": {
      "MedicSoft.Core.Services.Audit": "Information",
      "MedicSoft.Api.Middleware.AuditMiddleware": "Information"
    }
  }
}
```

### 10. Deployment e Infraestrutura

#### 10.1 Docker Compose
```yaml
# docker-compose.audit.yml
version: '3.8'

services:
  medicsoft-api:
    environment:
      - AuditSettings__Enabled=true
      - AuditSettings__AsyncLogging=true
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
    volumes:
      - audit-logs:/app/logs/audit
    depends_on:
      - postgres
      - redis

  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: medicsoft_audit
      POSTGRES_USER: ${DB_USER}
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_audit_data:/var/lib/postgresql/data
      - ./scripts/audit-indexes.sql:/docker-entrypoint-initdb.d/01-indexes.sql
    ports:
      - "5432:5432"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: redis-server --appendonly yes

  audit-archiver:
    image: medicsoft-audit-archiver:latest
    environment:
      - SCHEDULE="0 2 * * *"  # Executar às 2AM diariamente
      - ARCHIVE_THRESHOLD_DAYS=365
      - BLOB_STORAGE_CONNECTION=${BLOB_CONNECTION_STRING}
    depends_on:
      - postgres

volumes:
  audit-logs:
  postgres_audit_data:
  redis_data:
```

#### 10.2 Kubernetes Deployment
```yaml
# k8s/audit-system.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: medicsoft-api-audit
  labels:
    app: medicsoft-api
    component: audit
spec:
  replicas: 3
  selector:
    matchLabels:
      app: medicsoft-api
  template:
    metadata:
      labels:
        app: medicsoft-api
    spec:
      containers:
      - name: api
        image: medicsoft-api:latest
        env:
        - name: AuditSettings__Enabled
          value: "true"
        - name: AuditSettings__AsyncLogging
          value: "true"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-credentials
              key: connection-string
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5

---
apiVersion: batch/v1
kind: CronJob
metadata:
  name: audit-archiver
spec:
  schedule: "0 2 * * *"  # Diariamente às 2AM
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: archiver
            image: medicsoft-audit-archiver:latest
            env:
            - name: DB_CONNECTION
              valueFrom:
                secretKeyRef:
                  name: db-credentials
                  key: connection-string
            - name: BLOB_CONNECTION
              valueFrom:
                secretKeyRef:
                  name: storage-credentials
                  key: connection-string
          restartPolicy: OnFailure
```

#### 10.3 Monitoramento e Alertas
```yaml
# prometheus/audit-alerts.yml
groups:
  - name: audit_alerts
    interval: 30s
    rules:
      - alert: HighAuditLogFailureRate
        expr: rate(audit_log_failures_total[5m]) > 0.01
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High audit log failure rate"
          description: "Audit logging is failing at {{ $value }} per second"
      
      - alert: AuditQueueFull
        expr: audit_queue_size > 9000
        for: 2m
        labels:
          severity: critical
        annotations:
          summary: "Audit queue is nearly full"
          description: "Audit queue size is {{ $value }}, max is 10000"
      
      - alert: SlowAuditQueries
        expr: histogram_quantile(0.95, rate(audit_query_duration_seconds_bucket[5m])) > 1
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "Slow audit queries detected"
          description: "95th percentile query time is {{ $value }} seconds"

      - alert: AuditStorageSpacelow
        expr: audit_storage_free_bytes < 10737418240  # 10GB
        for: 30m
        labels:
          severity: warning
        annotations:
          summary: "Audit storage space is low"
          description: "Only {{ $value | humanize }} bytes remaining"
```

#### 10.4 Backup e Disaster Recovery
```bash
#!/bin/bash
# scripts/backup-audit-logs.sh

# Backup diário de logs de auditoria
DATE=$(date +%Y%m%d)
BACKUP_DIR="/backups/audit-logs/$DATE"
DB_NAME="medicsoft_audit"

mkdir -p "$BACKUP_DIR"

# Backup do banco de dados
pg_dump -h localhost -U postgres -d $DB_NAME \
  -t audit_logs -t data_access_logs -t data_consent_logs \
  -F c -f "$BACKUP_DIR/audit-logs-$DATE.backup"

# Comprimir
gzip "$BACKUP_DIR/audit-logs-$DATE.backup"

# Upload para cloud storage
aws s3 cp "$BACKUP_DIR/audit-logs-$DATE.backup.gz" \
  s3://medicsoft-backups/audit-logs/$DATE/

# Manter apenas últimos 90 dias localmente
find /backups/audit-logs/* -type d -mtime +90 -exec rm -rf {} \;

echo "Audit logs backup completed: $DATE"
```

#### 10.5 Política de Retenção e Arquivamento
```csharp
// src/MedicSoft.Core/Services/Audit/RetentionPolicyService.cs
public class RetentionPolicyService : BackgroundService
{
    private readonly ILogger<RetentionPolicyService> _logger;
    private readonly IConfiguration _configuration;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retentionDays = _configuration.GetValue<int>("AuditSettings:RetentionPeriodDays");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ApplyRetentionPolicyAsync(retentionDays);
                
                // Executar semanalmente
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying retention policy");
            }
        }
    }
    
    private async Task ApplyRetentionPolicyAsync(int retentionDays)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        
        // IMPORTANTE: Nunca deletar logs de auditoria para compliance LGPD
        // Apenas mover para archive storage
        
        _logger.LogInformation("Retention policy: All logs are maintained for {RetentionDays} days", retentionDays);
        _logger.LogInformation("Logs older than {CutoffDate} should be in archive storage", cutoffDate);
        
        // Validar que logs antigos estão arquivados
        var unarchivedOldLogs = await context.AuditLogs
            .Where(l => l.Timestamp < cutoffDate && !l.IsArchived)
            .CountAsync();
        
        if (unarchivedOldLogs > 0)
        {
            _logger.LogWarning("{Count} old logs are not archived yet", unarchivedOldLogs);
        }
    }
}
```

### 11. Documentação de Compliance
```markdown
# Guia de Compliance LGPD - Sistema de Auditoria

## Artigos LGPD Atendidos

### Art. 37 - Registro de Operações
✅ **Implementado**: Sistema registra todas operações de tratamento de dados pessoais
- Tabela: `AuditLogs`
- Campos: Timestamp, UserId, Action, EntityType, EntityId, etc.
- Retenção: 7+ anos

### Art. 18, II - Direito ao Esquecimento
✅ **Implementado**: Processo de anonimização de dados
- Serviço: `DataDeletionService`
- Método: `ProcessDataDeletionAsync()`
- Conformidade: CFM 1.821/2007 (20 anos de retenção)

### Art. 18, IV - Portabilidade de Dados
✅ **Implementado**: Exportação estruturada de dados
- Serviço: `DataPortabilityService`
- Formatos: JSON, XML, PDF
- Tempo: < 30 segundos

### Art. 8 - Consentimento
✅ **Implementado**: Gestão completa de consentimentos
- Tabela: `DataConsentLogs`
- Rastreabilidade: Versão, data, IP, texto apresentado

## Checklist de Auditoria

- [ ] Todos os acessos a dados sensíveis são registrados
- [ ] Logs são imutáveis (append-only)
- [ ] Sistema de backup automático ativo
- [ ] Política de retenção configurada (7+ anos)
- [ ] Processo de portabilidade testado
- [ ] Processo de esquecimento testado
- [ ] Alertas de segurança configurados
- [ ] Documentação atualizada

## Evidências para ANPD

### Relatório de Acessos
```sql
-- Gerar relatório de acessos para ANPD
SELECT 
    DATE(Timestamp) as Data,
    COUNT(*) as TotalAcessos,
    COUNT(DISTINCT UserId) as UsuariosUnicos,
    COUNT(CASE WHEN IsSensitiveData = 1 THEN 1 END) as AcessosDadosSensiveis
FROM AuditLogs
WHERE Timestamp >= DATEADD(month, -6, GETDATE())
GROUP BY DATE(Timestamp)
ORDER BY Data DESC;
```

### Relatório de Incidentes
```sql
-- Acessos não autorizados nos últimos 30 dias
SELECT *
FROM AuditLogs
WHERE Result = 'Unauthorized'
  AND Timestamp >= DATEADD(day, -30, GETDATE())
ORDER BY Timestamp DESC;
```
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] 100% das operações sensíveis são auditadas
- [ ] Impacto de performance < 5% (benchmarking necessário)
- [ ] Logs retidos por no mínimo 7 anos (compliance LGPD)
- [ ] Exportação de dados de paciente em < 30 segundos
- [ ] Sistema de busca de logs funcional e eficiente

### Funcionais
- [ ] Interface de visualização de logs intuitiva
- [ ] Filtros avançados (usuário, ação, período, entidade)
- [ ] Dashboard com estatísticas de auditoria
- [ ] Alertas de atividades suspeitas
- [ ] Relatórios LGPD para ANPD

### LGPD
- [ ] Registro completo de consentimentos
- [ ] Direito ao esquecimento implementado (anonimização)
- [ ] Portabilidade de dados (JSON, XML, PDF)
- [ ] Relatório de acessos por paciente
- [ ] Conformidade com Art. 37 (registro de acesso)

### Segurança
- [ ] Logs são imutáveis (append-only)
- [ ] Acesso aos logs restrito a administradores
- [ ] Logs protegidos contra adulteração
- [ ] Backup automático de logs

## 📦 Entregáveis

1. **Backend**
   - Entidades de auditoria (AuditLog, DataConsentLog, DataAccessLog)
   - AuditService completo
   - Middleware de auditoria automática
   - APIs de consulta de logs
   - Serviços LGPD (consentimento, esquecimento, portabilidade)

2. **Frontend**
   - Tela de visualização de logs de auditoria
   - Filtros avançados e busca
   - Dashboard de estatísticas
   - Visualização de histórico por entidade
   - Relatório de acessos do paciente

3. **Documentação**
   - Guia de compliance LGPD
   - Documentação de APIs
   - Procedimentos de auditoria
   - Templates de relatórios para ANPD

4. **Testes**
   - Testes unitários de AuditService
   - Testes de integração de middleware
   - Testes de performance

## 🔗 Dependências

### Pré-requisitos
- ✅ Sistema de autenticação funcionando
- ✅ Entity Framework configurado
- ✅ Banco de dados em produção

### Dependências Externas
- Nenhuma (sistema interno)

### Tarefas Dependentes
- Task #09 (Criptografia) - pode se beneficiar de auditoria
- Task #12 (Melhorias de Segurança) - complementar

## 🧪 Testes

### Testes de Performance
```bash
# Benchmark de impacto no sistema
# Deve ser < 5% de overhead
k6 run tests/load/audit-impact-test.js
```

### Testes de Compliance
1. Verificar que TODAS as operações sensíveis são logadas
2. Confirmar que logs são imutáveis
3. Testar exportação de dados de paciente
4. Validar processo de direito ao esquecimento
5. Aprovar com consultor LGPD externo

## 📊 Métricas de Sucesso

- **Performance:** < 5% de overhead
- **Cobertura:** 100% de operações sensíveis auditadas
- **Retenção:** Logs por 7+ anos
- **Compliance:** Aprovação de consultor LGPD
- **Adoção:** Administradores usam dashboards semanalmente

## 🚨 Riscos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Impacto de performance alto | Média | Alto | Async logging, índices otimizados, cache |
| Volume de logs muito grande | Alta | Médio | Particionamento, arquivamento, compressão |
| Custo de armazenamento | Média | Médio | Armazenamento em blob storage (mais barato) |
| Complexidade de consultas | Baixa | Médio | Elasticsearch para logs (opcional) |

## 📚 Referências

### Legal
- [Lei 13.709/2018 - LGPD](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [Guia ANPD - Segurança da Informação](https://www.gov.br/anpd/)
- LGPD Art. 37 - Controlador deve manter registro de operações
- LGPD Art. 18 - Direitos do titular (portabilidade, esquecimento)

### Técnico
- [Serilog](https://serilog.net/) - Structured logging
- [ELK Stack](https://www.elastic.co/elastic-stack/) - Log management (opcional)
- [OWASP Logging Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html)

### Código
- `src/MedicSoft.Core/Entities/Audit/` - Entidades de auditoria
- `src/MedicSoft.Core/Services/Audit/` - Serviços de auditoria
- `src/MedicSoft.Api/Middleware/AuditMiddleware.cs` - Middleware
- `frontend/src/app/admin/audit-logs/` - Interface de auditoria

---

> **IMPORTANTE:** Esta task é **obrigatoriedade legal LGPD** e tem risco de multa de até R$ 50 milhões  
> **Próximos Passos:** Após aprovação, iniciar com modelagem de dados  
> **Última Atualização:** 23 de Janeiro de 2026
