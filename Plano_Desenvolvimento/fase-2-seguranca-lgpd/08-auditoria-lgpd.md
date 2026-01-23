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
            // Implementar paginação e filtros
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
            var logs = await _auditService.GetAuditLogsAsync(filter);
            var csv = GenerateCsv(logs);
            
            return File(
                Encoding.UTF8.GetBytes(csv),
                "text/csv",
                $"audit-logs-{DateTime.UtcNow:yyyyMMdd}.csv"
            );
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
        // LGPD Art. 18 - Direito ao esquecimento
        
        public async Task<DataDeletionRequest> RequestDataDeletionAsync(
            Guid patientId, 
            string reason)
        {
            // Verificar se pode deletar (não pode ter pendências)
            var canDelete = await CanDeletePatientDataAsync(patientId);
            
            var request = new DataDeletionRequest
            {
                PatientId = patientId,
                RequestDate = DateTime.UtcNow,
                Reason = reason,
                Status = canDelete ? DeletionStatus.Pending : DeletionStatus.Blocked
            };
            
            await _requestRepository.AddAsync(request);
            
            return request;
        }
        
        public async Task ProcessDataDeletionAsync(Guid requestId)
        {
            // Anonimizar ao invés de deletar completamente
            // LGPD permite manter dados anonimizados para fins estatísticos
            
            var request = await _requestRepository.GetByIdAsync(requestId);
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            
            // Anonimizar dados pessoais
            patient.Name = "ANONIMIZADO";
            patient.CPF = null;
            patient.Email = null;
            patient.Phone = null;
            patient.Address = null;
            patient.IsAnonymized = true;
            
            await _patientRepository.UpdateAsync(patient);
            
            request.Status = DeletionStatus.Completed;
            request.CompletedDate = DateTime.UtcNow;
            await _requestRepository.UpdateAsync(request);
        }
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
        // LGPD Art. 18 - Portabilidade de dados
        
        public async Task<string> ExportPatientDataAsync(
            Guid patientId, 
            ExportFormat format)
        {
            // Coletar todos os dados do paciente
            var patient = await _patientRepository.GetByIdAsync(patientId);
            var medicalRecords = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
            var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId);
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
            
            var exportData = new PatientDataExport
            {
                PersonalData = patient,
                MedicalRecords = medicalRecords,
                Prescriptions = prescriptions,
                Appointments = appointments,
                ExportDate = DateTime.UtcNow
            };
            
            // Auditar exportação
            await _auditService.LogActionAsync(
                patientId.ToString(),
                AuditActionType.DataPortabilityRequest,
                "PatientDataExport",
                patientId.ToString()
            );
            
            return format switch
            {
                ExportFormat.JSON => JsonSerializer.Serialize(exportData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }),
                ExportFormat.XML => SerializeToXml(exportData),
                ExportFormat.PDF => GeneratePdf(exportData),
                _ => throw new NotSupportedException()
            };
        }
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
}
```

### 7. Migração de Banco de Dados

```bash
# Criar migration
dotnet ef migrations add AddAuditingSystem -p src/MedicSoft.Infrastructure -s src/MedicSoft.Api

# Aplicar migration
dotnet ef database update -p src/MedicSoft.Infrastructure -s src/MedicSoft.Api
```

### 8. Configuração e Deploy (1 semana)

#### 8.1 Startup Configuration
```csharp
// src/MedicSoft.Api/Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Auditoria
    services.AddScoped<IAuditService, AuditService>();
    services.AddScoped<IConsentManagementService, ConsentManagementService>();
    services.AddScoped<IDataDeletionService, DataDeletionService>();
    services.AddScoped<IDataPortabilityService, DataPortabilityService>();
}

public void Configure(IApplicationBuilder app)
{
    // Middleware de auditoria (antes de MVC)
    app.UseMiddleware<AuditMiddleware>();
    
    app.UseAuthentication();
    app.UseAuthorization();
}
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
