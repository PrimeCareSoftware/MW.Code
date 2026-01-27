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
    /// Service implementation for managing TISS glosa appeals (recursos)
    /// </summary>
    public class TissRecursoGlosaService : ITissRecursoGlosaService
    {
        private readonly ITissRecursoGlosaRepository _recursoRepository;
        private readonly ITissGlosaRepository _glosaRepository;
        private readonly IMapper _mapper;

        public TissRecursoGlosaService(
            ITissRecursoGlosaRepository recursoRepository,
            ITissGlosaRepository glosaRepository,
            IMapper mapper)
        {
            _recursoRepository = recursoRepository;
            _glosaRepository = glosaRepository;
            _mapper = mapper;
        }

        public async Task<TissRecursoGlosaDto> CreateAsync(CreateTissRecursoGlosaDto dto, string tenantId)
        {
            // Validate glosa exists
            var glosa = await _glosaRepository.GetByIdAsync(dto.GlosaId, tenantId);
            if (glosa == null)
            {
                throw new InvalidOperationException($"Glosa with ID {dto.GlosaId} not found");
            }

            var recurso = new TissRecursoGlosa(
                dto.GlosaId,
                dto.Justificativa,
                tenantId,
                dto.AnexosJson
            );

            // Add recurso to glosa
            glosa.AdicionarRecurso(recurso);

            await _recursoRepository.AddAsync(recurso);
            await _glosaRepository.UpdateAsync(glosa);
            await _recursoRepository.SaveChangesAsync();

            return MapToDto(recurso);
        }

        public async Task<TissRecursoGlosaDto> GetByIdAsync(Guid id, string tenantId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id, tenantId);
            if (recurso == null)
            {
                throw new InvalidOperationException($"Recurso with ID {id} not found");
            }

            return MapToDto(recurso);
        }

        public async Task<IEnumerable<TissRecursoGlosaDto>> GetByGlosaIdAsync(Guid glosaId, string tenantId)
        {
            var recursos = await _recursoRepository.GetAllAsync(tenantId);
            var filtered = recursos.Where(r => r.GlosaId == glosaId);
            return filtered.Select(MapToDto);
        }

        public async Task<IEnumerable<TissRecursoGlosaDto>> GetPendingResponseAsync(string tenantId)
        {
            var recursos = await _recursoRepository.GetPendingResponseAsync(tenantId);
            return recursos.Select(MapToDto);
        }

        public async Task<IEnumerable<TissRecursoGlosaDto>> GetByResultadoAsync(ResultadoRecurso resultado, string tenantId)
        {
            var recursos = await _recursoRepository.GetByResultadoAsync(resultado, tenantId);
            return recursos.Select(MapToDto);
        }

        public async Task<TissRecursoGlosaDto> RegistrarRespostaAsync(
            Guid id,
            ResultadoRecurso resultado,
            string? justificativaOperadora,
            decimal? valorDeferido,
            string tenantId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id, tenantId);
            if (recurso == null)
            {
                throw new InvalidOperationException($"Recurso with ID {id} not found");
            }

            var glosa = await _glosaRepository.GetByIdAsync(recurso.GlosaId, tenantId);
            if (glosa == null)
            {
                throw new InvalidOperationException($"Glosa with ID {recurso.GlosaId} not found");
            }

            // Register response on recurso
            recurso.RegistrarResposta(resultado, justificativaOperadora, valorDeferido);

            // Update glosa status based on resultado
            switch (resultado)
            {
                case ResultadoRecurso.Deferido:
                    glosa.DeferirRecurso(valorDeferido);
                    break;
                case ResultadoRecurso.Parcial:
                    glosa.DeferirRecurso(valorDeferido);
                    break;
                case ResultadoRecurso.Indeferido:
                    glosa.IndeferirRecurso(justificativaOperadora);
                    break;
            }

            await _recursoRepository.UpdateAsync(recurso);
            await _glosaRepository.UpdateAsync(glosa);
            await _recursoRepository.SaveChangesAsync();

            return MapToDto(recurso);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id, tenantId);
            if (recurso == null)
            {
                return false;
            }

            await _recursoRepository.DeleteAsync(id, tenantId);
            await _recursoRepository.SaveChangesAsync();

            return true;
        }

        private TissRecursoGlosaDto MapToDto(TissRecursoGlosa recurso)
        {
            return new TissRecursoGlosaDto
            {
                Id = recurso.Id,
                GlosaId = recurso.GlosaId,
                DataEnvio = recurso.DataEnvio,
                Justificativa = recurso.Justificativa,
                DataResposta = recurso.DataResposta,
                Resultado = recurso.Resultado?.ToString(),
                JustificativaOperadora = recurso.JustificativaOperadora,
                ValorDeferido = recurso.ValorDeferido
            };
        }
    }
}
