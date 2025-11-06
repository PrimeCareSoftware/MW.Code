using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum QueueDisplayMode
    {
        InternalOnly = 1,      // Only for system users (doctors, secretaries)
        PublicDisplay = 2,     // Public screen for patients to track
        Both = 3               // Both internal and public display
    }

    /// <summary>
    /// Configuration for the waiting queue display and behavior
    /// </summary>
    public class WaitingQueueConfiguration : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public QueueDisplayMode DisplayMode { get; private set; }
        public bool ShowEstimatedWaitTime { get; private set; }
        public bool ShowPatientNames { get; private set; }
        public bool ShowPriority { get; private set; }
        public int AutoRefreshSeconds { get; private set; }
        public bool EnableSoundNotifications { get; private set; }
        public bool ShowPosition { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }

        private WaitingQueueConfiguration()
        {
            // EF Core constructor
        }

        public WaitingQueueConfiguration(
            Guid clinicId,
            string tenantId,
            QueueDisplayMode displayMode = QueueDisplayMode.InternalOnly,
            bool showEstimatedWaitTime = true,
            bool showPatientNames = true,
            bool showPriority = false,
            int autoRefreshSeconds = 30,
            bool enableSoundNotifications = true,
            bool showPosition = true) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("O ID da clínica não pode estar vazio", nameof(clinicId));

            if (autoRefreshSeconds < 5)
                throw new ArgumentException("O intervalo de atualização deve ser pelo menos 5 segundos", nameof(autoRefreshSeconds));

            ClinicId = clinicId;
            DisplayMode = displayMode;
            ShowEstimatedWaitTime = showEstimatedWaitTime;
            ShowPatientNames = showPatientNames;
            ShowPriority = showPriority;
            AutoRefreshSeconds = autoRefreshSeconds;
            EnableSoundNotifications = enableSoundNotifications;
            ShowPosition = showPosition;
        }

        public void UpdateDisplayMode(QueueDisplayMode displayMode)
        {
            DisplayMode = displayMode;
            UpdateTimestamp();
        }

        public void UpdateDisplaySettings(
            bool? showEstimatedWaitTime = null,
            bool? showPatientNames = null,
            bool? showPriority = null,
            bool? showPosition = null)
        {
            if (showEstimatedWaitTime.HasValue)
                ShowEstimatedWaitTime = showEstimatedWaitTime.Value;

            if (showPatientNames.HasValue)
                ShowPatientNames = showPatientNames.Value;

            if (showPriority.HasValue)
                ShowPriority = showPriority.Value;

            if (showPosition.HasValue)
                ShowPosition = showPosition.Value;

            UpdateTimestamp();
        }

        public void UpdateAutoRefresh(int seconds)
        {
            if (seconds < 5)
                throw new ArgumentException("O intervalo de atualização deve ser pelo menos 5 segundos", nameof(seconds));

            AutoRefreshSeconds = seconds;
            UpdateTimestamp();
        }

        public void ToggleSoundNotifications()
        {
            EnableSoundNotifications = !EnableSoundNotifications;
            UpdateTimestamp();
        }

        public bool IsPublicDisplayEnabled()
        {
            return DisplayMode == QueueDisplayMode.PublicDisplay || DisplayMode == QueueDisplayMode.Both;
        }

        public bool IsInternalDisplayEnabled()
        {
            return DisplayMode == QueueDisplayMode.InternalOnly || DisplayMode == QueueDisplayMode.Both;
        }
    }
}
