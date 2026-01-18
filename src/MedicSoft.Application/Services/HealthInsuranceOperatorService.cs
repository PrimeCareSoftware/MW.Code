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
    /// Service implementation for managing health insurance operators
    /// </summary>
    public class HealthInsuranceOperatorService : IHealthInsuranceOperatorService
    {
        private readonly IHealthInsuranceOperatorRepository _repository;
        private readonly IMapper _mapper;

        public HealthInsuranceOperatorService(
            IHealthInsuranceOperatorRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HealthInsuranceOperatorDto> CreateAsync(CreateHealthInsuranceOperatorDto dto, string tenantId)
        {
            // Check if operator with same ANS register already exists
            var existing = await _repository.GetByRegisterNumberAsync(dto.RegisterNumber, tenantId);
            if (existing != null)
            {
                throw new InvalidOperationException($"An operator with ANS register {dto.RegisterNumber} already exists");
            }

            var operatorEntity = new HealthInsuranceOperator(
                dto.TradeName,
                dto.CompanyName,
                dto.RegisterNumber,
                dto.Document,
                tenantId,
                dto.Phone,
                dto.Email,
                dto.ContactPerson
            );

            if (!string.IsNullOrWhiteSpace(dto.WebsiteUrl))
            {
                operatorEntity.ConfigureIntegration(
                    OperatorIntegrationType.Manual,
                    dto.WebsiteUrl,
                    null,
                    null,
                    dto.RequiresPriorAuthorization
                );
            }
            else
            {
                operatorEntity.ConfigureIntegration(
                    OperatorIntegrationType.Manual,
                    null,
                    null,
                    null,
                    dto.RequiresPriorAuthorization
                );
            }

            await _repository.AddAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<HealthInsuranceOperatorDto> UpdateAsync(Guid id, UpdateHealthInsuranceOperatorDto dto, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {id} not found");
            }

            operatorEntity.UpdateBasicInfo(
                dto.TradeName,
                dto.CompanyName,
                dto.Phone,
                dto.Email,
                dto.ContactPerson
            );

            operatorEntity.ConfigureIntegration(
                operatorEntity.IntegrationType,
                dto.WebsiteUrl ?? operatorEntity.WebsiteUrl,
                operatorEntity.ApiEndpoint,
                operatorEntity.ApiKey,
                dto.RequiresPriorAuthorization
            );

            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<HealthInsuranceOperatorDto> ConfigureIntegrationAsync(Guid id, ConfigureOperatorIntegrationDto dto, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {id} not found");
            }

            if (!Enum.TryParse<OperatorIntegrationType>(dto.IntegrationType, true, out var integrationType))
            {
                throw new ArgumentException($"Invalid integration type: {dto.IntegrationType}");
            }

            operatorEntity.ConfigureIntegration(
                integrationType,
                dto.WebsiteUrl,
                dto.ApiEndpoint,
                dto.ApiKey,
                operatorEntity.RequiresPriorAuthorization
            );

            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<HealthInsuranceOperatorDto> ConfigureTissAsync(Guid id, ConfigureOperatorTissDto dto, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {id} not found");
            }

            operatorEntity.ConfigureTiss(
                dto.TissVersion,
                dto.SupportsTissXml,
                dto.BatchSubmissionEmail
            );

            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<IEnumerable<HealthInsuranceOperatorDto>> GetAllAsync(string tenantId, bool includeInactive = false)
        {
            var operators = await _repository.GetAllAsync(tenantId);
            
            if (!includeInactive)
            {
                operators = operators.Where(o => o.IsActive);
            }

            return _mapper.Map<IEnumerable<HealthInsuranceOperatorDto>>(operators);
        }

        public async Task<HealthInsuranceOperatorDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            return operatorEntity != null ? _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity) : null;
        }

        public async Task<HealthInsuranceOperatorDto?> GetByRegisterNumberAsync(string registerNumber, string tenantId)
        {
            var operatorEntity = await _repository.GetByRegisterNumberAsync(registerNumber, tenantId);
            return operatorEntity != null ? _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity) : null;
        }

        public async Task<IEnumerable<HealthInsuranceOperatorDto>> SearchByNameAsync(string name, string tenantId)
        {
            var operators = await _repository.SearchByNameAsync(name, tenantId);
            return _mapper.Map<IEnumerable<HealthInsuranceOperatorDto>>(operators);
        }

        public async Task<HealthInsuranceOperatorDto> ActivateAsync(Guid id, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {id} not found");
            }

            operatorEntity.Activate();
            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<HealthInsuranceOperatorDto> DeactivateAsync(Guid id, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {id} not found");
            }

            operatorEntity.Deactivate();
            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<HealthInsuranceOperatorDto>(operatorEntity);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var operatorEntity = await _repository.GetByIdAsync(id, tenantId);
            if (operatorEntity == null)
            {
                return false;
            }

            operatorEntity.Deactivate();
            _repository.UpdateAsync(operatorEntity);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
