using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing patient health insurance links
    /// </summary>
    public class PatientHealthInsuranceService : IPatientHealthInsuranceService
    {
        private readonly IPatientHealthInsuranceRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly IHealthInsurancePlanRepository _planRepository;
        private readonly IMapper _mapper;

        public PatientHealthInsuranceService(
            IPatientHealthInsuranceRepository repository,
            IPatientRepository patientRepository,
            IHealthInsurancePlanRepository planRepository,
            IMapper mapper)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _planRepository = planRepository;
            _mapper = mapper;
        }

        public async Task<PatientHealthInsuranceDto> CreateAsync(CreatePatientHealthInsuranceDto dto, string tenantId)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId, tenantId);
            if (patient == null)
            {
                throw new InvalidOperationException($"Patient with ID {dto.PatientId} not found");
            }

            // Validate plan exists
            var plan = await _planRepository.GetByIdAsync(dto.HealthInsurancePlanId, tenantId);
            if (plan == null)
            {
                throw new InvalidOperationException($"Health insurance plan with ID {dto.HealthInsurancePlanId} not found");
            }

            // Check if card number already exists
            var existingCard = await _repository.GetByCardNumberAsync(dto.CardNumber, tenantId);
            if (existingCard != null)
            {
                throw new InvalidOperationException($"Card number {dto.CardNumber} is already registered");
            }

            var insurance = new PatientHealthInsurance(
                dto.PatientId,
                dto.HealthInsurancePlanId,
                dto.CardNumber,
                dto.ValidFrom,
                tenantId,
                dto.IsHolder,
                dto.CardValidationCode,
                dto.ValidUntil,
                dto.HolderName,
                dto.HolderDocument
            );

            await _repository.AddAsync(insurance);
            await _repository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _repository.GetByIdWithDetailsAsync(insurance.Id, tenantId);
            return _mapper.Map<PatientHealthInsuranceDto>(result);
        }

        public async Task<PatientHealthInsuranceDto> UpdateAsync(Guid id, UpdatePatientHealthInsuranceDto dto, string tenantId)
        {
            var insurance = await _repository.GetByIdAsync(id, tenantId);
            if (insurance == null)
            {
                throw new InvalidOperationException($"Patient health insurance with ID {id} not found");
            }

            // Check if new card number already exists (excluding current)
            var existingCard = await _repository.GetByCardNumberAsync(dto.CardNumber, tenantId);
            if (existingCard != null && existingCard.Id != id)
            {
                throw new InvalidOperationException($"Card number {dto.CardNumber} is already registered");
            }

            insurance.UpdateCardInfo(dto.CardNumber, dto.CardValidationCode);
            insurance.UpdateValidityPeriod(dto.ValidFrom, dto.ValidUntil);
            
            if (!dto.IsHolder && !string.IsNullOrWhiteSpace(dto.HolderName) && !string.IsNullOrWhiteSpace(dto.HolderDocument))
            {
                insurance.UpdateHolderInfo(dto.IsHolder, dto.HolderName, dto.HolderDocument);
            }

            _repository.UpdateAsync(insurance);
            await _repository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<PatientHealthInsuranceDto>(result);
        }

        public async Task<IEnumerable<PatientHealthInsuranceDto>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            var insurances = await _repository.GetByPatientIdAsync(patientId, tenantId);
            return _mapper.Map<IEnumerable<PatientHealthInsuranceDto>>(insurances);
        }

        public async Task<PatientHealthInsuranceDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var insurance = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return insurance != null ? _mapper.Map<PatientHealthInsuranceDto>(insurance) : null;
        }

        public async Task<PatientHealthInsuranceDto?> GetByCardNumberAsync(string cardNumber, string tenantId)
        {
            var insurance = await _repository.GetByCardNumberAsync(cardNumber, tenantId);
            return insurance != null ? _mapper.Map<PatientHealthInsuranceDto>(insurance) : null;
        }

        public async Task<IEnumerable<PatientHealthInsuranceDto>> GetActiveByPatientIdAsync(Guid patientId, string tenantId)
        {
            var insurances = await _repository.GetByPatientIdAsync(patientId, tenantId);
            var activeInsurances = insurances.Where(i => i.IsActive && i.IsValid());
            return _mapper.Map<IEnumerable<PatientHealthInsuranceDto>>(activeInsurances);
        }

        public async Task<CardValidationResultDto> ValidateCardAsync(string cardNumber, string tenantId)
        {
            var insurance = await _repository.GetByCardNumberAsync(cardNumber, tenantId);
            
            if (insurance == null)
            {
                return new CardValidationResultDto
                {
                    IsValid = false,
                    Message = "Card number not found"
                };
            }

            if (!insurance.IsActive)
            {
                return new CardValidationResultDto
                {
                    IsValid = false,
                    Message = "Card is inactive",
                    Insurance = _mapper.Map<PatientHealthInsuranceDto>(insurance)
                };
            }

            if (!insurance.IsValid())
            {
                return new CardValidationResultDto
                {
                    IsValid = false,
                    Message = "Card has expired or is not yet valid",
                    Insurance = _mapper.Map<PatientHealthInsuranceDto>(insurance)
                };
            }

            return new CardValidationResultDto
            {
                IsValid = true,
                Message = "Card is valid",
                Insurance = _mapper.Map<PatientHealthInsuranceDto>(insurance)
            };
        }

        public async Task<PatientHealthInsuranceDto> ActivateAsync(Guid id, string tenantId)
        {
            var insurance = await _repository.GetByIdAsync(id, tenantId);
            if (insurance == null)
            {
                throw new InvalidOperationException($"Patient health insurance with ID {id} not found");
            }

            insurance.Activate();
            _repository.UpdateAsync(insurance);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<PatientHealthInsuranceDto>(result);
        }

        public async Task<PatientHealthInsuranceDto> DeactivateAsync(Guid id, string tenantId)
        {
            var insurance = await _repository.GetByIdAsync(id, tenantId);
            if (insurance == null)
            {
                throw new InvalidOperationException($"Patient health insurance with ID {id} not found");
            }

            insurance.Deactivate();
            _repository.UpdateAsync(insurance);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<PatientHealthInsuranceDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var insurance = await _repository.GetByIdAsync(id, tenantId);
            if (insurance == null)
            {
                return false;
            }

            insurance.Deactivate();
            _repository.UpdateAsync(insurance);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
