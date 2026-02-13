using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for module configuration details
    /// </summary>
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
        public bool RequiresConfiguration { get; set; }
        public string? ConfigurationType { get; set; }
        public string? ConfigurationExample { get; set; }
        public string? ConfigurationHelp { get; set; }
    }

    /// <summary>
    /// DTO for module usage statistics
    /// </summary>
    public class ModuleUsageDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int TotalClinics { get; set; }
        public int ClinicsWithModuleEnabled { get; set; }
        public decimal AdoptionRate { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for module adoption rates
    /// </summary>
    public class ModuleAdoptionDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public decimal AdoptionRate { get; set; }
        public int EnabledCount { get; set; }
    }

    /// <summary>
    /// DTO for module usage by subscription plan
    /// </summary>
    public class ModuleUsageByPlanDto
    {
        public string PlanName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public int ClinicsCount { get; set; }
        public decimal UsagePercentage { get; set; }
    }

    /// <summary>
    /// DTO for module configuration history
    /// </summary>
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

    /// <summary>
    /// DTO for clinic module information
    /// </summary>
    public class ClinicModuleDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? Configuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for module usage statistics details
    /// </summary>
    public class ModuleUsageStatsDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public int TotalClinics { get; set; }
        public int ClinicsWithModuleEnabled { get; set; }
        public decimal AdoptionRate { get; set; }
    }

    /// <summary>
    /// DTO for module information
    /// </summary>
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
        public bool RequiresConfiguration { get; set; }
        public string? ConfigurationType { get; set; }
        public string? ConfigurationExample { get; set; }
        public string? ConfigurationHelp { get; set; }
    }
}
