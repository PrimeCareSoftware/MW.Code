namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of clinics for public classification and filtering
    /// </summary>
    public enum ClinicType
    {
        /// <summary>
        /// General medical clinic
        /// </summary>
        Medical = 0,
        
        /// <summary>
        /// Dental clinic
        /// </summary>
        Dental = 1,
        
        /// <summary>
        /// Nutritionist clinic
        /// </summary>
        Nutritionist = 2,
        
        /// <summary>
        /// Psychology/therapy clinic
        /// </summary>
        Psychology = 3,
        
        /// <summary>
        /// Physical therapy clinic
        /// </summary>
        PhysicalTherapy = 4,
        
        /// <summary>
        /// Veterinary clinic
        /// </summary>
        Veterinary = 5,
        
        /// <summary>
        /// Other specialty clinics
        /// </summary>
        Other = 99
    }
}
