using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO representing a TISS glosa (rejection)
    /// </summary>
    public class TissGlosaDto
    {
        public Guid Id { get; set; }
        public Guid GuideId { get; set; }
        public string NumeroGuia { get; set; } = string.Empty;
        public DateTime DataGlosa { get; set; }
        public DateTime DataIdentificacao { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string CodigoGlosa { get; set; } = string.Empty;
        public string DescricaoGlosa { get; set; } = string.Empty;
        public decimal ValorGlosado { get; set; }
        public decimal ValorOriginal { get; set; }
        public int? SequenciaItem { get; set; }
        public string? CodigoProcedimento { get; set; }
        public string? NomeProcedimento { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? JustificativaRecurso { get; set; }
        public List<TissRecursoGlosaDto> Recursos { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating a glosa
    /// </summary>
    public class CreateTissGlosaDto
    {
        public Guid GuideId { get; set; }
        public string NumeroGuia { get; set; } = string.Empty;
        public DateTime DataGlosa { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string CodigoGlosa { get; set; } = string.Empty;
        public string DescricaoGlosa { get; set; } = string.Empty;
        public decimal ValorGlosado { get; set; }
        public decimal ValorOriginal { get; set; }
        public int? SequenciaItem { get; set; }
        public string? CodigoProcedimento { get; set; }
        public string? NomeProcedimento { get; set; }
    }

    /// <summary>
    /// DTO representing a glosa appeal (recurso)
    /// </summary>
    public class TissRecursoGlosaDto
    {
        public Guid Id { get; set; }
        public Guid GlosaId { get; set; }
        public DateTime DataEnvio { get; set; }
        public string Justificativa { get; set; } = string.Empty;
        public DateTime? DataResposta { get; set; }
        public string? Resultado { get; set; }
        public string? JustificativaOperadora { get; set; }
        public decimal? ValorDeferido { get; set; }
    }

    /// <summary>
    /// DTO for creating a recurso
    /// </summary>
    public class CreateTissRecursoGlosaDto
    {
        public Guid GlosaId { get; set; }
        public string Justificativa { get; set; } = string.Empty;
        public string? AnexosJson { get; set; }
    }
}
