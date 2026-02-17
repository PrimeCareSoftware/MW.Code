using MedicSoft.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Helpers
{
    /// <summary>
    /// Helper class for mapping AccessProfile names to UserRole enum values
    /// Provides backward compatibility between profile-based and role-based systems
    /// </summary>
    public static class ProfileMappingHelper
    {
        /// <summary>
        /// Collection of known profile name mappings
        /// Used for validation and ensuring consistency
        /// </summary>
        private static readonly HashSet<string> KnownProfileNames = new(StringComparer.OrdinalIgnoreCase)
        {
            // Owner/Proprietário
            "proprietário", "proprietario", "owner",
            
            // Medical Doctor/Médico
            "médico", "medico", "doctor",
            
            // Dentist/Dentista
            "dentista", "dentist",
            
            // Nurse/Enfermeiro
            "enfermeiro", "enfermeira", "nurse",
            
            // Receptionist/Recepção
            "recepção", "recepcao", "recepcionista", "receptionist",
            
            // Secretary/Secretaria
            "secretaria", "secretário", "secretario", "secretary",
            "recepção/secretaria", "recepcao/secretaria",
            
            // Financial
            "financeiro", "financial",
            
            // Healthcare Specialties
            "nutricionista", "nutritionist",
            "psicólogo", "psicologo", "psychologist",
            "fisioterapeuta", "physiotherapist",
            "terapeuta ocupacional", "occupational therapist",
            "fonoaudiólogo", "fonoaudiologo", "speech therapist",
            "veterinário", "veterinario", "veterinarian"
        };

        /// <summary>
        /// Maps an AccessProfile name to a UserRole enum for backward compatibility
        /// Supports both Portuguese and English profile names
        /// </summary>
        /// <param name="profileName">The name of the AccessProfile</param>
        /// <param name="logger">Optional logger for warnings about unrecognized profiles</param>
        /// <returns>The corresponding UserRole enum value</returns>
        public static UserRole MapProfileNameToRole(string profileName, ILogger? logger = null)
        {
            if (string.IsNullOrWhiteSpace(profileName))
            {
                logger?.LogWarning("Empty profile name provided, falling back to Receptionist role");
                return UserRole.Receptionist;
            }

            // Try direct enum parsing first (case insensitive)
            if (Enum.TryParse<UserRole>(profileName, true, out var directRole))
            {
                return directRole;
            }

            // Map Portuguese and English profile names to UserRole enum
            var profileNameLower = profileName.ToLowerInvariant();
            
            var mappedRole = profileNameLower switch
            {
                // Owner/Proprietário
                "proprietário" or "proprietario" or "owner" => UserRole.ClinicOwner,
                
                // Medical Doctor/Médico
                "médico" or "medico" or "doctor" => UserRole.Doctor,
                
                // Dentist/Dentista
                "dentista" or "dentist" => UserRole.Dentist,
                
                // Nurse/Enfermeiro
                "enfermeiro" or "enfermeira" or "nurse" => UserRole.Nurse,
                
                // Receptionist/Recepção
                "recepção" or "recepcao" or "recepcionista" or "receptionist" => UserRole.Receptionist,
                
                // Secretary/Secretaria
                "secretaria" or "secretário" or "secretario" or "secretary" => UserRole.Secretary,
                "recepção/secretaria" or "recepcao/secretaria" => UserRole.Secretary,
                
                // Financial - Maps to Secretary as there's no dedicated Financial role yet
                // This allows financial staff to have secretary-level permissions for managing payments
                "financeiro" or "financial" => UserRole.Secretary,
                
                // Healthcare Specialties - All map to Doctor role
                // Note: These specialties (nutritionist, psychologist, physiotherapist, etc.) 
                // currently map to the Doctor role because:
                // 1. They are all healthcare professionals who need similar base permissions
                // 2. They require access to patient records, appointments, and medical documentation
                // 3. The granular permission system (AccessProfile) provides specialty-specific access control
                // 4. Creating separate roles for each specialty would complicate the core role-based system
                // Future: Consider adding specialized roles if distinct permission sets are needed
                "nutricionista" or "nutritionist" => UserRole.Doctor,
                "psicólogo" or "psicologo" or "psychologist" => UserRole.Doctor,
                "fisioterapeuta" or "physiotherapist" => UserRole.Doctor,
                "terapeuta ocupacional" or "occupational therapist" => UserRole.Doctor,
                "fonoaudiólogo" or "fonoaudiologo" or "speech therapist" => UserRole.Doctor,
                "veterinário" or "veterinario" or "veterinarian" => UserRole.Doctor,
                
                // Default fallback - log warning for unrecognized profiles
                _ => UserRole.Receptionist
            };

            // Log warning if profile name is not in our known list
            if (!IsRecognizedProfile(profileName))
            {
                logger?.LogWarning(
                    "Unrecognized profile name '{ProfileName}' mapped to {Role} role. " +
                    "Consider adding explicit mapping for this profile.", 
                    profileName, mappedRole);
            }

            return mappedRole;
        }

        /// <summary>
        /// Checks if a profile name is recognized by the mapping system
        /// </summary>
        public static bool IsRecognizedProfile(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
                return false;

            // If it parses directly to a UserRole, it's recognized
            if (Enum.TryParse<UserRole>(profileName, true, out _))
                return true;

            // Check if it's in our known mappings
            return KnownProfileNames.Contains(profileName);
        }
    }
}
