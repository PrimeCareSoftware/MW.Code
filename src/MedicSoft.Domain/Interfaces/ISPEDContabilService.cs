using System;
using System.Threading.Tasks;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface para geração e validação de arquivos SPED Contábil (ECD)
    /// </summary>
    public interface ISPEDContabilService
    {
        /// <summary>
        /// Gera arquivo SPED Contábil para o período especificado
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <returns>Conteúdo do arquivo SPED Contábil</returns>
        Task<string> GerarSPEDContabilAsync(Guid clinicaId, DateTime inicio, DateTime fim);

        /// <summary>
        /// Valida estrutura de arquivo SPED Contábil
        /// </summary>
        /// <param name="conteudoSPED">Conteúdo do arquivo SPED</param>
        /// <returns>Resultado da validação com mensagens de erro (se houver)</returns>
        Task<SPEDValidationResult> ValidarSPEDContabilAsync(string conteudoSPED);

        /// <summary>
        /// Exporta arquivo SPED Contábil e salva em arquivo
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="caminhoArquivo">Caminho onde salvar o arquivo</param>
        /// <returns>Caminho do arquivo gerado</returns>
        Task<string> ExportarSPEDContabilAsync(Guid clinicaId, DateTime inicio, DateTime fim, string caminhoArquivo);
    }
}
