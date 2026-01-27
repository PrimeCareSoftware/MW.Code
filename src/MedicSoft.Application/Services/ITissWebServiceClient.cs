using System;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Response from TISS webservice batch submission
    /// </summary>
    public class TissRetornoLote
    {
        public string ProtocoloOperadora { get; set; } = string.Empty;
        public DateTime DataRecebimento { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Mensagem { get; set; }
        public string? XmlRetorno { get; set; }
    }

    /// <summary>
    /// Response from TISS webservice guide query
    /// </summary>
    public class TissRetornoGuia
    {
        public string NumeroGuia { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? ValorAprovado { get; set; }
        public decimal? ValorGlosado { get; set; }
        public string? MotivoGlosa { get; set; }
        public string? XmlRetorno { get; set; }
    }

    /// <summary>
    /// Response from TISS webservice appeal submission
    /// </summary>
    public class TissRetornoRecurso
    {
        public string ProtocoloRecurso { get; set; } = string.Empty;
        public DateTime DataRecebimento { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Mensagem { get; set; }
    }

    /// <summary>
    /// Interface for TISS webservice client
    /// </summary>
    public interface ITissWebServiceClient
    {
        /// <summary>
        /// Send a batch to the operator
        /// </summary>
        Task<TissRetornoLote> EnviarLoteAsync(Guid loteId);

        /// <summary>
        /// Query batch status from the operator
        /// </summary>
        Task<TissRetornoLote> ConsultarLoteAsync(string protocoloOperadora);

        /// <summary>
        /// Query guide status from the operator
        /// </summary>
        Task<TissRetornoGuia> ConsultarGuiaAsync(string numeroGuia);

        /// <summary>
        /// Cancel a guide
        /// </summary>
        Task<bool> CancelarGuiaAsync(string numeroGuia, string motivo);

        /// <summary>
        /// Submit an appeal (recurso) for a glosa
        /// </summary>
        Task<TissRetornoRecurso> EnviarRecursoAsync(Guid recursoId);
    }
}
