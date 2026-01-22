using System;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Configuration for doctor-specific fields (ProfessionalId/CRM and Specialty)
    /// Used to define if these fields are required when creating/editing users with Doctor role
    /// </summary>
    public class DoctorFieldsConfiguration
    {
        public bool ProfessionalIdRequired { get; set; }
        public bool SpecialtyRequired { get; set; }

        public DoctorFieldsConfiguration()
        {
            // Default: both fields optional
            ProfessionalIdRequired = false;
            SpecialtyRequired = false;
        }

        public DoctorFieldsConfiguration(bool professionalIdRequired, bool specialtyRequired)
        {
            ProfessionalIdRequired = professionalIdRequired;
            SpecialtyRequired = specialtyRequired;
        }
    }
}
