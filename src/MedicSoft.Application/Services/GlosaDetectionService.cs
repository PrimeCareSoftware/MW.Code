using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for detecting and processing glosas from TISS batch responses
    /// </summary>
    public interface IGlosaDetectionService
    {
        Task ProcessarRetornoLoteAsync(Guid loteId, XDocument xmlRetorno, string tenantId);
        Task<List<TissGlosa>> ExtractGlosasFromXmlAsync(XDocument xmlRetorno, string tenantId);
    }

    public class GlosaDetectionService : IGlosaDetectionService
    {
        private readonly ITissGuideRepository _guiaRepository;
        private readonly ITissGlosaRepository _glosaRepository;
        private readonly ILogger<GlosaDetectionService> _logger;

        public GlosaDetectionService(
            ITissGuideRepository guiaRepository,
            ITissGlosaRepository glosaRepository,
            ILogger<GlosaDetectionService> logger)
        {
            _guiaRepository = guiaRepository;
            _glosaRepository = glosaRepository;
            _logger = logger;
        }

        public async Task ProcessarRetornoLoteAsync(Guid loteId, XDocument xmlRetorno, string tenantId)
        {
            _logger.LogInformation("Processing batch return for batch {LoteId}", loteId);

            try
            {
                var glosas = await ExtractGlosasFromXmlAsync(xmlRetorno, tenantId);
                
                foreach (var glosa in glosas)
                {
                    await _glosaRepository.AddAsync(glosa);
                    _logger.LogInformation(
                        "Created glosa for guide {NumeroGuia}. Amount: {Valor}",
                        glosa.NumeroGuia, glosa.ValorGlosado);
                }

                _logger.LogInformation(
                    "Processed {Count} glosas for batch {LoteId}",
                    glosas.Count, loteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch return for {LoteId}", loteId);
                throw;
            }
        }

        public async Task<List<TissGlosa>> ExtractGlosasFromXmlAsync(XDocument xmlRetorno, string tenantId)
        {
            var glosas = new List<TissGlosa>();

            try
            {
                // Parse XML to extract glosa information
                // This is a simplified implementation - actual parsing depends on ANS XML schema
                var glosaElements = xmlRetorno.Descendants("glosa");

                foreach (var glosaElement in glosaElements)
                {
                    var numeroGuia = glosaElement.Element("numeroGuia")?.Value ?? string.Empty;
                    var codigoGlosa = glosaElement.Element("codigoGlosa")?.Value ?? string.Empty;
                    var descricao = glosaElement.Element("descricaoGlosa")?.Value ?? string.Empty;
                    var valorGlosadoStr = glosaElement.Element("valorGlosado")?.Value ?? "0";
                    
                    if (string.IsNullOrWhiteSpace(numeroGuia))
                        continue;

                    // Find the guide
                    var guia = await _guiaRepository.GetByGuideNumberAsync(numeroGuia, tenantId);
                    if (guia == null)
                    {
                        _logger.LogWarning("Guide {NumeroGuia} not found for glosa", numeroGuia);
                        continue;
                    }

                    var tipoGlosa = ClassificarTipoGlosa(codigoGlosa);
                    var valorGlosado = decimal.TryParse(valorGlosadoStr, out var valor) ? valor : 0m;

                    var glosa = new TissGlosa(
                        guia.Id,
                        numeroGuia,
                        DateTime.UtcNow,
                        tipoGlosa,
                        codigoGlosa,
                        descricao,
                        valorGlosado,
                        guia.TotalAmount,
                        tenantId);

                    glosas.Add(glosa);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting glosas from XML");
                throw;
            }

            return glosas;
        }

        private TipoGlosa ClassificarTipoGlosa(string codigoGlosa)
        {
            // Classification based on ANS glosa codes
            // This is a simplified implementation
            if (codigoGlosa.StartsWith("A", StringComparison.OrdinalIgnoreCase))
                return TipoGlosa.Administrativa;
            
            if (codigoGlosa.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                return TipoGlosa.Tecnica;
            
            return TipoGlosa.Financeira;
        }
    }
}
