namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Represents the specialty of a healthcare professional for consultation form configuration
    /// </summary>
    public enum ProfessionalSpecialty
    {
        /// <summary>
        /// General medical doctor
        /// </summary>
        Medico = 1,
        
        /// <summary>
        /// Psychologist
        /// </summary>
        Psicologo = 2,
        
        /// <summary>
        /// Nutritionist
        /// </summary>
        Nutricionista = 3,
        
        /// <summary>
        /// Physiotherapist
        /// </summary>
        Fisioterapeuta = 4,
        
        /// <summary>
        /// Dentist
        /// </summary>
        Dentista = 5,
        
        /// <summary>
        /// Nurse
        /// </summary>
        Enfermeiro = 6,
        
        /// <summary>
        /// Occupational therapist
        /// </summary>
        TerapeutaOcupacional = 7,
        
        /// <summary>
        /// Speech therapist
        /// </summary>
        Fonoaudiologo = 8,
        
        /// <summary>
        /// Veterinarian
        /// </summary>
        Veterinario = 9,
        
        /// <summary>
        /// Custom/Other specialty
        /// </summary>
        Outro = 99
    }
}
