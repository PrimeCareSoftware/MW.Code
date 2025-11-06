using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IWaitingQueueService
    {
        Task<WaitingQueueSummaryDto> GetQueueSummaryAsync(Guid clinicId, string tenantId);
        Task<List<PublicQueueDisplayDto>> GetPublicQueueDisplayAsync(Guid clinicId, string tenantId);
        Task<WaitingQueueEntryDto> AddToQueueAsync(CreateWaitingQueueEntryDto dto, string tenantId);
        Task<WaitingQueueEntryDto> UpdateTriageAsync(Guid entryId, UpdateQueueTriageDto dto, string tenantId);
        Task<WaitingQueueEntryDto> CallPatientAsync(Guid entryId, string tenantId);
        Task<WaitingQueueEntryDto> StartServiceAsync(Guid entryId, string tenantId);
        Task<WaitingQueueEntryDto> CompleteServiceAsync(Guid entryId, string tenantId);
        Task CancelEntryAsync(Guid entryId, string tenantId);
        Task<WaitingQueueConfigurationDto?> GetConfigurationAsync(Guid clinicId, string tenantId);
        Task<WaitingQueueConfigurationDto> UpdateConfigurationAsync(Guid clinicId, UpdateWaitingQueueConfigurationDto dto, string tenantId);
    }

    public class WaitingQueueService : IWaitingQueueService
    {
        private readonly IWaitingQueueRepository _queueRepository;
        private readonly IWaitingQueueConfigurationRepository _configRepository;
        private readonly IPatientRepository _patientRepository;
        private const int AverageServiceTimeMinutes = 15;

        public WaitingQueueService(
            IWaitingQueueRepository queueRepository,
            IWaitingQueueConfigurationRepository configRepository,
            IPatientRepository patientRepository)
        {
            _queueRepository = queueRepository;
            _configRepository = configRepository;
            _patientRepository = patientRepository;
        }

        public async Task<WaitingQueueSummaryDto> GetQueueSummaryAsync(Guid clinicId, string tenantId)
        {
            var entries = await _queueRepository.GetActiveEntriesByClinicAsync(clinicId, tenantId);
            
            // Recalculate positions based on priority and check-in time
            var sortedEntries = entries
                .Where(e => e.Status == QueueStatus.Waiting)
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.CheckInTime)
                .ToList();

            // Update positions
            for (int i = 0; i < sortedEntries.Count; i++)
            {
                sortedEntries[i].UpdatePosition(i + 1);
                
                // Estimate wait time based on position and average service time
                var estimatedWaitTime = i * AverageServiceTimeMinutes;
                sortedEntries[i].UpdateEstimatedWaitTime(estimatedWaitTime);
            }

            await _queueRepository.SaveChangesAsync();

            var allEntries = await _queueRepository.GetActiveEntriesByClinicAsync(clinicId, tenantId);
            
            var waitingEntries = allEntries.Where(e => e.Status == QueueStatus.Waiting).ToList();
            var averageWaitTime = waitingEntries.Any() 
                ? (int)waitingEntries.Average(e => e.GetWaitingTime().TotalMinutes)
                : 0;

            // Map entries to DTOs (patient names are already loaded via Include in repository)
            var entryDtos = allEntries
                .OrderBy(e => e.Status == QueueStatus.Waiting ? e.Position : int.MaxValue)
                .ThenBy(e => e.CheckInTime)
                .Select(e => new WaitingQueueEntryDto
                {
                    Id = e.Id,
                    AppointmentId = e.AppointmentId,
                    ClinicId = e.ClinicId,
                    PatientId = e.PatientId,
                    PatientName = e.Patient?.Name ?? "Desconhecido",
                    Position = e.Position,
                    Priority = e.Priority.ToString(),
                    Status = e.Status.ToString(),
                    CheckInTime = e.CheckInTime,
                    CalledTime = e.CalledTime,
                    CompletedTime = e.CompletedTime,
                    TriageNotes = e.TriageNotes,
                    EstimatedWaitTimeMinutes = e.EstimatedWaitTimeMinutes,
                    ActualWaitTimeMinutes = (int)e.GetWaitingTime().TotalMinutes,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToList();

            return new WaitingQueueSummaryDto
            {
                ClinicId = clinicId,
                TotalWaiting = allEntries.Count(e => e.Status == QueueStatus.Waiting),
                TotalCalled = allEntries.Count(e => e.Status == QueueStatus.Called),
                TotalInProgress = allEntries.Count(e => e.Status == QueueStatus.InProgress),
                AverageWaitTimeMinutes = averageWaitTime,
                Entries = entryDtos
            };
        }

        public async Task<List<PublicQueueDisplayDto>> GetPublicQueueDisplayAsync(Guid clinicId, string tenantId)
        {
            var config = await _configRepository.GetByClinicIdAsync(clinicId, tenantId);
            
            if (config == null || !config.IsPublicDisplayEnabled())
            {
                throw new InvalidOperationException("A exibição pública da fila não está habilitada para esta clínica");
            }

            var entries = await _queueRepository.GetActiveEntriesByClinicAsync(clinicId, tenantId);
            
            var waitingEntries = entries
                .Where(e => e.Status == QueueStatus.Waiting || e.Status == QueueStatus.Called)
                .OrderBy(e => e.Position)
                .ToList();

            return waitingEntries.Select(e => new PublicQueueDisplayDto
            {
                Position = e.Position,
                PatientIdentifier = config.ShowPatientNames ? (e.Patient?.Name ?? $"Paciente {e.Position}") : $"Paciente {e.Position}",
                Status = e.Status.ToString(),
                EstimatedWaitTimeMinutes = config.ShowEstimatedWaitTime ? e.EstimatedWaitTimeMinutes : null
            }).ToList();
        }

        public async Task<WaitingQueueEntryDto> AddToQueueAsync(CreateWaitingQueueEntryDto dto, string tenantId)
        {
            if (!Enum.TryParse<TriagePriority>(dto.Priority, out var priority))
                throw new ArgumentException("Prioridade de triagem inválida");

            var entry = new WaitingQueueEntry(
                dto.AppointmentId,
                dto.ClinicId,
                dto.PatientId,
                priority,
                tenantId,
                dto.TriageNotes
            );

            await _queueRepository.AddAsync(entry);
            await _queueRepository.SaveChangesAsync();

            // Recalculate positions for all waiting entries
            await RecalculatePositionsAsync(dto.ClinicId, tenantId);

            // Reload entry with patient data
            entry = await _queueRepository.GetByIdAsync(entry.Id, tenantId);

            return new WaitingQueueEntryDto
            {
                Id = entry!.Id,
                AppointmentId = entry.AppointmentId,
                ClinicId = entry.ClinicId,
                PatientId = entry.PatientId,
                PatientName = entry.Patient?.Name ?? "Desconhecido",
                Position = entry.Position,
                Priority = entry.Priority.ToString(),
                Status = entry.Status.ToString(),
                CheckInTime = entry.CheckInTime,
                CalledTime = entry.CalledTime,
                CompletedTime = entry.CompletedTime,
                TriageNotes = entry.TriageNotes,
                EstimatedWaitTimeMinutes = entry.EstimatedWaitTimeMinutes,
                ActualWaitTimeMinutes = (int)entry.GetWaitingTime().TotalMinutes,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }

        public async Task<WaitingQueueEntryDto> UpdateTriageAsync(Guid entryId, UpdateQueueTriageDto dto, string tenantId)
        {
            var entry = await _queueRepository.GetByIdAsync(entryId, tenantId);
            if (entry == null)
                throw new InvalidOperationException("Entrada da fila não encontrada");

            if (!Enum.TryParse<TriagePriority>(dto.Priority, out var priority))
                throw new ArgumentException("Prioridade de triagem inválida");

            entry.UpdatePriority(priority, dto.TriageNotes);
            await _queueRepository.UpdateAsync(entry);
            await _queueRepository.SaveChangesAsync();

            // Recalculate positions after priority change
            await RecalculatePositionsAsync(entry.ClinicId, tenantId);

            return MapEntryToDto(entry);
        }

        public async Task<WaitingQueueEntryDto> CallPatientAsync(Guid entryId, string tenantId)
        {
            var entry = await _queueRepository.GetByIdAsync(entryId, tenantId);
            if (entry == null)
                throw new InvalidOperationException("Entrada da fila não encontrada");

            entry.Call();
            await _queueRepository.UpdateAsync(entry);
            await _queueRepository.SaveChangesAsync();

            return MapEntryToDto(entry);
        }

        public async Task<WaitingQueueEntryDto> StartServiceAsync(Guid entryId, string tenantId)
        {
            var entry = await _queueRepository.GetByIdAsync(entryId, tenantId);
            if (entry == null)
                throw new InvalidOperationException("Entrada da fila não encontrada");

            entry.StartService();
            await _queueRepository.UpdateAsync(entry);
            await _queueRepository.SaveChangesAsync();

            // Recalculate positions for remaining waiting entries
            await RecalculatePositionsAsync(entry.ClinicId, tenantId);

            return MapEntryToDto(entry);
        }

        public async Task<WaitingQueueEntryDto> CompleteServiceAsync(Guid entryId, string tenantId)
        {
            var entry = await _queueRepository.GetByIdAsync(entryId, tenantId);
            if (entry == null)
                throw new InvalidOperationException("Entrada da fila não encontrada");

            entry.Complete();
            await _queueRepository.UpdateAsync(entry);
            await _queueRepository.SaveChangesAsync();

            return MapEntryToDto(entry);
        }

        public async Task CancelEntryAsync(Guid entryId, string tenantId)
        {
            var entry = await _queueRepository.GetByIdAsync(entryId, tenantId);
            if (entry == null)
                throw new InvalidOperationException("Entrada da fila não encontrada");

            entry.Cancel();
            await _queueRepository.UpdateAsync(entry);
            await _queueRepository.SaveChangesAsync();

            // Recalculate positions after cancellation
            await RecalculatePositionsAsync(entry.ClinicId, tenantId);
        }

        public async Task<WaitingQueueConfigurationDto?> GetConfigurationAsync(Guid clinicId, string tenantId)
        {
            var config = await _configRepository.GetByClinicIdAsync(clinicId, tenantId);
            return config != null ? MapConfigToDto(config) : null;
        }

        public async Task<WaitingQueueConfigurationDto> UpdateConfigurationAsync(Guid clinicId, UpdateWaitingQueueConfigurationDto dto, string tenantId)
        {
            var config = await _configRepository.GetByClinicIdAsync(clinicId, tenantId);

            if (!Enum.TryParse<QueueDisplayMode>(dto.DisplayMode, out var displayMode))
                throw new ArgumentException("Modo de exibição inválido");

            if (config == null)
            {
                // Create new configuration
                config = new WaitingQueueConfiguration(
                    clinicId,
                    tenantId,
                    displayMode,
                    dto.ShowEstimatedWaitTime,
                    dto.ShowPatientNames,
                    dto.ShowPriority,
                    dto.AutoRefreshSeconds,
                    dto.EnableSoundNotifications,
                    dto.ShowPosition
                );
                await _configRepository.AddAsync(config);
            }
            else
            {
                // Update existing configuration
                config.UpdateDisplayMode(displayMode);
                config.UpdateDisplaySettings(
                    dto.ShowEstimatedWaitTime,
                    dto.ShowPatientNames,
                    dto.ShowPriority,
                    dto.ShowPosition
                );
                config.UpdateAutoRefresh(dto.AutoRefreshSeconds);
                
                if (config.EnableSoundNotifications != dto.EnableSoundNotifications)
                    config.ToggleSoundNotifications();

                await _configRepository.UpdateAsync(config);
            }

            await _configRepository.SaveChangesAsync();

            return MapConfigToDto(config);
        }

        private async Task RecalculatePositionsAsync(Guid clinicId, string tenantId)
        {
            var entries = await _queueRepository.GetActiveEntriesByClinicAsync(clinicId, tenantId);
            
            var waitingEntries = entries
                .Where(e => e.Status == QueueStatus.Waiting)
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.CheckInTime)
                .ToList();

            for (int i = 0; i < waitingEntries.Count; i++)
            {
                waitingEntries[i].UpdatePosition(i + 1);
                var estimatedWaitTime = i * AverageServiceTimeMinutes;
                waitingEntries[i].UpdateEstimatedWaitTime(estimatedWaitTime);
            }

            await _queueRepository.SaveChangesAsync();
        }

        private static WaitingQueueEntryDto MapEntryToDto(WaitingQueueEntry entry)
        {
            return new WaitingQueueEntryDto
            {
                Id = entry.Id,
                AppointmentId = entry.AppointmentId,
                ClinicId = entry.ClinicId,
                PatientId = entry.PatientId,
                PatientName = entry.Patient?.Name ?? "Desconhecido",
                Position = entry.Position,
                Priority = entry.Priority.ToString(),
                Status = entry.Status.ToString(),
                CheckInTime = entry.CheckInTime,
                CalledTime = entry.CalledTime,
                CompletedTime = entry.CompletedTime,
                TriageNotes = entry.TriageNotes,
                EstimatedWaitTimeMinutes = entry.EstimatedWaitTimeMinutes,
                ActualWaitTimeMinutes = (int)entry.GetWaitingTime().TotalMinutes,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }

        private static WaitingQueueConfigurationDto MapConfigToDto(WaitingQueueConfiguration config)
        {
            return new WaitingQueueConfigurationDto
            {
                Id = config.Id,
                ClinicId = config.ClinicId,
                DisplayMode = config.DisplayMode.ToString(),
                ShowEstimatedWaitTime = config.ShowEstimatedWaitTime,
                ShowPatientNames = config.ShowPatientNames,
                ShowPriority = config.ShowPriority,
                ShowPosition = config.ShowPosition,
                AutoRefreshSeconds = config.AutoRefreshSeconds,
                EnableSoundNotifications = config.EnableSoundNotifications
            };
        }
    }
}
