using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class AccessProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public Guid? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public Guid? ConsultationFormProfileId { get; set; }
        public string? ConsultationFormProfileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Permissions { get; set; } = new();
        public int UserCount { get; set; }
    }

    public class CreateAccessProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class UpdateAccessProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    public class PermissionDto
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PermissionCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class AssignProfileDto
    {
        public Guid UserId { get; set; }
        public Guid ProfileId { get; set; }
    }

    public class SetConsultationFormProfileDto
    {
        public Guid? ConsultationFormProfileId { get; set; }
    }
}
