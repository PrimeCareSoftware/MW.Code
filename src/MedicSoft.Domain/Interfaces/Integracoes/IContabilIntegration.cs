using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces.Integracoes
{
    /// <summary>
    /// Formato de exportação de arquivos contábeis
    /// </summary>
    public enum FormatoExportacao
    {
        TXT = 1,
        CSV = 2,
        XML = 3,
        JSON = 4
    }

    /// <summary>
    /// Status de integração
    /// </summary>
    public enum StatusIntegracao
    {
        NaoConfigurada = 0,
        Ativa = 1,
        Inativa = 2,
        Erro = 3
    }

    /// <summary>
    /// Interface base para integração com sistemas contábeis
    /// </summary>
    public interface IContabilIntegration
    {
        /// <summary>
        /// Nome do provedor de integração
        /// </summary>
        string NomeProvedor { get; }

        /// <summary>
        /// Testa a conexão com o sistema contábil
        /// </summary>
        Task<bool> TestarConexaoAsync();

        /// <summary>
        /// Envia um lançamento contábil para o sistema
        /// </summary>
        Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento);

        /// <summary>
        /// Envia múltiplos lançamentos em lote
        /// </summary>
        Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(IEnumerable<LancamentoContabil> lancamentos);

        /// <summary>
        /// Envia o plano de contas para o sistema
        /// </summary>
        Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas);

        /// <summary>
        /// Exporta arquivo contábil no formato especificado
        /// </summary>
        Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato);

        /// <summary>
        /// Valida as credenciais de integração
        /// </summary>
        Task<bool> ValidarCredenciaisAsync();
    }

    /// <summary>
    /// Resultado do envio de lançamentos em lote
    /// </summary>
    public class ResultadoEnvioLote
    {
        public int TotalEnviados { get; set; }
        public int TotalSucesso { get; set; }
        public int TotalErros { get; set; }
        public List<ErroEnvio> Erros { get; set; } = new();
        public bool Sucesso => TotalErros == 0;
    }

    /// <summary>
    /// Detalhes de erro no envio
    /// </summary>
    public class ErroEnvio
    {
        public Guid LancamentoId { get; set; }
        public string Mensagem { get; set; } = null!;
        public string? Detalhes { get; set; }
    }

    /// <summary>
    /// Arquivo exportado
    /// </summary>
    public class ArquivoExportacao
    {
        public string NomeArquivo { get; set; } = null!;
        public byte[] Conteudo { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public FormatoExportacao Formato { get; set; }
        public DateTime DataGeracao { get; set; }
    }
}
