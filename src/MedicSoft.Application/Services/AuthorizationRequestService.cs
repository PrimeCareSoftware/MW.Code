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
    /// Service implementation for managing authorization requests
    /// </summary>
    public class AuthorizationRequestService : IAuthorizationRequestService
    {
        private readonly IAuthorizationRequestRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientHealthInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public AuthorizationRequestService(
            IAuthorizationRequestRepository repository,
            IPatientRepository patientRepository,
            IPatientHealthInsuranceRepository insuranceRepository,
            IMapper mapper)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<AuthorizationRequestDto> CreateAsync(CreateAuthorizationRequestDto dto, string tenantId)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId, tenantId);
            if (patient == null)
            {
                throw new InvalidOperationException($"Patient with ID {dto.PatientId} not found");
            }

            // Validate patient health insurance exists and is valid
            var insurance = await _insuranceRepository.GetByIdAsync(dto.PatientHealthInsuranceId, tenantId);
            if (insurance == null)
            {
                throw new InvalidOperationException($"Patient health insurance with ID {dto.PatientHealthInsuranceId} not found");
            }

            if (!insurance.IsValid())
            {
                throw new InvalidOperationException("Patient health insurance is not valid");
            }

            // Generate request number (could be improved with a sequence generator)
            var requestNumber = $"AUTH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var request = new AuthorizationRequest(
                dto.PatientId,
                dto.PatientHealthInsuranceId,
                requestNumber,
                dto.ProcedureCode,
                dto.ProcedureDescription,
                dto.Quantity,
                tenantId,
                dto.AppointmentId,
                dto.ClinicalIndication,
                dto.Diagnosis
            );

            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _repository.GetByIdWithDetailsAsync(request.Id, tenantId);
            return _mapper.Map<AuthorizationRequestDto>(result);
        }

        public async Task<AuthorizationRequestDto> ApproveAsync(Guid id, ApproveAuthorizationDto dto, string tenantId)
        {
            var request = await _repository.GetByIdAsync(id, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Authorization request with ID {id} not found");
            }

            request.Approve(dto.AuthorizationNumber, dto.ExpirationDate);

            _repository.UpdateAsync(request);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<AuthorizationRequestDto>(result);
        }

        public async Task<AuthorizationRequestDto> DenyAsync(Guid id, DenyAuthorizationDto dto, string tenantId)
        {
            var request = await _repository.GetByIdAsync(id, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Authorization request with ID {id} not found");
            }

            request.Deny(dto.DenialReason);

            _repository.UpdateAsync(request);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<AuthorizationRequestDto>(result);
        }

        public async Task<AuthorizationRequestDto> CancelAsync(Guid id, string tenantId)
        {
            var request = await _repository.GetByIdAsync(id, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Authorization request with ID {id} not found");
            }

            request.Cancel();

            _repository.UpdateAsync(request);
            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return _mapper.Map<AuthorizationRequestDto>(result);
        }

        public async Task<IEnumerable<AuthorizationRequestDto>> GetAllAsync(string tenantId)
        {
            var requests = await _repository.GetAllWithDetailsAsync(tenantId);
            return _mapper.Map<IEnumerable<AuthorizationRequestDto>>(requests);
        }

        public async Task<AuthorizationRequestDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var request = await _repository.GetByIdWithDetailsAsync(id, tenantId);
            return request != null ? _mapper.Map<AuthorizationRequestDto>(request) : null;
        }

        public async Task<IEnumerable<AuthorizationRequestDto>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            var requests = await _repository.GetByPatientIdAsync(patientId, tenantId);
            return _mapper.Map<IEnumerable<AuthorizationRequestDto>>(requests);
        }

        public async Task<IEnumerable<AuthorizationRequestDto>> GetByStatusAsync(string status, string tenantId)
        {
            if (!Enum.TryParse<AuthorizationStatus>(status, true, out var statusEnum))
            {
                throw new ArgumentException($"Invalid status: {status}");
            }

            var requests = await _repository.GetByStatusAsync(statusEnum, tenantId);
            return _mapper.Map<IEnumerable<AuthorizationRequestDto>>(requests);
        }

        public async Task<IEnumerable<AuthorizationRequestDto>> GetPendingAsync(string tenantId)
        {
            var requests = await _repository.GetByStatusAsync(AuthorizationStatus.Pending, tenantId);
            return _mapper.Map<IEnumerable<AuthorizationRequestDto>>(requests);
        }

        public async Task<AuthorizationRequestDto?> GetByAuthorizationNumberAsync(string authorizationNumber, string tenantId)
        {
            var request = await _repository.GetByAuthorizationNumberAsync(authorizationNumber, tenantId);
            return request != null ? _mapper.Map<AuthorizationRequestDto>(request) : null;
        }

        public async Task<int> MarkExpiredAuthorizationsAsync(string tenantId)
        {
            var approvedRequests = await _repository.GetByStatusAsync(AuthorizationStatus.Approved, tenantId);
            var expiredCount = 0;

            foreach (var request in approvedRequests)
            {
                if (request.IsExpired())
                {
                    request.MarkAsExpired();
                    _repository.UpdateAsync(request);
                    expiredCount++;
                }
            }

            if (expiredCount > 0)
            {
                await _repository.SaveChangesAsync();
            }

            return expiredCount;
        }
    }
}
