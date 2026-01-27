using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa uma glosa (rejeição/desconto) de guia TISS
    /// </summary>
    public class TissGlosa : BaseEntity
    {
        public Guid GuideId { get; private set; }
        public TissGuide? Guide { get; private set; }
        
        public string NumeroGuia { get; private set; }
        public DateTime DataGlosa { get; private set; }
        public DateTime DataIdentificacao { get; private set; }
        
        // Dados da glosa
        public TipoGlosa Tipo { get; private set; }
        public string CodigoGlosa { get; private set; }
        public string DescricaoGlosa { get; private set; }
        public decimal ValorGlosado { get; private set; }
        public decimal ValorOriginal { get; private set; }
        
        // Item específico glosado
        public int? SequenciaItem { get; private set; }
        public string? CodigoProcedimento { get; private set; }
        public string? NomeProcedimento { get; private set; }
        
        // Status
        public StatusGlosa Status { get; private set; }
        public string? JustificativaRecurso { get; private set; }
        
        // Navigation properties
        private readonly List<TissRecursoGlosa> _recursos = new();
        public IReadOnlyCollection<TissRecursoGlosa> Recursos => _recursos.AsReadOnly();
        
        private TissGlosa() 
        { 
            // EF Constructor
            NumeroGuia = null!;
            CodigoGlosa = null!;
            DescricaoGlosa = null!;
        }

        public TissGlosa(
            Guid guideId,
            string numeroGuia,
            DateTime dataGlosa,
            TipoGlosa tipo,
            string codigoGlosa,
            string descricaoGlosa,
            decimal valorGlosado,
            decimal valorOriginal,
            string tenantId) : base(tenantId)
        {
            if (guideId == Guid.Empty)
                throw new ArgumentException("Guide ID cannot be empty", nameof(guideId));
            
            if (string.IsNullOrWhiteSpace(numeroGuia))
                throw new ArgumentException("Guide number cannot be empty", nameof(numeroGuia));
            
            if (string.IsNullOrWhiteSpace(codigoGlosa))
                throw new ArgumentException("Glosa code cannot be empty", nameof(codigoGlosa));
            
            if (string.IsNullOrWhiteSpace(descricaoGlosa))
                throw new ArgumentException("Glosa description cannot be empty", nameof(descricaoGlosa));
            
            if (valorGlosado < 0)
                throw new ArgumentException("Glosa value cannot be negative", nameof(valorGlosado));
            
            if (valorOriginal < 0)
                throw new ArgumentException("Original value cannot be negative", nameof(valorOriginal));

            GuideId = guideId;
            NumeroGuia = numeroGuia.Trim();
            DataGlosa = dataGlosa;
            DataIdentificacao = DateTime.UtcNow;
            Tipo = tipo;
            CodigoGlosa = codigoGlosa.Trim();
            DescricaoGlosa = descricaoGlosa.Trim();
            ValorGlosado = valorGlosado;
            ValorOriginal = valorOriginal;
            Status = StatusGlosa.Nova;
        }

        public void SetItemGlosado(int? sequenciaItem, string? codigoProcedimento, string? nomeProcedimento)
        {
            SequenciaItem = sequenciaItem;
            CodigoProcedimento = codigoProcedimento?.Trim();
            NomeProcedimento = nomeProcedimento?.Trim();
            UpdateTimestamp();
        }

        public void MarcarEmAnalise()
        {
            if (Status != StatusGlosa.Nova)
                throw new InvalidOperationException($"Cannot mark glosa as in analysis from status {Status}");

            Status = StatusGlosa.EmAnalise;
            UpdateTimestamp();
        }

        public void AdicionarRecurso(TissRecursoGlosa recurso)
        {
            if (recurso == null)
                throw new ArgumentNullException(nameof(recurso));

            if (Status == StatusGlosa.RecursoDeferido || Status == StatusGlosa.RecursoIndeferido || Status == StatusGlosa.Acatada)
                throw new InvalidOperationException($"Cannot add resource to glosa in final status {Status}");

            _recursos.Add(recurso);
            Status = StatusGlosa.RecursoEnviado;
            UpdateTimestamp();
        }

        public void DeferirRecurso(decimal? valorDeferido = null)
        {
            if (Status != StatusGlosa.RecursoEnviado)
                throw new InvalidOperationException($"Cannot defer resource for glosa in status {Status}");

            Status = StatusGlosa.RecursoDeferido;
            
            if (valorDeferido.HasValue)
            {
                if (valorDeferido.Value < 0)
                    throw new ArgumentException("Deferred value cannot be negative", nameof(valorDeferido));
                
                ValorGlosado = Math.Max(0, ValorGlosado - valorDeferido.Value);
            }
            else
            {
                ValorGlosado = 0; // Recurso totalmente deferido
            }
            
            UpdateTimestamp();
        }

        public void IndeferirRecurso(string? justificativa = null)
        {
            if (Status != StatusGlosa.RecursoEnviado)
                throw new InvalidOperationException($"Cannot reject resource for glosa in status {Status}");

            Status = StatusGlosa.RecursoIndeferido;
            
            if (!string.IsNullOrWhiteSpace(justificativa))
            {
                JustificativaRecurso = justificativa.Trim();
            }
            
            UpdateTimestamp();
        }

        public void AcatarGlosa()
        {
            if (Status == StatusGlosa.RecursoDeferido)
                throw new InvalidOperationException("Cannot accept glosa that has been deferred");

            Status = StatusGlosa.Acatada;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Tipo de glosa segundo padrão ANS
    /// </summary>
    public enum TipoGlosa
    {
        Administrativa = 1, // Dados incorretos, ausência de documentos
        Tecnica = 2,        // Procedimento não autorizado, incompatível
        Financeira = 3      // Valores divergentes
    }

    /// <summary>
    /// Status do processo de glosa
    /// </summary>
    public enum StatusGlosa
    {
        Nova = 1,               // Glosa recém identificada
        EmAnalise = 2,          // Em análise pela equipe
        RecursoEnviado = 3,     // Recurso enviado à operadora
        RecursoDeferido = 4,    // Recurso aceito pela operadora
        RecursoIndeferido = 5,  // Recurso rejeitado pela operadora
        Acatada = 6             // Glosa aceita pela clínica
    }
}
