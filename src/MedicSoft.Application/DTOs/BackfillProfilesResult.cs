using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Result of backfilling missing default profiles for clinics
    /// </summary>
    public class BackfillProfilesResult
    {
        public int ClinicsProcessed { get; set; }
        public int ProfilesCreated { get; set; }
        public int ProfilesSkipped { get; set; }
        public List<ClinicBackfillDetail> ClinicDetails { get; set; } = new();
    }

    /// <summary>
    /// Details of profile backfill for a specific clinic
    /// </summary>
    public class ClinicBackfillDetail
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public List<string> ProfilesCreated { get; set; } = new();
        public List<string> ProfilesSkipped { get; set; } = new();
    }
}
