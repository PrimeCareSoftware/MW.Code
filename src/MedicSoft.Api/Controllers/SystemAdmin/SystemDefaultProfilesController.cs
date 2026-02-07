using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for managing system-wide default profile templates
    /// Only accessible by system administrators
    /// </summary>
    [ApiController]
    [Route("api/system-admin/default-profiles")]
    [Authorize(Roles = "SystemAdmin")]
    public class SystemDefaultProfilesController : BaseController
    {
        private readonly IAccessProfileService _profileService;

        public SystemDefaultProfilesController(
            ITenantContext tenantContext,
            IAccessProfileService profileService) : base(tenantContext)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Get all default profile templates available in the system
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<DefaultProfileTemplateDto>>> GetDefaultTemplates()
        {
            try
            {
                var templates = new List<DefaultProfileTemplateDto>();

                // Get all clinic types
                foreach (ClinicType clinicType in Enum.GetValues(typeof(ClinicType)))
                {
                    // Create temporary profiles to extract their structure
                    var profilesForType = AccessProfile.GetDefaultProfilesForClinicType("system", Guid.Empty, clinicType);
                    
                    foreach (var profile in profilesForType)
                    {
                        // Check if template already exists (avoid duplicates across clinic types)
                        if (!templates.Any(t => t.Name == profile.Name))
                        {
                            templates.Add(new DefaultProfileTemplateDto
                            {
                                Name = profile.Name,
                                Description = profile.Description,
                                Permissions = profile.GetPermissionKeys().ToList(),
                                ApplicableClinicTypes = new List<string> { clinicType.ToString() }
                            });
                        }
                        else
                        {
                            // Add clinic type to existing template
                            var existing = templates.First(t => t.Name == profile.Name);
                            if (!existing.ApplicableClinicTypes.Contains(clinicType.ToString()))
                            {
                                existing.ApplicableClinicTypes.Add(clinicType.ToString());
                            }
                        }
                    }
                }

                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get default profiles for a specific clinic type
        /// </summary>
        [HttpGet("templates/clinic-type/{clinicType}")]
        public ActionResult<IEnumerable<DefaultProfileTemplateDto>> GetTemplatesByClinicType(ClinicType clinicType)
        {
            try
            {
                var profiles = AccessProfile.GetDefaultProfilesForClinicType("system", Guid.Empty, clinicType);
                
                var templates = profiles.Select(p => new DefaultProfileTemplateDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    Permissions = p.GetPermissionKeys().ToList(),
                    ApplicableClinicTypes = new List<string> { clinicType.ToString() }
                }).ToList();

                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all available clinic types
        /// </summary>
        [HttpGet("clinic-types")]
        public ActionResult<IEnumerable<ClinicTypeDto>> GetClinicTypes()
        {
            var clinicTypes = Enum.GetValues(typeof(ClinicType))
                .Cast<ClinicType>()
                .Select(ct => new ClinicTypeDto
                {
                    Value = ct.ToString(),
                    Name = GetClinicTypeName(ct)
                })
                .ToList();

            return Ok(clinicTypes);
        }

        /// <summary>
        /// Get all available permissions
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<PermissionCategoryDto>>> GetAllPermissions()
        {
            try
            {
                var permissions = await _profileService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetClinicTypeName(ClinicType clinicType)
        {
            return clinicType switch
            {
                ClinicType.Medical => "Médica",
                ClinicType.Dental => "Odontológica",
                ClinicType.Nutritionist => "Nutrição",
                ClinicType.Psychology => "Psicologia",
                ClinicType.PhysicalTherapy => "Fisioterapia",
                ClinicType.Veterinary => "Veterinária",
                ClinicType.Other => "Outra",
                _ => clinicType.ToString()
            };
        }
    }

    /// <summary>
    /// DTO for default profile templates
    /// </summary>
    public class DefaultProfileTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public List<string> ApplicableClinicTypes { get; set; } = new();
    }

    /// <summary>
    /// DTO for clinic types
    /// </summary>
    public class ClinicTypeDto
    {
        public string Value { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
