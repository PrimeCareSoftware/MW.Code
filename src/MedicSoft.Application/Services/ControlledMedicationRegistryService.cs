using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing controlled medication registry (Livro de Registro).
    /// Implements ANVISA RDC 27/2007 requirements for tracking controlled substances.
    /// </summary>
    public class ControlledMedicationRegistryService : IControlledMedicationRegistryService
    {
        private readonly IControlledMedicationRegistryRepository _registryRepository;
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<ControlledMedicationRegistryService> _logger;

        public ControlledMedicationRegistryService(
            IControlledMedicationRegistryRepository registryRepository,
            IDigitalPrescriptionRepository prescriptionRepository,
            ILogger<ControlledMedicationRegistryService> logger)
        {
            _registryRepository = registryRepository ?? throw new ArgumentNullException(nameof(registryRepository));
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ControlledMedicationRegistry> RegisterPrescriptionAsync(
            Guid prescriptionId, 
            string tenantId, 
            Guid userId)
        {
            if (prescriptionId == Guid.Empty)
                throw new ArgumentException("Prescription ID cannot be empty", nameof(prescriptionId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            _logger.LogInformation(
                "Registering controlled medication prescription {PrescriptionId} for tenant {TenantId}",
                prescriptionId, tenantId);

            // Check if prescription is already registered
            var existingRegistry = await _registryRepository.GetByPrescriptionIdAsync(prescriptionId, tenantId);
            if (existingRegistry != null)
            {
                _logger.LogWarning(
                    "Prescription {PrescriptionId} is already registered",
                    prescriptionId);
                throw new InvalidOperationException($"Prescription {prescriptionId} is already registered");
            }

            // Get the prescription with items
            var prescription = await _prescriptionRepository.GetByIdWithItemsAsync(prescriptionId, tenantId);
            if (prescription == null)
            {
                _logger.LogError(
                    "Prescription {PrescriptionId} not found for tenant {TenantId}",
                    prescriptionId, tenantId);
                throw new InvalidOperationException($"Prescription {prescriptionId} not found");
            }

            // For simplicity, we'll assume the first item is the controlled medication
            // In a real implementation, you would iterate through items and register each controlled medication
            var firstItem = prescription.Items?.FirstOrDefault();
            if (firstItem == null)
            {
                _logger.LogError(
                    "Prescription {PrescriptionId} has no items",
                    prescriptionId);
                throw new InvalidOperationException("Prescription has no items");
            }

            // Get the medication name from the prescription item
            // Note: In a real implementation, you'd get more details from a medication catalog
            var medicationName = firstItem.MedicationName ?? "Unknown Medication";
            var quantity = firstItem.Quantity;

            // Get previous balance for this medication
            var previousBalance = await _registryRepository.GetLatestBalanceAsync(medicationName, tenantId);

            // Create the registry entry
            var registryEntry = ControlledMedicationRegistry.CreatePrescriptionEntry(
                tenantId: tenantId,
                prescriptionId: prescriptionId,
                date: DateTime.UtcNow,
                medicationName: medicationName,
                activeIngredient: medicationName, // In a real implementation, get from medication catalog
                anvisaList: "B1", // In a real implementation, get from medication catalog
                concentration: "100mg", // In a real implementation, get from medication catalog
                pharmaceuticalForm: "Comprimido", // In a real implementation, get from medication catalog
                quantity: quantity,
                previousBalance: previousBalance,
                documentNumber: prescription.SequenceNumber ?? prescriptionId.ToString(),
                documentDate: prescription.IssuedAt,
                patientName: prescription.PatientName ?? "Unknown Patient",
                patientCPF: prescription.PatientDocument ?? string.Empty,
                doctorName: prescription.DoctorName ?? "Unknown Doctor",
                doctorCRM: prescription.DoctorCRM ?? string.Empty,
                registeredByUserId: userId
            );

            await _registryRepository.AddAsync(registryEntry);

            _logger.LogInformation(
                "Successfully registered prescription {PrescriptionId}. New balance: {Balance}",
                prescriptionId, registryEntry.Balance);

            return registryEntry;
        }

        public async Task<ControlledMedicationRegistry> RegisterStockEntryAsync(
            StockEntryDto dto, 
            string tenantId, 
            Guid userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            _logger.LogInformation(
                "Registering stock entry for medication {MedicationName} (Quantity: {Quantity}) for tenant {TenantId}",
                dto.MedicationName, dto.Quantity, tenantId);

            // Validate DTO
            if (string.IsNullOrWhiteSpace(dto.MedicationName))
                throw new ArgumentException("Medication name cannot be empty", nameof(dto.MedicationName));

            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(dto.Quantity));

            // Get previous balance for this medication
            var previousBalance = await _registryRepository.GetLatestBalanceAsync(dto.MedicationName, tenantId);

            // Create the stock entry registry
            var registryEntry = ControlledMedicationRegistry.CreateStockEntry(
                tenantId: tenantId,
                date: dto.Date,
                medicationName: dto.MedicationName,
                activeIngredient: dto.ActiveIngredient,
                anvisaList: dto.AnvisaList,
                concentration: dto.Concentration,
                pharmaceuticalForm: dto.PharmaceuticalForm,
                quantity: dto.Quantity,
                previousBalance: previousBalance,
                documentType: dto.DocumentType,
                documentNumber: dto.DocumentNumber,
                documentDate: dto.DocumentDate,
                supplierName: dto.SupplierName,
                supplierCNPJ: dto.SupplierCNPJ,
                registeredByUserId: userId
            );

            await _registryRepository.AddAsync(registryEntry);

            _logger.LogInformation(
                "Successfully registered stock entry for {MedicationName}. New balance: {Balance}",
                dto.MedicationName, registryEntry.Balance);

            return registryEntry;
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetRegistryByPeriodAsync(
            DateTime startDate, 
            DateTime endDate, 
            string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (startDate > endDate)
                throw new ArgumentException("Start date must be before or equal to end date");

            _logger.LogInformation(
                "Getting registry entries from {StartDate} to {EndDate} for tenant {TenantId}",
                startDate, endDate, tenantId);

            return await _registryRepository.GetByPeriodAsync(startDate, endDate, tenantId);
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetRegistryByMedicationAsync(
            string medicationName, 
            string tenantId)
        {
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name cannot be empty", nameof(medicationName));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Getting registry entries for medication {MedicationName} for tenant {TenantId}",
                medicationName, tenantId);

            return await _registryRepository.GetByMedicationAsync(medicationName, tenantId);
        }

        public async Task<decimal> GetCurrentBalanceAsync(
            string medicationName, 
            string tenantId)
        {
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name cannot be empty", nameof(medicationName));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Getting current balance for medication {MedicationName} for tenant {TenantId}",
                medicationName, tenantId);

            var balance = await _registryRepository.GetLatestBalanceAsync(medicationName, tenantId);

            _logger.LogInformation(
                "Current balance for {MedicationName}: {Balance}",
                medicationName, balance);

            return balance;
        }
    }
}
