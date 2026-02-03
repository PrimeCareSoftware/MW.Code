# 🔧 PROMPT: Backend - Sistema de Configuração de Módulos

> **Fase:** 1 de 5  
> **Duração Estimada:** 2-3 semanas  
> **Desenvolvedores:** 1-2  
> **Prioridade:** 🔥🔥🔥 ALTA

---

## 📋 Contexto

### Situação Atual

O sistema Omni Care já possui uma **base funcional** para gerenciamento de módulos:

**Existente:**
- ✅ `ModuleConfiguration` (entidade do domínio)
- ✅ `ModuleConfigController` (API básica)
- ✅ `SystemModules` (constantes de módulos)
- ✅ Vinculação com `SubscriptionPlan`
- ✅ Endpoints básicos de enable/disable por clínica

**Localização dos Arquivos:**
```
/src/MedicSoft.Domain/Entities/
  ├── ModuleConfiguration.cs
  ├── SubscriptionPlan.cs
  └── SystemModules (static class dentro de ModuleConfiguration.cs)

/src/MedicSoft.Api/Controllers/
  └── ModuleConfigController.cs

/src/MedicSoft.Application/Services/
  └── (não existe serviço específico ainda)

/src/MedicSoft.Domain/Interfaces/
  └── IModuleConfigurationRepository.cs
```

### O Que Precisa Ser Desenvolvido

Expandir o sistema existente para suportar:

1. **Configuração Global (System Admin)**
   - Habilitar/desabilitar módulos para todas as clínicas
   - Configurar módulos disponíveis por plano
   - Criar/editar/deletar novos módulos

2. **Configuração Avançada por Clínica**
   - Ajustes finos de configuração por módulo
   - Validações complexas de permissões
   - Histórico de mudanças

3. **Métricas e Analytics**
   - Contabilizar uso de módulos
   - Identificar módulos mais/menos usados
   - Relatórios de adoção

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais

1. Criar endpoints REST para configuração global de módulos
2. Expandir endpoints existentes com funcionalidades avançadas
3. Implementar serviços de negócio para validações
4. Adicionar auditoria completa de mudanças
5. Criar endpoints de métricas e analytics

### Benefícios Esperados

- 📊 **API Completa:** Todos os CRUDs necessários
- 🔐 **Segurança:** Validações de permissão robustas
- 📈 **Visibilidade:** Métricas de uso de módulos
- 🔍 **Auditoria:** Rastreamento de todas as mudanças

---

## 📝 Tarefas Detalhadas

### 1. Expandir Entidade de Domínio (2 dias)

#### 1.1. Adicionar Propriedades ao SystemModules

**Arquivo:** `/src/MedicSoft.Domain/Entities/ModuleConfiguration.cs`

**Adicionar metadados aos módulos:**

```csharp
public static class SystemModules
{
    // Módulos existentes...
    public const string PatientManagement = "PatientManagement";
    // ... outros módulos ...

    // NOVA ESTRUTURA: Informações detalhadas dos módulos
    public static Dictionary<string, ModuleInfo> GetModulesInfo() => new()
    {
        [PatientManagement] = new ModuleInfo
        {
            Name = PatientManagement,
            DisplayName = "Gestão de Pacientes",
            Description = "Cadastro, edição e consulta de pacientes",
            Category = "Core",
            Icon = "people",
            IsCore = true, // Não pode ser desabilitado
            RequiredModules = new[] { "UserManagement" },
            MinimumPlan = SubscriptionPlanType.Basic
        },
        [AppointmentScheduling] = new ModuleInfo
        {
            Name = AppointmentScheduling,
            DisplayName = "Agendamento de Consultas",
            Description = "Sistema de agendamento e controle de horários",
            Category = "Core",
            Icon = "calendar_today",
            IsCore = true,
            RequiredModules = new[] { "PatientManagement" },
            MinimumPlan = SubscriptionPlanType.Basic
        },
        [Reports] = new ModuleInfo
        {
            Name = Reports,
            DisplayName = "Relatórios Avançados",
            Description = "Geração de relatórios e dashboards",
            Category = "Analytics",
            Icon = "assessment",
            IsCore = false,
            RequiredModules = Array.Empty<string>(),
            MinimumPlan = SubscriptionPlanType.Standard
        },
        // ... configurar todos os 13 módulos
    };

    public static string[] GetAllModules() => 
        GetModulesInfo().Keys.ToArray();

    public static ModuleInfo GetModuleInfo(string moduleName) =>
        GetModulesInfo().TryGetValue(moduleName, out var info) 
            ? info 
            : throw new ArgumentException($"Module {moduleName} not found");
}

public class ModuleInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Core", "Advanced", "Premium", "Analytics"
    public string Icon { get; set; } = string.Empty; // Material icon name
    public bool IsCore { get; set; } // Se true, não pode ser desabilitado
    public string[] RequiredModules { get; set; } = Array.Empty<string>();
    public SubscriptionPlanType MinimumPlan { get; set; }
}
```

#### 1.2. Adicionar Histórico de Mudanças

**Criar nova entidade:** `/src/MedicSoft.Domain/Entities/ModuleConfigurationHistory.cs`

```csharp
namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Histórico de mudanças de configuração de módulos
    /// </summary>
    public class ModuleConfigurationHistory : BaseEntity
    {
        public Guid ModuleConfigurationId { get; private set; }
        public Guid ClinicId { get; private set; }
        public string ModuleName { get; private set; }
        public string Action { get; private set; } // "Enabled", "Disabled", "ConfigUpdated"
        public string? PreviousConfiguration { get; private set; }
        public string? NewConfiguration { get; private set; }
        public string ChangedBy { get; private set; } // User ID ou "System"
        public DateTime ChangedAt { get; private set; }
        public string? Reason { get; private set; } // Motivo da mudança

        // Navigation
        public ModuleConfiguration? ModuleConfiguration { get; private set; }

        private ModuleConfigurationHistory()
        {
            ModuleName = null!;
            Action = null!;
            ChangedBy = null!;
        }

        public ModuleConfigurationHistory(
            Guid moduleConfigurationId,
            Guid clinicId,
            string moduleName,
            string action,
            string changedBy,
            string tenantId,
            string? previousConfig = null,
            string? newConfig = null,
            string? reason = null) : base(tenantId)
        {
            ModuleConfigurationId = moduleConfigurationId;
            ClinicId = clinicId;
            ModuleName = moduleName;
            Action = action;
            ChangedBy = changedBy;
            ChangedAt = DateTime.UtcNow;
            PreviousConfiguration = previousConfig;
            NewConfiguration = newConfig;
            Reason = reason;
        }
    }
}
```

#### 1.3. Expandir SubscriptionPlan

**Arquivo:** `/src/MedicSoft.Domain/Entities/SubscriptionPlan.cs`

**Adicionar propriedade para módulos customizados:**

```csharp
public class SubscriptionPlan : BaseEntity
{
    // Propriedades existentes...
    
    // NOVA PROPRIEDADE: Módulos habilitados (JSON)
    public string? EnabledModules { get; private set; } // JSON array de módulos
    
    // Método para gerenciar módulos
    public void SetEnabledModules(string[] modules)
    {
        EnabledModules = System.Text.Json.JsonSerializer.Serialize(modules);
        UpdateTimestamp();
    }
    
    public string[] GetEnabledModules()
    {
        if (string.IsNullOrEmpty(EnabledModules))
            return Array.Empty<string>();
            
        return System.Text.Json.JsonSerializer.Deserialize<string[]>(EnabledModules) 
            ?? Array.Empty<string>();
    }
    
    // Verificar se módulo está habilitado no plano
    public bool HasModule(string moduleName)
    {
        var enabledModules = GetEnabledModules();
        if (enabledModules.Length > 0)
            return enabledModules.Contains(moduleName);
            
        // Fallback para propriedades antigas
        return moduleName switch
        {
            SystemModules.Reports => HasReports,
            SystemModules.WhatsAppIntegration => HasWhatsAppIntegration,
            SystemModules.SMSNotifications => HasSMSNotifications,
            SystemModules.TissExport => HasTissExport,
            _ => true // Módulos core estão em todos os planos
        };
    }
}
```

---

### 2. Criar Services de Negócio (3-4 dias)

#### 2.1. ModuleConfigurationService

**Criar:** `/src/MedicSoft.Application/Services/ModuleConfigurationService.cs`

```csharp
namespace MedicSoft.Application.Services
{
    public interface IModuleConfigurationService
    {
        // Configuração por Clínica
        Task<ModuleConfigDto> GetModuleConfigAsync(Guid clinicId, string moduleName);
        Task<IEnumerable<ModuleConfigDto>> GetAllModuleConfigsAsync(Guid clinicId);
        Task EnableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null);
        Task DisableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null);
        Task UpdateModuleConfigAsync(Guid clinicId, string moduleName, string configuration, string userId);
        
        // Configuração Global (System Admin)
        Task<IEnumerable<ModuleUsageDto>> GetGlobalModuleUsageAsync();
        Task EnableModuleGloballyAsync(string moduleName, string userId);
        Task DisableModuleGloballyAsync(string moduleName, string userId);
        Task<IEnumerable<ModuleConfigHistoryDto>> GetModuleHistoryAsync(Guid clinicId, string moduleName);
        
        // Validações
        Task<bool> CanEnableModuleAsync(Guid clinicId, string moduleName);
        Task<bool> HasRequiredModulesAsync(Guid clinicId, string moduleName);
        Task<ValidationResult> ValidateModuleConfigAsync(Guid clinicId, string moduleName);
    }

    public class ModuleConfigurationService : IModuleConfigurationService
    {
        private readonly IModuleConfigurationRepository _repository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ModuleConfigurationService> _logger;

        public ModuleConfigurationService(
            IModuleConfigurationRepository repository,
            ISubscriptionPlanRepository planRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            MedicSoftDbContext context,
            ILogger<ModuleConfigurationService> logger)
        {
            _repository = repository;
            _planRepository = planRepository;
            _subscriptionRepository = subscriptionRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<ModuleConfigDto> GetModuleConfigAsync(Guid clinicId, string moduleName)
        {
            // Implementar lógica para buscar configuração
            // Incluir informações do plano e disponibilidade
        }

        public async Task EnableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null)
        {
            // 1. Validar se módulo existe
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            // 2. Validar se módulo está disponível no plano
            var validation = await ValidateModuleConfigAsync(clinicId, moduleName);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            // 3. Verificar módulos requeridos
            if (!await HasRequiredModulesAsync(clinicId, moduleName))
                throw new InvalidOperationException("Required modules are not enabled");

            // 4. Habilitar módulo
            var config = await _repository.GetByClinicAndModuleAsync(clinicId, moduleName);
            if (config == null)
            {
                config = new ModuleConfiguration(clinicId, moduleName, userId, true);
                await _repository.AddAsync(config);
            }
            else
            {
                config.Enable();
                await _repository.UpdateAsync(config);
            }

            // 5. Registrar histórico
            var history = new ModuleConfigurationHistory(
                config.Id,
                clinicId,
                moduleName,
                "Enabled",
                userId,
                config.TenantId,
                reason: reason
            );
            await _context.ModuleConfigurationHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Module {moduleName} enabled for clinic {clinicId} by user {userId}");
        }

        public async Task<ValidationResult> ValidateModuleConfigAsync(Guid clinicId, string moduleName)
        {
            // 1. Validar se módulo existe
            if (!SystemModules.GetAllModules().Contains(moduleName))
                return new ValidationResult(false, "Module not found");

            // 2. Obter informações do módulo
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);

            // 3. Verificar se é módulo core (não pode ser desabilitado)
            if (moduleInfo.IsCore)
                return new ValidationResult(false, "Core modules cannot be disabled");

            // 4. Obter plano da clínica
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId);
            if (subscription == null)
                return new ValidationResult(false, "Clinic has no active subscription");

            var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId);
            if (plan == null)
                return new ValidationResult(false, "Invalid subscription plan");

            // 5. Verificar se plano permite o módulo
            if (!plan.HasModule(moduleName))
                return new ValidationResult(false, $"Module {moduleName} not available in current plan. Please upgrade.");

            // 6. Verificar plano mínimo
            if (plan.Type < moduleInfo.MinimumPlan)
                return new ValidationResult(false, $"Module requires at least {moduleInfo.MinimumPlan} plan");

            return new ValidationResult(true);
        }

        public async Task<bool> HasRequiredModulesAsync(Guid clinicId, string moduleName)
        {
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);
            
            if (moduleInfo.RequiredModules.Length == 0)
                return true;

            foreach (var requiredModule in moduleInfo.RequiredModules)
            {
                var config = await _repository.GetByClinicAndModuleAsync(clinicId, requiredModule);
                if (config == null || !config.IsEnabled)
                    return false;
            }

            return true;
        }

        // ... implementar outros métodos
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public ValidationResult(bool isValid, string errorMessage = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
```

#### 2.2. ModuleAnalyticsService

**Criar:** `/src/MedicSoft.Application/Services/ModuleAnalyticsService.cs`

```csharp
namespace MedicSoft.Application.Services
{
    public interface IModuleAnalyticsService
    {
        Task<ModuleUsageStatsDto> GetModuleUsageStatsAsync(string moduleName);
        Task<IEnumerable<ModuleAdoptionDto>> GetModuleAdoptionRatesAsync();
        Task<IEnumerable<ModuleUsageByPlanDto>> GetUsageByPlanAsync();
        Task<Dictionary<string, int>> GetModuleCountsAsync();
    }

    public class ModuleAnalyticsService : IModuleAnalyticsService
    {
        private readonly MedicSoftDbContext _context;

        public ModuleAnalyticsService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<ModuleUsageStatsDto> GetModuleUsageStatsAsync(string moduleName)
        {
            var totalClinics = await _context.Clinics.CountAsync();
            var clinicsWithModule = await _context.ModuleConfigurations
                .Where(mc => mc.ModuleName == moduleName && mc.IsEnabled)
                .Select(mc => mc.ClinicId)
                .Distinct()
                .CountAsync();

            return new ModuleUsageStatsDto
            {
                ModuleName = moduleName,
                TotalClinics = totalClinics,
                ClinicsWithModuleEnabled = clinicsWithModule,
                AdoptionRate = totalClinics > 0 ? (decimal)clinicsWithModule / totalClinics * 100 : 0
            };
        }

        public async Task<IEnumerable<ModuleAdoptionDto>> GetModuleAdoptionRatesAsync()
        {
            var modules = SystemModules.GetAllModules();
            var result = new List<ModuleAdoptionDto>();

            foreach (var module in modules)
            {
                var stats = await GetModuleUsageStatsAsync(module);
                result.Add(new ModuleAdoptionDto
                {
                    ModuleName = module,
                    DisplayName = SystemModules.GetModuleInfo(module).DisplayName,
                    AdoptionRate = stats.AdoptionRate,
                    EnabledCount = stats.ClinicsWithModuleEnabled
                });
            }

            return result.OrderByDescending(r => r.AdoptionRate);
        }

        // ... implementar outros métodos
    }
}
```

---

### 3. Expandir Controllers da API (4-5 dias)

#### 3.1. Expandir ModuleConfigController

**Arquivo:** `/src/MedicSoft.Api/Controllers/ModuleConfigController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class ModuleConfigController : BaseController
{
    private readonly IModuleConfigurationService _service;
    private readonly IModuleAnalyticsService _analyticsService;

    // Endpoints EXISTENTES mantidos...
    
    // NOVOS ENDPOINTS:

    /// <summary>
    /// Get detailed information about all modules
    /// </summary>
    [HttpGet("info")]
    [ProducesResponseType(typeof(IEnumerable<ModuleInfoDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ModuleInfoDto>> GetModulesInfo()
    {
        var modules = SystemModules.GetModulesInfo();
        var result = modules.Values.Select(m => new ModuleInfoDto
        {
            Name = m.Name,
            DisplayName = m.DisplayName,
            Description = m.Description,
            Category = m.Category,
            Icon = m.Icon,
            IsCore = m.IsCore,
            RequiredModules = m.RequiredModules,
            MinimumPlan = m.MinimumPlan.ToString()
        });

        return Ok(result);
    }

    /// <summary>
    /// Validate if a module can be enabled for a clinic
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidationResponseDto>> ValidateModuleConfig(
        [FromBody] ValidateModuleRequest request)
    {
        var clinicId = GetClinicIdFromToken();
        var validation = await _service.ValidateModuleConfigAsync(clinicId, request.ModuleName);

        return Ok(new ValidationResponseDto
        {
            IsValid = validation.IsValid,
            ErrorMessage = validation.ErrorMessage
        });
    }

    /// <summary>
    /// Get module configuration history
    /// </summary>
    [HttpGet("{moduleName}/history")]
    [ProducesResponseType(typeof(IEnumerable<ModuleConfigHistoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ModuleConfigHistoryDto>>> GetModuleHistory(string moduleName)
    {
        var clinicId = GetClinicIdFromToken();
        var history = await _service.GetModuleHistoryAsync(clinicId, moduleName);
        return Ok(history);
    }

    /// <summary>
    /// Enable module with reason (for audit)
    /// </summary>
    [HttpPost("{moduleName}/enable-with-reason")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> EnableModuleWithReason(
        string moduleName, 
        [FromBody] EnableModuleRequest request)
    {
        var clinicId = GetClinicIdFromToken();
        var userId = User.FindFirst("sub")?.Value ?? "Unknown";
        
        await _service.EnableModuleAsync(clinicId, moduleName, userId, request.Reason);
        return Ok(new { message = $"Module {moduleName} enabled successfully" });
    }
}

// DTOs
public class ValidateModuleRequest
{
    public string ModuleName { get; set; } = string.Empty;
}

public class EnableModuleRequest
{
    public string? Reason { get; set; }
}

public class ValidationResponseDto
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ModuleInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsCore { get; set; }
    public string[] RequiredModules { get; set; } = Array.Empty<string>();
    public string MinimumPlan { get; set; } = string.Empty;
}
```

#### 3.2. Criar SystemAdminModuleController

**Criar:** `/src/MedicSoft.Api/Controllers/SystemAdmin/SystemAdminModuleController.cs`

```csharp
namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// System Admin endpoints for global module configuration
    /// </summary>
    [ApiController]
    [Route("api/system-admin/modules")]
    [Authorize(Roles = "SystemAdmin")]
    public class SystemAdminModuleController : ControllerBase
    {
        private readonly IModuleConfigurationService _service;
        private readonly IModuleAnalyticsService _analyticsService;
        private readonly MedicSoftDbContext _context;

        public SystemAdminModuleController(
            IModuleConfigurationService service,
            IModuleAnalyticsService analyticsService,
            MedicSoftDbContext context)
        {
            _service = service;
            _analyticsService = analyticsService;
            _context = context;
        }

        /// <summary>
        /// Get global module usage statistics
        /// </summary>
        [HttpGet("usage")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleUsageDto>>> GetGlobalModuleUsage()
        {
            var usage = await _service.GetGlobalModuleUsageAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Get module adoption rates across all clinics
        /// </summary>
        [HttpGet("adoption")]
        [ProducesResponseType(typeof(IEnumerable<ModuleAdoptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleAdoptionDto>>> GetModuleAdoption()
        {
            var adoption = await _analyticsService.GetModuleAdoptionRatesAsync();
            return Ok(adoption);
        }

        /// <summary>
        /// Get module usage grouped by subscription plan
        /// </summary>
        [HttpGet("usage-by-plan")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageByPlanDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleUsageByPlanDto>>> GetUsageByPlan()
        {
            var usage = await _analyticsService.GetUsageByPlanAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Enable module globally (for all clinics with appropriate plan)
        /// </summary>
        [HttpPost("{moduleName}/enable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EnableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _service.EnableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} enabled globally" });
        }

        /// <summary>
        /// Disable module globally (for all clinics)
        /// </summary>
        [HttpPost("{moduleName}/disable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DisableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _service.DisableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} disabled globally" });
        }

        /// <summary>
        /// Get all clinics with a specific module enabled
        /// </summary>
        [HttpGet("{moduleName}/clinics")]
        [ProducesResponseType(typeof(IEnumerable<ClinicModuleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicModuleDto>>> GetClinicsWithModule(string moduleName)
        {
            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ModuleName == moduleName && mc.IsEnabled)
                .Include(mc => mc.Clinic)
                .ToListAsync();

            var result = configs.Select(mc => new ClinicModuleDto
            {
                ClinicId = mc.ClinicId,
                ClinicName = mc.Clinic?.Name ?? "Unknown",
                IsEnabled = mc.IsEnabled,
                Configuration = mc.Configuration,
                UpdatedAt = mc.UpdatedAt
            });

            return Ok(result);
        }
    }
}
```

---

### 4. Criar DTOs e ViewModels (1-2 dias)

**Criar:** `/src/MedicSoft.Application/DTOs/ModuleDtos.cs`

```csharp
namespace MedicSoft.Application.DTOs
{
    public class ModuleConfigDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public bool IsAvailableInPlan { get; set; }
        public bool IsCore { get; set; }
        public string[] RequiredModules { get; set; } = Array.Empty<string>();
        public string? Configuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ModuleUsageDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int TotalClinics { get; set; }
        public int ClinicsWithModuleEnabled { get; set; }
        public decimal AdoptionRate { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class ModuleAdoptionDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public decimal AdoptionRate { get; set; }
        public int EnabledCount { get; set; }
    }

    public class ModuleUsageByPlanDto
    {
        public string PlanName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public int ClinicsCount { get; set; }
        public decimal UsagePercentage { get; set; }
    }

    public class ModuleConfigHistoryDto
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? Reason { get; set; }
        public string? PreviousConfiguration { get; set; }
        public string? NewConfiguration { get; set; }
    }

    public class ClinicModuleDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? Configuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ModuleUsageStatsDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public int TotalClinics { get; set; }
        public int ClinicsWithModuleEnabled { get; set; }
        public decimal AdoptionRate { get; set; }
    }
}
```

---

### 5. Migrations e Configurações (1 dia)

#### 5.1. Criar Migration

```bash
cd /src/MedicSoft.Repository
dotnet ef migrations add AddModuleConfigurationHistory --context MedicSoftDbContext --output-dir Migrations/PostgreSQL
```

#### 5.2. Configurar Entity Framework

**Criar:** `/src/MedicSoft.Repository/Configurations/ModuleConfigurationHistoryConfiguration.cs`

```csharp
public class ModuleConfigurationHistoryConfiguration : IEntityTypeConfiguration<ModuleConfigurationHistory>
{
    public void Configure(EntityTypeBuilder<ModuleConfigurationHistory> builder)
    {
        builder.ToTable("ModuleConfigurationHistories");
        
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.ModuleName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(h => h.Action)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(h => h.ChangedBy)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(h => h.PreviousConfiguration)
            .HasColumnType("jsonb");
            
        builder.Property(h => h.NewConfiguration)
            .HasColumnType("jsonb");
            
        builder.HasIndex(h => new { h.ClinicId, h.ModuleName });
        builder.HasIndex(h => h.ChangedAt);
    }
}
```

#### 5.3. Atualizar DbContext

**Arquivo:** `/src/MedicSoft.Repository/Context/MedicSoftDbContext.cs`

```csharp
public class MedicSoftDbContext : DbContext
{
    // DbSets existentes...
    
    // NOVO:
    public DbSet<ModuleConfigurationHistory> ModuleConfigurationHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações existentes...
        
        // NOVA:
        modelBuilder.ApplyConfiguration(new ModuleConfigurationHistoryConfiguration());
    }
}
```

---

### 6. Registrar Serviços no DI (30 min)

**Arquivo:** `/src/MedicSoft.Api/Program.cs`

```csharp
// Registrar novos serviços
builder.Services.AddScoped<IModuleConfigurationService, ModuleConfigurationService>();
builder.Services.AddScoped<IModuleAnalyticsService, ModuleAnalyticsService>();
```

---

### 7. Testes Unitários e de Integração (ver 04-PROMPT-TESTES.md)

Os testes serão detalhados no prompt específico de testes.

---

## ✅ Critérios de Sucesso

### Funcional
- ✅ Todos os endpoints da API implementados e funcionando
- ✅ Validações de permissões implementadas
- ✅ Auditoria de mudanças funcionando
- ✅ Métricas de uso calculadas corretamente

### Técnico
- ✅ Código seguindo padrões do projeto
- ✅ DTOs e ViewModels criados
- ✅ Migrations aplicadas corretamente
- ✅ Serviços registrados no DI
- ✅ Swagger documentado

### Qualidade
- ✅ Código limpo e documentado
- ✅ Tratamento de erros adequado
- ✅ Logs de auditoria implementados
- ✅ Performance otimizada

---

## 📊 Endpoints da API (Resumo)

### Módulos por Clínica
```
GET    /api/module-config                    - Listar módulos da clínica
GET    /api/module-config/info                - Informações de todos os módulos
GET    /api/module-config/available           - Módulos disponíveis
POST   /api/module-config/{moduleName}/enable - Habilitar módulo
POST   /api/module-config/{moduleName}/disable - Desabilitar módulo
PUT    /api/module-config/{moduleName}/config - Atualizar configuração
POST   /api/module-config/validate            - Validar módulo
GET    /api/module-config/{moduleName}/history - Histórico de mudanças
```

### System Admin (Global)
```
GET    /api/system-admin/modules/usage          - Uso global de módulos
GET    /api/system-admin/modules/adoption       - Taxa de adoção
GET    /api/system-admin/modules/usage-by-plan  - Uso por plano
POST   /api/system-admin/modules/{moduleName}/enable-globally
POST   /api/system-admin/modules/{moduleName}/disable-globally
GET    /api/system-admin/modules/{moduleName}/clinics
```

---

## 🔧 Ferramentas e Tecnologias

- **ASP.NET Core 8.0**
- **Entity Framework Core**
- **PostgreSQL**
- **Swagger/OpenAPI**
- **Serilog** (logging)

---

## 📚 Referências

- [Documentação ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## ⏭️ Próximos Passos

Após completar este prompt:
1. Testar todos os endpoints no Swagger
2. Validar funcionalidades com Postman
3. Executar migrations no banco
4. Prosseguir para **02-PROMPT-FRONTEND-SYSTEM-ADMIN.md**

---

> **Status:** 📝 Pronto para desenvolvimento  
> **Última Atualização:** 29 de Janeiro de 2026
