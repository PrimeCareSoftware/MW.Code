using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing TUSS procedures
    /// </summary>
    public class TussProcedureService : ITussProcedureService
    {
        private readonly ITussProcedureRepository _repository;
        private readonly IMapper _mapper;

        public TussProcedureService(
            ITussProcedureRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TussProcedureDto> CreateAsync(CreateTussProcedureDto dto, string tenantId)
        {
            // Check if procedure with same code already exists
            var existing = await _repository.GetByCodeAsync(dto.Code, tenantId);
            if (existing != null)
            {
                throw new InvalidOperationException($"A procedure with code {dto.Code} already exists");
            }

            var procedure = new TussProcedure(
                dto.Code,
                dto.Description,
                dto.Category,
                dto.ReferencePrice,
                tenantId,
                dto.RequiresAuthorization
            );

            await _repository.AddAsync(procedure);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TussProcedureDto>(procedure);
        }

        public async Task<TussProcedureDto> UpdateAsync(Guid id, UpdateTussProcedureDto dto, string tenantId)
        {
            var procedure = await _repository.GetByIdAsync(id, tenantId);
            if (procedure == null)
            {
                throw new InvalidOperationException($"TUSS procedure with ID {id} not found");
            }

            procedure.UpdateInfo(
                dto.Description,
                dto.Category,
                dto.ReferencePrice,
                dto.RequiresAuthorization
            );

            _repository.UpdateAsync(procedure);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TussProcedureDto>(procedure);
        }

        public async Task<IEnumerable<TussProcedureDto>> GetAllAsync(string tenantId, bool includeInactive = false)
        {
            var procedures = await _repository.GetAllAsync(tenantId);
            
            if (!includeInactive)
            {
                procedures = procedures.Where(p => p.IsActive);
            }

            return _mapper.Map<IEnumerable<TussProcedureDto>>(procedures);
        }

        public async Task<TussProcedureDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var procedure = await _repository.GetByIdAsync(id, tenantId);
            return procedure != null ? _mapper.Map<TussProcedureDto>(procedure) : null;
        }

        public async Task<TussProcedureDto?> GetByCodeAsync(string code, string tenantId)
        {
            var procedure = await _repository.GetByCodeAsync(code, tenantId);
            return procedure != null ? _mapper.Map<TussProcedureDto>(procedure) : null;
        }

        public async Task<IEnumerable<TussProcedureDto>> SearchAsync(string query, string tenantId)
        {
            var procedures = await _repository.SearchByDescriptionAsync(query, tenantId);
            return _mapper.Map<IEnumerable<TussProcedureDto>>(procedures);
        }

        public async Task<IEnumerable<TussProcedureDto>> GetByCategoryAsync(string category, string tenantId)
        {
            var procedures = await _repository.GetByCategoryAsync(category, tenantId);
            return _mapper.Map<IEnumerable<TussProcedureDto>>(procedures);
        }

        public async Task<IEnumerable<TussProcedureDto>> GetRequiringAuthorizationAsync(string tenantId)
        {
            var allProcedures = await _repository.GetAllAsync(tenantId);
            var procedures = allProcedures.Where(p => p.RequiresAuthorization && p.IsActive);
            return _mapper.Map<IEnumerable<TussProcedureDto>>(procedures);
        }

        public async Task<TussImportResultDto> ImportFromCsvAsync(string filePath, string tenantId)
        {
            var result = new TussImportResultDto
            {
                Success = false,
                Errors = new List<string>()
            };

            try
            {
                if (!File.Exists(filePath))
                {
                    result.Errors.Add($"File not found: {filePath}");
                    result.Message = "Import failed: file not found";
                    return result;
                }

                var lines = await File.ReadAllLinesAsync(filePath);
                result.TotalRecords = lines.Length - 1; // Exclude header

                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var parts = lines[i].Split(',', ';', '\t');
                        
                        if (parts.Length < 4)
                        {
                            result.Errors.Add($"Line {i + 1}: Invalid format");
                            result.FailedRecords++;
                            continue;
                        }

                        var code = parts[0].Trim();
                        var description = parts[1].Trim();
                        var category = parts[2].Trim();
                        var referencePrice = decimal.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
                        var requiresAuth = parts.Length > 4 && bool.Parse(parts[4].Trim());

                        // Check if exists
                        var existing = await _repository.GetByCodeAsync(code, tenantId);
                        
                        if (existing != null)
                        {
                            // Update existing
                            existing.UpdateInfo(description, category, referencePrice, requiresAuth);
                            _repository.UpdateAsync(existing);
                            result.UpdatedRecords++;
                        }
                        else
                        {
                            // Create new
                            var procedure = new TussProcedure(
                                code,
                                description,
                                category,
                                referencePrice,
                                tenantId,
                                requiresAuth
                            );
                            await _repository.AddAsync(procedure);
                            result.ImportedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Line {i + 1}: {ex.Message}");
                        result.FailedRecords++;
                    }
                }

                await _repository.SaveChangesAsync();
                
                result.Success = result.FailedRecords < result.TotalRecords;
                result.Message = $"Import completed: {result.ImportedRecords} imported, {result.UpdatedRecords} updated, {result.FailedRecords} failed";
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Import failed: {ex.Message}");
                result.Message = "Import failed with errors";
            }

            return result;
        }

        public async Task<TussProcedureDto> ActivateAsync(Guid id, string tenantId)
        {
            var procedure = await _repository.GetByIdAsync(id, tenantId);
            if (procedure == null)
            {
                throw new InvalidOperationException($"TUSS procedure with ID {id} not found");
            }

            procedure.Activate();
            _repository.UpdateAsync(procedure);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TussProcedureDto>(procedure);
        }

        public async Task<TussProcedureDto> DeactivateAsync(Guid id, string tenantId)
        {
            var procedure = await _repository.GetByIdAsync(id, tenantId);
            if (procedure == null)
            {
                throw new InvalidOperationException($"TUSS procedure with ID {id} not found");
            }

            procedure.Deactivate();
            _repository.UpdateAsync(procedure);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TussProcedureDto>(procedure);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var procedure = await _repository.GetByIdAsync(id, tenantId);
            if (procedure == null)
            {
                return false;
            }

            procedure.Deactivate();
            _repository.UpdateAsync(procedure);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
