namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Represents the type/size of business for configuration purposes
    /// </summary>
    public enum BusinessType
    {
        /// <summary>
        /// Solo practitioner (1 professional, may not have physical office)
        /// </summary>
        SoloPractitioner = 1,
        
        /// <summary>
        /// Small clinic (2-5 professionals)
        /// </summary>
        SmallClinic = 2,
        
        /// <summary>
        /// Medium clinic (6-20 professionals)
        /// </summary>
        MediumClinic = 3,
        
        /// <summary>
        /// Large clinic (20+ professionals)
        /// </summary>
        LargeClinic = 4
    }
}
