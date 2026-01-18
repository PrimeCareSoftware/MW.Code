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
    /// Service for managing health insurance plans
    /// </summary>
    public class HealthInsurancePlanService : IHealthInsurancePlanService
    {
        private readonly IHealthInsurancePlanRepository _repository;
        private readonly IMapper _mapper;

        public HealthInsurancePlanService(IHealthInsurancePlanRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<HealthInsurancePlanDto> CreateAsync(CreateHealthInsurancePlanDto dto, string tenantId)
        {
            var planType = Enum.Parse<HealthInsurancePlanType>(dto.Type);
            
            var plan = new HealthInsurancePlan(
                dto.OperatorId,
                dto.PlanName,
                dto.PlanCode,
                planType,
                tenantId,
                dto.RegisterNumber,
                dto.CoversConsultations,
                dto.CoversExams,
                dto.CoversProcedures,
                dto.RequiresPriorAuthorization,
                dto.TissEnabled
            );

            await _repository.AddAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<HealthInsurancePlanDto> UpdateAsync(Guid id, UpdateHealthInsurancePlanDto dto, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                throw new InvalidOperationException($"Health insurance plan with ID {id} not found");

            var planType = Enum.Parse<HealthInsurancePlanType>(dto.Type);
            plan.UpdatePlanInfo(dto.PlanName, dto.PlanCode, dto.RegisterNumber, planType);
            plan.UpdateCoverage(dto.CoversConsultations, dto.CoversExams, dto.CoversProcedures, dto.RequiresPriorAuthorization);

            await _repository.UpdateAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<IEnumerable<HealthInsurancePlanDto>> GetAllAsync(string tenantId, bool includeInactive = false)
        {
            var plans = includeInactive 
                ? await _repository.GetAllAsync(tenantId)
                : await _repository.GetActiveAsync(tenantId);
                
            return _mapper.Map<IEnumerable<HealthInsurancePlanDto>>(plans);
        }

        public async Task<HealthInsurancePlanDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            return plan != null ? _mapper.Map<HealthInsurancePlanDto>(plan) : null;
        }

        public async Task<IEnumerable<HealthInsurancePlanDto>> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            var plans = await _repository.GetByOperatorIdAsync(operatorId, tenantId);
            return _mapper.Map<IEnumerable<HealthInsurancePlanDto>>(plans);
        }

        public async Task<HealthInsurancePlanDto?> GetByPlanCodeAsync(string planCode, string tenantId)
        {
            var plan = await _repository.GetByPlanCodeAsync(planCode, tenantId);
            return plan != null ? _mapper.Map<HealthInsurancePlanDto>(plan) : null;
        }

        public async Task<HealthInsurancePlanDto> EnableTissAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                throw new InvalidOperationException($"Health insurance plan with ID {id} not found");

            plan.EnableTiss();
            await _repository.UpdateAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<HealthInsurancePlanDto> DisableTissAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                throw new InvalidOperationException($"Health insurance plan with ID {id} not found");

            plan.DisableTiss();
            await _repository.UpdateAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<HealthInsurancePlanDto> ActivateAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                throw new InvalidOperationException($"Health insurance plan with ID {id} not found");

            plan.Activate();
            await _repository.UpdateAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<HealthInsurancePlanDto> DeactivateAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                throw new InvalidOperationException($"Health insurance plan with ID {id} not found");

            plan.Deactivate();
            await _repository.UpdateAsync(plan);
            return _mapper.Map<HealthInsurancePlanDto>(plan);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var plan = await _repository.GetByIdAsync(id, tenantId);
            if (plan == null)
                return false;

            plan.Deactivate();
            await _repository.UpdateAsync(plan);
            return true;
        }
    }
}
