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
    /// Service implementation for managing TISS glosas (rejections/discounts)
    /// </summary>
    public class TissGlosaService : ITissGlosaService
    {
        private readonly ITissGlosaRepository _glosaRepository;
        private readonly ITissGuideRepository _guideRepository;
        private readonly IMapper _mapper;

        public TissGlosaService(
            ITissGlosaRepository glosaRepository,
            ITissGuideRepository guideRepository,
            IMapper mapper)
        {
            _glosaRepository = glosaRepository;
            _guideRepository = guideRepository;
            _mapper = mapper;
        }

        public async Task<TissGlosaDto> CreateAsync(CreateTissGlosaDto dto, string tenantId)
        {
            // Validate guide exists
            var guide = await _guideRepository.GetByIdAsync(dto.GuideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"Guide with ID {dto.GuideId} not found");
            }

            // Parse tipo glosa
            if (!Enum.TryParse<TipoGlosa>(dto.Tipo, true, out var tipoGlosa))
            {
                throw new ArgumentException($"Invalid glosa type: {dto.Tipo}");
            }

            var glosa = new TissGlosa(
                dto.GuideId,
                dto.NumeroGuia,
                dto.DataGlosa,
                tipoGlosa,
                dto.CodigoGlosa,
                dto.DescricaoGlosa,
                dto.ValorGlosado,
                dto.ValorOriginal,
                tenantId
            );

            // Set item details if provided
            if (dto.SequenciaItem.HasValue || !string.IsNullOrWhiteSpace(dto.CodigoProcedimento))
            {
                glosa.SetItemGlosado(dto.SequenciaItem, dto.CodigoProcedimento, dto.NomeProcedimento);
            }

            await _glosaRepository.AddAsync(glosa);
            await _glosaRepository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _glosaRepository.GetWithRecursosAsync(glosa.Id, tenantId);
            return MapToDto(result!);
        }

        public async Task<TissGlosaDto> GetByIdAsync(Guid id, string tenantId)
        {
            var glosa = await _glosaRepository.GetWithRecursosAsync(id, tenantId);
            if (glosa == null)
            {
                throw new InvalidOperationException($"Glosa with ID {id} not found");
            }

            return MapToDto(glosa);
        }

        public async Task<IEnumerable<TissGlosaDto>> GetByGuideIdAsync(Guid guideId, string tenantId)
        {
            var glosas = await _glosaRepository.GetByGuideIdAsync(guideId, tenantId);
            return glosas.Select(MapToDto);
        }

        public async Task<IEnumerable<TissGlosaDto>> GetByStatusAsync(StatusGlosa status, string tenantId)
        {
            var glosas = await _glosaRepository.GetByStatusAsync(status, tenantId);
            return glosas.Select(MapToDto);
        }

        public async Task<IEnumerable<TissGlosaDto>> GetByTipoAsync(TipoGlosa tipo, string tenantId)
        {
            var glosas = await _glosaRepository.GetByTipoAsync(tipo, tenantId);
            return glosas.Select(MapToDto);
        }

        public async Task<IEnumerable<TissGlosaDto>> GetByDateRangeAsync(DateTime start, DateTime end, string tenantId)
        {
            var glosas = await _glosaRepository.GetByDateRangeAsync(start, end, tenantId);
            return glosas.Select(MapToDto);
        }

        public async Task<IEnumerable<TissGlosaDto>> GetPendingRecursosAsync(string tenantId)
        {
            var glosas = await _glosaRepository.GetPendingRecursosAsync(tenantId);
            return glosas.Select(MapToDto);
        }

        public async Task<TissGlosaDto> MarcarEmAnaliseAsync(Guid id, string tenantId)
        {
            var glosa = await _glosaRepository.GetByIdAsync(id, tenantId);
            if (glosa == null)
            {
                throw new InvalidOperationException($"Glosa with ID {id} not found");
            }

            glosa.MarcarEmAnalise();
            await _glosaRepository.UpdateAsync(glosa);
            await _glosaRepository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _glosaRepository.GetWithRecursosAsync(id, tenantId);
            return MapToDto(result!);
        }

        public async Task<TissGlosaDto> AcatarGlosaAsync(Guid id, string tenantId)
        {
            var glosa = await _glosaRepository.GetByIdAsync(id, tenantId);
            if (glosa == null)
            {
                throw new InvalidOperationException($"Glosa with ID {id} not found");
            }

            glosa.AcatarGlosa();
            await _glosaRepository.UpdateAsync(glosa);
            await _glosaRepository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _glosaRepository.GetWithRecursosAsync(id, tenantId);
            return MapToDto(result!);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var glosa = await _glosaRepository.GetByIdAsync(id, tenantId);
            if (glosa == null)
            {
                return false;
            }

            await _glosaRepository.DeleteAsync(id, tenantId);
            await _glosaRepository.SaveChangesAsync();

            return true;
        }

        private TissGlosaDto MapToDto(TissGlosa glosa)
        {
            return new TissGlosaDto
            {
                Id = glosa.Id,
                GuideId = glosa.GuideId,
                NumeroGuia = glosa.NumeroGuia,
                DataGlosa = glosa.DataGlosa,
                DataIdentificacao = glosa.DataIdentificacao,
                Tipo = glosa.Tipo.ToString(),
                CodigoGlosa = glosa.CodigoGlosa,
                DescricaoGlosa = glosa.DescricaoGlosa,
                ValorGlosado = glosa.ValorGlosado,
                ValorOriginal = glosa.ValorOriginal,
                SequenciaItem = glosa.SequenciaItem,
                CodigoProcedimento = glosa.CodigoProcedimento,
                NomeProcedimento = glosa.NomeProcedimento,
                Status = glosa.Status.ToString(),
                JustificativaRecurso = glosa.JustificativaRecurso,
                Recursos = glosa.Recursos.Select(r => new TissRecursoGlosaDto
                {
                    Id = r.Id,
                    GlosaId = r.GlosaId,
                    DataEnvio = r.DataEnvio,
                    Justificativa = r.Justificativa,
                    DataResposta = r.DataResposta,
                    Resultado = r.Resultado?.ToString(),
                    JustificativaOperadora = r.JustificativaOperadora,
                    ValorDeferido = r.ValorDeferido
                }).ToList()
            };
        }
    }
}
