using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um recurso (contestação) de glosa TISS
    /// </summary>
    public class TissRecursoGlosa : BaseEntity
    {
        public Guid GlosaId { get; private set; }
        public TissGlosa? Glosa { get; private set; }
        
        public DateTime DataEnvio { get; private set; }
        public string Justificativa { get; private set; }
        
        // Resposta da operadora
        public DateTime? DataResposta { get; private set; }
        public ResultadoRecurso? Resultado { get; private set; }
        public string? JustificativaOperadora { get; private set; }
        public decimal? ValorDeferido { get; private set; }
        
        // Anexos (lista de caminhos de arquivo ou IDs de documentos)
        public string? AnexosJson { get; private set; }
        
        private TissRecursoGlosa() 
        { 
            // EF Constructor
            Justificativa = null!;
        }

        public TissRecursoGlosa(
            Guid glosaId,
            string justificativa,
            string tenantId,
            string? anexosJson = null) : base(tenantId)
        {
            if (glosaId == Guid.Empty)
                throw new ArgumentException("Glosa ID cannot be empty", nameof(glosaId));
            
            if (string.IsNullOrWhiteSpace(justificativa))
                throw new ArgumentException("Justification cannot be empty", nameof(justificativa));

            GlosaId = glosaId;
            Justificativa = justificativa.Trim();
            DataEnvio = DateTime.UtcNow;
            AnexosJson = anexosJson?.Trim();
        }

        public void RegistrarResposta(
            ResultadoRecurso resultado,
            string? justificativaOperadora = null,
            decimal? valorDeferido = null)
        {
            if (DataResposta.HasValue)
                throw new InvalidOperationException("Resource response has already been registered");

            if (valorDeferido.HasValue && valorDeferido.Value < 0)
                throw new ArgumentException("Deferred value cannot be negative", nameof(valorDeferido));

            DataResposta = DateTime.UtcNow;
            Resultado = resultado;
            JustificativaOperadora = justificativaOperadora?.Trim();
            ValorDeferido = valorDeferido;
            UpdateTimestamp();
        }

        public void AtualizarAnexos(string anexosJson)
        {
            if (DataResposta.HasValue)
                throw new InvalidOperationException("Cannot update attachments after response is received");

            AnexosJson = anexosJson?.Trim();
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Resultado do recurso de glosa
    /// </summary>
    public enum ResultadoRecurso
    {
        Deferido = 1,      // Glosa revertida totalmente
        Parcial = 2,       // Glosa parcialmente revertida
        Indeferido = 3     // Glosa mantida
    }
}
