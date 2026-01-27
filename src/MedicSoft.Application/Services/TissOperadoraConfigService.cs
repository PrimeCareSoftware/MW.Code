using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing TISS operator webservice configurations
    /// </summary>
    public class TissOperadoraConfigService : ITissOperadoraConfigService
    {
        private readonly ITissOperadoraConfigRepository _repository;
        private readonly IHealthInsuranceOperatorRepository _operatorRepository;
        private readonly IMapper _mapper;

        public TissOperadoraConfigService(
            ITissOperadoraConfigRepository repository,
            IHealthInsuranceOperatorRepository operatorRepository,
            IMapper mapper)
        {
            _repository = repository;
            _operatorRepository = operatorRepository;
            _mapper = mapper;
        }

        public async Task<TissOperadoraConfigDto> CreateAsync(CreateTissOperadoraConfigDto dto, string tenantId)
        {
            // Validate operator exists
            var operatorEntity = await _operatorRepository.GetByIdAsync(dto.OperatorId, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Operator with ID {dto.OperatorId} not found");
            }

            // Check if configuration already exists for this operator
            var existing = await _repository.GetByOperatorIdAsync(dto.OperatorId, tenantId);
            if (existing != null)
            {
                throw new InvalidOperationException($"Configuration for operator {operatorEntity.TradeName} already exists");
            }

            // Encrypt password if provided
            string? encryptedPassword = null;
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                encryptedPassword = EncryptPassword(dto.Senha);
            }

            var config = new TissOperadoraConfig(
                dto.OperatorId,
                dto.WebServiceUrl,
                tenantId,
                dto.Usuario,
                encryptedPassword
            );

            // Configure additional settings
            config.ConfigureRetryPolicy(dto.TimeoutSegundos, dto.TentativasReenvio);
            config.SetUsaSoapHeader(dto.UsaSoapHeader);
            config.ConfigureCertificadoDigital(dto.UsaCertificadoDigital, dto.CertificadoDigitalPath);

            await _repository.AddAsync(config);
            await _repository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _repository.GetByIdAsync(config.Id, tenantId);
            return MapToDto(result!);
        }

        public async Task<TissOperadoraConfigDto> GetByIdAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
            {
                throw new InvalidOperationException($"Configuration with ID {id} not found");
            }

            return MapToDto(config);
        }

        public async Task<TissOperadoraConfigDto?> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            var config = await _repository.GetByOperatorIdAsync(operatorId, tenantId);
            return config != null ? MapToDto(config) : null;
        }

        public async Task<IEnumerable<TissOperadoraConfigDto>> GetAllAsync(string tenantId)
        {
            var configs = await _repository.GetAllAsync(tenantId);
            return configs.Select(MapToDto);
        }

        public async Task<IEnumerable<TissOperadoraConfigDto>> GetActiveConfigsAsync(string tenantId)
        {
            var configs = await _repository.GetActiveConfigsAsync(tenantId);
            return configs.Select(MapToDto);
        }

        public async Task<TissOperadoraConfigDto> UpdateAsync(Guid id, CreateTissOperadoraConfigDto dto, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
            {
                throw new InvalidOperationException($"Configuration with ID {id} not found");
            }

            // Encrypt password if provided
            string? encryptedPassword = null;
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                encryptedPassword = EncryptPassword(dto.Senha);
            }

            // Update credentials
            config.UpdateCredentials(dto.Usuario, encryptedPassword);
            
            // Update retry policy
            config.ConfigureRetryPolicy(dto.TimeoutSegundos, dto.TentativasReenvio);
            
            // Update SOAP and certificate settings
            config.SetUsaSoapHeader(dto.UsaSoapHeader);
            config.ConfigureCertificadoDigital(dto.UsaCertificadoDigital, dto.CertificadoDigitalPath);

            await _repository.UpdateAsync(config);
            await _repository.SaveChangesAsync();

            return MapToDto(config);
        }

        public async Task<bool> ActivateAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
            {
                return false;
            }

            config.Activate();
            await _repository.UpdateAsync(config);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
            {
                return false;
            }

            config.Deactivate();
            await _repository.UpdateAsync(config);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
            {
                return false;
            }

            await _repository.DeleteAsync(id, tenantId);
            await _repository.SaveChangesAsync();

            return true;
        }

        private TissOperadoraConfigDto MapToDto(TissOperadoraConfig config)
        {
            return new TissOperadoraConfigDto
            {
                Id = config.Id,
                OperatorId = config.OperatorId,
                OperatorName = config.Operator?.TradeName ?? string.Empty,
                WebServiceUrl = config.WebServiceUrl,
                Usuario = config.Usuario,
                TimeoutSegundos = config.TimeoutSegundos,
                TentativasReenvio = config.TentativasReenvio,
                UsaSoapHeader = config.UsaSoapHeader,
                UsaCertificadoDigital = config.UsaCertificadoDigital,
                CertificadoDigitalPath = config.CertificadoDigitalPath,
                IsActive = config.IsActive
            };
        }

        private string EncryptPassword(string password)
        {
            // TODO: In production, use a proper password hashing algorithm like bcrypt, scrypt, or Argon2
            // This is a simplified implementation using SHA256 for demonstration
            // SHA256 is not suitable for password storage as it's vulnerable to rainbow table attacks
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
