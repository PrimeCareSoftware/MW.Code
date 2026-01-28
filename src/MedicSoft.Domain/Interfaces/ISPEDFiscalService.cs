using System;
using System.Threading.Tasks;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface para geração e validação de arquivos SPED Fiscal
    /// </summary>
    public interface ISPEDFiscalService
    {
        /// <summary>
        /// Gera arquivo SPED Fiscal para o período especificado
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Conteúdo do arquivo SPED Fiscal</returns>
        Task<string> GerarSPEDFiscalAsync(Guid clinicaId, DateTime inicio, DateTime fim, string tenantId);

        /// <summary>
        /// Valida estrutura de arquivo SPED Fiscal
        /// </summary>
        /// <param name="conteudoSPED">Conteúdo do arquivo SPED</param>
        /// <returns>Resultado da validação com mensagens de erro (se houver)</returns>
        Task<SPEDValidationResult> ValidarSPEDFiscalAsync(string conteudoSPED);

        /// <summary>
        /// Exporta arquivo SPED Fiscal e salva em arquivo
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="caminhoArquivo">Caminho onde salvar o arquivo</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Caminho do arquivo gerado</returns>
        Task<string> ExportarSPEDFiscalAsync(Guid clinicaId, DateTime inicio, DateTime fim, string caminhoArquivo, string tenantId);
    }

    /// <summary>
    /// Resultado da validação de arquivo SPED
    /// </summary>
    public class SPEDValidationResult
    {
        public bool Valido { get; set; }
        public string[] Erros { get; set; } = Array.Empty<string>();
        public string[] Avisos { get; set; } = Array.Empty<string>();
        public int TotalRegistros { get; set; }
        public int TotalBlocos { get; set; }
    }
}
