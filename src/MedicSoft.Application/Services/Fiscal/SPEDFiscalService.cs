using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services.Fiscal
{
    /// <summary>
    /// Serviço para geração e validação de arquivos SPED Fiscal
    /// </summary>
    public class SPEDFiscalService : ISPEDFiscalService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IConfiguracaoFiscalRepository _configuracaoFiscalRepository;
        private readonly IElectronicInvoiceRepository _electronicInvoiceRepository;

        public SPEDFiscalService(
            IClinicRepository clinicRepository,
            IConfiguracaoFiscalRepository configuracaoFiscalRepository,
            IElectronicInvoiceRepository electronicInvoiceRepository)
        {
            _clinicRepository = clinicRepository;
            _configuracaoFiscalRepository = configuracaoFiscalRepository;
            _electronicInvoiceRepository = electronicInvoiceRepository;
        }

        public async Task<string> GerarSPEDFiscalAsync(Guid clinicaId, DateTime inicio, DateTime fim, string tenantId)
        {
            var clinica = await _clinicRepository.GetByIdAsync(clinicaId, tenantId)
                ?? throw new ArgumentException("Clínica não encontrada", nameof(clinicaId));

            var configuracoesFiscais = await _configuracaoFiscalRepository.GetAllAsync(tenantId);
            var configuracaoFiscal = configuracoesFiscais.FirstOrDefault(cf => cf.ClinicaId == clinicaId);

            var sb = new StringBuilder();

            // Bloco 0: Abertura, Identificação e Referências
            GerarBloco0(sb, clinica, configuracaoFiscal, inicio, fim);

            // Bloco C: Documentos Fiscais
            await GerarBlocoCAsync(sb, clinicaId, inicio, fim, tenantId);

            // Bloco 9: Controle e Encerramento
            GerarBloco9(sb);

            return sb.ToString();
        }

        private void GerarBloco0(StringBuilder sb, Clinic clinica, ConfiguracaoFiscal? config, DateTime inicio, DateTime fim)
        {
            // Limpar CNPJ/CPF
            var documento = clinica.Document?.Replace(".", "").Replace("/", "").Replace("-", "") ?? "";
            
            // Extrair estado do endereço (últimos 2 caracteres antes do CEP, se disponível)
            // Por ora, usar valor padrão se não disponível
            var estado = "SP"; // Valor padrão - pode ser configurado posteriormente

            // |0000| - Abertura do Arquivo Digital e Identificação da Entidade
            sb.AppendLine($"|0000|013|0|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|{clinica.Name}|{documento}||||{estado}||A|1|");

            // |0001| - Abertura do Bloco 0
            sb.AppendLine("|0001|0|");

            // |0100| - Dados do Contabilista (Opcional - deixar vazio por ora)
            // sb.AppendLine("|0100|Nome Contador|CPF|CRC||Telefone||");

            // |0150| - Cadastro de Participantes
            // Por ora, incluímos apenas a própria clínica
            sb.AppendLine($"|0150|{documento}|{clinica.Name}||||||||{estado}||");

            // |0190| - Identificação das Unidades de Medida
            sb.AppendLine("|0190|UN|Unidade|");

            // |0200| - Tabela de Identificação do Item (Produto e Serviços)
            var codigoServico = config?.CodigoServico ?? "01";
            sb.AppendLine($"|0200|{codigoServico}|Serviços Médicos|||||UN||");

            // |0990| - Encerramento do Bloco 0
            var totalLinhasBloco0 = sb.ToString().Split('\n').Count(l => l.StartsWith("|0"));
            sb.AppendLine($"|0990|{totalLinhasBloco0 + 1}|");
        }

        private async Task GerarBlocoCAsync(StringBuilder sb, Guid clinicaId, DateTime inicio, DateTime fim, string tenantId)
        {
            // |C001| - Abertura do Bloco C
            sb.AppendLine("|C001|0|");

            // Buscar notas fiscais autorizadas no período
            var todasNotas = await _electronicInvoiceRepository.GetAllAsync(tenantId);
            var notas = todasNotas
                .Where(n => n.IssueDate >= inicio
                         && n.IssueDate <= fim
                         && n.Status == ElectronicInvoiceStatus.Authorized)
                .OrderBy(n => n.IssueDate)
                .ToList();

            foreach (var nota in notas)
            {
                // |C100| - Nota Fiscal (Código 21 = Nota Fiscal de Serviços de Comunicação e Telecomunicação - Modelo 21)
                // Para NFS-e usamos modelo 99 (Outros)
                var modelo = nota.Type == ElectronicInvoiceType.NFSe ? "99" : "55";
                var situacao = "00"; // Documento regular

                // Valor total é ServiceAmount + impostos
                var valorTotal = nota.ServiceAmount + nota.TotalTaxes;

                sb.AppendLine($"|C100|0|1|{nota.Number}|{modelo}|{situacao}|{nota.Series}|{nota.IssueDate:ddMMyyyy}|{nota.IssueDate:ddMMyyyy}|{valorTotal:F2}|0|0|{nota.ServiceAmount:F2}|{nota.IssAmount:F2}|");

                // |C110| - Informação Complementar da Nota Fiscal (Opcional)
                // Deixar comentado por ora, pois não há campo AdditionalInformation
                // if (!string.IsNullOrEmpty(nota.AdditionalInformation))
                // {
                //     sb.AppendLine($"|C110|{nota.AdditionalInformation}|");
                // }

                // |C170| - Itens do Documento (Nota Fiscal Eletrônica)
                var codigoServico = nota.ServiceCode ?? "01";
                sb.AppendLine($"|C170|1|{codigoServico}|{nota.ServiceDescription}|1|UN|{nota.ServiceAmount:F2}||||{nota.ServiceAmount:F2}|0|");
            }

            // |C990| - Encerramento do Bloco C
            var totalLinhasBlocoC = sb.ToString().Split('\n').Count(l => l.StartsWith("|C"));
            sb.AppendLine($"|C990|{totalLinhasBlocoC + 1}|");
        }

        private void GerarBloco9(StringBuilder sb)
        {
            // |9001| - Abertura do Bloco 9
            sb.AppendLine("|9001|0|");

            // |9900| - Registros do Arquivo
            var registros = new Dictionary<string, int>();
            var linhas = sb.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                if (linha.StartsWith("|"))
                {
                    var registro = linha.Split('|')[1];
                    if (!registros.ContainsKey(registro))
                        registros[registro] = 0;
                    registros[registro]++;
                }
            }

            foreach (var registro in registros.OrderBy(r => r.Key))
            {
                sb.AppendLine($"|9900|{registro.Key}|{registro.Value}|");
            }

            // |9990| - Encerramento do Bloco 9
            var totalLinhasBloco9 = sb.ToString().Split('\n').Count(l => l.StartsWith("|9"));
            sb.AppendLine($"|9990|{totalLinhasBloco9 + 2}|"); // +2 para incluir 9990 e 9999

            // |9999| - Encerramento do Arquivo Digital
            var totalLinhas = sb.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries).Count(l => l.StartsWith("|"));
            sb.AppendLine($"|9999|{totalLinhas + 1}|"); // +1 para incluir o próprio 9999
        }

        public async Task<SPEDValidationResult> ValidarSPEDFiscalAsync(string conteudoSPED)
        {
            var result = new SPEDValidationResult
            {
                Valido = true
            };

            var erros = new List<string>();
            var avisos = new List<string>();

            if (string.IsNullOrWhiteSpace(conteudoSPED))
            {
                result.Valido = false;
                erros.Add("Conteúdo SPED vazio");
                result.Erros = erros.ToArray();
                result.Avisos = avisos.ToArray();
                return result;
            }

            var linhas = conteudoSPED.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var registros = new Dictionary<string, int>();
            var blocos = new HashSet<string>();

            // Validação básica de estrutura
            foreach (var linha in linhas.Select((l, i) => new { Linha = l, Index = i }))
            {
                if (!linha.Linha.StartsWith("|"))
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Linha não inicia com '|'");
                    continue;
                }

                if (!linha.Linha.EndsWith("|"))
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Linha não termina com '|'");
                }

                var campos = linha.Linha.Split('|');
                if (campos.Length < 2)
                {
                    result.Valido = false;
                    erros.Add($"Linha {linha.Index + 1}: Formato inválido");
                    continue;
                }

                var registro = campos[1];
                if (!registros.ContainsKey(registro))
                    registros[registro] = 0;
                registros[registro]++;

                // Identificar blocos
                if (registro.Length >= 1)
                    blocos.Add(registro[0].ToString());
            }

            result.TotalRegistros = linhas.Length;
            result.TotalBlocos = blocos.Count;

            // Validações obrigatórias
            if (!registros.ContainsKey("0000"))
            {
                result.Valido = false;
                erros.Add("Registro 0000 (abertura do arquivo) não encontrado");
            }

            if (!registros.ContainsKey("0001"))
            {
                result.Valido = false;
                erros.Add("Registro 0001 (abertura do bloco 0) não encontrado");
            }

            if (!registros.ContainsKey("0990"))
            {
                result.Valido = false;
                erros.Add("Registro 0990 (encerramento do bloco 0) não encontrado");
            }

            if (!registros.ContainsKey("9001"))
            {
                result.Valido = false;
                erros.Add("Registro 9001 (abertura do bloco 9) não encontrado");
            }

            if (!registros.ContainsKey("9990"))
            {
                result.Valido = false;
                erros.Add("Registro 9990 (encerramento do bloco 9) não encontrado");
            }

            if (!registros.ContainsKey("9999"))
            {
                result.Valido = false;
                erros.Add("Registro 9999 (encerramento do arquivo) não encontrado");
            }

            // Avisos
            if (!registros.ContainsKey("C001"))
            {
                avisos.Add("Bloco C (Documentos Fiscais) não encontrado");
            }

            result.Erros = erros.ToArray();
            result.Avisos = avisos.ToArray();

            return result;
        }

        public async Task<string> ExportarSPEDFiscalAsync(Guid clinicaId, DateTime inicio, DateTime fim, string caminhoArquivo, string tenantId)
        {
            var conteudo = await GerarSPEDFiscalAsync(clinicaId, inicio, fim, tenantId);

            // Criar diretório se não existir
            var diretorio = Path.GetDirectoryName(caminhoArquivo);
            if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            // Salvar arquivo
            await File.WriteAllTextAsync(caminhoArquivo, conteudo, Encoding.UTF8);

            return caminhoArquivo;
        }
    }
}
